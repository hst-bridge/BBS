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
using Maverick.SSH;
using Maverick.SSH2;
using Maverick.Events;
using System.Globalization;
using Maverick.Crypto.Util;
using System.Collections.Generic;
using Common;

namespace Maverick.SFTP
{
	/// <summary>
	/// SFTP is a secure file transfer program similar to FTP, but performs all operations over an encrypted 
	/// SSH transport, thus gaining the features of public key encryption and compression. 
	/// </summary>
	internal class NamespaceDoc
	{
	}

	/// <summary>
	/// Implements a Secure File Transfer (SFTP) client and which provides working directory management 
	/// with the remote server.
	/// </summary>
	/// <remarks>
	/// SFTP was designed to be executed as an SSH2 subsystem and therefore only SSH2 servers support the 
	/// protocol by default. It is possible in some circumstances to execute SFTP over the SSH1 protocol
	/// because SSH servers such as OpenSSH support both versions of the protocol, and while you cannot
	/// execute a subsystem using SSH1 you can execute the binary "sftp-server" directly.
	/// 
	/// The Maverick.NET SFTP client attempts to find and execute the SFTP binary when it detects an 
	/// SSH1 connection is in use.
	/// </remarks>
	/// <example>
	/// <code>
	/// // Create a connection
	/// SSHConnector con = SSHConnector.Create();
	/// SSHClient ssh = con.Connect(new TcpClientTransport("my.domain.com, 22),
	///				"root", true);
	///	
	///	// Prepare the authentication request								
	///	PasswordAuthentication pwd = new PasswordAuthentication();
	///	pwd.Password = "*********";
	///	
	///	// Authenticate the user
	///	if(ssh.Authenticate(pwd)==AuthenticationResult.COMPLETE)
	///	{
	///		// Open up a session and do something..
	///		SFTPClient sftp = new SFTPClient(ssh, "C:\\Documents and Settings\\lee");
	///		
	///		// Make some directories
	///		sftp.Mkdirs("maverick.net/sftp");
	/// 
	///		// Change the working directory
	///		sftp.Cd("maverick.net/sftp");
	///		
	///		// Put a file to the remote file system
	///		sftp.PutFile("test.file");
	///		
	///		// Put a file using a UNC path
	///		sftp.PutFile("\\\\PDC\\Public\\UNC.TEST");
	/// 
	///		sftp.Rename("test.file", "sftp-download");
	/// 
	///		// Change the permissions on the file
	///		sftp.Chmod("600", "sftp-download");
	/// 
	///		// Download the file from the remote system
	///		sftp.GetFile("sftp-download");
	/// 
	///		// Remove the remote file
	///		sftp.Rm("sftp-download");
	/// 
	///		// Remove the directories recursively
	///		sftp.Rm("maverick.net", true, true);
	/// 
	///		// Quit and exit
	///		sftp.Quit():
	///	}
	///	
	///	ssh.Disconnect();
	///	</code>
	/// </example>
	public class SFTPClient
	{

		internal SFTPSubsystemChannel sftp;
		internal SSHSession session;
		internal String cwd;
		internal String lcwd;
		
		private int blocksize = 32768;
		private int asyncRequests = 100;
		private int buffersize = - 1;
		
		// Default permissions is determined by default_permissions ^ umask
		internal int umask = 18; 

		/// <summary>
		/// An event for receiving file transfer progress information.
		/// </summary>
		public event FileTransferProgress TransferEvents;

		/// <summary>
		/// Construct an uninitialized client.
		/// </summary>
		/// <remarks>
		/// This contructor does not connect the client, call the <see cref="Maverick.SFTP.SFTPClient.Connect"/>
		/// method before attempting to perform any client operations.
		/// </remarks>
		public SFTPClient()
		{

		}

		/// <summary>
		/// Construct a client instance and initialize the SFTP subsystem. This 
		/// constructor sets the local working directory to the users home folder.
		/// </summary>
		/// <remarks>
		/// Using this constructor is the same as using the following code:
		/// <code>
		/// SFTPClient sftp = new SFTPClient();
		/// sftp.Connect(ssh, "C:\\Documents and settings\\lee", null);
		/// </code>
		/// </remarks>
		/// <param name="ssh">The client instance</param>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public SFTPClient(SSHClient ssh)
		{
			Connect(ssh, SupportClass.GetHomeDir(), null);
		}

		/// <summary>
		/// Construct a client instance and initialize the SFTP subsystem.
		/// </summary>
		/// <remarks>
		/// Using this constructor is the same as using the following code:
		/// <code>
		/// SFTPClient sftp = new SFTPClient();
		/// sftp.Connect(ssh, "C:\\Documents and settings\\lee", null);
		/// </code>
		/// </remarks>
		/// <param name="ssh">The client instance</param>
		/// <param name="lcwd">The local working directory</param>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public SFTPClient(SSHClient ssh, String lcwd)
		{
			Connect(ssh, lcwd, null);
		}

		internal bool FireTransferEvent(FileTransferState state, String filename, long bytes)
		{
			try
			{
				if(TransferEvents!=null)
					return TransferEvents(state, filename, bytes);
				else
					return true;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine("WARNING! Exception caught in Maverick.NET event handler: " + ex.Message );
				System.Diagnostics.Trace.WriteLine(ex.StackTrace );
				return true;
			}

		}
		
		/// <summary>
		/// Initialize the SFTP subsystem.
		/// </summary>
		/// <remarks>
		/// This method MUST be called if the default contructor is used to create an instance of
		/// the SFTP client.
		/// 
		/// Connecting to the SFTP server involves creating a new session and executing the SFTP subsystem. With 
		/// an SSH2 connection this is a straight forward procedure, with SSH1 an attempt to locate the SFTP binary
		/// "sftp-server" is made before executing it directly.
		/// </remarks>
		/// <param name="ssh">The client instance from which to obtain a new session for the SFTP channel</param>
		/// <param name="lcwd">The local working directory</param>
		/// <param name="listener">An optional listener delegate to receive notification of the channels state.</param>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Connect(SSHClient ssh, String lcwd, ChannelStateListener listener) 
		{

#if DEBUGA
#endif 
#if DEBUG
#endif		
				session = ((SSH2Client)ssh).OpenSessionChannel(131070, 34000, listener);
			

				SSH2Session ssh2 = (SSH2Session) session;
				if (!ssh2.StartSubsystem("sftp"))
				{
					// We could not start the subsystem try to fallback to the
					// provider specified by the user
					if (!ssh2.ExecuteCommand(ssh.Context.SFTPProvider))
					{
						ssh2.Close();
						throw new SSHException("Failed to start SFTP subsystem or SFTP provider " + ssh.Context.SFTPProvider,
							SSHException.CHANNEL_FAILURE);
					}
				}
			
			sftp = new SFTPSubsystemChannel(session);
			
			sftp.Initialize();

			// Get the users default directory
			cwd = sftp.DefaultDirectory;
			this.lcwd = lcwd;

#if DEBUG
			System.Diagnostics.Trace.WriteLine("Setting CWD to " + cwd );
#endif

		}

