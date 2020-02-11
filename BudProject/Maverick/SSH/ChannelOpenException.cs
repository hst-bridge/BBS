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
	/// Exception thrown when the remote server denies a channel open request.
	/// </summary>
	public class ChannelOpenException:System.Exception
	{
		
		/// <summary>
		/// The reason code for the channel open failure.
		/// </summary>
		public int Reason
		{
			get
			{
				return reason;
			}
			
		}
		
		/// <summary>
		/// The administrator does not permit this channel to be opened 
		/// </summary>
		public const int ADMINISTRATIVIVELY_PROHIBITED = 1;
		
		/// <summary>
		/// The connection could not be established
		/// </summary>
		public const int CONNECT_FAILED = 2;
		
		/// <summary>
		/// The channel type is unknown
		/// </summary>
		public const int UNKNOWN_CHANNEL_TYPE = 3;
		
		/// <summary>
		/// There are no more resources available to open the channel
		/// </summary>
		public const int RESOURCE_SHORTAGE = 4;
		
		internal int reason;

		/// <summary>
		/// Construct the channel failure exception.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="reason"></param>
		public ChannelOpenException(System.String msg, int reason):base(msg)
		{
			this.reason = reason;
		}
		
	}
}
