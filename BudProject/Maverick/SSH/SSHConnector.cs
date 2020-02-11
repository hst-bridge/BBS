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
using System.IO;
using System.Reflection;
using Maverick.SSH2;
using Maverick.Crypto.Util;

namespace Maverick.SSH
{
	/// <summary>
	/// This utility class establishes a connection with an SSH server, determines which SSH protocol 
	/// versions are supported and creates an initialized connection ready for authentication.
	/// </summary>
	/// <remarks>
	/// Each instance of this class maintains a set of configurations and settings for the connection of 
	/// <see cref="Maverick.SSH.SSHClient"/>'s. The <see cref="Maverick.SSH.SSHContext"/> interface 
	/// describes the common options but there are many more options available for the individual protocols. If
	/// you want to set advanced options take a look at the <see cref="Maverick.SSH1.SSH1Context"/> and
	/// <see cref="Maverick.SSH2.SSH2Context"/> documentation.<br/>
	/// <br/>
	/// To connect to an SSH server you need to provide an <see cref="Maverick.SSH.SSHTransport"/> which 
	/// provides the transport layer communication. In most cases this will be a Socket however this API is 
	/// designed so that connections can be made through any communication medium supported by the platform 
	/// providing that a suitable wrapper <see cref="Maverick.SSH.SSHTransport"/> can be created. The
	/// <see cref="Maverick.SSH.TcpClientTransport"/> class provides a suitable Socket implementation.<br/>
	/// <br/>
	/// An instance of this class can be used to create as many client connections as required. There is
	/// no need to create a connector for every single client instance required. 
	/// <example>
	/// <code>
	/// // Create a connection
	/// SSHConnector con = SSHConnector.Create();
	/// 
	/// // Configure the connector before creating clients so that the same configuration
	/// // can be used for multiple client instances
	/// con.KnownHosts = new ConsoleKnownHostsKeyVerification();
	/// 
	/// // Create an SSH client
	/// SSHClient ssh = con.Connect(new TcpClientTransport("my.domain.com, 22),
	///									"root", true);
	///	
	///	// Prepare the authentication request								
	///	PasswordAuthentication pwd = new PasswordAuthentication();
	///	pwd.Password = "*********";
	///	
	///	// Authenticate the user
	///	if(ssh.Authenticate(pwd)==AuthenticationResult.COMPLETE)
	///	{
	///		// Open up a session and do something..
	///		SSHSession session = ssh.OpenSessionChannel();
	///		session.RequestPseudoTerminal("vt100", 80, 24, 0, 0);
	///		session.ExecuteCommand("find / -name 'httpd.conf'");
	///		
	///		// Do something with the returned data
	///     System.IO.TextReader reader = new System.IO.StreamReader(session.GetStream());
	///     String line;
	///     while((line = reader.ReadLine()) != null)
	///        System.Console.WriteLine(line);
	///        
	///		session.Close():
	///	}
	///	
	///	ssh.Disconnect();
	///	</code>
	/// </example>
	/// </remarks>
	public sealed class SSHConnector
	{
		/// <summary>
		/// SSH1 support flag
        /// </summary>
        public const int SSH1 = 0x01;

		/// <summary>
		/// SSH2 support flag
		/// </summary>
		public const int SSH2 = 0x02;

		internal int supportedVersions = 0;
		internal String softwareComments = "Maverick.NET";

		internal SSHContext ssh2Context;

		internal SSHConnector()
		{
			
			try
			{
				ssh2Context = new SSH2Context();
				if(ssh2Context!=null)
					supportedVersions |= SSH2;
			}
			catch
			{
			}
		}


		/// <summary>
		/// Set the <see cref="Maverick.SSH.HostKeyVerification"/> instance for this connector. The verification
		/// object will be used for both SSH1 and SSH2 protocols. If you want to set different verification instances 
		/// for each protocol this can also be acheived by getting each context seperately and configuring it with the
		/// required verification object.
		/// </summary>
		public HostKeyVerification KnownHosts
		{
			set
			{


				if((supportedVersions & SSH2) != 0 && ssh2Context != null)
					ssh2Context.KnownHosts = value;
			}

		}

