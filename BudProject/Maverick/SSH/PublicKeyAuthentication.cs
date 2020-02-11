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
	/// Summary description for PublicKeyAuthentication.
	/// </summary>
	public class PublicKeyAuthentication : SSHAuthentication
	{
		String username;
		SSHKeyPair pair;
		bool verify = false;

		/// <summary>
		/// Construct an initialized authentication object.
		/// </summary>
		/// <param name="pair"></param>
		public PublicKeyAuthentication(SSHKeyPair pair)
		{
			this.pair = pair;
		}

		/// <summary>
		/// Construct an unintialized authentication object.
		/// </summary>
		public PublicKeyAuthentication() 
		{
		}

		/// <summary>
		/// The key pair to or being used by this authentication.
		/// </summary>
		public SSHKeyPair KeyPair
		{
			get
			{
				return pair;
			}

			set
			{
				this.pair = value;
			}
		
		}

		/// <summary>
		/// The username for this authentication.
		/// </summary>
		public String Username
		{
			get
			{
				return username;
			}

			set
			{
				this.username = value;
			}
		}

		/// <summary>
		/// Flag to instruct the authentication attempt to simply verify the 
		/// key being used.
		/// </summary>
		/// <remarks>
		/// Use this when you want to verify that a key pair is acceptable by the server. The
		/// server will return <see cref="Maverick.SSH.AuthenticationResult.PUBLIC_KEY_ACCEPTABLE"/>
		/// if the key can be used for authentication. This typically indicates that the public key is 
		/// configured in the users <em>authorized_keys</em> file.
		/// </remarks>
		public bool VerifyOnly
		{
			get
			{
				return verify;
			}

			set
			{
				this.verify = value;
			}

		}
	}
}
