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
using Maverick.SSH;

namespace Maverick.SSH2
{
	/// <summary>
	/// This class implements an SSH2 forwarding channel.
	/// </summary>
	public class SSH2ForwardingChannel : SSH2Channel, SSHTunnel
	{

		/// <summary>
		/// The name of the remote host where data is forwarded to.
		/// </summary>
		public System.String Hostname
		{
			get
			{
				return host;
			}
			
		}
		
		
		/// <summary>
		/// The port to which data is being forwarded to.
		/// </summary>
		public int Port
		{
			get
			{
				return port;
			}
			
		}
		
		/// <summary>
		/// The name of the originating host.
		/// </summary>
		public System.String OriginatingHost
		{
			get
			{
				return originatingHost;
			}
			
		}
		
		/// <summary>
		/// The port of the originating host.
		/// </summary>
		public int OriginatingPort
		{
			get
			{
				return originatingPort;
			}
			
		}
		
		/// <summary>
		/// The source ip address of the connection that is being forwarded. 
		/// </summary>
		public System.String ListeningAddress
		{
			get
			{
				return listeningAddress;
			}
			
		}
		
		/// <summary>
		/// The source port of the connection being forwarded. 
		/// </summary>
		public int ListeningPort
		{
			get
			{
				return listeningPort;
			}
			
		}

		/// <summary>
		/// The type of forwarding in use by this tunnel.
		/// </summary>
		public ForwardingChannelType ForwardingType
		{
			get
			{
				if(Name.Equals(SSH2ForwardingChannel.LOCAL_FORWARDING_CHANNEL))
					return ForwardingChannelType.LOCAL;
				else if(Name.Equals(SSH2ForwardingChannel.REMOTE_FORWARDING_CHANNEL))
					return ForwardingChannelType.REMOTE;
				else
					return ForwardingChannelType.X11;
			}
		}
		
		/// <summary>
		/// The connection being forwarded (local forwarding) or the destination of the forwarding 
		/// (remote forwarding). 
		/// </summary>	
		public Object Transport
		{
			get
			{
				return transport;
			}
			
		}
		
		internal const System.String X11_FORWARDING_CHANNEL = "x11";
		internal const System.String LOCAL_FORWARDING_CHANNEL = "direct-tcpip";
		internal const System.String REMOTE_FORWARDING_CHANNEL = "forwarded-tcpip";
		
		internal const System.String X11AUTH_PROTO = "MIT-MAGIC-COOKIE-1";

		internal Object transport;
		internal System.String host;
		internal int port;
		internal System.String listeningAddress;
		internal int listeningPort;
		internal System.String originatingHost;
		internal int originatingPort;
		internal byte[] buf;
		internal bool hasSpoofedCookie = false;
		internal int idx = 0;
		internal int requiredLength = 12; // header len
		internal int protocolLength;
		internal int cookieLength;

		/// <summary>
		/// Construct an uninitialized forwarding channel.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="localwindow"></param>
		/// <param name="localpacket"></param>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="listeningAddress"></param>
		/// <param name="listeningPort"></param>
		/// <param name="originatingHost"></param>
		/// <param name="originatingPort"></param>
		/// <param name="transport"></param>
		/// <param name="delayWindow"></param>
		public SSH2ForwardingChannel(System.String name, 
			int localwindow, 
			int localpacket, 
			System.String host, 
			int port, 
			System.String listeningAddress, 
			int listeningPort, 
			System.String originatingHost, 
			int originatingPort, 
			Object transport, 
			bool delayWindow)
			: base(name, localwindow, localpacket, delayWindow)
		{
			this.transport = transport;
			this.host = host;
			this.port = port;
			this.listeningAddress = listeningAddress;
			this.listeningPort = listeningPort;
			this.originatingHost = originatingHost;
			this.originatingPort = originatingPort;

			this.buf = new byte[1024];
		}
			
