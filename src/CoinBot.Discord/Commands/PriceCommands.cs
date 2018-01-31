using CoinBot.Discord.Extensions;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using CoinBot.Clients.CoinMarketCap;
using CoinBot.Core;
using Microsoft.Extensions.Options;

namespace CoinBot.Discord.Commands
{
	public class PriceCommands : CommandBase
	{
		private readonly CurrencyManager _currencyManager;
		private readonly ILogger _logger;
		private readonly DiscordBotPriceSettings _settings;

		public PriceCommands(IOptions<DiscordBotPriceSettings> settings,CurrencyManager currencyManager, ILogger logger)
		{
			this._settings=settings.Value;
			this._currencyManager = currencyManager;
		    this._logger = logger;
		}

		[Command("price"), Summary("get price info for a coin, e.g. !price FUN")]
		public async Task Price([Remainder, Summary("The symbol for the coin")] string symbol)
		{
			using (this.Context.Channel.EnterTypingState())
			{
				try
				{
					Currency currency = this._currencyManager.Get(symbol);
					
					if (currency != null)
					{
						CoinMarketCapCoin details = currency.Getdetails<CoinMarketCapCoin>();
						await this.ReplyAsync($"{currency.Symbol} - {details.GetPriceSummary(this._settings)} ({details.GetChangeSummary()})");
					}
					else
					{
						await this.ReplyAsync($"sorry, {symbol} was not found");
					}
				}
				catch (Exception e)
				{
				    this._logger.LogError(new EventId(e.HResult), e, e.Message);
					await this.ReplyAsync($"oops, something went wrong, sorry!");

					return;
				}
			}
		}
	}
}
