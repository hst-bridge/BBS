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
using Maverick.Crypto.Digests;
using Maverick.SSH.Packets;
using System.Security.Cryptography;

namespace Maverick.SSH2.Algorithms
{
	/// <summary>
	/// Summary description for DiffieHellmanGroup1SHA1.
	/// </summary>
	public class DiffieHellmanGroup1SHA1 : SSH2KeyExchange
	{
		/// <summary>
		/// 
		/// </summary>
		public const System.String DIFFIE_HELLMAN_GROUP1_SHA1 = "diffie-hellman-group1-sha1";
		
		internal const int SSH_MSG_KEXDH_INIT = 30;
		internal const int SSH_MSG_KEXDH_REPLY = 31;
		
		internal static BigInteger g = BigInteger.valueOf(2);
		internal static BigInteger p = new BigInteger("FFFFFFFFFFFFFFFFC90FDAA22168C234C4C6628B80DC1CD1" 
		+ "29024E088A67CC74020BBEA63B139B22514A08798E3404DD" 
		+ "EF9519B3CD3A431B302B0A6DF25F14374FE1356D6D51C245" 
		+ "E485B576625E7EC6F44C42E9A637ED6B0BFF5CB6F406B7ED" 
		+ "EE386BFB5A899FA5AE9F24117C4B1FE649286651ECE65381" 
		+ "FFFFFFFFFFFFFFFF", 16);
		internal BigInteger e = null;
		internal BigInteger f = null;
		internal BigInteger x = null;
		internal BigInteger y = null;
		internal System.String clientId;
		internal System.String serverId;
		internal byte[] clientKexInit;
		internal byte[] serverKexInit;

		/// <summary>
		/// 
		/// </summary>
		public DiffieHellmanGroup1SHA1()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="serverId"></param>
		/// <param name="clientKexInit"></param>
		/// <param name="serverKexInit"></param>
		public override void PerformClientExchange(String clientId,
			String serverId,
			byte[] clientKexInit,
			byte[] serverKexInit)
		{
			this.clientId = clientId;
			this.serverId = serverId;
			this.clientKexInit = clientKexInit;
			this.serverKexInit = serverKexInit;

			BigInteger q = p.subtract(BigInteger.ONE).divide(g);

			do 
			{
				x = new BigInteger(p.bitLength(), new RNGCryptoServiceProvider());
			}
			while((x.compareTo(BigInteger.ONE) < 0)
				&& (x.compareTo(q) > 0));

			e = g.modPow(x, p);

			if(e.compareTo(BigInteger.ONE) < 0 ||
				e.compareTo(p.subtract(BigInteger.ONE)) > 0) 
			{
				throw new SSHException("Key exchange failed to generate e value", SSHException.INTERNAL_ERROR);
			}
				
			SSHPacket packet = transport.GetSSHPacket(true);
			packet.WriteByte(SSH_MSG_KEXDH_INIT);
			packet.WriteBigInteger(e);

			transport.SendMessage(packet);

			packet = transport.NextMessage();

			if(packet.MessageID != SSH_MSG_KEXDH_REPLY)
				throw new SSHException("Expected SSH_MSG_KEXDH_REPLY but got message id "
					+ packet.MessageID, SSHException.KEY_EXCHANGE_FAILED);

			hostKey = packet.ReadBinaryString();
			f = packet.ReadBigInteger();
			signature = packet.ReadBinaryString();

			secret = f.modPow(x, p);

			CalculateExchangeHash();

		}

		/// <summary>
		/// 
		/// </summary>
		protected internal virtual void  CalculateExchangeHash()
		{
			Hash hash = new Hash(new SHA1CryptoServiceProvider());

			// The local software version comments
			hash.WriteString(clientId);
			
			// The remote software version comments
			hash.WriteString(serverId);
			
			// The local kex init payload
			hash.WriteBinaryString(clientKexInit);
						
			// The remote kex init payload
			hash.WriteBinaryString(serverKexInit);
			
			// The host key
			hash.WriteBinaryString(hostKey);
						
			// The diffie hellman e value
			hash.WriteBigInteger(e);
			
			// The diffie hellman f value
			hash.WriteBigInteger(f);
			
			// The diffie hellman k value
			hash.WriteBigInteger(secret);
			
			// Do the final outWrite
			exchangeHash = hash.DoFinal();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="messageid"></param>
		/// <returns></returns>
		public override bool IsKeyExchangeMessage(int messageid)
		{
			switch (messageid)
			{
				
				case SSH_MSG_KEXDH_INIT: 
				case SSH_MSG_KEXDH_REPLY: 
					return true;
				
				default: 
					return false;
				
			}
		}
	}
}