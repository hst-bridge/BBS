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
using Maverick.SSH;
using Maverick.Crypto.IO;
using System.Text;

namespace Maverick.SSH2
{
	/// <summary>
	/// Implementation of an SSH2 <see cref="Maverick.SSH.SSHSession"/>
	/// </summary>
	public class SSH2Session : SSH2Channel, SSHSession
	{

		internal const int SSH_EXTENDED_DATA_STDERR = 1;
		internal SSH2ChannelStream stderr;
		internal bool flowControlEnabled = false;

        public const int EXIT_CODE_NOT_RECEIVED = 0xFFFFFF;

		internal int exitcode = EXIT_CODE_NOT_RECEIVED;
		internal String exitsignalinfo = "";
		internal SSH2Client client;

		/// <summary>
		/// An event that provides notification of the arrival of error data on the channel.
		/// </summary>
		public event DataListener ErrorStreamListener;

		/// <summary>
		/// Create an uninitialized SSH2 session.
		/// </summary>
		/// <param name="windowsize"></param>
		/// <param name="packetlen"></param>
		/// <param name="client"></param>
		public SSH2Session(int windowsize, int packetlen, SSH2Client client)
			: base("session", windowsize, packetlen)
		{
			this.client = client;
			this.stderr = createExtendedDataStream();
		}

		/// <summary>
		/// The client that created this session.
		/// </summary>
		public SSHClient Client
		{
			get
			{
				return client;
			}
		
		}

		/// <summary>
		/// Obtain a stream that contains the STDERR output of the session.
		/// </summary>
		/// <returns></returns>
		public System.IO.Stream GetStderrStream()
		{
			return stderr;
		}


		/// <summary>
		/// On many systems it is possible to determine whether a pseudo-terminal is using control-S/ control-Q flow control. When flow control is allowed it is often esirable to do the flow control at the client end to speed up responses to user requests. If this method returns true the client is allowed to do flow control using control-S and control-Q
		/// </summary>
		public bool IsFlowControlEnabled
		{
			get
			{
				return flowControlEnabled;
			}
			
		}

		/// <summary>
		/// Get the exit signal information, may be an empty string.
		/// </summary>
		public System.String ExitSignalInfo
		{
			get
			{
				return exitsignalinfo;
			}
			
		}

		/// <summary>
		/// Processes extended data and routes to the STDERR stream.
		/// </summary>
		/// <param name="typecode"></param>
		/// <param name="buf"></param>
		/// <param name="offset"></param>
		/// <param name="len"></param>
		protected internal override void  ProcessExtendedData(int typecode, byte[] buf, int offset, int len)
		{
			
			base.ProcessExtendedData(typecode, buf, offset, len);
			
			if (typecode == SSH_EXTENDED_DATA_STDERR)
			{
				stderr.FillInputBuffer(buf, offset, len);
			}
		}


		/// <summary>
		/// The remote process may require a pseudo terminal. Call this method before executing a command or starting a shell. 
		/// </summary>
		/// <param name="term">the terminal type e.g "vt100"</param>
		/// <param name="cols">the number of columns</param>
		/// <param name="rows">the number of rows</param>
		/// <param name="width">the width of the terminal (informational only, can be zero)</param>
		/// <param name="height">the height of the terminal (informational only, can be zero)</param>
		/// <returns>true if the pty was allocated, otherwise false</returns>
		public bool RequestPseudoTerminal(System.String term, int cols, int rows, int width, int height)
		{
			return RequestPseudoTerminal(term, cols, rows, width, height, new PseudoTerminalModes(client));
		}

