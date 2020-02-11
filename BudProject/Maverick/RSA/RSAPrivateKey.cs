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
 * 
 * This file has been derived from the BouncyCastle Java Crypto API
 * 
 * Copyright (c) 2000 The Legion Of The Bouncy Castle (http://www.bouncycastle.org)
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do 
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *****************************************************************************/
using System;
using Maverick.Crypto.Math;
using Maverick.Crypto.Digests;
using System.Security.Cryptography;

namespace Maverick.Crypto.RSA
{
	/// <summary>
	/// Summary description for RSAPrivateKey.
	/// </summary>
	public class RSAPrivateKey : RSAKey
	{
		protected internal BigInteger privateExponent;

		public RSAPrivateKey(BigInteger modulus, BigInteger privateExponent): base(modulus) {
			this.privateExponent = privateExponent;
		}

		public BigInteger PrivateExponent
		{
			get
			{
				return privateExponent;
			}
		}

		public virtual byte[] Sign(byte[] msg) {


			SHA1Digest hash = new SHA1Digest();
			hash.update(msg, 0, msg.Length);

			byte[] data = new byte[hash.getDigestSize()];
			hash.doFinal(data, 0);

			byte[] tmp = new byte[data.Length + ASN_SHA1.Length];
			Array.Copy(ASN_SHA1, 0, tmp, 0, ASN_SHA1.Length);
			Array.Copy(data, 0, tmp, ASN_SHA1.Length, data.Length);
			data = tmp;

			BigInteger dataInt = new BigInteger(1, data);
			int mLen = (modulus.bitLength() + 7) / 8;

			dataInt = RSA.padPKCS1(dataInt, 1, mLen, new RNGCryptoServiceProvider());

			BigInteger signatureInt = null;

			signatureInt = RSA.doPrivate(dataInt,
										modulus, privateExponent);

			byte[] sig = unsignedBigIntToBytes(signatureInt, mLen);

			return sig;

		}

		protected static byte[] unsignedBigIntToBytes(BigInteger bi, int size) {
			byte[] tmp = bi.toByteArray();
			byte[] tmp2 = null;
			if (tmp.Length > size) {
			tmp2 = new byte[size];
			Array.Copy(tmp, tmp.Length - size, tmp2, 0, size);
			}
			else if (tmp.Length < size) {
			tmp2 = new byte[size];
			Array.Copy(tmp, 0, tmp2, size - tmp.Length, tmp.Length);
			}
			else {
			tmp2 = tmp;
			}
			return tmp2;
		}

		public override bool Equals(Object obj) {
			if(obj is RSAPrivateKey) {
			RSAPrivateKey key = (RSAPrivateKey) obj;
			return key.BitLength == BitLength
				&& key.Modulus == Modulus
				&& key.PrivateExponent == PrivateExponent;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

	}
}
