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

namespace Maverick.Crypto.Ciphers
{
	/**
	* a class that provides a basic DESede (or Triple DES) engine.
	*/
	public class DESedeEngine : DESEngine
	{
		new protected static readonly int  BLOCK_SIZE = 8;

		private int[]               workingKey1 = null;
		private int[]               workingKey2 = null;
		private int[]               workingKey3 = null;

		private bool             forEncryption;

		/**
		* standard constructor.
		*/
		public DESedeEngine()
		{
		}

		/**
		* initialise a DESede cipher.
		*
		* @param forEncryption whether or not we are for encryption.
		* @param params the parameters required to set up the cipher.
		* @exception IllegalArgumentException if the params argument is
		* inappropriate.
		*/
		public override void init(
			bool           encrypting,
			byte[] key)
		{

			byte[]      keyMaster = key;
			byte[]      key1 = new byte[8], key2 = new byte[8], key3 = new byte[8];

			this.forEncryption = encrypting;

			if (keyMaster.Length == 24)
			{
				Array.Copy(keyMaster, 0, key1, 0, key1.Length);
				Array.Copy(keyMaster, 8, key2, 0, key2.Length);
				Array.Copy(keyMaster, 16, key3, 0, key3.Length);

				workingKey1 = generateWorkingKey(encrypting, key1);
				workingKey2 = generateWorkingKey(!encrypting, key2);
				workingKey3 = generateWorkingKey(encrypting, key3);
			}
			else        // 16 byte key
			{
				Array.Copy(keyMaster, 0, key1, 0, key1.Length);
				Array.Copy(keyMaster, 8, key2, 0, key2.Length);

				workingKey1 = generateWorkingKey(encrypting, key1);
				workingKey2 = generateWorkingKey(!encrypting, key2);
				workingKey3 = workingKey1;
			}
		}

		public override String getAlgorithmName()
		{
			return "DESede";
		}

		public override int getBlockSize()
		{
			return BLOCK_SIZE;
		}

		public override int processBlock(
			byte[] inBytes,
			int inOff,
			byte[] outBytes,
			int outOff)
		{
			if (workingKey1 == null)
			{
				throw new Exception("DESede engine not initialised");
			}

			if ((inOff + BLOCK_SIZE) > inBytes.Length)
			{
				throw new Exception("input buffer too short");
			}

			if ((outOff + BLOCK_SIZE) > outBytes.Length)
			{
				throw new Exception("output buffer too short");
			}

			if (forEncryption)
			{
				desFunc(workingKey1, inBytes, inOff, outBytes, outOff);
				desFunc(workingKey2, outBytes, outOff, outBytes, outOff);
				desFunc(workingKey3, outBytes, outOff, outBytes, outOff);
			}
			else
			{
				desFunc(workingKey3, inBytes, inOff, outBytes, outOff);
				desFunc(workingKey2, outBytes, outOff, outBytes, outOff);
				desFunc(workingKey1, outBytes, outOff, outBytes, outOff);
			}

			return BLOCK_SIZE;
		}

		public override void reset()
		{
		}
	}

}