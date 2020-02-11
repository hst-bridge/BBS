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
using Maverick.Crypto.DSA;
using Maverick.Crypto.Math;

namespace Maverick.SSH
{
	/// <summary>
	/// This class provides the <see cref="Maverick.SSH.SSHPrivateKey"/> implementation
	/// for an SSH DSA private key.
	/// </summary>
	public class SSHDSAPrivateKey : DSAPrivateKey, SSHPrivateKey
	{

		/// <summary>
		/// Construct an SSH DSA key from an existing DSA key instance
		/// </summary>
		/// <param name="key"></param>
		internal SSHDSAPrivateKey(DSAPrivateKey key) : base(key.P,
			key.Q,
			key.G,
			key.X)
		{
			
		}

		/// <summary>
		/// Construct an SSH DSA private key from the keys parameters.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="q"></param>
		/// <param name="g"></param>
		/// <param name="x"></param>
		public SSHDSAPrivateKey(BigInteger p,
			BigInteger q,
			BigInteger g,
			BigInteger x) : base(p, q, g, x)
		{
			
		}

	}
}
