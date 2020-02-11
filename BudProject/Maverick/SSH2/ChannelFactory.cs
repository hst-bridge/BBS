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

namespace Maverick.SSH2
{
	/// <summary>
	/// A factory for creating SSH2 channels.
	/// </summary>
	/// <remarks>
	/// The SSH2 protocol supports many different channel types including sesisons, port forwarding and 
	/// x11 forwarding. Most channels are requested by the client and created by the server however it 
	/// is possible for the server to request any type of channel from the client, this interface defines 
	/// the contract for supporting a standard and custom channel creation. 
	/// </remarks>
	public interface ChannelFactory
	{
		/// <summary>
		/// Return the supported channel types.
		/// </summary>
		String[] SupportedChannelTypes
		{
			get;
		}

		/// <summary>
		/// Create an instance of an SSH channel. The new instance should be returned, if for any reason the 
		/// channel cannot be created either because the channel is not supported or there are not enough 
		/// resources an exception should be thrown.
		/// </summary>
		/// <param name="channeltype"></param>
		/// <param name="requestdata"></param>
		/// <returns></returns>
		/// <exception cref="Maverick.SSH.ChannelOpenException">
		/// Throw this exception if the channel cannot be opened, is not supported or there are not enough resources.
		/// </exception>
		SSH2Channel CreateChannel(String channeltype, byte[] requestdata);
	}
}
