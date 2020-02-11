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
using Maverick.SSH2.Algorithms;
using Maverick.Crypto.Util;

namespace Maverick.SSH2
{
	
	

	/// <summary>
	/// Implementation of an SSH2 <see cref="Maverick.SSH.SSHContext"/>.
	/// </summary>
	/// <remarks>
	/// To configure SSH2 specific options obtain a copy of the <see cref="Maverick.SSH.SSHConnector"/>'s
	/// context and cast to an instance of this class.
	/// <example>
	/// <code>
	/// SSHConnector con = SSHConnector.Create();
	/// SSH2Context ssh2Context = (SSH2Context)con.GetContext(SSHConnector.SSH2);
	/// 
	/// // Set a different preferred cipher
	/// ssh2Context.PreferredCipherCS = SSH2Context.CIPHER_AES128_CBC;
	/// ssh2Context.PreferredCipherSC = SSH2Context.CIPHER_AES128_CBC;
	/// </code>
	/// </example>
	/// </remarks>
	public class SSH2Context : SSHContext
	{
		CipherComponentFactory supportedCiphers;
		SSHHmacComponentFactory supportedMacs;
		SSHKeyExchangeFactory supportedKeyExchanges;
		SSHPublicKeyFactory supportedPublicKeys;
		byte[] x11FakeCookie;
		byte[] x11RealCookie;
		String x11Display;
		ForwardingRequestListener x11RequestListener;
		HostKeyVerification knownhosts;
		String sftpProvider;
		BannerDisplay banner;

		int maximumNumberChannels = 100;

		/// <summary>
		/// Constant for identifying the 3DES cipher
		/// </summary>
		public const String CIPHER_3DES_CBC = "3des-cbc";

		/// <summary>
		/// Constant for identifying the Blowfish cipher
		/// </summary>
		public const String CIPHER_BLOWFISH_CBC = "blowfish-cbc";

		/// <summary>
		/// Constant for identifying the AES cipher with a 128 bit key
		/// </summary>
		public const String CIPHER_AES128_CBC = "aes128-cbc";

		/// <summary>
		/// Constant for identifying the AES cipher with a 192 bit key
		/// </summary>
		public const String CIPHER_AES192_CBC = "aes192-cbc";

		/// <summary>
		/// Constant for identifying the AES cipher with a 256 bit key
		/// </summary>
		public const String CIPHER_AES256_CBC = "aes256-cbc";

		/// <summary>
		/// Constant for identifying the Serpent cipher with a 128 bit key
		/// </summary>
		public const String CIPHER_SERPENT128_CBC = "serpent128-cbc";

		/// <summary>
		/// Constant for identifying the Serpent cipher with a 192 bit key
		/// </summary>
		public const String CIPHER_SERPENT192_CBC = "serpent192-cbc";

		/// <summary>
		/// Constant for identifying the Serpent cipher with a 256 bit key
		/// </summary>
		public const String CIPHER_SERPENT256_CBC = "serpent256-cbc";

		/// <summary>
		/// Constant for identifying the Twofish cipher with a 128 bit key
		/// </summary>
		public const String CIPHER_TWOFISH128_CBC = "twofish128-cbc";

		/// <summary>
		/// Constant for identifying the Twofish cipher with a 192 bit key
		/// </summary>
		public const String CIPHER_TWOFISH192_CBC = "twofish192-cbc";

		/// <summary>
		/// Constant for identifying the Twofish cipher with a 256 bit key
		/// </summary>
		public const String CIPHER_TWOFISH256_CBC = "twofish256-cbc";

		/// <summary>
		/// Constant for identifying the Cast cipher with a 128 bit key
		/// </summary>
		public const String CIPHER_CAST128_CBC = "cast128-cbc";
		
		/// <summary>
		/// Constant for identifying the SHA-1 MAC
		/// </summary>
		public const String HMAC_SHA1 = "hmac-sha1";

		/// <summary>
		/// Constant for identifying the MD5 MAC
		/// </summary>
		public const String HMAC_MD5 = "hmac-md5";

		/// <summary>
		/// Constant for identifying the Diffie Hellman Group1 SHA1 key exchange
		/// </summary>
		public const String KEX_DH_GROUP1_SHA1 = DiffieHellmanGroup1SHA1.DIFFIE_HELLMAN_GROUP1_SHA1;

		/// <summary>
		/// Constant for identifying the SSH DSA public key scheme
		/// </summary>
		public const String PK_SSH_DSS = "ssh-dss";

		/// <summary>
		/// Constant for identifying the SSH RSA public key scheme
		/// </summary>
		public const String PK_SSH_RSA = "ssh-rsa";
		
		internal String prefCipherCS = CIPHER_AES128_CBC;
		internal String prefCipherSC = CIPHER_AES128_CBC;

		internal String prefHmacCS = HMAC_SHA1;
		internal String prefHmacSC = HMAC_SHA1;

		internal String prefPK = PK_SSH_DSS;

		internal String prefKex = KEX_DH_GROUP1_SHA1;

