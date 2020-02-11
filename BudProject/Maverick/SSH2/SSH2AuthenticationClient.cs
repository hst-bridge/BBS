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

namespace Maverick.SSH2
{
	/// <summary>
	/// Defines the contract for an SSH2 authentication client.
	/// </summary>
	public interface SSH2AuthenticationClient
	{
		/// <summary>
		/// Start the authentication request.
		/// </summary>
		/// <param name="authentication">The authentication protocol instance.</param>
		/// <param name="serviceName">The name of the service to start upon a successful authentication.</param>
		void Authenticate(AuthenticationProtocol authentication,
			String serviceName);
	}
}
