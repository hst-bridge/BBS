/**
 * Copyright (c) 2003-2013 SSHTOOLS LIMITED. All Rights Reserved.
 *
 * This file contains Original Code and/or Modifications of Original Code and
 * its use is subject to the terms of the GNU Public License v3.0. You may not use
 * this file except in compliance with the license terms.
 *
 * You should have received a copy of the GNU Public License v3.0 along with this
 * software; see the file LICENSE.html.  If not, write to or contact:
 *
 * SSHTOOLS, PO BOX 9700, Langar, Nottinghamshire. NG13 9WE
 *
 * Email:     support@sshtools.com
 * 
 * WWW:       http://www.sshtools.com
 *****************************************************************************/
using System;
using System.Collections;
using Maverick.SSH;
using Maverick.SSH.Packets;
using Maverick.Crypto.IO;

namespace Maverick.SSH2
{
	/// <summary>
	/// Implementation of an <see cref="Maverick.SSH.SSHClient"/> for the SSH2 protocol.
	/// </summary>
	/// <remarks>
	/// This provides the ability to create custom channels and sending/receiving of global requests in addition to the standard 
	/// contract.
	/// </remarks>
	public class SSH2Client : SSHClient
	{
		SSH2Context context;
		String username;
		String localIdentification;
		String remoteIdentification;

		TransportProtocol transport;
		AuthenticationProtocol authentication;
		ConnectionProtocol connection;

		SSHTransport io;
		SSHAuthentication auth;
		SSHConnector con;

		String[] authenticationMethods;
		bool isXForwarding = false;
		bool threaded = false;

		Hashtable forwardingListeners;
		Hashtable forwardingDestinations;
		ForwardingRequestChannelFactory requestFactory;

		/// <summary>
		/// Create an uninitialized client
		/// </summary>
		public SSH2Client()
		{
			this.forwardingDestinations = new Hashtable();
			this.forwardingListeners = new Hashtable();
			this.requestFactory = new ForwardingRequestChannelFactory(this);

		}

		/// <summary>
		/// An event enabling listeners to receive notification of SSH client state change
		/// events.
		/// </summary>
		public event SSHStateListener StateChange;

		internal void OnTransportEvent(TransportProtocol transport, TransportProtocolState state)
		{
			switch(state)
			{
				case TransportProtocolState.DISCONNECTED:
				{
					FireEvent(SSHState.DISCONNECTED);
					break;
				}
				case TransportProtocolState.PERFORMING_KEYEXCHANGE:
				{
					/*FireEvent(SSHState.PERFORMING_KEYEXCHANGE);*/
					break;
				}
			}

		}

		/// <summary>
		/// Connect to an SSH server. This method is not designed to be called directly but from 
		/// an <see cref="Maverick.SSH.SSHConnector"/> instance.
		/// </summary>
		/// <param name="io">The underlying transport mechanism</param>
		/// <param name="context">The configuration context for this connection</param>
		/// <param name="connector">The connector creating this client</param>
		/// <param name="username">The username for the connection</param>
		/// <param name="localIdentification">The identification string sent to the server</param>
		/// <param name="remoteIdentification">The identification string received from the server</param>
		/// <param name="threaded">Should this connection use an additional thread to buffer messages</param>
		public void Connect(SSHTransport io, 
			SSHContext context, 
			SSHConnector connector, 
			System.String username, 
			System.String localIdentification, 
			System.String remoteIdentification,
			bool threaded)
		{

			FireEvent(SSHState.CONNECTING);
			
			this.context = (SSH2Context) context;
			this.con = connector;
			this.io = io;
			this.threaded = threaded;
			this.username = username;
			this.localIdentification = localIdentification;
			this.remoteIdentification = remoteIdentification;
			this.transport = new TransportProtocol(io,
				this);
			
			this.transport.StateChange += new TransportProtocolStateChangeEvent(this.OnTransportEvent);

			this.transport.StartProtocol();

			FireEvent(SSHState.CONNECTED);

			this.connection = new ConnectionProtocol(transport, threaded);
			this.connection.AddChannelFactory(requestFactory);
			this.authentication = new AuthenticationProtocol(transport, connection);

			RequestAuthenticationMethods();

		}

