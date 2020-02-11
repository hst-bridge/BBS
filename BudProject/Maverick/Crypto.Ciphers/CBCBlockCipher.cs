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
	/// <summary>
	/// Summary description for CBCBlockCipher.
	/// </summary>
	public abstract class CBCBlockCipher : Cipher
	{
		protected internal CipherEngine engine;
		protected internal int mode;
		protected internal byte[] ivStart = null;
		protected internal byte[] ivBlock = null;
		protected internal byte[] xorBlock = null;
		protected internal byte[] key = null;


		public CBCBlockCipher(int keybits, CipherEngine engine) {
			key = new byte[keybits / 8];
			this.engine = engine;
		}

		public override void Init(int mode, byte[] iv, byte[] keydata) {
			this.mode = mode;
			// Copy keydata into the correct size key
			System.Array.Copy(keydata, 0, key, 0, key.Length);
			// Initiate the engine
			engine.init(mode == ENCRYPT_MODE, key);
			// Setup the IV
			ivStart = new byte[engine.getBlockSize()];
			System.Array.Copy(iv, 0, ivStart, 0, ivStart.Length);
			ivBlock = new byte[engine.getBlockSize()];
			System.Array.Copy(ivStart, 0, ivBlock, 0, ivBlock.Length);
			xorBlock = new byte[engine.getBlockSize()];
		}

		public override int BlockSize
		{
			get
			{
				return engine.getBlockSize();
			}
		}

		public override void Transform(byte[] input, int start, byte[] output, int offset,
								int len)
		{
			if (ivBlock == null) {
				throw new Exception("Cipher not initialized!");
			}

			if ( (len % engine.getBlockSize()) != 0) {
				throw new Exception(
					"Input data length MUST be a multiple of the cipher block size!");
			}

			for (int pos = 0; pos < len; pos += engine.getBlockSize()) {
				switch (mode) {
					case ENCRYPT_MODE: {
					for (int i = 0; i < engine.getBlockSize(); i++) {
						xorBlock[i] = (byte) (input[start + pos + i] ^ ivBlock[i]);
					}
					engine.processBlock(xorBlock, 0, ivBlock, 0);
					System.Array.Copy(ivBlock, 0, output, offset + pos, engine.getBlockSize());
					break;
					}
					case DECRYPT_MODE: {
					byte[] ivTmp = new byte[engine.getBlockSize()];
					System.Array.Copy(input, start + pos, ivTmp, 0, engine.getBlockSize());
					engine.processBlock(input, offset + pos, xorBlock, 0);
					for (int i = 0; i < engine.getBlockSize(); i++) {
						output[offset + i + pos] = (byte) (xorBlock[i] ^ ivBlock[i]);
					}
					System.Array.Copy(ivTmp, 0, ivBlock, 0, engine.getBlockSize());
					ivTmp = null;
					break;
					}
					default:
						throw new Exception("Invalid cipher mode!");
				}
			}
			//return output;
		}
	}
}