		/// <summary>
		/// Contains a bit mask of the supported versions. You can use this property to restrict the 
		/// connector to an individual protocol.
		/// </summary>
		/// <example>
		/// <code>
		/// SSHConnector con = SSHConnector.Create();
		/// 
		/// // Force the use of the SSH1 protocol
		/// con.SupportedVersions = SSHConnector.SSH1;
		/// 
		/// // Force the use of the SSH2 protocol
		/// con.SupportedVersions = SSHConnector.SSH2;
		/// 
		/// // Allow the use of both SSH1 and SSH2 (default)
		/// con.SupportedVersions = SSHConnector.SSH1 | SSHConnector.SSH2;
		/// </code>
		/// </example>
		public int SupportedVersions
		{
			get
			{
				return supportedVersions;
			}

			set
			{

				
				if ((value & SSH2) != 0 && ssh2Context == null)
				{
					throw new SSHException("SSH2 protocol support is not installed!", 
						SSHException.BAD_API_USAGE);
				}
				
				if ((value & SSH1) == 0 && (value & SSH2) == 0)
				{
					throw new SSHException("You must specify at least one supported version of the SSH protocol!", 
						SSHException.BAD_API_USAGE);
				}
				
				this.supportedVersions = value;
			}
		}

		/// <summary>
		/// Create an instance of the connector.
		/// </summary>
		/// <returns></returns>
		public static SSHConnector Create() 
		{
								return new SSHConnector();
		}

		/// <summary>
		/// Create a new connection to an SSH server. This method takes a newly created <see cref="Maverick.SSH.SSHTransport"/>
		/// instance and performs the initial protocol negotiation to determine which type of client to 
		/// create.
		/// </summary>
		/// <param name="transport">The transport layer to use</param>
		/// <param name="username">The name of the user making the connection</param>
		/// <param name="threaded">Should the client buffer input to ensure the smooth running of events. If you do not buffer input 
		/// and require the use of state or data events they may not operate correctly as the API will operate in single threaded
		/// mode.</param>
		/// <returns></returns>
		public SSHClient Connect(SSHTransport transport,
			String username, bool threaded) 
		{
			return Connect(transport, username, threaded, null, null);
		}

		/// <summary>
		/// Create a new connection to an SSH server. This method takes a newly created <see cref="Maverick.SSH.SSHTransport"/>
		/// instance and performs the initial protocol negotiation to determine which type of client to 
		/// create.
		/// </summary>
		/// <param name="transport">The transport layer to use</param>
		/// <param name="username">The name of the user making the connection</param>
		/// <param name="threaded">Should the client buffer input to ensure the smooth running of events. If you do not buffer input 
		/// and require the use of state or data events they may not operate correctly as the API will operate in single threaded
		/// mode.</param>
		/// <param name="events">A delegate state listener for receiving state change events</param>
		/// <returns></returns>
		public SSHClient Connect(SSHTransport transport, 
			System.String username, bool threaded, SSHStateListener events)
		{
			return Connect(transport, username, threaded, null, events);
		}

		/// <summary>
		/// Create a new connection to an SSH server. This method takes a newly created <see cref="Maverick.SSH.SSHTransport"/>
		/// instance and performs the initial protocol negotiation to determine which type of client to 
		/// create.
		/// </summary>
		/// <param name="transport">The transport layer to use</param>
		/// <param name="username">The name of the user making the connection</param>
		/// <param name="threaded">Should the client buffer input to ensure the smooth running of events. If you do not buffer input 
		/// and require the use of state or data events they may not operate correctly as the API will operate in single threaded
		/// mode.</param>
		/// <param name="context">The configuration context to use for the connection</param>
		/// <param name="events">A delegate state listener for receiving state change events</param>
		/// <returns></returns>
		internal SSHClient Connect(SSHTransport transport, 
			System.String username, bool threaded, SSHContext context, SSHStateListener events)
		{
			System.String localIdentification;
			System.String remoteIdentification = GetRemoteIdentification(transport);
			// Lets try SSH2 first because its a better protocol
			SSHClient client;
			
			System.Exception lastError = null;
			bool pointOfNoReturn = false;
			
			Stream stream = transport.GetStream();

			if ((ssh2Context != null || (context != null 
				&& context is SSH2Context)) 
				&& (supportedVersions & SSH2) != 0)
			{
				//			Send our identification depending upon which versions we can support
				if ((SelectVersion(remoteIdentification) & SSH2) != 0)
				{
					try
					{
						if(context==null)
							context = ssh2Context;
						
						client = (SSHClient) new SSH2Client();
						localIdentification = "SSH-2.0-" + softwareComments + "\n";
						pointOfNoReturn = true;
						byte[] tempArray = SupportClass.ToByteArray(localIdentification);
						stream.Write(tempArray, 0, tempArray.Length);
						
						client.StateChange += events;
						// Now get the client to connect using the selected protocol
						client.Connect(transport,
							context,
							this,
							username,
							localIdentification.Trim(),
							remoteIdentification,
							threaded);
						return client;
					}
					catch (System.Exception t)
					{
						lastError = t;
					}
					finally
					{
						if (lastError != null && pointOfNoReturn)
						{
							if (lastError is SSHException)
								throw (SSHException) lastError;
							else
							{
								throw new SSHException(lastError.Message != null?lastError.Message:lastError.GetType().FullName, SSHException.CONNECT_FAILED);
							}
						}
					}
				}
			}




			
			try
			{
				transport.Close();
			}
			catch (System.IO.IOException)
			{
			}
			
			if (lastError == null)
			{
				throw new SSHException("Failed to negotiate a version with the server! " + remoteIdentification, SSHException.CONNECT_FAILED);
			}
			else
			{
				if (lastError is SSHException)
					throw (SSHException) lastError;
				else
				{
					throw new SSHException(lastError.Message != null?lastError.Message:lastError.GetType().FullName, SSHException.CONNECT_FAILED);
				}
			}
		}


