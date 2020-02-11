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

namespace Maverick.Crypto.DSA
{
	/// <summary>
	/// This class implements a DSA private key.
	/// </summary>
	public class DSAPrivateKey : DSAKey
	{
		protected BigInteger x;

		public DSAPrivateKey(BigInteger p, BigInteger q, BigInteger g, BigInteger x) : base(p, q, g)
		{
			this.x = x;
		}

		public BigInteger X 
		{
			get
			{ 
				return x;
			}
		}

		public byte[] Sign(byte[] msg) 
		{

			SHA1Digest h = new SHA1Digest();
			h.update(msg, 0, msg.Length);
			byte[] data = new byte[h.getDigestSize()];
			h.doFinal(data, 0);
			return DSA.Sign(x, p, q, g, data);
		}

		public override bool Equals(Object obj) 
		{
			if (obj is DSAPrivateKey) 
			{
					DSAPrivateKey key = (DSAPrivateKey) obj;
					return x.Equals(key.x)
						&& p.Equals(key.p)
						&& q.Equals(key.q)
						&& g.Equals(key.g);
				}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

	}
}
