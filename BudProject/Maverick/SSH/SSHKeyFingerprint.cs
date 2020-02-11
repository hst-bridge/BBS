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
using Maverick.Crypto.Digests;
using Maverick.Crypto.Util;
using System.Security.Cryptography;

namespace Maverick.SSH
{
	/// <summary>
	/// Utility methods to generate an SSH public key fingerprint. 
	/// </summary>
	public class SSHKeyFingerprint
	{

		internal static char[] HEX = new char[]{'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'};
		
		/// <summary>
		/// Generate an SSH key fingerprint as defined in draft-ietf-secsh-fingerprint-00.txt. 
		/// </summary>
		/// <param name="key"></param>
		/// <returns>the key fingerprint, for example "c1:b1:30:29:d7:b8:de:6c:97:77:10:d7:46:41:63:87"
		/// </returns>
		public static System.String GenerateFingerprint(SSHPublicKey key)
		{
			
			Hash md5 = new Hash(new MD5CryptoServiceProvider());
			
			byte[] encoded = key.GetEncoded();

			md5.WriteBytes(encoded);
			
			byte[] digest = md5.DoFinal();
			
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			int ch;
			for (int i = 0; i < digest.Length; i++)
			{
				ch = digest[i];
				if (i > 0)
				{
					buf.Append(':');
				}
				buf.Append(HEX[(SupportClass.URShift(ch, 4)) & 0x0F]);
				buf.Append(HEX[ch & 0x0F]);
			}
			
			return buf.ToString();
		}
	}
}
