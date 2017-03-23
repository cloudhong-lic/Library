using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Library.WebApi.Helpers
{
	public static class CredentialHttpRequestHelper
	{
		/// <summary>
		///     Used when we want to create a new HttpClient and want to pass through authentication to another service boundary
		/// </summary>
		/// <param name="currentRequest"></param>
		public static HttpClient CreateMindaApiRequestWithExistingAuthentication(HttpRequestMessage currentRequest)
		{
			var httpClient = new HttpClient(new HttpClientHandler {CookieContainer = GetCookies(currentRequest)});
			httpClient.DefaultRequestHeaders.Authorization = currentRequest.Headers.Authorization;

			return httpClient;
		}

		public static HttpRequestMessage CreateMindaApiRequestWithExistingAuthentication(
			HttpMethod method,
			string requestUri,
			HttpRequestMessage currentRequest)
		{
			return CreateMindaApiRequestWithExistingAuthentication(method, new Uri(requestUri), currentRequest);
		}

		public static HttpRequestMessage CreateMindaApiRequestWithExistingAuthentication(HttpMethod method,
			Uri requestUri,
			HttpRequestMessage currentRequest)
		{
			var request = new HttpRequestMessage(method, requestUri);

			// forward MINDAWeb authorization cookie
			var cookieContainer = GetCookies(currentRequest);
			request.Headers.Add("Cookie", cookieContainer.GetCookieHeader(requestUri));

			// forward OAuth authorization token
			request.Headers.Authorization = currentRequest.Headers.Authorization;

			return request;
		}

		private static CookieContainer GetCookies(HttpRequestMessage currentRequest)
		{
			var cookies = new CookieContainer();

			//In pure API's cookies are not normally a thing and the http context is not used - ignore them in this case.
			if (HttpContext.Current != null)
			{
				HttpCookieCollection currentCookies = HttpContext.Current.Request.Cookies;

				// see http://stackoverflow.com/a/1214428/3229870
				if (currentCookies != null)
					foreach (var key in currentCookies.AllKeys)
					{
						// convert a System.Net.Cookie to a System.Web.HttpCookie
						HttpCookie currentCookie = currentCookies.Get(key);
						var cookie = new Cookie();

						// required for cross-domain requests, e.g. MINDAWeb to WebAPI
						cookie.Domain = new Uri(ConfigurationManager.AppSettings["ApisBaseUrl"]).Host;

						cookie.Expires = currentCookie.Expires;
						cookie.Name = currentCookie.Name;
						cookie.Path = currentCookie.Path;
						cookie.Secure = currentCookie.Secure;

						// Check for characters that System.Net.Cookie does not support (semicolon, comma)
						// See https://msdn.microsoft.com/en-us/library/system.net.cookie.value(v=vs.110).aspx#Anchor_1 for more detail
						var illegalChars = ";,".ToCharArray();
						if (currentCookie.Value.IndexOfAny(illegalChars) != -1)
						{
							// Found illegal characters, so log and ignore this cookie
							Trace.WriteLine(string.Format("Ignored the cookie '{0}' as it contained invalid characters: '{1}'",
								currentCookie.Name, currentCookie.Value));
						}
						else
						{
							cookie.Value = currentCookie.Value;

							cookies.Add(cookie);
						}
					}
			}

			return cookies;
		}
	}
}