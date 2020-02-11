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
using Maverick.Crypto.IO;

namespace Maverick.SSH
{

	
	/// <summary>
	/// When a client requests a pseudo terminal it informs the server of any terminal modes that 
	/// it knows of. This is typically used in situations where advance terminal configuration is 
	/// required but it can also be used to perform simple configuration such as turning off character 
	/// echo.
	/// </summary>
	/// <remarks>
	/// The server may ignore some of the modes set if it does not support them.
	/// </remarks>
	/// <example>
	/// SSHSession session = ssh.OpenSessionChannel();	
	/// PseudoTerminalModes modes = new PseudoTerminalModes(ssh); 
	/// // Turning off echo	
	/// modes.SetTerminalMode(PseudoTerminalModes.ECHO, false);	
	/// // Setting the Input/Output baud rate	
	/// modes.SetTerminalMode(PseudoTerminalModes.TTY_OP_ISPEED, 38400);	
	/// modes.SetTerminalMode(PseudoTerminalModes.TTY_OP_OSPEED, 38400);    
	/// 
	/// // Passing into the pseudo terminal request
	/// session.RequestPseudoTerminal("vt100", 80, 24, 0, 0, modes);
	/// </example>
	public class PseudoTerminalModes
	{


		/// <summary>
		/// Interrupt character; 255 if none.
		/// </summary>
		public static readonly int VINTR = 1;

		/// <summary>
		/// The quit character (sends SIGQUIT signal on POSIX systems). 
		/// </summary>
		public static readonly int VQUIT = 2;

		/// <summary>
		/// Erase the character to left of the cursor. 
		/// </summary>
		public static readonly int VERASE = 3;

		/// <summary>
		/// Kill the current input line.
		/// </summary>
		public static readonly int VKILL = 4;

		/// <summary>
		/// End-of-file character (sends EOF from the terminal).
		/// </summary>
		public static readonly int VEOF = 5;

		/// <summary>
		/// End-of-line character in addition to carriage return and/or linefeed.
		/// </summary>
		public static readonly int VEOL = 6;

		/// <summary>
		/// Additional end-of-line character. 
		/// </summary>
		public static readonly int VEOL2 = 7;

		/// <summary>
		/// Continues paused output (normally control-Q). 
		/// </summary>
		public static readonly int VSTART = 8;

		/// <summary>
		/// Pauses output (normally control-S). 
		/// </summary>
		public static readonly int VSTOP = 9;

		/// <summary>
		/// Suspends the current program. 
		/// </summary>
		public static readonly int VSUSP = 10;

		/// <summary>
		/// Another suspend character. 
		/// </summary>
		public static readonly int VDSUSP = 11;

		/// <summary>
		/// Reprints the current input line. 
		/// </summary>
		public static readonly int VREPRINT = 12;

		/// <summary>
		/// Erases a word left of cursor. 
		/// </summary>
		public static readonly int VWERASE = 13;

		/// <summary>
		/// Enter the next character typed literally, even if it is a special character 
		/// </summary>
		public static readonly int VLNEXT = 14;

		/// <summary>
		/// Character to flush output. 
		/// </summary>
		public static readonly int VFLUSH = 15;


		/// <summary>
		/// Switch to a different shell layer. 
		/// </summary>
		public static readonly int VSWITCH = 16;

		/// <summary>
		/// Prints system status line (load, command, pid, etc). 
		/// </summary>
		public static readonly int VSTATUS = 17;

		/// <summary>
		/// Toggles the flushing of terminal output. 
		/// </summary>
		public static readonly int VDISCARD = 18;

		/// <summary>
		/// The ignore parity flag.  The parameter SHOULD be 0 if this flag is FALSE set,
		/// and 1 if it is TRUE. 
		/// </summary>
		public static readonly int IGNPAR = 30;

		/// <summary>
		/// Mark parity and framing errors. 
		/// </summary>
		public static readonly int PARMRK = 31;

		/// <summary>
		/// Enable checking of parity errors. 
		/// </summary>
		public static readonly int INPCK = 32;


		/// <summary>
		/// Strip 8th bit off characters. 
		/// </summary>
		public static readonly int ISTRIP = 33;

		/// <summary>
		/// Map NL into CR on input. 
		/// </summary>
		public static readonly int INLCR = 34;

		/// <summary>
		/// Ignore CR on input. 
		/// </summary>
		public static readonly int IGNCR = 35;

		/// <summary>
		/// Map CR to NL on input. 
		/// </summary>
		public static readonly int ICRNL = 36;

		/// <summary>
		/// Translate uppercase characters to lowercase. 
		/// </summary>
		public static readonly int IUCLC = 37;

		/// <summary>
		/// Enable output flow control. 
		/// </summary>
		public static readonly int IXON = 38;

