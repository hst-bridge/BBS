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

namespace Maverick.SSH.Packets
{
	/// <summary>
	/// This interface is implemented by a class interested in receiving
	/// notification of a messages arrival in the message store.
	/// </summary>
	public interface PacketObserver
	{
		/// <summary>
		/// Implementor should check the packet type and return a bool value
		/// to indicate whether it wants to process the supplied message
		/// </summary>
		/// <param name="packet"></param>
		/// <returns></returns>
		bool WantsNotification(SSHPacket packet);
	}
}
