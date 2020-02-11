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
using Maverick.SSH;
using Maverick.SSH.Packets;
using System.Threading;

namespace Maverick.SSH2
{
	/// <summary>
	/// Implementation of an SSH2 <see cref="Maverick.SSH.SSHChannel"/>.
	/// </summary>
	/// <remarks>
	/// All terminal sessions, forwarded connections, etc are channels and this class implements the base 
	/// SSH2 channel. Either side may open a channel and multiple channels are multiplexed into a single 
	/// SSH connection. SSH2 channels are flow controlled, no data may be sent to a channel until a message 
	/// is received to indicate that window space is available.
	/// </remarks>
	public class SSH2Channel : SSHAbstractChannel
	{
		/// <summary>
		/// The identifier for session channels "session"
		/// </summary>
		public const System.String SESSION_CHANNEL = "session";
		
		internal ConnectionProtocol connection;
		internal int remoteid;
		internal String name;
		
		internal const int SSH_MSG_CHANNEL_OPEN_CONFIRMATION = 91;
		internal const int SSH_MSG_CHANNEL_OPEN_FAILURE = 92;
				
		internal const int SSH_MSG_CHANNEL_CLOSE = 97;
		internal const int SSH_MSG_CHANNEL_EOF = 96;
		internal const int SSH_MSG_CHANNEL_REQUEST = 98;
		
		internal const int SSH_MSG_CHANNEL_SUCCESS = 99;
		internal const int SSH_MSG_CHANNEL_FAILURE = 100;
		
		internal const int SSH_MSG_WINDOW_ADJUST = 93;
		internal const int SSH_MSG_CHANNEL_DATA = 94;
		internal const int SSH_MSG_CHANNEL_EXTENDED_DATA = 95;
		
		
		internal class WindowAdjustPacketObserver : PacketObserver 
		{
			public bool WantsNotification(SSHPacket msg) 
			{
				switch(msg.MessageID)
				{
					case SSH_MSG_WINDOW_ADJUST:
					case SSH_MSG_CHANNEL_EOF:
					case SSH_MSG_CHANNEL_CLOSE:
						return true;
					default:
						return false;
				}
			}
		}

		/// <summary>
		/// Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		static internal WindowAdjustPacketObserver WINDOW_ADJUST_MESSAGES = new WindowAdjustPacketObserver();

		internal class ChannelDataPacketObserver : PacketObserver 
		{
			public bool WantsNotification(SSHPacket msg) 
			{
				switch(msg.MessageID)
				{
					case SSH_MSG_CHANNEL_DATA:
					case SSH_MSG_CHANNEL_EOF:
					case SSH_MSG_CHANNEL_CLOSE:
						return true;
					default:
						return false;
				}
			}
		}

		internal ChannelDataPacketObserver CHANNEL_DATA_MESSAGES = new ChannelDataPacketObserver();

		internal class ExtendedDataPacketObserver : PacketObserver 
		{
			public bool WantsNotification(SSHPacket msg) 
			{
				switch(msg.MessageID)
				{
					case SSH_MSG_CHANNEL_EXTENDED_DATA:
					case SSH_MSG_CHANNEL_EOF:
					case SSH_MSG_CHANNEL_CLOSE:
						return true;
					default:
						return false;
				}
			}
		}

		internal ExtendedDataPacketObserver EXTENDED_DATA_MESSAGES = new ExtendedDataPacketObserver();

		internal class ChannelRequestPacketObserver : PacketObserver 
		{
			public bool WantsNotification(SSHPacket msg) 
			{
				switch(msg.MessageID)
				{
					case SSH_MSG_CHANNEL_SUCCESS:
					case SSH_MSG_CHANNEL_FAILURE:
					case SSH_MSG_CHANNEL_CLOSE:
						return true;
					default:
						return false;
				}
			}
		}

		internal ChannelRequestPacketObserver CHANNEL_REQUEST_MESSAGES = new ChannelRequestPacketObserver();
	
