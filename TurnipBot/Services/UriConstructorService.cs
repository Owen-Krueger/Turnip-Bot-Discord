using System;
using System.Collections.Generic;
using System.Text;
using TurnipBot.Models;

namespace TurnipBot.Services
{
    public static class UriConstructorService
    {
        private static string _baseUrl = "https://turnipprophet.io/?prices="; //Url goes ...?prices=100.97.85...

        public static string GenerateTurnipUrl(TurnipInfo info, bool firstTime = false)
        {
            string response = $"{_baseUrl}{info.BuyPrice}.{string.Join('.', info.SellPrices)}";

            if (firstTime)
                response += "&first=true";

            if (info.Pattern != PatternEnum.Unknown)
                response += $"&pattern={(int)info.Pattern}";

            return response;
        }
    }
}
