﻿using HackTheJokesOut.Models;
using AdaptiveCards.Templating;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;
using static Microsoft.Graph.Constants;

namespace HackTheJokesOut.Commands
{
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

            // Render adaptive card content
            var cardContent = new AdaptiveCardTemplate(cardTemplate).Expand
            (
                new HelloWorldModel
                {
                    Title = "You got a new DevJoke:",
                    Body = "Bla bla bla Haha",
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

	public class MyBot : ActivityHandler
	{
		private static readonly HttpClient client = new HttpClient();

		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
		{
			string url = "https://api.example.com";
			string response = await client.GetStringAsync(url);
			var message = MessageFactory.Text(response);
			await turnContext.SendActivityAsync(message, cancellationToken);
		}
	}
}