		/// <summary>
		/// Any char will restart after stop. 
		/// </summary>
		public static readonly int IXANY = 39;

		/// <summary>
		/// Enable input flow control. 
		/// </summary>
		public static readonly int IXOFF = 40;

		/// <summary>
		/// Ring bell on input queue full. 
		/// </summary>
		public static readonly int IMAXBEL = 41;

		/// <summary>
		/// Enable signals INTR, QUIT, [D]SUSP. 
		/// </summary>
		public static readonly int ISIG = 50;

		/// <summary>
		/// Canonicalize input lines. 
		/// </summary>
		public static readonly int ICANON = 51;

		/// <summary>
		/// Enable input and output of uppercase characters by preceding their lowercase equivalents with "\". 
		/// </summary>
		public static readonly int XCASE = 52;

		/// <summary>
		/// Enable echoing. 
		/// </summary>
		public static readonly int ECHO = 53;

		/// <summary>
		/// Visually erase chars. 
		/// </summary>
		public static readonly int ECHOE = 54;

		/// <summary>
		/// Kill character discards current line. 
		/// </summary>
		public static readonly int ECHOK = 55;

		/// <summary>
		/// Echo NL even if ECHO is off.
		/// </summary>
		public static readonly int ECHONL = 56;

		/// <summary>
		/// Don't flush after interrupt. 
		/// </summary>
		public static readonly int NOFLSH = 57;

		/// <summary>
		/// Stop background jobs from output. 
		/// </summary>
		public static readonly int TOSTOP = 58;

		/// <summary>
		/// Enable extensions. 
		/// </summary>
		public static readonly int IEXTEN = 59;

		/// <summary>
		/// Echo control characters as ^(Char). 
		/// </summary>
		public static readonly int ECHOCTL = 60;

		/// <summary>
		/// Visual erase for line kill. 
		/// </summary>
		public static readonly int ECHOKE = 61;

		/// <summary>
		/// Retype pending input. 
		/// </summary>
		public static readonly int PENDIN = 62;

		/// <summary>
		/// Enable output processing. 
		/// </summary>
		public static readonly int OPOST = 70;

		/// <summary>
		/// Convert lowercase to uppercase. 
		/// </summary>
		public static readonly int OLCUC = 71;

		/// <summary>
		/// Map NL to CR-NL. 
		/// </summary>
		public static readonly int ONLCR = 72;

		/// <summary>
		/// Translate carriage return to newline (output).
		/// </summary>
		public static readonly int OCRNL = 73;

		/// <summary>
		/// Translate newline to carriage return-newline (output). 
		/// </summary>
		public static readonly int ONOCR = 74;

		/// <summary>
		/// Newline performs a carriage return (output). 
		/// </summary>
		public static readonly int ONLRET = 75;

		/// <summary>
		/// 7 bit mode. 
		/// </summary>
		public static readonly int CS7 = 90;

		/// <summary>
		/// 8 bit mode. 
		/// </summary>
		public static readonly int CS8 = 91;

		/// <summary>
		/// Parity enable. 
		/// </summary>
		public static readonly int PARENB = 92;

		/// <summary>
		/// Odd parity, else even. 
		/// </summary>
		public static readonly int PARODD = 93;

		/// <summary>
		/// Specifies the input baud rate in bits per second. 
		/// </summary>
		public static readonly int TTY_OP_ISPEED = 128;

		/// <summary>
		/// Specifies the output baud rate in bits per second. 
		/// </summary>
		public static readonly int TTY_OP_OSPEED = 129;

		int version;
		ByteBuffer encodedModes = new ByteBuffer();
		byte[] modes = null;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="client"></param>
		public PseudoTerminalModes(SSHClient client)
		{
			version = client.Version;
		}

		/// <summary>
		/// Set an terminal mode integer value.
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="setting"></param>
		public void SetTerminalMode(int mode, int setting)
		{

            encodedModes.Write(mode);

            if (version == 1 && mode <= 127) {
                encodedModes.Write(setting);
            } else {
                encodedModes.WriteInt(setting);
            }

	    }

		/// <summary>
		/// Set a terminal mode bool value.
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="setting"></param>
		public void SetTerminalMode(int mode, bool setting)
		{
			SetTerminalMode(mode, setting ? 1 : 0);
		}

		/// <summary>
		/// Reset the modes to their default state.
		/// </summary>
		public void Reset()
		{
			encodedModes.Reset();
			modes = null;
		}

		/// <summary>
		/// Encode the modes into a byte array.
		/// </summary>
		/// <returns></returns>
		public byte[] ToByteArray() 
		{
	
			if(modes==null)
			{
				encodedModes.Write(0);
				modes = encodedModes.ToByteArray();
			}
	
			return modes;
		}
	}
}
