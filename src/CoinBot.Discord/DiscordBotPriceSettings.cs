namespace CoinBot.Discord
{
	public class DiscordBotPriceSettings
	{

		public DiscordBotPriceSettings(){
			UsdPricePrecision=7;
		}
		/// <summary>
		/// The USD precision to use when formatting <see cref="CoinMarketCapCoin"/> prices. Default is 7.
		/// </summary>
		/// <returns></returns>
		public int UsdPricePrecision{get;set;}
	}
}