		/**
		 * this method is called if a user attempts password authentication
		 * it determines whether password authentication is possible.  
		 * if it isnt, but keyboard interactive is possible, it authenticates using that instead
		 */
		private SSHAuthentication checkForPasswordOverKBI(SSHAuthentication auth) 
		{
			bool kbiAuthenticationPossible=false;
			for(int i=0;i<authenticationMethods.Length;i++) 
			{
				if(authenticationMethods[i].Equals("password")) 
				{   			
					//password authentication is possible so return auth unchanged
					return auth;
				}
				else 
				{
					if((authenticationMethods[i].Equals("keyboard-interactive"))) 
					{
						//if none of the subsequent methods are password then have option to use kbi instead
						kbiAuthenticationPossible=true;
					}
				}
			}
			//password is not possible, so attempt kbi
			if(kbiAuthenticationPossible) 
			{
				//create KBIAuthentication instance
				KBIAuthentication kbi=new KBIAuthentication();
				//set the username that the user entered
				kbi.Username=( (PasswordAuthentication) auth).Username;
		
				//set request handler, that sets the password the user entered as response to any prompts
				KBIRequestHandlerWhenUserUsingPasswordAuthentication handler=new KBIRequestHandlerWhenUserUsingPasswordAuthentication((PasswordAuthentication) auth); 				
				kbi.InteractivePrompt+=new ShowAuthenticationPrompts(handler.showPrompts);

				return kbi;
			}
			//neither password nor kbi is possible so return auth unchanged so that the normal error message is returned
			return auth;
		}  

		/**<p>Request handler that sets the password the user entered as response to any prompts</p>
		*
		*@author David Hodgins
		*/
		private class KBIRequestHandlerWhenUserUsingPasswordAuthentication
		{
			private String password;
			public KBIRequestHandlerWhenUserUsingPasswordAuthentication(PasswordAuthentication pwdAuth) 
			{
				password=pwdAuth.Password;
			}
		
			/**
			* Called by the <em>keyboard-interactive</em> authentication mechanism when
			* the server requests information from the user. Each prompt should be displayed
			* to the user with their response recorded within the prompt object.
			*
			* @param name
			* @param instruction
			* @param prompts
			*/
			public bool showPrompts(String name, String instruction, KBIPrompt[] prompts) 
			{
				for(int i=0;i<prompts.Length;i++) 
				{
					prompts[i].Response=password;
				}
				return true;
			}
		  
		}

