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
using Maverick.Crypto.IO;
using Maverick.Crypto.Util;

namespace Maverick.SFTP
{
	/// <summary>
	/// This class represents the ATTRS structure defined in the draft-ietf-secsh-filexfer-02.txt 
	/// which is used by the protocol to store file attribute information. 
	/// </summary>
	public class SFTPFileAttributes
	{

		/// <summary>
		/// Get the owners UID
		/// </summary>
		public int UID
		{
			get
			{
				return uid;
			}
			
			set
			{
				flags |= SSH_FILEXFER_ATTR_UIDGID;
				this.uid = value;
			}
			
		}

		/// <summary>
		/// Get the group owners GID
		/// </summary>
		public int GID
		{
			get
			{
				return gid;
			}
			
			set
			{
				flags |= SSH_FILEXFER_ATTR_UIDGID;
				this.gid = value;
			}
			
		}

		/// <summary>
		/// Get the size of the file.
		/// </summary>
		public long Size
		{
			get
			{
				return size;
			}
			
			set
			{
				this.size = value;
				
				// Set the flag
				if (value != 0)
				{
					flags |= SSH_FILEXFER_ATTR_SIZE;
				}
				else
				{
					flags ^= SSH_FILEXFER_ATTR_SIZE;
				}
			}
			
		}

		/// <summary>
		/// Get the integer value of the current permissions
		/// </summary>
		public int Permissions
		{
			get
			{
				return permissions;
			}
			
			set
			{
				this.permissions = value;
				
				// Set the flag
				if (value != 0)
				{
					flags |= SSH_FILEXFER_ATTR_PERMISSIONS;
				}
				else
				{
					flags ^= SSH_FILEXFER_ATTR_PERMISSIONS;
				}
			}
			
		}

		/*public System.String PermissionsString
		{
			set
			{
				int cp = 0;
				
				if (permissions != null)
				{
					cp = cp | (((permissions & S_IFMT) == S_IFMT)?S_IFMT:0);
					cp = cp | (((permissions & S_IFSOCK) == S_IFSOCK)?S_IFSOCK:0);
					cp = cp | (((permissions & S_IFLNK) == S_IFLNK)?S_IFLNK:0);
					cp = cp | (((permissions & S_IFREG) == S_IFREG)?S_IFREG:0);
					cp = cp | (((permissions & S_IFBLK) == S_IFBLK)?S_IFBLK:0);
					cp = cp | (((permissions & S_IFDIR) == S_IFDIR)?S_IFDIR:0);
					cp = cp | (((permissions & S_IFCHR) == S_IFCHR)?S_IFCHR:0);
					cp = cp | (((permissions & S_IFIFO) == S_IFIFO)?S_IFIFO:0);
					cp = cp | (((permissions & S_ISUID) == S_ISUID)?S_ISUID:0);
					cp = cp | (((permissions & S_ISGID) == S_ISGID)?S_ISGID:0);
				}
				
				int len = value.Length;
				
				if (len >= 1)
				{
					cp = cp | ((value[0] == 'r')?SFTPFileAttributes.S_IRUSR:0);
				}
				
				if (len >= 2)
				{
					cp = cp | ((value[1] == 'w')?SFTPFileAttributes.S_IWUSR:0);
				}
				
				if (len >= 3)
				{
					cp = cp | ((value[2] == 'x')?SFTPFileAttributes.S_IXUSR:0);
				}
				
				if (len >= 4)
				{
					cp = cp | ((value[3] == 'r')?SFTPFileAttributes.S_IRGRP:0);
				}
				
				if (len >= 5)
				{
					cp = cp | ((value[4] == 'w')?SFTPFileAttributes.S_IWGRP:0);
				}
				
				if (len >= 6)
				{
					cp = cp | ((value[5] == 'x')?SFTPFileAttributes.S_IXGRP:0);
				}
				
				if (len >= 7)
				{
					cp = cp | ((value[6] == 'r')?SFTPFileAttributes.S_IROTH:0);
				}
				
				if (len >= 8)
				{
					cp = cp | ((value[7] == 'w')?SFTPFileAttributes.S_IWOTH:0);
				}
				
				if (len >= 9)
				{
					cp = cp | ((value[8] == 'x')?SFTPFileAttributes.S_IXOTH:0);
				}
				
				Permissions = new UnsignedInteger32(cp);
			}
		
		}*/

