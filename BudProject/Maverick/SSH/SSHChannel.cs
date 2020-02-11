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

namespace Maverick.SSH
{
	/// <summary>
	/// An enumeration of channel states. The channel is either OPEN or CLOSED however each side of
	/// the connection may also be in an EOF state which indicates that they will no longer be sending
	/// any data.
	/// </summary>
	public enum ChannelState 
	{

		/// <summary>
		/// The channel has been initialized but is not open
		/// </summary>
		INITIALIZED,

		/// <summary>
		/// The channel is now open and ready for sending/receiving data
		/// </summary>
		OPEN,

		/// <summary>
		/// The channel will not be sending any more data
		/// </summary>
		LOCAL_EOF,

		/// <summary>
		/// The channel will not be receiving any more data
		/// </summary>
		REMOTE_EOF,

		/// <summary>
		/// The channel is closed
		/// </summary>
		CLOSED
	}

	/// <summary>
	/// A delegate for receiving <see cref="Maverick.SSH.ChannelState"/> change events.
	/// </summary>
	public delegate void ChannelStateListener(SSHChannel channel, ChannelState newstate);
	
	/// <summary>
	/// A delegate for receiving notification of <see cref="Maverick.SSH.SSHChannel"/> data events.
	/// </summary>
	public delegate void DataListener(SSHChannel channel, byte[] buf, int off, int len);

	/// <summary>
	/// The base interface for all SSH channels. SSH Channels enable the multiplexing of several unique data 
	/// channels over a single SSH connection, each channel is identified by an unique ID and provides a 
	/// <see cref="System.IO.Stream"/> for sending and recieving data.
	/// </summary>
	public interface SSHChannel : SSHIO
	{
		/// <summary>
		/// An event that fires when the channel state has changed.
		/// </summary>
		event ChannelStateListener StateChange;

		/// <summary>
		/// Provides notification of when data is received by the channel
		/// </summary>
		event DataListener InputStreamListener;

		/// <summary>
		/// Provides notification of when data is sent by the channel
		/// </summary>
		event DataListener OutputStreamListener;

		/// <summary>
		/// The connection's configuration context
		/// </summary>
		SSHContext Context
		{
			get;
		}

		/// <summary>
		/// The channels local id.
		/// </summary>
		int ChannelID
		{
			get;
		}

		/// <summary>
		/// Allows an object to be attached to the channel
		/// </summary>
		Object Attachment
		{
			get;

			set;
		}

		/// <summary>
		/// Returns true if the channel is closed, otherwise false.
		/// </summary>
		bool IsClosed
		{
			get;
		}

		/// <summary>
		/// This property when set to true will instruct the channel to consume its own input. This
		/// is required when receiving data through events only as the channels buffer must be constantly 
		/// emptied to allow more data to be sent from the remote side.
		/// </summary>
		bool AutoConsumeInput
		{
			get;

			set;
		}

		/// <summary>
		/// Send a channel close message but don't wait for the remote side's reply.
		/// </summary>
		void SendAsyncClose();
	}
}
