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
using System.Text;
using Maverick.SSH;
using Maverick.Events;
using Maverick.Crypto.IO;
using Maverick.Crypto.Util;

namespace Maverick.SCP
{
	/// <summary>
	/// Provides an implementation of the Secure Copy (SCP) client for use in .NET applications. Secure 
	/// Copy was the original method for transfering files using the SSH1 protocol and was designed to 
	/// replace the insecure rcp command. It has since been superceeded by the <see cref="Maverick.SFTP">SFTP</see> 
	/// protocol but is still supported by both SSH1 and SSH2 servers alike.
	/// </summary>
	internal class NamespaceDoc
	{
	}

	/// <summary>
	/// SCP (Secure Copy) is used to copy files over the network securely. It is probably the simplest way to 
	/// copy a file into a remote machine
	/// </summary>
	/// <remarks>
	/// SCP was the original method for transfering files using the SSH1 protocol and was designed to replace
	/// the insecure rcp command. It has since been superceeded by the <see cref="Maverick.SFTP">SFTP</see> 
	/// protocol but is still supported by both SSH1 and SSH2 servers alike.
	/// 
	/// If your application must support transfering files to both SSH1 and SSH2 servers regardless of the 
	/// platform and SSH server involved then SCP will provide compatibility where SFTP may not.
	/// </remarks>
	public class SCPClient
	{
		private SSHClient ssh;

		/// <summary>
		/// Provides an event to track the progress of SCP transfers.
		/// </summary>
		public event FileTransferProgress TransferEvents;

		/// <summary>
		/// Construct an SCP client instance.
		/// </summary>
		/// <param name="ssh">An initialized and authenticated SSH client.</param>
		public SCPClient(SSHClient ssh)
		{
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Creating SCPClient");
#endif
			
			this.ssh = ssh;
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
        /// Transfer the local file or directory to the remote SSH server.
        /// </summary>
        /// <remarks>
        /// When transfering a directory the entire contents are recursively copied to the remote
        /// server. If for example you copy the local directory "C:\scp" and specify the remote
        /// folder "." then a folder called scp will be created on the remote filesystem with all
        /// the contents of the local directory.
        /// 
        /// When transfering files, you can either specify the remote directory or an alternative
        /// name and path for the file. 
        /// </remarks>
        /// <example>
        /// [This example assumes parameter ssh is an authenticated instance of SSHClient]
        /// <code>
        /// // Create an SCPClient from an authenticated SSHClient instance
        /// SCPClient scp = new SCPClient(ssh);
        /// 
        /// // Copy the contents of a local folder to a remote folder 
        /// scp.Put(new FileInfo("C:\\workspace"), ".");
        /// 
        /// // Copy a file from the local machime to a remote folder
        /// scp.Put(new FileInfo("C:\\workspace\\readme.txt"), ".");
        /// 
        /// // Copy a file from the local machine and rename on the remote machine
        /// scp.Put(new FileInfo("C:\\workspace\\readme.txt"), "sometext.txt");
        /// </code>
        /// </example>
        /// <param name="stream">A stream that contains the file data; the stream must report its full length in order to be transfered correctly.</param>
        /// <param name="remotefile">The location to place the copied file or directory</param>
        /// <exception cref="Maverick.SSH.SSHException"/>
        public void Put(Stream stream, String remotefile)
        {
            try
            {
                String cmd = "scp "
                    + "-t "
                    + remotefile;
#if DEBUG
                System.Diagnostics.Trace.WriteLine("Creating SCPEngine with command: " + cmd );
#endif
                SCPEngine scp = new SCPEngine(this, cmd, ssh.OpenSessionChannel());

                try
                {

#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Waiting for response to command");
#endif
                    scp.WaitForResponse();

#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Got response, writing file " + remotefile + " to " + remotefile );
#endif
                    scp.WriteStreamToRemote(stream, stream.Length, remotefile);

#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Write complete");
#endif
                }
                finally
                {
                    scp.Close();
                }
            }
            catch (Exception ex)
            {
                throw ProcessException(ex);
            }
        }


		/// <summary>
		/// Transfer the local file or directory to the remote SSH server.
		/// </summary>
		/// <remarks>
		/// When transfering a directory the entire contents are recursively copied to the remote
		/// server. If for example you copy the local directory "C:\scp" and specify the remote
		/// folder "." then a folder called scp will be created on the remote filesystem with all
		/// the contents of the local directory.
		/// 
		/// When transfering files, you can either specify the remote directory or an alternative
		/// name and path for the file. 
		/// </remarks>
		/// <example>
		/// [This example assumes parameter ssh is an authenticated instance of SSHClient]
		/// <code>
		/// // Create an SCPClient from an authenticated SSHClient instance
		/// SCPClient scp = new SCPClient(ssh);
		/// 
		/// // Copy the contents of a local folder to a remote folder 
		/// scp.Put(new FileInfo("C:\\workspace"), ".");
		/// 
		/// // Copy a file from the local machime to a remote folder
		/// scp.Put(new FileInfo("C:\\workspace\\readme.txt"), ".");
		/// 
		/// // Copy a file from the local machine and rename on the remote machine
		/// scp.Put(new FileInfo("C:\\workspace\\readme.txt"), "sometext.txt");
		/// </code>
		/// </example>
		/// <param name="localfile">The name of a local file or directory to copy.</param>
		/// <param name="remotefile">The location to place the copied file or directory</param>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Put(FileInfo localfile, String remotefile)
		{
			if(!File.Exists(localfile.FullName) && !Directory.Exists(localfile.FullName))
				throw new FileNotFoundException(localfile.FullName + " does not exist as either a file or directory!");

			bool recursive = Directory.Exists(localfile.FullName);

			try
			{
				String cmd = "scp " +
					(Directory.Exists(localfile.FullName) ? "-d " : "") 
					+ "-t "
					+ (recursive ? "-r " : "")
					+ remotefile;
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Creating SCPEngine with command: " + cmd );
#endif
				SCPEngine scp = new SCPEngine(this, cmd, ssh.OpenSessionChannel());

				try
				{

#if DEBUG
					System.Diagnostics.Trace.WriteLine("Waiting for response to command" );
#endif
					scp.WaitForResponse();

#if DEBUG
					System.Diagnostics.Trace.WriteLine("Got response, writing file " + localfile.FullName + " to " + remotefile + "" );
#endif
					scp.WriteFileToRemote(localfile, recursive);

#if DEBUG
					System.Diagnostics.Trace.WriteLine("Write complete");
#endif
				}
				finally
				{
					scp.Close();
				}
			} 
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}
		}



