using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Helpers
{
	public class FunctionHelper
	{
		public static string ConvertTwoFA(string token)
		{
			for (var i = 0; i < 5; i++)
			{
				try
				{
					var totp = new Totp(Base32Encoding.ToBytes(token));
					var code = totp.ComputeTotp();
					if (code != "")
					{
						return code;
					}
				}
				catch
				{
					//
				}
			}

			return "";
		}
	}
}
