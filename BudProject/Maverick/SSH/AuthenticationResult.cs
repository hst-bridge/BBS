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
	/// An enumeration of the available authentication results.
	/// </summary>
	public enum AuthenticationResult
	{
		/// <summary>
		/// The authentication attempt failed.
		/// </summary>
		FAILED,
		/// <summary>
		/// The authentication attempt succeeded but a further authentication is required.
		/// </summary>
		FURTHER_AUTHENTICATION_REQUIRED,
		/// <summary>
		/// The authentication was successful.
		/// </summary>
		COMPLETE,
		/// <summary>
		/// The user canceled the authentication process.
		/// </summary>
		CANCELLED,
		/// <summary>
		/// The public key was acceptable for authentication. This is used soley by public key
		/// authentication mechanisms.
		/// </summary>
		PUBLIC_KEY_ACCEPTABLE,
		/// <summary>
		/// No result was returned.
		/// </summary>
		NO_RESULT
	}
}
