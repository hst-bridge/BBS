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
using System.Net.Sockets;
using System.Globalization;
using System.Net;
using Maverick.HTTP;
using Maverick.Crypto.Util;
using Maverick.Crypto.IO;

namespace Maverick.SSH
{

	/// <summary>
	/// An enumeration of values for defining the proxy type of a <see cref="Maverick.SSH.TcpClientTransport"/>.
	/// </summary>
	public enum TcpClientProxy 
	{
		/// <summary>
		/// No proxy is required for the connection.
		/// </summary>
		NONE,

		/// <summary>
		/// Use HTTP proxy
		/// </summary>
		HTTP,

		/// <summary>
		/// Use a SOCKS 4 proxy
		/// </summary>
		SOCKS4,

		/// <summary>
		/// Use a SOCKS 5 proxy
		/// </summary>
		SOCKS5
	}

	/// <summary>
	/// A basic implementation of an SSH transport using System.Net.Socket and a 
	/// NetworkStream.
	/// </summary>
	/// <remarks>
	/// This implementation now supports the use of a proxy to bypass a firewall. The supported
	/// proxies are HTTP with basic authentication, SOCKS4 and SOCKS5 with cleartext authentication
	/// (RFC1929).
	/// </remarks>
	/// <example>
	/// <code>
	/// SSHConnector con = SSHConnector.Create();
	/// 
	/// // Setup the proxy settings
	/// TcpClientTransport transport = new TcpClientTransport();
	/// transport.ProxyType = TcpClientProxy.HTTP;
	/// transport.ProxyHost = "myproxy.net";
	/// transport.ProxyPort = 8888;
	/// transport.Username = "joeb";
	/// transport.Password = "xxxxxxxx";
	/// 
	/// // Ensure that the transport is connected before passing into the SSHConnector
	/// transport.Connect(hostname, port);
	/// 
	/// // Create the SSHClient
	/// ssh = con.Connect(transport, username, true);
	/// </code>
	/// </example>
	public class TcpClientTransport : SSHTransport
	{
		internal Socket socket;
		internal String hostname;
		internal EndPoint remoteEndPoint;
		internal int port;
		internal NetworkStream netStream;

		internal String proxyHost = null;
		internal int proxyPort = -1;
		internal TcpClientProxy type = TcpClientProxy.NONE;
		internal String username = null;
		internal String password = null;

		private const int SOCKS4 = 0x04;
		private const int SOCKS5 = 0x05;
		private const int CONNECT = 0x01;
		private const int NULL_TERMINATION = 0x00;

		private static String[] SOCKSV5_ERROR = {
			"Success", "General SOCKS server failure",
			"Connection not allowed by ruleset", "Network unreachable",
			"Host unreachable", "Connection refused", "TTL expired",
			"Command not supported", "Address type not supported"
		};

		private static String[] SOCKSV4_ERROR = {
			"Request rejected or failed",
			"SOCKS server cannot connect to identd on the client",
			"The client program and identd report different user-ids"
		};


		/// <summary>
		/// Create the transport connecting to the destination host supplied.
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <exception cref="Maverick.SSH.SSHException">
		/// </exception>
		public TcpClientTransport(String hostname, int port)
		{
			Connect(hostname, port);
		}

		/// <summary>
		/// Construct a transport directly from a connected socket.
		/// </summary>
		/// <param name="socket">The socket to use as a transport.</param>
		public TcpClientTransport(Socket socket)
		{
			this.socket = socket;
			this.remoteEndPoint = socket.RemoteEndPoint;
			this.netStream = new NetworkStream(socket, false);
		}

		/// <summary>
		/// Construct an uninitialized transport.
		/// </summary>
		/// <remarks>
		/// It is important that when using an uninitialized transport that you 
		/// call the <see cref="Maverick.SSH.TcpClientTransport.Connect"/> method
        /// before passing to <see cref="Maverick.SSH.SSHConnector.Connect(Maverick.SSH.SSHTransport, string, bool, Maverick.SSH.SSHContext, Maverick.SSH.SSHStateListener)"/>.
		/// </remarks>
		public TcpClientTransport()
		{
		}

