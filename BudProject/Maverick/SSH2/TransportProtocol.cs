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
using System.Collections;
using Maverick.Crypto.Ciphers;
using Maverick.Crypto.Math;
using Maverick.Crypto.IO;
using Maverick.Crypto.Digests;
using Maverick.SSH.Packets;
using System.Security.Cryptography;

namespace Maverick.SSH2
{

	internal enum TransportProtocolState
	{
		NEGOTIATING_PROTOCOL,
		PERFORMING_KEYEXCHANGE,
		CONNECTED,
		DISCONNECTED
	};

	internal delegate void TransportProtocolStateChangeEvent(TransportProtocol transport, TransportProtocolState state);

	/// <summary>
	/// An enumeration of disconnection reasons which are defined in the SSH2 transport
	/// protocol
	/// </summary>
	public enum DisconnectionReason 
	{
		/// <summary>
		/// The host being connected to is not allowed by system or user policy.
		/// </summary>
		HOST_NOT_ALLOWED = 1,
		/// <summary>
		/// An SSH protocol error occured.
		/// </summary>
		PROTOCOL_ERROR,
		/// <summary>
		/// The client and server failed to negotiate keys.
		/// </summary>
		KEY_EXCHANGE_FAILED,
		/// <summary>
		/// Reserved.
		/// </summary>
		RESERVED,
		/// <summary>
		/// A message was received that may have been corrupted or tampered with.
		/// </summary>
		MAC_ERROR,
		/// <summary>
		/// A compression error occured.
		/// </summary>
		COMPRESSION_ERROR,
		/// <summary>
		/// The service requested is not available.
		/// </summary>
		SERVICE_NOT_AVAILABLE,
		/// <summary>
		/// The version of the protocol requested is not supported.
		/// </summary>
		PROTOCOL_VERSION_NOT_SUPPORTED,
		/// <summary>
		/// The server's host key could not be verified.
		/// </summary>
		HOST_KEY_NOT_VERIFIABLE,
		/// <summary>
		/// The connection was lost.
		/// </summary>
		CONNECTION_LOST,
		/// <summary>
		/// The application disconnected.
		/// </summary>
		BY_APPLICATION,
		/// <summary>
		/// There are currently too many connections.
		/// </summary>
		TOO_MANY_CONNECTIONS,
		/// <summary>
		/// The user cancelled authentication.
		/// </summary>
		AUTH_CANCELLED_BY_USER,
		/// <summary>
		/// No more authentication methods are available.
		/// </summary>
		NO_MORE_AUTH_METHODS_AVAILABLE,
		/// <summary>
		/// The username cannot access the system or is illegal.
		/// </summary>
		ILLEGAL_USER_NAME
	};

	/// <summary>
	/// This class implements the SSH2 transport protocol.
	/// </summary>
	public class TransportProtocol : SSHPacketReader
	{
		SSHTransport transport;
		SSHClient client;

		
		internal TransportProtocolState currentState = TransportProtocolState.NEGOTIATING_PROTOCOL;

        internal RandomNumberGenerator rnd = new RNGCryptoServiceProvider();
		internal byte[] localkex = null;
		internal byte[] remotekex = null;
		internal byte[] sessionIdentifier = null;
		
		internal const int SSH_MSG_DISCONNECT = 1;
		internal const int SSH_MSG_IGNORE = 2;
		internal const int SSH_MSG_UNIMPLEMENTED = 3;
		internal const int SSH_MSG_DEBUG = 4;
		internal const int SSH_MSG_SERVICE_REQUEST = 5;
		internal const int SSH_MSG_SERVICE_ACCEPT = 6;
		
		internal const int SSH_MSG_KEX_INIT = 20;
		internal const int SSH_MSG_NEWKEYS = 21;

		internal ArrayList kexqueue;

		internal uint outgoingSequence = 0;
		internal uint incomingSequence = 0;
		
		internal const uint MAX_NUM_PACKETS_BEFORE_REKEY = 2147483647;
		internal const uint MAX_NUM_BYTES_BEFORE_REKEY = 1073741824;
		
		internal uint numIncomingBytesSinceKEX;
		internal uint numIncomingSSHPacketsSinceKEX;
		internal uint numOutgoingBytesSinceKEX;
		internal uint numOutgoingSSHPacketsSinceKEX;
		
		internal long outgoingBytes = 0;
		internal long incomingBytes = 0;

