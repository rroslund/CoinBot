﻿using Discord;
using Discord.Commands;
using System;

namespace CoinBot.Discord.Commands
{
	public abstract class CommandBase : ModuleBase
	{
		protected static void AddAuthor(EmbedBuilder builder)
		{
			builder.WithAuthor(new EmbedAuthorBuilder
			{
				Name = "FunFair CoinBot - right click above to block",
				Url = "https://funfair.io",
				IconUrl = "https://files.coinmarketcap.com/static/img/coins/32x32/funfair.png"
			});
		}

		protected static void AddFooter(EmbedBuilder builder, DateTime? dateTime = null)
		{
			if (dateTime.HasValue)
			{
				builder.Timestamp = dateTime;
				builder.Footer = new EmbedFooterBuilder
				{
					Text = "Prices updated"
				};
			}
		}
	}
}