		/// <summary>
		/// Authenticate the user. Once connected call to authenticate the user. When a connection is made 
		/// no other operations can be performed until the user has been authenticated.
		/// </summary>
		/// <param name="auth"></param>
		/// <returns></returns>
		public AuthenticationResult Authenticate(SSHAuthentication auth)
		{
			VerifyConnection(false);
	
			if (auth.Username == null) 
			{
				auth.Username = username;
			}

			//if authentication method is Password authentication then check if password is available else attempt kbi if its available
			if (auth is PasswordAuthentication || auth is SSH2PasswordAuthentication) {
    			auth=checkForPasswordOverKBI(auth);
			}

			AuthenticationResult result = AuthenticationResult.FAILED;

			if((auth is PasswordAuthentication) 
				&& !(auth is SSH2PasswordAuthentication))
			{

				SSH2PasswordAuthentication pwd = new SSH2PasswordAuthentication();
				pwd.Username = auth.Username;
				pwd.Password = ((PasswordAuthentication)auth).Password;

				result = authentication.Authenticate(pwd,
					ConnectionProtocol.SERVICE_NAME);
			}
			else if(auth is SSH2AuthenticationClient)
			{
				result = authentication.Authenticate((SSH2AuthenticationClient)auth,
					ConnectionProtocol.SERVICE_NAME);
			}
			else if(auth is PublicKeyAuthentication
				&& !(auth is SSH2PublicKeyAuthentication))
			{
				SSH2PublicKeyAuthentication pk = new SSH2PublicKeyAuthentication(((PublicKeyAuthentication)auth).KeyPair);
				pk.Username = ((PublicKeyAuthentication)auth).Username;
				pk.VerifyOnly = ((PublicKeyAuthentication)auth).VerifyOnly;
			
				result = authentication.Authenticate(pk,
					ConnectionProtocol.SERVICE_NAME);

			}
			else if(auth is SSH2PublicKeyAuthentication)
			{
				result = authentication.Authenticate((SSH2AuthenticationClient)auth,
					ConnectionProtocol.SERVICE_NAME);
			}
			else				   
			{
				throw new SSHException("Invalid authentication client",
                             SSHException.BAD_API_USAGE);
			}

			if(result == AuthenticationResult.COMPLETE)
			{
				FireEvent(SSHState.AUTHENTICATED);
				this.auth  = auth;
			}

			return result;
		}

		private void FireEvent(SSHState state)
		{
			try
			{
				if(StateChange!=null)
					StateChange(this, state);
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine("WARNING! Exception caught in Maverick.NET event handler: " + ex.Message );
				System.Diagnostics.Trace.WriteLine(ex.StackTrace );
			}
		}

		/// <summary>
		/// Get a list of authentication schemes that are available to the user.
		/// </summary>
		/// <returns></returns>
		public String[] RequestAuthenticationMethods()
		{

			VerifyConnection(false);
			if (authenticationMethods == null)
			{
				
				String methods = authentication.RequestAuthenticationMethods(username, 
					ConnectionProtocol.SERVICE_NAME);
				ArrayList tmp = new ArrayList();
				int idx;
				while (methods != null)
				{
					idx = methods.IndexOf((System.Char) ',');
					if (idx > - 1)
					{
						tmp.Add(methods.Substring(0, (idx) - (0)));
						methods = methods.Substring(idx + 1);
					}
					else
					{
						tmp.Add(methods);
						methods = null;
					}
				}
				authenticationMethods = new System.String[tmp.Count];
				tmp.CopyTo(authenticationMethods);
			}
			
			return authenticationMethods;


		}

		/// <summary>
		/// Force the keys to be exchanged.
		/// </summary>
		public void ForceKeyExchange()
		{
			transport.SendKeyExchangeInit();
		}

		/// <summary>
		/// Send a global request
		/// </summary>
		/// <param name="request"></param>
		/// <param name="wantreply"></param>
		/// <returns></returns>
		public bool SendGlobalRequest(GlobalRequest request, bool wantreply)
		{
			VerifyConnection(true);
			return connection.SendGlobalRequest(request, wantreply);
		}

		/// <summary>
		/// Add a channel factory.
		/// </summary>
		/// <param name="factory"></param>
		public void  AddChannelFactory(ChannelFactory factory)
		{
			connection.AddChannelFactory(factory);
		}
		
		/// <summary>
		/// Add a global request handler
		/// </summary>
		/// <param name="handler"></param>
		public void  AddRequestHandler(GlobalRequestHandler handler)
		{
			connection.AddRequestHandler(handler);
		}

		internal void VerifyConnection(bool requireAuthentication)
		{
			if (authentication == null || transport == null || connection == null) 
			{
				throw new SSHException("Not connected!",
					SSHException.BAD_API_USAGE);
			}
			
			if (!transport.IsConnected) 
			{
				throw new SSHException("The connection has been terminated!",
						SSHException.REMOTE_HOST_DISCONNECTED);
			}
			
			if (!authentication.IsAuthenticated && requireAuthentication) 
			{
				throw new SSHException("The connection is not authenticated!",
				SSHException.BAD_API_USAGE);
			}

		}