		internal Cipher encryption = null;
		internal Cipher decryption = null;
		internal SSH2Hmac outgoingMac = null;
		internal SSH2Hmac incomingMac = null;
		internal int incomingCipherLength = 8;
		internal int outgoingCipherLength = 8;
		internal int incomingMacLength = 0;
		internal int outgoingMacLength = 0;

		internal SSH2KeyExchange keyexchange = null;

		internal Object kexlock;

		internal String disconnectReason = null;

		internal TransportProtocol(SSHTransport transport, SSHClient client)
		{
			this.transport = transport;
			this.client = client;
			this.kexqueue = new ArrayList();
			this.kexlock = new Object();

		}

		internal event TransportProtocolStateChangeEvent StateChange;

		private void FireStateChange(TransportProtocolState state)
		{
			try
			{
				if(StateChange != null)
					StateChange(this, state);
			} 
			catch(Exception ex) 
			{
				System.Diagnostics.Trace.WriteLine("WARNING! Exception caught in Maverick.NET event handler: " + ex.Message );
				System.Diagnostics.Trace.WriteLine(ex.StackTrace );
			}

		}

		/// <summary>
		/// The session identifier.
		/// </summary>
		public byte[] SessionIdentifier
		{
			get
			{
				return sessionIdentifier;
			}
		}

		/// <summary>
		/// Evaluate whether the transport is still connected.
		/// </summary>
		public bool IsConnected
		{
			get
			{
				return currentState == TransportProtocolState.CONNECTED 
					|| currentState == TransportProtocolState.PERFORMING_KEYEXCHANGE;
			}
		}

		internal void StartProtocol()
		{
			SendKeyExchangeInit();

			while (ProcessMessage(ReadMessage()) 
				&& currentState != TransportProtocolState.CONNECTED)
			{
				;
			}

		}

		/// <summary>
		/// The configuration context for this connection.
		/// </summary>
		internal SSHContext Context
		{
			get
			{
				return client.Context;
			}
		}

		/// <summary>
		/// Get the next message from the transport.
		/// </summary>
		/// <remarks>
		/// If there is no message available this method will block. Additionally this
		/// method filters messages and only returns non transport protocol messages.
		/// </remarks>
		/// <returns></returns>
		public SSHPacket NextMessage()
		{
			
			lock (kexlock)
			{
				
				SSHPacket packet;
				
				do 
				{
					packet = ReadMessage();
				}
				while (ProcessMessage(packet));

				return packet;
			}
		}

		internal void StartService(String serviceName)
		{
			SSHPacket packet = GetSSHPacket(true);

			packet.WriteByte(SSH_MSG_SERVICE_REQUEST);
			packet.WriteString(serviceName);

#if DEBUG
			System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_SERVICE_REQUEST");
			System.Diagnostics.Trace.WriteLine(serviceName );
#endif
			SendMessage(packet);

			do 
			{
				packet = ReadMessage();
			}
			while(ProcessMessage(packet) || packet.MessageID != SSH_MSG_SERVICE_ACCEPT);


		}

		internal bool ProcessMessage(SSHPacket packet)
		{
			try
			{
				if (packet.Length < 1)
				{
					Disconnect("Invalid message received", DisconnectionReason.PROTOCOL_ERROR);
					throw new SSHException("Invalid transport protocol message", SSHException.INTERNAL_ERROR);
				}
				
				switch (packet.MessageID)
				{
					
					case SSH_MSG_DISCONNECT:  
					{
						packet.ReadInt();
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Received SSH_MSG_DISCONNECT: " + packet.ReadString() );
#endif
						InternalDisconnect();
						throw new SSHException(packet.ReadString(), SSHException.REMOTE_HOST_DISCONNECTED);
					}
					
					case SSH_MSG_IGNORE: 
					{
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Received SSH_MSG_IGNORE");
#endif
						return true;

					}
					case SSH_MSG_DEBUG: 
					{
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Received SSH_MSG_DEBUG");
						packet.Skip(1);
						System.Diagnostics.Trace.WriteLine(packet.ReadString() );
#endif
						return true;
					}
					case SSH_MSG_NEWKEYS:  
					{
						// This lightweight implemention ignores these messages
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Received SSH_MSG_NEWKEYS");
#endif
						return true;
					}
					
					case SSH_MSG_KEX_INIT:  
					{

#if DEBUG
						System.Diagnostics.Trace.WriteLine("Received SSH_MSG_KEX_INIT");
#endif
						if (remotekex != null)
						{
							Disconnect("Key exchange already in progress!",
								DisconnectionReason.PROTOCOL_ERROR);
							throw new SSHException("Key exchange already in progress!", SSHException.PROTOCOL_VIOLATION);
						}
							
						PerformKeyExchange(packet);
							
						return true;
					}
					
					default:  
					{
						// Not a transport protocol message
						return false;
					}
					
				}
			}
			catch (System.IO.IOException ex1)
			{
				throw new SSHException(ex1.Message, SSHException.INTERNAL_ERROR);
			}

		}