		/// <summary>
		/// Set the permissions using an octal string in the form "0666".
		/// </summary>
		/// <remarks>
		///	The string supplied must be 4 characters long and start with a "0" to indicate
		///	that the number is octal.
		/// </remarks>
		/// 
		public System.String PermissionsFromMaskString
		{
			set
			{
				if (value.Length != 4)
				{
					throw new System.ArgumentException("Mask length must be 4");
				}
				
				try
				{
					Permissions = System.Convert.ToInt32(value, 8);
				}
				catch (System.FormatException)
				{
					throw new System.ArgumentException("Mask must be 4 digit octal number.");
				}
			}
			
		}

		/*public System.String PermissionsFromUmaskString
		{
			set
			{
				if (value.Length != 4)
				{
					throw new System.ArgumentException("umask length must be 4");
				}
				
				try
				{
					Permissions = (System.Convert.ToInt32(value, 8) ^ 511);
				}
				catch (System.FormatException ex)
				{
					throw new System.ArgumentException("umask must be 4 digit octal number");
				}
			}
			
		}*/

		/// <summary>
		/// Get the last time this file was accessed.
		/// </summary>
		public uint AccessedTime
		{
			get
			{
				return atime;
			}
			
		}

		/// <summary>
		/// Get the last time that this file was modified.
		/// </summary>
		public uint ModifiedTime
		{
			get
			{
				return mtime;
			}
			
		}

        /// <summary>
        /// çÏê¨éûä‘
        /// </summary>
        public uint CreateTime
        {
            get
            {
                return createtime;
            }

        }

		/// <summary>
		/// Get or set the current permsions with a string representation. This property expects and returns string's
		/// in the format "rw-r--r--"
		/// </summary>
		public System.String PermissionsString
		{
			get
			{
				if (permissions != 0)
				{
					System.Text.StringBuilder str = new System.Text.StringBuilder();
					str.Append(types[SupportClass.URShift(((int) permissions & S_IFMT), 13)]);
					str.Append(rwxString((int) permissions, 6));
					str.Append(rwxString((int) permissions, 3));
					str.Append(rwxString((int) permissions, 0));
					
					return str.ToString();
				}
				else
				{
					return "";
				}
			}

			set
			{
				int cp = 0;
				
				if (permissions != 0)
				{
					cp = cp | (((permissions & S_IFMT) == S_IFMT)?S_IFMT:0);
					cp = cp | (((permissions & S_IFSOCK) == S_IFSOCK)?S_IFSOCK:0);
					cp = cp | (((permissions & S_IFLNK) == S_IFLNK)?S_IFLNK:0);
					cp = cp | (((permissions & S_IFREG) == S_IFREG)?S_IFREG:0);
					cp = cp | (((permissions & S_IFBLK) == S_IFBLK)?S_IFBLK:0);
					cp = cp | (((permissions & S_IFDIR) == S_IFDIR)?S_IFDIR:0);
					cp = cp | (((permissions & S_IFCHR) == S_IFCHR)?S_IFCHR:0);
					cp = cp | (((permissions & S_IFIFO) == S_IFIFO)?S_IFIFO:0);
					cp = cp | (((permissions & S_ISUID) == S_ISUID)?S_ISUID:0);
					cp = cp | (((permissions & S_ISGID) == S_ISGID)?S_ISGID:0);
				}
				
				int len = value.Length;
				
				if (len >= 1)
				{
					cp = cp | ((value[0] == 'r')?SFTPFileAttributes.S_IRUSR:0);
				}
				
				if (len >= 2)
				{
					cp = cp | ((value[1] == 'w')?SFTPFileAttributes.S_IWUSR:0);
				}
				
				if (len >= 3)
				{
					cp = cp | ((value[2] == 'x')?SFTPFileAttributes.S_IXUSR:0);
				}
				
				if (len >= 4)
				{
					cp = cp | ((value[3] == 'r')?SFTPFileAttributes.S_IRGRP:0);
				}
				
				if (len >= 5)
				{
					cp = cp | ((value[4] == 'w')?SFTPFileAttributes.S_IWGRP:0);
				}
				
				if (len >= 6)
				{
					cp = cp | ((value[5] == 'x')?SFTPFileAttributes.S_IXGRP:0);
				}
				
				if (len >= 7)
				{
					cp = cp | ((value[6] == 'r')?SFTPFileAttributes.S_IROTH:0);
				}
				
				if (len >= 8)
				{
					cp = cp | ((value[7] == 'w')?SFTPFileAttributes.S_IWOTH:0);
				}
				
				if (len >= 9)
				{
					cp = cp | ((value[8] == 'x')?SFTPFileAttributes.S_IXOTH:0);
				}
				
				Permissions = cp;
			}
			
		}