		/// <summary>
		/// Get the underlying <see cref="Maverick.SSH.SSHSession"/> for this client instance.
		/// </summary>
		/// <remarks>
		/// This should be used to determine such things as the exit status of the command, DO NOT
		/// attempt to write or read data from the channel's stream. This will corrupt the SFTP
		/// protocol executing within the session and render the client useless.
		/// </remarks>
		/// <returns></returns>
		public SSHSession GetChannel()
		{
			return session;
		}

		/// <summary>
		/// Returns <tt>true</tt> if the SFTP client has been closed, otherwise <tt>false</tt>
		/// </summary>
		public bool IsClosed
		{
			get
			{
				return sftp.IsClosed;
			}
			
		}

		/// <summary>
		/// Set the maximum number of asynchronous requests that are outstanding at any one time. This 
		/// setting is used to optimize the reading and writing of files to/from the remote file system 
		/// when using the get and put methods. The default for this setting is 100. 
		/// </summary>
		public int MaxAsyncRequests
		{
			set
			{
				if (value < 1)
				{
					throw new System.ArgumentException("Maximum asynchronous requests must be greater or equal to 1");
				}
				this.asyncRequests = value;
			}
			
		}

		/// <summary>
		/// Sets the block size used when transfering files, defaults to the optimized setting of 32768. You should 
		/// not increase this value as the remote server may not be able to support higher blocksizes however you
		/// may decrease the setting to no less than 4096 bytes.
		/// </summary>
		public int BlockSize
		{
			set
			{
				if (value < 4096)
				{
					throw new System.ArgumentException("Block size must be greater than 4096");
				}
				this.blocksize = value;
			}

			get
			{
				return blocksize;
			}
			
		}

		/// <summary>
		/// Set the size of the buffer which is used to read from the local file system. This setting is used to 
		/// optimize the writing of files by allowing for a large chunk of data to be read in one operation from 
		/// a local file. This increases performance and so this setting should be set to the highest value 
		/// possible. The default setting is negative which means that no additional buffering is performed. 
		/// </summary>
		public int BufferSize
		{
			set
			{
				this.buffersize = value;
			}

			get
			{
				return buffersize;
			}
			
		}

		/// <summary>
		/// Returns the underlying <see cref="Maverick.SSH.SubsystemChannel"/> for this client.
		/// </summary>
		public SFTPSubsystemChannel SubsystemChannel
		{
			get
			{
				return sftp;
			}
		}

		/// <summary>
		/// Changes the working directory on the remote server, or the home directory if null or 
		/// any empty string is provided as the directory path.
		/// </summary>
		/// <param name="dir">The new remote working directory</param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Cd(System.String dir)
		{
			String actual;
			
			if (dir.Equals(""))
			{
				actual = sftp.DefaultDirectory;
			}
			else
			{
				actual = ResolveRemotePath(dir);
				actual = sftp.GetAbsolutePath(actual);
			}
			
			SFTPFileAttributes attr = sftp.GetAttributes(actual);
			
			if (!attr.IsDirectory)
			{
				throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE, dir + " is not a directory");
			}
			
			cwd = actual;
		}

		/// <summary>
		/// Change the working directory to the parent directory.
		/// </summary>
		public void Cdup()
		{
			
			SFTPFile cd = sftp.GetFile(cwd);
			SFTPFile parent = cd.Parent;
			
			if (parent != null)
				cwd = parent.AbsolutePath;
		}

		private String FormatRemotePath(String path)
		{
			return path.Replace("\\", "/");
		}

		private System.IO.FileInfo ResolveLocalPath(String path)
		{
			if(!IsLocalAbsolutePath(path))
			{
				return new System.IO.FileInfo(lcwd + "\\" + path);
			} 
			else
				return new System.IO.FileInfo(path);

		}

		private String ResolveRemotePath(String path)
		{
			VerifyConnection();
			
			path = FormatRemotePath(path);

			String actual;
			
			if (!path.StartsWith("/"))
			{
				actual = cwd + (cwd.EndsWith("/")?"":"/") + path;
			}
			else
			{
				actual = path;
			}
			
			return actual;
		}

		private void VerifyConnection()
		{
			if (sftp.IsClosed)
			{
				throw new SFTPStatusException(SFTPStatusException.SSH_FX_CONNECTION_LOST, 
						"The SFTP connection has been closed");
			}
		}

