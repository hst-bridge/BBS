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
using System.IO;
using Maverick.Crypto.IO;

namespace Maverick.SSH.Packets
{
	/// <summary>
	/// Adds useful operations to a byte buffer to enable quick and easy 
	/// creation of SSH packets.
	/// </summary>
	public abstract class SSHPacket : ByteBuffer
	{
		SSHPacket next = null;
		SSHPacket previous = null;
		//int payloadLength;

		/// <summary>
		/// Returns the id of the message
		/// </summary>
		public abstract int MessageID
		{
			get;
		}

		/// <summary>
		/// Returns the payload of the message.
		/// </summary>
		public abstract byte[] Payload
		{
			get;
		}

		/// <summary>
		/// Get the next message in the list
		/// </summary>
		public SSHPacket Next
		{
			get
			{
				return next;
			}

			set
			{
				next = value;
			}
		}

		/// <summary>
		/// Get the previous message in the list
		/// </summary>
		public SSHPacket Previous
		{
			get
			{
				return previous;
			}

			set
			{
				previous = value;
			}
		}

		/// <summary>
		/// Create an SSH packet with a default buffer of 4096 bytes
		/// </summary>
		public SSHPacket() : base(4096)
		{
			
		}

		/// <summary>
		/// Create an SSH packet with the specified buffer size
		/// </summary>
		/// <param name="size"></param>
		public SSHPacket(int size) : base(size)
		{
		}

		/// <summary>
		/// Create an SSH packet from an existing packet.
		/// </summary>
		/// <param name="packet"></param>
		public SSHPacket(SSHPacket packet) : base(packet)
		{
		}

		/// <summary>
		/// Create a packet from an existing byte array.
		/// </summary>
		/// <param name="buf"></param>
		public SSHPacket(byte[] buf) : base(buf)
		{
		}

	}
}