		/// <summary>
		/// Gets the current mask string representation of the current permissions. This property
		/// will return values in the format "0666".
		/// </summary>
		public System.String MaskString
		{
			get
			{
				System.Text.StringBuilder buf = new System.Text.StringBuilder();
				buf.Append('0');
				
				int i = (int) permissions;
				buf.Append(octal(i, 6));
				buf.Append(octal(i, 3));
				buf.Append(octal(i, 0));
				
				return buf.ToString();
			}
			
		}

		/// <summary>
		/// Determines whether this file is a directory.
		/// </summary>
		public bool IsDirectory
		{
			get
			{
				//  TODO This is long hand because gcj chokes when it is not? Investigate why
				if (permissions != 0 && (permissions & SFTPFileAttributes.S_IFDIR) == SFTPFileAttributes.S_IFDIR)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
		}

		/// <summary>
		/// Determines whether this file is a file.
		/// </summary>
		public bool IsFile
		{
			get
			{
				//  TODO This is long hand because gcj chokes when it is not? Investigate why
				if (permissions != 0 && (permissions & SFTPFileAttributes.S_IFREG) == SFTPFileAttributes.S_IFREG)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
		}

		/// <summary>
		/// Determines whether this file is a symbolic link.
		/// </summary>
		public bool IsLink
		{
			get
			{
				//  TODO This is long hand because gcj chokes when it is not? Investigate why
				if (permissions != 0 && (permissions & SFTPFileAttributes.S_IFLNK) == SFTPFileAttributes.S_IFLNK)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
		}

		/// <summary>
		/// Determines wether this file is a FIFO special file.
		/// </summary>
		public bool IsFifo
		{
			get
			{
				//  TODO This is long hand because gcj chokes when it is not? Investigate why
				if (permissions != 0 && (permissions & SFTPFileAttributes.S_IFIFO) == SFTPFileAttributes.S_IFIFO)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
		}

		/// <summary>
		/// Determines whether this file is a block device.
		/// </summary>
		public bool IsBlock
		{
			get
			{
				//  TODO This is long hand because gcj chokes when it is not? Investigate why
				if (permissions != 0 && (permissions & SFTPFileAttributes.S_IFBLK) == SFTPFileAttributes.S_IFBLK)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
		}

		/// <summary>
		/// Determines whether this file is a character device.
		/// </summary>
		public bool IsCharacter
		{
			get
			{
				//  TODO This is long hand because gcj chokes when it is not? Investigate why
				if (permissions != 0 && (permissions & SFTPFileAttributes.S_IFCHR) == SFTPFileAttributes.S_IFCHR)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
		}

		/// <summary>
		/// Determines whether this file is a Socket.
		/// </summary>
		public bool IsSocket
		{
			get
			{
				//  TODO This is long hand because gcj chokes when it is not? Investigate why
				if (permissions != 0 && (permissions & SFTPFileAttributes.S_IFSOCK) == SFTPFileAttributes.S_IFSOCK)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
		}
		
		private const long SSH_FILEXFER_ATTR_SIZE = 0x0000001;
		private const long SSH_FILEXFER_ATTR_UIDGID = 0x00000002;
		private const long SSH_FILEXFER_ATTR_PERMISSIONS = 0x0000004;
		
		// Also as ACMODTIME for version 3
		private const long SSH_FILEXFER_ATTR_ACCESSTIME = 0x0000008;
		
		// Posix stats
		
		/// <summary>
		/// Permissions flag: Format mask constant can be used to mask off a file type from the mode.
		/// </summary>
		public const int S_IFMT = 0xF000;
		/// <summary>
		/// Permissions flag: Identifies the file as a socket
		/// </summary>
		public const int S_IFSOCK = 0xC000;
		/// <summary>
		/// Permissions flag: Identifies the file as a symbolic link
		/// </summary>
		public const int S_IFLNK = 0xA000;
		/// <summary>
		/// Permissions flag: Identifies the file as a regular file
		/// </summary>
		public const int S_IFREG = 0x8000;
		/// <summary>
		/// Permissions flag: Identifies the file as a block special file
		/// </summary>
		public const int S_IFBLK = 0x6000;
		/// <summary>
		/// Permissions flag: Identifies the file as a directory
		/// </summary>
		public const int S_IFDIR = 0x4000;
		/// <summary>
		/// Permissions flag: Identifies the file as a character device
		/// </summary>
		public const int S_IFCHR = 0x2000;
		/// <summary>
		/// Permissions flag: Identifies the file as a pipe
		/// </summary>
		public const int S_IFIFO = 0x1000;
		/// <summary>
		/// Permissions flag: Bit to determine whether a file is executed as the owner
		/// </summary>
		public const int S_ISUID = 0x800;
		/// <summary>
		/// Permissions flag: Bit to determine whether a file is executed as the group owner
		/// </summary>
		public const int S_ISGID = 0x400;
		/// <summary>
		/// Permissions flag: Permits the owner of a file to read the file.
		/// </summary>
		public const int S_IRUSR = 0x100;
		/// <summary>
		/// Permissions flag: Permits the owner of a file to write to the file.
		/// </summary>
		public const int S_IWUSR = 0x80;
		/// <summary>
		/// Permissions flag: Permits the owner of a file to execute the file or to search the file's directory.
		/// </summary>
		public const int S_IXUSR = 0x40;
		/// <summary>
		/// Permissions flag: Permits a file's group to read the file.
		/// </summary>
		public const int S_IRGRP = 0x20;
		/// <summary>
		/// Permissions flag: Permits a file's group to write to the file.
		/// </summary>
		public const int S_IWGRP = 0x10;
		/// <summary>
		/// Permissions flag: Permits a file's group to execute the file or to search the file's directory.
		/// </summary>
		public const int S_IXGRP = 0x08;
		/// <summary>
		/// Permissions flag: Permits others to read the file.
		/// </summary>
		public const int S_IROTH = 0x04;
		/// <summary>
		/// Permissions flag: Permits others to write to the file.
		/// </summary>
		public const int S_IWOTH = 0x02;
		/// <summary>
		/// Permissions flag: Permits others to execute the file or to search the file's directory.
		/// </summary>
		public const int S_IXOTH = 0x01;
		
		internal int version = 3;
		internal long flags = 0x0000000; // Version 3 & 4
		//internal int type; // Version 4 only
		internal long size = 0; // Version 3 & 4
		internal int uid = 0; // Version 3 only
		internal int gid = 0; // Version 3 only
		internal int permissions = 0; // Version 3 & 4
		internal uint atime = 0; // Version 3 & 4
		internal uint createtime = 0; // Version 4 only
		internal uint mtime = 0; // Version 3 & 4
		
		internal char[] types = new char[]{'p', 'c', 'd', 'b', '-', 'l', 's'};
		
		/// <summary>
		/// Create an empty attributes instance.
		/// </summary>
		/// <remarks>
		/// The SFTP protocol allows a permissions structure to contain variable information. This contructor
		/// creates an instance that contains non of the attribute fields set.</remarks>
		public SFTPFileAttributes()
		{
		}
		
		/// <summary>
		/// Create an attributes instance reading the buffer to extract the permissions from the strucutre defined
		/// by the SFTP protocol.
		/// </summary>
		/// <param name="bar"></param>
		public SFTPFileAttributes(ByteBuffer bar)
		{
			flags = bar.ReadUINT32();
			
			if (isFlagSet(SSH_FILEXFER_ATTR_SIZE))
			{
				size = (long)bar.ReadUINT64();
			}
			
			if (isFlagSet(SSH_FILEXFER_ATTR_UIDGID))
			{
				uid = (int)bar.ReadUINT32();
				gid = (int)bar.ReadUINT32();
			}
			
			if (isFlagSet(SSH_FILEXFER_ATTR_PERMISSIONS))
			{
				permissions = (int)bar.ReadUINT32();
			}
			
			if (isFlagSet(SSH_FILEXFER_ATTR_ACCESSTIME))
			{
				atime = bar.ReadUINT32();
				mtime = bar.ReadUINT32();
			}
		}

		/// <summary>
		/// Set the time fields of the attributes.
		/// </summary>
		/// <param name="atime"></param>
		/// <param name="mtime"></param>
		/// <remarks>
		/// Setting the times using this method does not affect the remote file until the attributes have been
        /// set using the <see cref="Maverick.SFTP.SFTPSubsystemChannel.SetAttributes(string, Maverick.SFTP.SFTPFileAttributes)"/> method.
		/// </remarks>
		public void SetTimes(DateTime atime, DateTime mtime)
		{
			SetTimes((uint)SupportClass.ConvertToEpochTime(atime),
				(uint)SupportClass.ConvertToEpochTime(mtime));
		}
		
		/// <summary>
		/// Set the last access and last modified times. These times are represented by integers containing 
		/// the number of seconds from Jan 1, 1970 UTC.
		/// </summary>
		/// <param name="atime">The time in seconds since Midnight January 1, 1970</param>
		/// <param name="mtime">The time in seconds since Midnight January 1, 1970</param>
		public void SetTimes(uint atime, uint mtime)
		{
			this.atime = atime;
			this.mtime = mtime;
			
			// Set the flag
			if (atime != 0)
			{
				flags |= SSH_FILEXFER_ATTR_ACCESSTIME;
			}
			else
			{
				flags ^= SSH_FILEXFER_ATTR_ACCESSTIME;
			}
		}
		
		/// <summary>
		/// Determines if the given flag is set in the attributes struture.
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		public bool isFlagSet(long flag)
		{
			return ((flags & (flag & 0xFFFFFFFFL)) == (flag & 0xFFFFFFFFL));
		}
		
		/// <summary>
		/// Returns an array of bytes encoded to the format expected by the SFTP protocol for the ATTRS structure.
		/// </summary>
		/// <returns></returns>
		public byte[] ToByteArray()
		{
			ByteBuffer baw = new ByteBuffer();
			
			baw.WriteUINT32((uint)flags);
			
			if (isFlagSet(SSH_FILEXFER_ATTR_SIZE))
			{
				baw.WriteUINT64(size);
			}
			
			if (isFlagSet(SSH_FILEXFER_ATTR_UIDGID))
			{
				if (uid != 0)
				{
					baw.WriteUINT32(uid);
				}
				else
				{
					baw.WriteUINT32(0);
				}
				
				if (gid != 0)
				{
					baw.WriteUINT32(gid);
				}
				else
				{
					baw.WriteUINT32(0);
				}
			}
			
			if (isFlagSet(SSH_FILEXFER_ATTR_PERMISSIONS))
			{
				baw.WriteUINT32(permissions);
			}
			
			if (isFlagSet(SSH_FILEXFER_ATTR_ACCESSTIME))
			{
				baw.WriteUINT32(atime);
				baw.WriteUINT32(mtime);
			}
			
			return baw.ToByteArray();
		}
		
		private int octal(int v, int r)
		{
			v = SupportClass.URShift(v, r);
			
			return (((v & 0x04) != 0)?4:0) + (((v & 0x02) != 0)?2:0) + + (((v & 0x01) != 0)?1:0);
		}
		
		private System.String rwxString(int v, int r)
		{
			v = SupportClass.URShift(v, r);
			
			System.String rwx = ((((v & 0x04) != 0)?"r":"-") + (((v & 0x02) != 0)?"w":"-"));
			
			if (((r == 6) && ((permissions & S_ISUID) == S_ISUID)) || ((r == 3) && ((permissions & S_ISGID) == S_ISGID)))
			{
				rwx += (((v & 0x01) != 0)?"s":"S");
			}
			else
			{
				rwx += (((v & 0x01) != 0)?"x":"-");
			}
			
			return rwx;
		}
	}
}
