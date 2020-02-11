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
using Maverick.Crypto.Digests;
using Maverick.Crypto.Math;

namespace Maverick.Crypto.DSA
{

	public class DSAPublicKey : DSAKey
	{
		public BigInteger Y
		{
			get
			{
				return y;
			}
			
		}

		public int BitLength
		{
			get
			{
				return p.bitCount();
			}
			
		}
		
		protected internal BigInteger y;
		
		public DSAPublicKey(BigInteger p, BigInteger q, BigInteger g, BigInteger y) : base(p, q, g)
		{
			this.y = y;
		}
		
		public DSAPublicKey()
		{
		}
				

		public virtual bool VerifySignature(byte[] signature, byte[] msg)
		{
					
			// Create a SHA1 hash of the message
			SHA1Digest h = new SHA1Digest();
			h.update(msg, 0, msg.Length);
			byte[] data = new byte[h.getDigestSize()];
			h.doFinal(data, 0);
					
			return DSA.Verify(y, p, q, g, signature, data);
		}
			
		protected internal bool VerifySignature(byte[] msg, BigInteger r, BigInteger s)
		{
					
			// Create a SHA1 hash of the message
			SHA1Digest h = new SHA1Digest();
			h.update(msg, 0, msg.Length);
			byte[] data = new byte[h.getDigestSize()];
			h.doFinal(data, 0);
						
						
			BigInteger m = new BigInteger(1, data);
			m = m.mod(q);

			if (BigInteger.valueOf(0).compareTo(r) >= 0 || q.compareTo(r) <= 0) 
			{
				return false;
			}

			if (BigInteger.valueOf(0).compareTo(s) >= 0 || q.compareTo(s) <= 0) 
			{
				return false;
			}

			BigInteger w = s.modInverse(q);
			BigInteger u1 = m.multiply(w).mod(q);
			BigInteger u2 = r.multiply(w).mod(q);

			BigInteger v = g.modPow(u1, p).multiply(y.modPow(u2, p)).mod(p).mod(q);

			return (v.compareTo(r) == 0);
		}
	}
}
