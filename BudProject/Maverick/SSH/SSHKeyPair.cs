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
using Maverick.SSH;

namespace Maverick.SSH
{
	/// <summary>
	/// This class holds a public/private key pair.
	/// </summary>
	public class SSHKeyPair
	{
		SSHPublicKey publickey;
		SSHPrivateKey privatekey;

		internal SSHKeyPair(SSHPublicKey publickey, SSHPrivateKey privatekey)
		{
			this.publickey = publickey;
			this.privatekey = privatekey;
		}

		/// <summary>
		/// Get the private key for this key pair.
		/// </summary>
		public SSHPrivateKey PrivateKey
		{
			get
			{
				return privatekey;
			}
		}

		/// <summary>
		/// Get the public key for this key pair.
		/// </summary>
		public SSHPublicKey PublicKey
		{
			get
			{
				return publickey;
			}
		}
		
	}
}
