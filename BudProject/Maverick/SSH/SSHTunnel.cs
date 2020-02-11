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
	/// Types of forwarding channels
	/// </summary>
	public enum ForwardingChannelType
	{
		/// <summary>
		/// Local forwarding channel operates with the client accepting socket connections, 
		/// forwarding each connections data over the SSH tunnel, and the server delivers data
		/// to the destination host.
		/// </summary>
		LOCAL = 1,

		/// <summary>
		/// Remote forwarding operates in the opposite direction; the server accepts socket
		/// connections and forwards each connections data over the SSH tunnel,
		/// and the client delivers the data to the destination host.
		/// </summary>
		REMOTE,

		/// <summary>
		/// Very similar to remote forwarding but with special processing for the forwarding
		/// of the X protocol.
		/// </summary>
		X11
	}


	/// <summary>
	/// Interface defining the contract for SSH forwarding channels.
	/// </summary>
	/// <remarks>
	/// Forwarding channels can either be local or remote. Local forwarding transfers data from the local 
	/// computer to the remote side where it is delivered to the specified host. Remote forwarding is when 
	/// the remote computer forwards data to the local side.<br/>
	/// <br/>
	/// Another useful feature of the <see cref="Maverick.SSH.SSHTunnel"/> interface it implements the <see cref="Maverick.SSH.SSHTransport"/>
	/// interface. This enables it to be passed back into the <see cref="Maverick.SSH.SSHConnector"/> to 
	/// be used as the transport for a forwarded <see cref="Maverick.SSH.SSHClient"/> instance.
	/// </remarks>
	public interface SSHTunnel : SSHChannel, SSHTransport
	{
		/// <summary>
		/// The port to which the data is being forwarded. 
		/// </summary>
		int Port
		{
			get;
				
		}

		/// <summary>
		/// The type of forwarding in use by this tunnel.
		/// </summary>
		ForwardingChannelType ForwardingType
		{
			get;
		}
		
		/// <summary>
		/// The source ip address of the connection that is being forwarded. 
		/// </summary>
		System.String ListeningAddress
		{
			get;
				
		}
		
		/// <summary>
		/// The source port of the connection being forwarded. 
		/// </summary>
		int ListeningPort
		{
			get;
				
		}
		
		/// <summary>
		/// The host that made the initial connection to the listening address. 
		/// </summary>
		System.String OriginatingHost
		{
			get;
				
		}
		
		/// <summary>
		/// The port of the initial connection. 
		/// </summary>
		int OriginatingPort
		{
			get;
				
		}


		/// <summary>
		/// The connection being forwarded (local forwarding) or the destination of the forwarding 
		/// (remote forwarding). 
		/// </summary>
		Object Transport
		{
			get;
				
		}
	}
}