		/// <summary>
		/// Transfer the local file or directory to the remote SSH server.
		/// </summary>
		/// <remarks>
		/// When transfering a directory the entire contents are recursively copied to the remote
		/// server. If for example you copy the local directory "C:\scp" and specify the remote
		/// folder "." then a folder called scp will be created on the remote filesystem with all
		/// the contents of the local directory.
		/// 
		/// When transfering files, you can either specify the remote directory or an alternative
		/// name and path for the file. 
		/// </remarks>
		/// <example>
		/// [This example assumes parameter ssh is an authenticated instance of SSHClient]
		/// <code>
		/// // Create an SCPClient from an authenticated SSHClient instance
		/// SCPClient scp = new SCPClient(ssh);
		/// 
		/// // Copy the contents of a local folder to a remote folder 
		/// scp.Put("C:\\workspace", ".");
		/// 
		/// // Copy a file from the local machime to a remote folder
		/// scp.Put("C:\\workspace\\readme.txt", ".");
		/// 
		/// // Copy a file from the local machine and rename on the remote machine
		/// scp.Put("C:\\workspace\\readme.txt", "sometext.txt");
		/// </code>
		/// </example>
		/// <param name="localfile">The name of a local file or directory to copy.</param>
		/// <param name="remotefile">The location to place the copied file or directory</param>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Put(String localfile, String remotefile)
		{
			Put(new FileInfo(localfile), remotefile);
		}

		/// <summary>
		/// Transfer the local files or directories to the remote SSH server.
		/// </summary>
		/// <remarks>
		/// The local file list can be a mixture of files and directories. Because the local 
		/// files types can be evaluated the correct recursive mode is automatically set.
		/// </remarks>
		/// <example>
		/// [This example assumes parameter ssh is an authenticated instance of SSHClient]
		/// <code>
		/// // Create an SCPClient from an authenticated SSHClient instance
		/// SCPClient scp = new SCPClient(ssh);
		/// 
		/// FileInfo files = new FileInfo[2];
		/// files[0] = new FileInfo("C:\\workspace\\scp"); 
		/// files[1] = new FileInfo("C:\\workspace\\readme.txt");
		/// 
		/// // Copy the list of files the remote folder 
		/// scp.Put(files, ".");
		/// </code>
		/// </example>
		/// <param name="localfiles">A list of local files and directories.</param>
		/// <param name="remotefile">The directory in which to place the copied files.</param>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Put(FileInfo[] localfiles, String remotefile)
		{
			for(int i=0;i<localfiles.Length;i++)
				Put(localfiles[i], remotefile);
		}

