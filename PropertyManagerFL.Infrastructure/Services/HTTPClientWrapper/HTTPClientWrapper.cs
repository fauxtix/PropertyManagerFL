using Newtonsoft.Json;
using System.Net.Http.Formatting;

namespace PropertyManagerFL.Infrastructure.Services.HTTPClientWrapper
{

	/// <summary>
	/// A generic wrapper class to REST API calls
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class HTTPClientWrapper<T> where T : class
	{
		/// <summary>
		/// For getting the resources from a web api
		/// </summary>
		/// <param name="url">API Url</param>
		/// <returns>A Task with result object of type T</returns>
		public static async Task<T> Get(string url)
		{
			T? result = null;
			using (var httpClient = new HttpClient())
			{
				var response = httpClient.GetAsync(new Uri(url)).Result;

				response.EnsureSuccessStatusCode();
				await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
				{
					if (x.IsFaulted)
						throw x.Exception;

					result = JsonConvert.DeserializeObject<T>(x.Result);
				});
			}

			return result;
		}

		/// <summary>
		/// For creating a new item over a web api using POST
		/// </summary>
		/// <param name="apiUrl">API Url</param>
		/// <param name="postObject">The object to be created</param>
		/// <returns>A Task with created item</returns>
		public static async Task<T> PostRequest(string apiUrl, T postObject)
		{
			T result = null;

			using (var client = new HttpClient())
			{
				var response = await client.PostAsync(apiUrl, postObject, new JsonMediaTypeFormatter()).ConfigureAwait(false);

				response.EnsureSuccessStatusCode();

				await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
				{
					if (x.IsFaulted)
						throw x.Exception;

					result = JsonConvert.DeserializeObject<T>(x.Result);

				});
			}

			return result;
		}

		/// <summary>
		/// For updating an existing item over a web api using PUT
		/// </summary>
		/// <param name="apiUrl">API Url</param>
		/// <param name="putObject">The object to be edited</param>
		public static async Task PutRequest(string apiUrl, T putObject)
		{
			using (var client = new HttpClient())
			{
				var response = await client.PutAsync(apiUrl, putObject, new JsonMediaTypeFormatter()).ConfigureAwait(false);

				response.EnsureSuccessStatusCode();
			}
		}
	}
}
