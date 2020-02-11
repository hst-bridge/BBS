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
using Maverick.Crypto.IO;
using Maverick.SSH.Packets;

namespace Maverick.SSH2
{
	/// <summary>
	/// Implementation of the SSH2 password authentication mechanism.
	/// </summary>
	/// <remarks>
	/// This extends the basic password mechanism by providing support for the changing
	/// of passwords.
	/// </remarks>
	public class SSH2PasswordAuthentication : PasswordAuthentication, SSH2AuthenticationClient
	{
		String newpassword;
		bool passwordChangeRequired = false;

		const int SSH_MSG_USERAUTH_PASSWD_CHANGEREQ = 60;

		/// <summary>
		/// Specifies the users new password in a password change operation.
		/// </summary>
		public String NewPassword
		{
			set
			{
				newpassword = value;
			}
		}

		/// <summary>
		/// Indicates that the users password required changing.
		/// </summary>
		/// <remarks>
		/// Once an authentication has taken place and failed you may check this property
		/// to determine whether the the failure was due to the user requiring a new password.
		/// If the user does require a password change you can proceed by prompting the user and
		/// setting the <see cref="Maverick.SSH2.SSH2PasswordAuthentication.NewPassword"/> property.
		/// </remarks>
		public bool RequiresPasswordChange
		{
			get
			{
				return passwordChangeRequired;
			}
		}

		/// <summary>
		/// Constructs an uninitialized authentication instance.
		/// </summary>
		public SSH2PasswordAuthentication()
		{
		}

		/// <summary>
		/// Constructs an initialized authentication instance
		/// </summary>
		/// <param name="password"></param>
		public SSH2PasswordAuthentication(String password) : base(password)
		{

		}

		/// <summary>
		/// Attempts to authenticate the user using their password.
		/// </summary>
		/// <param name="authentication"></param>
		/// <param name="servicename"></param>
		public void Authenticate(AuthenticationProtocol authentication,
								String servicename)	
		{

				try 
				{
					if(Username == null || Password == null) 
					{
						throw new SSHException("Username or password not set!",
									SSHException.BAD_API_USAGE);
					}

					if(passwordChangeRequired && newpassword == null) 
					{
						throw new SSHException("You must set a new password!",
							SSHException.BAD_API_USAGE);
					}

					ByteBuffer buf = new ByteBuffer();
					buf.WriteBool(passwordChangeRequired);
					buf.WriteString(Password);
					if(passwordChangeRequired) 
					{
						buf.WriteString(newpassword);

					}
					authentication.SendRequest(Username,
												servicename,
												"password",
												buf.ToByteArray());

					// We need to read the response since we may have password change.
					SSHPacket packet = authentication.ReadMessage();

					if(packet.MessageID != SSH_MSG_USERAUTH_PASSWD_CHANGEREQ) 
					{
						authentication.transport.Disconnect(
										"Unexpected message received",
											DisconnectionReason.PROTOCOL_ERROR);
						throw new SSHException(
								"Unexpected response from Authentication Protocol",
								SSHException.PROTOCOL_VIOLATION);
					}

					passwordChangeRequired = true;
					throw new SSH2AuthenticationResult(AuthenticationResult.FAILED, "");
				}
				catch(IOException ex) 
				{
					throw new SSHException(ex.Message,
						SSHException.INTERNAL_ERROR);
				}
		}

	}
}