		/// <summary>
		/// Transfer the local files or directories to the remote SSH server.
		/// </summary>
		/// <remarks>
		/// The local file list can be a mixture of files and directories. Because the local 
		/// files types can be evaluated the correct recursive mode is automatically set.
		/// </remarks>
		/// <example>
		/// [This example assumes parameter ssh is an authenticated instance of SSHClient]
		/// <code>
		/// // Create an SCPClient from an authenticated SSHClient instance
		/// SCPClient scp = new SCPClient(ssh);
		/// 
		/// String files = new String[2];
		/// files[0] = "C:\\workspace\\scp"; 
		/// files[1] = "C:\\workspace\\readme.txt";
		/// 
		/// // Copy the list of files the remote folder 
		/// scp.Put(files, ".");
		/// </code>
		/// </example>
		/// <param name="localfiles">A list of local files and directories.</param>
		/// <param name="remotefile">The directory in which to place the copied files.</param>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Put(String[] localfiles, String remotefile)
		{
			for(int i=0;i<localfiles.Length;i++)
				Put(localfiles[i], remotefile);
		}

		/// <summary>
		/// Transfer the remote file or directory to the local machine.
		/// </summary>
		/// <remarks>
		/// When transfering a directory the entire contents are recursively copied from the remote
		/// server. If for example you copy the remote directory "scp" and specify the local
		/// folder "C:\\" then a folder called "C:\\scp" will be created on the local filesystem with all
		/// the contents of the local directory.
		/// 
		/// When transfering files, you can either specify the local directory or an alternative
		/// name and path for the file. 
		/// </remarks>
		/// <example>
		/// [This example assumes parameter ssh is an authenticated instance of SSHClient]
		/// <code>
		/// // Create an SCPClient from an authenticated SSHClient instance
		/// SCPClient scp = new SCPClient(ssh);
		/// 
		/// // Get the contents of a directory to the local users home folder
		/// scp.Get(new FileInfo(SCPClient.GetHomeDirectory()), "docs", true);
		/// 
		/// // Get a remote file into a local directory
		/// scp.Get(new FileInfo("C:\\docs"), "docs/readme.txt", false);
		/// 
		/// // Get a remote file and rename on the local machine
		/// scp.Get(new FileInfo("C:\\docs\\sometext.txt"), "docs/readme.txt", false);
		/// </code>
		/// </example>
		/// <param name="localFile">The name of the local file or directory to copy to.</param>
		/// <param name="remoteFile">The name of the remote file or directory to copy</param>
		/// <param name="recursive">If the remote file is a directory you should pass a true value, otherwise pass false for a file.</param>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Get(FileInfo localFile, String remoteFile, bool recursive) 																				
		{
		
			try
			{
			
				String cmd = "scp " + "-f "
					+ (recursive ? "-r " : "")
					+ remoteFile;
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Creating SCPEngine with command: " + cmd );
#endif
				SCPEngine scp = new SCPEngine(this, cmd, ssh.OpenSessionChannel());

				try
				{
#if DEBUG
					System.Diagnostics.Trace.WriteLine("Reading file(s) from remote into " + localFile.FullName + "" );
#endif
					scp.ReadFromRemote(localFile);
#if DEBUG
					System.Diagnostics.Trace.WriteLine("Completed transfer into " + localFile.FullName + "" );
#endif
				}
				catch(Exception ex)
				{
                   throw ProcessException(ex);
				}
				finally
				{
					scp.Close();
				}

			} 
			catch(Exception ex)
			{
				throw ProcessException(ex);
			}


		}


        /// <summary>
        /// Transfer the remote file or directory to the local machine.
        /// </summary>
        /// <remarks>
        /// When transfering a directory the entire contents are recursively copied from the remote
        /// server. If for example you copy the remote directory "scp" and specify the local
        /// folder "C:\\" then a folder called "C:\\scp" will be created on the local filesystem with all
        /// the contents of the local directory.
        /// 
        /// When transfering files, you can either specify the local directory or an alternative
        /// name and path for the file. 
        /// </remarks>
        /// <example>
        /// [This example assumes parameter ssh is an authenticated instance of SSHClient]
        /// <code>
        /// // Create an SCPClient from an authenticated SSHClient instance
        /// SCPClient scp = new SCPClient(ssh);
        /// 
        /// // Get the contents of a directory to the local users home folder
        /// scp.Get(new FileInfo(SCPClient.GetHomeDirectory()), "docs", true);
        /// 
        /// // Get a remote file into a local directory
        /// scp.Get(new FileInfo("C:\\docs"), "docs/readme.txt", false);
        /// 
        /// // Get a remote file and rename on the local machine
        /// scp.Get(new FileInfo("C:\\docs\\sometext.txt"), "docs/readme.txt", false);
        /// </code>
        /// </example>
        /// <param name="stream">A stream to copy the local file to.</param>
        /// <param name="remoteFile">The name of the remote file or directory to copy</param>
        /// <exception cref="Maverick.SSH.SSHException"/>
        public Stream Get(Stream stream, String remoteFile)
        {

            try
            {

                String cmd = "scp " + "-f " + remoteFile;
#if DEBUG
                System.Diagnostics.Trace.WriteLine("Creating SCPEngine with command: " + cmd );
#endif
                SCPEngine scp = new SCPEngine(this, cmd, ssh.OpenSessionChannel());

                try
                {
#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Reading file(s) from remote into " + remoteFile );
#endif
                    return scp.ReadStreamFromRemote();
                }
                catch (Exception ex)
                {
                    throw ProcessException(ex);
                }
                finally
                {
                    scp.Close();
                }

            }
            catch (Exception ex)
            {
                throw ProcessException(ex);
            }


        }


