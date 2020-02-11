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

namespace Maverick.SFTP
{
	/// <summary>
	/// This class is the representation of a remote file object.
	/// </summary>
	/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
	public class SFTPFile
	{

		/// <summary>
		/// Returns the parent of the current file.
		/// </summary>
		public SFTPFile Parent
		{
			get
			{
				
				if (absolutePath.LastIndexOf((Char) '/') == - 1)
				{
					// This is simply a filename so the parent is the users default directory
					String dir = sftp.DefaultDirectory;
					return sftp.GetFile(dir);
				}
				else
				{
					
					// Extract the filename from the absolute path and return the parent
					String path = sftp.GetAbsolutePath(absolutePath);
					
					if (path.Equals("/"))
						return null;
					
					// If we have . or .. then strip the path and let getParent start over
					// again with the correct canonical path
					if (filename.Equals(".") || filename.Equals(".."))
					{
						return sftp.GetFile(path).Parent;
					}
					else
					{
						int idx = path.IndexOf((Char) '/');
						
						String parent = "";
						while (idx != - 1)
						{
							int next = path.IndexOf((System.Char) '/', idx + 1);
							
							if (next != - 1)
							{
								String name = path.Substring(idx + 1, (next) - (idx + 1));
								if (name.Equals(filename))
									break;
								else
									parent += "/" + name;
							}
							idx = next;
						}
						
						// Check if we at the root if so we will have to add /
						if (parent.Equals(""))
							parent = "/";
						
						return sftp.GetFile(parent);
					}
				}
			}
			
		}

		/// <summary>
		/// Indicates whether a valid file handle is currently open for this file.
		/// </summary>
		public bool IsOpen
		{
			get
			{
				if (sftp == null || handle == null)
				{
					return false;
				}
				
				return sftp.IsValidHandle(handle);
			}
			
		}

		/// <summary>
		/// The open file handle.
		/// </summary>
		/// <remarks>
		/// This property may return a null value.
		/// </remarks>
		public byte[] Handle
		{
			get
			{
				return handle;
			}

			set
			{
				this.handle = value;
			}
			
		}

		internal SFTPSubsystemChannel SFTPSubsystem
		{
			set
			{
				this.sftp = value;
			}
			
		}

		/// <summary>
		/// Returns the underlying channel instance that created this file.
		/// </summary>
		public SFTPSubsystemChannel SFTPChannel
		{
			get
			{
				return sftp;
			}
			
		}

		/// <summary>
		/// Get the filename for the file. 
		/// </summary>
		/// <remarks>
		/// This property returns just the name of the file. If you require the absolute path use
		/// the <see cref="Maverick.SFTP.SFTPFile.AbsolutePath"/> property.
		/// </remarks>
		public String Filename
		{
			get
			{
				return filename;
			}
			
		}

		/// <summary>
		/// Get the attrbributes of this file.
		/// </summary>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		public SFTPFileAttributes Attributes
		{
			get
			{
				if (attrs == null)
				{
					attrs = sftp.GetAttributes(this);
				}
				
				return attrs;
			}
			
		}

		/// <summary>
		/// Get the absolute path of this file.
		/// </summary>
		public String AbsolutePath
		{
			get
			{
				return absolutePath;
			}
			
		}

		/// <summary>
		/// Determines whether this file is a directory.
		/// </summary>
		public bool IsDirectory
		{
			get
			{
				if ((Attributes.Permissions & SFTPFileAttributes.S_IFDIR) == SFTPFileAttributes.S_IFDIR)
					return true;
				else
					return false;
			}
			
		}

		/// <summary>
		/// Determines whether this file is a file.
		/// </summary>
		public bool IsFile
		{
			get
			{
				if ((Attributes.Permissions & SFTPFileAttributes.S_IFREG) == SFTPFileAttributes.S_IFREG)
					return true;
				else
					return false;
			}
			
		}

		/// <summary>
		/// Determines whether this file is a symbolic link.
		/// </summary>
		public bool IsLink
		{
			get
			{
				if ((Attributes.Permissions & SFTPFileAttributes.S_IFLNK) == SFTPFileAttributes.S_IFLNK)
					return true;
				else
					return false;
			}
			
		}