		internal void PerformKeyExchange(SSHPacket packet)
		{
			lock(kexlock)
			{
				if (localkex == null)
				{
					SendKeyExchangeInit();
				}

				SSH2Context context = (SSH2Context) client.Context;

				currentState = TransportProtocolState.PERFORMING_KEYEXCHANGE;

				remotekex = packet.Payload;

				// Ignore the cookie
				packet.Skip(16);

				String remoteKeyExchanges = packet.ReadString();
				String remotePublicKeys = packet.ReadString();
				String remoteCiphersCS = packet.ReadString();
				String remoteCiphersSC = packet.ReadString();
				String remoteHMacCS = packet.ReadString();
				String remoteHMacSC = packet.ReadString();


				String cipherCS = SelectNegotiatedComponent(context.SupportedCiphers.List(
					context.PreferredCipherCS), remoteCiphersCS);

				String cipherSC = SelectNegotiatedComponent(context.SupportedCiphers.List(
					context.PreferredCipherSC), remoteCiphersSC);

				Cipher encryption = (Cipher) context.SupportedCiphers.GetInstance(cipherCS);
					
				Cipher decryption = (Cipher) context.SupportedCiphers.GetInstance(cipherSC);

				String macCS = SelectNegotiatedComponent(context.SupportedMACs.List(
					context.PreferredMacCS), remoteHMacCS);

				String macSC = SelectNegotiatedComponent(context.SupportedMACs.List(
					context.PreferredMacSC), remoteHMacSC);

				SSH2Hmac outgoingMac = (SSH2Hmac) context.SupportedMACs.GetInstance(macCS);
					
				SSH2Hmac incomingMac = (SSH2Hmac) context.SupportedMACs.GetInstance(macSC);

				// Ignore compression and languages as were not interested in that atm
				
				// Create a Key Exchange instance
				String kex = SelectNegotiatedComponent(context.SupportedKeyExchanges.List(
					context.PreferredKeyExchange), remoteKeyExchanges);
				
				String publickey = SelectNegotiatedComponent(context.SupportedPublicKeys.List(
					context.PreferredPublicKey), remotePublicKeys);

#if DEBUG     
				// Output the local settings
				System.Diagnostics.Trace.WriteLine("Local Key exchange settings follow:");
				System.Diagnostics.Trace.WriteLine("Key exchange: " + context.SupportedKeyExchanges.List(
					context.PreferredKeyExchange) );
				System.Diagnostics.Trace.WriteLine("Public keys : " + context.SupportedPublicKeys.List(
					context.PreferredPublicKey) );
				System.Diagnostics.Trace.WriteLine("Ciphers client->server: " + context.SupportedCiphers.List(
					context.PreferredCipherCS) );
				System.Diagnostics.Trace.WriteLine("Ciphers server->client: " + context.SupportedCiphers.List(
					context.PreferredCipherSC) );
				System.Diagnostics.Trace.WriteLine("HMAC client->server: " + context.SupportedMACs.List(
					context.PreferredMacCS) );
				System.Diagnostics.Trace.WriteLine("HMAC server->client: " + context.SupportedMACs.List(
					context.PreferredMacSC) );

				// Output the remote settings
				System.Diagnostics.Trace.WriteLine("Remote Key exchange settings follow:");
				System.Diagnostics.Trace.WriteLine("Key exchange: " + remoteKeyExchanges );
				System.Diagnostics.Trace.WriteLine("Public keys : " + remotePublicKeys );
				System.Diagnostics.Trace.WriteLine("Ciphers client->server: " + remoteCiphersCS );
				System.Diagnostics.Trace.WriteLine("Ciphers server->client: " + remoteCiphersSC );
				System.Diagnostics.Trace.WriteLine("HMAC client->server: " + remoteHMacCS );
				System.Diagnostics.Trace.WriteLine("HMAC server->client: " + remoteHMacSC );

				// Output the selected settigns
				System.Diagnostics.Trace.WriteLine("Selected kex exchange: " + kex );
				System.Diagnostics.Trace.WriteLine("Selected public key: " + publickey );
				System.Diagnostics.Trace.WriteLine("Selected cipher client->server: " + cipherCS );
				System.Diagnostics.Trace.WriteLine("Selected cipher server->client: " + cipherSC );
				System.Diagnostics.Trace.WriteLine("Selected HMAC client->server: " + macCS );
				System.Diagnostics.Trace.WriteLine("Selected HMAC server->client: " + macSC );

#endif
				keyexchange = (SSH2KeyExchange) context.SupportedKeyExchanges.GetInstance(kex);

				keyexchange.Init(this);

				// Perform the key exchange
				keyexchange.PerformClientExchange(client.LocalIdentification,
					client.RemoteIdentification,
					localkex,
					remotekex);

				SSHPublicKey hostkey = (SSHPublicKey) context.SupportedPublicKeys.GetInstance(publickey);

				hostkey.Init(keyexchange.HostKey, 0, keyexchange.HostKey.Length);

				if(context.KnownHosts!=null) 
				{
					if(!context.KnownHosts.VerifyHost(transport.Hostname, hostkey))
					{
						Disconnect("Host key not accepted", DisconnectionReason.HOST_KEY_NOT_VERIFIABLE);
						throw new SSHException("The host key was not accepted",
							SSHException.CANCELLED_CONNECTION);

					}
				}

				if(!hostkey.VerifySignature(keyexchange.Signature,
					keyexchange.ExchangeHash)) 
				{
					Disconnect("The host key signature is invalid",
						DisconnectionReason.HOST_KEY_NOT_VERIFIABLE);
					throw new SSHException("The host key signature is invalid",
						SSHException.PROTOCOL_VIOLATION);
				}

				if(sessionIdentifier==null)
					sessionIdentifier = keyexchange.ExchangeHash;

				packet = GetSSHPacket(true);
				packet.WriteByte(SSH_MSG_NEWKEYS);
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_NEWKEYS");
#endif
				SendMessage(packet);
				

				encryption.Init(Cipher.ENCRYPT_MODE,
					MakeSSHKey('A'),
					MakeSSHKey('C'));

				outgoingCipherLength = encryption.BlockSize;

				outgoingMac.Init(MakeSSHKey('E'));
				outgoingMacLength = outgoingMac.MacLength;

				this.encryption = encryption;
				this.outgoingMac = outgoingMac;

				do 
				{
					packet = ReadMessage();

					// Process the transport protocol message, must only be
					// SSH_MSH_INGORE, SSH_MSG_DEBUG, SSH_MSG_DISCONNECT or SSH_MSG_NEWKEYS
					if(!ProcessMessage(packet)) 
					{
						Disconnect("Invalid message received during key exchange",
							DisconnectionReason.PROTOCOL_ERROR);
						throw new SSHException(
							"Invalid message received during key exchange",
							SSHException.PROTOCOL_VIOLATION);
					}

				}
				while(packet.MessageID != SSH_MSG_NEWKEYS);

				// Put the incoming components into use
				decryption.Init(Cipher.DECRYPT_MODE,
					MakeSSHKey('B'),
					MakeSSHKey('D'));
				incomingCipherLength = decryption.BlockSize;

				incomingMac.Init(MakeSSHKey('F'));
				incomingMacLength = incomingMac.MacLength;

				this.decryption = decryption;
				this.incomingMac = incomingMac;
				//this.incomingCompression = incomingCompression;

				currentState = TransportProtocolState.CONNECTED;

				FireStateChange(TransportProtocolState.CONNECTED);

				lock(kexqueue) 
				{

#if DEBUG
					System.Diagnostics.Trace.WriteLine("Sending queued messages");
#endif
					for (System.Collections.IEnumerator e = kexqueue.GetEnumerator(); e.MoveNext(); )
					{
						SendMessage((SSHPacket) e.Current);
					}

					kexqueue.Clear();
				}

				// Clean up and reset any parameters
				localkex = null;
				remotekex = null;

			}

		}

