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
using Maverick.Crypto.IO;
using Maverick.SSH.Packets;

namespace Maverick.SSH2
{

	/// <summary>
	/// This class represents a single prompt in the <em>keyboard-interactive</em>
	/// authentication mechanism.
	/// </summary>
	/// <remarks>
	/// Each challege-response may contain several prompts and the information should be 
	/// displayed to the user and their answer set in the <see cref="Maverick.SSH2.KBIPrompt.Response"/>
	/// property.
	/// </remarks>
	public class KBIPrompt 
	{
		String prompt;
		String response;
		bool echo;

		internal KBIPrompt(String prompt, bool echo) 
		{
			this.prompt = prompt;
			this.echo = echo;
		}

		/// <summary>
		/// The users response to the prompt.
		/// </summary>
		public String Response 
		{
			set
			{
				this.response = value;
			}
			get
			{
				return response;
			}
		}

		/// <summary>
		/// The prompt which should be displayed to the user.
		/// </summary>
		public String Prompt
		{
			get
			{
				return prompt;
			}
		}

		/// <summary>
		/// Indicates whether the users answer should be echo'd back on screen.
		/// </summary>
		public bool Echo
		{
			get
			{
				return echo;
			}
		}

	}

	/// <summary>
	/// A delegate that received requests to prompt the user for a challege-response.
	/// </summary>
	public delegate bool ShowAuthenticationPrompts(String name,
									String instruction,
									KBIPrompt[] prompts);
	/// <summary>
	/// This class implements the SSH2 <em>keyboard-interactive</em> authentication
	/// method. 
	/// </summary>
	/// <remarks>
	/// This method is suitable for	interactive authentication methods which do not need any special
	/// software support on the client side.  Instead all authentication data
	/// should be entered via the keyboard.  The major goal of this method is
	/// to allow the SSH client to have little or no knowledge of the
	/// specifics of the underlying authentication mechanism(s) used by the
	/// SSH server.  This will allow the server to arbitrarily select or
	/// change the underlying authentication mechanism(s) without having to
	/// update client code.
	/// </remarks>
	public class KBIAuthentication : SSH2AuthenticationClient, SSHAuthentication
	{
		String username;
		const int SSH_MSG_USERAUTH_INFO_REQUEST = 60;
		const int SSH_MSG_USERAUTH_INFO_RESPONSE = 61;

		/// <summary>
		/// This event allows the <em>keyboard-interactive</em> authentication
		/// mechanism to request a response from the user.
		/// </summary>
		public event ShowAuthenticationPrompts InteractivePrompt;

		/// <summary>
		/// Construct an unintialized authentication instance.
		/// </summary>
		public KBIAuthentication()
		{
		}

		#region SSH2AuthenticationClient Members

		/// <summary>
		/// Start the authentication request.
		/// </summary>
		/// <param name="authentication">The authentication protocol instance.</param>
		/// <param name="serviceName">The name of the service to start upon a successful authentication.</param>
		public void Authenticate(AuthenticationProtocol authentication, String serviceName)
		{
			
			if(InteractivePrompt==null)
				throw new SSHException("An interactive prompt event must be set!",
					SSHException.BAD_API_USAGE);

			ByteBuffer baw = new ByteBuffer();
			baw.WriteString("");
			baw.WriteString("");

			authentication.SendRequest(username,
				serviceName,
				"keyboard-interactive",
				baw.ToByteArray());

			while(true)
			{
				SSHPacket msg = authentication.ReadMessage();
				if(msg.MessageID != SSH_MSG_USERAUTH_INFO_REQUEST)
				{
					authentication.transport.Disconnect("Unexpected authentication message received!",
						DisconnectionReason.PROTOCOL_ERROR);
					throw new SSHException("Unexpected authentication message received!",
						SSHException.PROTOCOL_VIOLATION);
				}

				String name = msg.ReadString();
				String instruction = msg.ReadString();
				String langtag = msg.ReadString();

				int num = (int)msg.ReadInt();
				String prompt;
				bool echo;
				KBIPrompt[] prompts = new KBIPrompt[num];
				for(int i = 0; i < num; i++) 
				{
					prompt = msg.ReadString();
					echo = (msg.ReadBool());
					prompts[i] = new KBIPrompt(prompt, echo);
				}

				if(InteractivePrompt(name, instruction, prompts))
				{
					msg = authentication.transport.GetSSHPacket(true);
					msg.WriteByte(SSH_MSG_USERAUTH_INFO_RESPONSE);
					msg.WriteInt(prompts.Length);

					for(int i = 0; i < prompts.Length; i++) 
					{
						msg.WriteString(prompts[i].Response);
					}

					authentication.transport.SendMessage(msg);
				}
				else
				{
					throw new SSHException("User cancelled during authentication",
						SSHException.USER_CANCELATION);
				}
			}

			

		}

		#endregion

		#region SSHAuthentication Members

		/// <summary>
		/// The username for this authentication.
		/// </summary>
		public String Username
		{
			get
			{
				return username;
			}
			set
			{
				this.username = value;
			}
		}

		#endregion
	}
}
