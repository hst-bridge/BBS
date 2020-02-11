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

namespace Maverick.Crypto.DSA
{
	/// <summary>
	/// Summary description for DSA.
	/// </summary>
	public class DSA
	{
		public static byte[] Sign(BigInteger x, BigInteger p, BigInteger q, BigInteger g, byte[] data)
		{

			BigInteger hM = new BigInteger(1, data);

			hM = hM.mod(q);

			BigInteger r = g.modPow(x, p).mod(q);
			BigInteger s = x.modInverse(q).multiply(hM.add(x.multiply(r))).mod(q);

			int dataSz = data.Length;
			byte[] signature = new byte[dataSz * 2];
			byte[] tmp;

			tmp = unsignedBigIntToBytes(r, dataSz);
			Array.Copy(tmp, 0, signature, 0, dataSz);

			tmp = unsignedBigIntToBytes(s, dataSz);
			Array.Copy(tmp, 0, signature, dataSz, dataSz);

			return signature;
		}
		
		public static bool Verify(BigInteger y, BigInteger p, BigInteger q, BigInteger g, byte[] signature, byte[] data)
		{
			int dataSz = signature.Length / 2;
			byte[] ra = new byte[dataSz];
			byte[] sa = new byte[dataSz];
			
			Array.Copy(signature, 0, ra, 0, dataSz);
			Array.Copy(signature, dataSz, sa, 0, dataSz);
			
			BigInteger hM = new BigInteger(data);
			BigInteger r = new BigInteger(ra);
			BigInteger s = new BigInteger(sa);
			
			hM = hM.mod(q);

			BigInteger w = s.modInverse(q);
			BigInteger u1 = hM.multiply(w).mod(q);
			BigInteger u2 = r.multiply(w).mod(q);
			BigInteger v = g.modPow(u1, p).multiply(y.modPow(u2, p)).mod(p).mod(q);

			return (v.compareTo(r) == 0);
		}
		
		private static byte[] unsignedBigIntToBytes(BigInteger bi, int size)
		{
			byte[] tmp = bi.toByteArray();
			byte[] tmp2 = null;
			if (tmp.Length > size)
			{
				tmp2 = new byte[size];
				Array.Copy(tmp, tmp.Length - size, tmp2, 0, size);
			}
			else if (tmp.Length < size)
			{
				tmp2 = new byte[size];
				Array.Copy(tmp, 0, tmp2, size - tmp.Length, tmp.Length);
			}
			else
			{
				tmp2 = tmp;
			}
			return tmp2;
		}
		
		public static BigInteger generatePublicKey(BigInteger g, BigInteger p, BigInteger x)
		{
			return g.modPow(x, p);
		}
		
		/*public static DsaPrivateKey generateKey(int bits, SecureRandom rnd)
		{
			
			BigInteger p, q, g, x, y;
			BigInteger ZERO = BigInteger.valueOf(0);
			DSAParametersGenerator dsaParams = new DSAParametersGenerator();
			dsaParams.init(bits, 80, rnd);
			
			DSAParameters dsa = dsaParams.generateParameters();
			
			q = dsa.Q;
			p = dsa.P;
			g = dsa.G;
			
			do 
			{
				x = new BigInteger(160, rnd);
			}
			while (x.Equals(ZERO) || x.compareTo(q) >= 0);
			
			//
			// calculate the public key.
			//
			y = g.modPow(x, p);
			
			return new DsaPrivateKey(p, q, g, x);
		}
	}*/
	
