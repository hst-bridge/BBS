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
using Maverick.SSH;
using System.Security.Cryptography;
using Maverick.Crypto.Ciphers;

namespace Maverick.SSH2.Algorithms
{
	/// <summary>
	/// Summary description for TripleDesCipher.
	/// </summary>
	public class TripleDesCipher : CBCBlockCipher
	{

		/// <summary>
		/// 
		/// </summary>
		public TripleDesCipher() : base(192, new DESedeEngine())
		{

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class BlowfishCipher : CBCBlockCipher
	{
		/// <summary>
		/// 
		/// </summary>
		public BlowfishCipher() : base(128, new BlowfishEngine())
		{

		}

	}

	/// <summary>
	/// 
	/// </summary>
	public class CASTCipher : CBCBlockCipher
	{
		/// <summary>
		/// 
		/// </summary>
		public CASTCipher() : base(128, new CAST5Engine())
		{

		}

	}

	/// <summary>
	/// 
	/// </summary>
	public class AESCipher : CBCBlockCipher
	{
		/// <summary>
		/// 
		/// </summary>
		public AESCipher() : base(128, new AESFastEngine())
		{

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class AES192Cipher : CBCBlockCipher
	{
		/// <summary>
		/// 
		/// </summary>
		public AES192Cipher() : base(192, new AESFastEngine())
		{

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class AES256Cipher : CBCBlockCipher
	{
		/// <summary>
		/// 
		/// </summary>
		public AES256Cipher() : base(256, new AESFastEngine())
		{

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class SerpentCipher : CBCBlockCipher
	{
		/// <summary>
		/// 
		/// </summary>
		public SerpentCipher() : base(128, new SerpentEngine())
		{

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class Serpent192Cipher : CBCBlockCipher
	{
		/// <summary>
		/// 
		/// </summary>
		public Serpent192Cipher() : base(192, new SerpentEngine())
		{

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class Serpent256Cipher : CBCBlockCipher
	{
		/// <summary>
		/// 
		/// </summary>
		public Serpent256Cipher() : base(256, new SerpentEngine())
		{

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class TwofishCipher : CBCBlockCipher
	{
		/// <summary>
		/// 
		/// </summary>
		public TwofishCipher() : base(128, new TwofishEngine())
		{

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class Twofish192Cipher : CBCBlockCipher
	{
		/// <summary>
		/// 
		/// </summary>
		public Twofish192Cipher() : base(192, new TwofishEngine())
		{

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class Twofish256Cipher : CBCBlockCipher
	{
		/// <summary>
		/// 
		/// </summary>
		public Twofish256Cipher() : base(256, new TwofishEngine())
		{

		}
	}


}
