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
 * 
 * This file has been derived from the BouncyCastle Java Crypto API
 * 
 * Copyright (c) 2000 The Legion Of The Bouncy Castle (http://www.bouncycastle.org)
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do 
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *****************************************************************************/
using System;
using Maverick.Crypto.Math;

namespace Maverick.Crypto.DSA
{
	/// <summary>
	/// Summary description for DSAKey.
	/// </summary>
	public class DSAKey
	{
		virtual public BigInteger P
		{
			get
			{
				return p;
			}
			
		}
		virtual public BigInteger Q
		{
			get
			{
				return q;
			}
			
		}
		virtual public BigInteger G
		{
			get
			{
				return g;
			}
			
		}
		
		protected internal BigInteger p;
		protected internal BigInteger q;
		protected internal BigInteger g;
		
		internal DSAKey()
		{
		}
		
		internal DSAKey(BigInteger p, BigInteger q, BigInteger g)
		{
			this.p = p;
			this.q = q;
			this.g = g;
		}
		
	}
}
