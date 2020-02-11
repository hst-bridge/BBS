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

namespace Maverick.Events
{

	/// <summary>
	/// Contains delegates and state implementations used by Maverick.NET classes to provide events to interested 
	/// parties.
	/// </summary>
	internal class NamespaceDoc
	{
	}

	/// <summary>
	/// An enumeration used by the <see cref="Maverick.Events.FileTransferProgress"/> delegate to inform
	/// of the current transfer state.
	/// </summary>
	public enum FileTransferState 
	{
		/// <summary>
		/// The transfer has been started.
		/// </summary>
		/// <remarks>
		/// When this state is received in the <see cref="Maverick.Events.FileTransferProgress"/> delegate
		/// it indicates that a file transfer has started. The bytes parameter is set to the length of the
		/// file.
		/// </remarks>
		TRANSFER_STARTED = 1,

		/// <summary>
		/// The transfer has progressed.
		/// </summary>
		/// <remarks>
		/// When this state is recevied in the <see cref="Maverick.Events.FileTransferProgress"/> delegate
		/// it indicates that the file transfer has progressed. The bytes parameter of the delegate is set to
		/// the number of bytes now transfered.
		/// </remarks>
		TRANSFER_PROGRESSED,

		/// <summary>
		/// The transfer has completed.
		/// </summary>
		/// <remarks>
		/// When this state is received in the <see cref="Maverick.Events.FileTransferProgress"/> delegate it
		/// indicates that the file transfer has completed. The bytes parameter of the delegate is set to the
		/// number of bytes transfered.
		/// </remarks>
		TRANSFER_COMPLETED,

		/// <summary>
		/// The transfer was cancelled.
		/// </summary>
		/// <remarks>
		/// When this state is received in the <see cref="Maverick.Events.FileTransferProgress"/> delegate it 
		/// indicates that the file transfer was cancelled by the previous delegate call returning a false 
		/// value. The bytes parameter will be set to the number of bytes transfered before the transfer was
		/// cancelled.
		/// </remarks>
		TRANSFER_CANCELLED,


		/// <summary>
		/// The transfer was stopped due to an error.
		/// </summary>
		/// <remarks>
		/// When this state is received in the <see cref="Maverick.Events.FileTransferProgress"/> delegate it
		/// indicates that a serious error was encountered during the file transfer. 
		/// </remarks>
		TRANSFER_ERROR

	}


	/// <summary>
	/// A delegate for receiving file transfer information from a file subsystem.
	/// </summary>
	/// <remarks>
	/// This delegate is used by the <see cref="Maverick.SCP.SCPClient"/> and <see cref="Maverick.SFTP.SFTPClient"/>
	/// to provide information on the progress of file transfers.
	/// </remarks>
	/// <returns>If an implementation wants to cancel a transfer they should return a false value, 
	/// otherwise return a true value to continue the file transfer.</returns>
	public delegate bool FileTransferProgress(FileTransferState state, String filename, long bytesValue);

}
