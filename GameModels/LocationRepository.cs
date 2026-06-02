using System;
using System.Collections.Generic;
using System.Data.OleDb;
using WebApplication4.GameModels;

namespace WebApplication4.Data
{
    public static class LocationRepository
    {
        // =========================
        // إضافة موقع جديد
        // =========================
        public static bool AddLocation(Location loc)
        {
            Connector cn = new Connector(configs.DatabaseLocationGame);

            string sql = "INSERT INTO Locations ([Namee], [Latitude], [Longitude], [PlayersId]) " +
                         "VALUES ('" + loc.Namee.Replace("'", "''") + "', " +
                         loc.Latitude + ", " +
                         loc.Longitude + ", " +
                         loc.PlayersId + ")";

            int result = cn.RunUpdateInsertDelete(sql);
            cn.CloseConnection();
            return result > 0;
        }

        // =========================
        // الحصول على موقع بواسطة ID
        // =========================
        public static Location GetLocationByID(int id)
        {
            try
            {
                using (var conn = new OleDbConnection(configs.DatabaseLocationGame))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Locations WHERE LocationId=@ID";
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapReaderToLocation(reader);
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        // =========================
        // الحصول على كل المواقع
        // =========================
        public static List<Location> GetAllLocations()
        {
            List<Location> locations = new List<Location>();
            try
            {
                using (var conn = new OleDbConnection(configs.DatabaseLocationGame))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Locations";
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                locations.Add(MapReaderToLocation(reader));
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch { }
            return locations;
        }

        // =========================
        // الحصول على كل المواقع الخاصة بلاعب معين
        // =========================
        public static List<Location> GetLocationsByPlayerId(int playerId)
        {
            List<Location> locations = new List<Location>();
            Connector cn = new Connector(configs.DatabaseLocationGame);

            string sql = "SELECT * FROM Locations WHERE PlayersId=" + playerId;
            OleDbDataReader reader = cn.RunSelect(sql);

            while (reader.Read())
            {
                locations.Add(MapReaderToLocation(reader));
            }

            reader.Close();
            cn.CloseConnection();
            return locations;
        }

        // =========================
        // البحث عن موقع بالاسم
        // =========================
        public static Location GetLocationByName(string namee)
        {
            Location loc = null;
            Connector cn = new Connector(configs.DatabaseLocationGame);

            string sql = "SELECT * FROM Locations WHERE Namee='" + namee.Replace("'", "''") + "'";
            OleDbDataReader reader = cn.RunSelect(sql);

            if (reader.Read())
            {
                loc = MapReaderToLocation(reader);
            }

            reader.Close();
            cn.CloseConnection();
            return loc;
        }

        // =========================
        // تعديل موقع
        // =========================
        public static bool UpdateLocation(Location loc)
        {
            try
            {
                using (var conn = new OleDbConnection(configs.DatabaseLocationGame))
                {
                    conn.Open();
                    string sql = "UPDATE Locations SET Namee=@Namee, Latitude=@Latitude, Longitude=@Longitude, PlayersId=@PlayersId WHERE LocationId=@ID";
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Namee", loc.Namee);
                        cmd.Parameters.AddWithValue("@Latitude", loc.Latitude);
                        cmd.Parameters.AddWithValue("@Longitude", loc.Longitude);
                        cmd.Parameters.AddWithValue("@PlayersId", loc.PlayersId);
                        cmd.Parameters.AddWithValue("@ID", loc.LocationId);

                        cmd.ExecuteNonQuery();
                    }
                }
                
                


                return true;
            }
            catch { return false; }
        }

        // =========================
        // حذف موقع
        // =========================
        public static bool DeleteLocation(int id)
        {
            try
            {
                using (var conn = new OleDbConnection(configs.DatabaseLocationGame))
                {
                    conn.Open();
                    string sql = "DELETE FROM Locations WHERE LocationId=@ID";
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        // =========================
        // دالة مساعدة لتحويل القارئ إلى Location
        // =========================
        private static Location MapReaderToLocation(OleDbDataReader reader)
        {
            return new Location
            {
                LocationId = (int)reader["LocationId"],
                Namee = reader["Namee"].ToString(),
                Latitude = reader["Latitude"] != DBNull.Value ? Convert.ToDouble(reader["Latitude"]) : 0,
                Longitude = reader["Longitude"] != DBNull.Value ? Convert.ToDouble(reader["Longitude"]) : 0,
                PlayersId = reader["PlayersId"] != DBNull.Value ? Convert.ToInt32(reader["PlayersId"]) : 0
            };
        }
    }
}
