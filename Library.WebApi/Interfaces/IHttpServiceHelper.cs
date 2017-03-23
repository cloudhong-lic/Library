using System.Net.Http;
using System.Threading.Tasks;

namespace Library.WebApi.Interfaces
{
	public interface IHttpServiceHelper
	{
		T DeserialiseObject<T>(string content);

		Task<T> PostAsync<T>(string uri, object data);

		Task PostAsync(string uri, object data);

		Task<T> PutAsync<T>(string uri, object data);

		Task PutAsync(string uri, object data);

		Task<T> GetAsync<T>(string uri);
	}
}