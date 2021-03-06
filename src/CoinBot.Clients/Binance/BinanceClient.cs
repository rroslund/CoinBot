﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBot.Clients.Binance
{
	public class BinanceClient : IMarketClient
	{
		/// <summary>
		/// The <see cref="CurrencyManager"/>.
		/// </summary>
		private readonly CurrencyManager _currencyManager;

		/// <summary>
		/// The Exchange name.
		/// </summary>
		public string Name => "Binance";

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCap endpoint.
		/// </summary>
		private readonly Uri _endpoint = new Uri("https://www.binance.com/exchange/public/", UriKind.Absolute);

		/// <summary>
		/// The <see cref="HttpClient"/>.
		/// </summary>
		private readonly HttpClient _httpClient;

		/// <summary>
		/// The <see cref="ILogger"/>.
		/// </summary>
		private readonly ILogger _logger;

		/// <summary>
		/// The <see cref="JsonSerializerSettings"/>.
		/// </summary>
		private readonly JsonSerializerSettings _serializerSettings;

		public BinanceClient(ILogger logger, CurrencyManager currencyManager)
		{
			this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
		    this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
		    this._httpClient = new HttpClient
			{
				BaseAddress = this._endpoint
			};

		    this._serializerSettings = new JsonSerializerSettings
			{
				Error = (sender, args) =>
				{
					Exception ex = args.ErrorContext.Error.GetBaseException();
					this._logger.LogError(new EventId(args.ErrorContext.Error.HResult), ex, ex.Message);
				}
			};
		}

		/// <inheritdoc/>
		public async Task<IReadOnlyCollection<MarketSummaryDto>> Get()
		{
			try
			{
				List<BinanceProduct> products = await this.GetProducts();
				return products.Select(p => new MarketSummaryDto
				{
					BaseCurrrency = this._currencyManager.Get(p.BaseAsset),
					MarketCurrency = this._currencyManager.Get(p.QuoteAsset),
					Market = "Binance",
					Volume = p.Volume,
					Last = p.PrevClose,
				}).ToList();
			}
			catch (Exception e)
			{
				EventId eventId = new EventId(e.HResult);
				this._logger.LogError(eventId, e, e.Message);
				throw;
			}
		}

		/// <summary>
		/// Get the market summaries.
		/// </summary>
		/// <returns></returns>
		private async Task<List<BinanceProduct>> GetProducts()
		{
			using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri("product", UriKind.Relative)))
			{
				string json = await response.Content.ReadAsStringAsync();
				JObject jObject = JObject.Parse(json);
				List<BinanceProduct> products = JsonConvert.DeserializeObject<List<BinanceProduct>>(jObject["data"].ToString(), this._serializerSettings);
				return products;
			}
		}
	}
}
