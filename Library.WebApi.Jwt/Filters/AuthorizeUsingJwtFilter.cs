using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Library.WebApi.Jwt.Helpers;

namespace Library.WebApi.Jwt.Filters
{
	/// <summary>
	/// Implements JWT-based authentication
	/// </summary>
	public class AuthorizeUsingJwtFilter : IAuthenticationFilter
	{
		private readonly JwtValidatorForRs256 _jwtValidatorForRs256;

		/// <summary>
		/// <inheritdoc />
		/// </summary>
		public AuthorizeUsingJwtFilter(JwtValidatorForRs256 jwtValidatorForRs256)
		{
			AllowMultiple = true;
			_jwtValidatorForRs256 = jwtValidatorForRs256;
		}

		/// <inheritdoc />
		public bool AllowMultiple { get; }

		public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
		{
			return new TaskFactory().StartNew(() =>
			{
				var request = context.Request;
				var authorization = request.Headers.Authorization;
				if (authorization == null)
					return;

				if (authorization.Scheme != "Bearer")
					return;

				var token = authorization.Parameter;
				if (string.IsNullOrEmpty(token))
				{
					context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
					return;
				}

				var principal = _jwtValidatorForRs256.GetPrincipal(token);
				if (principal == null)
					context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);

				context.Principal = principal;
			});
		}

		public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			// We may need to implement a challenge once we have expiring tokens. 
			return Task.FromResult(0);
		}
	}
}