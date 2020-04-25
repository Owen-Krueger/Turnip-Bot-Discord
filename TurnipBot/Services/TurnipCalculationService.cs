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
        private readonly int _weekNum;


        public TurnipCalculationService()
        {
            _turnipRepository = new TurnipRepository();
            _weekNum = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
        }

        public bool AddOrUpdateTurnipPriceToDB(int id, string name, int price)
        {
            List<TurnipInfo> infoList = _turnipRepository.GetAllTurnipsTableEntries();

            TurnipInfo turnipInfo = infoList.FirstOrDefault(i => i.Id == id);

            if (turnipInfo != null)
            {
                if (turnipInfo.WeekNum != _weekNum || DateTime.Now.DayOfWeek == DayOfWeek.Sunday) //New week has started and we must clean out the table
                {
                    _turnipRepository.DeleteAllTurnipTableEntries(_weekNum - 1); //Delete entries from last week
                }
                else
                {
                    if (GetSellPeriodFromDate() != GetSellPeriodFromSellPrices(turnipInfo.SellPrices)) //New sell period entry
                    {

                    }
                    else //Update sell period entry
                    {

                    }
                }
            }
            else
            {
                
            }

            return true;
        }

        public bool AddBuyPriceToTable(int id, string name, int buyPrice)
        {
            List<TurnipInfo> infoList = _turnipRepository.GetAllTurnipsTableEntries();

            TurnipInfo turnipInfo = infoList.FirstOrDefault(i => i.Id == id);

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

        public bool AddFirstTimeToRecord(int id, bool firstTime)
        {
            try
            {
                TurnipInfo info = _turnipRepository.GetTurnipTableEntry(id);
                info.FirstTime = firstTime;
                _turnipRepository.UpdateTurnipTableEntry(info);

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

        private (DayOfWeek, bool) GetSellPeriodFromDate(DateTime? dateTime = null)
        {
            if (dateTime == null)
                dateTime = DateTime.Now;

            bool isAM = true;
            if (dateTime?.Hour >= 12)
                isAM = false;

            return ((DayOfWeek)dateTime?.DayOfWeek, isAM);
        }

        private (DayOfWeek, bool) GetSellPeriodFromSellPrices(List<int> sellPrices)
        {
            int pricesCount = sellPrices.Count;

            DayOfWeek dayOfWeek = (DayOfWeek)((pricesCount / 2) + (pricesCount % 2));

            bool isAM = (pricesCount % 2 == 1);

            return (dayOfWeek, isAM);
        }
    }
}