		/// <summary>
		/// The maximum number of channels allowed open at any one time by the client
		/// </summary>
		public int MaximumNumberChannels
		{
			get
			{
				return maximumNumberChannels;
			}

			set
			{
				maximumNumberChannels = value;
			}
		}

		/// <summary>
		/// The supported ciphers for an SSH2 connection.
		/// </summary>
		public AbstractComponentFactory SupportedCiphers 
		{
			get
			{
				return supportedCiphers;
			}
		}

		/// <summary>
		/// The supported MACs for an SSH2 connection.
		/// </summary>
		public AbstractComponentFactory SupportedMACs 
		{
			get
			{
				return supportedMacs;
			}
		}

		/// <summary>
		/// The supported key exchanges for an SSH2 connection.
		/// </summary>
		public AbstractComponentFactory SupportedKeyExchanges
		{
			get 
			{
				return supportedKeyExchanges;
			}
	    }

		/// <summary>
		/// The supported public key types for an SSH2 connection.
		/// </summary>
		public AbstractComponentFactory SupportedPublicKeys
		{
			get
			{
				return supportedPublicKeys;
			}
		}


		/// <summary>
		/// The preferred cipher for the client->server communication.
		/// </summary>
		/// <remarks>
		/// This defaults to <see cref="Maverick.SSH2.SSH2Context.CIPHER_BLOWFISH_CBC"/>
		/// </remarks>
		public String PreferredCipherCS
		{
			get
			{
				return prefCipherCS;
			}

			set
			{
				this.prefCipherCS = value;
			}
		}

		/// <summary>
		/// The preferred cipher for the server->client communication.
		/// </summary>
		/// <remarks>
		/// This defaults to <see cref="Maverick.SSH2.SSH2Context.CIPHER_BLOWFISH_CBC"/>
		/// </remarks>
		public String PreferredCipherSC
		{
			get
			{
				return prefCipherSC;
			}

			set
			{
				this.prefCipherSC = value;
			}
		}

		/// <summary>
		/// The preferred MAC for the client->server communication.
		/// </summary>
		/// <remarks>
		/// This defaults to <see cref="Maverick.SSH2.SSH2Context.HMAC_SHA1"/>
		/// </remarks>
		public String PreferredMacCS
		{
			get
			{
				return prefHmacCS;
			}

			set
			{
				this.prefHmacCS = value;
			}
		}

		/// <summary>
		/// The preferred MAC for the server->client communication.
		/// </summary>
		/// <remarks>
		/// This defaults to <see cref="Maverick.SSH2.SSH2Context.HMAC_SHA1"/>
		/// </remarks>
		public String PreferredMacSC
		{
			get
			{
				return prefHmacSC;
			}

			set
			{
				this.prefHmacSC = value;
			}
		}

		/// <summary>
		/// The preferred key exchange for the connection.
		/// </summary>
		/// <remarks>
		/// This defaults to <see cref="Maverick.SSH2.SSH2Context.KEX_DH_GROUP1_SHA1"/>
		/// </remarks>
		public String PreferredKeyExchange
		{	
			get
			{
				return prefKex;
			}

			set
			{
				this.prefKex = value;
			}
		}

		/// <summary>
		/// The custom SFTP provider
		/// </summary>
		public String SFTPProvider
		{
			get
			{
				return sftpProvider;
			}

			set
			{
				sftpProvider = value;
			}
		}

		/// <summary>
		/// The forwarding request listener for X11 forwarding operations.
		/// </summary>
		public ForwardingRequestListener X11RequestListener
		{
			get
			{
				return x11RequestListener;
			}

			set
			{
				x11RequestListener = value;
			}
		}

		/// <summary>
		/// The host verification instance for this configuration
		/// </summary>
		public HostKeyVerification KnownHosts
		{
			get
			{
				return knownhosts;
			}

			set
			{
				knownhosts = value;
			}
		}

		/// <summary>
		/// The preferred public key type for the key exchange operation.
		/// </summary>
		/// <remarks>
		/// This defaults to <see cref="Maverick.SSH2.SSH2Context.PK_SSH_DSS"/>
		/// </remarks>
		public String PreferredPublicKey
		{
			get
			{
				return prefPK;
			}

			set
			{
				this.prefPK = value;
			}
		}

		/// <summary>
		/// The X11 forwarding authentication cookie
		/// </summary>
		public byte[] X11AuthenticationCookie
		{
			get
			{
				if (x11FakeCookie == null)
				{
					x11FakeCookie = new byte[16];
					SecureRandom.GetInstance().NextBytes(x11FakeCookie);
				}
				return x11FakeCookie;
			}
		}

		/// <summary>
		/// A callback interface for receiving the authentication banner.
		/// </summary>
		public BannerDisplay Banner
		{
			get
			{
				return banner;
			}

			set 
			{
				banner = value;
			}
		}


		/// <summary>
		/// The number of the X11 display [Reserved for future use]
		/// </summary>
		public String X11Display
		{
			get
			{
				return x11Display;
			}

			set
			{
				x11Display = value;
			}
		}