		/// <summary>
		/// Open a TCPIP forwarding channel to the remote computer. If successful the remote computer will open 
		/// a socket to the host/port specified and return a channel which can be used to forward TCPIP data 
		/// from the local computer to the remotley connected socket.<br/>
		/// <br/>
		///	It should be noted that this is a low level API method and it does not connect the transport to the 
		///	channel. Future versions of this API will provide a ForwardingClient to automate transfer between
		///	the tunnel and a local listening Socket.
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <param name="listeningAddress"></param>
		/// <param name="listeningPort"></param>
		/// <param name="originatingHost"></param>
		/// <param name="originatingPort"></param>
		/// <param name="transport"></param>
		/// <returns></returns>
		public SSHTunnel OpenForwardingChannel(System.String hostname, 
			int port, 
			System.String listeningAddress, 
			int listeningPort, 
			System.String originatingHost, 
			int originatingPort, 
			Object transport)
		{
			return OpenForwardingChannel(hostname,
				port,
				listeningAddress,
				listeningPort,
				originatingHost,
				originatingPort,
				transport,
				null);
		}

		/// <summary>
		/// Open a TCPIP forwarding channel to the remote computer. If successful the remote computer will open 
		/// a socket to the host/port specified and return a channel which can be used to forward TCPIP data 
		/// from the local computer to the remotley connected socket.<br/>
		/// <br/>
		///	It should be noted that this is a low level API method and it does not connect the transport to the 
		///	channel. Future versions of this API will provide a ForwardingClient to automate transfer between
		///	the tunnel and a local listening Socket.
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <param name="listeningAddress"></param>
		/// <param name="listeningPort"></param>
		/// <param name="originatingHost"></param>
		/// <param name="originatingPort"></param>
		/// <param name="transport"></param>
		/// <param name="listener"></param>
		/// <returns></returns>
		public SSHTunnel OpenForwardingChannel(System.String hostname, 
			int port, 
			System.String listeningAddress, 
			int listeningPort, 
			System.String originatingHost, 
			int originatingPort, 
			Object transport,
			ChannelStateListener listener)
		{
			
				try
				{
					SSH2ForwardingChannel tunnel = new SSH2ForwardingChannel(
						SSH2ForwardingChannel.LOCAL_FORWARDING_CHANNEL, 
						131072, 
						32768, 
						hostname, 
						port, 
						listeningAddress, 
						listeningPort, 
						originatingHost, 
						originatingPort, 
						transport,
						true);
				
					ByteBuffer request = new ByteBuffer();
					request.WriteString(hostname);
					request.WriteUINT32(port);
					request.WriteString(originatingHost);
					request.WriteUINT32(originatingPort);
				
					if(listener!=null)
						tunnel.StateChange+=listener;

					OpenChannel(tunnel, request.ToByteArray());

					tunnel.ResumeWindow();

					return tunnel;
				}
				catch (System.IO.IOException ex)
				{
					throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
				}
			
		}

