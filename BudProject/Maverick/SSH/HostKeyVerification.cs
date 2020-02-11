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
	/// This interface provides a callback method so that the user can verify the identity of 
	/// the server (by checking the public key) during the initial protocol negotiation.
	/// </summary>
	/// <remarks>
	/// This check is performed at the beginning of each connection to prevent trojan horses 
	/// (by routing or DNS spoofing) and man-in-the-middle attacks. 
	/// 
	/// The user should verify that the key is acceptable; the most usual method being a local 
	/// database file called known_hosts. The core Maverick.NET engine does not enforce any 
	/// specific host key verification implementation.
	/// <example>
	/// <code>
	/// class SimpleHostKeyVerification : HostKeyVerification
	/// {
	///		public bool VerifyHost(String hostname, SSHPublicKey key)
	///		{
	///			return true;
	///		}
	///	}
	///	
	///	// Then to configure before establishing a connection
	///	
	///	SSHConnector con = SSHConnector.Create();
	///	con.KnownHosts = new SimpleHostKeyVerification();
	///	</code>
	/// </example>
	/// </remarks> 
	public interface HostKeyVerification
	{
		/// <summary>
		/// Verify that the public key is acceptable for the host. If this method returns false 
		/// the connection will be cancelled.
		/// </summary>
		/// <param name="hostname">The name of the server.</param>
		/// <param name="key">The public key supplied by the server during key exchange.</param>
		/// <returns>true if the key has been verified, otherwise false</returns>
		bool VerifyHost(String hostname, SSHPublicKey key);
	}
}
