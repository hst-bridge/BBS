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

namespace Maverick.SSH
{
	/// <summary>
	/// This class provides useful methods for implementing an SSH2 subsystem.</summary>
	/// <remarks>Subsystems typically send messages in the following format.<br/>
	/// <br/>
	/// UINT           length<br/>
	/// byte           type<br/>
	/// byte[length-1] payload<br/>
	/// <br/>
	/// Messages sent using the methods of this class will have the UINT length automatically added and 
	/// messages received will be unwrapped with just the type and payload being returned. Although 
	/// subsystems were defined within the SSH2 connection protocol this class takes a single <see cref="Maverick.SSH.SSHChannel"/> 
	/// as an argument to its constructor which enables subsystems to run over both SSH1 and SSH2 channels.
	/// </remarks>
	public class SubsystemChannel
	{
		/// <summary>
		/// The underlying channel.
		/// </summary>
		SSHChannel channel;

		/// <summary>
		/// Determines if the subsystem has been closed (tied to the channel)
		/// </summary>
		public bool IsClosed
		{
			get
			{
				return channel.IsClosed;
			}
		}

		/// <summary>
		/// Create a new subsystem channel
		/// </summary>
		/// <param name="channel"></param>
		public SubsystemChannel(SSHChannel channel)
		{
			this.channel = channel;
		}

		/// <summary>
		/// Create a new buffer for an outgoing message
		/// </summary>
		/// <returns></returns>
		public ByteBuffer CreateMessage()
		{
			ByteBuffer msg = new ByteBuffer();
			msg.Skip(4);

			return msg;
		}

		/// <summary>
		/// Send a subsystem message.
		/// </summary>
		/// <param name="msg"></param>
		public void SendMessage(ByteBuffer msg)
		{
			try 
			{
				msg.MoveToPosition(0);
				msg.WriteUINT32(msg.Length - 4);
				msg.MoveToEnd();

				msg.WriteToStream(channel.GetStream());
			} 
			catch(EndOfStreamException)
			{
				Close();

				throw new SSHException("The channel unexpectedly terminated",
					SSHException.CHANNEL_FAILURE);
			}
			catch(IOException ex)
			{
				throw new SSHException(ex.Message,
					SSHException.CHANNEL_FAILURE);
			}
		}

		/// <summary>
		/// Close the subsystem and therefore close the channel.
		/// </summary>
		public void Close()
		{
			channel.Close();
		}

		/// <summary>
		/// Read the next subsystem message available from the channels input stream.
		/// </summary>
		/// <returns></returns>
		public ByteBuffer ReadMessage()
		{
			try 
			{
				ByteBuffer msg = new ByteBuffer();
				msg.ReadFromStream(channel.GetStream(), 4);
				msg.MoveToPosition(0);
				int len = (int) msg.ReadUINT32();
				msg.Mark();
				msg.ReadFromStream(channel.GetStream(), len);
				msg.MoveToMark();
				return msg;
			} 
			catch(EndOfStreamException)
			{
				Close();

				throw new SSHException("The channel unexpectedly terminated", 
					SSHException.CHANNEL_FAILURE);
			}
			catch(IOException ex)
			{
				throw new SSHException(ex.Message,
					SSHException.CHANNEL_FAILURE);
			}

		}
	}
}
