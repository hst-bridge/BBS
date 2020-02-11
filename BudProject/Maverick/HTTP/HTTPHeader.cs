using System;
using System.Collections;
using System.Text;
using System.IO;

namespace Maverick.HTTP
{
	/// <summary>
	/// Base class for HTTP headers
	/// </summary>
	public class HTTPHeader
	{
		Hashtable fields = new Hashtable();

		/// <summary>
		/// The first line of the HTTP header
		/// </summary>
		protected String begin;

		/// <summary>
		/// Default constructor
		/// </summary>
		public HTTPHeader()
		{
		}

		/// <summary>
		/// Read a HTTP header line.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		protected String ReadLine(Stream input) 
		{
			StringBuilder lineBuf = new StringBuilder();

			int c;

			while (true) 
			{
				c = input.ReadByte();

				if (c == -1) 
				{
					throw new IOException(
						"Failed to read expected HTTP header line");
				}

				if (c == '\n') 
				{
					continue;
				}

				if (c != '\r') 
				{
					lineBuf.Append((char) c);
				} 
				else 
				{
					break;
				}
			}

			return lineBuf.ToString();
		}

		/// <summary>
		/// Get the first line of the HTTP header
		/// </summary>
		/// <returns></returns>
		public String GetStartLine() 
		{
			return begin;
		}

		/// <summary>
		/// Returns a hashtable of all the header fields.
		/// </summary>
		/// <returns></returns>
		public Hashtable GetHeaderFields() 
		{
			return fields;
		}

		/// <summary>
		/// Returns an enumerator for the field names.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetHeaderFieldNames() 
		{
			return fields.Keys.GetEnumerator();
		}

		/// <summary>
		/// Get a field value.
		/// </summary>
		/// <param name="headerName"></param>
		/// <returns></returns>
		public String GetHeaderField(String headerName) 
		{
			return (String)fields[headerName.ToLower()];
		}

		/// <summary>
		/// Set a header field.
		/// </summary>
		/// <param name="headerName"></param>
		/// <param name="value"></param>
		public void SetHeaderField(String headerName, String value) 
		{
			fields.Add(headerName.ToLower(), value);
		}

		/// <summary>
		/// Outputs the HTTP header
		/// </summary>
		/// <returns></returns>
		public override String ToString() {
			String str = begin + "\r\n";
			IEnumerator it = GetHeaderFieldNames();

			while (it.MoveNext()) {
				String fieldName = (String) it.Current;
				str += (fieldName + ": " + GetHeaderField(fieldName) + "\r\n");
			}

			str += "\r\n";

			return str;
		}

		/// <summary>
		/// Process a HTTP header from the input stream.
		/// </summary>
		/// <param name="input"></param>
		protected void ProcessHeaderFields(Stream input)
		{
			fields.Clear();

			StringBuilder lineBuf = new StringBuilder();
			String lastHeaderName = null;
			int c;

			while (true) 
			{
				c = input.ReadByte();

				if (c == -1) 
				{
					throw new IOException("The HTTP header is corrupt");
				}

				if (c == '\n') 
				{
					continue;
				}

				if (c != '\r') 
				{
					lineBuf.Append((char) c);
				} 
				else 
				{
					if (lineBuf.Length != 0) {
						String line = lineBuf.ToString();
						lastHeaderName = ProcessNextLine(line, lastHeaderName);
						lineBuf.Length = 0;
					} else {
						break;
					}
				}
			}

			c = input.ReadByte();
		}

		private String ProcessNextLine(String line, String lastHeaderName)
		{
			String name;
			String value;
			char c = line[0];

			if ((c == ' ') || (c == '\t')) {
				name = lastHeaderName;
				value = GetHeaderField(lastHeaderName) + " " + line.Trim();
			} else {
				int n = line.IndexOf(':');

				if (n == -1) {
					throw new IOException(
						"HTTP Header encoutered a corrupt field: '" + line + "'");
				}

				name = line.Substring(0, n).ToLower();
				value = line.Substring(n + 1).Trim();
			}

			SetHeaderField(name, value);

			return name;
		}
	}
}
