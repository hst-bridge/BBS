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
using Maverick.SSH;
using Maverick.SSH.Packets;

namespace Maverick.SSH2
{

	/// <summary>
	/// This namespace contains the SSH2 implementation of the <see cref="Maverick.SSH"/> interfaces.
	/// </summary>
	internal class NamespaceDoc
	{
	}

	/// <summary>
	/// Main implementation of the SSH Authentication Protocol. 
	/// </summary>
	/// <remarks>
	/// This class is used by <see cref="Maverick.SSH2.SSH2AuthenticationClient"/>
	/// implementations and exposes <see cref="Maverick.SSH2.AuthenticationProtocol.ReadMessage"/> which implementations use to read authentication 
	/// specific messages and <see cref="Maverick.SSH2.AuthenticationProtocol.SendRequest"/> to send authenticaiton requests. By 
	/// using these method's the protocol is also able to detect when authentication has succeeded or failed; when this happens an 
	/// <see cref="Maverick.SSH2.SSH2AuthenticationResult"/> is thrown.
	/// </remarks>
	public class AuthenticationProtocol
	{

		/// <summary>
		/// Message sent by the client to initiate an authentication attempt.
		/// </summary>
		public const int SSH_MSG_USERAUTH_REQUEST = 50;

		/// <summary>
		/// Message sent by the server to indicate that the authentication attempt failed.
		/// </summary>
		public const int SSH_MSG_USERAUTH_FAILURE = 51;

		/// <summary>
		/// Message sent by the server to indicate that the authentication attempt succeeded.
		/// </summary>
		public const int SSH_MSG_USERAUTH_SUCCESS = 52;

		/// <summary>
		/// Message sent by the server which provides the banner text to display to the user prior to 
		/// authentication
		/// </summary>
		public const int SSH_MSG_USERAUTH_BANNER = 53;

		internal TransportProtocol transport;
		internal ConnectionProtocol connection;
		internal AuthenticationResult state = AuthenticationResult.FAILED;

		/// <summary>
		/// Defines the name of the Authentication protocols transport service "ssh-userauth".
		/// </summary>
		public const String SERVICE_NAME = "ssh-userauth";

		/// <summary>
		/// Create an authentication protocol instance
		/// </summary>
		/// <param name="transport"></param>
		/// <param name="connection"></param>
		public AuthenticationProtocol(TransportProtocol transport,
			ConnectionProtocol connection) 
		{
			this.transport = transport;
			this.connection = connection;
			transport.StartService(SERVICE_NAME);
		}

		/// <summary>
		/// Determine whether this connection has been authenticated
		/// </summary>
		public bool IsAuthenticated
		{
			get
			{
				return state == AuthenticationResult.COMPLETE;
			}
		}

		/// <summary>
		/// Read a message from the transport. This method will only return when a suitable message is received
		/// from the server. If an SSH_MSG_USERAUTH_SUCCESS or SSH_MSG_USERAUTH_FAILURE message is received
		/// this method throws an <see cref="Maverick.SSH2.SSH2AuthenticationResult"/>.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Maverick.SSH2.SSH2AuthenticationResult"/>
		public SSHPacket ReadMessage() 
		{

			SSHPacket packet;

			do
			{
				packet = transport.NextMessage();

			}while (ProcessMessage(packet));

			return packet;
		}

		/// <summary>
		/// Create an SSH packet for use by authentication schemes.
		/// </summary>
		/// <returns></returns>
		public SSHPacket GetSSHPacket()
		{
			return transport.GetSSHPacket(true);
		}

		/// <summary>
		/// Attempt to authenticate with the authentication scheme provided.
		/// </summary>
		/// <param name="auth"></param>
		/// <param name="servicename"></param>
		/// <returns></returns>
		public AuthenticationResult Authenticate(SSH2AuthenticationClient auth, 
			String servicename) 
		{

			try 
			{
				auth.Authenticate(this, servicename);
				ReadMessage();
				transport.Disconnect(
					"Unexpected response received from Authentication Protocol",
					DisconnectionReason.PROTOCOL_ERROR);
				throw new SSHException("Unexpected response received from Authentication Protocol",
					SSHException.PROTOCOL_VIOLATION);
			}
			catch (SSH2AuthenticationResult result) 
			{
				state = result.Result;
				return state;
			}
		}

		/// <summary>
		/// Get the available authentication methods for the given user.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="servicename"></param>
		/// <returns></returns>
		public String RequestAuthenticationMethods(String username,
			String servicename) 
		{
			SendRequest(username, servicename, "none", null);

			try 
			{
				ReadMessage();
				transport.Disconnect(
					"Unexpected response received from Authentication Protocol",
					DisconnectionReason.PROTOCOL_ERROR);
				throw new SSHException(
					"Unexpected response received from Authentication Protocol",
					SSHException.PROTOCOL_VIOLATION);
			}
			catch (SSH2AuthenticationResult result) 
			{
				state = result.Result;
				return result.AvailableAuthenticationMethods;
			}
		}

		/// <summary>
		/// Send an authentication request. This method will be called from the <see cref="Maverick.SSH2.SSH2AuthenticationClient.Authenticate"/>
		/// method to initiate the authentication proceedure.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="servicename"></param>
		/// <param name="methodname"></param>
		/// <param name="requestdata"></param>
		public void SendRequest(String username,
			String servicename,
			String methodname,
			byte[] requestdata) 
		{
			try 
			{
				SSHPacket packet = transport.GetSSHPacket(true);
				packet.WriteByte(SSH_MSG_USERAUTH_REQUEST);
				packet.WriteString(username);
				packet.WriteString(servicename);
				packet.WriteString(methodname);
				if(requestdata != null) 
				{
					packet.WriteBytes(requestdata);

				}
				transport.SendMessage(packet);
			}
			catch(IOException ex) 
			{
				throw new SSHException(ex.Message,
					SSHException.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// Get the session id for this connection
		/// </summary>
		public byte[] SessionIdentifier 
		{
			get 
			{
				return transport.SessionIdentifier;
			}
		}

		internal bool ProcessMessage(SSHPacket packet)
		{

			try 
			{
				switch(packet.MessageID) 
				{
					case SSH_MSG_USERAUTH_FAILURE: 
					{
						
						String auths = packet.ReadString();
						
						throw new SSH2AuthenticationResult(
							packet.ReadBool() 
							? AuthenticationResult.FURTHER_AUTHENTICATION_REQUIRED
							: AuthenticationResult.FAILED,
							auths);
					}

					case SSH_MSG_USERAUTH_SUCCESS: 
					{
						connection.Start();
						throw new SSH2AuthenticationResult(AuthenticationResult.COMPLETE, "");
					}

					case SSH_MSG_USERAUTH_BANNER: 
					{
						if(((SSH2Context)transport.Context).Banner!=null)
						{
							((SSH2Context)transport.Context).Banner.DisplayBanner(packet.ReadString());
						}
						return true;
						
					}
					default:
						return false;
				
				}

			}
			catch(IOException ex)
			{
				throw new SSHException(ex.Message,
					SSHException.INTERNAL_ERROR);
			}
		}
	}
}
