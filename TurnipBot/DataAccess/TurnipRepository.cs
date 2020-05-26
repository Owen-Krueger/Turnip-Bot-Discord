using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using TurnipBot.Models;

namespace TurnipBot.DataAccess
{
    public class TurnipRepository
    {
        private string _connectionString;

        public TurnipRepository()
        {
            _connectionString = "Data Source = turnips.db;";
        }

        public void InsertIntoTurnipsTable(TurnipInfo turnipInfo)
        {
            try
            {
                string sqlString = "INSERT INTO Turnips (WeekNum, Id, Name, BuyPrice, SellPrices, Pattern, FirstTime) VALUES " +
                    "(@WeekNum, @Id, @Name, @BuyPrice, @SellPrices, @Pattern, @FirstTime)";
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = sqlString;
                command.Parameters.AddWithValue("@WeekNum", turnipInfo.WeekNum);
                command.Parameters.AddWithValue("@Id", turnipInfo.Id);
                command.Parameters.AddWithValue("@Name", turnipInfo.Name);
                command.Parameters.AddWithValue("@BuyPrice", turnipInfo.BuyPrice);
                command.Parameters.AddWithValue("@SellPrices", turnipInfo.SellPricesString());
                command.Parameters.AddWithValue("@Pattern", turnipInfo.Pattern.ToString());
                command.Parameters.AddWithValue("@FirstTime", turnipInfo.FirstTime);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void UpdateTurnipTableEntry(TurnipInfo turnipInfo)
        {
            try
            {
                string sqlString = "UPDATE Turnips SET Name = @Name, BuyPrice = @BuyPrice, SellPrices = @SellPrices, Pattern = @Pattern, FirstTime = @FirstTime Where Id = @Id";
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = sqlString;
                command.Parameters.AddWithValue("@Name", turnipInfo.Name);
                command.Parameters.AddWithValue("@BuyPrice", turnipInfo.BuyPrice);
                command.Parameters.AddWithValue("@SellPrices", turnipInfo.SellPricesString());
                command.Parameters.AddWithValue("@Pattern", turnipInfo.Pattern);
                command.Parameters.AddWithValue("@FirstTime", turnipInfo.FirstTime);
                command.Parameters.AddWithValue("@Id", turnipInfo.Id);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public List<TurnipInfo> GetAllTurnipsTableEntries()
        {
            List<TurnipInfo> turnipInfos = new List<TurnipInfo>();
            try
            {
                string sqlString = "SELECT WeekNum, Id, Name, BuyPrice, SellPrices, Pattern, FirstTime FROM Turnips";
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = sqlString;
                SqliteDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    turnipInfos.Add(TurnipInfo.Create(dataReader));
                }
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return turnipInfos;
        }

        public TurnipInfo GetTurnipTableEntry(int id)
        {
            TurnipInfo turnipInfo = null;
            try
            {
                string sqlString = "SELECT WeekNum, Id, Name, BuyPrice, SellPrices, Pattern, FirstTime FROM Turnips WHERE Id = @Id";
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = sqlString;
                command.Parameters.AddWithValue("@Id", id);
                SqliteDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                {
                    turnipInfo = TurnipInfo.Create(dataReader);
                }
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return turnipInfo;
        }

        public void DeleteAllTurnipTableEntriesForWeek(int weekNum)
        {
            try
            {
                string sqlString = "DELETE FROM Turnips WHERE WeekNum = @WeekNum";
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = sqlString;
                command.Parameters.AddWithValue("@WeekNum", weekNum);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void DeleteAllTurnipTableEntries()
        {
            try
            {
                string sqlString = "DELETE FROM Turnips";
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = sqlString;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
