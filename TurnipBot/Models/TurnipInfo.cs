using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TurnipBot.Models
{
    public class TurnipInfo
    {
        public static TurnipInfo Create(IDataRecord record)
        {
            TurnipInfo turnipInfo = new TurnipInfo()
            {
                WeekNum = Convert.ToInt32(record["WeekNum"].ToString()),
                Id = Convert.ToInt32(record["Id"].ToString()),
                Name = record["Name"].ToString(),
                BuyPrice = Convert.ToInt32(record["BuyPrice"].ToString()),
                FirstTime = Convert.ToBoolean(record["FirstTime"].ToString())
            };

            if (Enum.TryParse(record["Pattern"].ToString(), out PatternEnum pattern))
                turnipInfo.Pattern = pattern;
            else
                turnipInfo.Pattern = PatternEnum.Unknown;

            List<string> sellPricesString = record["SellPrices"].ToString().Split(',').ToList();
            List<int> sellPrices = new List<int>();

            foreach (string sellPrice in sellPricesString)
            {
                try
                {
                    sellPrices.Add(Convert.ToInt32(sellPrice));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            turnipInfo.SellPrices = sellPrices;

            return turnipInfo;
        }

        public int WeekNum { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int BuyPrice { get; set; }

        public List<int> SellPrices { get; set; } = new List<int>();

        public PatternEnum Pattern { get; set; } = PatternEnum.Unknown;

        public bool FirstTime { get; set; } = false;

        public string SellPricesString()
        {
            return string.Join(',', SellPrices);
        }
    }
}
