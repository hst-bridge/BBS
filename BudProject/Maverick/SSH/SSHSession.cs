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
using System.Text;

namespace Maverick.SSH
{
	/// <summary>
	/// Base interface for SSH sessions supporting all the features common to both SSH1 and SSH2.
	/// </summary>
	/// <remarks>Sessions are created using the <see cref="Maverick.SSH.SSHClient"/>. Once a session has been obtained the 
	/// session will not be active until you execute a command, start the users shell or in the case of SSH2
	/// sessions start a subsystem.
	/// </remarks>
	public interface SSHSession : SSHChannel
	{


        
		/// <summary>
		/// The client that created this session.
		/// </summary>
		SSHClient Client
		{
			get;
		}

		/// <summary>
		/// An event that provides notification of the arrival of error data on the channel.
		/// </summary>
		event DataListener ErrorStreamListener;

		/// <summary>
		/// Obtain a stream that contains the STDERR output of the session.
		/// </summary>
		/// <returns></returns>
		Stream GetStderrStream();

		/// <summary>
		/// Starts the users default shell.
		/// </summary>
		/// <returns></returns>
		bool StartShell();

		/// <summary>
		/// Execute a command.
		/// </summary>
		/// <param name="str">The command to execute</param>
		/// <remarks>
		/// A successful command execution does not mean that the command itself executed and
		/// completed successfully. The return value simply indicates that the instruction was
		/// sent to the remote machine and was acknolwedged by the server. For
		/// example this may still return a <tt>true</tt> value if the command does not exist.
		/// </remarks>
		/// <returns>true if the command executed, otherwise false</returns>
		bool ExecuteCommand(String str);


		/// <summary>
		/// Execute a command.
		/// </summary>
		/// <param name="cmd">The command to execute</param>
		/// <param name="encoding">The character encoding of the command string</param>
		/// <remarks>
		/// A successful command execution does not mean that the command itself executed and
		/// completed successfully. The return value simply indicates that the instruction was
		/// sent to the remote machine and was acknolwedged by the server. For
		/// example this may still return a <tt>true</tt> value if the command does not exist.
		/// </remarks>
		/// <returns>true if the command executed, otherwise false</returns>
		bool ExecuteCommand(String cmd, Encoding encoding);

		/// <summary>
		/// The remote process may require a pseudo terminal. Call this method before executing a command or starting a shell. 
		/// </summary>
		/// <param name="term">the terminal type e.g "vt100"</param>
		/// <param name="cols">the number of columns</param>
		/// <param name="rows">the number of rows</param>
		/// <param name="width">the width of the terminal (informational only, can be zero)</param>
		/// <param name="height">the height of the terminal (informational only, can be zero)</param>
		/// <param name="modes">encoded terminal modes as described in the SSH protocol specifications. </param>
		/// <returns>true if the pty was allocated, otherwise false</returns>
		bool RequestPseudoTerminal(String term, int cols, int rows, int width, int height, PseudoTerminalModes modes);

		/// <summary>
		/// The remote process may require a pseudo terminal. Call this method before executing a command or starting a shell. 
		/// </summary>
		/// <param name="term">the terminal type e.g "vt100"</param>
		/// <param name="cols">the number of columns</param>
		/// <param name="rows">the number of rows</param>
		/// <param name="width">the width of the terminal (informational only, can be zero)</param>
		/// <param name="height">the height of the terminal (informational only, can be zero)</param>
		/// <returns>true if the pty was allocated, otherwise false</returns>
		bool RequestPseudoTerminal(String term, int cols, int rows, int width, int height);

		/// <summary>
		/// Return the exit code of the process once complete. Call this after the session has been closed to obtain the exit 
		/// code of the process. It MAY or MAY NOT be sent by the server.
		/// </summary>
		int ExitCode
		{
			get;
		}

		/// <summary>
		/// Change the dimensions of the terminal window. This method should be called when the session is active and the user 
		/// or application changes the size of the terminal window. 
		/// </summary>
		/// <param name="cols">The number of columns.</param>
		/// <param name="rows">The number of rows.</param>
		/// <param name="width">The width of the terminal in pixels (informational only).</param>
		/// <param name="height">The height of the terminal in pixels (informational only).</param>
		void ChangeTerminalDimensions(int cols, int rows, int width, int height);
	}
}
