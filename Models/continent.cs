using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace WebApplication4.Models
{
    public class Continent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryCount { get; set; }
        public double Area { get; set; }
        public long Population { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsInhabited { get; set; }

        // ====== جلب كل القارات ======
        public static List<Continent> GetAllContinents()
        {
            List<Continent> continents = new List<Continent>();
            Connector cn = new Connector(configs.databaselocation);
            string sql = "SELECT * FROM CONTINENTS";
            OleDbDataReader reader = cn.RunSelect(sql);

            if (reader != null)
            {
                while (reader.Read())
                {
                    continents.Add(MapReaderToContinent(reader));
                }
                reader.Close();
            }

            cn.CloseConnection();
            return continents;
        }

        // ====== جلب قارة بالـ Id ======
        public static Continent GetContinentById(int id)
        {
            Continent c = null;
            string sql = "SELECT * FROM CONTINENTS WHERE Id=" + id;
            Connector cn = new Connector(configs.databaselocation);
            OleDbDataReader reader = cn.RunSelect(sql);

            if (reader != null && reader.Read())
            {
                c = MapReaderToContinent(reader);
                reader.Close();
            }

            cn.CloseConnection();
            return c;
        }

        // ====== إضافة قارة جديدة ======
        public static int AddContinentToDB(Continent c)
        {
            string sql = "INSERT INTO CONTINENTS (Name, CountryCount, Area, Population, Description, PhotoUrl, IsInhabited) " +
                         "VALUES ('" + c.Name.Replace("'", "''") + "', " + c.CountryCount + ", " + c.Area + ", " + c.Population + ", '" +
                         (c.Description ?? "").Replace("'", "''") + "', '" + (c.PhotoUrl ?? "") + "', " + (c.IsInhabited ? "True" : "False") + ")";
            Connector cn = new Connector(configs.databaselocation);
            int result = cn.RunUpdateInsertDelete(sql);
            cn.CloseConnection();
            return result;
        }

        // ====== تحديث بيانات القارة ======
        public static int UpdateContinentInDB(Continent c)
        {
            string sql = "UPDATE CONTINENTS SET " +
                         "Name='" + c.Name.Replace("'", "''") + "', " +
                         "CountryCount=" + c.CountryCount + ", " +
                         "Area=" + c.Area + ", " +
                         "Population=" + c.Population + ", " +
                         "Description='" + (c.Description ?? "").Replace("'", "''") + "', " +
                         "PhotoUrl='" + (c.PhotoUrl ?? "") + "', " +
                         "IsInhabited=" + (c.IsInhabited ? "True" : "False") +
                         " WHERE Id=" + c.Id;
            Connector cn = new Connector(configs.databaselocation);
            int result = cn.RunUpdateInsertDelete(sql);
            cn.CloseConnection();
            return result;
        }

        // ====== حذف قارة ======
        public static bool DeleteContinent(int id)
        {
            try
            {
                string sql = "DELETE FROM CONTINENTS WHERE Id=" + id;
                Connector cn = new Connector(configs.databaselocation);
                int result = cn.RunUpdateInsertDelete(sql);
                cn.CloseConnection();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        // ====== دالة مساعدة لتحويل القيم من reader إلى كائن Continent ======
        private static Continent MapReaderToContinent(OleDbDataReader reader)
        {
            return new Continent
            {
                Id = int.Parse(reader["Id"].ToString()),
                Name = reader["Name"].ToString(),
                CountryCount = int.Parse(reader["CountryCount"].ToString()),
                Area = double.Parse(reader["Area"].ToString()),
                Population = long.Parse(reader["Population"].ToString()),
                Description = reader["Description"].ToString(),
                PhotoUrl = reader["PhotoUrl"].ToString(),
                IsInhabited = bool.Parse(reader["IsInhabited"].ToString())
            };
        }
    }
}

