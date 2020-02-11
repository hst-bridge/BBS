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
	/// An exception thrown by the SFTP subsystem to indicate some type of failure. These exceptions are mostly non critical
	/// (although critical errors may be returned) and indicate that a problem with an SFTP operation was encountered. 
	/// Typically this would be a permission denied or file not found exception
	/// </summary>
	public class SFTPStatusException : Exception
	{
		/// <summary>
		/// Get the status of the exception
		/// </summary>
		/// <remarks>
		/// The value returned will be one of the constant fields provided by this class.
		/// </remarks>
		public int Status
		{
			get
			{
				return status;
			}
			
		}
		
		/// <summary>
		/// Everything is OK, the operation completed successfully.
		/// </summary>
		/// <remarks>
		/// This is for internal use and will never be returned in an exception.
		/// </remarks>
		public const int SSH_FX_OK = 0;

		/// <summary>
		/// The file has reached EOF
		/// </summary>
		/// <remarks>
		/// This is for internal use and will never be returned in an exception.
		/// </remarks>
		public const int SSH_FX_EOF = 1;

		/// <summary>
		/// The specified in the operation does not exist
		/// </summary>
		public const int SSH_FX_NO_SUCH_FILE = 2;

		/// <summary>
		/// The current user does not have permission to access the file.
		/// </summary>
		public const int SSH_FX_PERMISSION_DENIED = 3;

		/// <summary>
		/// A general failure occurred
		/// </summary>
		public const int SSH_FX_FAILURE = 4;

		/// <summary>
		/// A bad message was received by the server
		/// </summary>
		public const int SSH_FX_BAD_MESSAGE = 5;

		/// <summary>
		/// No connection to the file system could be obtained
		/// </summary>
		public const int SSH_FX_NO_CONNECTION = 6;

		/// <summary>
		/// The connection to the SFTP server was lost/disconnected
		/// </summary>
		public const int SSH_FX_CONNECTION_LOST = 7;

		/// <summary>
		/// The operation requested is not supported by the server
		/// </summary>
		public const int SSH_FX_OP_UNSUPPORTED = 8;

		/// <summary>
		/// An invalid handle was presented to the client
		/// </summary>
		public const int INVALID_HANDLE = 100;

		/// <summary>
		/// An attempt to resume an SFTP transfer could not be completed because the file either
		/// does not exist or the destination file is larger than the source file.
		/// </summary>
		public const int INVALID_RESUME_STATE = 101;
		internal int status;
		
		/// <summary>
		/// Create an SFTP exception
		/// </summary>
		/// <param name="status"></param>
		/// <param name="msg"></param>
		public SFTPStatusException(int status, System.String msg) : base(msg)
		{
			this.status = status;
		}
		
		/// <summary>
		/// Create an SFTP exception with default reason text.
		/// </summary>
		/// <param name="status"></param>
		public SFTPStatusException(int status) : this(status, GetStatusText(status))
		{
		}
		
		
		private static System.String GetStatusText(int status)
		{
			switch (status)
			{
						
				case SSH_FX_OK: 
					return "OK";
						
				case SSH_FX_EOF: 
					return "EOF";
						
				case SSH_FX_NO_SUCH_FILE: 
					return "No such file.";
						
				case SSH_FX_PERMISSION_DENIED: 
					return "Permission denied.";
						
				case SSH_FX_FAILURE: 
					return "Server responded with an unknown failure.";
						
				case SSH_FX_BAD_MESSAGE: 
					return "Server responded to a bad message.";
						
				case SSH_FX_NO_CONNECTION: 
					return "No connection available.";
						
				case SSH_FX_CONNECTION_LOST: 
					return "Connection lost.";
						
				case SSH_FX_OP_UNSUPPORTED: 
					return "The operation is unsupported.";
						
				default: 
					return "Unknown status type " + status.ToString();
						
			}
		}
	}
}