		/// <summary>
		/// Creates a new directory on the remote server. To create directories and disregard any 
		/// errors use the <see cref="Maverick.SFTP.SFTPClient.Mkdirs"/> method. 
		/// </summary>
		/// <param name="dir"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void  Mkdir(String dir)
		{
			String actual = ResolveRemotePath(dir);
			
			try
			{
				SFTPFileAttributes attrs = Stat(actual);
				if (!attrs.IsDirectory)
				{
					throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE, 
							"File already exists named " + dir);
				}
			}
			catch (SFTPStatusException)
			{
				SFTPFileAttributes attrs = new SFTPFileAttributes();
				attrs.Permissions = 511 ^ umask;
				sftp.MakeDirectory(actual, attrs);
			}
		}

		/// <summary>
		/// Create a directory or set of directories. This method will not fail even if the 
		/// directories exist. It is advisable to test whether the directory exists before attempting 
		/// an operation by using <see cref="Maverick.SFTP.SFTPClient.Stat"/> to return the directories 
		/// attributes. 
		/// </summary>
		/// <param name="dir"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Mkdirs(System.String dir)
		{
			SupportClass.Tokenizer tokens = new SupportClass.Tokenizer(dir, "/");
			String path = dir.StartsWith("/")?"/":"";
			
			while (tokens.HasMoreTokens())
			{
				path += (String) tokens.NextToken();
				
				try
				{
					Stat(path);
				}
				catch
				{
					try
					{
						Mkdir(path);
					}
					catch (SFTPStatusException ex2)
					{
						if (ex2.Status == SFTPStatusException.SSH_FX_PERMISSION_DENIED)
							throw ex2;
					}
				}
				
				path += "/";
			}
		}

		/// <summary>
		/// Returns the absolute path name of the current remote working directory. 
		/// </summary>
		/// <returns></returns>
		public String Pwd()
		{
			return cwd;
		}

		/// <summary>
		/// List the contents of the current remote working directory.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public SFTPFile[] Ls()
		{
			return Ls(cwd);
		}

		/// <summary>
		// action ssh
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
        public void LsAndDo(String path, Action<List<SFTPFileArgs>> action)
		{
            try
            {
                String actual = ResolveRemotePath(path);
                SFTPFile file = null;
                try
                {
                     file = sftp.OpenDirectory(actual);
                     SFTPSubsystemChannel.rqueue.SetEventHandler((list) => action(list));
                     sftp.ListChildrenByQueue(file);
                }
                catch (Exception)
                {
                    //there  is  something wrong
                   // throw;
                }
                finally
                {
                    if(file!=null)
                        file.Close();
                }
               
            }
            catch (Exception) {
                throw;
            }
		}

        /// <summary>
        /// List the contents of the given remote directory.  and  writelog
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Maverick.SFTP.SFTPStatusException"/>
        /// <exception cref="Maverick.SSH.SSHException"/>
        public SFTPFile[] Ls(String path,Action<List<string>> action)
        {
            try
            {
                //cleare the file list before
                sftp.FilePathList.Clear();
                String actual = ResolveRemotePath(path);
                SFTPFile file = null;
                try
                {
                    file = sftp.OpenDirectory(actual);
                    //System.Collections.ArrayList children = new System.Collections.ArrayList();
                    SFTPSubsystemChannel.failPathQueue.SetEventHandler((list) => action(list));
                    SFTPSubsystemChannel.failPathQueue.MaxFactor = 2;
                    sftp.ListChildren(file);
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(LogFile.WARNING, ex.Message);
                    //there  is  something wrong
                    //action(ex.Message);
                    //sftp.FilePathList.Clear();
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
                //while (sftp.ListChildren(file) > - 1)
                //{
                //    ;
                //}

                SFTPFile[] files = null;
                if (sftp.FilePathList.Count > 0)
                {
                    files = new SFTPFile[sftp.FilePathList.Count];
                    int index = 0;
                    for (System.Collections.IEnumerator e = sftp.FilePathList.GetEnumerator(); e.MoveNext(); )
                    {
                        files[index++] = (SFTPFile)e.Current;
                    }
                }
                return files;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// List the contents of the given remote directory. 
        /// 
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Maverick.SFTP.SFTPStatusException"/>
        /// <exception cref="Maverick.SSH.SSHException"/>
        public SFTPFile[] Ls(String path)
        {
            try
            {
                //cleare the file list before
                sftp.FilePathList.Clear();
                String actual = ResolveRemotePath(path);
                SFTPFile file = null;
                try
                {
                    file = sftp.OpenDirectory(actual);
                    //System.Collections.ArrayList children = new System.Collections.ArrayList();

                    sftp.ListChildren(file);
                }
                catch (Exception)
                {
                    //there  is  something wrong

                    sftp.FilePathList.Clear();
                    throw;
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
                //while (sftp.ListChildren(file) > - 1)
                //{
                //    ;
                //}

                SFTPFile[] files = null;
                if (sftp.FilePathList.Count > 0)
                {
                    files = new SFTPFile[sftp.FilePathList.Count];
                    int index = 0;
                    for (System.Collections.IEnumerator e = sftp.FilePathList.GetEnumerator(); e.MoveNext(); )
                    {
                        files[index++] = (SFTPFile)e.Current;
                    }
                }
                return files;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// List the contents of the given remote directory. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Maverick.SFTP.SFTPStatusException"/>
        /// <exception cref="Maverick.SSH.SSHException"/>
        public SFTPFile[] TopFileLs(String path)
        {
            String actual = ResolveRemotePath(path);
            SFTPFile file = sftp.OpenDirectory(actual);
            //System.Collections.ArrayList children = new System.Collections.ArrayList();
            sftp.TopListChildren(file);
            //while (sftp.ListChildren(file) > - 1)
            //{
            //    ;
            //}
            file.Close();
            SFTPFile[] files = null;
            if (sftp.TopFilePathList.Count > 0)
            {
                files = new SFTPFile[sftp.TopFilePathList.Count];
                int index = 0;
                for (System.Collections.IEnumerator e = sftp.TopFilePathList.GetEnumerator(); e.MoveNext(); )
                {
                    files[index++] = (SFTPFile)e.Current;
                }
            }


            return files;
        }

        /// <summary>
        /// List the contents of the given remote directory. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Maverick.SFTP.SFTPStatusException"/>
        /// <exception cref="Maverick.SSH.SSHException"/>
        public SFTPFile[] TopDirLs(String path)
        {
            String actual = ResolveRemotePath(path);
            SFTPFile file = sftp.OpenDirectory(actual);
            //System.Collections.ArrayList children = new System.Collections.ArrayList();
            sftp.TopListChildren(file);
            //while (sftp.ListChildren(file) > - 1)
            //{
            //    ;
            //}
            file.Close();
            SFTPFile[] files = null;
            if (sftp.TopDirPathList.Count > 0)
            {
                files = new SFTPFile[sftp.TopDirPathList.Count];
                int index = 0;
                for (System.Collections.IEnumerator e = sftp.TopDirPathList.GetEnumerator(); e.MoveNext(); )
                {
                    files[index++] = (SFTPFile)e.Current;
                }
            }
            return files;
        }

		/// <summary>
		/// Changes the local working directory. 
		/// </summary>
		/// <param name="path"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void  Lcd(String path)
		{
			System.IO.FileInfo actual;
			
			if (!IsLocalAbsolutePath(path))
			{
				actual = new System.IO.FileInfo(lcwd + (lcwd.EndsWith("\\") ? "" : "\\") + path);
			}
			else
			{
				actual = new System.IO.FileInfo(path);
			}
			
			if (!System.IO.Directory.Exists(actual.FullName))
			{
				throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE, path + " is not a directory");
			}
			
			try
			{
				lcwd = actual.FullName;
			}
			catch (System.IO.IOException)
			{
				throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE, "Failed to canonicalize path " + path);
			}
		}

		private static bool IsLocalAbsolutePath(System.String path)
		{
			// Is this path a UNC path or does it have a drive?
			return System.IO.Path.IsPathRooted(path);
		}

		/// <summary>
		/// Returns the absolute path to the local working directory. 
		/// </summary>
		/// <returns></returns>
		public System.String Lpwd()
		{
			return lcwd;
		}

		/// <summary>
		/// Download the remote file to the local computer.
		/// </summary>
		/// <param name="path">The path of the file to retrieve from the remote computer.</param>
		/// <param name="resume">Should the client attempt to resume from a previous attempt.</param>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public SFTPFileAttributes GetFile(String path, bool resume)
		{
			String localfile;
			
			if (path.LastIndexOf("/") > - 1)
			{
				localfile = path.Substring(path.LastIndexOf("/") + 1);
			}
			else
			{
				localfile = path;
			}
			
			return GetFile(path, localfile, resume);
		}

		/// <summary>
		/// Download the remote file to the local computer.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public SFTPFileAttributes GetFile(System.String path)
		{
			return GetFile(path, false);
		}

	/*	private void  TransferFile(System.IO.Stream input, System.IO.Stream output, String remoteFile)
		{
			TransferFile(input, output, remoteFile,null);
		}

		private void TransferFile(System.IO.Stream input, System.IO.Stream output, String remoteFile, FileTransferProgress progress)
		{
			try
			{
				long bytesSoFar = 0;
				byte[] buffer = new byte[blocksize];
				int read;
				
				while ((read = input.Read(buffer, 0, buffer.Length)) > - 1)
				{
				
					if (read > 0)
					{
						output.Write(buffer, 0, read);
						output.Flush();

						bytesSoFar += read;
						
							if(!FireTransferEvent(FileTransferState.TRANSFER_PROGRESSED, remoteFile, bytesSoFar))
								throw new TransferCancelledException();
					}
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE, "IO Error during data transfer: " + ex.Message);
			}
			finally
			{
				try
				{
					input.Close();
				}
				catch
				{
				}
				
				try
				{
					output.Close();
				}
				catch
				{
				}
			}
		}*/

		/// <summary>
		/// Download the remote file to the local computer.
		/// </summary>
		/// <param name="remote"></param>
		/// <param name="local"></param>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public SFTPFileAttributes GetFile(System.String remote, System.String local)
		{
			return GetFile(remote, local, false);
		}

		/// <summary>
		/// Download the remote file to the local computer.
		/// </summary>
		/// <param name="remote"></param>
		/// <param name="local"></param>
		/// <param name="resume"></param>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public SFTPFileAttributes GetFile(System.String remote, System.String local, bool resume)
		{
			System.IO.FileInfo localPath = ResolveLocalPath(local);
			remote = FormatRemotePath(remote);

			if(System.IO.Directory.Exists(localPath.FullName))
			{
				// Local path is a directory			
				int idx = remote.IndexOf('/');

				if(idx > -1) 
				{
					localPath = new System.IO.FileInfo(localPath.FullName + "\\" + remote.Substring(idx+1));
				} 
				else 
				{
					localPath = new System.IO.FileInfo(localPath.FullName + "\\" + remote);
				}

			}
			else if(!System.IO.Directory.Exists(localPath.DirectoryName))
			{
				// Local path parent directory does not exist
				System.IO.FileInfo parent = new System.IO.FileInfo(localPath.DirectoryName);
				System.IO.Directory.CreateDirectory(parent.FullName);
			}
			
			long position = 0;
			bool fileExists = System.IO.File.Exists(localPath.FullName);
			try
			{
				
				System.IO.Stream output;
				if (resume && System.IO.File.Exists(localPath.FullName))
				{
					output = new System.IO.FileStream(localPath.FullName, System.IO.FileMode.Append, System.IO.FileAccess.Write);
					position = output.Position;

				}
				else
				{
					output = new System.IO.FileStream(localPath.FullName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
				}
				
				SFTPFileAttributes attrs = GetFile(remote, output, position);
				
				// TODO: Try to set the last modified time on the file to the remote setting

				return attrs;
			}
			catch (System.IO.IOException)
			{
				throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE, "Failed to open outputstream to " + local);
			}
			catch (SFTPStatusException ex)
			{
				if(!fileExists)
					System.IO.File.Delete(localPath.FullName);
				throw ex;
			}
		}



		/// <summary>
		/// Download the remote file to the local computer.
		/// </summary>
		/// <param name="remote"></param>
		/// <param name="local"></param>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public SFTPFileAttributes GetFile(System.String remote, System.IO.Stream local)
		{
			return GetFile(remote, local, 0);
		}

		/// <summary>
		/// Download the remote file to the local computer.
		/// </summary>
		/// <param name="remote"></param>
		/// <param name="local"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public SFTPFileAttributes GetFile(System.String remote, System.IO.Stream local, long position)
		{
			System.String remotePath = remote;

			try
			{
				remotePath = ResolveRemotePath(remote);
			
				SFTPFileAttributes attrs = Stat(remotePath);
			
				if (position > attrs.Size)
				{
					throw new SFTPStatusException(SFTPStatusException.INVALID_RESUME_STATE, 
						"The local file size is greater than the remote file");
				}

				if(!FireTransferEvent(FileTransferState.TRANSFER_STARTED, remotePath, attrs.Size))
					throw new TransferCancelledException();
			
				SFTPFile file = sftp.OpenFile(remotePath, SFTPSubsystemChannel.OPEN_READ);
			
				try
				{
					FireTransferEvent(FileTransferState.TRANSFER_COMPLETED, remotePath, 
						sftp.PerformOptimizedRead(file.Handle, attrs.Size, blocksize, local, asyncRequests, TransferEvents, position));
				}
				catch (TransferCancelledException tce)
				{
					FireTransferEvent(FileTransferState.TRANSFER_CANCELLED, remotePath, 0);
					throw tce;
				}
				finally
				{
				
					try
					{
						sftp.CloseFile(file);
					}
					catch (SFTPStatusException)
					{
					}
				}
			
				return attrs;
			}
			catch(Exception ex)
			{
				FireTransferEvent(FileTransferState.TRANSFER_ERROR, remotePath, 0);
				throw ProcessException(ex);
			}
			finally
			{
				try
				{
					local.Close();
				}
				catch
				{
				}
			}
		}

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public SFTPFile OpenFile(string absolutePath)
        {
            string remotePath = ResolveRemotePath(absolutePath);

            SFTPFileAttributes attrs = Stat(remotePath);

            SFTPFile file = sftp.OpenFile(remotePath, SFTPSubsystemChannel.OPEN_READ);
            return file;
        }


		/// <summary>
		/// Upload a file to the remote computer. 
		/// </summary>
		/// <param name="local"></param>
		/// <param name="resume"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void PutFile(System.String local, bool resume)
		{
			System.IO.FileInfo f = new System.IO.FileInfo(local);
			PutFile(local, f.Name, resume);
		}

		/// <summary>
		/// Upload a file to the remote computer. 
		/// </summary>
		/// <param name="local"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void PutFile(System.String local)
		{
			PutFile(local, false);
		}

		/// <summary>
		/// Upload a file to the remote computer. 
		/// </summary>
		/// <param name="local"></param>
		/// <param name="remote"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void PutFile(System.String local, System.String remote)
		{
			PutFile(local, remote, false);
		}

		/// <summary>
		/// Upload a file to the remote computer. 
		/// </summary>
		/// <param name="local"></param>
		/// <param name="remote"></param>
		/// <param name="resume"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void PutFile(System.String local, System.String remote, bool resume)
		{
			System.IO.FileInfo localPath = ResolveLocalPath(local);
			remote = FormatRemotePath(remote);

			if(!System.IO.File.Exists(localPath.FullName))
				throw new SFTPStatusException(SFTPStatusException.SSH_FX_NO_SUCH_FILE,
					localPath.FullName + " does not exist!");

			System.IO.FileStream input = new System.IO.FileStream(localPath.FullName, 
				System.IO.FileMode.Open, 
				System.IO.FileAccess.Read);

			System.IO.FileInfo f = new System.IO.FileInfo(local);
			long position = 0;
			
			try
			{
				SFTPFileAttributes attrs = Stat(remote);
				if (attrs.IsDirectory)
				{
					
					remote += (remote.EndsWith("/")?"":"/") + f.Name;
					attrs = Stat(remote);
				}
				
				if (resume)
				{
					
					if (SupportClass.FileLength(f) < attrs.Size)
						throw new SFTPStatusException(SFTPStatusException.INVALID_RESUME_STATE, 
							"The remote file size is greater than the local file");
					try
					{
						input.Seek(attrs.Size, System.IO.SeekOrigin.Begin);
						position = attrs.Size;
					}
					catch (System.IO.IOException ex)
					{
						throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE, 
							"Failed to seek to file position " + position + ": " + ex.Message);
					}
				}
			}
			catch(Exception ex)
			{
				ProcessException(ex);
			}

			PutFile(input, remote, position);
		}

		/// <summary>
		/// Upload a file to the remote computer. 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="remote"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void PutFile(System.IO.Stream input, System.String remote)
		{
			PutFile(input, remote, 0);
		}
		
		/// <summary>
		/// Upload a file to the remote computer. 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="remote"></param>
		/// <param name="position"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		/// <exception cref="Maverick.SFTP.TransferCancelledException"/>
		public void PutFile(System.IO.Stream input, System.String remote, long position)
		{
			System.String remotePath = remote;

			try
			{
				remotePath = ResolveRemotePath(remote);
			
				SFTPFileAttributes attrs;
			
				SFTPFile file;
				try
				{
					attrs = Stat(remotePath);
				
					if (position > 0)
					{
						file = sftp.OpenFile(remotePath, SFTPSubsystemChannel.OPEN_APPEND | SFTPSubsystemChannel.OPEN_WRITE);
					}
					else
					{
						file = sftp.OpenFile(remotePath, SFTPSubsystemChannel.OPEN_CREATE | SFTPSubsystemChannel.OPEN_TRUNCATE | SFTPSubsystemChannel.OPEN_WRITE);
					}
				}
				catch (SFTPStatusException)
				{
				
					attrs = new SFTPFileAttributes();
					attrs.Permissions = 438 ^ (438 & umask);
				
					file = sftp.OpenFile(remotePath, SFTPSubsystemChannel.OPEN_CREATE | SFTPSubsystemChannel.OPEN_WRITE, attrs);
				}
			
				if(!FireTransferEvent(FileTransferState.TRANSFER_STARTED, remotePath, input.Length))
					throw new TransferCancelledException();

		
				try
				{
					FireTransferEvent(FileTransferState.TRANSFER_COMPLETED, remotePath, 
						sftp.PerformOptimizedWrite(file.Handle, blocksize, asyncRequests, input, buffersize, TransferEvents, position));
				}
				catch(TransferCancelledException tce) 
				{
					FireTransferEvent(FileTransferState.TRANSFER_CANCELLED, remotePath, 0);
					throw tce;
				}
				finally
				{
					try
					{
						sftp.CloseFile(file);
					}
					catch(SFTPStatusException)
					{
					}
				}
		
			}			
			catch(Exception ex)
			{
				FireTransferEvent(FileTransferState.TRANSFER_ERROR, remotePath, 0);
				throw ProcessException(ex);
			}
			finally
			{

				try
				{
					input.Close();
				}
				catch
				{
				}
			}
		}


		/// <summary>
		/// Sets the user ID to owner for the file or directory. 
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="path"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Chown(int uid, System.String path)
		{
			try
			{
				System.String actual = ResolveRemotePath(path);
			
				SFTPFileAttributes attrs = sftp.GetAttributes(actual);
				attrs.UID = uid;
				sftp.SetAttributes(actual, attrs);
			}
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
		}

		/// <summary>
		/// Sets the group ID to owner for the file or directory. 
		/// </summary>
		/// <param name="gid"></param>
		/// <param name="path"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Chgrp(int gid, System.String path)
		{
			try 
			{
				System.String actual = ResolveRemotePath(path);
			
				SFTPFileAttributes attrs = sftp.GetAttributes(actual);
				attrs.GID = gid;
				sftp.SetAttributes(actual, attrs);
			}
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
		}

		/// <summary>
		/// Changes the access permissions or modes of the specified file or directory. 
		/// Modes determine who can read, change or execute a file. <br/>
		/// <br/>
		/// Absolute modes are octal numbers specifying the complete list of
		/// attributes for the files; you specify attributes by OR'ing together	these bits.<br/>
		/// 0400       Individual read<br/>
		/// 0200       Individual write<br/>
		/// 0100       Individual execute (or list directory)<br/>
		/// 0040       Group read<br/>
		/// 0020       Group write<br/>
		/// 0010       Group execute<br/>
		/// 0004       Other read<br/>
		/// 0002       Other write<br/>
		/// 0001       Other execute<br/>
		/// </summary>
		/// <param name="permissions"></param>
		/// <param name="path"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Chmod(int permissions, System.String path)
		{
			try
			{
				System.String actual = ResolveRemotePath(path);
				sftp.ChangePermissions(actual, permissions);
			}
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
		}

		/// <summary>
		/// Changes the access permissions or modes of the specified file or directory. 
		/// Modes determine who can read, change or execute a file. <br/>
		/// <br/>
		/// Absolute modes are octal numbers specifying the complete list of
		/// attributes for the files; you specify attributes by OR'ing together	these bits.<br/>
		/// 0400       Individual read<br/>
		/// 0200       Individual write<br/>
		/// 0100       Individual execute (or list directory)<br/>
		/// 0040       Group read<br/>
		/// 0020       Group write<br/>
		/// 0010       Group execute<br/>
		/// 0004       Other read<br/>
		/// 0002       Other write<br/>
		/// 0001       Other execute<br/>
		/// </summary>
		/// <param name="permissions"></param>
		/// <param name="path"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Chmod(String permissions, String path)
		{
			try
			{
				// Must be an octal string
				String actual = ResolveRemotePath(path);
				sftp.ChangePermissions(actual, System.Convert.ToInt32(permissions, 8));
			}
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
		}

		/// <summary>
		/// Sets the umask for this client.<br/>
		/// <br/>
		/// To give yourself full permissions for both files and directories and
		/// prevent the group and other users from having access:<br/>
		/// <br/>
		/// Umask("077");<br/>
		/// <br/>
		/// This subtracts 077 from the system defaults for files and directories
		/// 666 and 777. Giving a default access permissions for your files of
		/// 600 (rw-------) and for directories of 700 (rwx------).<br/>
		/// <br/>
		/// To give all access permissions to the group and allow other users read
		/// and execute permission:<br/>
		/// <br/>
		/// Umask("002");<br/>
		/// <br/>
		/// This subtracts 002 from the sytem defaults to give a default access permission
		/// for your files of 664 (rw-rw-r--) and for your directories of 775 (rwxrwxr-x).<br/>
		/// <br/>
		/// To give the group and other users all access except write access:<br/>
		/// <br/>
		/// Umask("022");<br/>
		/// <br/>
		/// This subtracts 022 from the system defaults to give a default access permission
		/// for your files of 644 (rw-r--r--) and for your directories of 755 (rwxr-xr-x).
		/// </summary>
		/// <param name="umask"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Umask(System.String umask)
		{
			try
			{
				this.umask = System.Convert.ToInt32(umask, 8);
			}
			catch (System.FormatException)
			{
				throw new SSHException("umask must be 4 digit octal number e.g. 0022", SSHException.BAD_API_USAGE);
			}
		}

		/// <summary>
		/// Rename a file on the remote computer. 
		/// </summary>
		/// <param name="oldpath"></param>
		/// <param name="newpath"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Rename(System.String oldpath, System.String newpath)
		{
			try
			{
				System.String from = ResolveRemotePath(oldpath);
				System.String to = ResolveRemotePath(newpath);
			
				bool error = false;
				try
				{
					SFTPFileAttributes attrs = Stat(to);
					error = true;
				}
				catch
				{
					sftp.RenameFile(from, to);
				}

				if(error)
					throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE,
						newpath + " already exists on the remote filesystem");
			}
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
			
		}

		/// <summary>
		/// Remove a file or directory from the remote computer. 
		/// </summary>
		/// <param name="path"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Rm(System.String path)
		{
			try
			{
				System.String actual = ResolveRemotePath(path);
			
				SFTPFileAttributes attrs = sftp.GetAttributes(actual);
				if (attrs.IsDirectory)
				{
					sftp.RemoveDirectory(actual);
				}
				else
				{
					sftp.RemoveFile(actual);
				}
			}
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
		}

		/// <summary>
		/// Remove a file or directory on the remote computer with options to force deletion of existig files and recursion.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="force"></param>
		/// <param name="recurse"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Rm(System.String path, bool force, bool recurse)
		{

			try
			{
				System.String actual = ResolveRemotePath(path);
			
				SFTPFileAttributes attrs = null;
			
				attrs = sftp.GetAttributes(actual);
			
				SFTPFile file;
			
				if (attrs.IsDirectory)
				{
					SFTPFile[] list = Ls(path);
				
					if (!force && (list.Length > 0))
					{
						throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE, 
							"You cannot delete non-empty directory, use force=true to overide");
					}
					else
					{
						for (int i = 0; i < list.Length; i++)
						{
							file = (SFTPFile) list[i];
						
							if (file.IsDirectory && !file.Filename.Equals(".") && !file.Filename.Equals(".."))
							{
								if (recurse)
								{
									Rm(file.AbsolutePath, force, recurse);
								}
								else
								{
									throw new SFTPStatusException(SFTPStatusException.SSH_FX_FAILURE, 
										"Directory has contents, cannot delete without recurse=true");
								}
							}
							else if (file.IsFile)
							{
								sftp.RemoveFile(file.AbsolutePath);
							}
						}
					}
				
					sftp.RemoveDirectory(actual);
				}
				else
				{
					sftp.RemoveFile(actual);
				}
			}
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
		}

		/// <summary>
		/// Create a symbolic link on the remote computer. 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="link"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Symlink(System.String path, System.String link)
		{
			try 
			{
				System.String actualPath = ResolveRemotePath(path);
				System.String actualLink = ResolveRemotePath(link);
			
				sftp.CreateSymbolicLink(actualPath, actualLink);
			} 
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
		}

		/// <summary>
		/// Returns the attributes of the file from the remote computer. 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public SFTPFileAttributes Stat(System.String path)
		{
			try 
			{
				System.String actual = ResolveRemotePath(path);
				return sftp.GetAttributes(actual);
			} 
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
		}

		/// <summary>
		/// Get the absolute path for a file. 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public System.String GetAbsolutePath(System.String path)
		{
			try
			{
				System.String actual = ResolveRemotePath(path);
				return sftp.GetAbsolutePath(actual);
			} 
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
		}

		/// <summary>
		/// Close the SFTP client. 
		/// </summary>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Quit()
		{
			try
			{
				sftp.Close();
			}
			catch (System.IO.IOException ex1)
			{
				throw new SSHException(ex1.Message, SSHException.CHANNEL_FAILURE);
			}
		}


		/// <summary>
		/// Format a String with the details of the file. <br/>
		/// <br/>
		/// <code>
		/// -rwxr-xr-x   1 mjos     staff      348911 Mar 25 14:29 t-filexfer
		/// </code>
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static System.String FormatLongname(SFTPFile file)
		{
			return FormatLongname(file.Attributes, file.Filename);
		}

		/// <summary>
		/// Format a String with the details of the file. <br/>
		/// <br/>
		/// <code>
		/// -rwxr-xr-x   1 mjos     staff      348911 Mar 25 14:29 t-filexfer
		/// </code>
		/// </summary>
		/// <param name="attrs"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static System.String FormatLongname(SFTPFileAttributes attrs, System.String filename)
		{
			Int32 i = attrs.UID;
			
			System.Text.StringBuilder str = new System.Text.StringBuilder();
			str.Append(pad(10 - attrs.PermissionsString.Length) + attrs.PermissionsString);
			str.Append("   1 ");
			str.Append(i.ToString() + pad(8 - i.ToString().Length));
			//uid
			str.Append(" ");
			i = attrs.GID;
			str.Append(i.ToString() + pad(8 - i.ToString().Length));
			//gid
			str.Append(" ");
			Int64 l = attrs.Size;
			str.Append(pad(8 - l.ToString().Length) + l.ToString());
			str.Append(" ");
			String mod = GetModTimeString(attrs.ModifiedTime);
			str.Append(pad(12 - mod.Length) + mod);
			str.Append(" ");
			str.Append(filename);
			
			return str.ToString();
		}

		private static System.String GetModTimeString(uint mtime)
		{
			DateTimeFormatInfo format = new DateTimeFormatInfo();

			long mt = mtime * 1000L;
			long now = SupportClass.ConvertToEpochTime(DateTime.Now);

			if((now - mt) > (6*30*24*60*60*1000L))
			{
				format.FullDateTimePattern = "MMM dd  yyyy";
			}
			else
			{
				format.FullDateTimePattern = "MMM dd hh:mm";
			}

			return SupportClass.ConvertFromEpochTime(mt).ToString("F", format);

		}
		
		private static System.String pad(int num)
		{
			System.String str = "";
			
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					str += " ";
				}
			}
			
			return str;
		}

		/// <summary>
		/// Copy the contents of a remote directory to a local directory 
		/// </summary>
		/// <param name="remotedir">the remote directory whose contents will be copied.</param>
		/// <param name="localdir">the local directory to where the contents will be copied</param>
		/// <param name="recurse">recurse into child folders</param>
		/// <param name="sync">synchronized the directories by removing files and directories that do not exist on the remote server</param>
		/// <param name="commit">actually perform the operation. If false the operation will be processed and a DirectoryOperation will be returned without actually transfering any files</param>
		/// <param name="progress"></param>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public DirectoryOperation CopyRemoteDirectory(System.String remotedir, 
			System.String localdir, bool recurse, bool sync, bool commit, FileTransferProgress progress)
		{

			try 
			{
				// Create an operation object to hold the information
				DirectoryOperation op = new DirectoryOperation();
			
				// Record the previous working directoies
				System.String pwd = Pwd();
				//String lpwd = lpwd();
				Cd(remotedir);
			
				// Setup the local cwd
				System.String remotefile = remotedir;
				int idx = remotefile.LastIndexOf((System.Char) '/');
			
				if (idx != - 1)
				{
					remotefile = remotefile.Substring(idx + 1);
				}
			
				System.IO.FileInfo local = new System.IO.FileInfo(localdir + "\\" + remotefile);
			
				if (!System.IO.Path.IsPathRooted(localdir))
				{
					local = new System.IO.FileInfo(Lpwd() + "\\" + localdir);
				}

				if (!System.IO.Directory.Exists(local.FullName) && commit)
				{
					System.IO.Directory.CreateDirectory(local.FullName);
				}
			
				SFTPFile[] files = Ls();
				SFTPFile file;
				System.IO.FileInfo f;
			
				for (int i = 0; i < files.Length; i++)
				{
					file = files[i];
				
					if (file.IsDirectory && !file.Filename.Equals(".") && !file.Filename.Equals(".."))
					{
						if (recurse)
						{
							f = new System.IO.FileInfo(local.FullName + "\\" + file.Filename);
							op.AddDirectoryOperation(CopyRemoteDirectory(file.Filename, local.FullName, recurse, sync, commit, progress), f);
						}
					}
					else if (file.IsFile)
					{
						f = new System.IO.FileInfo(local.FullName + "\\" + file.Filename);
					
						if (System.IO.File.Exists(f.FullName)
							&& (SupportClass.FileLength(f) == file.Attributes.Size) 
							&& ((f.LastWriteTime.Ticks / 1000) == file.Attributes.ModifiedTime))
						{
							if (commit)
							{
								op.UnchangedFiles.Add(f);
							}
							else
							{
								op.UnchangedFiles.Add(file);
							}
						
							continue;
						}
					
						if (System.IO.File.Exists(f.FullName))
						{
							if (commit)
							{
								op.UpdatedFiles.Add(f);
							}
							else
							{
								op.UpdatedFiles.Add(file);
							}
						}
						else
						{
							if (commit)
							{
								op.NewFiles.Add(f);
							}
							else
							{
								op.NewFiles.Add(file);
							}
						}
					
						if (commit)
						{
							SFTPFileAttributes attrs = GetFile(file.Filename, f.FullName, false);
						}
					}
				}

				if (sync)
				{
					// List the contents of the new local directory and remove any
					// files/directories that were not updated
					System.String[] contents = System.IO.Directory.GetFileSystemEntries(local.FullName);
					System.IO.FileInfo f2;
					if (contents != null)
					{
						for (int i = 0; i < contents.Length; i++)
						{
							f2 = new System.IO.FileInfo(contents[i]);
							if (!op.ContainsFile(f2))
							{
								op.DeletedFiles.Add(f2);
							
								if (System.IO.Directory.Exists(f2.FullName) && !f2.Name.Equals(".") && !f2.Name.Equals(".."))
								{
									RecurseMarkForDeletion(f2, op);
								
									if (commit)
									{
										RecurseDeleteDirectory(f2);
									}
								}
								else if (commit)
								{
									System.IO.File.Delete(f2.FullName);
								}
							}
						}
					}
				}
			
				Cd(pwd);
			
				return op;
			} 
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
		}

		/// <summary>
		/// Copy the contents of a local directory into a remote remote directory.
		/// </summary>
		/// <param name="localdir">the path to the local directory</param>
		/// <param name="remotedir">the remote directory which will receive the contents</param>
		/// <param name="recurse">recurse through child folders</param>
		/// <param name="sync">synchronize the directories by removing files on the remote server that do not exist locally</param>
		/// <param name="commit">actually perform the operation. If false a <see cref="Maverick.SFTP.DirectoryOperation"/> will 
		/// be returned so that the operation can be evaluated and no actual files will be created/transfered.</param>
		/// <param name="progress"></param>
		/// <returns></returns>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public DirectoryOperation CopyLocalDirectory(System.String localdir, System.String remotedir, bool recurse, bool sync, bool commit, FileTransferProgress progress)
		{

			try
			{
				DirectoryOperation op = new DirectoryOperation();
			
				System.IO.FileInfo local = ResolveLocalPath(localdir);
				remotedir = FormatRemotePath(remotedir);

				remotedir = ResolveRemotePath(remotedir);
				remotedir += (remotedir.EndsWith("/")?"":"/");
				remotedir += local.Name;
				remotedir += (remotedir.EndsWith("/")?"":"/");
			
				// Setup the remote directory if were committing
				if (commit)
				{
					try
					{
						Stat(remotedir);
					}
					catch (SFTPStatusException)
					{
						Mkdirs(remotedir);
					}
				}
			
				// List the local files and verify against the remote server
				System.String[] ls = System.IO.Directory.GetFileSystemEntries(local.FullName);
				System.IO.FileInfo source;
				if (ls != null)
				{
					for (int i = 0; i < ls.Length; i++)
					{
						source = new System.IO.FileInfo(ls[i]);
						if (System.IO.Directory.Exists(source.FullName) && !source.Name.Equals(".") && !source.Name.Equals(".."))
						{
							if (recurse)
							{
								//File f = new File(local, source.getName());
								op.AddDirectoryOperation(CopyLocalDirectory(source.FullName, remotedir, recurse, sync, commit, progress), source);
							}
						}
						else if (System.IO.File.Exists(source.FullName))
						{
							try
							{
								SFTPFileAttributes attrs = Stat(remotedir + source.Name);
							
								if ((SupportClass.FileLength(source) == attrs.Size) 
									&& ((source.LastWriteTime.Ticks / 1000) == attrs.ModifiedTime))
								{
									op.UnchangedFiles.Add(source);
								}
								else
								{
									op.UpdatedFiles.Add(source);
								}
							}
							catch(SFTPStatusException)
							{
								op.NewFiles.Add(source);
							}
						
							if (commit)
							{
								PutFile(source.FullName, remotedir + source.Name, false);
								SFTPFileAttributes attrs = Stat(remotedir + source.Name);
								attrs.SetTimes((uint)source.LastWriteTime.Ticks / 1000, (uint)source.LastWriteTime.Ticks / 1000);
								sftp.SetAttributes(remotedir + source.Name, attrs);
							}
						}
					}
				}
			
				if (sync)
				{
					// List the contents of the new local directory and remove any
					// files/directories that were not updated
					try
					{
						SFTPFile[] files = Ls(remotedir);
						SFTPFile file;
					
						System.IO.FileInfo f;
					
						for (int i = 0; i < files.Length; i++)
						{
							file = files[i];
						
							// Create a local file object to test for its existence
							f = new System.IO.FileInfo(local.FullName + "\\" + file.Filename);
						
							if (!op.ContainsFile(f) && !file.Filename.Equals(".") && !file.Filename.Equals(".."))
							{
								op.DeletedFiles.Add(file);
							
								if (commit)
								{
									if (file.IsDirectory)
									{
										// Recurse through the directory, deleting stuff
										RecurseMarkForDeletion(file, op);
									
										if (commit)
										{
											Rm(file.AbsolutePath, true, true);
										}
									}
									else if (file.IsFile)
									{
										Rm(file.AbsolutePath);
									}
								}
							}
						}
					}
					catch(SFTPStatusException)
					{
						// Ignore since if it does not exist we cant delete it
					}
				}
			
				// Return the operation details
				return op;
			} 
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
		}

		private void RecurseMarkForDeletion(SFTPFile file, DirectoryOperation op)
		{
			SFTPFile[] list = Ls(file.AbsolutePath);
			op.DeletedFiles.Add(file);
			
			for (int i = 0; i < list.Length; i++)
			{
				file = list[i];
				
				if (file.IsDirectory && !file.Filename.Equals(".") && !file.Filename.Equals(".."))
				{
					RecurseMarkForDeletion(file, op);
				}
				else if (file.IsFile)
				{
					op.DeletedFiles.Add(file);
				}
			}
		}
		
		private void  RecurseMarkForDeletion(System.IO.FileInfo file, DirectoryOperation op)
		{
			System.String[] list = System.IO.Directory.GetFileSystemEntries(file.FullName);
			op.DeletedFiles.Add(file);
			
			if (list != null)
			{
				for (int i = 0; i < list.Length; i++)
				{
					file = new System.IO.FileInfo(list[i]);
					
					if (System.IO.Directory.Exists(file.FullName) && !file.Name.Equals(".") && !file.Name.Equals(".."))
					{
						RecurseMarkForDeletion(file, op);
					}
					else if (System.IO.File.Exists(file.FullName))
					{
						op.DeletedFiles.Add(file);
					}
				}
			}
		}

		/// <summary>
		/// Recurse through a directory and delete all files and child directories.
		/// </summary>
		/// <param name="dir"></param>
		/// <exception cref="Maverick.SFTP.SFTPStatusException"/>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public static void RecurseDeleteDirectory(System.IO.FileInfo dir)
		{
			
			System.String[] files = System.IO.Directory.GetFileSystemEntries(dir.FullName);
			
			if (files == null)
			{
				return ; // Directory could not be read
			}
			
			for (int i = 0; i < files.Length; i++)
			{
				System.IO.FileInfo f = new System.IO.FileInfo(dir.FullName + "\\" + files[i]);
				
				if (System.IO.Directory.Exists(f.FullName))
				{
					RecurseDeleteDirectory(f);
				}
				else if (System.IO.File.Exists(f.FullName))
				{
					System.IO.File.Delete(f.FullName);
				}
			}
			
			System.IO.Directory.Delete(dir.FullName);
			
		}


		private Exception ProcessException(Exception ex) 
		{
			if(ex is SSHException || ex is SFTPStatusException)
				return ex;
			else
				return new SSHException(ex.Message,
					SSHException.INTERNAL_ERROR);

		}
	}
}
