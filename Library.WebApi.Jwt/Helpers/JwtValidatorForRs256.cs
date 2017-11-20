using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Library.WebApi.Jwt.Helpers
{
	/// <summary>
	/// Validates Jwt token encoded using RS256 and, if valid, returns a ClaimsPrinciple.
	/// </summary>
	public class JwtValidatorForRs256
	{
		private static string _publicKeyXml;
		/// <summary>
		/// Validates a JWT token against the supplied publicKey (in xml notation)
		///  You can convert from PEM notation to xml using https://superdry.apphb.com/tools/online-rsa-key-converter
		/// or using the BouncyCastle libraries https://www.nuget.org/packages/BouncyCastle/.
		/// </summary>
		/// <param name="publicKeyXml"></param>
		public JwtValidatorForRs256(string publicKeyXml)
		{
			_publicKeyXml = publicKeyXml;
		}

		/// <summary>
		/// Validates a JWT token and returns a ClaimsPrinciple
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public ClaimsPrincipal GetPrincipal(string token)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

				if (jwtToken == null)
					return null;
			}
			catch (Exception)
			{
				return null;
			}

			var rsaCryptoServiceProvider = new RSACryptoServiceProvider();
			rsaCryptoServiceProvider.FromXmlString(_publicKeyXml);

			var validationParameters = new TokenValidationParameters
			{
				RequireExpirationTime = false,
				ValidateLifetime = true,
				ValidateIssuer = false,
				ValidateAudience = false,
				IssuerSigningToken = new RsaSecurityToken(rsaCryptoServiceProvider),
			};

			try
			{
				SecurityToken securityToken;
				var recipientTokenHandler = new JwtSecurityTokenHandler();
				var principal = recipientTokenHandler.ValidateToken(token, validationParameters, out securityToken);

				return principal;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}