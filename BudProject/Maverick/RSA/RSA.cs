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
using System.Security.Cryptography;

namespace Maverick.Crypto.RSA
{
	/// <summary>
	/// Summary description for RSA.
	/// </summary>
	public class RSA
	{
		public RSA()
		{
		}

		public static BigInteger doPrivateCrt(BigInteger input,
			BigInteger privateExponent,
			BigInteger primeP, BigInteger primeQ,
			BigInteger crtCoefficient) 
		{
			return doPrivateCrt(input,
				primeP, primeQ,
				getPrimeExponent(privateExponent, primeP),
				getPrimeExponent(privateExponent, primeQ),
				crtCoefficient);
		}

		public static BigInteger doPrivateCrt(BigInteger input,
			BigInteger p, BigInteger q,
			BigInteger dP,
			BigInteger dQ,
			BigInteger qInv) 
		{
			if (!qInv.Equals(q.modInverse(p))) 
			{
				BigInteger t = p;
				p = q;
				q = t;
				t = dP;
				dP = dQ;
				dQ = t;
			}

			BigInteger s_1 = input.modPow(dP, p);
			BigInteger s_2 = input.modPow(dQ, q);

			BigInteger h = qInv.multiply(s_1.subtract(s_2)).mod(p);
			return s_2.add(h.multiply(q));

		}

		public static BigInteger getPrimeExponent(BigInteger privateExponent,
			BigInteger prime) 
		{
			BigInteger pe = prime.subtract(BigInteger.ONE);
			return privateExponent.mod(pe);
		}


		/*public static BigInteger PKCS1PadType2(BigInteger input, int pad_len, Random rand) 
		{
			int input_byte_length = (input.bitCount()+7)/8;

			byte[] pad = new byte[pad_len - input_byte_length - 3];

			for(int i = 0; i < pad.Length; i++) 
			{
				byte[] b = new byte[1];
				rand.NextBytes(b);
				while(b[0] == 0) rand.NextBytes(b);
				pad[i] = b[0];
			}

			BigInteger pad_int = new BigInteger(pad);
			pad_int = pad_int << ((input_byte_length + 1) * 8);
			BigInteger result = new BigInteger(2);
			result = result << ((pad_len - 2) * 8);
			result = result | pad_int;
			result = result | input;

			return result;
		}
		
		public static BigInteger PKCS1PadType1(BigInteger input, int pad_len) 
		{
			int input_byte_length = (input.bitCount()+7)/8;

			byte[] pad = new byte[pad_len - input_byte_length - 3];
			
			for(int i = 0; i < pad.Length; i++) 
			{
				pad[i] = (byte)0xff;
			}

			BigInteger pad_int = new BigInteger(pad);
			pad_int = pad_int << ((input_byte_length + 1) * 8);
			BigInteger result = new BigInteger(1);
			result = result << ((pad_len - 2) * 8);
			result = result | pad_int;
			result = result | input;

			return result;
		}*/

		public static BigInteger removePKCS1(BigInteger input, int type) 
		{

			byte[] strip = input.toByteArray();
			byte[] val;
			int i;

			if (strip[0] != type) 
			{
				throw new Exception("PKCS1 padding type " +
					type + " is not valid");
			}

			for (i = 1; i < strip.Length; i++) 
			{
				if (strip[i] == 0) 
				{
					break;
				}
				if (type == 0x01 && strip[i] != (byte) 0xff) 
				{
					throw new Exception("Corrupt data found in expected PKSC1 padding");
				}
			}

			if (i == strip.Length) 
			{
				throw new Exception("Corrupt data found in expected PKSC1 padding");
			}

			val = new byte[strip.Length - i];
			System.Array.Copy(strip, i, val, 0, val.Length);
			return new BigInteger(val);
		}