		/// <summary> generate suitable parameters for DSA, in line with FIPS 186-2.
		/// </summary>
		/*class DSAParametersGenerator
		{
			private int size;
			private int certainty;
			private SecureRandom random;
		
			//UPGRADE_NOTE: The initialization of  'ONE' was moved to static method 'com.maverick.crypto.publickey.DSAParametersGenerator'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
			private static BigInteger ONE;
			//UPGRADE_NOTE: The initialization of  'TWO' was moved to static method 'com.maverick.crypto.publickey.DSAParametersGenerator'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
			private static BigInteger TWO;
		
			/// <summary> initialise the key generator.
			/// *
			/// </summary>
			/// <param name="size">size of the key (range 2^512 -> 2^1024 - 64 bit increments)
			/// </param>
			/// <param name="certainty">measure of robustness of prime (for FIPS 186-2 compliance this should be at least 80).
			/// </param>
			/// <param name="random">random byte source.
			/// 
			/// </param>
			public virtual void  init(int size, int certainty, SecureRandom random)
			{
				this.size = size;
				this.certainty = certainty;
				this.random = random;
			}
		
			/// <summary> add value to b, returning the result in a. The a value is treated
			/// as a BigInteger of length (a.length * 8) bits. The result is
			/// modulo 2^a.length in case of overflow.
			/// </summary>
			private void  add(sbyte[] a, sbyte[] b, int value_Renamed)
			{
				int x = (b[b.Length - 1] & 0xff) + value_Renamed;
			
				a[b.Length - 1] = (sbyte) x;
				x = SupportClass.URShift(x, 8);
			
				for (int i = b.Length - 2; i >= 0; i--)
				{
					x += (b[i] & 0xff);
					a[i] = (sbyte) x;
					x = SupportClass.URShift(x, 8);
				}
			}
		
			/// <summary> which generates the p and g values from the given parameters,
			/// returning the DSAParameters object.
			/// <p>
			/// Note: can take a while...
			/// </summary>
			public virtual DSAParameters generateParameters()
			{
				sbyte[] seed = new sbyte[20];
				sbyte[] part1 = new sbyte[20];
				sbyte[] part2 = new sbyte[20];
				sbyte[] u = new sbyte[20];
				SHA1Digest sha1 = new SHA1Digest();
				int n = (size - 1) / 160;
				sbyte[] w = new sbyte[size / 8];
			
				BigInteger q = null, p = null, g = null;
				int counter = 0;
				bool primesFound = false;
			
				while (!primesFound)
				{
					do 
					{
						random.NextBytes(SupportClass.ToByteArray(seed));
					
						sha1.update(seed, 0, seed.Length);
					
						sha1.doFinal(part1, 0);
					
						Array.Copy(SupportClass.ToByteArray(seed), 0, SupportClass.ToByteArray(part2), 0, seed.Length);
					
						add(part2, seed, 1);
					
						sha1.update(part2, 0, part2.Length);
					
						sha1.doFinal(part2, 0);
					
						for (int i = 0; i != u.Length; i++)
						{
							u[i] = (sbyte) (part1[i] ^ part2[i]);
						}
					
						u[0] |= (sbyte) SupportClass.Identity(0x80);
						u[19] |= (sbyte) 0x01;
					
						q = new BigInteger(1, u);
					}
					while (!q.isProbablePrime(certainty));
				
					counter = 0;
				
					int offset = 2;
				
					while (counter < 4096)
					{
						for (int k = 0; k < n; k++)
						{
							add(part1, seed, offset + k);
							sha1.update(part1, 0, part1.Length);
							sha1.doFinal(part1, 0);
							Array.Copy(SupportClass.ToByteArray(part1), 0, SupportClass.ToByteArray(w), w.Length - (k + 1) * part1.Length, part1.Length);
						}
					
						add(part1, seed, offset + n);
						sha1.update(part1, 0, part1.Length);
						sha1.doFinal(part1, 0);
						Array.Copy(SupportClass.ToByteArray(part1), part1.Length - ((w.Length - (n) * part1.Length)), SupportClass.ToByteArray(w), 0, w.Length - n * part1.Length);
					
						w[0] |= (sbyte) SupportClass.Identity(0x80);
					
						BigInteger x = new BigInteger(1, w);
					
						BigInteger c = x.mod(q.multiply(TWO));
					
						p = x.subtract(c.subtract(ONE));
					
						if (p.testBit(size - 1))
						{
							if (p.isProbablePrime(certainty))
							{
								primesFound = true;
								break;
							}
						}
					
						counter += 1;
						offset += n + 1;
					}
				}
			
				//
				// calculate the generator g
				//
				BigInteger pMinusOneOverQ = p.subtract(ONE).divide(q);
			
				for (; ; )
				{
					BigInteger h = new BigInteger(size, random);
					if (h.compareTo(ONE) <= 0 || h.compareTo(p.subtract(ONE)) >= 0)
					{
						continue;
					}
				
					g = h.modPow(pMinusOneOverQ, p);
					if (g.compareTo(ONE) <= 0)
					{
						continue;
					}
				
					break;
				}
			
				return new DSAParameters(p, q, g, new DSAValidationParameters(seed, counter));
			}
			static DSAParametersGenerator()
			{
				ONE = BigInteger.valueOf(1);
				TWO = BigInteger.valueOf(2);
			}
		}
	
		class DSAParameters
		{
			virtual public BigInteger P
			{
				get
				{
					return p;
				}
			
			}
			virtual public BigInteger Q
			{
				get
				{
					return q;
				}
			
			}
			virtual public BigInteger G
			{
				get
				{
					return g;
				}
			
			}
			virtual public DSAValidationParameters ValidationParameters
			{
				get
				{
					return validation;
				}
			
			}
			private BigInteger g;
			private BigInteger q;
			private BigInteger p;
			private DSAValidationParameters validation;
		
			public DSAParameters(BigInteger p, BigInteger q, BigInteger g)
			{
				this.g = g;
				this.p = p;
				this.q = q;
			}
		
			public DSAParameters(BigInteger p, BigInteger q, BigInteger g, DSAValidationParameters params_Renamed)
			{
				this.g = g;
				this.p = p;
				this.q = q;
				this.validation = params_Renamed;
			}
		
		
		
		
		
			public  override bool Equals(System.Object obj)
			{
				if (!(obj is DSAParameters))
				{
					return false;
				}
			
				DSAParameters pm = (DSAParameters) obj;
			
				return (pm.P.Equals(p) && pm.Q.Equals(q) && pm.G.Equals(g));
			}
		}
	
		class DSAValidationParameters
		{
			virtual public int Counter
			{
				get
				{
					return counter;
				}
			
			}
			virtual public sbyte[] Seed
			{
				get
				{
					return seed;
				}
			
			}
			private sbyte[] seed;
			private int counter;
		
			public DSAValidationParameters(sbyte[] seed, int counter)
			{
				this.seed = seed;
				this.counter = counter;
			}
		
		
		
			public  override bool Equals(System.Object o)
			{
				if (o == null || !(o is DSAValidationParameters))
				{
					return false;
				}
			
				DSAValidationParameters other = (DSAValidationParameters) o;
			
				if (other.counter != this.counter)
				{
					return false;
				}
			
				if (other.seed.Length != this.seed.Length)
				{
					return false;
				}
			
				for (int i = 0; i != other.seed.Length; i++)
				{
					if (other.seed[i] != this.seed[i])
					{
						return false;
					}
				}
			
				return true;
			}
		}*/
	}
}
