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

namespace Maverick.Crypto.Math
{

	/**
	 * An implementation of SecureRandom specifically for the
	 * light-weight API, JDK 1.0, and the J2ME. Random generation is 
	 * based on the traditional SHA1 with counter. Calling setSeed
	 * will always increase the entropy of the hash.
	 */
	public class SecureRandom : System.Random
	{
		private static  SecureRandom rand = new SecureRandom();
		private long        counter = 1;
		private SHA1Digest  digest = new SHA1Digest();
		private byte[]      state;


		public SecureRandom() : base(0)
		{
			state = new byte[digest.getDigestSize()];
			setSeed(DateTime.Now.Ticks);
		}

		public SecureRandom(byte[] inSeed)
		{
			state = new byte[digest.getDigestSize()];
			setSeed(inSeed);
		}

		public static SecureRandom GetInstance()
		{
			rand.setSeed(DateTime.Now.Ticks);
			return rand;
		}


		public virtual void setSeed(byte[] inSeed)
		{
			lock(this)
			{
				digest.update(inSeed, 0, inSeed.Length);
			}
		}

		public virtual void setSeed(long rSeed)
		{
			if (rSeed != 0) setSeed(longToBytes(rSeed));
		}

		public static byte[] getSeed(int numBytes)
		{
			byte[] rv = new byte[numBytes];

			rand.setSeed(DateTime.Now.Ticks);
			rand.NextBytes(rv);
			return rv;
		}

		// public instance methods
		public virtual byte[] generateSeed(int numBytes)
		{
			lock(this)
			{
				byte[] rv = new byte[numBytes];
				NextBytes(rv);	
				return rv;
			}
		}


		// public methods overriding random
		public override void NextBytes(byte[] bytes)
		{
			lock(this)
			{
				int stateOff = 0;

				digest.doFinal(state, 0);

				for (int i = 0; i < bytes.Length; i++)
				{
					if (stateOff == state.Length)
					{
						byte[]  b = longToBytes(counter++);

						digest.update(b, 0, b.Length);
						digest.update(state, 0, state.Length);
						digest.doFinal(state, 0);
						stateOff = 0;
					}
					bytes[i] = state[stateOff++];
				}

				byte[]  bb = longToBytes(counter++);

				digest.update(bb, 0, bb.Length);
				digest.update(state, 0, state.Length);
			}
		}


		private byte[]  intBytes = new byte[4];

		public virtual int NextInt()
		{
			lock(this)
			{
				NextBytes(intBytes);

				int result = 0;

				for (int i = 0; i < 4; i++)
				{
					result = (result << 8) + (intBytes[i] & 0xff);
				}

				return result;
			}
		}

		protected int next(int numBits)
		{
			int     size = (numBits + 7) / 8;
			byte[]  bytes = new byte[size];

			NextBytes(bytes);

			int result = 0;

			for (int i = 0; i < size; i++)
			{
				result = (result << 8) + (bytes[i] & 0xff);
			}

			return result & ((1 << numBits) - 1);

		}

		private byte[]  longBytes = new byte[8];

		private byte[] longToBytes(long    val)
		{
			for (int i = 0; i != 8; i++)
			{
				longBytes[i] = (byte)val;
				val =  (long) ((ulong) val >> 8);
			}
			return longBytes;
		}
	}
}