		/// <summary>
		/// Transfer the remote file or directory to the local machine.
		/// </summary>
		/// <remarks>
		/// When transfering a directory the entire contents are recursively copied from the remote
		/// server. If for example you copy the remote directory "scp" and specify the local
		/// folder "C:\\" then a folder called "C:\\scp" will be created on the local filesystem with all
		/// the contents of the local directory.
		/// 
		/// When transfering files, you can either specify the local directory or an alternative
		/// name and path for the file. 
		/// </remarks>
		/// <example>
		/// [This example assumes parameter ssh is an authenticated instance of SSHClient]
		/// <code>
		/// // Create an SCPClient from an authenticated SSHClient instance
		/// SCPClient scp = new SCPClient(ssh);
		/// 
		/// // Get the contents of a directory to the local users home folder
		/// scp.Get(SCPClient.GetHomeDirectory(), "docs", true);
		/// 
		/// // Get a remote file into a local directory
		/// scp.Get("C:\\docs", "docs/readme.txt", false);
		/// 
		/// // Get a remote file and rename on the local machine
		/// scp.Get("C:\\docs\\sometext.txt", "docs/readme.txt", false);
		/// </code>
		/// </example>
		/// <param name="localfile">The name of the local file or directory to copy to.</param>
		/// <param name="remotefile">The name of the remote file or directory to copy</param>
		/// <param name="recursive">If the remote file is a directory you should pass a true value, otherwise pass false for a file.</param>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Get(String localfile, String remotefile, bool recursive)
		{
			Get(new FileInfo(localfile), remotefile, recursive);
		}

		/// <summary>
		/// Transfer a number of remote files or directories to the local machine
		/// </summary>
		/// <remarks>
		/// The list of remote files must either be a list of directories or a list of files. It
		/// is an error to mix files and directories because there is no method for the client
		/// to detect the file type of the remote file. If you are transfering directories remember
		/// to set the recursive parameter to true.
		/// </remarks>
		/// <example>
		/// [This example assumes parameter ssh is an authenticated instance of SSHClient]
		/// <code>
		/// // Create an SCPClient from an authenticated SSHClient instance
		/// SCPClient scp = new SCPClient(ssh);
		/// 
		/// // Create an array of directory names
		/// String[] files = new String[2];
		/// files[0] = "docs";
		/// files[1] = "src";
		/// 
		/// // Copy the contents of all the directories to the local users home folder
		/// scp.Get(new FileInfo(SCPClient.GetHomeDirectory()), files, true);
		/// </code>
		/// </example>
		/// <param name="localFile">The local directory where the files will be copied</param>
		/// <param name="remoteFiles">A list of remote files or directories</param>
		/// <param name="recursive">If the remote files are directories you should pass a true value, otherwise pass false for files.</param>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Get(FileInfo localFile, String[] remoteFiles, bool recursive)  
		{
			StringBuilder buf = new StringBuilder();

			for (int i = 0; i < remoteFiles.Length; i++) 
			{
				buf.Append("\"");
				buf.Append(remoteFiles[i]);
				buf.Append("\" ");
			}

			String remoteFile = buf.ToString();
			remoteFile = remoteFile.Trim();
			Get(localFile, remoteFile, recursive);
		}

		/// <summary>
		/// Transfer a number of remote files or directories to the local machine
		/// </summary>
		/// <remarks>
		/// The list of remote files must either be a list of directories or a list of files. It
		/// is an error to mix files and directories because there is no method for the client
		/// to detect the file type of the remote file. If you are transfering directories remember
		/// to set the recursive parameter to true.
		/// </remarks>
		/// <example>
		/// [This example assumes parameter ssh is an authenticated instance of SSHClient]
		/// <code>
		/// // Create an SCPClient from an authenticated SSHClient instance
		/// SCPClient scp = new SCPClient(ssh);
		/// 
		/// // Create an array of directory names
		/// String[] files = new String[2];
		/// files[0] = "docs";
		/// files[1] = "src";
		/// 
		/// // Copy the contents of all the directories to the local users home folder
		/// scp.Get(SCPClient.GetHomeDirectory(), files, true);
		/// </code>
		/// </example>
		/// <param name="localfile">The local directory where the files will be copied</param>
		/// <param name="remotefiles">A list of remote files or directories</param>
		/// <param name="recursive">If the remote files are directories you should pass a true value, otherwise pass false for files.</param>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public void Get(String localfile, String[] remotefiles, bool recursive)
		{	
			Get(new FileInfo(localfile), remotefiles, recursive);
		}