		internal System.String SelectNegotiatedComponent(System.String locallist, System.String remotelist)
		{
			
			System.Collections.ArrayList r = new System.Collections.ArrayList();
			int idx;
			System.String name;
			while ((idx = remotelist.IndexOf(",")) > - 1)
			{
				r.Add(remotelist.Substring(0, (idx) - (0)));
				remotelist = remotelist.Substring(idx + 1);
			}
			
			r.Add(remotelist);
			
			while ((idx = locallist.IndexOf(",")) > - 1)
			{
				name = locallist.Substring(0, (idx) - (0));
				if (r.Contains(name))
				{
					return name;
				}
				locallist = locallist.Substring(idx + 1);
			}
			
			if (r.Contains(locallist))
			{
				return locallist;
			}
			
			throw new SSHException("Failed to negotiate a transport component [" + locallist + "] [" + remotelist + "]", 
				SSHException.KEY_EXCHANGE_FAILED);
		}


		internal bool IsTransportMessage(int messageid)
		{
			switch (messageid)
			{
				
				case SSH_MSG_DISCONNECT: 
				case SSH_MSG_IGNORE: 
				case SSH_MSG_DEBUG: 
				case SSH_MSG_NEWKEYS: 
				case SSH_MSG_KEX_INIT:  
				{
					return true;
				}
				
				default:  
				{
					if (keyexchange != null)
					{
						return keyexchange.IsKeyExchangeMessage(messageid);
					}

					// Not a transport protocol message
					return false;
				}
				
			}
		}

