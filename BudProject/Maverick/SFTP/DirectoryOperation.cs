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
using Maverick.Crypto.Util;

namespace Maverick.SFTP
{
	/// <summary>
	/// This class provides a list of operations that have been/or will be completed by 
	/// the <see cref="Maverick.SFTP.SFTPClient"/>'s <see cref="Maverick.SFTP.SFTPClient.CopyRemoteDirectory"/> 
	/// and <see cref="Maverick.SFTP.SFTPClient.CopyLocalDirectory"/> methods.
	/// </summary>
	public class DirectoryOperation
	{
		private void  InitBlock()
		{
			unchangedFiles = new System.Collections.ArrayList();
			newFiles = new System.Collections.ArrayList();
			updatedFiles = new System.Collections.ArrayList();
			deletedFiles = new System.Collections.ArrayList();
			recursedDirectories = new System.Collections.ArrayList();
		}

		/// <summary>
		/// The collection of new files that will be transfered during this directory operation.
		/// </summary>
		public System.Collections.ArrayList NewFiles
		{
			get
			{
				return newFiles;
			}
			
		}

		/// <summary>
		/// The collection of files that will be updated during this directory operation
		/// </summary>
		public System.Collections.ArrayList UpdatedFiles
		{
			get
			{
				return updatedFiles;
			}
			
		}

		/// <summary>
		/// The collection of files that will remain unchanged during this directory operation.
		/// </summary>
		public System.Collections.ArrayList UnchangedFiles
		{
			get
			{
				return unchangedFiles;
			}
			
		}

		/// <summary>
		/// The collection of files that will be deleted by this directory operation.
		/// </summary>
		public System.Collections.ArrayList DeletedFiles
		{
			get
			{
				return deletedFiles;
			}
			
		}

		/// <summary>
		/// The number of files to be transfered.
		/// </summary>
		public int FileCount
		{
			get
			{
				return newFiles.Count + updatedFiles.Count;
			}
			
		}

		/// <summary>
		/// The total number of bytes that will/or have been transfered during this directory operation.
		/// </summary>
		public long TransferSize
		{
			get
			{
				
				System.Object obj;
				long size = 0;
				SFTPFile sftpfile;
				System.IO.FileInfo file;

				for (System.Collections.IEnumerator e = newFiles.GetEnumerator(); e.MoveNext(); )
				{
					obj = e.Current;
					if (obj is System.IO.FileInfo)
					{
						file = (System.IO.FileInfo) obj;
						if (System.IO.File.Exists(file.FullName))
						{
							size += SupportClass.FileLength(file);
						}
					}
					else if (obj is SFTPFile)
					{
						sftpfile = (SFTPFile) obj;
						if (sftpfile.IsFile)
						{
							size += sftpfile.Attributes.Size;
						}
					}
				}

				for (System.Collections.IEnumerator e = updatedFiles.GetEnumerator(); e.MoveNext(); )
				{
					obj = e.Current;
					
					if (obj is System.IO.FileInfo)
					{
						file = (System.IO.FileInfo) obj;
						if (System.IO.File.Exists(file.FullName))
						{
							size += SupportClass.FileLength(file);
						}
					}
					else if (obj is SFTPFile)
					{
						sftpfile = (SFTPFile) obj;
						if (sftpfile.IsFile)
						{
							size += sftpfile.Attributes.Size;
						}
					}
				}
				
				// Add a value for deleted files??
				
				return size;
			}
			
		}
		
		internal System.Collections.ArrayList unchangedFiles;
		internal System.Collections.ArrayList newFiles;
		internal System.Collections.ArrayList updatedFiles;
		internal System.Collections.ArrayList deletedFiles;
		internal System.Collections.ArrayList recursedDirectories;
		
		/// <summary>Construct a new directory operation.
		/// </summary>
		public DirectoryOperation()
		{
			InitBlock();
		}
		
		/// <summary>
		/// Returns <em>true</em> if the operation contains the given file.
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public bool ContainsFile(System.IO.FileInfo f)
		{
			return ContainsObject(f, unchangedFiles) 
				|| ContainsObject(f, newFiles) 
				|| ContainsObject(f, updatedFiles) 
				|| ContainsObject(f, deletedFiles) 
				|| ContainsObject(f, recursedDirectories);
		}
		
		/// <summary>
		/// Returns <em>true</em> if the operation contains the given <see cref="Maverick.SFTP.SFTPFile"/>.
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public bool containsFile(SFTPFile f)
		{
			return ContainsObject(f, unchangedFiles) 
				|| ContainsObject(f, newFiles) 
				|| ContainsObject(f, updatedFiles) 
				|| ContainsObject(f, deletedFiles) 
				|| ContainsObject(f, recursedDirectories);
		}
		
		/// <summary>
		/// Add an existing operation to another. This is typically used when directories are recursed.
		/// </summary>
		/// <param name="op"></param>
		/// <param name="f">The local directory which is described by the operation.</param>
		public void AddDirectoryOperation(DirectoryOperation op, System.IO.FileInfo f)
		{
			AddAll(op.UpdatedFiles, updatedFiles);
			AddAll(op.NewFiles, newFiles);
			AddAll(op.UnchangedFiles, unchangedFiles);
			AddAll(op.DeletedFiles, deletedFiles);
			recursedDirectories.Add(f);
		}
		
		internal void  AddAll(System.Collections.ArrayList source, System.Collections.ArrayList dest)
		{
			for (System.Collections.IEnumerator e = source.GetEnumerator(); e.MoveNext(); )
			{
				dest.Add(e.Current);
			}
		}
		
		/// <summary>
		/// Add an existing remote operation to another. This is typically used when directories are recursed.
		/// </summary>
		/// <param name="op"></param>
		/// <param name="file">The remote directory which is described by the operation.</param>
		public void  AddDirectoryOperation(DirectoryOperation op, System.String file)
		{
			AddAll(op.UpdatedFiles, updatedFiles);
			AddAll(op.NewFiles, newFiles);
			AddAll(op.UnchangedFiles, unchangedFiles);
			AddAll(op.DeletedFiles, deletedFiles);
			recursedDirectories.Add(file);
		}

		private bool ContainsObject(Object obj, System.Collections.ICollection list)
		{
			System.Collections.IEnumerator e = list.GetEnumerator();
			while(e.MoveNext())
			{
				Object o = e.Current;
				if(o is System.IO.FileInfo) 
				{
					System.IO.FileInfo f1 = (System.IO.FileInfo) o;
					if(obj is System.IO.FileInfo)
					{
						System.IO.FileInfo f2 = (System.IO.FileInfo) obj;
						if(f1.FullName == f2.FullName
							&& f1.GetType() == f2.GetType())
							return true;
					}
				} else if(o.Equals(obj))
					return true;
			}

			return false;
		}
	}
}
