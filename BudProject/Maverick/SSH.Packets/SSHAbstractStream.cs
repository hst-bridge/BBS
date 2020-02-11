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

namespace Maverick.SSH.Packets
{

	/// <summary>
	/// Implements the absract features of an SSH channels stream.
	/// </summary>
	public abstract class SSHAbstractStream : Stream
	{
			private void  InitBlock(SSHAbstractChannel enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			/// <summary>
			/// Used to access the enclosing channel instance.
			/// </summary>
			protected SSHAbstractChannel enclosingInstance;
			
		
			internal SSHAbstractStream(SSHAbstractChannel enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}
			
			/// <summary>
			/// Close down the input part of the stream.
			/// </summary>
			public abstract void CloseInput();

			/// <summary>
			/// Close down the output part of the stream.
			/// </summary>
			public abstract void CloseOutput();

			/// <summary>
			/// Returns how many bytes are available to be read from this stream.
			/// </summary>
			/// <returns></returns>
			public abstract int Available();

			/// <summary>
			/// Indicates whether the stream can be read from. This method will always return true.
			/// </summary>
			public override bool CanRead
			{
				get
				{
					return true;
				}
			}
			
			/// <summary>
			/// Indicates whether the stream can be written to. This method will always return true
			/// </summary>
			public override bool CanWrite
			{
				get
				{
					return true;
				}
			}

			/// <summary>
			/// Flushing SSH streams has no effect.
			/// </summary>
			public override void Flush()
			{
			}

			/// <summary>
			/// Indicates whether the stream can be used for random access. SSH streams DO NOT support
			/// seeking in any form.
			/// </summary>
			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

		/// <summary>
		/// This method is not supported by SSH streams.
		/// </summary>
		/// <param name="len"></param>
		/// <param name="origin"></param>
		/// <returns></returns>
		public override long Seek(long len, SeekOrigin origin)
		{
			throw new InvalidOperationException("Seek is not supported on SSH streams");
								
		}

			/// <summary>
			/// Not supported by this implementation.
			/// </summary>
			public override long Length
			{
				get
				{
					throw new NotSupportedException("Length property is not supported");
				}
			}

			/// <summary>
			/// Not supported by this implementation
			/// </summary>
			public override long Position
			{
				get
				{
					throw new NotSupportedException("Position property is not supported");
				}

				set
				{
					throw new NotSupportedException("Position property is not supported");
				}
			}

			/// <summary>
			/// Not supported by this implementation.
			/// </summary>
			/// <param name="length"></param>
			public override void SetLength(long length)
			{
				throw new NotSupportedException("SetLength method is not supported");
			}

	}
}