		/// <summary>
		/// Requests that the remote computer cancel an existing remote forwarding request.
		/// </summary>
		/// <param name="bindAddress"></param>
		/// <param name="bindPort"></param>
		/// <returns></returns>
		public bool CancelRemoteForwarding(String bindAddress,
			int bindPort)
		{
			
			ByteBuffer baw = new ByteBuffer();
			baw.WriteString(bindAddress);
			baw.WriteInt(bindPort);

			GlobalRequest request = new GlobalRequest("cancel-tcpip-forward",
				baw.ToByteArray());

			if(SendGlobalRequest(request, true)) 
			{
				forwardingListeners.Remove(bindAddress + ":" + bindPort);
				forwardingDestinations.Remove(bindAddress + ":" + bindPort);
				return true;
			}
			else
				return false;


		}

		
		/// <summary>
		/// Requests that the remote computer accepts socket connections and forward them to the local 
		/// computer. The <see cref="Maverick.SSH.ForwardingRequestListener"/> provides callback methods to create 
		/// the connections and to initialize the tunnel.
		/// </summary>
		/// <param name="bindAddress"></param>
		/// <param name="bindPort"></param>
		/// <param name="hostToConnect"></param>
		/// <param name="portToConnect"></param>
		/// <param name="listener"></param>
		/// <returns></returns>
		public bool RequestRemoteForwarding(String bindAddress, 
			int bindPort,
			String hostToConnect,
			int portToConnect,
			ForwardingRequestListener listener)  
		{
				if(listener == null) 
				{
					throw new SSHException(
							"You must specify a listener to receive connection requests",
								SSHException.BAD_API_USAGE);
				}

				ByteBuffer baw = new ByteBuffer();
				baw.WriteString(bindAddress);
				baw.WriteInt(bindPort);
				GlobalRequest request = new GlobalRequest("tcpip-forward",
													baw.ToByteArray());

				if(SendGlobalRequest(request, true)) 
				{

					forwardingListeners.Add((bindAddress + ":" + bindPort),
												listener);
					forwardingDestinations.Add((bindAddress + ":" + bindPort),
									(hostToConnect + ":" + portToConnect));
					// Setup the forwarding listener
					return true;
				}
				else 
				{
					return false;
				}
		}

		/// <summary>
		/// Open a session channel
		/// </summary>
		/// <returns></returns>
		public SSHSession OpenSessionChannel()
		{
			return OpenSessionChannel(null);
		}

		/// <summary>
		/// Open a session channel and attach the given state listening delegate.
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		public SSHSession OpenSessionChannel(ChannelStateListener listener)
		{
			VerifyConnection(true);

			return OpenSessionChannel(32768, 32768, listener);
		}


		/// <summary>
		/// Open a session channel and configure the required parameters.
		/// </summary>
		/// <param name="windowsize"></param>
		/// <param name="packetlen"></param>
		/// <param name="listener"></param>
		/// <returns></returns>
		public SSHSession OpenSessionChannel(int windowsize, int packetlen, ChannelStateListener listener)
		{
			VerifyConnection(true);

			SSH2Session session = new SSH2Session(windowsize, packetlen, this);

			session.StateChange+=listener;

			connection.OpenChannel(session, null);

			if (context.X11Display != null)
			{
				
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Requesting X Forwarding for " + context.X11Display );
#endif
				System.String display = context.X11Display;

				String hostname = "localhost";
				String tmp;
				int screen = 0;

				int idx = display.IndexOf(':');
				if(idx!=-1) 
				{
					hostname = display.Substring(0, idx);
					tmp = display.Substring(idx+1);
				} 
				else
					tmp = display;

				idx = tmp.IndexOf(".");
				if(idx!=-1)
				{
					screen = Int32.Parse(tmp.Substring(idx+1));
					tmp = tmp.Substring(0, idx);
				}

				
				byte[] x11FakeCookie = context.X11AuthenticationCookie;
				System.Text.StringBuilder cookieBuf = new System.Text.StringBuilder();
				for (int i = 0; i < 16; i++)
				{
					System.String b = System.Convert.ToString(x11FakeCookie[i], 16);
					if (b.Length == 1)
					{
						b = "0" + b;
					}
					cookieBuf.Append(b);
				}
				
				if (session.RequestX11Forwarding(false, "MIT-MAGIC-COOKIE-1", cookieBuf.ToString(), screen))
				{
					isXForwarding = true;
				}
			}
			return session;
		}

		/// <summary>
		/// Determines if the client is using a threaded model to buffer messages
		/// </summary>
		public bool IsBuffered
		{
			get
			{
				return threaded;
			}
		}

