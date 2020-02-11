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
using Maverick.Crypto.DSA;
using Maverick.SSH;
using Maverick.Crypto.Math;
using Maverick.Crypto.IO;
using Maverick.Crypto.Util;

namespace Maverick.SSH2.Algorithms
{
	/// <summary>
	/// This class provides an SSH2 DSA public key implementation
	/// </summary>
	public class SSH2DSAPublicKey : DSAPublicKey, SSHPublicKey
	{

		/// <summary>
		/// Construct an uninitialized DSA key.
		/// </summary>
		public SSH2DSAPublicKey()
		{
		}

		/// <summary>
		/// Construct an SSH2 DSA key from the DSA parameters.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="q"></param>
		/// <param name="g"></param>
		/// <param name="y"></param>
		public SSH2DSAPublicKey(BigInteger p,
			BigInteger q,
			BigInteger g,
			BigInteger y) : base(p, q, g, y)
		{

		}

		/// <summary>
		/// Encode the public key into a blob of binary data, the encoded result will be 
		/// passed into init to recreate the key. 
		/// </summary>
		/// <returns></returns>
		public byte[] GetEncoded()
		{
			ByteBuffer buffer = new ByteBuffer(4096);

			buffer.WriteString("ssh-dss");
			buffer.WriteBigInteger(p);
			buffer.WriteBigInteger(q);
			buffer.WriteBigInteger(g);
			buffer.WriteBigInteger(y);

			return buffer.ToByteArray();
		}

		/// <summary>
		/// Initialize the public key from a blob of binary data. 
		/// </summary>
		/// <param name="blob">The encoded public key</param>
		/// <param name="start">The location in the array where the encoded data starts</param>
		/// <param name="len">The length of the encoded data</param>
		public void Init(byte[] blob, int start, int len)
		{
			ByteBuffer buffer = new ByteBuffer(blob);
			buffer.Skip(start);

			String header = buffer.ReadString();

			if(!header.Equals("ssh-dss")) 
			{
				throw new SSHException("Invalid ssh-dss key",
					SSHException.BAD_API_USAGE);
			}

			p = buffer.ReadBigInteger();
			q = buffer.ReadBigInteger();
			g = buffer.ReadBigInteger();
			y = buffer.ReadBigInteger();

		}

		/// <summary>
		/// Returns the algorithm name of the public key.
		/// </summary>
		public String Algorithm
		{
			get
			{
				return "ssh-dss";
			}
		}
	
		/// <summary>
		/// Verify a signature.
		/// </summary>
		/// <param name="signature">The signature to verify.</param>
		/// <param name="data">The data from which the signature was created.</param>
		/// <returns></returns>
		public override bool VerifySignature(byte[] signature, byte[] data)
		{
			ByteBuffer buffer = new ByteBuffer(signature);

			byte[] sig = buffer.ReadBinaryString();
			String header = new String(SupportClass.ToCharArray(sig));
			if(!header.Equals("ssh-dss")) 
			{
				return false;
			}

			signature = buffer.ReadBinaryString();

			return base.VerifySignature(signature, data);
		}

		/// <summary>
		/// Return an SSH fingerprint of the public key.
		/// </summary>
		public String Fingerprint
		{
			get
			{
				return SSHKeyFingerprint.GenerateFingerprint(this);
			}
		}

		/// <summary>
		/// Evaluates whether this instance is the same public key as another instance.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(Object obj)
		{
			if(obj is SSH2DSAPublicKey)
		    {
				return ((SSH2DSAPublicKey)obj).Fingerprint.Equals(Fingerprint);
			}
			else
			{
				return false;
			}

		}

		/// <summary>
		/// Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
	}
}