		internal class ChannelClosePacketObserver : PacketObserver 
		{
			public bool WantsNotification(SSHPacket msg) 
			{
				switch(msg.MessageID)
				{
					case SSH_MSG_CHANNEL_CLOSE:
						return true;
					default:
						return false;
				}
			}
		}

		internal ChannelClosePacketObserver CHANNEL_CLOSE_MESSAGES = 
			new ChannelClosePacketObserver();

		internal SSH2ChannelStream stream;
		
		internal DataWindow localwindow;
		internal DataWindow remotewindow;
		
		internal bool closing = false;
        internal bool remoteClosed;

		internal class StickyPacketObserver : PacketObserver
		{
			public bool WantsNotification(SSHPacket msg) 
			{
				switch(msg.MessageID) 
				{
					case SSH_MSG_CHANNEL_CLOSE:
					case SSH_MSG_CHANNEL_EOF:
						return true;
					default:
						return false;
				}
			}
		}

		internal StickyPacketObserver stickyMessageIDs = new StickyPacketObserver();
		

		/// <summary>
		/// Create an uninitialized channel.
		/// </summary>
		/// <param name="name">The name of the channel, for example "session"</param>
		/// <param name="windowsize">The initial window size</param>
		/// <param name="packetsize">The maximum packet size</param>
		public SSH2Channel(String name, int windowsize, int packetsize)
		{
			this.name = name;
			this.localwindow = new DataWindow(this, windowsize, packetsize);
			this.stream = new SSH2ChannelStream(this, CHANNEL_DATA_MESSAGES); 
		}

		/// <summary>
		/// Create an uninitialized channel.
		/// </summary>
		/// <param name="name">The name of the channel, for example "session"</param>
		/// <param name="windowsize">The initial window size</param>
		/// <param name="packetsize">The maximum packet size</param>
		/// <param name="delayWindow">Allows the window space allocation to be delayed.</param>
		public SSH2Channel(String name, int windowsize, int packetsize, bool delayWindow)
			: this(name, windowsize, packetsize)
		{		
			if(delayWindow)
				localwindow.consume(localwindow.available());
		}

		/// <summary>
		/// Resumes the allocation of window space.
		/// </summary>
		/// <remarks>
		/// This method should be called to resume data flow if a channel has been created 
		/// with the delay window flag set to <tt>true</tt>.
		/// </remarks>
		public void ResumeWindow()
		{
			AdjustWindow(stream.buffer.Length);
		}

		/// <summary>
		/// Get the stream for this channel
		/// </summary>
		/// <returns></returns>
		public override Stream GetStream()
		{
			return stream;
		}



		/// <summary>
		/// An array of message ids that will remain in the message store even 
		/// after they have been read.
		/// </summary>
		protected internal override PacketObserver StickyMessageIDs
		{
			get
			{
				return stickyMessageIDs;
			}
		}

		/// <summary>
		/// The name of this channel i.e. "session"
		/// </summary>
		public System.String Name
		{
			get
			{
				return name;
			}
			
		}

		/// <summary>
		/// The window size for this channel.
		/// </summary>
		public int WindowSize
		{
			get
			{
				return localwindow.windowsize;
			}
		}

		/// <summary>
		/// The maximum packet size.
		/// </summary>
		public int PacketSize
		{
			get
			{
				return localwindow.PacketSize;
			}
		}

		internal virtual void Init(ConnectionProtocol connection, int channelid)
		{
			this.connection = connection;
			base.Init(connection, channelid, connection.Context);
		}

		/// <summary>
		/// Called after the channel has been created but before the SSH_MSG_CHANNEL_OPEN_CONFIRMATION message
		/// has been sent to the remote side. If the channel wants to return any specific request data to the 
		/// remote side of the channel it should return an array of bytes now, otherwise return null.
		/// </summary>
		/// <returns></returns>
		protected internal virtual byte[] Create()
		{
			return null;
		}