		/// <summary>
		/// Creates an identical copy of the client using cached credentials.
		/// </summary>
		/// <returns></returns>
		public SSHClient Duplicate()
		{
			// Set the supported version to this protocol
			con.SupportedVersions = SSHConnector.SSH2;
			try
			{
				SSHClient duplicate = con.Connect(io.Duplicate(), username, IsBuffered, context, this.StateChange);

                if (duplicate.IsAuthenticated)
                    return duplicate;

                if ((username == null || auth == null))
                {
                    duplicate.Disconnect();

                    throw new SSHException("Cannot duplicate! The existing connection does not have a set of credentials",
                        SSHException.BAD_API_USAGE);
                }

				if (duplicate.Authenticate(auth) != AuthenticationResult.COMPLETE)
				{
					duplicate.Disconnect();
					throw new SSHException("Duplication attempt failed to authenicate user!", 
						SSHException.INTERNAL_ERROR);
				}
				
				return duplicate;
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.CONNECT_FAILED);
			}
		}


		/// <summary>
		/// Open a channel.
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="requestdata"></param>
		public void OpenChannel(SSH2Channel channel, byte[] requestdata)
		{
			VerifyConnection(true);
			connection.OpenChannel(channel, requestdata);
		}
		
		/// <summary>
		/// Open a channel.
		/// </summary>
		/// <param name="channel"></param>
		public void OpenChannel(SSHAbstractChannel channel)
		{
			VerifyConnection(true);

			if (channel is SSH2Channel)
			{
				connection.OpenChannel((SSH2Channel) channel, null);
			}
			else
			{
				throw new SSHException("The channel is not an SSH2 channel!", SSHException.BAD_API_USAGE);
			}
		}


		/// <summary>
		/// Disconnect the connection.
		/// </summary>
		public void Disconnect() 
		{
			connection.SignalClosingState();
			transport.Disconnect("User has disconnected",
				DisconnectionReason.BY_APPLICATION);
		}

		/// <summary>
		/// Determine if this instance has been authenticated
		/// </summary>
		public bool IsAuthenticated
		{
			get 
			{
				return authentication.IsAuthenticated;	
			}
				
		}

		/// <summary>
		/// Determines if this instance is connected.
		/// </summary>
		public bool IsConnected
		{
			get 
			{
				return transport.IsConnected;
			}
				
		}

		/// <summary>
		/// Returns the identification string sent by the server during protocol negotiation
		/// </summary>
		public System.String RemoteIdentification
		{
			get
			{
				return remoteIdentification;
			}		
		}

		/// <summary>
		/// Returns the identification string sent by the client during protocol negotiation.
		/// </summary>
		public System.String LocalIdentification
		{
			get
			{
				return localIdentification;
			}
				
		}

		/// <summary>
		/// Returns the name of the current user.
		/// </summary>
		public System.String Username
		{
			get
			{
				return username;
			}
				
		}

		/// <summary>
		/// Returns the configuration context in use.
		/// </summary>
		public SSHContext Context
		{
			get
			{
				return context;
			}
		}

		/// <summary>
		/// Returns the major version number of the SSH protocol that the client implements
		/// </summary>
		public int Version
		{
			get
			{
				return 2;
			}
		}

		internal class ForwardingRequestChannelFactory : ChannelFactory
		{
			public ForwardingRequestChannelFactory(SSH2Client enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}

			private void  InitBlock(SSH2Client enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
				types = new System.String[]{ SSH2ForwardingChannel.REMOTE_FORWARDING_CHANNEL, "x11"};
			}

			internal SSH2Client enclosingInstance;
			internal System.String[] types;
			
			public System.String[] SupportedChannelTypes
			{
				get
				{
					return types;
				}
			}
			