		/// <summary>
		/// Utility method to return the local users home directory on the local machine. 
		/// </summary>
		/// <remarks>
		/// This returns the users profile directory, for example if the user "lee" is currently logged
		/// on then this will return "C:\Documents and settings\lee".
		/// </remarks>
		/// <returns>The local home directory of the currently logged on user.</returns>
		/// <exception cref="Maverick.SSH.SSHException"/>
		public static String GetHomeDirectory() 
		{
			return SupportClass.GetHomeDir();
		}

		private Exception ProcessException(Exception ex)
		{
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Encountered Exception: " + ex.Message + "" );
			System.Diagnostics.Trace.WriteLine(ex.StackTrace );
#endif
			if(ex is SSHException)
				return ex;
			else
				return new SSHException(ex.Message, SSHException.INTERNAL_ERROR);
		}		
	}


	internal class SCPEngine
	{
		byte[] buffer = new byte[16384];
		String cmd;
		SSHSession session;
		Stream stream;
		SCPClient client;

		internal SCPEngine(SCPClient client, String cmd, SSHSession session)
		{
			this.client = client;
			this.session = session;
			this.cmd = cmd;
			this.stream = session.GetStream();

			if(!session.ExecuteCommand(cmd)) 
			{
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Server failed to execute command" );
#endif
				session.Close();
				throw new SSHException("Failed to execute the command " + cmd,
					SSHException.SCP_ERROR);
			}
#if DEBUG
			System.Diagnostics.Trace.WriteLine("Server reported command was executed" );
#endif
		}

		internal void Close()
		{
			if(session!=null)
				session.Close();
		}

		internal bool WriteDirToRemote(FileInfo dir, bool recursive) 
		{
			try 
			{
				if(!recursive) 
				{
					WriteError("File " + dir.FullName
						+ " is a directory, use recursive mode");

					return false;
				}

				String cmd = "D0755 0 " + dir.Name + "\n";
				byte[] b = ByteBuffer.ConvertAsciiStringToBytes(cmd);
				stream.Write(b, 0, b.Length);

#if DEBUG
				System.Diagnostics.Trace.WriteLine("Writing command: " + cmd + "" );
				System.Diagnostics.Trace.WriteLine("Waiting for response" );
#endif
				WaitForResponse();

#if DEBUG
				System.Diagnostics.Trace.WriteLine("Got response" );
#endif

				String[] list = System.IO.Directory.GetFileSystemEntries(dir.FullName);

				for(int i = 0; i < list.Length; i++) 
				{
					FileInfo f = new FileInfo(list[i]);
					WriteFileToRemote(f, recursive);
				}

				cmd = "E\n";
				b = ByteBuffer.ConvertAsciiStringToBytes(cmd);

#if DEBUG
				System.Diagnostics.Trace.WriteLine("Writing command: " + cmd + "" );
#endif				
				stream.Write(b, 0,b.Length);

				return true;
			}
			catch(IOException ex) 
			{
				Close();
				throw new SSHException(ex.Message,
					SSHException.SCP_ERROR);
			}
		}

