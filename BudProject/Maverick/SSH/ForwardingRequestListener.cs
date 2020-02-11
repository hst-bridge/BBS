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
	/// This interface is required when a request for remote port forwarding is made. 
	/// The methods enable you to establish a connection to the host and initialize 
	/// the forwarding channel before it is opened. 
	/// </summary>
	public interface ForwardingRequestListener
	{
		/// <summary>
		/// Create a connection to the specified host. When requesting remote forwarding 
		/// you specify the host and port to which incoming connections are bound. This 
		/// method should create a connection to the host and return the transport instance
		/// as an <see cref="System.Object"/>
		/// </summary>
		/// <param name="hostToConnect">The host to which the transport should be connected.</param>
		/// <param name="portToConnect">The port on which the transport should be connected.</param>
		/// <param name="listeningAddress">The address on which the connection was made</param>
		/// <param name="listeningPort">The port on which the connection was made</param>
		/// <returns></returns>
		Object CreateConnection(String hostToConnect, int portToConnect, String listeningAddress, int listeningPort);

		/// <summary>
		/// Called once a connection has been established and a forwarding channel is about to be opened. 
		/// Please note that the channel IS NOT open when this method is called and therefore cannot be used 
		/// to start transfering data. This provides you with the ability to configure the channel, for instance 
		/// by adding a <see cref="Maverick.SSH.ChannelStateListener"/> to detect the opening of the channel or
		/// a <see cref="Maverick.SSH.DataListener"/> to read data once the channel has been opened.  
		/// </summary>
		/// <param name="tunnel"></param>
		void InitializeTunnel(SSHTunnel tunnel);
	}
}
