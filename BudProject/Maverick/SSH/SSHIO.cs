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
	/// Base interface for all SSH related IO interfaces.
	/// </summary>
	public interface SSHIO
	{
		/// <summary>
		/// Get a Stream for the transfer of all incoming/outgoing data. 
		/// </summary>
		/// <returns></returns>
		Stream GetStream();
		
		/// <summary>
		/// Close the IO
		/// </summary>
		void Close();
	}
}
