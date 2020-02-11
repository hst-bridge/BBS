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
using Maverick.Crypto.Math;
using Maverick.Crypto.RSA;

namespace Maverick.SSH
{
	/// <summary>
	/// This class provides an <see cref="Maverick.SSH.SSHPrivateKey"/> implementation
	/// of an SSH RSA private key.
	/// </summary>
	public class SSHRSAPrivateKey : RSAPrivateKey, SSHPrivateKey
	{
		/// <summary>
		/// Construct a private key from an existing RSA private key.
		/// </summary>
		/// <param name="key"></param>
		internal SSHRSAPrivateKey(RSAPrivateKey key) : base(key.Modulus, key.PrivateExponent)
		{
			
		}

		/// <summary>
		/// Construct a private key from its RSA parameters.
		/// </summary>
		/// <param name="modulus"></param>
		/// <param name="privateExponent"></param>
		public SSHRSAPrivateKey(BigInteger modulus, BigInteger privateExponent) : base(modulus, privateExponent)
		{
		}

	}
}
