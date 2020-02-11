using System;
using System.IO;
using Maverick.Crypto.Util;

namespace Maverick.HTTP
{
	/// <summary>
	/// Defines the properties of a HTTP response.
	/// </summary>
	public class HTTPResponse : HTTPHeader
	{
		private String version;
		private int status;
		private String reason;

		/// <summary>
		/// Build the HTTP response from the stream.
		/// </summary>
		/// <param name="input"></param>
		public HTTPResponse(Stream input)
		{
			begin = ReadLine(input);

			while(begin.Trim().Length == 0) 
			{
				begin = ReadLine(input);
			}

			ProcessResponse();
			ProcessHeaderFields(input);		
		}

		/// <summary>
		/// The HTTP version of this response.
		/// </summary>
		public String Version
		{
			get
			{
				return version;
			}

		}

		/// <summary>
		/// The HTTP status code.
		/// </summary>
		public int Status
		{
			get
			{
				return status;
			}
		}

		/// <summary>
		/// The descriptive reason.
		/// </summary>
		public String Reason
		{
			get 
			{
				return reason;
			}
		}

		private void ProcessResponse()
		{
			
			SupportClass.Tokenizer tokens = new SupportClass.Tokenizer(begin, " \r\t");

			try 
			{
				version = tokens.NextToken();
				status = Int32.Parse(tokens.NextToken());
                if (tokens.HasMoreTokens())
                    reason = tokens.NextToken();
                else
                    reason = "";
			} 
			catch 
			{
				throw new IOException("Failed to parse HTTP response \"" + begin + "\"");	
			} 

		}

		/// <summary>
		/// The authentication mehtod required.
		/// </summary>
		public String AuthenticationMethod 
		{
			get
			{
				String auth = GetHeaderField("Proxy-Authenticate");
				String method = null;

				if (auth != null) 
				{
					int n = auth.IndexOf(' ');
					method = auth.Substring(0, n);
				}

				return method;
			}
		}

		/// <summary>
		/// The authentication realm.
		/// </summary>
		public String AuthenticationRealm 
		{
			get
			{
				String auth = GetHeaderField("Proxy-Authenticate");
				String realm = "";

				if (auth != null) 
				{
					int l;
					int r = auth.IndexOf('=');

					while (r >= 0) 
					{
						l = auth.LastIndexOf(' ', r);
						if (l > -1) 
						{
							String val = auth.Substring(l + 1, r);

							if (val.Equals("realm")) 
							{
								l = r + 2;
								r = auth.IndexOf('"', l);
								realm = auth.Substring(l, r);

								break;
							}

							r = auth.IndexOf('=', r + 1);
						}
					}
				}

				return realm;
			}
		}
	}
}
