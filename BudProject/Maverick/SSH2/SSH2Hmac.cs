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

namespace Maverick.SSH2
{
	/// <summary>
	/// This class provides an abstract base for all SSH2 MAC implementations.
	/// </summary>
	public abstract class SSH2Hmac
	{

		/// <summary>
		/// The length in bytes of the MAC
		/// </summary>
		public abstract int MacLength
		{
			get;
		}
		
		/// <summary>
		/// Construct an uninitialized MAC.
		/// </summary>
		public SSH2Hmac()
		{
		}
		
		/// <summary>
		/// Generate the MAC.
		/// </summary>
		/// <param name="sequenceNo">The sequence number of the message.</param>
		/// <param name="data">The message data array.</param>
		/// <param name="offset">The offset in the array at which the data starts.</param>
		/// <param name="len">The length of the data</param>
		/// <param name="output">The output array</param>
		/// <param name="start">The location in the output array to write the MAC</param>
		public void  Generate(uint sequenceNo, byte[] data, int offset, int len, byte[] output, int start)
		{
			// Write the sequence no
			byte[] sequenceBytes = new byte[4];
			sequenceBytes[0] = (byte) (sequenceNo >> 24);
			sequenceBytes[1] = (byte) (sequenceNo >> 16);
			sequenceBytes[2] = (byte) (sequenceNo >> 8);
			sequenceBytes[3] = (byte) (sequenceNo >> 0);
			
			Update(sequenceBytes, 0, sequenceBytes.Length);
			
			Update(data, offset, len);
			
			DoFinal(output, start);

		}

		/// <summary>
		/// Add data to the MAC.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="offset"></param>
		/// <param name="len"></param>
		public abstract void Update(byte[] data, int offset, int len);

		/// <summary>
		/// Create the MAC
		/// </summary>
		/// <param name="output"></param>
		/// <param name="offset"></param>
		public abstract void DoFinal(byte[] output, int offset);

		/// <summary>
		/// Initialize the MAC with key data.
		/// </summary>
		/// <param name="keydata"></param>
		public abstract void Init(byte[] keydata);
		
		/// <summary>
		/// Verify the MAC.
		/// </summary>
		/// <param name="sequenceNo"></param>
		/// <param name="data"></param>
		/// <param name="start"></param>
		/// <param name="len"></param>
		/// <param name="mac"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		public bool Verify(uint sequenceNo, byte[] data, int start, int len, byte[] mac, int offset)
		{
			
			if (data.Length < start + len)
			{
				throw new SSHException("Not enough data for message and mac!", 
					SSHException.INTERNAL_ERROR);
			}
			
			if (mac.Length - offset < MacLength)
			{
				throw new SSHException(MacLength.ToString() + " bytes of MAC data required!", 
					SSHException.INTERNAL_ERROR);
			}
			
			byte[] generated = new byte[MacLength];

			Generate(sequenceNo, data, start, len, generated, 0);
			
			for (int i = 0; i < generated.Length; i++)
			{
				if (generated[i] != mac[offset + i])
				{
					return false;
				}
			}
			
			return true;
		}
	}
}
