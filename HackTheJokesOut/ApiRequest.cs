using System;
using System.Net.Http;
using System.Threading.Tasks;



	public class ApiRequest
	{
		private static readonly HttpClient client = new HttpClient();

		public async Task<string> GetResponseAsync(string url)
		{
			try
			{
				HttpResponseMessage response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();
				string responseBody = await response.Content.ReadAsStringAsync();
				return responseBody;
			}
			catch (HttpRequestException e)
			{
				Console.WriteLine("\nException Caught!");
				Console.WriteLine("Message :{0} ", e.Message);
				return null;
			}
		}
	}

