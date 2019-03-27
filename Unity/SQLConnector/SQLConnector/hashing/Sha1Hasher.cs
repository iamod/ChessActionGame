using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace SQL
{
	class Sha1Hasher
	{
		private string Salt = "sdfFasdfadf213rt35FDS5435dsf12AJHGJYdfg";
		public string GetHash(string value)
		{
			var data = Encoding.ASCII.GetBytes(Salt + value);
			var hashData = new SHA1Managed().ComputeHash(data);
			var hash = string.Empty;
			foreach (var b in hashData)
			{
				hash += b.ToString("x2");
			}
			return hash;
		}
	}
}