		internal void WriteFileToRemote(FileInfo file, bool recursive) 
		{
            bool writtenFile = false;

			if(file.Attributes == FileAttributes.Directory) 
			{
				if(!WriteDirToRemote(file, recursive)) 
				{
					return;
				}
			}
			else 
			{

                try
                {
                    String cmd = "C0644 " + file.Length + " " + file.Name
                        + "\n";

                    byte[] b = ByteBuffer.ConvertAsciiStringToBytes(cmd);

#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Writing command: " + cmd );
#endif
                    stream.Write(b, 0, b.Length);
#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Waiting for response");
#endif
                    WaitForResponse();

#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Got response");
#endif
                    if (client.FireTransferEvent(FileTransferState.TRANSFER_STARTED, file.FullName, file.Length))
                    {

                        FileStream input = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);

#if DEBUG
                        System.Diagnostics.Trace.WriteLine("Writing file");
#endif
                        if (WriteCompleteFile(input, file.Length, file.Name))
                        {
                            writtenFile = true;
                            client.FireTransferEvent(FileTransferState.TRANSFER_COMPLETED, file.FullName, file.Length);
#if DEBUG
                            System.Diagnostics.Trace.WriteLine("Writing file completed, sending OK");
#endif
                            WriteOk();

#if DEBUG
                            System.Diagnostics.Trace.WriteLine("Waiting for response");
#endif
                            WaitForResponse();

#if DEBUG
                            System.Diagnostics.Trace.WriteLine("Got response");
#endif
                            
                        }
                        else
                        {
#if DEBUG
                            System.Diagnostics.Trace.WriteLine("User cancelled transfer");
#endif
                            client.FireTransferEvent(FileTransferState.TRANSFER_CANCELLED, file.FullName, 0);
                            Close();
                        }


                    }
                    else
                    {
#if DEBUG
                        System.Diagnostics.Trace.WriteLine("User cancelled transfer");
#endif
                        client.FireTransferEvent(FileTransferState.TRANSFER_CANCELLED, file.FullName, 0);
                    }
                }
                catch (EndOfStreamException ex)
                {
                    if (!writtenFile)
                    {
                        client.FireTransferEvent(FileTransferState.TRANSFER_ERROR, file.FullName, 0);
                        throw ex;
                    }
                }
                catch (IOException ex)
                {
#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Received IO error: " + ex.Message );
#endif
                    client.FireTransferEvent(FileTransferState.TRANSFER_ERROR, file.FullName, 0);

                    Close();
                    throw new SSHException(ex.Message,
                        SSHException.SCP_ERROR);
                }
			}
				
		}

		internal bool WriteStreamToRemote(Stream input, 
			long length,
			String filename) 
		{

            bool writtenFile = false;

            try
            {
                String cmd = "C0644 " + length + " " + filename + "\n";
                byte[] b = ByteBuffer.ConvertAsciiStringToBytes(cmd);

#if DEBUG
                System.Diagnostics.Trace.WriteLine("Writing command: " + cmd );
#endif
                stream.Write(b, 0, b.Length);

#if DEBUG
                System.Diagnostics.Trace.WriteLine("Waiting for response");
#endif

                WaitForResponse();

#if DEBUG
                System.Diagnostics.Trace.WriteLine("Got response");
#endif

                if (client.FireTransferEvent(FileTransferState.TRANSFER_STARTED, filename, length))
                {

#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Writing file");
#endif
                    WriteCompleteFile(input, length, filename);
                    writtenFile = true;
                    client.FireTransferEvent(FileTransferState.TRANSFER_COMPLETED, filename, length);

#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Writing file complete, sending OK");
#endif
                    WriteOk();

#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Waiting for response");
#endif
                    WaitForResponse();

#if DEBUG
                    System.Diagnostics.Trace.WriteLine("Got response");
#endif

                    return true;
                }
                else
                {
#if DEBUG
                    System.Diagnostics.Trace.WriteLine("User cancelled transfer");
#endif
                    client.FireTransferEvent(FileTransferState.TRANSFER_CANCELLED, filename, 0);
                    return false;
                }
            }
            catch (EndOfStreamException ex)
            {
                if (!writtenFile)
                {
                    client.FireTransferEvent(FileTransferState.TRANSFER_ERROR, filename, 0);
                    throw ex;
                }
                return true;
            }
            catch (Exception ex)
            {
                client.FireTransferEvent(FileTransferState.TRANSFER_ERROR, filename, 0);
                throw ex;
            }
		}

		internal Stream ReadStreamFromRemote()
		{

#if DEBUG
			System.Diagnostics.Trace.WriteLine("Reading from remote" );
#endif
			String cmd;
			String[] cmdParts = new String[3];

#if DEBUG
			System.Diagnostics.Trace.WriteLine("Writing OK" );
#endif
			WriteOk();

			try 
			{
				cmd = ReadString();

#if DEBUG
				System.Diagnostics.Trace.WriteLine("Got command: " + cmd + "" );
#endif
			}
			catch (EndOfStreamException) 
			{
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Stream is EOF" );
#endif
				return null;
			}

			char cmdChar = cmd[0];

			while(true)
			{
				switch (cmdChar) 
				{
					case 'E':
					{
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Writing OK" );
#endif
						WriteOk();
						return null;
					}
					case 'T':
					{
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Received time" );
#endif
						continue;
					}
					case 'D':
					{
						throw new SSHException(
							"Directories cannot be copied to a stream",
							SSHException.SCP_ERROR);
					}
					case 'C':
					{
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Parsing command" );
#endif
						ParseCommand(cmd, cmdParts);
#if DEBUG
						System.Diagnostics.Trace.WriteLine("Writing OK" );
#endif
						WriteOk();

						long len = Int64.Parse(cmdParts[1]);

						return new SCPStream(len, stream, this);
					}
					default:
					{
						WriteError("Unexpected cmd: " + cmd);
						throw new SSHException("SCP unexpected cmd: " + cmd, 
							SSHException.BAD_API_USAGE);
					}
				}
			}
		}