		public static BigInteger padPKCS1(BigInteger input, int type,
			int padLen, RandomNumberGenerator rand) 
		{
			BigInteger result;
			BigInteger rndInt;
			int inByteLen = (input.bitLength() + 7) / 8;

			if (inByteLen > padLen - 11) 
			{
				throw new Exception("PKCS1 failed to pad input! "
					+ "input=" + inByteLen
					+ " padding=" + padLen);
			}

			byte[] padBytes = new byte[ (padLen - inByteLen - 3) + 1];
			padBytes[0] = 0;

			for (int i = 1; i < (padLen - inByteLen - 3) + 1; i++) 
			{
				if (type == 0x01) 
				{
					padBytes[i] = (byte) 0xff;
				}
				else 
				{
					byte[] b = new byte[1];
					do 
					{
						rand.GetBytes(b);
					}
					while (b[0] == 0);
					padBytes[i] = b[0];
				}
			}

			rndInt = new BigInteger(1, padBytes);
			rndInt = rndInt.shiftLeft( (inByteLen + 1) * 8);
			result = BigInteger.valueOf(type);
			result = result.shiftLeft( (padLen - 2) * 8);
			result = result.or(rndInt);
			result = result.or(input);

			return result;
		}

		/*		public static BigInteger StripPKCS1Pad(BigInteger input, int type) 
				{
					byte[] strip = input.getBytes();
					int i;

					if(strip[0] != type) 
						throw new Exception(String.Format("Invalid PKCS1 padding {0}", type));

					for(i = 1; i < strip.Length; i++) 
					{
						if(strip[i] == 0) break;

						if(type == 0x01 && strip[i] != (byte)0xff)
							throw new Exception("Invalid PKCS1 padding, corrupt data");
					}

					if(i == strip.Length)
						throw new Exception("Invalid PKCS1 padding, corrupt data");

					byte[] val = new byte[strip.Length - i];
					Array.Copy(strip, i, val, 0, val.Length);
					return new BigInteger(val);
				}*/
	

		public static RSAPrivateCrtKey generateKey(int bits, RandomNumberGenerator secRand) 
		{
			return generateKey(bits, BigInteger.valueOf(0x10001L), secRand);
		}

		public static RSAPrivateCrtKey generateKey(int bits, BigInteger e,
			RandomNumberGenerator secRand) 
		{
			BigInteger p = null;
			BigInteger q = null;
			BigInteger t = null;
			BigInteger phi = null;
			BigInteger d = null;
			BigInteger u = null;
			BigInteger n = null;
			bool finished = false;

			int pbits = (bits + 1) / 2;
			int qbits = bits - pbits;

			while (!finished) 
			{
				p = new BigInteger(pbits, 25, secRand);

				q = new BigInteger(qbits, 25, secRand);
				

				if (p == q) 
				{
					continue;
				}
				else if (p == q) 
				{
					t = q;
					q = p;
					p = t;
				}

				if (!p.isProbablePrime(25))
					continue;

				if(!q.isProbablePrime(25))
					continue;

				t = p.gcd(q);
				if (t.compareTo(BigInteger.ONE) != 0) 
				{
					continue;
				}

				n = p.multiply(q);

				if (n.bitLength() != bits) 
				{
					continue;
				}

				phi = p.subtract(BigInteger.ONE).multiply(q.subtract(BigInteger.ONE));
				d = e.modInverse(phi);
				u = q.modInverse(p);

				finished = true;
			}

			return new RSAPrivateCrtKey(n, e, d, p, q,
			RSA.getPrimeExponent(d, p),
			RSA.getPrimeExponent(d, q),
			u);


		}
		
		public static BigInteger doPublic(BigInteger input, BigInteger modulus,
			BigInteger publicExponent) 
		{
			return input.modPow(publicExponent, modulus);
		}

		public static BigInteger doPrivate(BigInteger input, BigInteger modulus,
			BigInteger privateExponent) 
		{
			return doPublic(input, modulus, privateExponent);
		}
	}
	
}
