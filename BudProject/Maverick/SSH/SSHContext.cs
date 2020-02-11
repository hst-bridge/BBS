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
	/// This interface defines the general configuration items available to both SSH1 and SSH2.
	/// </summary>
	/// <remarks>
	/// Each new instance of <see cref="Maverick.SSH.SSHConnector"/> is initialized with a configuration context 
	/// for each protocol version. When the user connects to a remote SSH server using the <see cref="Maverick.SSH.SSHConnector"/>
	/// the returned <see cref="Maverick.SSH.SSHClient"/> is configured with the context according to the 
	/// protocol version. Multiple connections can be made from the <see cref="Maverick.SSH.SSHConnector"/> with the 
	/// same context. 
	/// </remarks>
	public interface SSHContext
	{
		/// <summary>
		/// The maximum number of channels allowed open at any one time by the client
		/// </summary>
		int MaximumNumberChannels
		{
			get;

			set;
		}

		/// <summary>
		/// The X11 forwarding authentication cookie.
		/// </summary>
		byte[] X11AuthenticationCookie
		{
			get;
		}

		/// <summary>
		/// The real X11 authentication cookie.
		/// </summary>
		byte[] X11RealCookie
		{
			get;

			set;
		}

		/// <summary>
		/// The number of the X11 display.
		/// </summary>
		String X11Display
		{
			get;

			set;
		}


		/// <summary>
		/// The request listener used to establish X11 tunnels.
		/// </summary>
		ForwardingRequestListener X11RequestListener
		{
			get;

			set;
		}

		/// <summary>
		/// The host verification instance for this configuration
		/// </summary>
		HostKeyVerification KnownHosts
		{
			get;

			set;
		}

		/// <summary>
		/// The name of the SFTP provider
		/// </summary>
		String SFTPProvider
		{
			get;

			set;
		}


	}
}