		/// <summary>
		/// Determines whether this file a FIFO special file.
		/// </summary>
		public bool IsFifo
		{
			get
			{
				if ((Attributes.Permissions & SFTPFileAttributes.S_IFIFO) == SFTPFileAttributes.S_IFIFO)
					return true;
				else
					return false;
			}
			
		}


		/// <summary>
		/// Determines whether this file is a block device.
		/// </summary>
		public bool IsBlock
		{
			get
			{
				if ((Attributes.Permissions & SFTPFileAttributes.S_IFBLK) == SFTPFileAttributes.S_IFBLK)
					return true;
				else
					return false;
			}
			
		}

		/// <summary>
		/// Determines whether this file is a character device
		/// </summary>
		public bool IsCharacter
		{
			get
			{
				if ((Attributes.Permissions & SFTPFileAttributes.S_IFCHR) == SFTPFileAttributes.S_IFCHR)
					return true;
				else
					return false;
			}
			
		}

		/// <summary>
		/// Determines whether this file is a Socket.
		/// </summary>
		public bool IsSocket
		{
			get
			{
				if ((Attributes.Permissions & SFTPFileAttributes.S_IFSOCK) == SFTPFileAttributes.S_IFSOCK)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
		}

		internal System.String filename;
		internal byte[] handle;
		internal SFTPFileAttributes attrs;
		internal SFTPSubsystemChannel sftp;
		internal String absolutePath;
		

		/// <summary>
		/// Create a file instance with the absolute path and its attributes.
		/// </summary>
		/// <param name="absolutePath"></param>
		/// <param name="attrs"></param>
		public SFTPFile(String absolutePath, SFTPFileAttributes attrs)
		{
			this.absolutePath = absolutePath;
			
			int i = absolutePath.LastIndexOf((System.Char) '/');
			
			if (i > - 1)
			{
				this.filename = absolutePath.Substring(i + 1);
			}
			else
			{
				this.filename = absolutePath;
			}
			
			this.attrs = attrs;
		}
		
		/// <summary>
		/// Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return absolutePath.GetHashCode();
		}
		
		/// <summary>
		/// Determines whether two Object instances are equal.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public  override bool Equals(System.Object obj)
		{
			if (obj is SFTPFile)
			{
				bool match = ((SFTPFile) obj).AbsolutePath.Equals(absolutePath);
				if (handle == null && (((SFTPFile) obj).handle == null))
				{
					return match;
				}
				else
				{
					if (handle != null && ((SFTPFile) obj).handle != null)
					{
						for (int i = 0; i < handle.Length; i++)
						{
							if (((SFTPFile) obj).handle[i] != handle[i])
								return false;
						}
					}
					return match;
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// Create a file instance with the absolute path
		/// </summary>
		/// <param name="absolutePath"></param>
		public SFTPFile(System.String absolutePath):this(absolutePath, new SFTPFileAttributes())
		{
		}

		/// <summary>
		/// Attempt to delete this file.
		/// </summary>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		public void Delete()
		{
			if (sftp == null)
			{
				throw new SSHException("Instance not connected to SFTP subsystem", SSHException.BAD_API_USAGE);
			}
			
			if (IsDirectory)
			{
				sftp.RemoveDirectory(AbsolutePath);
			}
			else
			{
				sftp.RemoveFile(AbsolutePath);
			}
		}
		
		/// <summary>
		/// Rename the file.
		/// </summary>
		/// <param name="newFilename"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		public void Rename(String newFilename)
		{
			if (sftp == null)
			{
				throw new SSHException("Instance not connected to SFTP subsystem", 
					SSHException.BAD_API_USAGE);
			}
			
			sftp.RenameFile(AbsolutePath + filename, newFilename);
		}
		
		/// <summary>
		/// Close the open file handle.
		/// </summary>		
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		public void Close()
		{
			sftp.CloseFile(this);
		}
	}
}
