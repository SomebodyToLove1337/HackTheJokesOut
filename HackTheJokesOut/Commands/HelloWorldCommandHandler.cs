using AdaptiveCards.Templating;
using HackTheJokesOut.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;

namespace HackTheJokesOut.Commands
{
	// Get Jokes from API
	public class ApiRequest
	{
		private static readonly HttpClient client = new HttpClient();

		public async Task<string> GetResponseApi(string url)
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

	public class Joke
	{
		public string Question { get; set; }
		public string Punchline { get; set; }
	}


	/// <summary>
	/// The <see cref="HelloWorldCommandHandler"/> registers a pattern with the <see cref="ITeamsCommandHandler"/> and 
	/// responds with an Adaptive Card if the user types the <see cref="TriggerPatterns"/>.
	/// </summary>
	public class HelloWorldCommandHandler : ITeamsCommandHandler
	{
		private readonly ILogger<HelloWorldCommandHandler> _logger;
		private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "HelloWorldCard.json");

		public IEnumerable<ITriggerPattern> TriggerPatterns => new List<ITriggerPattern>
		{
            // Used to trigger the command handler if the command text contains 'helloWorld'
            new RegExpTrigger("devJoke")
		};

		public HelloWorldCommandHandler(ILogger<HelloWorldCommandHandler> logger)
		{
			_logger = logger;
		}



		public async Task<ICommandResponse> HandleCommandAsync(ITurnContext turnContext, CommandMessage message, CancellationToken cancellationToken = default)
		{
			_logger?.LogInformation($"Bot received message: {message.Text}");

			// Read adaptive card template
			var cardTemplate = await File.ReadAllTextAsync(_adaptiveCardFilePath, cancellationToken);

			// API Call
			ApiRequest apiRequest = new ApiRequest();
			string response = await apiRequest.GetResponseApi("https://backend-omega-seven.vercel.app/api/getjoke");
			List<Joke> jokes = JsonConvert.DeserializeObject<List<Joke>>(response);
			string jokeQuestion = jokes[0].Question;
			string jokePunchline = jokes[0].Punchline;

			// Render adaptive card content
			var cardContent = new AdaptiveCardTemplate(cardTemplate).Expand
			(
				new HelloWorldModel
				{
					Title = jokeQuestion,
					Body = jokePunchline
				}
			);

			// Build attachment
			var activity = MessageFactory.Attachment
			(
				new Attachment
				{
					ContentType = "application/vnd.microsoft.card.adaptive",
					Content = JsonConvert.DeserializeObject(cardContent),
				}
			);

			// send response
			return new ActivityCommandResponse(activity);
		}


	}


}