		/// <summary>
		/// This is an unsupported operation,
		/// </summary>
		/// <returns></returns>
		public SSHTransport Duplicate()
		{
			throw new SSHException("SSH tunnels cannot be duplicated!", SSHException.BAD_API_USAGE);
		}
		
		/// <summary>
		/// Process channel data and forward to the host.
		/// </summary>
		/// <param name="b"></param>
		/// <param name="off"></param>
		/// <param name="len"></param>
		protected internal override void  ProcessStandardData(byte[] b, int off, int len)
		{
			if (Name.Equals(X11_FORWARDING_CHANNEL))
			{
				if (!hasSpoofedCookie)
				{
					int n;
					
					if (idx < 12)
					{
						n = ReadMore(b, off, len);
						len -= n;
						off += n;
						if (requiredLength == 0)
						{
							if (buf[0] == 0x42)
							{
								protocolLength = (buf[6] << 8) | buf[7];
								cookieLength = (buf[8] << 8) | buf[9];
							}
							else if (buf[0] == 0x6c)
							{
								protocolLength = (buf[7] << 8) | buf[6];
								cookieLength = (buf[9] << 8) | buf[8];
							}
							else
							{
								Close();
								throw new SSHException("Corrupt X11 authentication packet", SSHException.CHANNEL_FAILURE);
							}

							requiredLength = (protocolLength + 0x03) & ~ 0x03;
							requiredLength += (cookieLength + 0x03) & ~ 0x03;

							if (requiredLength + idx > buf.Length)
							{
								Close();
								throw new SSHException("Corrupt X11 authentication packet", SSHException.CHANNEL_FAILURE);
							}
							if (requiredLength == 0)
							{
								Close();
								throw new SSHException("X11 authentication cookie not found", SSHException.CHANNEL_FAILURE);
							}
						}
					}
					
					// Read payload of authentication packet
					//
					if (len > 0)
					{
						n = ReadMore(b, off, len);
						len -= n;
						off += n;
						if (requiredLength == 0)
						{
							byte[] fakeCookie = connection.Context.X11AuthenticationCookie;
							char[] tmpChar;
							tmpChar = new char[buf.Length];
							buf.CopyTo(tmpChar, 0);
							System.String protoStr = new System.String(tmpChar, 12, protocolLength);
							byte[] recCookie = new byte[fakeCookie.Length];
							
							protocolLength = ((protocolLength + 0x03) & ~ 0x03);
							
							Array.Copy(buf, 12 + protocolLength, recCookie, 0, fakeCookie.Length);
							if (!X11AUTH_PROTO.Equals(protoStr) || !CompareCookies(fakeCookie, recCookie, fakeCookie.Length))
							{
								Close();
								throw new SSHException("Incorrect X11 cookie", SSHException.CHANNEL_FAILURE);
							}
							byte[] realCookie = connection.Context.X11RealCookie;
							if (realCookie.Length != cookieLength)
							{
								throw new SSHException("Invalid X11 cookie", SSHException.CHANNEL_FAILURE);
							}
							Array.Copy(realCookie, 0, buf, 12 + protocolLength, realCookie.Length);
							hasSpoofedCookie = true;
							base.ProcessStandardData(buf, 0, idx);
							buf = null;
						}
					}
					
					if (!hasSpoofedCookie || len == 0)
					{
						return ;
					}
				}
			}
			
			base.ProcessStandardData(b, off, len);
		}
		
		
		private bool CompareCookies(byte[] src, byte[] dst, int len)
		{
			int i = 0;
			for (; i < len; i++)
			{
				if (src[i] != dst[i])
				{
					break;
				}
			}
			return i == len;
		}
		
		private int ReadMore(byte[] b, int off, int len)
		{
			if (len > requiredLength)
			{
				Array.Copy(b, off, buf, idx, requiredLength);
				idx += requiredLength;
				len = requiredLength;
				requiredLength = 0;
			}
			else
			{
				Array.Copy(b, off, buf, idx, len);
				idx += len;
				requiredLength -= len;
			}
			return len;
		}
	}
}