		/// <summary>
		/// Fires an error stream listener event.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <param name="len"></param>
		protected override void FireErrorStreamListenerEvent(int type, byte[] buf, int off, int len)
		{
			try
			{
				if(type == SSH_EXTENDED_DATA_STDERR && ErrorStreamListener!=null)
					ErrorStreamListener(this, buf, off, len);
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine("WARNING! Exception caught in Maverick.NET event handler: " + ex.Message );
				System.Diagnostics.Trace.WriteLine(ex.StackTrace );
			}

		}
		
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
		public bool RequestPseudoTerminal(String term, int cols, int rows, int width, int height, PseudoTerminalModes modes)
		{
			
			try
			{
				ByteBuffer buf = new ByteBuffer();
				buf.WriteString(term);
				buf.WriteUINT32(cols);
				buf.WriteUINT32(rows);
				buf.WriteUINT32(width);
				buf.WriteUINT32(height);
				buf.WriteBinaryString(modes.ToByteArray());
				return SendRequest("pty-req", true, buf.ToByteArray());
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// Starts the users default shell.
		/// </summary>
		/// <returns></returns>
		public bool StartShell()
		{
			return SendRequest("shell", true, null);
		}
		
		/// <summary>
		/// Execute a command.
		/// </summary>
		/// <param name="cmd">The command to execute</param>
		/// <remarks>
		/// A successful command execution does not mean that the command itself executed and
		/// completed successfully. The return value simply indicates that the instruction was
		/// sent to the remote machine and was acknolwedged by the server. For
		/// example this may still return a <tt>true</tt> value if the command does not exist.
		/// </remarks>
		/// <returns>true if the command executed, otherwise false</returns>
		public bool ExecuteCommand(String cmd)
		{
			try
			{
				ByteBuffer buf = new ByteBuffer();
				buf.WriteString(cmd);
				return SendRequest("exec", true, buf.ToByteArray());
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

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
		public bool ExecuteCommand(String cmd, Encoding encoding)
		{
			try
			{
				ByteBuffer buf = new ByteBuffer();
				buf.WriteString(cmd, encoding);
				return SendRequest("exec", true, buf.ToByteArray());
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}
		
		/// <summary>
		/// Start an SSH2 subsystem.
		/// </summary>
		/// <param name="subsystem">The name of the subsystem e.g "sftp"</param>
		/// <returns></returns>
		public bool StartSubsystem(String subsystem)
		{
			try
			{
				ByteBuffer buf = new ByteBuffer();
				buf.WriteString(subsystem);
				return SendRequest("subsystem", true, buf.ToByteArray());
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

		internal virtual bool RequestX11Forwarding(bool singleconnection, 
			System.String protocol, 
			System.String cookie, 
			int screen)
		{
			try
			{
				ByteBuffer request = new ByteBuffer();
				request.WriteBool(singleconnection);
				request.WriteString(protocol);
				request.WriteString(cookie);
				request.WriteUINT32(screen);   // Always zero
				return SendRequest("x11-req", true, request.ToByteArray());
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// Sets an envionment variable in the sessions environment.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="theValue"></param>
		/// <remarks>
		/// This operation is not gaurenteed to work. Servers may not support the setting
		/// of environment variables and from our experiance most servers do not.
		/// </remarks>
		/// <returns></returns>
		public bool SetEnvironmentVariable(String name, String theValue)
		{
			try
			{
				ByteBuffer request = new ByteBuffer();
				request.WriteString(name);
				request.WriteString(theValue);
				
				return SendRequest("env", true, request.ToByteArray());
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

		
		/// <summary>
		/// Change the dimensions of the terminal window. This method should be called when the session is active and the user 
		/// or application changes the size of the terminal window. 
		/// </summary>
		/// <param name="cols">The number of columns.</param>
		/// <param name="rows">The number of rows.</param>
		/// <param name="width">The width of the terminal in pixels (informational only).</param>
		/// <param name="height">The height of the terminal in pixels (informational only).</param>
		public void  ChangeTerminalDimensions(int cols, int rows, int width, int height)
		{
			try
			{
				ByteBuffer request = new ByteBuffer();
				request.WriteUINT32(cols);
				request.WriteUINT32(rows);
				request.WriteUINT32(height);
				request.WriteUINT32(width);
				
				SendRequest("window-change", false, request.ToByteArray());
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

 
		/// <summary>
		/// Send a signal to the remote process. A signal can be delivered to the remote process using this method, some systems may not implement signals. The signal name should be one of the following values:<br/>
		/// <br/>
		/// ABRT<br/>
		/// ALRM<br/>
		/// FPE<br/>
		/// HUP<br/>
		/// ILL<br/>
		/// INT<br/>
		/// KILL<br/>
		/// PIPE<br/>
		/// QUIT<br/>
		/// SEGV<br/>
		/// TERM<br/>
		/// USR1<br/>
		/// USR2<br/>
		/// </summary>
		/// <param name="signal"></param>
		public void Signal(String signal)
		{
			try
			{
				ByteBuffer request = new ByteBuffer();
				request.WriteString(signal);
				
				SendRequest("signal", false, request.ToByteArray());
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// This overidden method handles the "exit-status", "exit-signal" and "xon-xoff" channel requests.
		/// </summary>
		/// <param name="requesttype">the name of the request</param>
		/// <param name="wantreply">specifies whether the remote side requires a success/failure message</param>
		/// <param name="requestdata">the request data</param>
		protected internal override void ChannelRequest(String requesttype, bool wantreply, byte[] requestdata)
		{
			
			try
			{
				if (requesttype.Equals("exit-status"))
				{
					if (requestdata != null)
					{
						exitcode = (int) ByteBuffer.ReadUINT32(requestdata, 0);
					}
				}
				
				if (requesttype.Equals("exit-signal"))
				{
					
					if (requestdata != null)
					{
						ByteBuffer buf = new ByteBuffer(requestdata);
						exitsignalinfo = "Signal=" + buf.ReadString() + " CoreDump=" + buf.ReadBool().ToString() + " Message=" + buf.ReadString();
					}
				}
				
				if (requesttype.Equals("xon-xoff"))
				{
					flowControlEnabled = (requestdata != null && requestdata[0] != 0);
				}
				
				base.ChannelRequest(requesttype, wantreply, requestdata);
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}
		
		
		/// <summary>
		/// Return the exit code of the process once complete. Call this after the session has been closed to obtain the exit 
		/// code of the process. It MAY or MAY NOT be sent by the server.
		/// </summary>
		public int ExitCode
		{
			get
			{
                if(exitcode==EXIT_CODE_NOT_RECEIVED)
                {
                   connection.SendGlobalRequest(new GlobalRequest("keep-alive@sshtools.com", null), true);
                }

				return exitcode;
			}
		}
		
		/// <summary>
		/// Determine whether the remote process was signalled.
		/// </summary>
		/// <returns></returns>
		public bool HasExitSignal()
		{
			return !exitsignalinfo.Equals("");
		}
	}
}