			public SSH2Channel CreateChannel(String channeltype, byte[] requestdata)
			{
				
				if (channeltype.Equals(SSH2ForwardingChannel.REMOTE_FORWARDING_CHANNEL))
				{
					
					try
					{
						ByteBuffer bar = new ByteBuffer(requestdata);
						System.String address = bar.ReadString();
						int port = (int) bar.ReadUINT32();
						System.String originatorIP = bar.ReadString();
						int originatorPort = (int) bar.ReadUINT32();
						
						System.String key = address + ":" + port.ToString();
						if (enclosingInstance.forwardingListeners.ContainsKey(key))
						{
							ForwardingRequestListener listener = (ForwardingRequestListener) enclosingInstance.forwardingListeners[key];
							String destination = (System.String) enclosingInstance.forwardingDestinations[key];
							String hostToConnect = destination.Substring(0, (destination.IndexOf((System.Char) ':')) - (0));
							int portToConnect = System.Int32.Parse(destination.Substring(destination.IndexOf((System.Char) ':') + 1));
							
							SSH2ForwardingChannel channel = new SSH2ForwardingChannel(SSH2ForwardingChannel.REMOTE_FORWARDING_CHANNEL, 
								32768, 
								32768,
								hostToConnect, 
								portToConnect,
								address,
								port, 
								originatorIP, 
								originatorPort,
								listener.CreateConnection(hostToConnect, portToConnect, address, port),
								false);
							
							listener.InitializeTunnel(channel);
							
							return channel;
						}
						else
						{
							throw new ChannelOpenException("Forwarding had not previously been requested", 
								ChannelOpenException.ADMINISTRATIVIVELY_PROHIBITED);
						}
					}
					catch (System.IO.IOException ex)
					{
						throw new ChannelOpenException(ex.Message, ChannelOpenException.RESOURCE_SHORTAGE);
					}
					catch (SSHException ex)
					{
						throw new ChannelOpenException(ex.Message, ChannelOpenException.CONNECT_FAILED);
					}
				}
				else if (channeltype.Equals("x11"))
				{
					
					if (!enclosingInstance.isXForwarding)
						throw new ChannelOpenException("X Forwarding had not previously been requested", 
							ChannelOpenException.ADMINISTRATIVIVELY_PROHIBITED);
					
					try
					{
						ByteBuffer bar = new ByteBuffer(requestdata);
						
						System.String originatorIP = bar.ReadString();
						int originatorPort = (int) bar.ReadUINT32();
						
						System.String display = enclosingInstance.connection.Context.X11Display;
						
						int i = display.IndexOf(":");
						System.String targetAddr;
						int targetPort;
						int displ = 0;
						int screen = 0;
						String tmp;

						if (i != - 1)
						{
							targetAddr = display.Substring(0, (i) - (0));
							tmp = display.Substring(i+1);
						}
						else
						{
							targetAddr = "localhost";
							tmp = display;
							
						}

						i = tmp.IndexOf(".");
						if(i!=-1)
						{
							displ = Int32.Parse(tmp.Substring(0, i));
							screen = Int32.Parse(tmp.Substring(i+1));
						}
						else
							displ = Int32.Parse(tmp);

						targetPort = displ;
						
						if (targetPort < 64)
						{
							targetPort += 6000;
						}
						
						
						ForwardingRequestListener listener = enclosingInstance.connection.Context.X11RequestListener;
						
						SSH2ForwardingChannel channel = new SSH2ForwardingChannel(SSH2ForwardingChannel.X11_FORWARDING_CHANNEL, 
							32768, 
							32768, 
							targetAddr, 
							targetPort,
							targetAddr, 
							6000 + displ, 
							originatorIP, 
							originatorPort, 
							listener.CreateConnection(targetAddr, targetPort, originatorIP, originatorPort),
							false);
						
						listener.InitializeTunnel(channel);
						
						return channel;
					}
					catch (System.Exception ex)
					{
						throw new ChannelOpenException(ex.Message, ChannelOpenException.CONNECT_FAILED);
					}
				}
				
				throw new ChannelOpenException(channeltype + " is not supported", ChannelOpenException.UNKNOWN_CHANNEL_TYPE);
			}
		}
	}
}
