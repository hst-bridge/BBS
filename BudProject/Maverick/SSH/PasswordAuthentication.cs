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
	/// Basic password authentication class used for SSH password authentication. 
	/// </summary>
	/// <remarks>
	/// Once a connection has been established to an SSH server the user is normally required to authenticate themselves. 
	/// This class implements a basic password <see cref="Maverick.SSH.SSHAuthentication"/> that can 
	/// be passed into the <see cref="Maverick.SSH.SSHClient"/> to authenticate. As a username is 
	/// required to establish a connection it is not required that it be set on the password object, 
	/// however if you wish to change the username you can do so (although this may not be allowed by
	/// some server implementations).<br/>
	/// <br/>
	/// It is recommended that in situations where you may be connecting to an SSH2 server, that the
	/// <see cref="Maverick.SSH2.SSH2PasswordAuthentication"/> subclass is used instead. This extends 
	/// the basic functionality provided here by supporting the changing of the users password. 
	/// <example>
	/// <code>
	/// // Create a connection
	/// SSHConnector con = SSHConnector.Create();
	/// SSHClient ssh = con.Connect(new TcpClientTransport("my.domain.com, 22),
	///									"root", true);
	///	
	///	// Prepare the authentication request								
	///	PasswordAuthentication pwd = new PasswordAuthentication();
	///	pwd.Password = "*********";
	///	
	///	// Authenticate
	///	switch(ssh.Authenticate(pwd))
	/// {
	///		case AuthenticationResult.COMPLETE:
	///		{
	///			// Authentication succeeded
	///			break;
	///		}
	///		default:
	///		{
	///			// Authentication failed
	///			break;
	///		}
	///	}
	///	</code>
	/// </example>
	/// </remarks>
	public class PasswordAuthentication : SSHAuthentication
	{
		String username;
		String password;

		/// <summary>
		/// Set or get the username to be used in the authentication process.
		/// </summary>
		public String Username
		{
			get
			{
				return username;
			}

			set
			{
				username = value;
			}
		}

		/// <summary>
		/// Set or get the password to be used in the authentication process.
		/// </summary>
		public String Password
		{
			get
			{
				return password;
			}

			set
			{
				password = value;
			}
		}

		/// <summary>
		/// Constructs an empty instance of authentication object.
		/// </summary>
		public PasswordAuthentication()
		{
		}

		/// <summary>
		/// Constructs an initialized instance.
		/// </summary>
		/// <param name="password"></param>
		public PasswordAuthentication(String password)
		{
			this.password = password;
		}
	}
}
