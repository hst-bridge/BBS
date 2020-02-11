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
using System.Text;
using Maverick.SSH;
using System.Collections;
using Maverick.SSH.Packets;
using Maverick.Crypto.IO;
using Maverick.Events;
using Maverick.Crypto.Util;
using System.Collections.Generic;
using Common;

namespace Maverick.SFTP
{
	/// <summary>
	/// This class implements the SFTP protocol which is executed as an SSH subsystem. The example 
	/// demonstrates how to execute subsystem on both SSH1 and SSH2 connections. 
	///	There is no subsystem support within SSH1 which requires that the command is executed directly.<br/>
	///	<br/>
	/// </summary>
	/// <example>
	/// <code>
	/// // Create an SshClient
	/// SSHConnector con = SSHConnector.Create();
	/// // Connect and authenticate an SshClient
	/// SSHClient ssh = con.Connect(....);
	/// ....
	/// 
	/// // Create and initialize an SftpSubsystemChannel
	/// SSHSession session = ssh.OpenSessionChannel();
	/// if(session is SSH2Session)
	///		((SSH2Session)session).StartSubsystem("sftp");
	///	else
	///		session.ExecuteCommand("/usr/sbin/sftp-server");
	///	
	///	SFTPSubsystemChannel sftp = new SFTPSubsystemChannel(session);
	///	sftp.Initialize();
	///	</code>
	///	</example>
	/// <remarks>
	/// It is recommended that this class only be used when it is not suitable to use the  
	/// <see cref="Maverick.SFTP.SFTPClient"/>
	/// </remarks>
	public class SFTPSubsystemChannel : SubsystemChannel
	{
		internal SFTPThreadSynchronizer sync;
		//private System.Text.Encoding CHARSET_ENCODING = System.Text.Encoding.GetEncoding("ISO-8859-1");
        private System.Text.Encoding CHARSET_ENCODING = System.Text.Encoding.UTF8;

		/// <summary>
		/// File open flag, opens the file for reading.
		/// </summary>
		public const int OPEN_READ = 0x00000001;
		/// <summary>
		/// File open flag, opens the file for writing.
		/// </summary>
		public const int OPEN_WRITE = 0x00000002;
		/// <summary>
		/// File open flag, forces all writes to append data at the end of the file.
		/// </summary>
		public const int OPEN_APPEND = 0x00000004;
		/// <summary>
		/// File open flag, if specified a new file will be created if one does not already exist.
		/// </summary>
		public const int OPEN_CREATE = 0x00000008;
		/// <summary>
		/// File open flag, forces an existing file with the same name to be truncated to zero length when creating a file by specifying OPEN_CREATE.
		/// </summary>
		public const int OPEN_TRUNCATE = 0x00000010;
		/// <summary>
		/// File open flag, causes an open request to fail if the named file already exists.
		/// </summary>
		public const int OPEN_EXCLUSIVE = 0x00000020;
		
		internal const int STATUS_FX_OK = 0;
		internal const int STATUS_FX_EOF = 1;
		
		internal const int SSH_FXP_INIT = 1;
		internal const int SSH_FXP_VERSION = 2;
		internal const int SSH_FXP_OPEN = 3;
		internal const int SSH_FXP_CLOSE = 4;
		internal const int SSH_FXP_READ = 5;
		internal const int SSH_FXP_WRITE = 6;
		
		internal const int SSH_FXP_FSTAT = 8;
		internal const int SSH_FXP_SETSTAT = 9;
		internal const int SSH_FXP_FSETSTAT = 10;
		internal const int SSH_FXP_OPENDIR = 11;
		internal const int SSH_FXP_READDIR = 12;
		internal const int SSH_FXP_REMOVE = 13;
		internal const int SSH_FXP_MKDIR = 14;
		internal const int SSH_FXP_RMDIR = 15;
		internal const int SSH_FXP_REALPATH = 16;
		internal const int SSH_FXP_STAT = 17;
		internal const int SSH_FXP_RENAME = 18;
		internal const int SSH_FXP_READLINK = 19;
		internal const int SSH_FXP_SYMLINK = 20;
		
		internal const int SSH_FXP_STATUS = 101;
		internal const int SSH_FXP_HANDLE = 102;
		internal const int SSH_FXP_DATA = 103;
		internal const int SSH_FXP_NAME = 104;
		internal const int SSH_FXP_ATTRS = 105;
		
		internal const int MAX_VERSION = 3;
		internal int version;
		internal System.Collections.Hashtable openhandles;
		internal uint requestId;
		internal System.Collections.Hashtable responses;
		internal System.Collections.ArrayList bufferPool;

        //
        private System.Collections.ArrayList filePathList = new System.Collections.ArrayList();
        private System.Collections.ArrayList topDirPathList = new System.Collections.ArrayList();
        private System.Collections.ArrayList topFilePathList = new System.Collections.ArrayList();

        /// <summary>
        /// フォルダーの下にファイル全体のリスト 
        /// SSH wangdan 20140415
        /// </summary>
        public System.Collections.ArrayList FilePathList
        {
            get
            {
                return filePathList;
            }
        }

        /// <summary>
        /// フォルダーの下にファイル全体のリスト 
        /// SSH wangdan 20140417
        /// </summary>
        public System.Collections.ArrayList TopDirPathList
        {
            get
            {
                return topDirPathList;
            }
        }

        /// <summary>
        /// フォルダーの下にファイル全体のリスト 
        /// SSH wangdan 20140417
        /// </summary>
        public System.Collections.ArrayList TopFilePathList
        {
            get
            {
                return topFilePathList;
            }
        }

		/// <summary>
		/// Constructs an uninitialized sftp channel with an unitialized session channel 
		/// </summary>
		/// <param name="session">A session on which the SFTP subsystem has already been request/started</param>
		public SFTPSubsystemChannel(SSHSession session) : base(session)
		{
			InitBlock();

            //清空队列
            rqueue.Queue.Clear();
            failPathQueue.Queue.Clear();
            brqueue.Queue.Clear();
        }

		/// <summary>
		/// When called after the <see cref="Maverick.SFTP.SFTPSubsystemChannel.Initialize"/> method this will 
		/// return the version in operation for this sftp session. 
		/// </summary>
		public int Version
		{
			get
			{
				return version;
			}
		}

		internal bool FireTransferEvent(FileTransferState state, String filename, long bytes, FileTransferProgress progress)
		{
			if(progress!=null)
				return progress(state, filename, bytes);
			else
				return true;
		}

		private void  InitBlock()
		{
			openhandles = new Hashtable();
			responses = new Hashtable();
			sync = new SFTPThreadSynchronizer(responses);
			bufferPool = new ArrayList();
			requestId = 0;
		}

		/// <summary>
		/// Gets the users default directory which is typicaly the users home directory.
		/// </summary>
		public String DefaultDirectory
		{
			get
			{
				return GetAbsolutePath("");
			}
			
		}

		/// <summary>
		/// Set the character encoding to use on filename fields.
		/// </summary>
		/// <param name="encoding"></param>
		public void SetEncoding(Encoding encoding)
		{
			this.CHARSET_ENCODING = encoding;
		}

		/// <summary>
		/// Gets the current character encoding in use on filename fields.
		/// </summary>
		/// <returns></returns>
		public Encoding GetEncoding()
		{
			return CHARSET_ENCODING;
		}

		/// <summary>
		/// Initializes the sftp subsystem and negotiates a version with the server. This method must 
		/// be the first method called after the channel has been opened. This implementation current 
		/// supports SFTP protocol version 3 and below. 
		/// </summary>
		public void Initialize()
		{
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Initializing SFTP channel");
#endif			
			// Initialize the SFTP subsystem
			try
			{
				
				ByteBuffer packet = CreateMessage();
				packet.WriteByte(SSH_FXP_INIT);
				packet.WriteUINT32(MAX_VERSION);
				
				SendMessage(packet);
				
				packet = ReadMessage();
				
				if (packet.ReadByte() != SSH_FXP_VERSION)
				{
					Close();
					throw new SSHException("Unexpected response from SFTP subsystem.", 
						SSHException.CHANNEL_FAILURE);
				}
				
				version = (int) packet.ReadUINT32();
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Using SFTP version " + version );
#endif
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(SSHException.INTERNAL_ERROR, ex);
			}
			
			
			// TODO: Extensions......
		}