		internal System.String GetRemoteIdentification(SSHTransport transport)
		{
			
			try
			{
				System.String remoteIdentification = "";
				
				// Now wait for a reply and evaluate the ident string
				System.Text.StringBuilder buffer;
				Stream stream = transport.GetStream();

				int MAX_BUFFER_LENGTH = 255;
				
				// Look for a string starting with "SSH-"
				while (!remoteIdentification.StartsWith("SSH-"))
				{
					// Get the next string
					int ch;
					buffer = new System.Text.StringBuilder(MAX_BUFFER_LENGTH);
					while (((ch = stream.ReadByte()) != '\n') 
						&& (buffer.Length < MAX_BUFFER_LENGTH) 
						&& ch > - 1)
					{
						buffer.Append((char) ch);
					}
					
					if (ch == - 1)
					{
						throw new SSHException("Failed to read remote identification", SSHException.CONNECT_FAILED);
					}
					// Set trimming off any EOL characters
					remoteIdentification = buffer.ToString().Trim();
				}
				
				return remoteIdentification;
			}
			catch (System.Exception ex)
			{
				throw new SSHException(ex.Message, SSHException.CONNECT_FAILED);
			}
		}

		internal int SelectVersion(System.String remoteIdentification)
		{
			
			// Get the index of the seperators
			int l = remoteIdentification.IndexOf("-");
			int r = remoteIdentification.IndexOf("-", l + 1);
			
			// Get the version
			System.String remoteVersion = remoteIdentification.Substring(l + 1, (r) - (l + 1));
			
			// Evaluate the version
			if (remoteVersion.Equals("2.0"))
			{
				return SSH2;
			}
			else if (remoteVersion.Equals("1.99"))
			{
				return SSH1 | SSH2;
			}
			else if (remoteVersion.Equals("1.5"))
			{
				return SSH1;
			}
			else
			{
				throw new SSHException("Unsupported version " + remoteVersion + " detected!", SSHException.CONNECT_FAILED);
			}
		}

		/// <summary>
		/// Determine the version of a server.
		/// </summary>
		/// <param name="transport">A newly created transport connection</param>
		/// <returns></returns>
		public int DetermineVersion(SSHTransport transport)
		{
			int version = SelectVersion(GetRemoteIdentification(transport));
			try
			{
				transport.Close();
			}
			catch(System.IO.IOException)
			{
			}
			return version;
		}

		/// <summary>
		/// Get the <see cref="Maverick.SSH.SSHContext"/> for a given protocol
		/// </summary>
		/// <param name="version"></param>
		/// <returns></returns>
		public SSHContext GetContext(int version)
		{
			
			if ((version & SSH1) == 0 && (version & SSH2) == 0)
				throw new SSHException("SSHContext.GetContext(int) requires value of either SSH1 or SSH2", SSHException.BAD_API_USAGE);
			
			
			if (version == SSH2 && (supportedVersions & SSH2) != 0)
			{
				return ssh2Context;
			}
			
			throw new SSHException((version == SSH1?"SSH1":"SSH2") + " context is not available because it is not supported by this configuration", SSHException.BAD_API_USAGE);
		}
	}
}