		internal void ParseCommand(String cmd, String[] cmdParts)
		{
			int l;
			int r;
			l = cmd.IndexOf(' ');
			r = cmd.IndexOf(' ', l + 1);

			if ((l == -1) || (r == -1)) 
			{
				WriteError("Syntax error in cmd");
				throw new SSHException("Syntax error in cmd", SSHException.SCP_ERROR);
			}

			cmdParts[0] = cmd.Substring(1, l);
			cmdParts[1] = cmd.Substring(l + 1, r - (l+1));
			cmdParts[2] = cmd.Substring(r + 1);
		}

		internal String ReadString() 
		{
			int ch;
			int i = 0;

			while ( ( (ch = Read()) != ('\n')) && (ch >= 0)) 
			{
				buffer[i++] = (byte) ch;
			}

			if (ch == -1) 
			{
				throw new EndOfStreamException("Unexpected EOF");
			}

			if (buffer[0] == (byte) '\n') 
			{
				throw new SSHException("Unexpected <NL>", SSHException.SCP_ERROR);
			}

			if ( (buffer[0] == (byte) '\x2') || (buffer[0] == (byte) '\x1')) 
			{
				String msg = ByteBuffer.ConvertBytesToAsciiString(buffer, 1, i-1);

				if (buffer[0] == (byte) '\x2') 
				{
					throw new SSHException(msg, SSHException.SCP_ERROR);
				}

				throw new SSHException("SCP returned an unexpected error: "
					+ msg, SSHException.SCP_ERROR);
			}

			return ByteBuffer.ConvertBytesToAsciiString(buffer, 0, i);
		}

		int Read()
		{

			byte[] b = new byte[1];
			if(stream.Read(b, 0, 1) != 1)
				throw new EndOfStreamException("SCP returned unexpected EOF");

			return (int) b[0];
		}

		void Write(int i)
		{
			byte[] b = new byte[] { (byte) i };
			stream.Write(b, 0, 1);
		}

		internal void WaitForResponse() 
		{
			int r = Read();

			if (r == 0) 
			{
				// All is well, no error
				return;
			}

			if (r == -1) 
			{
				throw new EndOfStreamException("SCP returned unexpected EOF");
			}

			String msg = ReadString();

			if (r == (byte) '\x2') 
			{
				throw new SSHException(msg, SSHException.SCP_ERROR);
			}

			throw new SSHException("SCP returned an unexpected error: " + msg, SSHException.SCP_ERROR);
		}

		internal void WriteOk()
		{
			Write(0);
		}

		internal void WriteError(String reason)
		{
#if DEBUG
			System.Diagnostics.Trace.WriteLine("SCP Error: " + reason + "" );
#endif
			Write(1);
			byte[] b = ByteBuffer.ConvertAsciiStringToBytes(reason);
			stream.Write(b, 0, b.Length);
		}

		internal bool WriteCompleteFile(Stream input, long size, String filename)  
		{
			long count = 0;
			int read;

			try 
			{
				while (count < size) 
				{
					read = input.Read(buffer, 0,(int)(((size - count) < buffer.Length)
						? (size - count) : buffer.Length));

					if (read == 0) 
					{
						throw new EndOfStreamException("SCP received an unexpected EOF");
					}

					count += read;
					stream.Write(buffer, 0, read);

					// If the transfer is cancelled return
					if(!client.FireTransferEvent(FileTransferState.TRANSFER_PROGRESSED,
						filename, 
						count))
					  return false;

				}

				return true;
			}
			finally 
			{
				input.Close();
			}
		}