		/// <summary>
		/// Once a SSH_MSG_CHANNEL_OPEN_CONFIRMATION message is received the framework calls 
		/// this method to complete the channel open operation. 
		/// </summary>
		/// <param name="remoteid">the senders id</param>
		/// <param name="remotewindow">the initial window space available for sending data</param>
		/// <param name="remotepacket">the maximum packet size available for sending data</param>
		protected internal virtual void  Open(int remoteid, int remotewindow, int remotepacket)
		{
			this.remoteid = remoteid;
			this.remotewindow = new DataWindow(this, remotewindow, remotepacket);
			
			this.state = CHANNEL_OPEN;

			FireEvent(this, ChannelState.OPEN);
		}

		/// <summary>
		/// Once a SSH_MSG_CHANNEL_OPEN_CONFIRMATION message is received the framework calls 
		/// this method to complete the channel open operation. 
		/// </summary>
		/// <param name="remoteid">the senders id</param>
		/// <param name="remotewindow">the initial window space available for sending data</param>
		/// <param name="remotepacket">the maximum packet size available for sending data</param>
		/// <param name="responsedata">the data returned from the remote side in the SSH_MSG_CHANNEL_OPEN_CONFIRMATION message</param>
		protected internal virtual void Open(int remoteid, int remotewindow, int remotepacket, byte[] responsedata)
		{
			
			Open(remoteid, remotewindow, remotepacket);
		}

		/// <summary>
		/// Sends a close message to the remote side without waiting for a reply
		/// </summary>
		public override void SendAsyncClose()
		{
			bool performClose = false;

			lock(this)
			{
				if(!closing)
					performClose = closing = true;
			}

			if (state == CHANNEL_OPEN && performClose)
			{
			
				try
				{
					// Close the ChannelOutputStream
					stream.Close();
					
					// Send our close message
					SSHPacket packet = connection.GetPacket();
					packet.WriteByte((System.Byte) SSH_MSG_CHANNEL_CLOSE);
					packet.WriteUINT32(remoteid);
					
					try
					{
#if DEBUG 
						System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_CHANNEL_CLOSE");
						System.Diagnostics.Trace.WriteLine("Channelid=" + ChannelID );
#endif 
						connection.SendMessage(packet);
					}
					catch (SSHException)
					{
					}
				}
				catch (System.IO.EndOfStreamException)
				{
					// Ignore this is the message store informing of close/eof
				}
				catch (System.IO.IOException ex)
				{
					// IO Error during close so the connection has dropped
					connection.SignalClosingState();
					connection.transport.Disconnect("IOException during channel close: " + ex.Message,
						DisconnectionReason.CONNECTION_LOST);
				}
			
			}
		}

		/// <summary>
		/// Closes the channel
		/// </summary>
		public override void Close()
		{

			bool performClose = false;

			lock(this)
			{
				if(!closing)
					performClose = closing = true;
			}

			if (state == CHANNEL_OPEN && performClose)
			{
			
				try
				{
					// Close the ChannelOutputStream
					stream.Close();
					
					// Send our close message
					SSHPacket packet = connection.GetPacket();
					packet.WriteByte((System.Byte) SSH_MSG_CHANNEL_CLOSE);
					packet.WriteUINT32(remoteid);
					
#if DEBUG 
					System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_CHANNEL_CLOSE");
					System.Diagnostics.Trace.WriteLine("Channelid=" + ChannelID );
#endif 
					connection.SendMessage(packet);
                    
                    CheckCloseStatus(false);
				}
				catch (System.IO.EndOfStreamException)
				{
					// Ignore this is the message store informing of close/eof
				}
				catch (System.IO.IOException ex)
				{
					// IO Error during close so the connection has dropped
					connection.SignalClosingState();
					connection.transport.Disconnect("IOException during channel close: " + ex.Message,
						DisconnectionReason.CONNECTION_LOST);
				}

			}

		}

