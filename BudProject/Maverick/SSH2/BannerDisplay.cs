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
	/// Callback interface to display authentication banner messages.
	/// </summary>
	/// <remarks>
	/// In some jurisdictions sending a warning message before authentication may be relevant 
	/// for getting legal protection. Many UNIX machines, for example, normally display text 
	/// from `/etc/issue', or use "tcp wrappers" or similar software to display a banner before 
	/// issuing a login prompt. Implement this interface to show the authentication banner message. The method should 
	/// display the message and should not return until the user accepts the message.<br/>
    /// <br/>
	/// To active a banner create your display and set using the <see cref="Maverick.SSH2.SSH2Context.Banner"/>
	/// property before establishing a connection.
	/// </remarks>
	public interface BannerDisplay
	{
		/// <summary>
		/// Display the text to the user, wait and return once the user has accepted.
		/// </summary>
		/// <param name="message"></param>
		void DisplayBanner(String message);
	}
}