		internal void ReadFromRemote(FileInfo file) 
		{
            bool readFile = false;

            try
            {
                String cmd;
                String[] cmdParts = new String[3];

#if DEBUG
                System.Diagnostics.Trace.WriteLine("Writing OK");
#endif
                WriteOk();

                while (true)
                {
                    try
                    {
                        cmd = ReadString();
#if DEBUG
                        System.Diagnostics.Trace.WriteLine("Remote SCP command: " + cmd );
#endif
                    }
                    catch (EndOfStreamException)
                    {
                        return;
                    }

                    char cmdChar = cmd[0];

                    switch (cmdChar)
                    {
                        case 'E':

#if DEBUG
                            System.Diagnostics.Trace.WriteLine("Writing OK");
#endif
                            WriteOk();

                            return;

                        case 'T':
                            break;

                        //throw new SSHException("SCP time not supported: " + cmd,
                        //	SSHException.SCP_ERROR);

                        case 'C':
                        case 'D':

                            readFile = false;

                            String targetName = file.FullName;
                            ParseCommand(cmd, cmdParts);

                            if (Directory.Exists(targetName))
                            {
                                targetName += ("\\" + cmdParts[2]);
                            }

                            FileInfo targetFile = new FileInfo(targetName);

                            if (cmdChar == 'D')
                            {
                                if (targetFile.Exists)
                                {
                                    if (targetFile.Attributes != FileAttributes.Directory)
                                    {
                                        String msg = "Invalid target "
                                            + targetFile.Name
                                            + ", must be a directory";
                                        WriteError(msg);
                                        throw new SSHException(msg, SSHException.SCP_ERROR);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        Directory.CreateDirectory(targetFile.FullName);
                                    }
                                    catch
                                    {
                                        String msg = "Could not create directory: "
                                                            + targetFile.Name;
                                        WriteError(msg);
                                        throw new SSHException(msg, SSHException.SCP_ERROR);
                                    }
                                }

                                ReadFromRemote(targetFile);
                                readFile = true;

                                continue;
                            }

                            FileStream output = new FileStream(targetFile.FullName, FileMode.OpenOrCreate,
                                FileAccess.Write);

                            WriteOk();

                            long len = Int64.Parse(cmdParts[1]);

                            if (client.FireTransferEvent(FileTransferState.TRANSFER_STARTED, targetFile.FullName, len))
                            {

                                try
                                {
                                    if (ReadCompleteFile(output, len, targetFile))
                                    {
                                        readFile = true;
                                        WaitForResponse();
                                        WriteOk();

                                        client.FireTransferEvent(FileTransferState.TRANSFER_COMPLETED, targetFile.FullName, len);
                                    }
                                    else
                                    {
                                        client.FireTransferEvent(FileTransferState.TRANSFER_CANCELLED, targetFile.FullName, 0);
                                    }
                                }
                                catch (EndOfStreamException ex)
                                {
                                    if (!readFile)
                                    {
                                        client.FireTransferEvent(FileTransferState.TRANSFER_ERROR, targetFile.FullName, 0);
                                        throw ex;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    client.FireTransferEvent(FileTransferState.TRANSFER_ERROR, targetFile.FullName, 0);
                                    throw ex;
                                }
                            }

                            break;

                        default:
                            WriteError("Unexpected cmd: " + cmd);
                            throw new SSHException("SCP unexpected cmd: " + cmd,
                                SSHException.SCP_ERROR);
                    }
                }
            }
            catch (EndOfStreamException ex)
            {
                if (!readFile)
                {
                    client.FireTransferEvent(FileTransferState.TRANSFER_ERROR, file.Name, 0);
                    throw ex;
                }
            }
            catch (IOException ex)
            {
                Close();
                throw new SSHException(ex.Message,
                    SSHException.SCP_ERROR);
            }
		}

		internal bool ReadCompleteFile(Stream output, long size, FileInfo localfile) 
		{
			long count = 0;
			int read;

			try 
			{
				while (count < size) 
				{
					read = stream.Read(buffer, 0,
						(int)(((size - count) < buffer.Length)
						? (size - count) : buffer.Length));

					if (read == 0) 
					{
						throw new EndOfStreamException("SCP received an unexpected EOF");
					}

					count += read;
					output.Write(buffer, 0, read);

					if(!client.FireTransferEvent(FileTransferState.TRANSFER_PROGRESSED, localfile.FullName, count))
						return false;
				}

				return true;
			}
			finally 
			{
				output.Close();
			}
		}


	}

	class SCPStream : Stream
	{
		long size;
		long position;
		Stream source;
		SCPEngine engine;
		internal SCPStream(long size, Stream source, SCPEngine engine)
		{
			this.size = size;
			this.source = source;
			this.engine = engine;
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override void Flush()
		{
		}

		public override long Length
		{
			get
			{
				return size;
			}
		}

		public override long Position
		{
			get
			{
				return position;
			}

			set
			{
				throw new IOException("Position cannot be set on non seekable SCPStream");
			}
		}


		public override int Read(byte[] b, int off, int len)
		{
			if (position == size) 
			{
				return 0;
			}

			if (position >= size) 
			{
				throw new EndOfStreamException("End of file.");
			}

			int num = (int) (size - position > len ? len : size - position);
			
			int r = source.Read(b, off, num);

			if(r == 0) 
			{
				throw new EndOfStreamException("Unexpected EOF.");
			}

			position += num;

			if (position == size) 
			{
				engine.WaitForResponse();
				engine.WriteOk();
			}

			return r;

		}

		public override void Close()
		{
			engine.Close();
		}


		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			throw new IOException("Seek operation not permitted on SCPStream");
		}

		public override void SetLength(long length)
		{
			throw new IOException("SetLength operation not permitted on SCPStream");
		}

		public override void Write(byte[] b, int off, int len)
		{
			throw new IOException("Write operation not permitted on SCPStream");
		}


	}
}
