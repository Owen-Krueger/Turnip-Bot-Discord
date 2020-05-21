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
                FirstTime = Convert.ToBoolean(Convert.ToInt16(record["FirstTime"].ToString()))
            };

            if (Enum.TryParse(record["Pattern"].ToString(), out PatternEnum pattern))
                turnipInfo.Pattern = pattern;
            else
                turnipInfo.Pattern = PatternEnum.Unknown;


            try
            {
                if (record["SellPrices"].ToString().Length > 0)
                {
                    string sellPricesString = record["SellPrices"].ToString();
                    foreach (string sellPrice in sellPricesString.Split(',').ToList())
                    {
                        try
                        {
                            if (sellPrice == "")
                                turnipInfo.SellPrices.Add(-1);
                            else
                                turnipInfo.SellPrices.Add(Convert.ToInt32(sellPrice));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }                
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }

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
            string sellPriceString = string.Join(',', SellPrices).Replace("-1","");

            return sellPriceString;
        }

        public string SellPricesUrlString()
        {
            string sellPriceString = string.Join('.', SellPrices).Replace("-1", "");

            return sellPriceString;
        }
    }
}
