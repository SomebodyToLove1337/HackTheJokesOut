using AdaptiveCards.Templating;
using HackTheJokesOut.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;

namespace HackTheJokesOut.Commands
{
	// Get Jokes from API



	/// <summary>
	/// The <see cref="DadJokeCommandHandler"/> registers a pattern with the <see cref="ITeamsCommandHandler"/> and 
	/// responds with an Adaptive Card if the user types the <see cref="TriggerPatterns"/>.
	/// </summary>
	public class DadJokeCommandHandler : ITeamsCommandHandler
	{
		private readonly ILogger<DadJokeCommandHandler> _logger;
		private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "HelloWorldCard.json");

		public IEnumerable<ITriggerPattern> TriggerPatterns => new List<ITriggerPattern>
		{
            // Used to trigger the command handler if the command text contains 'helloWorld'
            new RegExpTrigger("dadJoke")
		};

		public DadJokeCommandHandler(ILogger<DadJokeCommandHandler> logger)
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
			string response = await apiRequest.GetResponseApi("https://icanhazdadjoke.com");
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
