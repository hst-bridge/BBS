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

namespace Maverick.SSH
{
	/// <summary>
	/// Interface for SSH supported public keys.
	/// </summary>
	public interface SSHPublicKey
	{

			/// <summary>
			/// Initialize the public key from a blob of binary data. 
			/// </summary>
			/// <param name="blob">The encoded public key</param>
			/// <param name="start">The location in the array where the encoded data starts</param>
			/// <param name="len">The length of the encoded data</param>
			void Init(byte[] blob, int start, int len);

			/// <summary>
			/// Get the bit length of the public key.
			/// </summary>
			int BitLength
			{
				get;
			}

			/// <summary>
			/// Encode the public key into a blob of binary data, the encoded result will be 
			/// passed into init to recreate the key. 
			/// </summary>
			/// <returns></returns>
			byte[] GetEncoded();

			/// <summary>
			/// Return an SSH fingerprint of the public key.
			/// </summary>
			String Fingerprint
			{
				get;
			}

			/// <summary>
			/// Returns the algorithm name of the public key.
			/// </summary>
			String Algorithm
			{
				get;
			}

			/// <summary>
			/// Verify a signature.
			/// </summary>
			/// <param name="signature">The signature to verify.</param>
			/// <param name="data">The data from which the signature was created.</param>
			/// <returns></returns>
			bool VerifySignature(byte[] signature,
				byte[] data);
	}
}
