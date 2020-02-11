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
using Maverick.SSH.Packets;

namespace Maverick.SSH2
{
	/// <summary>
	/// This class implements an SSH2 packet structure.
	/// </summary>
	internal class SSH2Packet : SSHPacket
	{

		public override int MessageID
		{
			get
			{
				return buf[5];
			}
		}

		internal SSH2Packet(int size) : base(size)
		{

		}

		internal SSH2Packet() : base(4096)
		{

		}

		public override byte[] Payload
		{
			get
			{
				int length = (int)ReadUINT32(buf, 0);
				int padding = buf[4];

				int count = length - padding - 1;

				byte[] tmp = new byte[count];
				System.Array.Copy(buf, 5, tmp, 0, count);

				return tmp;
			}
		}
	}
}
