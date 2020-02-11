using System;
using Maverick.Crypto.Util;
namespace Maverick.HTTP
{
	/// <summary>
	/// Defines the properties of a HTTP request.
	/// </summary>
	public class HTTPRequest : HTTPHeader
	{
		/// <summary>
		/// Create a request with the method provided.
		/// </summary>
		/// <param name="method"></param>
		public HTTPRequest(String method)
		{
			this.begin = method;
		}
	}
}
