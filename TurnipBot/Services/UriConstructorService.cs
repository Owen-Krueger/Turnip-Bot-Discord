using System;
using System.Collections.Generic;
using System.Text;
using TurnipBot.Models;

namespace TurnipBot.Services
{
    public static class UriConstructorService
    {
        private static string _baseUrl = "https://turnipprophet.io/?prices="; //Url goes ...?prices=100.97.85...

        public static string GenerateTurnipUrl(TurnipInfo turnipInfo)
        {
            string response = $"{_baseUrl}{turnipInfo.BuyPrice}.{turnipInfo.SellPricesUrlString()}";

            if (turnipInfo.FirstTime)
                response += "&first=true";

            if (turnipInfo.Pattern != PatternEnum.Unknown)
                response += $"&pattern={(int)turnipInfo.Pattern}";

            return response;
        }
    }
}
