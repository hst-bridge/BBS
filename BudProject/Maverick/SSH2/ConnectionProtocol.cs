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
using Maverick.SSH;
using Maverick.SSH.Packets;
using Maverick.Crypto.Util;

namespace Maverick.SSH2
{
	/// <summary>
	/// The SSH Connection Protocol provides multiplexed channels over a single connection.
	/// </summary>
	public class ConnectionProtocol : SSHPacketRouter
	{
		/// <summary>
		/// Defines the name of the Connection protocols transport service "ssh-userauth".
		/// </summary>
		public const System.String SERVICE_NAME = "ssh-connection";
		
		internal const int SSH_MSG_CHANNEL_OPEN = 90;
		internal const int SSH_MSG_CHANNEL_OPEN_CONFIRMATION = 91;
		internal const int SSH_MSG_CHANNEL_OPEN_FAILURE = 92;
		
		internal const int SSH_MSG_GLOBAL_REQUEST = 80;
		internal const int SSH_MSG_REQUEST_SUCCESS = 81;
		internal const int SSH_MSG_REQUEST_FAILURE = 82;
		
		internal class ChannelOpenPacketObserver : PacketObserver
		{

			public bool WantsNotification(SSHPacket msg) 
			{
				switch(msg.MessageID) 
				{
					case SSH_MSG_CHANNEL_OPEN_CONFIRMATION:
					case SSH_MSG_CHANNEL_OPEN_FAILURE:
						return true;
					default:
						return false;
				}
			}
		};

		internal ChannelOpenPacketObserver CHANNEL_OPEN_RESPONSE_MESSAGES 
			= new ChannelOpenPacketObserver();

		internal class GlobalRequestPacketObserver : PacketObserver
		{

			public bool WantsNotification(SSHPacket msg) 
			{
				switch(msg.MessageID) 
				{
					case SSH_MSG_REQUEST_SUCCESS:
					case SSH_MSG_REQUEST_FAILURE:
						return true;
					default:
						return false;
				}
			}
		};

		internal GlobalRequestPacketObserver GLOBAL_REQUEST_MESSAGES
			= new GlobalRequestPacketObserver();
		
		internal TransportProtocol transport;
		internal System.Collections.Hashtable channelfactories;
		internal System.Collections.Hashtable requesthandlers;

		/// <summary>
		/// Create an instance of the Connection protocol.
		/// </summary>
		/// <param name="transport"></param>
		/// <param name="threaded"></param>
		public ConnectionProtocol(TransportProtocol transport, bool threaded)
			: base(transport, transport.Context.MaximumNumberChannels, threaded)
		{
			this.transport = transport;
			channelfactories = new System.Collections.Hashtable();
			requesthandlers = new System.Collections.Hashtable();
		}

		/// <summary>
		/// The current configuration context.
		/// </summary>
		public SSHContext Context
		{
			get
			{
				return transport.Context;
			}
			
		}

		/// <summary>
		/// Adds a factory to the list channel factories. 
		/// </summary>
		/// <param name="factory"></param>
		public void AddChannelFactory(ChannelFactory factory)
		{
			String[] types = factory.SupportedChannelTypes;
			for (int i = 0; i < types.Length; i++)
			{
				if (channelfactories.ContainsKey(types[i]))
				{
					throw new SSHException(types[i] + " channel is already registered!", SSHException.BAD_API_USAGE);
				}
				
				SupportClass.PutElement(channelfactories, types[i], factory);
			}
		}

		/// <summary>
		/// Adds a handler to the list of global request handlers.
		/// </summary>
		/// <param name="handler"></param>
		public void AddRequestHandler(GlobalRequestHandler handler)
		{
#if DEBUG
            System.Diagnostics.Trace.WriteLine("Adding global request handler");
#endif
			System.String[] types = handler.SupportedRequests;
			for (int i = 0; i < types.Length; i++)
			{
				if (requesthandlers.ContainsKey(types[i]))
				{
					throw new SSHException(types[i] + " request is already registered!", 
						SSHException.BAD_API_USAGE);
				}

#if DEBUG
                System.Diagnostics.Trace.WriteLine("Registering request " + types[i]);
#endif
                SupportClass.PutElement(requesthandlers, types[i], handler);
			}
		}

