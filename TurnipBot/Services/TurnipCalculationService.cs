using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TurnipBot.DataAccess;
using TurnipBot.Models;

namespace TurnipBot.Services
{
    public class TurnipCalculationService
    {
        private readonly TurnipRepository _turnipRepository;
        private int _weekNum;


        public TurnipCalculationService()
        {
            _turnipRepository = new TurnipRepository();
            EnsureTableIsClear();
        }
        
        public bool AddOrUpdateSellPriceInDB(int id, string name, int price, DateTime? date = null)
        {
            EnsureTableIsClear();
            if (date == null)
                date = DateTimeOffsetter.ToUSCentralTime(DateTime.Now).DateTime;
            
            TurnipInfo turnipInfo = GetTurnipEntry(id);

            if (turnipInfo == null) //Need to create a new Turnip Entry
            {
                turnipInfo = new TurnipInfo()
                {
                    Id = id,
                    Name = name,
                    WeekNum = _weekNum,
                    BuyPrice = -1 //Default value
                };

                _turnipRepository.InsertIntoTurnipsTable(turnipInfo);
            }

            //Get the count of sell prices and what count we expect to be at
            int sellPricesCount = turnipInfo.SellPrices.Count();
            int countAsOfDate = (int)date?.DayOfWeek * 2;
            countAsOfDate += date?.Hour < 12 ? -1 : 0;

            if (sellPricesCount == countAsOfDate) //Overwrite
            {
                turnipInfo.SellPrices[turnipInfo.SellPrices.Count - 1] = price;
            }
            else if (sellPricesCount < countAsOfDate) //Add blank prices in
            {
                for (int counter = sellPricesCount; counter < countAsOfDate - 1; counter++)
                {
                    turnipInfo.SellPrices.Add(-1);
                }

                turnipInfo.SellPrices.Add(price);
            }
            else if (sellPricesCount > countAsOfDate) //Overwrite the desired sell price
            {
                turnipInfo.SellPrices[countAsOfDate - 1] = price;
            }

            _turnipRepository.UpdateTurnipTableEntry(turnipInfo);

            return true;
        }

        public bool AddOrUpdateBuyPriceInTable(int id, string name, int buyPrice)
        {
            EnsureTableIsClear();
            TurnipInfo turnipInfo = GetTurnipEntry(id);

            if (turnipInfo != null) //This entry must be updated
            {
                turnipInfo.Name = name;
                turnipInfo.BuyPrice = buyPrice;
                _turnipRepository.UpdateTurnipTableEntry(turnipInfo);
            }
            else //This entry must be inserted
            {
                _turnipRepository.InsertIntoTurnipsTable(new TurnipInfo()
                {
                    WeekNum = _weekNum,
                    Id = id,
                    Name = name,
                    BuyPrice = buyPrice
                });
            }

            return true;
        }

        public bool AddFirstTimeToRecord(int id, string name, bool firstTime)
        {
            try
            {
                EnsureTableIsClear();
                TurnipInfo turnipInfo = _turnipRepository.GetTurnipTableEntry(id);
                if (turnipInfo != null) //Update
                {
                    turnipInfo.FirstTime = firstTime;
                    _turnipRepository.UpdateTurnipTableEntry(turnipInfo);
                }
                else //Insert
                {
                    _turnipRepository.InsertIntoTurnipsTable(new TurnipInfo()
                    {
                        WeekNum = _weekNum,
                        Id = id,
                        Name = name,
                        FirstTime = firstTime
                    });
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddPatternToRecord(int id, string name, PatternEnum pattern)
        {
            try
            {
                EnsureTableIsClear();
                TurnipInfo turnipInfo = _turnipRepository.GetTurnipTableEntry(id);
                if (turnipInfo != null) //Update
                {
                    turnipInfo.Pattern = pattern;
                    _turnipRepository.UpdateTurnipTableEntry(turnipInfo);
                }
                else //Insert
                {
                    _turnipRepository.InsertIntoTurnipsTable(new TurnipInfo()
                    {
                        WeekNum = _weekNum,
                        Id = id,
                        Name = name,
                        Pattern = pattern
                    });
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private TurnipInfo GetTurnipEntry(int id)
        {
            return _turnipRepository.GetTurnipTableEntry(id);
        }

        private void UpdateWeekNum()
        {
            _weekNum = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTimeOffsetter.ToUSCentralTime(DateTime.Now).DateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
        }

        private void EnsureTableIsClear()
        {
            UpdateWeekNum();
            TurnipInfo firstEntry = _turnipRepository.GetAllTurnipsTableEntries().FirstOrDefault();
            if (firstEntry != null && firstEntry.WeekNum != _weekNum) //New week has started and we must clean out the table
            {
                _turnipRepository.DeleteAllTurnipTableEntries(); //Delete all entries for a new week
            }
        }
    }
}
