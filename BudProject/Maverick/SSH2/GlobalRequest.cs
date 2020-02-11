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
	/// This class represents a global request. 
	/// </summary>
	public class GlobalRequest
	{
		/// <summary>
		/// The name of the global request.
		/// </summary>
		public System.String Name
		{
			get
			{
				return name;
			}
			
		}

		/// <summary>
		/// The requests data
		/// </summary>
		public byte[] Data
		{
			get
			{
				return requestdata;
			}
			
			set
			{
				this.requestdata = value;
			}
			
		}
		
		internal System.String name;
		internal byte[] requestdata;
		
		/// <summary>
		/// Create a global request.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="requestdata"></param>
		public GlobalRequest(System.String name, byte[] requestdata)
		{
			this.name = name;
			this.requestdata = requestdata;
		}
	}
}
