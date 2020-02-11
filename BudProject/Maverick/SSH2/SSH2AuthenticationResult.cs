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
	/// This exception is thrown when an authentication result is received from the remote server. This 
	/// forces the authentication client out of its reading state and enables the return of the 
	/// value to the caller.
	/// </summary>
	public class SSH2AuthenticationResult : Exception
	{
		AuthenticationResult result;
		String availableMethods;

		/// <summary>
		/// The actual result.
		/// </summary>
		public AuthenticationResult Result
		{
			get
			{
				return result;
			}
		}

		/// <summary>
		/// The list of available authentication methods that can be used to continue.
		/// </summary>
		public String AvailableAuthenticationMethods
		{
			get
			{
				return availableMethods;
			}
		}

		/// <summary>
		/// Create an authentication result exception.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="availableMethods"></param>
		public SSH2AuthenticationResult(AuthenticationResult result,
			String availableMethods)
		{
			this.result = result;
			this.availableMethods = availableMethods;
		}
	}
}
