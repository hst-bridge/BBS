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
	/// Stores SSH packets
	/// </summary>
	public class SSHPacketStore
	{

		internal SSHAbstractChannel channel;
		internal SSHPacketRouter manager;
		internal bool closed = false;
		internal SSHPacket header;
		internal int size = 0;

		String name;
		/// <summary>
		/// Create a new packet store for a given channel.
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="channel"></param>
		/// <param name="name"></param>
		public SSHPacketStore(SSHPacketRouter manager, SSHAbstractChannel channel, String name)
		{
			this.manager = manager;
			this.channel = channel;
			this.name = name;
			this.header = new HeaderPacket(); // Dummy, never used
			header.Next = header.Previous = header;
		}


		/// <summary>
		/// Returns the name of the store.
		/// </summary>
		/// <returns></returns>
		public override String ToString() 
		{
			return name;
		}

		/// <summary>
		/// Determine whether the store is closed.
		/// </summary>
		public bool Closed
		{
			get
			{
				lock (header)
				{
					return closed;
				}
			}
		}

		/// <summary>
		/// Get the next packet from the store that matches an id in the filter. If no id exists this 
		/// method will block until a suitable message is received or the store is closed.
		/// </summary>
		/// <param name="observer"></param>
		/// <returns></returns>
		public SSHPacket NextMessage(PacketObserver observer)
		{
			
			try
			{
				SSHPacket msg = manager.NextMessage(channel, observer);
				
				if (msg != null)
				{
					lock (header)
					{
						if(channel==null || !channel.StickyMessageIDs.WantsNotification(msg))
						{
							Remove(msg);
						}
						return msg;
					}
				}
			}
			catch (System.Threading.ThreadInterruptedException)
			{
				throw new SSHException("The thread was interrupted", SSHException.INTERNAL_ERROR);
			}
			
			throw new System.IO.EndOfStreamException("The required message could not be found in the message store");
		}


		
		internal void Remove(SSHPacket e)
		{
			lock(header)
			{
				if (e == header)
				{
					throw new System.IndexOutOfRangeException();
				}
			
				e.Previous.Next = e.Next;
				e.Next.Previous = e.Previous;
				size--;
			}
		}
		
		internal SSHPacket HasMessage(PacketObserver observer)
		{
			
			lock (header)
			{
				
				if (observer == null)
				{
					throw new System.ArgumentException("Message filter cannot be NULL!");
				}
				
				if (header.Next == null)
				{
					return null;
				}
				
				for (SSHPacket e = header.Next; e != header; e = e.Next)
				{
//#if DEBUG
//					System.Diagnostics.Trace.WriteLine("Checking message id " + e.MessageID );
//#endif

					if (observer.WantsNotification(e))
					{
						return e;
					}
				}
				
				return null;
			}
		}
		
		/// <summary>
		/// Close the packet store.
		/// </summary>
		public void  Close()
		{
			
			lock (header)
			{
				closed = true;
			}
		}
		
		internal void  AddMessage(SSHPacket msg)
		{
			lock(header)
			{
				msg.Next = header;
				msg.Previous = header.Previous;
				msg.Previous.Next = msg;
				msg.Next.Previous = msg;
				size++;
			}
		}

		class HeaderPacket : SSHPacket
		{
			protected internal HeaderPacket() : base(32)
			{

			}

			public override int MessageID
			{
				get
				{
					return 0;
				}
			}

			public override byte[] Payload
			{
				get
				{
					return null;
				}
			}

		}
	}
}