		/// <summary>
		/// Set or get the proxy hostname for this connection.
		/// </summary>
		public String ProxyHost
		{
			get
			{
				return proxyHost;
			}

			set
			{
				this.proxyHost = value;
			}
		}

		/// <summary>
		/// Set or get the proxy port for this connection.
		/// </summary>
		public int ProxyPort
		{
			get
			{
				return proxyPort;
			}

			set
			{
				this.proxyPort = value;
			}
		}

		/// <summary>
		/// Set or get username for proxy authentication.
		/// </summary>
		public String Username
		{
			get
			{
				return username;
			}

			set
			{
				this.username = value;
			}
		}

		/// <summary>
		/// Set or get the password for proxy authentication.
		/// </summary>
		public String Password
		{
			get
			{
				return password;
			}

			set
			{
				this.password = value;
			}
		}

		/// <summary>
		/// Set or get the type of proxy to be used upon connection.
		/// </summary>
		public TcpClientProxy ProxyType
		{
			get 
			{
				return type;
			}

			set
			{
				this.type = value;
			}
		}

		/// <summary>
		/// Connect the socket to the host and port specified. If any proxy is configured this
		/// will be used and a connection made through the proxy to the destination.
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		public void Connect(String hostname, int port)
		{
			try 
			{
				this.hostname = hostname;
				this.port = port;

				if(type != TcpClientProxy.NONE) 
				{
					if(proxyHost==null || proxyPort==-1)
						throw new IOException("Proxy type property set but no proxy host or port details were provided!");

					socket = new Socket(AddressFamily.InterNetwork,
						SocketType.Stream, ProtocolType.Tcp);
					//socket.Connect(new IPEndPoint(
                    //    Dns.GetHostEntry(proxyHost).AddressList[0], proxyPort));
                    // 20140701 C³@wangdan
                    socket.Connect(new IPEndPoint(Dns.GetHostAddresses(hostname)[0], proxyPort));
					
					this.remoteEndPoint = socket.RemoteEndPoint;
					this.netStream = new NetworkStream(socket, true);

					switch(type)
					{
						case TcpClientProxy.HTTP:
						{
							ConnectHTTPProxy();
							break;
						}
						case TcpClientProxy.SOCKS4:
						{
							ConnectSOCKS4Proxy();
							break;
						}
						case TcpClientProxy.SOCKS5:
						{
							ConnectSOCKS5Proxy();
							break;
						}

					}

				} 
				else 
				{
					socket = new Socket(AddressFamily.InterNetwork,
						SocketType.Stream, ProtocolType.Tcp);
					//socket.Connect(new IPEndPoint(
						//Dns.GetHostEntry(hostname).AddressList[0], port));
                    // 20140701 C³@wangdan
                    socket.Connect(new IPEndPoint(Dns.GetHostAddresses(hostname)[0], port));

					this.remoteEndPoint = socket.RemoteEndPoint;
					this.netStream = new NetworkStream(socket, true);
				}
			}
			catch(Exception ex)
			{
				throw new SSHException("Failed to connect to " + hostname + ": " + ex.Message, 
					SSHException.CONNECT_FAILED);
			}
		}

