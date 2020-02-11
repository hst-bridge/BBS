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
using Maverick.Crypto.Math;

namespace Maverick.SSH2
{
	/// <summary>
	/// This class provides an abstract base for all SSH2 key exchange mechanisms.
	/// </summary>
	public abstract class SSH2KeyExchange
	{

		/// <summary>
		/// The shared secret value.
		/// </summary>
		protected internal BigInteger secret;

		/// <summary>
		/// The exchange hash.
		/// </summary>
		protected internal byte[] exchangeHash;

		/// <summary>
		/// The server's encoded host key.
		/// </summary>
		protected internal byte[] hostKey;

		/// <summary>
		/// The signature provide by the server.
		/// </summary>
		protected internal byte[] signature;

		/// <summary>
		/// The connection's transport.
		/// </summary>
		protected internal TransportProtocol transport;

		/// <summary>
		/// Construct an ininitialized key exchange.
		/// </summary>
		public SSH2KeyExchange() 
		{
		}

		/// <summary>
		/// The "exchange hash" variable produced during key exchange.
		/// </summary>
		/// <remarks>
		/// The exchange hash is a value produced by hashing the contents of various
		/// data variables collated from the connection's configuration.
		/// </remarks>
		public byte[] ExchangeHash 
		{
			get
			{
				return exchangeHash;
			}
		}

		/// <summary>
		/// The SSH encoded public key of the server.
		/// </summary>
		public byte[] HostKey
		{
			get 
			{
				return hostKey;
			}
		}

		/// <summary>
		/// The shared secret created during key exchange.
		/// </summary>
		public BigInteger Secret
		{
			get
			{
				return secret;
			}
		}


		/// <summary>
		/// The signature provided by the server.
		/// </summary>
		public byte[] Signature
		{
			get
			{
				return signature;
			}
		}

		/// <summary>
		/// Initialize the key exchange.
		/// </summary>
		/// <param name="transport"></param>
		public void Init(TransportProtocol transport)
		{
			this.transport = transport;
		}

		/// <summary>
		/// Perform the client side of the key exchange operation.
		/// </summary>
		/// <param name="clientId">The identification string sent by the client.</param>
		/// <param name="serverId">The identification string sent by the server.</param>
		/// <param name="clientKexInit">The client's SSH_MSG_KEX_INIT message.</param>
		/// <param name="serverKexInit">The server's SSH_MSG_KEX_INIT message.</param>
		public abstract void PerformClientExchange(String clientId,
			String serverId,
			byte[] clientKexInit,
			byte[] serverKexInit);


		/// <summary>
		/// Called to determine whether a transport message belongs to the key exchange operation.
		/// </summary>
		/// <param name="messageid"></param>
		/// <returns></returns>
		public abstract bool IsKeyExchangeMessage(int messageid);

		/// <summary>
		/// Reset the key exchange so that the instance can be reused.
		/// </summary>
		public void Reset() 
		{
			exchangeHash = null;
			hostKey = null;
			signature = null;
			secret = null;
		}	
			
	}
}