		internal SSHPacket GetSSHPacket(bool forSending) 
		{
			// TODO: Pool some packets
			SSHPacket packet = new SSH2Packet(35000);
			// Skip the length and padding length fields for now
			if(forSending)
				packet.Skip(5);

			return packet;
		}

		internal void ReleaseSSHPacket(SSHPacket packet) 
		{
			// TODO: Add the packet back into the pool
		}

		internal void  SendKeyExchangeInit()
		{
			
			try
			{
				
				FireStateChange(TransportProtocolState.PERFORMING_KEYEXCHANGE);

				numIncomingBytesSinceKEX = 0;
				numIncomingSSHPacketsSinceKEX = 0;
				numOutgoingBytesSinceKEX = 0;
				numOutgoingSSHPacketsSinceKEX = 0;
				
				currentState = TransportProtocolState.PERFORMING_KEYEXCHANGE;
				
				SSHPacket packet = GetSSHPacket(true);
				
				SSH2Context transportContext = (SSH2Context) client.Context;
				byte[] cookie = new byte[16];
				rnd.GetBytes(cookie);
				packet.WriteByte((byte)SSH_MSG_KEX_INIT);
				packet.WriteBytes(cookie);
				packet.WriteString("diffie-hellman-group1-sha1");
				packet.WriteString(transportContext.SupportedPublicKeys.List(transportContext.PreferredPublicKey));
				packet.WriteString(transportContext.SupportedCiphers.List(transportContext.PreferredCipherCS));
				packet.WriteString(transportContext.SupportedCiphers.List(transportContext.PreferredCipherSC));
				packet.WriteString(transportContext.SupportedMACs.List(transportContext.PreferredMacCS));
				packet.WriteString(transportContext.SupportedMACs.List(transportContext.PreferredMacSC));
				packet.WriteString("none");
				packet.WriteString("none");
				packet.WriteString("");
				packet.WriteString("");
				packet.WriteByte((byte) 0);
				packet.WriteUINT32(0);
				
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_KEX_INIT" );
#endif 

				localkex = SendMessage(packet, true);
		
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

		internal SSHPacket ReadMessage()
		{
			
			lock(kexlock)
			{
				
				try
				{

					SSHPacket packet = GetSSHPacket(false);
					packet.ReadFromStream(transport.GetStream(), incomingCipherLength);

					// Mark the current position so we can read more data
					packet.Mark();
					packet.MoveToPosition(0);

					//	  Decrypt the data if we have a valid cipher
					if (decryption != null)
					{
						decryption.Transform(packet.Array, 0, packet.Array, 0, incomingCipherLength);
					}

					int msglen = (int)packet.ReadUINT32();
					int padlen = packet.ReadByte();

					int remaining = (msglen - (incomingCipherLength - 4));

					//      Verify that the packet length is good
					if (remaining < 0)
					{
						InternalDisconnect();
						throw new SSHException("EOF whilst reading message data block", SSHException.UNEXPECTED_TERMINATION);
					}
					else if (remaining > packet.Limit - packet.Length)
					{
						InternalDisconnect();
						throw new SSHException("Incoming packet length violates SSH protocol", 
							SSHException.UNEXPECTED_TERMINATION);
					}
					
					//	Read, decrypt and save the remaining data
					packet.MoveToMark();

					packet.ReadFromStream(transport.GetStream(), remaining);

					if (decryption != null)
					{
						decryption.Transform(packet.Array, incomingCipherLength, packet.Array, incomingCipherLength, remaining);
					}

					// Tell the packet where the payload ends
					//packet.PayloadLength = (int)msglen - padlen - 1;

					if (incomingMac != null)
					{
						packet.ReadFromStream(transport.GetStream(), incomingMacLength);
						
						// Verify the mac
						if (!incomingMac.Verify(incomingSequence, packet.Array, 0, incomingCipherLength + remaining, 
							packet.Array, incomingCipherLength + remaining))
						{
							Disconnect("Corrupt Mac on input", DisconnectionReason.MAC_ERROR);
							throw new SSHException("Corrupt Mac on input", SSHException.PROTOCOL_VIOLATION);
						}
					}
					
					if (++incomingSequence > 4294967295)
					{
						incomingSequence = 0;
					}
					
					
					incomingBytes += incomingCipherLength + remaining + incomingMacLength;
					

					// Uncompress the message payload if necersary
					/*if (incomingCompression != null)
					{
						return incomingCompression.uncompress(payload, 0, payload.Length);
					}*/
					
					numIncomingBytesSinceKEX += (uint)packet.Length;
					numIncomingSSHPacketsSinceKEX++;
					
					if (numIncomingBytesSinceKEX >= MAX_NUM_BYTES_BEFORE_REKEY 
						|| numIncomingSSHPacketsSinceKEX >= MAX_NUM_PACKETS_BEFORE_REKEY)
					{
						SendKeyExchangeInit();
					}
					
					// Get the packet ready for reading
					packet.MoveToPosition(6);

					return packet;
				}
				catch(System.ObjectDisposedException ex)
				{
					InternalDisconnect();
					
					throw new SSHException("Unexpected terminaton: " + ex.Message,
						SSHException.UNEXPECTED_TERMINATION);
				}
				catch(System.IO.IOException ex)
				{
					InternalDisconnect();
					
					throw new SSHException("Unexpected terminaton: " 
						+ (ex.Message != null? ex.Message:ex.GetType().FullName) 
						+ " sequenceNo = " + incomingSequence + " bytesIn = " 
						+ incomingBytes + " bytesOut = " + outgoingBytes, 
						SSHException.UNEXPECTED_TERMINATION);
				}
			}
			
		}

		/// <summary>
		/// Send a message to the remote side of the connection.
		/// </summary>
		/// <param name="packet"></param>
		public byte[]  SendMessage(SSHPacket packet)
		{
			return SendMessage(packet, false);
		}

		/// <summary>
		/// Send a message to the remote side of the connection.
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="returnPayload">Return the unecrypted payload of this packet.</param>
		public byte[]  SendMessage(SSHPacket packet, bool returnPayload)
		{

			byte[] payload = null;

			lock(this)
			{

					if (currentState == TransportProtocolState.PERFORMING_KEYEXCHANGE 
						&& !IsTransportMessage(packet.MessageID))
					{
						lock (kexqueue)
						{
							kexqueue.Add(packet);
							return payload;
						}
					}
				
					
					try
					{
						
						int padding = 4;
						
						// Compress the payload if necersary
						/*if (outgoingCompression != null)
						{
							msgdata = outgoingCompression.compress(msgdata, 0, msgdata.Length);
						}*/

                        // Determine the padding length
						padding += ((outgoingCipherLength - ((packet.Length + padding) % outgoingCipherLength)) % outgoingCipherLength);
						
						packet.MoveToPosition(0);
						// Write the packet length field
						packet.WriteUINT32(packet.Length - 4 + padding);
						// Write the padding length
						packet.WriteByte((byte)padding);					
						
						// Now skip back up to the end of the packet
						packet.MoveToEnd();

						if(returnPayload)
							payload = packet.Payload;

						// Create some random data for the padding
						byte[] pad = new byte[padding];
						rnd.GetBytes(pad);
						packet.WriteBytes(pad);
						
						
						// Generate the MAC
						if (outgoingMac != null)
						{
							outgoingMac.Generate(outgoingSequence, packet.Array, 0, packet.Length, packet.Array, packet.Length);
						}
						
						// Perfrom encrpytion
						if (encryption != null)
						{
							encryption.Transform(packet.Array, 0, packet.Array, 0, packet.Length);
						}
						
						packet.Skip(outgoingMacLength);
						outgoingBytes += packet.Length;
						
						// Send!
						packet.WriteToStream(transport.GetStream());
						
						outgoingSequence++;
						numOutgoingBytesSinceKEX += (uint)packet.Length;
						numOutgoingSSHPacketsSinceKEX++;
						
						ReleaseSSHPacket(packet);

						if (outgoingSequence > 4294967295)
						{
							outgoingSequence = 0;
						}
						
						if (numOutgoingBytesSinceKEX >= MAX_NUM_BYTES_BEFORE_REKEY 
							|| numOutgoingSSHPacketsSinceKEX >= MAX_NUM_PACKETS_BEFORE_REKEY)
						{
							SendKeyExchangeInit();
						}
					}
					catch (System.IO.IOException ex)
					{
						InternalDisconnect();
						
						throw new SSHException("Unexpected termination: " + ex.Message, SSHException.UNEXPECTED_TERMINATION);
					}
					catch(System.ObjectDisposedException ex)
					{
						InternalDisconnect();
					
						throw new SSHException("Unexpected terminaton: " + ex.Message, 
							SSHException.UNEXPECTED_TERMINATION);
					}
				}

				return payload;
			}

		internal void InternalDisconnect()
		{
			
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Performing disconnection");
#endif
			try 
			{
				transport.Close();
			} 
			catch(IOException ex) 
			{ 
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Exception during transport.Close()");
			System.Diagnostics.Trace.WriteLine(ex.StackTrace );
#endif
			}
			finally 
			{
				if(currentState!=TransportProtocolState.DISCONNECTED)
					FireStateChange(TransportProtocolState.DISCONNECTED);
				currentState = TransportProtocolState.DISCONNECTED;
			}


		}

		/// <summary>
		/// Disconnect the SSH transport protocol.
		/// </summary>
		/// <param name="disconnectReason">A descriptive reason for the disconnection.</param>
		/// <param name="reason">The SSH reason code.</param>
		public void Disconnect(String disconnectReason, DisconnectionReason reason)
		{
			try 
			{
				this.disconnectReason = disconnectReason;
				SSHPacket packet = GetSSHPacket(true);
				packet.WriteByte(SSH_MSG_DISCONNECT);
				packet.WriteUINT32((int)reason);
				packet.WriteString(disconnectReason);
				packet.WriteString("");

#if DEBUG
				System.Diagnostics.Trace.WriteLine("Sending SSH_MSG_DISCONNECT");
				System.Diagnostics.Trace.WriteLine(disconnectReason );
#endif
				SendMessage(packet);

			}
			catch
			{
			}
			finally 
			{
				InternalDisconnect();
			}
		}


		internal byte[] MakeSSHKey(char chr)
		{

			ByteBuffer keydata = new ByteBuffer(256);	
			// Create the first 20 bytes of key data

			byte[] data = new byte[20];

            Hash hash = new Hash(new SHA1CryptoServiceProvider());

			// Put the dh k value
			hash.WriteBigInteger(keyexchange.Secret);

			// Put in the exchange hash
			hash.WriteBytes(keyexchange.ExchangeHash);

			// Put in the character
			hash.WriteByte((byte) chr);

			// Put the exchange hash in again
			hash.WriteBytes(sessionIdentifier);

			// Create the fist 20 bytes
			data = hash.DoFinal();

			keydata.WriteBytes(data);

			// Now do the next 20
			hash.Reset();

			// Put the dh k value in again
			hash.WriteBigInteger(keyexchange.Secret);

			// And the exchange hash
			hash.WriteBytes(keyexchange.ExchangeHash);

			// Finally the first 20 bytes of data we created
			hash.WriteBytes(data);

			data = hash.DoFinal();

			// Put it all together
			keydata.WriteBytes(data);

			// Return it
			return keydata.ToByteArray();

	}
		
	}
}
