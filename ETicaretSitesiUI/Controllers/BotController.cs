﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace ETicaretSitesiUI.Controllers
{
	[Route("api/messages")]
	[ApiController]
	public class BotController : ControllerBase
	{
		private readonly IBotFrameworkHttpAdapter _adapter;
		private readonly IBot _bot;

		public BotController(IBotFrameworkHttpAdapter adapter, IBot bot)
		{
			_adapter = adapter;
			_bot = bot;
		}

		[HttpPost]
		public async Task PostAsync(CancellationToken cancellationToken)
		{
			await _adapter.ProcessAsync(Request, Response, _bot, cancellationToken);
		}
	}
}
