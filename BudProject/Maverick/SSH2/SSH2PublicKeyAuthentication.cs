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
using Maverick.SSH.Packets;
using Maverick.Crypto.IO;

namespace Maverick.SSH2
{
	/// <summary>
	/// Implementation of the SSH2 public key authentication mechanism.
	/// </summary>
	public class SSH2PublicKeyAuthentication : PublicKeyAuthentication, SSH2AuthenticationClient
	{
		const int SSH_MSG_USERAUTH_PK_OK = 60;

		/// <summary>
		/// Construct an initialized authentication instance.
		/// </summary>
		/// <param name="pair"></param>
		public SSH2PublicKeyAuthentication(SSHKeyPair pair) : base(pair)
		{
			
		}

		/// <summary>
		/// Construct an uninitialized public key authentication instance.
		/// </summary>
		public SSH2PublicKeyAuthentication()
		{

		}

		/// <summary>
		/// Attempts an authentication using the public/private key pair.
		/// </summary>
		/// <param name="authentication"></param>
		/// <param name="serviceName"></param>
		public void Authenticate(AuthenticationProtocol authentication,
			String serviceName)
		{
			ByteBuffer baw = new ByteBuffer();

			baw.WriteBinaryString(authentication.SessionIdentifier);
			baw.Write(AuthenticationProtocol.SSH_MSG_USERAUTH_REQUEST);
			baw.WriteString(Username);
			baw.WriteString(serviceName);
			baw.WriteString("publickey");
			baw.WriteBool(!VerifyOnly);

			byte[] encoded;
			String algorithm;			

			baw.WriteString(algorithm = KeyPair.PublicKey.Algorithm);
			baw.WriteBinaryString(encoded = KeyPair.PublicKey.GetEncoded());

			ByteBuffer baw2 = new ByteBuffer();
			// Generate the authentication request
			baw2.WriteBool(!VerifyOnly);
			baw2.WriteString(algorithm);
			baw2.WriteBinaryString(encoded);

			if(!VerifyOnly) 
			{
				byte[] signature = KeyPair.PrivateKey.Sign(baw.ToByteArray());
				
				// Format the signature correctly
				ByteBuffer sig = new ByteBuffer();
				sig.WriteString(algorithm);
				sig.WriteBinaryString(signature);
				baw2.WriteBinaryString(sig.ToByteArray());
			}

			authentication.SendRequest(Username,
				serviceName,
				"publickey",
				baw2.ToByteArray());

			SSHPacket packet = authentication.ReadMessage();

			if(packet.MessageID == SSH_MSG_USERAUTH_PK_OK) 
			{
				throw new SSH2AuthenticationResult(AuthenticationResult.PUBLIC_KEY_ACCEPTABLE, "");
			}
			else 
			{
				authentication.connection.transport.Disconnect("Unexpected message " + packet.MessageID + " received",
					DisconnectionReason.PROTOCOL_ERROR);

				throw new SSHException("Unexpected message "
					+ packet.MessageID
					+ " received",
					SSHException.PROTOCOL_VIOLATION);
			}

		}

	}
}
