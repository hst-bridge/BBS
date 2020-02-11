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

namespace Maverick.SFTP
{
	/// <summary>
	/// This exception is thrown by the <see cref="Maverick.SFTP.SFTPClient"/> when a transfer is cancelled
	/// by the user.
	/// </summary>
	/// <remarks>
	/// A transfer can only be cancelled if the <see cref="Maverick.SFTP.SFTPClient"/> was 
	/// initialized with a <see cref="Maverick.Events.FileTransferProgress"/>.
	/// </remarks>
	public class TransferCancelledException : Exception
	{
		/// <summary>
		/// Create an exception instance with the default test "The transfer was cancelled!"
		/// </summary>
		public TransferCancelledException() : base("The transfer was cancelled!")
		{
		}

		/// <summary>
		/// Create an exception with the specified message.
		/// </summary>
		/// <param name="msg"></param>
		public TransferCancelledException(String msg) : base(msg)
		{
		}

	}
}
