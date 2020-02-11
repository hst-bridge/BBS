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

namespace Maverick.SSH.Packets
{
	/// <summary>
	/// Wraps an SSH packet to extract channel information.
	/// </summary>
	public class SSHChannelMessage : SSHPacket
	{
		int channelid;
		SSHPacket packet;

		/// <summary>
		/// Contruct the channel message.
		/// </summary>
		/// <param name="packet"></param>
		public SSHChannelMessage(SSHPacket packet) : base(packet)
		{
			this.channelid = (int)ReadUINT32();
			this.packet = packet;
		}

		/// <summary>
		/// The id of the destination channel.
		/// </summary>
		public int ChannelID
		{
			get
			{
				return channelid;
			}
		}

		/// <summary>
		/// The channel message id
		/// </summary>
		public override int MessageID
		{
			get
			{
				return packet.MessageID;
			}
		}
		
		/// <summary>
		/// Returns the packets payload.
		/// </summary>
		public override byte[] Payload
		{
			get
			{
				return packet.Payload;
			}
		}

	}
}
