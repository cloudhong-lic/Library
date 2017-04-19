using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Library.WebApi.Handlers
{
	/// <summary>
	/// Creates an instance of a cors handler for handling cross domain requests.
	/// </summary>
	public class CorsHandler : DelegatingHandler
	{
		private readonly bool _allowCredentials;
		private readonly string[] _allowedOrigins;

		/// <summary>
		/// Creates an instance of a cors handler for cross domain calls to this WebAPI.
		/// </summary>
		/// <param name="allowedOrigins">An array of origins allowed to call this WebAPI cross-domain</param>
		/// <param name="allowCredentials">Whether or not credentials will allow to be passed cross-domain</param>
		public CorsHandler(string[] allowedOrigins, bool allowCredentials = false)
		{
			_allowedOrigins = allowedOrigins;
			_allowCredentials = allowCredentials;
		}

		/// <summary>
		/// Creates an instance of the cors handler using the cors settings stored in the machine configuration.
		/// </summary>
		/// <param name="allowCredentials">Whether or not credentials will allow to be passed cross-domain</param>
		public CorsHandler(bool allowCredentials = false)
		{
			_allowedOrigins = ConfigurationManager.AppSettings["WebApiCorsSites"].Replace(" ", "").Split(',');
			_allowCredentials = allowCredentials;
		}

		private const string Origin = "Origin";
		private const string AccessControlRequestMethod = "Access-Control-Request-Method";
		private const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
		private const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
		private const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
		private const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
		private const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			bool isCorsRequest = request.Headers.Contains(Origin);
			bool isPreflightRequest = request.Method == HttpMethod.Options;
			if (isCorsRequest)
			{
				if (isPreflightRequest)
				{
					HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
					if (_allowedOrigins.Contains(request.Headers.GetValues(Origin).First()))
					{
						response.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
					}

					if (_allowCredentials)
					{
						response.Headers.Add(AccessControlAllowCredentials, "true");
					}

					IEnumerable<string> requestedMethods;
					if (request.Headers.TryGetValues(AccessControlRequestMethod, out requestedMethods))
					{
						response.Headers.Add(AccessControlAllowMethods, string.Join(", ", requestedMethods));
					}

					IEnumerable<string> requestedHeaders;
					if (request.Headers.TryGetValues(AccessControlRequestHeaders, out requestedHeaders))
					{
						response.Headers.Add(AccessControlAllowHeaders, string.Join(", ", requestedHeaders));
					}

					return Task.FromResult(response);
				}
				else
				{
					return base.SendAsync(request, cancellationToken).ContinueWith(t =>
					{
						HttpResponseMessage response = t.Result;
						if (_allowedOrigins.Contains(request.Headers.GetValues(Origin).First()))
						{
							response.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
						}
						if (_allowCredentials)
						{
							response.Headers.Add(AccessControlAllowCredentials, "true");
						}

						return response;
					}, cancellationToken);
				}
			}
			else
			{
				return base.SendAsync(request, cancellationToken);
			}
		}
	}
}