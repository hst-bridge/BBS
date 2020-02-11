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

namespace Maverick.SSH
{
	/// <summary>
	/// Simple interface for transport layer communication. An SSH connection requires
	/// a transport layer for communication and this interface defines the general
	/// contract. Typically SSH will execute over a TCP/IP socket however the use of
	/// this interface allows any type of interface that exposes a System.IO.Stream.
	/// </summary>
	public interface SSHTransport : SSHIO
	{
		/// <summary>
		/// The name of the remote host.
		/// </summary>
		String Hostname
		{
			get;
		}

		/// <summary>
		/// Create a new connection to the same destination.
		/// </summary>
		/// <returns></returns>
		SSHTransport Duplicate();
	}
}