        internal void CheckCloseStatus(bool remClosed)
        {
            if (state != CHANNEL_CLOSED)
            {
                Close();
                if (!remClosed)
                    remClosed = (ms.HasMessage(CHANNEL_CLOSE_MESSAGES) != null);
            }

            if (remClosed)
            {
                connection.CloseChannel(this);
                FireEvent(this, ChannelState.CLOSED);
                this.state = CHANNEL_CLOSED;
            }
        }
		/// <summary>
		/// Fire an error stream listener event.
		/// </summary>
		/// <param name="type">The type of extended data.</param>
		/// <param name="buf">The actual data.</param>
		/// <param name="off">The offset in the array.</param>
		/// <param name="len">The lenght of the data.</param>
		/// <remarks>
		/// This method does nothing because no extended data channels are available in 
		/// a basic channel.
		/// </remarks>
		protected virtual void FireErrorStreamListenerEvent(int type, byte[] buf, int off, int len)
		{
			// Do nothing as the basic channel has no error stream types
		}

		/// <summary>
		/// Processes a channel message.
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		protected internal override bool ProcessChannelMessage(SSHChannelMessage msg)
		{
			try
			{
#if DEBUG
				LogMessage(msg);
#endif 
				switch (msg.MessageID)
				{
					
					case SSH_MSG_CHANNEL_REQUEST: 
					{
						String requesttype = msg.ReadString();
						bool wantreply = msg.ReadBool();
						byte[] requestdata = new byte[msg.Available];
						msg.ReadBytes(requestdata);
						ChannelRequest(requesttype, wantreply, requestdata);
						return true;
					}
					case SSH_MSG_CHANNEL_DATA: 
					{
						int count = (int) SSHPacket.ReadInt(msg.Payload, 5);
						if(autoConsumeInput) 
						{
							localwindow.consume(count);
							if (localwindow.available() <= stream.buffer.Length / 2 && !IsClosed) 
							{
								AdjustWindow(stream.buffer.Length - localwindow.available());
							}
						}

						// Fire the input listener event
						FireInputListenerEvent(msg.Payload, 9, count); 

						return autoConsumeInput;
					}
					case SSH_MSG_CHANNEL_EXTENDED_DATA:
					{
					    int type = (int) SSHPacket.ReadInt(msg.Payload, 5);
						int count = (int) SSHPacket.ReadInt(msg.Payload, 9);


						if (autoConsumeInput) 
						{
							localwindow.consume(count);
							if (localwindow.available() <= stream.buffer.Length / 2 && !IsClosed) 
							{
								AdjustWindow(stream.buffer.Length - localwindow.available());
							}
						}

						FireErrorStreamListenerEvent(type, msg.Payload, 13, count); 

						return autoConsumeInput;
					}			
		

					case SSH_MSG_CHANNEL_CLOSE:
					{
                        remoteClosed = true;
                        CheckCloseStatus(true);
						return false;
					}

					case SSH_MSG_CHANNEL_EOF:
					{

						FireEvent(this, ChannelState.REMOTE_EOF);
						ChannelEOF();
						return false;
					}
					default: 
						return false;
					
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

		
		internal void AdjustWindow(int increment)
		{
			
			try
			{
				SSHPacket packet = connection.GetPacket();
				packet.WriteByte((System.Byte) SSH_MSG_WINDOW_ADJUST);
				packet.WriteUINT32(remoteid);
				packet.WriteUINT32(increment);
				
				localwindow.adjust(increment);

#if DEBUG 
				System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_WINDOW_ADJUST");
				System.Diagnostics.Trace.WriteLine("Channelid=" + ChannelID );
#endif 
				connection.SendMessage(packet);
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// Sends a channel request. Many channels have extensions that are specific to that particular 
		/// channel type, an example of which is requesting a pseudo terminal from an interactive session. 
		/// </summary>
		/// <param name="requesttype">the name of the request, for example "pty-req"</param>
		/// <param name="wantreply">specifies whether the remote side should send a success/failure message</param>
		/// <param name="requestdata">the request data</param>
		/// <returns></returns>
		public bool SendRequest(System.String requesttype, bool wantreply, byte[] requestdata)
		{
			lock (this)
			{
				
				try
				{
					SSHPacket packet = connection.GetPacket();
					packet.WriteByte((System.Byte) SSH_MSG_CHANNEL_REQUEST);
					packet.WriteUINT32(remoteid);
					packet.WriteString(requesttype);
					packet.WriteBool(wantreply);
					if (requestdata != null)
					{
						packet.WriteBytes(requestdata);
					}

#if DEBUG 
					System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_CHANNEL_REQUEST " + requesttype );
					System.Diagnostics.Trace.WriteLine("Channelid=" + ChannelID );
#endif
					connection.SendMessage(packet);
					
					bool result = false;
					
					if (wantreply)
					{
						packet = ProcessMessages(CHANNEL_REQUEST_MESSAGES);
						return packet.MessageID == SSH_MSG_CHANNEL_SUCCESS;
					}
					
					return result;
				}
				catch (System.IO.IOException ex)
				{
					throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
				}
			}
		}

		/// <summary>
		/// Process the next available messages in the message store and only returns once 
		/// a message matching one of the ids in the filter array has arrived.
		/// </summary>
		/// <param name="observer"></param>
		/// <returns></returns>
		protected internal virtual SSHChannelMessage ProcessMessages(PacketObserver observer)
		{
			
			SSHChannelMessage msg;
			
			msg = (SSHChannelMessage) ms.NextMessage(observer);
			
			switch (msg.MessageID)
			{	
				
				case SSH_MSG_WINDOW_ADJUST: 
				{
#if DEBUG
					LogMessage(msg);
#endif 
					remotewindow.adjust((int)msg.ReadUINT32());
					break;
				}
				case SSH_MSG_CHANNEL_DATA: 
				{
					byte[] data = msg.ReadBinaryString();
					ProcessStandardData(data, 0, data.Length);
					break;
				}
				case SSH_MSG_CHANNEL_EXTENDED_DATA:
				{
					int type = (int)msg.ReadUINT32();
					byte[] data = msg.ReadBinaryString();
					ProcessExtendedData(type, data, 0, data.Length);
					break;
				}
				case SSH_MSG_CHANNEL_CLOSE: 
				{
                    remoteClosed = true;
                    CheckCloseStatus(true);
					throw new System.IO.EndOfStreamException("The channel is closed");
				}
				case SSH_MSG_CHANNEL_EOF: 
				{
					try
					{
						stream.CloseInput();
					}
					catch (System.IO.IOException)
					{
					}

					throw new System.IO.EndOfStreamException("The channel is EOF");
				}	
				default: 
					break;
				
			}
			
			return msg;
		}

		/// <summary>
		/// Data has arrived on the channel
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="offset"></param>
		/// <param name="len"></param>
		protected internal virtual void  ProcessStandardData(byte[] buf, int offset, int len)
		{
			stream.FillInputBuffer(buf, offset, len);
			
		}

		/// <summary>
		/// Extended data has arrived on the channel
		/// </summary>
		/// <param name="typecode"></param>
		/// <param name="buf"></param>
		/// <param name="offset"></param>
		/// <param name="len"></param>
		protected internal virtual void  ProcessExtendedData(int typecode, byte[] buf, int offset, int len)
		{
			// Default implementation is to ignore extended data
		}

		/// <summary>
		/// Create an extended data stream.
		/// </summary>
		/// <returns></returns>
		protected internal virtual SSH2ChannelStream createExtendedDataStream()
		{
			return new SSH2ChannelStream(this, EXTENDED_DATA_MESSAGES);
		}

		internal void  SendChannelData(byte[] buf, int offset, int len)
		{
			
			try
			{
				if (state != CHANNEL_OPEN)
				{
					throw new SSHException("The channel is closed", SSHException.CHANNEL_FAILURE);
				}
				
				if (len > 0)
				{
					SSHPacket packet = connection.GetPacket();
					packet.WriteByte((System.Byte) SSH_MSG_CHANNEL_DATA);
					packet.WriteUINT32(remoteid);
					packet.WriteBinaryString(buf, offset, len);
					
#if DEBUG 
					System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_CHANNEL_DATA");
					System.Diagnostics.Trace.WriteLine("Channelid=" + ChannelID );
#endif
					connection.SendMessage(packet);
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// Evaluates whether this channel is equal to another channel instance.
		/// </summary>
		/// <param name="obj">The Object instance to evaluate</param>
		/// <returns></returns>
		/// <remarks>
		/// This override checks the channel id against the evaluating instance's channel id.
		/// </remarks>
		public override bool Equals(System.Object obj)
		{
			if (obj is SSH2Channel)
			{
				return ((SSH2Channel) obj).ChannelID == channelid;
			}
			return false;
		}

		/// <summary>
		/// Process a channel request
		/// </summary>
		/// <param name="requesttype">the name of the request, for example "pty-req"</param>
		/// <param name="wantreply">specifies whether the remote side should send a success/failure message</param>
		/// <param name="requestdata">the request data</param>
		protected internal virtual void ChannelRequest(String requesttype, bool wantreply, byte[] requestdata)
		{
			if (wantreply)
			{
				SSHPacket packet = connection.GetPacket();
				packet.WriteByte(SSH_MSG_CHANNEL_FAILURE);

#if DEBUG 
				System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_CHANNEL_FAILURE");
				System.Diagnostics.Trace.WriteLine("Channelid=" + ChannelID );
#endif
				connection.SendMessage(packet);
			}
		}

		/// <summary>
		/// Called when the channel has reached EOF
		/// </summary>
		protected internal virtual void ChannelEOF()
		{
			
		}

		/// <summary>
		/// This class implements an SSH2 channel stream.
		/// </summary>
		protected internal class SSH2ChannelStream : SSHAbstractStream
		{
			internal byte[] buffer;
			internal int unread = 0;
			internal int position = 0;
			internal int currentBase = 0;
			internal bool inputEOF = false;
			internal bool outputEOF = false;
			internal PacketObserver observer;
			internal long transfered = 0;
			internal SSH2Channel channel;

			/// <summary>
			/// Construct an SSH2 channel stream.
			/// </summary>
			/// <param name="enclosingInstance">The channel that owns this stream.</param>
			/// <param name="observer">An observer to filter data messages.</param>
			protected internal SSH2ChannelStream(SSH2Channel enclosingInstance, PacketObserver observer)
				: base(enclosingInstance)
			{
				buffer = new byte[enclosingInstance.localwindow.available()];
				this.observer = observer;
				this.channel = (SSH2Channel)enclosingInstance;
			}
			
			/// <summary>
			/// Returns the number of bytes that are available to be read in the local buffer.
			/// </summary>
			/// <returns></returns>
			public override int Available()
			{
				try 
				{
					if (unread == 0) 
					{
						if (channel.ms.HasMessage(observer) != null) 
						{
							channel.ProcessMessages(observer);
						}
					}
					return unread;
				}
				catch(System.IO.EndOfStreamException)
				{
					return 0;
				}

			}

			/// <summary>
			/// Close down the input side of the stream.
			/// </summary>
			public override void CloseInput()
			{
				this.inputEOF = true;
			}

			/// <summary>
			/// Close down the output side of the stream.
			/// </summary>
			/// <remarks>
			/// This method sends an SSH_MSG_CHANNEL_EOF message to the remote side.
			/// </remarks>
			public override void CloseOutput()
			{
				if (!channel.IsClosed && !outputEOF && !channel.closing && !channel.remoteClosed)
				{
					
					SSHPacket packet = channel.connection.GetPacket();
					packet.WriteByte(SSH_MSG_CHANNEL_EOF);
					packet.WriteUINT32(channel.remoteid);
					
					try
					{
#if DEBUG 
						System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_CHANNEL_EOF");
						System.Diagnostics.Trace.WriteLine("Channelid=" + channel.ChannelID );
#endif
						channel.connection.SendMessage(packet);
					}
					finally
					{
						outputEOF = true;
						channel.FireEvent(channel, ChannelState.LOCAL_EOF);
					}
				}
				
				outputEOF = true;
			}

			/// <summary>
			/// Close the stream.
			/// </summary>
			/// <remarks>
			/// Closing the stream will actually close the channel.
			/// </remarks>
			public override void Close()
			{
				CloseOutput();
				CloseInput();
			}

//String lastline=null;
			/// <summary>
			/// Read data from the SSH stream.
			/// </summary>
			/// <param name="buf">The destination byte array.</param>
			/// <param name="offset">The offset to place the read data.</param>
			/// <param name="len">The maximum number of bytes to read.</param>
			/// <returns></returns>
			public override int Read(byte[] buf, int offset, int len)
			{
				try
				{
					
					if (unread <= 0) // Expect EndOfStreamException if channel is closed
					{
						channel.ProcessMessages(observer);
					}
					
					int count = unread < len ? unread : len;
					int index = currentBase;
					currentBase = (currentBase + count) % buffer.Length;
					if (buffer.Length - index > count)
					{
						Array.Copy(buffer, index, buf, offset, count);
					}
					else
					{
						int remaining = buffer.Length - index;
						Array.Copy(buffer, index, buf, offset, remaining);
						Array.Copy(buffer, 0, buf, offset + remaining, count - remaining);
					}
					
					unread -= count;
					
					if ((unread + channel.localwindow.available()) < (buffer.Length / 2) && !channel.IsClosed)
					{

						channel.AdjustWindow(buffer.Length - channel.localwindow.available() - unread);
					}
					
					transfered += count;					
					return count;
				}
				catch (System.IO.EndOfStreamException)
				{
					return 0;
				}
			}

			/// <summary>
			/// Write data to the SSH stream.
			/// </summary>
			/// <param name="buf">The destination byte array.</param>
			/// <param name="offset">The offset to start taking data for writing.</param>
			/// <param name="len">The number of bytes to write.</param>
			public override void Write(byte[] buf, int offset, int len)
			{

					int write;
					
					do 
					{
						
						if (outputEOF)
						{
							throw new SSHException("The channel is EOF", SSHException.CHANNEL_FAILURE);
						}
						
						if (channel.IsClosed)
						{
							throw new SSHException("The channel is closed!", SSHException.CHANNEL_FAILURE);
						}

						if (channel.remotewindow.available() <= 0)
						{
							SSHPacket packet = channel.ProcessMessages(WINDOW_ADJUST_MESSAGES);

						}
											
						write = channel.remotewindow.available() < channel.remotewindow.PacketSize 
							  ? (channel.remotewindow.available() < len 
									? channel.remotewindow.available():len)
									: (channel.remotewindow.PacketSize < len?channel.remotewindow.PacketSize:len);
						
						if (write > 0)
						{
														
							channel.SendChannelData(buf, offset, write);
							channel.remotewindow.consume(write);
							len -= write;
							offset += write;
						}
					}
					while (len > 0);

					channel.FireOutputListenerEvent(buf, offset, len);
			}

			/// <summary>
			/// Fills the input buffer with data.
			/// </summary>
			/// <param name="buf"></param>
			/// <param name="offset"></param>
			/// <param name="len"></param>
			protected internal void FillInputBuffer(byte[] buf, int offset, int len)
			{
				if (channel.localwindow.available() < len)
				{
					channel.connection.SignalClosingState();
					channel.connection.transport.Disconnect("Received data exceeding current window space",
						DisconnectionReason.PROTOCOL_ERROR);
					throw new SSHException("Window space exceeded", SSHException.PROTOCOL_VIOLATION);
				}

			
				int i = 0;
				int index;
				int count;
				while (i < len)
				{
					// Copy data up to the end of the array and start back
					// at the beginning
					index = (currentBase + unread) % buffer.Length;
					count = ((buffer.Length - index < len - i)?buffer.Length - index:len - i);
					Array.Copy(buf, offset + i, buffer, index, count);
					unread += count;
					i += count;
				}
				
				channel.localwindow.consume(len);

				channel.FireInputListenerEvent(buf, offset, len);
			}

		}


		internal class DataWindow
		{
			private void  InitBlock(SSH2Channel enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private SSH2Channel enclosingInstance;

			internal int PacketSize
			{
				get
				{
					return packetsize;
				}
				
			}

			internal int windowsize;
			internal int packetsize;
			
			internal DataWindow(SSH2Channel enclosingInstance, int windowsize, int packetsize)
			{
				InitBlock(enclosingInstance);
				this.windowsize = windowsize;
				this.packetsize = packetsize;
			}
			
			
			internal void  adjust(int count)
			{
				windowsize += count;
			}
			
			internal void  consume(int count)
			{
				windowsize -= count;
			}
			
			internal int available()
			{
				return windowsize;
			}
		}

#if DEBUG 

		internal void LogMessage(SSHPacket packet)
		{
		
			switch(packet.MessageID)
			{
				case SSH_MSG_CHANNEL_OPEN_FAILURE:
				{
					System.Diagnostics.Trace.WriteLine("Received SSH_MSG_CHANNEL_OPEN_FAILURE for channel id " + ChannelID );
					return;
				}
				case SSH_MSG_CHANNEL_OPEN_CONFIRMATION:
				{
					System.Diagnostics.Trace.WriteLine("Received SSH_MSG_CHANNEL_OPEN_CONFIRMATION for channel id " + ChannelID );
					return;
				}
				case SSH_MSG_CHANNEL_CLOSE:
				{
					System.Diagnostics.Trace.WriteLine("Received SSH_MSG_CHANNEL_CLOSE for channel id " + ChannelID );
					return;
				}
				case SSH_MSG_CHANNEL_EOF:
				{
					System.Diagnostics.Trace.WriteLine("Received SSH_MSG_CHANNEL_EOF for channel id " + ChannelID );
					return;
				}
				case SSH_MSG_CHANNEL_REQUEST:
				{
					System.Diagnostics.Trace.WriteLine("Received SSH_MSG_CHANNEL_REQUEST for channel id " + ChannelID );
					return;
				}			
				case SSH_MSG_CHANNEL_SUCCESS:
				{
					System.Diagnostics.Trace.WriteLine("Received SSH_MSG_CHANNEL_SUCCESS for channel id " + ChannelID );
					return;
				}	
				case SSH_MSG_CHANNEL_FAILURE:
				{
					System.Diagnostics.Trace.WriteLine("Received SSH_MSG_CHANNEL_FAILURE for channel id " + ChannelID );
					return;
				}	
				case SSH_MSG_WINDOW_ADJUST:
				{
					System.Diagnostics.Trace.WriteLine("Received SSH_MSG_WINDOW_ADJUST for channel id " + ChannelID );
					return;
				}
				case SSH_MSG_CHANNEL_DATA:
				{
					System.Diagnostics.Trace.WriteLine("Received SSH_MSG_CHANNEL_DATA for channel id " + ChannelID );
					return;
				}
				case SSH_MSG_CHANNEL_EXTENDED_DATA:
				{
					System.Diagnostics.Trace.WriteLine("Received SSH_MSG_CHANNEL_EXTENDED_DATA for channel id " + ChannelID );
					return;
				}
			}

		}

#endif 
	}
}
