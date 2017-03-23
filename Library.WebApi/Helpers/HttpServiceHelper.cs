using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Library.WebApi.Interfaces;
using Newtonsoft.Json;

namespace Library.WebApi.Helpers
{
	public class HttpServiceHelper : IHttpServiceHelper
	{
		private const string JsonMediaType = "application/json";

		/// <summary>
		///     But HttpClient is designed to be used in a static / singleton fashion as its methods are thread safe,
		///     and more importantly each HttpClient instance has its own connection pool, so sharing the same HttpClient
		///     means existing connections can be reused instead of creating new connection every time.
		///     Reference articles:
		///     http://chimera.labs.oreilly.com/books/1234000001708/ch14.html#_httpclient_class
		///     http://codereview.stackexchange.com/questions/69950/single-instance-of-reusable-httpclient
		///     http://stackoverflow.com/questions/22560971/what-is-the-overhead-of-creating-a-new-httpclient-per-call-in-a-webapi-client/35045301#35045301
		///     http://stackoverflow.com/questions/11178220/is-httpclient-safe-to-use-concurrently
		/// </summary>
		private static readonly HttpClient _httpClient;

		static HttpServiceHelper()
		{
			_httpClient = new HttpClient(new WebRequestHandler
			{
				// Set UseCookies to false allows individual request to set its own cookies header
				// http://stackoverflow.com/questions/12373738/how-do-i-set-a-cookie-on-httpclients-httprequestmessage
				UseCookies = false,

				// Satisfies a request for a resource either by using the cached copy of the resource
				// or by sending a request for the resource to the server. The action taken is
				// determined by the current cache policy and the age of the content in the cache.
				// This is the cache level that should be used by most applications.
				// see https://msdn.microsoft.com/en-us/library/system.net.cache.requestcachelevel(v=vs.110).aspx
				CachePolicy = new RequestCachePolicy(RequestCacheLevel.Default)
			});

			// Always want JSON response
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));

			// Default user agent is unset unless overriden by SetUserAgent
			_httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Unset", "0"));
		}

		/// <summary>
		///     Attempt to deserialize the result from the web request.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <returns></returns>
		public T DeserialiseObject<T>(string content)
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(content);
			}
			catch (JsonException)
			{
				throw new FriendlyHttpResponseException(HttpStatusCode.InternalServerError,
					string.Format("Unable to deserialise {0} into {1}", content, typeof(T).Name));
			}
		}

		/// <summary>
		///     Posts an object to the specified uri using the credentials of the current WebAPI request and returns an object of
		///     type "T"
		/// </summary>
		/// <typeparam name="T">The type of object that is returned</typeparam>
		/// <param name="uri">The uri to request</param>
		/// <param name="data">The data to send</param>
		public async Task<T> PostAsync<T>(string uri, object data)
		{
			var response = await _httpClient.PostAsJsonAsync(uri, data);

			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				throw new FriendlyHttpResponseException(response.StatusCode,
					string.Format("Error accessing url at {0} : Result of Response: {1}", uri, errorContent)
						.Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));
			}

			var content = await response.Content.ReadAsStreamAsync();

			return DeserialiseObject<T>(content);
		}

		public async Task PostAsync(string uri, object data)
		{
			var response = await _httpClient.PostAsJsonAsync(uri, data);

			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				throw new FriendlyHttpResponseException(response.StatusCode,
					string.Format("Error accessing url at {0} : Result of Response: {1}", uri, errorContent)
						.Replace("\r\n", "")
						.Replace("\r", "")
						.Replace("\n", ""));
			}
		}

		/// <summary>
		///     Puts an object to the specified uri using the credentials of the current WebAPI request and returns an object of
		///     type "T"
		/// </summary>
		/// <typeparam name="T">The type of object that is returned</typeparam>
		/// <param name="uri">The uri to request</param>
		/// <param name="data">The data to send</param>
		public async Task<T> PutAsync<T>(string uri, object data)
		{
			var response = await _httpClient.PutAsJsonAsync(uri, data);

			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				throw new FriendlyHttpResponseException(response.StatusCode,
					string.Format("Error accessing url at {0} : Result of Response: {1}", uri, errorContent)
						.Replace("\r\n", "")
						.Replace("\r", "")
						.Replace("\n", ""));
			}

			var content = await response.Content.ReadAsStreamAsync();

			return DeserialiseObject<T>(content);
		}

		public async Task PutAsync(string uri, object data)
		{
			var response = await _httpClient.PutAsJsonAsync(uri, data);

			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				throw new FriendlyHttpResponseException(response.StatusCode,
					string.Format("Error accessing url at {0} : Result of Response: {1}", uri, errorContent)
						.Replace("\r\n", "")
						.Replace("\r", "")
						.Replace("\n", ""));
			}
		}

		/// <summary>
		///     Gets an object to the specified uri using the credentials of the current WebAPI request and returns an object of
		///     type "T"
		/// </summary>
		/// <typeparam name="T">The type of object that is returned</typeparam>
		/// <param name="uri">The uri to request</param>
		public async Task<T> GetAsync<T>(string uri)
		{
			var response = await _httpClient.GetAsync(uri);

			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				throw new FriendlyHttpResponseException(response.StatusCode,
					string.Format("Error accessing url at {0} : Result of Response: {1}", uri, errorContent)
						.Replace("\r\n", "")
						.Replace("\r", "")
						.Replace("\n", ""));
			}

			var content = await response.Content.ReadAsStreamAsync();

			return DeserialiseObject<T>(content);
		}

		public static void SetUserAgent(string productName, string productVersion)
		{
			_httpClient.DefaultRequestHeaders.UserAgent.Clear();
			_httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(productName, productVersion));
		}

		private T DeserialiseObject<T>(Stream stream)
		{
			try
			{
				using (var sr = new StreamReader(stream))
				using (JsonReader reader = new JsonTextReader(sr))
				{
					var serialiser = new JsonSerializer();
					return serialiser.Deserialize<T>(reader);
				}
			}
			catch (JsonException)
			{
				throw new FriendlyHttpResponseException(HttpStatusCode.InternalServerError,
					string.Format("Unable to deserialise stream into {0}", typeof(T).Name));
			}
		}
	}
}