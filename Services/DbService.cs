using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Data.Sqlite;
using Masae.Services;
using System.IO;

namespace Masae.Services
{
    public class DbService
    {
        public const string BotConfigSql = @"CREATE TABLE IF NOT EXISTS BotConfig (Prefix TEXT)";
        public const string XPSql = @"CREATE TABLE IF NOT EXISTS XPStats (UserID INTEGER, XP INTEGER, LEVEL INTEGER)";
        public const string XPBGSql = @"CREATE TABLE IF NOT EXISTS XPBgs (UserID INTEGER, BGID INTEGER, BGPATH TEXT)";
        public const string XPConfig = @"CREATE TABLE IF NOT EXISTS XPConfig (basexp INTEGER, multiplier INTEGER)";
        public const string XPDefConfig = $@"INSERT INTO XPConfig (basexp, multiplier) VALUES ({35}, {4})";
        public string prefix_from_db = "";

        public int basexp = 35;
        public ulong ran_by_id = 0;
        public int got_from_db_xp = 0;
        public int insert_in_db_xp = 0;
        public int got_from_db_level = 0;
        public int level_up = 0;

        public void CreateTables()
        {
            var _dbcon = new SqliteConnection("Data Source=Data/Database/Masae.db");
            _dbcon.Open();

            var BC = _dbcon.CreateCommand();
            BC.CommandText = BotConfigSql;
            BC.ExecuteNonQuery();

            var XPTable = _dbcon.CreateCommand();
            XPTable.CommandText = XPSql;
            XPTable.ExecuteNonQuery();

            var XPBackgrounds = _dbcon.CreateCommand();
            XPBackgrounds.CommandText = XPBGSql;
            XPBackgrounds.ExecuteNonQuery();
            
            var XPCfg = _dbcon.CreateCommand();
            XPCfg.CommandText = XPConfig;
            XPCfg.ExecuteNonQuery();
            
            var XPDefCfg = _dbcon.CreateCommand();
            XPDefCfg.CommandText = XPDefConfig;
            XPDefCfg.ExecuteNonQuery();
            _dbcon.Close();
        }

        public void GetPrefix()
        {
            var _dbcon = new SqliteConnection("Data Source=Data/Database/Masae.db");
            _dbcon.Open();

            var AchievePrefix = _dbcon.CreateCommand();
            AchievePrefix.CommandText = "SELECT Prefix FROM BotConfig";
            SqliteDataReader PrefixReader = AchievePrefix.ExecuteReader();
            PrefixReader.Read();
            prefix_from_db = PrefixReader[0].ToString();
        }

        public void OpenConnection()
        {
            var con = new SqliteConnection("Data Source=Data/Database/Masae.db");
            con.Open();
        }

        public void UXP()
        {
            XPUpdate();
        }

        void XPUpdate()
        {
            var _dbcon = new SqliteConnection($"Data Source=Data/Database/Masae.db");
            _dbcon.Open();
            string getfirst = $"SELECT XP, LEVEL FROM XPStats WHERE UserID = {ran_by_id}";
            var Get = _dbcon.CreateCommand();
            Get.CommandText = getfirst;
            SqliteDataReader Read = Get.ExecuteReader();
            Read.Read();
            got_from_db_xp = Int32.Parse(Read[0].ToString());
            got_from_db_level = Int32.Parse(Read[1].ToString());

            insert_in_db_xp = got_from_db_xp + 3;
            var required = (int)(basexp / 4 * got_from_db_level);
            var current = (int)(got_from_db_xp);
            if (current >= required)
            {
                insert_in_db_xp = 0;
                level_up = got_from_db_level + 1;

                var UpdateXP = _dbcon.CreateCommand();
                UpdateXP.CommandText = $"UPDATE XPStats SET XP = {insert_in_db_xp} WHERE UserID = {ran_by_id}";
                UpdateXP.ExecuteNonQuery();

                var UpdateLevel = _dbcon.CreateCommand();
                UpdateLevel.CommandText = $"UPDATE XPStats SET LEVEL = {level_up} WHERE UserID = {ran_by_id}";
                UpdateLevel.ExecuteNonQuery();
            }
            else
            {
                var SetXP = _dbcon.CreateCommand();
                SetXP.CommandText = $"UPDATE XPStats SET XP = {insert_in_db_xp} WHERE UserID = {ran_by_id}";
                SetXP.ExecuteNonQuery();
            }
        }

        public void MakeSelfIfNone()
        {
            MakeSelf();
        }

        void MakeSelf()
        {
            var _dbcon = new SqliteConnection($"Data Source=Data/Database/Masae.db");
            _dbcon.Open();

            var MakeSelfXP = _dbcon.CreateCommand();
            MakeSelfXP.CommandText = $"INSERT INTO XPStats (UserID, XP, LEVEL) VALUES ({ran_by_id}, 0, 1)";
            MakeSelfXP.ExecuteNonQuery();
        }
    }
}