		internal void ConnectSOCKS5Proxy() 
		{

			byte[] tmp = {
				(byte) SOCKS5, (byte) 0x02, (byte) 0x00, (byte) 0x02
			};

			netStream.Write(tmp, 0, tmp.Length);
			netStream.Flush();

			int res = netStream.ReadByte();

			if (res == -1) 
			{
				throw new IOException("SOCKS5 server " + proxyHost + ":" +
					proxyPort + " disconnected");
			}

			if (res != 0x05) 
			{
				throw new IOException("Invalid response from SOCKS5 server (" +
					res + ") " + proxyHost + ":" + proxyPort);
			}

			int method = netStream.ReadByte();

			if (res == -1) 
			{
				throw new IOException("SOCKS5 server " + proxyHost + ":" +
					proxyPort + " disconnected");
			}

			switch(method)
			{
				case 0x00:
				{
					break;
				}
				case 0x02:
				{	
					if(username==null || password==null)
						throw new IOException("Username/password required for SOCKS5 proxy");

					ByteBuffer buffer = new ByteBuffer();
					buffer.WriteByte(0x01);
					tmp = SupportClass.ToByteArray(username);
					buffer.WriteByte(tmp.Length);
					buffer.WriteBytes(tmp);
					tmp = SupportClass.ToByteArray(password);
					buffer.WriteByte(tmp.Length);
					buffer.WriteBytes(tmp);
					tmp = buffer.ToByteArray();

					netStream.Write(tmp, 0, tmp.Length);
					netStream.Flush();

					res = netStream.ReadByte();

					if ((res != 0x01) && (res != 0x05)) 
					{
						throw new IOException("Invalid response from SOCKS5 server (" +
							res + ") " + proxyHost + ":" + proxyPort);
					}

					if (netStream.ReadByte() != 0x00) 
					{
						throw new IOException("Invalid username/password for SOCKS5 server");
					}

					break;
				}

				default:
					throw new IOException("Unsupported SOCKS5 authentication type " + method);
			}

			ByteBuffer buf = new ByteBuffer();
			buf.WriteByte(SOCKS5);
			buf.WriteByte(0x01);
			buf.WriteByte(0x00);
			buf.WriteByte(0x03);
			tmp = SupportClass.ToByteArray(hostname);
			buf.WriteByte(tmp.Length);
			buf.WriteBytes(tmp);
			buf.WriteByte((port >> 8) & 0xFF);
			buf.WriteByte(port & 0xFF);

			tmp = buf.ToByteArray();
			netStream.Write(tmp, 0, tmp.Length);

			res = netStream.ReadByte();

			if (res != 0x05) 
			{
				throw new IOException("Invalid response from SOCKS5 server (" +
					res + ") " + proxyHost + ":" + proxyPort);
			}

			int status = netStream.ReadByte();

			if (status != 0x00) 
			{
				if ((status > 0) && (status < 9)) 
				{
					throw new IOException(
						"SOCKS5 server unable to connect, reason: " +
						SOCKSV5_ERROR[status]);
				} 
				else 
				{
					throw new IOException(
						"SOCKS5 server unable to connect, reason: " + status);
				}
			}

			netStream.ReadByte();

			int aType = netStream.ReadByte();
			byte[] data = new byte[255];

			switch(aType) 
			{
				case 0x01:

					if (netStream.Read(data, 0, 4) != 4) 
					{
						throw new IOException("SOCKS5 error reading address");
					}
					break;

				case 0x03:

					int n = netStream.ReadByte();

					if (netStream.Read(data, 0, n) != n) 
					{
						throw new IOException("SOCKS5 error reading address");
					}

					break;

				default:
					throw new IOException("SOCKS5 gave unsupported address type: " + aType);
			}

			if (netStream.Read(data, 0, 2) != 2) 
			{
				throw new IOException("SOCKS5 error reading port");
			}
		}

