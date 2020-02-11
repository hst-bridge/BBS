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

namespace Maverick.SSH2
{
	/// <summary>
	/// There are several kinds of requests that affect the state of the remote end "globally", independent
	/// of any channels, this interface defines the contract for handling such global requests. 
	/// </summary>
	public interface GlobalRequestHandler
	{
		/// <summary>
		/// The list of support global request names.
		/// </summary>
		String[] SupportedRequests
		{
			get;
		}

		/// <summary>
		/// Process a global request.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		bool ProcessGlobalRequest(GlobalRequest request);
	}
}
