using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretDal.Concreate
{
	public class EchoBot:ActivityHandler
	{
		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
		{
			var userInput = turnContext.Activity.Text;
			var reply = $"Bot: \"{userInput}\" mesajını aldım.";
			await turnContext.SendActivityAsync(MessageFactory.Text(reply), cancellationToken);
		}
	}
}