		/// <summary>
		/// Change the permissions of a file.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="permissions">an integer value containing a base 10 representation of the file permissions mask</param>
		public void  ChangePermissions(SFTPFile file, int permissions)
		{
			SFTPFileAttributes attrs = new SFTPFileAttributes(); //file.getAttributes();
			attrs.Permissions = permissions;
			SetAttributes(file, attrs);
		}

		/// <summary>
		/// Change the permissions of a file.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="permissions">an integer value containing a base 10 representation of the file permissions mask</param>
		public void ChangePermissions(String filename, int permissions)
		{
			SFTPFileAttributes attrs = new SFTPFileAttributes();
			attrs.Permissions = permissions;
			SetAttributes(filename, attrs);
		}

		/// <summary>
		/// change the permissions of a file.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="permissions">A string representation of the permissions, i.e. "rw-r--r--"</param>
		public void ChangePermissions(String filename, String permissions)
		{
			SFTPFileAttributes attrs = new SFTPFileAttributes();
			attrs.PermissionsString = permissions;
			SetAttributes(filename, attrs);
		}

		/// <summary>
		/// Set the attributes for a file.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="attrs"></param>
		public void SetAttributes(String path, SFTPFileAttributes attrs)
		{
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Setting attributes of " + path + " to " + attrs.PermissionsString );
#endif
			try
			{
				uint requestId = NextRequestId();
								
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_SETSTAT);
				msg.WriteUINT32(requestId);
				msg.WriteString(path, CHARSET_ENCODING);
				msg.WriteBytes(attrs.ToByteArray());
				
				SendMessage(msg);
				
				GetOKRequestStatus(requestId);
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// Set the attributes for a file.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="attrs"></param>
		public void SetAttributes(SFTPFile file, SFTPFileAttributes attrs)
		{
			if (!IsValidHandle(file.Handle))
			{
				throw new SFTPStatusException(SFTPStatusException.INVALID_HANDLE, 
								"The handle is not an open file handle!");
			}
			
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Setting attributes of " + file.AbsolutePath + " to " + attrs.PermissionsString );
#endif
			try
			{
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_FSETSTAT);
				msg.WriteUINT32(requestId);
				msg.WriteBinaryString(file.Handle);
				msg.WriteBytes(attrs.ToByteArray());
				
				SendMessage(msg);
				
				GetOKRequestStatus(requestId);
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Send a write request for an open file but do not wait for the response from the server. 
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="position"></param>
		/// <param name="data"></param>
		/// <param name="off"></param>
		/// <param name="len"></param>
		/// <returns></returns>
		public uint PostWriteRequest(byte[] handle, long position, byte[] data, int off, int len)
		{
			
			if (!openhandles.Contains(new System.String(SupportClass.ToCharArray(handle))))
			{
				throw new SFTPStatusException(SFTPStatusException.INVALID_HANDLE,
					"The handle is not valid!");
			}
			
			if ((data.Length - off) < len)
			{
				throw new System.IndexOutOfRangeException("Incorrect data array size!");
			}
			
#if DEBUG
			String path = (String) openhandles[new String(SupportClass.ToCharArray(handle))];
			System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_WRITE for " + path );
#endif
			try
			{
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_WRITE);
				msg.WriteUINT32(requestId);
				msg.WriteBinaryString(handle);
				msg.WriteUINT64(position);
				msg.WriteBinaryString(data, off, len);
				
				SendMessage(msg);
				
				return requestId;
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Write a block of data to an open file. 
		/// </summary>
		/// <param name="handle">the open file handle.</param>
		/// <param name="offset">the offset in the file to start writing</param>
		/// <param name="data">a buffer containing the data to write</param>
		/// <param name="off">the offset to start in the buffer</param>
		/// <param name="len">the length of data to write</param>
		public void WriteFile(byte[] handle, long offset, byte[] data, int off, int len)
		{
			GetOKRequestStatus(PostWriteRequest(handle, offset, data, off, len));
		}

		/// <summary>
		/// Performs an optimized write of a file through asynchronous messaging and 
		/// through buffering the local file into memory. 
		/// </summary>
		/// <param name="handle">the open file handle to write to</param>
		/// <param name="blocksize">the block size to send data, should be between 4096 and 65535</param>
		/// <param name="outstandingRequests">the maximum number of requests that can be outstanding at any one time</param>
		/// <param name="input">the input Stream to read from</param>
		/// <param name="buffersize">the size of the temporary buffer to read from the input Stream. Data is buffered into a temporary
		/// buffer so that the number of local filesystem reads is reducted to a minimum. This increases performance and so the 
		/// buffer size should be as high as possible. The default operation, if buffersize is less than 0 is to allocate a buffer 
		/// the same size as the blocksize, meaning no buffer optimization is performed</param>
		/// <param name="progress">provides progress information, may be null.</param>
		public long PerformOptimizedWrite(byte[] handle, int blocksize, int outstandingRequests, System.IO.Stream input, int buffersize, FileTransferProgress progress)
		{
			return PerformOptimizedWrite(handle, blocksize, outstandingRequests, input, buffersize, progress, 0);
		}

		/// <summary>
		/// Performs an optimized write of a file through asynchronous messaging and 
		/// through buffering the local file into memory. 
		/// </summary>
		/// <param name="handle">the open file handle to write to</param>
		/// <param name="blocksize">the block size to send data, should be between 4096 and 65535</param>
		/// <param name="outstandingRequests">the maximum number of requests that can be outstanding at any one time</param>
		/// <param name="input">the input Stream to read from</param>
		/// <param name="buffersize">the size of the temporary buffer to read from the input Stream. Data is buffered into a temporary
		/// buffer so that the number of local filesystem reads is reducted to a minimum. This increases performance and so the 
		/// buffer size should be as high as possible. The default operation, if buffersize is less than 0 is to allocate a buffer 
		/// the same size as the blocksize, meaning no buffer optimization is performed</param>
		/// <param name="progress">provides progress information, may be null.</param>
		/// <param name="position">The position from which to start writing</param>
		/// <returns>The number of bytes transfered.</returns>
		public long PerformOptimizedWrite(byte[] handle, int blocksize, int outstandingRequests, System.IO.Stream input, int buffersize, FileTransferProgress progress, long position)
		{
			
			long transfered = position;

			try
			{
				// Check the handle is valid
				if (!openhandles.Contains(new System.String(SupportClass.ToCharArray(handle))))
				{
					throw new SFTPStatusException(SFTPStatusException.INVALID_HANDLE, 
								"The file handle is invalid!");
				}

				String remotePath = (String) openhandles[new System.String(SupportClass.ToCharArray(handle))];
				
				if (blocksize < 4096)
				{
					throw new SSHException("Block size cannot be less than 4096", SSHException.BAD_API_USAGE);
				}
				
				if (position < 0)
					throw new SSHException("Position value must be greater than zero!", 
						SSHException.BAD_API_USAGE);
				
				if (position > 0)
				{
					if(!FireTransferEvent(FileTransferState.TRANSFER_PROGRESSED, remotePath, position, progress))
						throw new TransferCancelledException();
				}
				
				if (buffersize <= 0)
				{
					buffersize = blocksize;
				}
				
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Performing optimized write on " + remotePath + " [blocksize=" + blocksize +", buffersize=" + buffersize + ", requests=" + outstandingRequests + "]");
#endif
				byte[] buf = new byte[blocksize];
				
				int buffered = 0;
				
				ArrayList requests = new ArrayList();

				while (true)
				{
					
					buffered = input.Read(buf, 0, buf.Length);

					if (buffered == 0)
						break;
					
					requests.Add(PostWriteRequest(handle, transfered, buf, 0, buffered));
                    
                    transfered += buffered;

                    if(requests.Count > 0 && requests.Count >= outstandingRequests)
                    {
                        GetOKRequestStatus((uint)requests[0]);
                        requests.RemoveAt(0);
                    }
                    
					if (progress != null)
					{
						if(!FireTransferEvent(FileTransferState.TRANSFER_PROGRESSED, remotePath, transfered, progress))
							throw new TransferCancelledException();
					}
				}

				for(IEnumerator e = requests.GetEnumerator();
					e.MoveNext();)
				{
					GetOKRequestStatus((uint) e.Current);
				}


			}
			catch (System.IO.EndOfStreamException)
			{
				// The channel has reached EOF before the transfer could complete so
				// make sure the channel is closed and throw a status exception
				try
				{
					Close();
				}
				catch (System.IO.IOException ex1)
				{
					throw new SSHException(ex1.Message, SSHException.CHANNEL_FAILURE);
				}
				
				throw new SFTPStatusException(SFTPStatusException.SSH_FX_CONNECTION_LOST, 
						"The SFTP channel terminated unexpectedly");
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
			catch (System.OutOfMemoryException)
			{
				throw new SSHException("Resource Shortage: try reducing the local file buffer size", 
					SSHException.BAD_API_USAGE);
			}

			return transfered;
		}

		/// <summary>
		/// Performs an optimized read of a file through use of asynchronous messages. The total number of outstanding 
		/// read requests is configurable. This should be safe on file objects as the SSH protocol states that file 
		/// read operations should return the exact number of bytes requested in each request. However the server 
		/// is not required to return the exact number of bytes on device files and so this method should not be used 
		/// for device files.
		/// </summary>
		/// <param name="handle">the open files handle</param>
		/// <param name="length">the length of the file</param>
		/// <param name="blocksize">the blocksize to read</param>
		/// <param name="output">an output Stream to output the file into</param>
		/// <param name="outstandingRequests">the maximum number of read requests to leave outstanding</param>
		/// <param name="progress">provides progress information, may be null.</param>
		/// <returns>The number of bytes transfered.</returns>
		public long PerformOptimizedRead(byte[] handle, long length, int blocksize, System.IO.Stream output, int outstandingRequests, FileTransferProgress progress)
		{
			return PerformOptimizedRead(handle, length, blocksize, output, outstandingRequests, progress, 0);
		}

		/// <summary>
		/// Performs an optimized read of a file through use of asynchronous messages. The total number of outstanding 
		/// read requests is configurable. This should be safe on file objects as the SSH protocol states that file 
		/// read operations should return the exact number of bytes requested in each request. However the server 
		/// is not required to return the exact number of bytes on device files and so this method should not be used 
		/// for device files.
		/// </summary>
		/// <param name="handle">the open files handle</param>
		/// <param name="length">the length of the file</param>
		/// <param name="blocksize">the blocksize to read</param>
		/// <param name="output">an output Stream to output the file into</param>
		/// <param name="outstandingRequests">the maximum number of read requests to leave outstanding</param>
		/// <param name="progress">provides progress information, may be null.</param>
		/// <param name="position">the position from which to start reading the remote file.</param>
		public long PerformOptimizedRead(byte[] handle, long length, int blocksize, System.IO.Stream output, int outstandingRequests, FileTransferProgress progress, long position)
		{
			
			long transfered = position;

			try
			{
				// Check the handle is valid
				if (!openhandles.Contains(new System.String(SupportClass.ToCharArray(handle))))
				{
					throw new SFTPStatusException(SFTPStatusException.INVALID_HANDLE, 
							"The file handle is invalid!");
				}

				String remotePath = (String) openhandles[new System.String(SupportClass.ToCharArray(handle))];
				
				if (blocksize < 1 || blocksize > 32768)
				{
					blocksize = 32768;
				}
				
				if (position < 0)
					throw new SSHException("Position value must be zero or greater!", 
							SSHException.BAD_API_USAGE);
				
				if(position > 0) 
				{
					if(!FireTransferEvent(FileTransferState.TRANSFER_PROGRESSED, remotePath, position, progress))
						throw new TransferCancelledException();
				}
				
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Performing optimized read on " + remotePath + " [blocksize=" + blocksize + ", requests=" + outstandingRequests + "]");
#endif				
				System.Collections.ArrayList requests = new System.Collections.ArrayList(outstandingRequests);
				uint offset = (uint)position;
				
				if (length / blocksize < outstandingRequests)
				{
					outstandingRequests = (int) (length / blocksize) + 1;
				}
				int expected = (int) (length / blocksize) + 2;
				int completed = 0;
				
				// Fire an initial round of requests
				for (int i = 0; i < outstandingRequests; i++)
				{
					requests.Add(PostReadRequest(handle, offset, blocksize));
					offset += (uint)blocksize;
				
				}
				
				byte[] tmp;
				uint requestId;
				
				while (true)
				{
					requestId = (uint)requests[0];
					requests.RemoveAt(0);
					ByteBuffer bar = new ByteBuffer(GetResponse(requestId));
					int type = bar.ReadByte();
					if (type == SSH_FXP_DATA)
					{
						bar.ReadUINT32();
						tmp = bar.ReadBinaryString();
						output.Write(tmp, 0, tmp.Length);
						completed++;

						if(!FireTransferEvent(FileTransferState.TRANSFER_PROGRESSED, remotePath, transfered += tmp.Length, progress))
							throw new TransferCancelledException();
					}
					else if (type == SSH_FXP_STATUS)
					{
						int id = (int) bar.ReadUINT32();
						int status = (int) bar.ReadUINT32();
						if (status == SFTPStatusException.SSH_FX_EOF)
							break;
						if (version >= 3)
						{
							String desc = bar.ReadString().Trim();
							throw new SFTPStatusException(status, desc);
						}
						else
							throw new SFTPStatusException(status);
					}
					else
					{
						Close();
						throw new SSHException("The server responded with an unexpected message", 
							SSHException.CHANNEL_FAILURE);
					}

					if (requests.Count == 0 || completed + requests.Count < expected)
					{
						requests.Add(PostReadRequest(handle, offset, blocksize));
						offset += (uint)blocksize;
					}
				}
			}
			catch (System.IO.EndOfStreamException)
			{
				// The channel has reached EOF before the transfer could complete so
				// make sure the channel is closed and throw a status exception
				try
				{
					Close();
				}
				catch (System.IO.IOException ex1)
				{
					throw new SSHException(ex1.Message, SSHException.CHANNEL_FAILURE);
				}
				
				throw new SFTPStatusException(SFTPStatusException.SSH_FX_CONNECTION_LOST, 
							"The SFTP channel terminated unexpectedly");
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}

			return transfered;
		}

		internal uint PostReadRequest(byte[] handle, ulong offset, int len)
		{
			try
			{
				if (!openhandles.Contains(new System.String(SupportClass.ToCharArray(handle))))
				{
					throw new SFTPStatusException(SFTPStatusException.INVALID_HANDLE, 
							"The file handle is invalid!");
				}
				
#if DEBUG
				String path = (String) openhandles[new String(SupportClass.ToCharArray(handle))];
				System.Diagnostics.Trace.WriteLine("Sending asynchronous SSH_FXP_READ for " + path );
#endif
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_READ);
				msg.WriteUINT32(requestId);
				msg.WriteBinaryString(handle);
				msg.WriteUINT64(offset);
				msg.WriteUINT32(len);
				
				SendMessage(msg);
				
				return requestId;
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Read a block of data from an open file
		/// </summary>
		/// <param name="handle">the open file handle</param>
		/// <param name="offset">the offset to start reading in the file</param>
		/// <param name="output">a buffer to write the returned data to</param>
		/// <param name="off">the starting offset in the output buffer</param>
		/// <param name="len">the length of data to read </param>
		/// <returns></returns>
		public int ReadFile(byte[] handle, ulong offset, byte[] output, int off, int len)
		{
			
			try
			{
				if (!openhandles.Contains(new System.String(SupportClass.ToCharArray(handle))))
				{
					throw new SFTPStatusException(SFTPStatusException.INVALID_HANDLE, 
						"The file handle is invalid!");
				}
				
				if ((output.Length - off) < len)
				{
					throw new System.IndexOutOfRangeException("Output array size is smaller than read length!");
				}
				
#if DEBUG
				String path = (String) openhandles[new String(SupportClass.ToCharArray(handle))];
				System.Diagnostics.Trace.WriteLine("Sending synchronous SSH_FXP_READ for " + path );
#endif
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_READ);
				msg.WriteUINT32(requestId);
				msg.WriteBinaryString(handle);
				msg.WriteUINT64(offset);
				msg.WriteUINT32(len);
				
				SendMessage(msg);
				
				ByteBuffer bar = GetResponse(requestId);
				int type = bar.ReadByte();
				
				if (type == SSH_FXP_DATA)
				{
					bar.ReadUINT32();
					byte[] msgdata = bar.ReadBinaryString();
					Array.Copy(msgdata, 0, output, off, msgdata.Length);
					return msgdata.Length;
				}
				else if (type == SSH_FXP_STATUS)
				{
					int id = (int) bar.ReadUINT32();
					int status = (int) bar.ReadUINT32();
					if (status == SFTPStatusException.SSH_FX_EOF)
						return - 1;
					if (version >= 3)
					{
						System.String desc = bar.ReadString().Trim();
						throw new SFTPStatusException(status, desc);
					}
					else
						throw new SFTPStatusException(status);
				}
				else
				{
					Close();
					throw new SSHException("The server responded with an unexpected message",
						SSHException.CHANNEL_FAILURE);
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Utility method to obtain an <see cref="Maverick.SFTP.SFTPFile"/> instance for a given path. 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public SFTPFile GetFile(String path)
		{
			String absolute = GetAbsolutePath(path);
			SFTPFile file = new SFTPFile(absolute, GetAttributes(absolute));
			file.sftp = this;
			return file;
		}

		/// <summary>
		/// Get the absolute path of a file. 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public String GetAbsolutePath(SFTPFile file)
		{
			return GetAbsolutePath(file.Filename);
		}

		/// <summary>
		/// Create a symbolic link. 
		/// </summary>
		/// <param name="targetpath">the path to link</param>
		/// <param name="linkpath">the new links path</param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException">
		/// if the remote SFTP version is less than 3 an exception is thrown as this feature is not 
		/// supported by previous versions of the protocol. 
		/// </exception>
		public void CreateSymbolicLink(String targetpath, String linkpath)
		{
			
			if (version < 3)
			{
				throw new SFTPStatusException(SFTPStatusException.SSH_FX_OP_UNSUPPORTED, 
					"Symbolic links are not supported by the server SFTP version " + version.ToString());
			}
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_SYMLINK for target " + targetpath + " and link " + linkpath );
#endif
			try
			{
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_SYMLINK);
				msg.WriteUINT32(requestId);
				msg.WriteString(linkpath, CHARSET_ENCODING);
				msg.WriteString(targetpath, CHARSET_ENCODING);
				
				SendMessage(msg);
				
				GetOKRequestStatus(requestId);
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Get the target path of a symbolic link. 
		/// </summary>
		/// <param name="linkpath"></param>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException">
		/// if the remote SFTP version is less than 3 an exception is thrown as this feature is not 
		/// supported by previous versions of the protocol. 
		/// </exception>
		public String GetSymbolicLinkTarget(String linkpath)
		{
			
			if (version < 3)
			{
				throw new SFTPStatusException(SFTPStatusException.SSH_FX_OP_UNSUPPORTED, 
					"Symbolic links are not supported by the server SFTP version " + version.ToString());
			}
			
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_READLINK for link " + linkpath );
#endif
			try
			{
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte((System.Byte) SSH_FXP_READLINK);
				msg.WriteUINT32(requestId);
				msg.WriteString(linkpath, CHARSET_ENCODING);
				
				SendMessage(msg);
				
				ByteBuffer bar = GetResponse(requestId);
				int type = bar.ReadByte();
				if (type == SSH_FXP_NAME)
				{
					SFTPFile[] files = ExtractFiles(bar, null);
					return files[0].AbsolutePath;
				}
				else if (type == SSH_FXP_STATUS)
				{
					int id = (int) bar.ReadUINT32();
					int status = (int) bar.ReadUINT32();
					if (version >= 3)
					{
						String desc = bar.ReadString().Trim();
						throw new SFTPStatusException(status, desc);
					}
					else
					{
						throw new SFTPStatusException(status);
					}
				}
				else
				{
					Close();
					throw new SSHException("The server responded with an unexpected message", 
						SSHException.CHANNEL_FAILURE);
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Get the absolute path of a file.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		/// <remarks>
		/// The base folder for relative paths will always be the users default directory. Additionally
		/// this method will also cannonicalize paths that contain ".." and ".".
		/// </remarks>
		public String GetAbsolutePath(String path)
		{

#if DEBUG
			System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_REALPATH for path " + path );
#endif
			try
			{
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_REALPATH);
				msg.WriteUINT32(requestId);
				msg.WriteString(path, CHARSET_ENCODING);
				SendMessage(msg);
				
				ByteBuffer bar = GetResponse(requestId);
				int type = bar.ReadByte();
				if (type == SSH_FXP_NAME)
				{
					SFTPFile[] files = ExtractFiles(bar, null);
					
					if (files.Length != 1)
					{
						Close();
						throw new SSHException("Server responded to SSH_FXP_REALPATH with too many files!", 
							SSHException.CHANNEL_FAILURE);
					}
					
					return files[0].AbsolutePath;
				}
				else if (type == SSH_FXP_STATUS)
				{
					int id = (int) bar.ReadUINT32();
					int status = (int) bar.ReadUINT32();
					if (version >= 3)
					{
						String desc = bar.ReadString().Trim();
						throw new SFTPStatusException(status, desc);
					}
					else
					{
						throw new SFTPStatusException(status);
					}
				}
				else
				{
					Close();
					throw new SSHException("The server responded with an unexpected message", 
						SSHException.CHANNEL_FAILURE);
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// List the children of a directory.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="children"></param>
		/// <returns></returns>
		/// <remarks>
		/// To use this method first open a directory with the <see cref="Maverick.SFTP.SFTPSubsystemChannel.OpenDirectory"/>
		/// method and then create an ArrayList to store the results. To retreive the results keep calling this method 
		/// until it returns -1 which indicates no more results will be returned. 
		/// <example>
		/// <code>
		/// SFTPFile dir = sftp.OpenDirectory("code/foobar");
		/// ArrayList results = new ArrayList();
		/// while(sftp.ListChildren(dir, results) > -1)
		/// {
		/// }
		/// sftp.CloseFile(dir);
		/// </code>
		/// </example>
		/// </remarks>
		public void ListChildren(SFTPFile file)
		{
			if (file.IsDirectory)
			{
                //if (!IsValidHandle(file.Handle))
                //{
                //    file = OpenDirectory(file.AbsolutePath);
                //    if (!IsValidHandle(file.Handle))
                //    {
                //        throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE,
                //            "Failed to open directory");
                //    }
                //}
                file = OpenDirectory(file.AbsolutePath);
			}
			else
			{
				throw new SSHException("Cannot list children for this file object", 
					SSHException.BAD_API_USAGE);
			}
			
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_READDIR for path " + file.AbsolutePath );
#endif
			try
			{
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_READDIR);
				msg.WriteUINT32(requestId);
                if (file.Handle != null)
                {
                    msg.WriteBinaryString(file.Handle);
                }
				//msg.WriteBinaryString(file.Handle);
				
				SendMessage(msg); ;
				
				ByteBuffer bar = GetResponse(requestId);
				int type = bar.ReadByte();
				if (type == SSH_FXP_NAME)
				{
					SFTPFile[] files = ExtractFiles(bar, file.AbsolutePath);
					
					for (int i = 0; i < files.Length; i++)
					{
                        // SSH wangdan 20140415
                        if (files[i].IsDirectory)
                        {
                            if (!files[i].Filename.Equals(".") && !files[i].Filename.Equals(".."))
                            {
                                ListChildren(files[i]);
                            }
                        }
                        else if (files[i].IsFile)
                        {
                            if (!filePathList.Contains(files[i]))
                            {
                                filePathList.Add(files[i]);
                            }
                        }
					}
					//return files.Length;
				}
				else if (type == SSH_FXP_STATUS)
				{

                    int id = (int)bar.ReadUINT32();
                    int status = (int)bar.ReadUINT32();

                    if (status == SFTPStatusException.SSH_FX_EOF)
                    {
                        //return - 1;
                    }

                    if (version >= 3)
                    {
                        String desc = bar.ReadString().Trim();
                       throw new SFTPStatusException(status, desc);
                        ///throw new Exception(file.AbsolutePath);
                    }
                    else
                    {
                        throw new SFTPStatusException(status);
                        //throw new Exception(file.AbsolutePath);
                    }
                    #region
                    // 20140707 wangdan
                    //SFTPFile[] files = ExtractFilesForFXPSTATUS(bar, file.AbsolutePath);

                    //for (int i = 0; i < files.Length; i++)
                    //{
                    //    // SSH wangdan 20140415
                    //    if (files[i].IsDirectory)
                    //    {
                    //        if (!files[i].Filename.Equals(".") && !files[i].Filename.Equals(".."))
                    //        {
                    //            ListChildren(files[i]);
                    //        }
                    //    }
                    //    else if (files[i].IsFile)
                    //    {
                    //        if (!filePathList.Contains(files[i]))
                    //        {
                    //            filePathList.Add(files[i]);
                    //        }
                    //    }
                    //}
                    #endregion
                }
				else
				{
					Close();
					throw new SSHException("The server responded with an unexpected message", 
						SSHException.CHANNEL_FAILURE);
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

        #region xiecongwen use queue to liste file
        public static ReactiveQueue<SFTPFileArgs> rqueue = new ReactiveQueue<SFTPFileArgs>();
        /// <summary>
        /// use queue to liste file xiecongwen 20140705
        /// </summary>
        /// <param name="file"></param>
        public void ListChildrenByQueue(SFTPFile file)
        {
            try
            {
                if (file.IsDirectory)
                {
                    //if (!IsValidHandle(file.Handle))
                    //{
                    //    file = OpenDirectory(file.AbsolutePath);
                    //    if (!IsValidHandle(file.Handle))
                    //    {
                    //        throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE,
                    //            "Failed to open directory");
                    //    }
                    //}
                    file = OpenDirectory(file.AbsolutePath);
                }
                else
                {
                    throw new SSHException("Cannot list children for this file object",
                        SSHException.BAD_API_USAGE);
                }

#if DEBUG
                System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_READDIR for path " + file.AbsolutePath);
#endif
                try
                {
                    #region
                    uint requestId = NextRequestId();
                    ByteBuffer msg = CreateMessage();
                    msg.WriteByte(SSH_FXP_READDIR);
                    msg.WriteUINT32(requestId);
                    if (file.Handle != null)
                    {
                        msg.WriteBinaryString(file.Handle);
                    }
                    //msg.WriteBinaryString(file.Handle);

                    SendMessage(msg); ;

                    List<SFTPFile> dirslist = new List<SFTPFile>();
                   
                    GetResponseAndEnqueue(requestId, (list) =>
                    {
                        foreach (ByteBuffer bar in list)
                        {
                            int type = bar.ReadByte();
                            if (type == SSH_FXP_NAME)
                            {
                                #region
                                dirslist.AddRange(ExtractFilesAndEnqueue(bar, file.AbsolutePath));

                                //return files.Length;
                                #endregion
                            }
                            else if (type == SSH_FXP_STATUS)
                            {
                                #region

                                dirslist.AddRange(ExtractFilesForFXPSTATUSAndEnqueue(bar, file.AbsolutePath));


                                #endregion
                            }
                            else
                            {
                                Close();
                                //throw new SSHException("The server responded with an unexpected message",
                                //    SSHException.CHANNEL_FAILURE);
                            }
                        }
                    });

                    dirslist.ForEach((dir) => ListChildrenByQueue(dir));
                    #endregion
                }
                catch (System.IO.IOException ex)
                {
                    throw new SSHException(ex);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //20140715 xiecongwen
        internal List<SFTPFile> ExtractFilesAndEnqueue(ByteBuffer bar, System.String parent)
        {
            List<SFTPFile> dirsList = new List<SFTPFile>();
            try
            {
                #region
                bar.ReadUINT32(); // Ignore the request id

                if (parent != null && !parent.EndsWith("/"))
                {
                    parent += "/";
                }
                int count = (int)bar.ReadUINT32();

               
                SFTPFile file = null;
                SFTPFileArgs fargs = null;

                String shortname;
                String longname;

                bool isOK = true;
                while (count-- > 0 && isOK)
                {
                    try
                    {
                        shortname = bar.ReadString(CHARSET_ENCODING);
                        longname = bar.ReadString(CHARSET_ENCODING);
                        file = new SFTPFile(parent != null ? parent + shortname : shortname, new SFTPFileAttributes(bar));
                        file.SFTPSubsystem = this;

                        fargs = new SFTPFileArgs() { SftpFile = file };
                       
                    }
                    catch (Exception ex)
                    {
                        isOK = false;

                        file = null;
                        fargs = new SFTPFileArgs() { SftpFile = null, FailPath = parent };
                        Common.LogManager.WriteLog(Common.LogFile.WARNING, ex.Message);
                    }
                    #region enqueue
                    if (file != null && file.IsDirectory)
                    {
                        if (!file.Filename.Equals(".") && !file.Filename.Equals(".."))
                        {
                            dirsList.Add(file);
                               
                        }
                    }
                    else if (file == null || file.IsFile)
                    {
                    again: bool isOk = rqueue.Enqueue(fargs);
                        if (!isOk)
                        {
                            System.Threading.Thread.Sleep(2000);//wait
                            goto again;
                        }
                    }
                    #endregion

                }
                #endregion

            }
            catch (System.IO.IOException ex)
            {
                throw new SSHException(ex);
            }

            return dirsList;
        }

        // 20140715 xiecongwen
        internal List<SFTPFile> ExtractFilesForFXPSTATUSAndEnqueue(ByteBuffer bar, System.String parent)
        {
            List<SFTPFile> dirsList = new List<SFTPFile>();
            try
            {
                #region
                bar.MoveToPosition(0);
                bar.ReadUINT32(); // Ignore the request id

                if (parent != null && !parent.EndsWith("/"))
                {
                    parent += "/";
                }
                int count = (int)bar.ReadUINT32();
                SFTPFile file = null;
                SFTPFileArgs fargs = null;
                String shortname;
                String longname;
                bool isOK = true;
               while(count-->0 && isOK)
                {
                    try
                    {
                        bar.ReadString();
                        shortname = bar.ReadString();
                        bar.ReadBinaryString();
                        longname = bar.ReadString();
                        file = new SFTPFile(parent + shortname, new SFTPFileAttributes(bar));
                        file.SFTPSubsystem = this;
                        fargs = new SFTPFileArgs() {  SftpFile=file};
                    }
                    catch (Exception ex)
                    {
                        isOK = false;

                        file = null;
                        fargs = new SFTPFileArgs() { SftpFile = null, FailPath=parent };
                        Common.LogManager.WriteLog(Common.LogFile.WARNING, ex.Message);
                    }
                    #region enqueue
                    if (file!=null && file.IsDirectory)
                    {
                        if (!file.Filename.Equals(".") && !file.Filename.Equals(".."))
                        {
                            dirsList.Add(file);
                        }
                    }
                    else if (file==null || file.IsFile)
                    {
                    again: bool isOk = rqueue.Enqueue(fargs);
                        if (!isOk)
                        {
                            System.Threading.Thread.Sleep(2000);//wait
                            goto again;
                        }
                    }
                    #endregion
                }
                #endregion
            }
            catch (System.IO.IOException ex)
            {
                throw new SSHException(ex);
            }
            catch (Exception)
            {
                throw;
            }

            return dirsList;
        }

        public static ReactiveQueue<string> failPathQueue = new ReactiveQueue<string>();
        public void ListChildrenPath(SFTPFile file)
        {
            if (file.IsDirectory)
            {
                //if (!IsValidHandle(file.Handle))
                //{
                //    file = OpenDirectory(file.AbsolutePath);
                //    if (!IsValidHandle(file.Handle))
                //    {
                //        throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE,
                //            "Failed to open directory");
                //    }
                //}
                file = OpenDirectory(file.AbsolutePath);
            }
            else
            {
                throw new SSHException("Cannot list children for this file object",
                    SSHException.BAD_API_USAGE);
            }

#if DEBUG
            System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_READDIR for path " + file.AbsolutePath);
#endif
            try
            {
                uint requestId = NextRequestId();
                ByteBuffer msg = CreateMessage();
                msg.WriteByte(SSH_FXP_READDIR);
                msg.WriteUINT32(requestId);
                if (file.Handle != null)
                {
                    msg.WriteBinaryString(file.Handle);
                }
                //msg.WriteBinaryString(file.Handle);

                SendMessage(msg); ;

                ByteBuffer bar = GetResponse(requestId);
                int type = bar.ReadByte();
                if (type == SSH_FXP_NAME)
                {
                    SFTPFile[] files = ExtractFiles(bar, file.AbsolutePath);

                    for (int i = 0; i < files.Length; i++)
                    {
                        // SSH wangdan 20140415
                        if (files[i].IsDirectory)
                        {
                            if (!files[i].Filename.Equals(".") && !files[i].Filename.Equals(".."))
                            {
                                ListChildrenPath(files[i]);
                            }
                        }
                        else if (files[i].IsFile)
                        {
                            if (!filePathList.Contains(files[i]))
                            {
                                filePathList.Add(files[i]);
                            }
                        }
                    }
                    //return files.Length;
                }
                else if (type == SSH_FXP_STATUS)
                {
                again: bool isOK = failPathQueue.Enqueue(file.AbsolutePath);
                    if (!isOK)
                    {
                        System.Threading.Thread.Sleep(2000);
                        goto again;
                    }
                    //int id = (int)bar.ReadUINT32();
                    //int status = (int)bar.ReadUINT32();

                    //if (status == SFTPStatusException.SSH_FX_EOF)
                    //{
                    //    //return - 1;
                    //}

                    //if (version >= 3)
                    //{
                    //    //String desc = bar.ReadString().Trim();
                    //    //throw new SFTPStatusException(status, desc);
                    //    throw new Exception(file.AbsolutePath);
                    //}
                    //else
                    //{
                    //    //throw new SFTPStatusException(status);
                    //    throw new Exception(file.AbsolutePath);
                    //}
                    #region
                    // 20140707 wangdan
                    //SFTPFile[] files = ExtractFilesForFXPSTATUS(bar, file.AbsolutePath);

                    //for (int i = 0; i < files.Length; i++)
                    //{
                    //    // SSH wangdan 20140415
                    //    if (files[i].IsDirectory)
                    //    {
                    //        if (!files[i].Filename.Equals(".") && !files[i].Filename.Equals(".."))
                    //        {
                    //            ListChildren(files[i]);
                    //        }
                    //    }
                    //    else if (files[i].IsFile)
                    //    {
                    //        if (!filePathList.Contains(files[i]))
                    //        {
                    //            filePathList.Add(files[i]);
                    //        }
                    //    }
                    //}
                    #endregion
                }
                else
                {
                    Close();
                    throw new SSHException("The server responded with an unexpected message",
                        SSHException.CHANNEL_FAILURE);
                }
            }
            catch (System.IO.IOException ex)
            {
                throw new SSHException(ex);
            }
        }

       
        #endregion
        /// <summary>
        /// List the children of a directory.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        /// <remarks>
        /// To use this method first open a directory with the <see cref="Maverick.SFTP.SFTPSubsystemChannel.OpenDirectory"/>
        /// method and then create an ArrayList to store the results. To retreive the results keep calling this method 
        /// until it returns -1 which indicates no more results will be returned. 
        /// <example>
        /// <code>
        /// SFTPFile dir = sftp.OpenDirectory("code/foobar");
        /// ArrayList results = new ArrayList();
        /// while(sftp.ListChildren(dir, results) > -1)
        /// {
        /// }
        /// sftp.CloseFile(dir);
        /// </code>
        /// </example>
        /// </remarks>
        public void TopListChildren(SFTPFile file)
        {
            //Clear previous  xiecongwen 20140717
            topDirPathList.Clear();
            topFilePathList.Clear();
            if (file.IsDirectory)
            {
                //if (!IsValidHandle(file.Handle))
                //{
                //    file = OpenDirectory(file.AbsolutePath);
                //    if (!IsValidHandle(file.Handle))
                //    {
                //        throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE,
                //            "Failed to open directory");
                //    }
                //}
                file = OpenDirectory(file.AbsolutePath);
            }
            else
            {
                throw new SSHException("Cannot list children for this file object",
                    SSHException.BAD_API_USAGE);
            }

#if DEBUG
            System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_READDIR for path " + file.AbsolutePath);
#endif
            try
            {
                uint requestId = NextRequestId();
                ByteBuffer msg = CreateMessage();
                msg.WriteByte(SSH_FXP_READDIR);
                msg.WriteUINT32(requestId);
                msg.WriteBinaryString(file.Handle);

                SendMessage(msg); ;

                ByteBuffer bar = GetResponse(requestId);
                int type = bar.ReadByte();
                if (type == SSH_FXP_NAME)
                {
                    SFTPFile[] files = ExtractFiles(bar, file.AbsolutePath);

                    for (int i = 0; i < files.Length; i++)
                    {
                        // SSH wangdan 20140415
                        if (files[i].IsDirectory)
                        {
                            if (!files[i].Filename.Equals(".") && !files[i].Filename.Equals(".."))
                            {
                                if (!topDirPathList.Contains(files[i]))
                                {
                                    topDirPathList.Add(files[i]);
                                }
                            }
                        }
                        else if (files[i].IsFile) 
                        {
                            if (!topFilePathList.Contains(files[i]))
                            {
                                topFilePathList.Add(files[i]);
                            }
                        }
                    }
                    //return files.Length;
                }
                else if (type == SSH_FXP_STATUS)
                {
                    int id = (int)bar.ReadUINT32();
                    int status = (int)bar.ReadUINT32();

                    if (status == SFTPStatusException.SSH_FX_EOF)
                    {
                        //return - 1;
                    }

                    if (version >= 3)
                    {
                        String desc = bar.ReadString().Trim();
                        throw new SFTPStatusException(status, desc);
                    }
                    else
                    {
                        throw new SFTPStatusException(status);
                    }
                }
                else
                {
                    Close();
                    throw new SSHException("The server responded with an unexpected message",
                        SSHException.CHANNEL_FAILURE);
                }
            }
            catch (System.IO.IOException ex)
            {
                throw new SSHException(ex);
            }
        }

		internal SFTPFile[] ExtractFiles(ByteBuffer bar, System.String parent)
		{
			
			try
			{
				bar.ReadUINT32(); // Ignore the request id
				
				if (parent != null && !parent.EndsWith("/"))
				{
					parent += "/";
				}
				int count = (int) bar.ReadUINT32();
				SFTPFile[] files = new SFTPFile[count];
				
				String shortname;
				String longname;
				
				for (int i = 0; i < files.Length; i++)
				{
					shortname = bar.ReadString(CHARSET_ENCODING);
					longname = bar.ReadString(CHARSET_ENCODING);
					files[i] = new SFTPFile(parent != null?parent + shortname:shortname, new SFTPFileAttributes(bar));
					files[i].SFTPSubsystem = this;
				}
				
				return files;
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

        // 20140707 wangdan
        internal SFTPFile[] ExtractFilesForFXPSTATUS(ByteBuffer bar, System.String parent)
        {

            try
            {
                bar.ReadUINT32(); // Ignore the request id

                if (parent != null && !parent.EndsWith("/"))
                {
                    parent += "/";
                }
                int count = (int)bar.ReadUINT32();
                SFTPFile[] files = new SFTPFile[count];

                String shortname;
                String longname;

                for (int i = 0; i < files.Length; i++)
                {
                    //String msg = bar.ReadString().Trim();
                    ////throw new SFTPStatusException(status, msg);
                    //byte[] handle = bar.ReadBinaryString();
                    //string w1 = bar.ReadString().Trim();
                    //string w2 = bar.ReadString().Trim();
                    //shortname = bar.ReadBinaryString().ToString();
                    //longname = bar.ReadBinaryString().ToString();
                    //shortname = bar.ReadString(CHARSET_ENCODING);
                    //longname = bar.ReadString(CHARSET_ENCODING);
                    //files[i] = new SFTPFile(parent != null ? parent + shortname : shortname, new SFTPFileAttributes(bar));
                    bar.ReadString();
                    shortname = bar.ReadString();
                    bar.ReadBinaryString();
                    longname = bar.ReadString();
                    files[i] = new SFTPFile(parent + shortname, new SFTPFileAttributes(bar));
                    files[i].SFTPSubsystem = this;
                }

                return files;
            }
            catch (System.IO.IOException ex)
            {
                throw new SSHException(ex);
            }
            catch (Exception)
            {
                //向上层传递错误路径
                throw new Exception(parent);
            }
        }

		/// <summary>
		/// Recurse through a hierarchy of directories creating them as necersary. 
		/// </summary>
		/// <param name="path"></param>
		public void RecurseMakeDirectory(String path)
		{
			SFTPFile file;
			
			if (path.Trim().Length > 0)
			{
				try
				{
					file = OpenDirectory(path);
					file.Close();
				}
				catch(SFTPStatusException)
				{
					
					int idx = -1;
					
					do 
					{
						idx = path.IndexOf((System.Char) '/', idx+1);
						System.String tmp = (idx > - 1?path.Substring(0, (idx + 1) - (0)):path);
						try
						{
							file = OpenDirectory(tmp);
							file.Close();
						}
						catch(SFTPStatusException)
						{
							MakeDirectory(tmp);
						}
					}
					while (idx > - 1);
				}
			}
		}

		/// <summary>
		/// Open a file. 
		/// </summary>
		/// <param name="absolutePath"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public SFTPFile OpenFile(String absolutePath, int flags)
		{
			return OpenFile(absolutePath, flags, new SFTPFileAttributes());
		}

		/// <summary>
		/// Open a file. 
		/// </summary>
		/// <param name="absolutePath"></param>
		/// <param name="flags"></param>
		/// <param name="attrs"></param>
		/// <returns></returns>
		public SFTPFile OpenFile(String absolutePath, int flags, SFTPFileAttributes attrs)
		{
			if (attrs == null)
			{
				attrs = new SFTPFileAttributes();
			}
			
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_OPEN for path " + absolutePath );
#endif
			try
			{
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_OPEN);
				msg.WriteUINT32(requestId);
				msg.WriteString(absolutePath, CHARSET_ENCODING);
				msg.WriteUINT32(flags);
				msg.WriteBytes(attrs.ToByteArray());
				
				SendMessage(msg);
				
				byte[] handle = GetHandleResponse(requestId);
				
				openhandles.Add(new System.String(SupportClass.ToCharArray(handle)), absolutePath);
				
				SFTPFile file = new SFTPFile(absolutePath, null);
				file.Handle = handle;
				file.SFTPSubsystem = this;
				
				return file;
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Open a directory. 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public SFTPFile OpenDirectory(String path)
		{
            try
            {
                String absolutePath = GetAbsolutePath(path);

                //if ("/Volumes/DATA01/-Job3E-G1_校了済み/-VOLDATAへ移動可/3E_20140616-1/U024460-リソルプレス2014春-B/P18-19_RandS提携-女子旅".Equals(absolutePath))
                //{
                //    System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_OPENDIR for path " + absolutePath);
                //}

                SFTPFileAttributes attrs = GetAttributes(absolutePath);

                if (!attrs.IsDirectory)
                {
                    //throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE, 
                    //    path + " is not a directory");
                    System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_OPENDIR for path " + absolutePath);
                }

#if DEBUG
                //System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_OPENDIR for path " + absolutePath );
#endif
                try
                {
                    #region
                    uint requestId = NextRequestId();
                    ByteBuffer msg = CreateMessage();
                    msg.WriteByte(SSH_FXP_OPENDIR);
                    msg.WriteUINT32(requestId);
                    msg.WriteString(path, CHARSET_ENCODING);
                    SendMessage(msg);

                    byte[] handle = GetHandleResponse(requestId);

                    if (handle != null)
                    {
                        string key = new System.String(SupportClass.ToCharArray(handle));
                        if(!openhandles.ContainsKey(key))
                             openhandles.Add(key, absolutePath);
                    }
                    //openhandles.Add(new System.String(SupportClass.ToCharArray(handle)), absolutePath);

                    SFTPFile file = new SFTPFile(absolutePath, attrs);
                    if (handle != null)
                    {
                        file.Handle = handle;
                    }
                    //file.Handle = handle;
                    file.SFTPSubsystem = this;
                    #endregion
                    return file;
                }
                catch (System.IO.IOException ex)
                {
                    throw new SSHException(ex);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
		}

		internal void CloseHandle(byte[] handle)
		{
			if (!IsValidHandle(handle))
			{
				throw new SFTPStatusException(SFTPStatusException.INVALID_HANDLE, 
						"The handle is invalid!");
			}
			
#if DEBUG
			String path = (String) openhandles[new String(SupportClass.ToCharArray(handle))];
			System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_CLOSE for path " + path );
#endif
			try
			{
				// We will remove the handle first so that even if an excpetion occurs
				// the file as far as were concerned is closed
				SupportClass.HashtableRemove(openhandles, new System.String(SupportClass.ToCharArray(handle)));
				
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_CLOSE);
				msg.WriteUINT32(requestId);
				msg.WriteBinaryString(handle);
				
				SendMessage(msg);
				
				GetOKRequestStatus(requestId);
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Close a file or directory. 
		/// </summary>
		/// <param name="file"></param>
		public void CloseFile(SFTPFile file)
		{
			CloseHandle(file.Handle);
			file.Handle = null;
		}
		
		internal bool IsValidHandle(byte[] handle)
		{
			return openhandles.Contains(new System.String(SupportClass.ToCharArray(handle)));
		}

		/// <summary>
		/// Remove an empty directory. 
		/// </summary>
		/// <param name="path"></param>
		public void RemoveDirectory(System.String path)
		{
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_RMDIR for path " + path );
#endif
			try
			{
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_RMDIR);
				msg.WriteUINT32(requestId);
				msg.WriteString(path, CHARSET_ENCODING);
				
				SendMessage(msg);
				
				GetOKRequestStatus(requestId);
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Remove a file. 
		/// </summary>
		/// <param name="filename"></param>
		public void RemoveFile(System.String filename)
		{

#if DEBUG
			System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_REMOVE for path " + filename );
#endif
			try
			{
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_REMOVE);
				msg.WriteUINT32(requestId);
				msg.WriteString(filename, CHARSET_ENCODING);
				
				SendMessage(msg);
				
				GetOKRequestStatus(requestId);
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Rename an existing file. 
		/// </summary>
		/// <param name="oldpath"></param>
		/// <param name="newpath"></param>
		public void RenameFile(System.String oldpath, System.String newpath)
		{
			
			if (version < 2)
			{
				throw new SFTPStatusException(SFTPStatusException.SSH_FX_OP_UNSUPPORTED,
					"Renaming files is not supported by the server SFTP version " + version.ToString());
			}

#if DEBUG
			System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_RENAME from path " + oldpath + " to " + newpath );
#endif
			try
			{
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_RENAME);
				msg.WriteUINT32(requestId);
				msg.WriteString(oldpath, CHARSET_ENCODING);
				msg.WriteString(newpath, CHARSET_ENCODING);
				
				SendMessage(msg);
				
				GetOKRequestStatus(requestId);
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Get the attributes of a file. 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public SFTPFileAttributes GetAttributes(String path)
		{
			
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_STAT for path " + path );
#endif
			try
			{
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_STAT);
				msg.WriteUINT32(requestId);
				msg.WriteString(path, CHARSET_ENCODING);
				
				SendMessage(msg);
				
				ByteBuffer bar = GetResponse(requestId);
				
				return ExtractAttributes(bar);
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		internal SFTPFileAttributes ExtractAttributes(ByteBuffer bar)
		{
			try
			{
				int type = bar.ReadByte();
				if (type == SSH_FXP_ATTRS)
				{
					bar.ReadUINT32(); //Ignore request id
					return new SFTPFileAttributes(bar);
				}
				else if (type == SSH_FXP_STATUS)
				{
					int id = (int) bar.ReadUINT32();
					int status = (int) bar.ReadUINT32();
					
					// Only read the message string if the version is >= 3
					if (version >= 3)
					{
						System.String msg = bar.ReadString().Trim();
						throw new SFTPStatusException(status, msg);
					}
					else
						throw new SFTPStatusException(status);
				}
				else
				{
					Close();
					throw new SSHException("The server responded with an unexpected message.", 
						SSHException.CHANNEL_FAILURE);
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Get the attributes of a file. 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public SFTPFileAttributes GetAttributes(SFTPFile file)
		{
			
			try
			{
				if (!IsValidHandle(file.Handle))
				{
					return GetAttributes(file.AbsolutePath);
				}
				else
				{
					uint requestId = NextRequestId();
					ByteBuffer msg = CreateMessage();
					msg.WriteByte(SSH_FXP_FSTAT);
					msg.WriteUINT32(requestId);
					msg.WriteBinaryString(file.Handle);
					
					SendMessage(msg);
					
					return ExtractAttributes(GetResponse(requestId));
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Make a directory.
		/// </summary>
		/// <param name="path"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException">
		/// If the directory exists an exception is thrown
		/// </exception>
		/// <remarks>
		/// If you want to ignore exceptions try using <see cref="Maverick.SFTP.SFTPClient.Mkdirs"/>
		/// instead.
		/// </remarks>
		public void MakeDirectory(System.String path)
		{
			MakeDirectory(path, new SFTPFileAttributes());
		}

		/// <summary>
		/// Make a directory.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="attrs">The attributes of the new directory</param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException">
		/// If the directory exists an exception is thrown
		/// </exception>
		/// <remarks>
		/// If you want to ignore exceptions try using <see cref="Maverick.SFTP.SFTPClient.Mkdirs"/>
		/// instead.
		/// </remarks>
		public void MakeDirectory(String path, SFTPFileAttributes attrs)
		{

#if DEBUG
			System.Diagnostics.Trace.WriteLine("Sending SSH_FXP_MKDIR for path " + path );
#endif
			try
			{
				uint requestId = NextRequestId();
				ByteBuffer msg = CreateMessage();
				msg.WriteByte(SSH_FXP_MKDIR);
				msg.WriteUINT32(requestId);
				msg.WriteString(path, CHARSET_ENCODING);
				msg.WriteBytes(attrs.ToByteArray());
				SendMessage(msg);
				GetOKRequestStatus(requestId);
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		internal byte[] GetHandleResponse(uint requestId)
		{
			
			try
			{
				ByteBuffer bar = GetResponse(requestId);
				int type = bar.ReadByte();
				if (type == SSH_FXP_HANDLE)
				{
#if DEBUG
					System.Diagnostics.Trace.WriteLine("Received SSH_FXP_HANDLE");
#endif
					bar.ReadUINT32(); // Ignore the request id
					return bar.ReadBinaryString();
				}
				else if (type == SSH_FXP_STATUS)
				{
					int id = (int) bar.ReadUINT32();
					int status = (int) bar.ReadUINT32();

#if DEBUG
					System.Diagnostics.Trace.WriteLine("Received SSH_FXP_STATUS code " + status );
#endif						
					if (version >= 3)
					{
						String msg = bar.ReadString().Trim();
						//throw new SFTPStatusException(status, msg);
                        byte[] handle = bar.ReadBinaryString();
                        return handle;
					}
					else
					{
						throw new SFTPStatusException(status);
					}
				}
				else
				{
					Close();
					throw new SSHException("The server responded with an unexpected message!", 
						SSHException.CHANNEL_FAILURE);
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		/// <summary>
		/// Verify that an OK status has been returned for a request id. 
		/// </summary>
		/// <param name="requestId"></param>
		public void GetOKRequestStatus(uint requestId)
		{
			
			try
			{
				ByteBuffer bar = GetResponse(requestId);
				if (bar.ReadByte() == SSH_FXP_STATUS)
				{

					int id = (int) bar.ReadUINT32();
					int status = (int) bar.ReadUINT32();

#if DEBUG
					System.Diagnostics.Trace.WriteLine("Received SSH_FXP_STATUS code " + status );
#endif					
					if (status == SFTPStatusException.SSH_FX_OK)
					{
						return ;
					}
					
					if (version >= 3)
					{
						String msg = bar.ReadString().Trim();
						throw new SFTPStatusException(status, msg);
					}
					else
					{
						throw new SFTPStatusException(status);
					}
				}
				else
				{
					Close();
					throw new SSHException("The server responded with an unexpected message!",
						SSHException.CHANNEL_FAILURE);
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SSHException(ex);
			}
		}

		internal class SFTPThreadSynchronizer
		{

			Hashtable responses;

			public SFTPThreadSynchronizer(Hashtable responses)
			{
				this.responses = responses;
			}

			bool isBlocking = false;

			public bool RequestBlock(uint requestId, out bool found)
			{

				lock(this) 
				{
					bool canBlock = !isBlocking;

					if(responses.ContainsKey(requestId))
					{
						found = true;
						return false;
					}

					if(canBlock)
					{
						isBlocking = true;
					}
					else
					{
						System.Threading.Monitor.Wait(this);
					}

					found = false;
					return canBlock;
				}

			}

			public void ReleaseBlock()
			{
				lock(this)
				{
					isBlocking = false;
					System.Threading.Monitor.PulseAll(this);
				}
			}


		}


		internal ByteBuffer GetResponse(uint requestId)
		{
			
			ByteBuffer msg;
			bool found = false;
			while (!found)
			{
				try
				{
					// Read the next response message
					if (sync.RequestBlock(requestId, out found))
					{
						msg = ReadMessage();
						msg.Mark();
						msg.Skip(1);
						uint responseId = msg.ReadUINT32();
						msg.MoveToMark();
						SupportClass.PutElement(responses, responseId, msg);
					}
				}
				catch (System.Threading.ThreadInterruptedException)
				{
					try
					{
						Close();
					}
					catch (System.IO.IOException ex1)
					{
						throw new SSHException(ex1.Message, SSHException.CHANNEL_FAILURE);
					}
					
					throw new SSHException("The thread was interrupted", SSHException.CHANNEL_FAILURE);
				}
				finally
				{
					sync.ReleaseBlock();
				}
			}
			
			return (ByteBuffer) SupportClass.HashtableRemove(responses, requestId);
		}
        static ReactiveQueue<ByteBuffer> brqueue = new ReactiveQueue<ByteBuffer>();
        /// <summary>
        /// yield return messge(ps:outofmemory )   xiecongwen 
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        internal void GetResponseAndEnqueue(uint requestId,Action<List<ByteBuffer>> action)
        {

            brqueue.SetEventHandler((list) => action(list));
            ByteBuffer msg;
            bool found = false;
            while (!found)
            {
                try
                {
                    // Read the next response message
                    if (sync.RequestBlock(requestId, out found))
                    {
                        msg = ReadMessage();
                        brqueue.Enqueue(msg);
                        msg.Mark();
                        msg.Skip(1);
                        uint responseId = msg.ReadUINT32();
                        msg.MoveToMark();
                        SupportClass.PutElement(responses, responseId, msg);
                    }
                }
                catch (System.Threading.ThreadInterruptedException)
                {
                    try
                    {
                        Close();
                    }
                    catch (System.IO.IOException ex1)
                    {
                        throw new SSHException(ex1.Message, SSHException.CHANNEL_FAILURE);
                    }

                    throw new SSHException("The thread was interrupted", SSHException.CHANNEL_FAILURE);
                }

                finally
                {
                    sync.ReleaseBlock();
                }
            }

            SupportClass.HashtableRemove(responses, requestId);

        }

		internal uint NextRequestId()
		{
			return requestId++;
		}
	}
}