		internal void ConnectSOCKS4Proxy()
		{
			if(username==null)
				throw new IOException("SOCKS4 requires username property set");

			ByteBuffer request = new ByteBuffer();
			request.Write(SOCKS4);
			request.Write(CONNECT);
			request.Write((port >> 8) & 0xFF);
            request.Write(port & 0xFF);
            request.WriteBytes(Dns.GetHostEntry(hostname).AddressList[0].GetAddressBytes());
            request.WriteBytes(SupportClass.ToByteArray(username));
            request.Write(NULL_TERMINATION);
			
			byte[] tmp = request.ToByteArray();
			netStream.Write(tmp, 0, tmp.Length);
			netStream.Flush();

			int result = netStream.ReadByte();

			if (result == -1) 
			{
				throw new IOException("SOCKS4 server " + proxyHost + ":" +
					proxyPort + " disconnected");
			}

			if (result != 0x00) 
			{
				throw new IOException("Invalid response from SOCKS4 server (" +
					result + ") " + proxyHost + ":" + proxyPort);
			}

			int code = netStream.ReadByte();

			if (code == -1) 
			{
				throw new IOException("SOCKS4 server " + proxyHost + ":" +
					proxyPort + " disconnected");
			}

			if (code != 90) 
			{
				if ((code > 90) && (code < 93)) 
				{
					throw new IOException(
						"SOCKS4 server unable to connect, reason: " +
						SOCKSV4_ERROR[code - 91]);
				} 
				else 
				{
					throw new IOException(
						"SOCKS4 server unable to connect, reason: " + code);
				}
			}

			byte[] data = new byte[6];

			if (netStream.Read(data, 0, 6) != 6) 
			{
				throw new IOException(
					"SOCKS4 error reading destination address/port");
			}

		}

		internal void ConnectHTTPProxy() 
		{

			HTTPRequest request = new HTTPRequest("CONNECT " 
													+ hostname 
													+ ":" 
													+ port 
													+ " HTTP/1.0");
			request.SetHeaderField("User-Agent", "Maverick.NET");
			request.SetHeaderField("Pragma", "No-Cache");
			request.SetHeaderField("Host", hostname);
			request.SetHeaderField("Proxy-Connection", "Keep-Alive");

			byte[] tmp;

			if(username!=null) 
			{
				tmp = SupportClass.ToByteArray(username + ":" + (password==null ? "" : password));
				request.SetHeaderField("Proxy-Authorization",
					"Basic " + Convert.ToBase64String(tmp, 0, tmp.Length));
			}

			tmp = SupportClass.ToByteArray(request.ToString());
			netStream.Write(tmp, 0, tmp.Length);

			HTTPResponse response = new HTTPResponse(netStream);

			if(response.Status == 407) 
			{
				throw new IOException("Proxy authentication required method=" 
					+ response.AuthenticationMethod 
					+ " realm=" 
					+ response.AuthenticationRealm);
			}

			if ((response.Status < 200) || (response.Status > 299)) 
			{
				throw new IOException("Proxy tunnel setup failed: " +
					response.GetStartLine());
			}
		}

		internal Socket GetSocket() 
		{
			return socket;
		}

		/// <summary>
		/// Get the stream
		/// </summary>
		/// <returns></returns>
		public Stream GetStream() 
		{
			return netStream;
		}

		/// <summary>
		/// Returns the address or name of the remote host.
		/// </summary>
		public String Hostname
		{
			get
			{
				return ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
			}
		}


		/// <summary>
		/// Returns the port of the remote host.
		/// </summary>
		public int Port
		{
			get
			{
				return ((IPEndPoint)socket.RemoteEndPoint).Port;
			}
		}

		/// <summary>
		/// Close the Socket and the NetworkStream.
		/// </summary>
		public void Close() 
		{
			socket.Close();
			netStream.Close();
		}

		/// <summary>
		/// Duplicates the current instance by returning a new connection connected to the 
		/// same remote host and port.
		/// </summary>
		/// <returns></returns>
		public SSHTransport Duplicate()
		{

			//rather than creating a new transport from hostname, port, create it from the socket.remoteendpoint, so that can create the connetion usign ? using either a socket or a sockettransport
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			// Connects to the host using IPEndPoint.

			socket.Connect(remoteEndPoint);
			
			return new TcpClientTransport(socket);
			//			return new TcpClientTransport(hostname, port);
		}
	}
}
