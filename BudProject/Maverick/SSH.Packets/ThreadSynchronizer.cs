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

namespace Maverick.SSH.Packets
{
	/// <summary>
	/// Summary description for ThreadSynchronizer.
	/// </summary>
	public class ThreadSynchronizer
	{
		internal bool isBlocking;

		/// <summary>
		/// Create a synchronization object specifying the blocking state.
		/// </summary>
		/// <remarks>
		/// This is useful if you want to stop other threads obtaining the real lock.
		/// </remarks>
		/// <param name="isBlocking"></param>
		public ThreadSynchronizer(bool isBlocking)
		{
			this.isBlocking = isBlocking;
		}

		/// <summary>
		/// Request to obtain the real block on the object.
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public bool RequestBlock(int timeout)
		{
			lock (this)
			{
				
				bool canBlock = !isBlocking;
				
				if (canBlock)
				{
					isBlocking = true;
				}
				else
				{
					System.Threading.Monitor.Wait(this, TimeSpan.FromMilliseconds(timeout));
				}
				return canBlock;
			}
		}

		/// <summary>
		/// Request to obtain the real block but check for messages first in the packet store.
		/// </summary>
		/// <param name="store"></param>
		/// <param name="observer"></param>
		/// <param name="holder"></param>
		/// <returns></returns>
		public bool RequestBlock(SSHPacketStore store, PacketObserver observer, PacketHolder holder)
		{
			lock (this)
			{

//#if DEBUG
//				System.Diagnostics.Trace.WriteLine(System.Threading.Thread.CurrentThread.Name + ": Obtained lock on sync");
//#endif
				
				if((holder.msg = store.HasMessage(observer)) != null) 
				{
//#if DEBUG
//					System.Diagnostics.Trace.WriteLine(System.Threading.Thread.CurrentThread.Name + ": w00t I've found a message");
//#endif
					return false;
				}

				
				bool canBlock = !isBlocking;
				
				if (canBlock)
				{
//#if DEBUG
//					System.Diagnostics.Trace.WriteLine(System.Threading.Thread.CurrentThread.Name + ": w00t I've got the *real* block");
//#endif
					isBlocking = true;
				}
				else
				{
//#if DEBUG
//					System.Diagnostics.Trace.WriteLine(System.Threading.Thread.CurrentThread.Name + ": :( Waiting on sync in pseudo block");
//#endif

					System.Threading.Monitor.Wait(this);
				}

//#if DEBUG
//				System.Diagnostics.Trace.WriteLine(System.Threading.Thread.CurrentThread.Name + ": Exiting RequestBlock");
//#endif
				return canBlock;
			}
		}

		/// <summary>
		/// Release any waiting threads.
		/// </summary>
		public void ReleaseWaiting() 
		{
//#if DEBUG
//			System.Diagnostics.Trace.WriteLine("Attempting to release waiting");
//#endif

			lock(this)
			{
//#if DEBUG
//				System.Diagnostics.Trace.WriteLine("Releasing the waiting threads");
//#endif

				System.Threading.Monitor.PulseAll(this);
			}
		}

		/// <summary>
		/// Release the block on this object.
		/// </summary>
		public void ReleaseBlock()
		{
			lock(this)
			{
				isBlocking = false;
				System.Threading.Monitor.PulseAll(this);
			}
		}

	}
}
