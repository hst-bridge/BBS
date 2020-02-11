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
	/// An abstract channel which provides basic framework operations for both SSH protocols.
	/// </summary>
	public abstract class SSHAbstractChannel : SSHChannel
	{

		private Object attachment = null;
		private SSHContext context;


		/// <summary>
		/// The channel is uninitialized.
		/// </summary>
		public const int CHANNEL_UNINITIALIZED = 1;

		/// <summary>
		/// The channel is open.
		/// </summary>
		public const int CHANNEL_OPEN = 2;

		/// <summary>
		/// The channel is closed.
		/// </summary>
		public const int CHANNEL_CLOSED = 3;
		
		/// <summary>
		/// The channels id
		/// </summary>
		protected internal int channelid = - 1;

		/// <summary>
		/// The current state of the channel
		/// </summary>
		protected internal int state;

		/// <summary>
		/// The router from which to obtain messages
		/// </summary>
		protected internal SSHPacketRouter manager;

		/// <summary>
		/// The channes packet store
		/// </summary>
		protected internal SSHPacketStore ms;

		/// <summary>
		/// Flag to indicate whether this channel consumes it's own input.
		/// </summary>
		protected internal bool autoConsumeInput = false;

		/// <summary>
		/// Construct a channel
		/// </summary>
		public SSHAbstractChannel()
		{
			state = CHANNEL_UNINITIALIZED;
		}

		/// <summary>
		/// Provides channel state change events.
		/// </summary>
		public event ChannelStateListener StateChange;

		/// <summary>
		/// Provides notification of when data is received by the channel
		/// </summary>
		public event DataListener InputStreamListener;

		/// <summary>
		/// Provides notification of when data is sent by the channel
		/// </summary>
		public event DataListener OutputStreamListener;


		/// <summary>
		/// Allows an object to be attached to the channel.
		/// </summary>
		public Object Attachment
		{
			get
			{
				return attachment;
			}
		
			set
			{
				attachment = value;
			}
		}

		/// <summary>
		/// Get the connections context
		/// </summary>
		public SSHContext Context
		{
			get
			{
				return context;
			}
		}

		/// <summary>
		/// Fire a data input event
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <param name="len"></param>
		protected internal void FireInputListenerEvent(byte[] buf, int off, int len)
		{

			try
			{
				if(InputStreamListener!=null)
				{
					InputStreamListener(this, buf, off, len);
				}
			} 
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine("WARNING! Exception caught in Maverick.NET event handler: " + ex.Message );
				System.Diagnostics.Trace.WriteLine(ex.StackTrace );
			}
		}

		/// <summary>
		/// Fire a data output event
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <param name="len"></param>
		protected internal void FireOutputListenerEvent(byte[] buf, int off, int len)
		{
			try
			{
				if(OutputStreamListener!=null)
				{
					OutputStreamListener(this, buf, off, len);
				}
			} 
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine("WARNING! Exception caught in Maverick.NET event handler: " + ex.Message );
				System.Diagnostics.Trace.WriteLine(ex.StackTrace );
			}
		}

		/// <summary>
		/// Get the message store for this channel
		/// </summary>
		protected internal SSHPacketStore MessageStore
		{
			get
			{
				if (ms == null)
				{
					throw new SSHException("Channel is not initialized!", SSHException.INTERNAL_ERROR);
				}
				return ms;
			}
			
		}

		/// <summary>
		/// Instructs the channel to automatically consume any input received.
		/// <remarks>
		/// This should be used when relying on events only to receive data. If you do not
		/// set a channel to automatically consume its input, the channel's stream will 
		/// fill and cause a deadlock on the channel. This is because in order to receive
		/// more data the channels buffer must be emptied.
		/// </remarks>
		/// </summary>
		public bool AutoConsumeInput
		{
			get
			{
				return autoConsumeInput;
			}

			set
			{
				this.autoConsumeInput = value;
			}
		}

		/// <summary>
		/// Fire a state change event
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="state"></param>
		protected internal void FireEvent(SSHChannel channel, ChannelState state)
		{

			try
			{
				if(StateChange!=null) 
				{
#if DEBUG
					switch(state)
					{
						case ChannelState.OPEN:
						{
							System.Diagnostics.Trace.WriteLine("Firing OPEN event for " + channelid );
							break;
						}
						case ChannelState.LOCAL_EOF:
						{
							System.Diagnostics.Trace.WriteLine("Firing LOCAL_EOF event for " + channelid );
							break;
						}
						case ChannelState.REMOTE_EOF:
						{
							System.Diagnostics.Trace.WriteLine("Firing REMOTE_EOF event for " + channelid );
							break;
						}
						case ChannelState.CLOSED:
						{
							System.Diagnostics.Trace.WriteLine("Firing CLOSED event for " + channelid );
							break;
						}



					}
				
#endif
					StateChange(channel, state);
				}
			} 
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine("WARNING! Exception caught in Maverick.NET event handler: " + ex.Message );
				System.Diagnostics.Trace.WriteLine(ex.StackTrace );
			}
			
		}

		/// <summary>
		/// Returns the channels id.
		/// </summary>
		public int ChannelID
		{
			get
			{
				return channelid;
			}
			
		}



		/// <summary>
		/// Determine whether the channel is closed.
		/// </summary>
		public bool IsClosed
		{
			get
			{
				return state == CHANNEL_CLOSED;
			}
			
		}

		/// <summary>
		/// Initialize the channel.
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="channelid"></param>
		/// <param name="context"></param>
		protected internal virtual void Init(SSHPacketRouter manager, int channelid, SSHContext context)
		{
			this.channelid = channelid;
			this.manager = manager;
			this.ms = new SSHPacketStore(manager, this, "Channel " + channelid);
			this.context = context;
			FireEvent(this, ChannelState.INITIALIZED);
		}
		
		/// <summary>
		/// Process a channel message.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		protected internal abstract bool ProcessChannelMessage(SSHChannelMessage m);

		/// <summary>
		/// Get this channels Stream.
		/// </summary>
		/// <returns></returns>
		public abstract System.IO.Stream GetStream();

		/// <summary>
		/// Send a close message to the remote side without waiting for a response.
		/// </summary>
		public abstract void SendAsyncClose();

		/// <summary>
		/// Close the channel.
		/// </summary>
		public abstract void  Close();

		/// <summary>
		/// An array of message id's that should not be removed from the message store when read.
		/// </summary>
		protected abstract internal PacketObserver StickyMessageIDs
		{
			get;
		}
	
	}
}
