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
        
        public bool AddOrUpdateSellPriceInDB(int id, string name, int price)
        {
            EnsureTableIsClear();
            TurnipInfo turnipInfo = GetTurnipEntry(id);
            DayOfWeek newDow = DateTime.Now.DayOfWeek;
            bool newMorning = DateTime.Now.Hour < 12 ? true : false;

            if (turnipInfo != null) //This entry must be updated
            {
                int day = turnipInfo.SellPrices.Count() / 2;
                bool currentMorning = turnipInfo.SellPrices.Count() % 2 != 0;

                if ((int)newDow == day && newMorning == currentMorning) //Update sell price
                {
                    turnipInfo.SellPrices[turnipInfo.SellPrices.Count - 1] = price;
                }
                else //Insert new price
                {
                    turnipInfo.SellPrices.Add(price);
                }

                _turnipRepository.UpdateTurnipTableEntry(turnipInfo);
            }
            else
            {
                
            }

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
                if (turnipInfo != null)
                {
                    turnipInfo.FirstTime = firstTime;
                    _turnipRepository.UpdateTurnipTableEntry(turnipInfo);
                }
                else
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

        public bool AddPatternToRecord(int id, PatternEnum pattern)
        {
            try
            {
                EnsureTableIsClear();
                TurnipInfo info = _turnipRepository.GetTurnipTableEntry(id);
                info.Pattern = pattern;
                _turnipRepository.UpdateTurnipTableEntry(info);

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
            _weekNum = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
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