		/// <summary>
		/// The real X11 authentication cookie [Reserved for future use]
		/// </summary>
		public byte[] X11RealCookie
		{
			get
			{
				if (x11RealCookie == null)
				{
					x11RealCookie = X11AuthenticationCookie;
				}
				return x11RealCookie;
			}
			
			set
			{
				this.x11RealCookie = value;
			}
		}


		internal SSH2Context()
		{
			this.supportedCiphers = new CipherComponentFactory();
			this.supportedCiphers.Add(CIPHER_3DES_CBC, System.Type.GetType("Maverick.SSH2.Algorithms.TripleDesCipher"));

			this.supportedCiphers.Add(CIPHER_BLOWFISH_CBC, System.Type.GetType("Maverick.SSH2.Algorithms.BlowfishCipher"));

			this.supportedCiphers.Add(CIPHER_AES128_CBC, System.Type.GetType("Maverick.SSH2.Algorithms.AESCipher"));
			this.supportedCiphers.Add(CIPHER_AES192_CBC, System.Type.GetType("Maverick.SSH2.Algorithms.AES192Cipher"));
			this.supportedCiphers.Add(CIPHER_AES256_CBC, System.Type.GetType("Maverick.SSH2.Algorithms.AES256Cipher"));

			this.supportedCiphers.Add(CIPHER_SERPENT128_CBC, System.Type.GetType("Maverick.SSH2.Algorithms.SerpentCipher"));
			this.supportedCiphers.Add(CIPHER_SERPENT192_CBC, System.Type.GetType("Maverick.SSH2.Algorithms.Serpent192Cipher"));
			this.supportedCiphers.Add(CIPHER_SERPENT256_CBC, System.Type.GetType("Maverick.SSH2.Algorithms.Serpent256Cipher"));
			
			this.supportedCiphers.Add(CIPHER_TWOFISH128_CBC, System.Type.GetType("Maverick.SSH2.Algorithms.TwofishCipher"));
			this.supportedCiphers.Add(CIPHER_TWOFISH192_CBC, System.Type.GetType("Maverick.SSH2.Algorithms.Twofish192Cipher"));
			this.supportedCiphers.Add(CIPHER_TWOFISH256_CBC, System.Type.GetType("Maverick.SSH2.Algorithms.Twofish256Cipher"));
			
			this.supportedCiphers.Add(CIPHER_CAST128_CBC, System.Type.GetType("Maverick.SSH2.Algorithms.CASTCipher"));

			this.supportedMacs = new SSHHmacComponentFactory();
			this.supportedMacs.Add(HMAC_SHA1, System.Type.GetType("Maverick.SSH2.Algorithms.HmacSHA1"));
			this.supportedMacs.Add(HMAC_MD5, System.Type.GetType("Maverick.SSH2.Algorithms.HmacMD5"));
            
			this.supportedKeyExchanges = new SSHKeyExchangeFactory();
			this.supportedKeyExchanges.Add(KEX_DH_GROUP1_SHA1,
				System.Type.GetType("Maverick.SSH2.Algorithms.DiffieHellmanGroup1SHA1"));

			this.supportedPublicKeys = new SSHPublicKeyFactory();
			this.supportedPublicKeys.Add(PK_SSH_DSS,
				System.Type.GetType("Maverick.SSH2.Algorithms.SSH2DSAPublicKey"));
			this.supportedPublicKeys.Add(PK_SSH_RSA,
				System.Type.GetType("Maverick.SSH2.Algorithms.SSH2RSAPublicKey"));
		}
	
		class CipherComponentFactory : AbstractComponentFactory 
		{
			protected internal CipherComponentFactory() : base(System.Type.GetType("Maverick.Crypto.Ciphers.Cipher"))
			{
			
			}

			protected internal override System.Object CreateInstance(System.String name, System.Type type)
			{
				return SupportClass.CreateNewInstance(type);
			}

		}
	
		class SSHHmacComponentFactory : AbstractComponentFactory 
		{
			protected internal SSHHmacComponentFactory() : base(System.Type.GetType("Maverick.SSH2.Algorithms.SSH2Hmac"))
			{
			
			}

			protected internal override System.Object CreateInstance(System.String name, System.Type type)
			{
				return SupportClass.CreateNewInstance(type);
			}

		}	
	
		class SSHKeyExchangeFactory : AbstractComponentFactory
		{
			protected internal SSHKeyExchangeFactory() : base(System.Type.GetType("Maverick.SSH2.Algorithms.SSH2KeyExchange"))
			{

			}

			protected internal override System.Object CreateInstance(System.String name, System.Type type)
			{
				return SupportClass.CreateNewInstance(type);
			}
		}

		class SSHPublicKeyFactory : AbstractComponentFactory
		{
			protected internal SSHPublicKeyFactory() : base(System.Type.GetType("Maverick.SSH2.Algorithms.SSHPublicKey"))
			{

			}

			protected internal override System.Object CreateInstance(System.String name, System.Type type)
			{
				return SupportClass.CreateNewInstance(type);
			}		
		}
	}


	
}
