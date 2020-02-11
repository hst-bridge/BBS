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
using System.Security.Cryptography;
using Maverick.SSH;
using System.IO;

namespace Maverick.SSH2.Algorithms
{
	/// <summary>
	/// Summary description for HmacSHA1.
	/// </summary>
	public class HmacSHA1 : SSH2Hmac
	{
        HMACSHA1 mac;
        MemoryStream currentMessage;
		/// <summary>
		/// 
		/// </summary>
		public override int MacLength
		{
			get
			{
                return 20;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public HmacSHA1()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="offset"></param>
		/// <param name="len"></param>
        public override void Update(byte[] data, int offset, int len) 
		{
            currentMessage.Write(data, offset, len);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="output"></param>
		/// <param name="offset"></param>
		public override void DoFinal(byte[] output, int offset) 
		{
            currentMessage.Position = 0;
            System.Array.Copy( mac.ComputeHash(currentMessage), 0, output, offset, MacLength);
            currentMessage.SetLength(0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="keydata"></param>
		public override void Init(byte[] keydata) 
		{
            
			byte[] actualKey = new byte[MacLength];
			System.Array.Copy(keydata,
				0,
				actualKey,
				0, 
				actualKey.Length);

            this.mac = new HMACSHA1(actualKey);

            this.currentMessage = new MemoryStream();

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class HmacMD5 : SSH2Hmac
	{
		HMACMD5 mac;
        MemoryStream currentMessage;

		/// <summary>
		/// 
		/// </summary>
		public override int MacLength
		{
			get
			{
                return 16;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public HmacMD5()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="offset"></param>
		/// <param name="len"></param>
		public override void Update(byte[] data, int offset, int len) 
		{
			currentMessage.Write(data, offset, len);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="output"></param>
		/// <param name="offset"></param>
		public override void DoFinal(byte[] output, int offset) 
		{
            System.Array.Copy(mac.ComputeHash(currentMessage), 0, output, offset, MacLength);
            currentMessage.SetLength(0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="keydata"></param>
		public override void Init(byte[] keydata) 
		{
            byte[] actualKey = new byte[MacLength];
            System.Array.Copy(keydata,
                0,
                actualKey,
                0,
                actualKey.Length);

            this.mac = new HMACMD5(actualKey);

            this.currentMessage = new MemoryStream(); ;

		}
	}
}
