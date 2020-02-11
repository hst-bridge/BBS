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
	/// Generic exception for J2SSH Maverick exception handling. When an exception is thrown 
	/// a reason is attached to the exception so that the developer can determine if its possible 
	/// to proceed with the connection.
	/// </summary>
	public class SSHException : System.Exception
	{

		/// <summary>
		/// The reason for the exception.
		/// </summary>
		virtual public int Reason
		{
			get
			{
				return reason;
			}
			
		}
		/// <summary>
		/// If an INTERNAL_ERROR reason is given this method MAY return the cause of
		/// the error.
		/// </summary>
		virtual public System.Exception Cause
		{
			get
			{
				return cause;
			}
			
		}
		/// <summary> The connection unexpectedly terminated and so the connection can
		/// no longer be used. The exception message will contain the message
		/// from the exception that caused the termination.
		/// </summary>
		public const int UNEXPECTED_TERMINATION = 1;
		
		/// <summary> The remote host disconnected following the normal SSH protocol
		/// disconnection procedure. The exception message will contain the
		/// message received from the remote host that describes the reason for
		/// the disconnection
		/// </summary>
		public const int REMOTE_HOST_DISCONNECTED = 2;
		
		/// <summary> The SSH protocol was violated in some way by the remote host and
		/// the connection has been terminated. The exception message will contain
		/// a description of the protocol violation.
		/// </summary>
		public const int PROTOCOL_VIOLATION = 3;
		
		/// <summary> The API has encountered an error because of incorrect usage. The state of
		/// the connection upon receiving this exception is unknown.
		/// </summary>
		public const int BAD_API_USAGE = 4;
		
		/// <summary> An internal error occured within the API; in all cases contact support@sshtools.com
		/// with the details of this error and the state of the connection when receiving
		/// this exception is unknown.
		/// </summary>
		public const int INTERNAL_ERROR = 5;
		
		/// <summary> Indicates that a channel has failed; this is used by channel implementations
		/// (such as port forwarding or session channels) to indicate that the
		/// channel has critically failed. Upon receiving this exception you should
		/// check the connection state to determine whether its still possible to use
		/// the connection.
		/// </summary>
		public const int CHANNEL_FAILURE = 6;
		
		/// <summary> In setting up a context an algorithm was specified that is not supported
		/// by the API.
		/// </summary>
		public const int UNSUPPORTED_ALGORITHM = 7;
		
		/// <summary> The user cancelled the connection.
		/// </summary>
		public const int CANCELLED_CONNECTION = 8;
		
		/// <summary> The protocol failed to negotiate a transport algorithm or failed to
		/// verify the host key of the remote host.
		/// </summary>
		public const int KEY_EXCHANGE_FAILED = 9;
		
		/// <summary> The connection could not be established.
		/// </summary>
		public const int CONNECT_FAILED = 10;
		
		/// <summary> The API is not licensed!
		/// </summary>
		public const int LICENSE_ERROR = 11;
		
		/// <summary> An attempt has been made to use a connection that has been closed and 
		/// is no longer valid.
		/// </summary>
		public const int CONNECTION_CLOSED = 12;
		
		/// <summary> An error has occured within the agent.
		/// </summary>
		public const int AGENT_ERROR = 13;
		
		/// <summary> An error has occured the port forwarding system.
		/// </summary>
		public const int FORWARDING_ERROR = 14;
		
		/// <summary> A request was made to allocate a pseudo terminal, but this request failed.
		/// </summary>
		public const int PSEUDO_TTY_ERROR = 15;
		
		/// <summary> A request was made to start a shell, but this request failed.
		/// </summary>
		public const int SHELL_ERROR = 16;
		
		/// <summary> An error occured whilst accessing a sessions streams
		/// </summary>
		public const int SESSION_STREAM_ERROR = 17;

		/// <summary>
		/// An SCP protocol error occurred.
		/// </summary>
		public const int SCP_ERROR = 18;

		/// <summary>
		/// A public/private key format error occurred.
		/// </summary>
		public const int KEY_FORMAT_ERROR = 19;
		
		/// <summary>
		/// The user cancelled an operation.
		/// </summary>
		public const int USER_CANCELATION = 20;

		internal int reason;
		internal System.Exception cause;

		/// <summary> Create an exception with the given description and reason.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="reason"></param>
		public SSHException(System.String msg, int reason):this(msg, reason, null)
		{
		}
		
		/// <summary> Create an exception with the given cause and reason.
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="cause"></param>
		public SSHException(int reason, System.Exception cause):this(null, reason, cause)
		{
		}

		/// <summary>
		/// Create an exception with the given description and cause. The reason given
		/// will be <see cref="Maverick.SSH.SSHException.INTERNAL_ERROR"/>
		/// </summary>
		/// <param name="msg">The exception message.</param>
		/// <param name="cause">The cause of the exception.</param>
		public SSHException(System.String msg, System.Exception cause):this(msg, INTERNAL_ERROR, cause)
		{
		}
		/// <summary>
		/// Create an exception by providing the cause of the error. The reason given
		/// will be <see cref="Maverick.SSH.SSHException.INTERNAL_ERROR"/>
		/// </summary>
		/// <param name="cause">The cause of the exception.</param>
		public SSHException(System.Exception cause):this("An unexpected exception was caught; examine SshException.getCause()", cause)
		{
		}

		/// <summary>
		/// Create an exception with the given description cause, reason.
		/// </summary>
		/// <param name="msg">The exception message.</param>
		/// <param name="reason">The exception reason code.</param>
		/// <param name="cause">The cause of the exception.</param>
		public SSHException(System.String msg, int reason, System.Exception cause):base(msg)
		{
			this.cause = cause;
			this.reason = reason;
		}
	}
}
