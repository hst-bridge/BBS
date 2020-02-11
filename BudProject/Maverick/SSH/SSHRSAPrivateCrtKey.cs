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
	/// of an SSH RSA key which supports the Chinese Remainder Theorem(CRT) representation
	/// to speed up private key operations.
	/// </summary>
	public class SSHRSAPrivateCrtKey : RSAPrivateCrtKey, SSHPrivateKey
	{

		/// <summary>
		/// Construct a key using an existing RSA key.
		/// </summary>
		/// <param name="key"></param>
		internal SSHRSAPrivateCrtKey(RSAPrivateCrtKey key) : base(key.Modulus,
			key.PublicExponent,
			key.PrivateExponent,
			key.PrimeP,
			key.PrimeQ,
			key.PrimeExponentP,
			key.PrimeExponentQ,
			key.CrtCoefficient)
		{


		}

		/// <summary>
		/// Construct a private key from its RSA parameters.
		/// </summary>
		/// <param name="modulus"></param>
		/// <param name="publicExponent"></param>
		/// <param name="privateExponent"></param>
		/// <param name="primeP"></param>
		/// <param name="primeQ"></param>
		/// <param name="primeExponentP"></param>
		/// <param name="primeExponentQ"></param>
		/// <param name="crtCoefficient"></param>
		public SSHRSAPrivateCrtKey(BigInteger modulus,
			BigInteger publicExponent,
			BigInteger privateExponent,
			BigInteger primeP, BigInteger primeQ,
			BigInteger primeExponentP,
			BigInteger primeExponentQ,
			BigInteger crtCoefficient) : base(modulus,
			publicExponent,
			privateExponent,
			primeP, 
			primeQ,
			primeExponentP,
			primeExponentQ,
			crtCoefficient)
		{
		}

		/// <summary>
		/// Construct a private key from its parameters.
		/// </summary>
		/// <param name="modulus"></param>
		/// <param name="publicExponent"></param>
		/// <param name="privateExponent"></param>
		/// <param name="primeP"></param>
		/// <param name="primeQ"></param>
		/// <param name="crtCoefficient"></param>
		public SSHRSAPrivateCrtKey(BigInteger modulus,
			BigInteger publicExponent,
			BigInteger privateExponent,
			BigInteger primeP,
			BigInteger primeQ,
			BigInteger crtCoefficient) : base(modulus, publicExponent, privateExponent, primeP, primeQ,
											 RSA.getPrimeExponent(privateExponent, primeP),
											 RSA.getPrimeExponent(privateExponent, primeQ),
											 crtCoefficient) 
		{
			
		}
	}
}