		/// <summary>
		/// Sends a global request.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="wantreply"></param>
		/// <returns></returns>
		public bool SendGlobalRequest(GlobalRequest request, bool wantreply)
		{
			try
			{
				SSHPacket packet = GetPacket();
				packet.WriteByte(SSH_MSG_GLOBAL_REQUEST);
				packet.WriteString(request.Name);
				packet.WriteBool(wantreply);
				if (request.Data != null)
				{
					packet.WriteBytes(request.Data);
				}

#if DEBUG
				System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_GLOBAL_REQUEST");
				System.Diagnostics.Trace.WriteLine(request.Name );
#endif
				SendMessage(packet);
				
				if (wantreply)
				{
					packet = GlobalMessages.NextMessage(GLOBAL_REQUEST_MESSAGES);
					if (packet.MessageID == SSH_MSG_REQUEST_SUCCESS)
					{
						if (packet.Available > 1)
						{
							byte[] tmp = new byte[packet.Available];
							packet.ReadBytes(tmp);
							request.Data = tmp;
						}
						else
						{
							request.Data = null;
						}
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return true;
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// Opens a channel
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="requestdata"></param>
		public void OpenChannel(SSH2Channel channel, byte[] requestdata)
		{
			lock (this)
			{
				
				try
				{
					int channelid = AllocateChannel(channel);
					
					if (channelid == - 1)
					{
						throw new ChannelOpenException("Maximum number of channels exceeded", 
							ChannelOpenException.RESOURCE_SHORTAGE);
					}
					
					channel.Init(this, channelid);
					SSHPacket packet = GetPacket();
					packet.WriteByte(SSH_MSG_CHANNEL_OPEN);
					packet.WriteString(channel.Name);
					packet.WriteUINT32(channel.ChannelID);
					packet.WriteUINT32(channel.WindowSize);
					packet.WriteUINT32(channel.PacketSize);
					if (requestdata != null)
					{
						packet.WriteBytes(requestdata);
					}

#if DEBUG
					System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_CHANNEL_OPEN");
					System.Diagnostics.Trace.WriteLine("Channelid=" + channel.ChannelID );
					System.Diagnostics.Trace.WriteLine("Name=" + channel.Name );
#endif

					transport.SendMessage(packet);
					
					packet = channel.MessageStore.NextMessage(CHANNEL_OPEN_RESPONSE_MESSAGES);

#if DEBUG
					System.Diagnostics.Trace.WriteLine("Received reply to SSH_MSG_CHANNEL_OPEN");
					channel.LogMessage(packet);
#endif
					if (packet.MessageID == SSH_MSG_CHANNEL_OPEN_FAILURE)
					{
						FreeChannel(channel);
						int reason = (int) packet.ReadUINT32();
						throw new ChannelOpenException(packet.ReadString(), reason);
					}
					else
					{
						int remoteid = (int) packet.ReadUINT32();
						int remotewindow = (int) packet.ReadUINT32();
						int remotepacket = (int) packet.ReadUINT32();
						byte[] responsedata = new byte[packet.Available];
						packet.ReadBytes(responsedata);
		
						channel.Open(remoteid, remotewindow, remotepacket, responsedata);
						
						return;
					}
				}
				catch (System.IO.IOException ex)
				{
					throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
				}
			}
		}

		/// <summary>
		/// Get an SSH packet
		/// </summary>
		/// <returns></returns>
		protected internal SSHPacket GetPacket()
		{
			return transport.GetSSHPacket(true);
		}

		/// <summary>
		/// Send a message
		/// </summary>
		/// <param name="packet"></param>
		protected internal void SendMessage(SSHPacket packet)
		{
			transport.SendMessage(packet);
		}

		/// <summary>
		/// Close a channel
		/// </summary>
		/// <param name="channel"></param>
		protected internal void CloseChannel(SSH2Channel channel)
		{
			FreeChannel(channel);
		}

		/// <summary>
		/// Evaluate whether the message id is a channel message id.
		/// </summary>
		/// <param name="messageid"></param>
		/// <returns></returns>
		protected internal override bool IsChannelMessage(int messageid)
		{
			if (messageid >= 91 && messageid <= 100)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		/// <summary>
		/// Process a global message.
		/// </summary>
		/// <param name="packet"></param>
		/// <returns></returns>
		protected internal override bool ProcessGlobalMessage(SSHPacket packet)
		{
			// We need to filter for any messages that require a response from the
			// connection protocol such as channel open or global requests. These
			// are not handled anywhere else within this implementation because
			// doing so would require a thread to wait.
 			
			try
			{
				switch (packet.MessageID)
				{
					
					case SSH_MSG_CHANNEL_OPEN:  
					{
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Received SSH_MSG_CHANNEL_OPEN");
#endif
						//		  Attempt to open the channel
						System.String type = packet.ReadString();
						int remoteid = (int) packet.ReadUINT32();
						int remotewindow = (int) packet.ReadUINT32();
						int remotepacket = (int) packet.ReadUINT32();
						byte[] requestdata = packet.Available > 0 ? new byte[packet.Available] : null;
						if(requestdata!=null)
							packet.ReadBytes(requestdata);

						ProcessChannelOpenRequest(type, remoteid, remotewindow, remotepacket, requestdata);
						return true;
					}
					
					case SSH_MSG_GLOBAL_REQUEST:  
					{
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Received SSH_MSG_GLOBAL_REQUEST");
#endif

						// Attempt to process the global request
						System.String requestname = packet.ReadString();
						bool wantreply = packet.ReadBool();
						byte[] requestdata = new byte[packet.Available];
						packet.ReadBytes(requestdata);
							
						// Process the request
						ProcessGlobalRequest(requestname, wantreply, requestdata);
						return true;
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

		/// <summary>
		/// Process a channel open request.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="remoteid"></param>
		/// <param name="remotewindow"></param>
		/// <param name="remotepacket"></param>
		/// <param name="requestdata"></param>
		internal void  ProcessChannelOpenRequest(System.String type, int remoteid, int remotewindow, int remotepacket, byte[] requestdata)
		{
			
			try
			{
				SSHPacket packet = GetPacket();
				
				if (channelfactories.ContainsKey(type))
				{
					try
					{
						SSH2Channel channel = ((ChannelFactory) channelfactories[type]).CreateChannel(type, requestdata);
						
						// Allocate a channel
						int localid = AllocateChannel(channel);
						
						if (localid > - 1)
						{
							try
							{
								channel.Init(this, localid);
								byte[] responsedata = channel.Create();
								packet.WriteByte(SSH_MSG_CHANNEL_OPEN_CONFIRMATION);
								packet.WriteUINT32(remoteid);
								packet.WriteUINT32(localid);
								packet.WriteUINT32(channel.WindowSize);
								packet.WriteUINT32(channel.PacketSize);
								if (responsedata != null)
								{
									packet.WriteBytes(requestdata);
								}

#if DEBUG
								System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_CHANNEL_OPEN_CONFIRMATION");
								System.Diagnostics.Trace.WriteLine("Channelid=" + localid );								
								System.Diagnostics.Trace.WriteLine("Remoteid=" + remoteid );
#endif
								transport.SendMessage(packet);
								
								channel.Open(remoteid, remotewindow, remotepacket);
								
								return;
							}
							catch (SSHException ex)
							{

#if DEBUG
								System.Diagnostics.Trace.WriteLine("Exception occured whilst opening channel");
								System.Diagnostics.Trace.WriteLine(ex.StackTrace );
#endif
								packet.WriteByte(SSH_MSG_CHANNEL_OPEN_FAILURE);
								packet.WriteUINT32(remoteid);
								packet.WriteUINT32(ChannelOpenException.CONNECT_FAILED);
								packet.WriteString(ex.Message);
								packet.WriteString("");
							}
						}
						else
						{
#if DEBUG
							System.Diagnostics.Trace.WriteLine("Maximum allowable open channel limit of " + MaximumNumChannels + " exceeded!");
#endif
							packet.WriteByte(SSH_MSG_CHANNEL_OPEN_FAILURE);
							packet.WriteUINT32(remoteid);
							packet.WriteUINT32(ChannelOpenException.RESOURCE_SHORTAGE);
							packet.WriteString("Maximum allowable open channel limit of " + MaximumNumChannels + " exceeded!");
							packet.WriteString("");
						}
					}
					catch (ChannelOpenException ex)
					{
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Channel open exception occured whilst opening channel");
						System.Diagnostics.Trace.WriteLine(ex.StackTrace );
#endif	
						packet.WriteByte(SSH_MSG_CHANNEL_OPEN_FAILURE);
						packet.WriteUINT32(remoteid);
						packet.WriteUINT32(ex.Reason);
						packet.WriteString(ex.Message);
						packet.WriteString("");
					}
				}
				else
				{

#if DEBUG
					System.Diagnostics.Trace.WriteLine(type + " is not a supported channel type");
#endif
					packet.WriteByte(SSH_MSG_CHANNEL_OPEN_FAILURE);
					packet.WriteUINT32(remoteid);
					packet.WriteUINT32(ChannelOpenException.UNKNOWN_CHANNEL_TYPE);
					packet.WriteString(type + " is not a supported channel type!");
					packet.WriteString("");
				}
				
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_CHANNEL_OPEN_FAILURE");
				System.Diagnostics.Trace.WriteLine("Remoteid=" + remoteid );
				System.Diagnostics.Trace.WriteLine("Name=" + type );
#endif
				transport.SendMessage(packet);
			}
			catch (System.IO.IOException ex1)
			{
				throw new SSHException(ex1.Message, SSHException.INTERNAL_ERROR);
			}
		}
		
		internal void  ProcessGlobalRequest(System.String requestname, bool wantreply, byte[] requestdata)
		{
			
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Processing global request " + requestname );
#endif
			try
			{
				bool success = false;
				GlobalRequest request = new GlobalRequest(requestname, requestdata);
                if (requesthandlers.ContainsKey(requestname))
                {
#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Found handler for request " + requestname);
#endif
                    success = ((GlobalRequestHandler)requesthandlers[requestname]).ProcessGlobalRequest(request);


                }
#if DEBUG
                else
                {
                    System.Diagnostics.Trace.WriteLine("Cannot find handler for request " + requestname);

                }
#endif
                if (wantreply)
				{
					SSHPacket packet = GetPacket();
					
					if (success)
					{

						packet.WriteByte(SSH_MSG_REQUEST_SUCCESS);
						if (request.Data != null)
						{
							packet.WriteBytes(requestdata);
						}
						
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_REQUEST_SUCCESS");
#endif
						transport.SendMessage(packet);
					}
					else
					{
						// Return a response
						packet.WriteByte(SSH_MSG_REQUEST_FAILURE);
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_REQUEST_FAILURE");
#endif
						
						transport.SendMessage(packet);
					}
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}
	}
}
