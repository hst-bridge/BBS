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

namespace Maverick.SSH
{
	/// <summary>
	/// An enumeration of SSH client states.  
	/// </summary>
	public enum SSHState 
	{
		/// <summary>
		/// The client is currently connecting.
		/// </summary>
		CONNECTING,

		/// <summary>
		/// The client is connected, but not authenticated.
		/// </summary>
		CONNECTED,

		/// <summary>
		/// The client has been authenticated.
		/// </summary>
		AUTHENTICATED,

		/// <summary>
		/// The client has disconnected.
		/// </summary>
		DISCONNECTED
	}

	/// <summary>
	/// A delegate for notification of <see cref="Maverick.SSH.SSHState"/> change events
	/// </summary>
	public delegate void SSHStateListener(SSHClient client, SSHState newstate);


	/// <summary>
	/// This interface defines the general contract for an SSH client that is compatible for 
	/// both the SSH1 and SSH2 protocols.
	/// </summary>
	/// <remarks>This provides general authentication and the opening 
	/// of sessions. Further features may be available depending upon the version of the SSH 
	/// server and installed protocol support.
	/// </remarks>
	public interface SSHClient
	{
		/// <summary>
		/// An event fired when the client state changes.
		/// </summary>
		/// <remarks>
		/// If you attempt to add a <see cref="Maverick.SSH.SSHStateListener"/> once you have aquired an instance
		/// of the <see cref="Maverick.SSH.SSHClient"/> from the <see cref="Maverick.SSH.SSHConnector"/> you will
		/// only be able to receive <see cref="Maverick.SSH.SSHState.AUTHENTICATED"/> and <see cref="Maverick.SSH.SSHState.DISCONNECTED"/>
		/// states. This is due to the connection procedure being called by the <see cref="Maverick.SSH.SSHConnector"/> before
		/// an instance of the client is returned. In order to receive <see cref="Maverick.SSH.SSHState.CONNECTING"/>
		/// and <see cref="Maverick.SSH.SSHState.CONNECTED"/> states you should pass a <see cref="Maverick.SSH.SSHStateListener"/>
        /// into the <see cref="Maverick.SSH.SSHConnector.Connect(Maverick.SSH.SSHTransport, string, bool)"/> method, this will be attached to the <see cref="Maverick.SSH.SSHClient"/>
		/// and the missing state change events will be fired as expected.
		/// </remarks>
		event SSHStateListener StateChange;

		/// <summary>
		/// Connect to an SSH server. This method is not designed to be called directly but from 
		/// an <see cref="Maverick.SSH.SSHConnector"/> instance.
		/// </summary>
		/// <param name="transport">The underlying transport mechanism</param>
		/// <param name="context">The configuration context for this connection</param>
		/// <param name="connector">The connector creating this client</param>
		/// <param name="username">The username for the connection</param>
		/// <param name="localIdentification">The identification string sent to the server</param>
		/// <param name="remoteIdentification">The identification string received from the server</param>
		/// <param name="threaded">Should this connection use an additional thread to buffer messages</param>
		void Connect(SSHTransport transport, 
			SSHContext context, 
			SSHConnector connector, 
			System.String username, 
			System.String localIdentification, 
			System.String remoteIdentification,
			bool threaded);

		/// <summary>
		/// Authenticate the user. Once connected call to authenticate the user. When a connection is made 
		/// no other operations can be performed until the user has been authenticated.
		/// </summary>
		/// <param name="auth"></param>
		/// <returns></returns>
		AuthenticationResult Authenticate(SSHAuthentication auth);

		/// <summary>
		/// Open a session on the remote computer. This can only be called once the user has been 
		/// authenticated. The session returned is uninitialized and will be opened when either a command 
		/// is executed or the users shell has been started.
		/// </summary>
		/// <returns></returns>
		SSHSession OpenSessionChannel();

		/// <summary>
		/// Open a session on the remote computer and attach a listener before the channel is opened.
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		SSHSession OpenSessionChannel(ChannelStateListener listener);

		/// <summary>
		/// Create an identical version of an <see cref="Maverick.SSH.SSHClient"/> using cached authentication 
		/// information and the <see cref="Maverick.SSH.SSHTransport.Duplicate"/> method. 
		/// </summary>
		/// <returns></returns>
		SSHClient Duplicate();

		/// <summary>
		/// Disconnects from the remote computer
		/// </summary>
		void Disconnect();

		/// <summary>
		/// Open a TCPIP forwarding channel to the remote computer. If successful the remote computer will open 
		/// a socket to the host/port specified and return a channel which can be used to forward TCPIP data 
		/// from the local computer to the remotley connected socket.
		/// 
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
		SSHTunnel OpenForwardingChannel(String hostname, 
			int port, 
			String listeningAddress, 
			int listeningPort, 
			String originatingHost, 
			int originatingPort, 
			Object transport);


		/// <summary>
		/// Open a TCPIP forwarding channel to the remote computer. If successful the remote computer will open 
		/// a socket to the host/port specified and return a channel which can be used to forward TCPIP data 
		/// from the local computer to the remotley connected socket.
		/// 
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
		SSHTunnel OpenForwardingChannel(System.String hostname, 
			int port, 
			System.String listeningAddress, 
			int listeningPort, 
			System.String originatingHost, 
			int originatingPort, 
			Object transport,
			ChannelStateListener listener);

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
		bool RequestRemoteForwarding(String bindAddress, 
			int bindPort,
			String hostToConnect,
			int portToConnect,
			ForwardingRequestListener listener);


		/// <summary>
		/// Requests that the remote computer cancel an existing remote forwarding request.
		/// </summary>
		/// <param name="bindAddress"></param>
		/// <param name="bindPort"></param>
		/// <returns></returns>
		bool CancelRemoteForwarding(String bindAddress,
			int bindPort);


		/// <summary>
		/// Returns true if the client has been authenticated, otherwise false.
		/// </summary>
		bool IsAuthenticated
		{
			get;
				
		}

		/// <summary>
		/// Returns true if the client is buffered, otherwise false.
		/// </summary>
		bool IsBuffered
		{
			get;
		}

		/// <summary>
		/// Returns true if the client is connected, otherwise false.
		/// </summary>
		bool IsConnected
		{
			get;
				
		}

		/// <summary>
		/// Returns the identification string sent by the remote SSH server.
		/// </summary>
		System.String RemoteIdentification
		{
			get;
				
		}

		/// <summary>
		/// Returns the identification string sent by the API to the remote SSH server.
		/// </summary>
		System.String LocalIdentification
		{
			get;
		}

		/// <summary>
		/// Returns the username used to create the initial connection.
		/// </summary>
		System.String Username
		{
			get;
				
		}

		/// <summary>
		/// Returns the configuration context used to connect this client.
		/// </summary>
		SSHContext Context
		{
			get;
				
		}

		/// <summary>
		/// Returns the major version number of the SSH protocol that the client implements
		/// </summary>
		int Version
		{
			get;
		}

	}
}
