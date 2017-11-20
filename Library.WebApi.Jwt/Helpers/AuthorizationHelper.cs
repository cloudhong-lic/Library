using Library.WebApi.Jwt.Filters;

namespace Library.WebApi.Jwt.Helpers
{
	public class AuthorizationHelper
	{
		/// <summary>
		/// Supply a RS256 public key in xml form. To use for JWT-based authorization
		/// </summary>
		/// <param name="publicKeyXml"></param>
		/// <returns></returns>
		public static AuthorizeUsingJwtFilter GetAuthorizeUsingJwtFilter(string publicKeyXml)
		{
			var jwtValidator = new JwtValidatorForRs256(publicKeyXml);
			return new AuthorizeUsingJwtFilter(jwtValidator);
		}
	}
}