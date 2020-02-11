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
using System.Threading;

namespace Maverick.SSH.Packets
{
	/// <summary>
	/// Maintains a list of active channels and routes messages by reading from a 
	/// <see cref="Maverick.SSH.Packets.SSHPacketReader"/> and routes them to either the 
	/// global message store or any one of the currently active channels.
	/// </summary>
	public abstract class SSHPacketRouter
	{
		internal SSHAbstractChannel[] channels;
		internal SSHPacketReader reader;
		internal SSHPacketStore global;		
		internal ThreadSynchronizer sync;
		internal int count = 0;
		internal bool running = false;
		internal Thread thread;
		internal bool buffered;
		internal bool isClosing = false;
		internal Exception lastError = null;

		/// <summary>
		/// Create a router
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="maxChannels"></param>
		/// <param name="buffered"></param>
		public SSHPacketRouter(SSHPacketReader reader, int maxChannels, bool buffered)
		{
			this.reader = reader;
			this.channels = new SSHAbstractChannel[maxChannels];
			this.global = new SSHPacketStore(this, null, "Global");
			this.sync = new ThreadSynchronizer(buffered);
			this.buffered = buffered;
			
		}

		/// <summary>
		/// Indicates whether this instance of the router is buffering messages to channels. i.e. is threaded.
		/// </summary>
		public bool IsBuffered
		{
			get
			{
				return buffered;
			}
		}

		/// <summary>
		/// This method checks and returns whether the thread passed is the thread performing the 
		/// buffering.
		/// </summary>
		/// <param name="thread"></param>
		/// <returns></returns>
		public bool IsBufferingThread(Thread thread) 
		{
			return this.thread==null ? false : this.thread.Equals(thread);
		}

		/// <summary>
		/// Tell the packet router that the connection is closing. This allows the 
		/// buffering thread to ignore disconnection exceptions.
		/// </summary>
		public void SignalClosingState() 
		{
			this.isClosing = true;
		}

		/// <summary>
		/// Start routing messages
		/// </summary>
		public void Start()
		{
			if(buffered)
			{
				thread = new Thread(new ThreadStart(this.Run));
                thread.IsBackground = true;
				thread.Start();
			}
		}

		/// <summary>
		/// Stop routing messages
		/// </summary>
		public void Stop()
		{
			running = false;
		}

		/// <summary>
		/// Get the global message store
		/// </summary>
		public SSHPacketStore GlobalMessages
		{
			get
			{
				return global;
			}
		}

		/// <summary>
		/// Allocate a new channel
		/// </summary>
		/// <param name="channel"></param>
		/// <returns></returns>
		protected internal virtual int AllocateChannel(SSHAbstractChannel channel)
		{
			
			lock (channels)
			{
				for (int i = 0; i < channels.Length; i++)
				{
					if (channels[i] == null)
					{
						channels[i] = channel;
						count++;
						return i;
					}
				}
				return - 1;
			}
		}
		
		/// <summary>
		/// Free an existing channel
		/// </summary>
		/// <param name="channel"></param>
		protected internal virtual void  FreeChannel(SSHAbstractChannel channel)
		{
			lock (channels)
			{
				channels[channel.ChannelID] = null;
				count--;
			}
		}
		
		/// <summary>
		/// Returns the maximum number of channels this router supports.
		/// </summary>
		protected internal int MaximumNumChannels
		{
			get
			{
				return channels.Length;
			}
		}
		
		
		/// <summary>
		/// Get the next message from the <see cref="Maverick.SSH.Packets.SSHPacketRouter"/> that matches one of the
		/// ids supplied in the message filter and return its index in the channels message store.
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="observer"></param>
		/// <returns></returns>
		protected internal SSHPacket NextMessage(SSHAbstractChannel channel, PacketObserver observer)
		{
			
			SSHPacketStore store = channel == null? global : channel.MessageStore;
			
			PacketHolder holder = new PacketHolder();
			
			while (!store.Closed && holder.msg == null)
			{

				// Check for an error from the buffering thread
				if(buffered) 
				{
					if(!isClosing) 
					{
						if (lastError != null) 
						{
							if (lastError is SSHException)
								throw lastError;
							else
								throw new SSHException(lastError);
						}
					}
               }
           

				if (sync.RequestBlock(store, observer, holder))
				{
					
					try
					{
						BlockForMessage();
					}
					finally
					{
						// Release the block so that other threads may block or return with the
						// newly arrived message
						sync.ReleaseBlock();
					}
					
				}
				
			}
			
			return holder.msg;
		}

		private void Run()
		{


				running = true;


				while(running)
				{
					try 
					{
						BlockForMessage();

						// We have a message so release waiting threads
						sync.ReleaseWaiting();
					}
					catch(Exception ex)
					{
						Stop();
						if(!isClosing)
						{
#if DEBUG
							System.Diagnostics.Trace.WriteLine("Buffering thread failed: " + ex.Message );
#endif
							this.lastError = ex;
						}
						
					}
				}

				
			sync.ReleaseBlock();

		}

		private bool BlockForMessage()
		{

			SSHPacket packet = reader.NextMessage();
			
#if DEBUG 
			System.Diagnostics.Trace.WriteLine("Received message id " + packet.MessageID );
#endif
					
			if(IsChannelMessage(packet.MessageID))
				packet = new SSHChannelMessage(packet);
						
											
			// Determine the destination channel (if any)
			SSHAbstractChannel destination = null;
			if (packet is SSHChannelMessage)
			{
				destination = channels[((SSHChannelMessage) packet).ChannelID];
			}
						
			// Call the destination so that they may process the message
			bool processed = destination == null 
				? ProcessGlobalMessage(packet)
				: destination.ProcessChannelMessage((SSHChannelMessage) packet);
						
				// If the previous call did not process the message then add to the
				// destinations message store
				if (!processed)
				{
						SSHPacketStore ms = destination == null? global : destination.MessageStore;
//#if DEBUG
//						System.Diagnostics.Trace.WriteLine("Adding message " + packet.MessageID + " to store " + ms.ToString() );
//#endif

						ms.AddMessage(packet);
				}


			return !processed;

		}
		
		/// <summary>
		/// Is the message id a channel message?
		/// </summary>
		/// <param name="messageid"></param>
		/// <returns></returns>
		protected internal abstract bool IsChannelMessage(int messageid);
		
		/// <summary>
		/// Process a global message.
		/// </summary>
		/// <param name="packet"></param>
		/// <returns></returns>
		protected internal abstract bool ProcessGlobalMessage(SSHPacket packet);
	}
}
