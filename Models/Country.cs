using System.Data.OleDb;
using System.Collections.Generic;

namespace WebApplication4.Models
{
    public class Country
    {
        public int Country_Id { get; set; }
        public int Continent_Id { get; set; }
        public string Country_Name { get; set; }
        public string Country_Capital { get; set; }
        public long Country_Population { get; set; }
        public double Country_Area { get; set; }
        public string Country_Flag { get; set; }
        public string Country_Description { get; set; }

        // جلب كل الدول
        public static List<Country> GetAllCountries()
        {
            string sql = "SELECT * FROM Countries";
            List<Country> countries = new List<Country>();
            Connector cn = new Connector(configs.databaselocation2);
            OleDbDataReader result = cn.RunSelect(sql);

            if (result == null)
                return null;

            while (result.Read())
            {
                Country c = new Country
                {
                    Country_Id = int.Parse(result["Country_Id"].ToString()),
                    Continent_Id = int.Parse(result["Continent_Id"].ToString()),
                    Country_Name = result["Country_Name"].ToString(),
                    Country_Capital = result["Country_Capital"].ToString(),
                    Country_Population = long.Parse(result["Country_Population"].ToString()),
                    Country_Area = double.Parse(result["Country_Area"].ToString()),
                    Country_Flag = result["Country_Flag"].ToString(),
                    Country_Description = result["Country_Description"].ToString()
                };
                countries.Add(c);
            }

            cn.CloseConnection();
            return countries;
        }

        // جلب الدول حسب القارة
        public static List<Country> GetCountriesByContinentId(int Continent_Id)
        {
            string sql = "SELECT * FROM Countries WHERE Continent_Id=" + Continent_Id;
            List<Country> countries = new List<Country>();
            Connector cn = new Connector(configs.databaselocation2);
            OleDbDataReader result = cn.RunSelect(sql);

            if (result == null)
                return null;

            while (result.Read())
            {
                Country c = new Country
                {
                    Country_Id = int.Parse(result["Country_Id"].ToString()),
                    Continent_Id = int.Parse(result["Continent_Id"].ToString()),
                    Country_Name = result["Country_Name"].ToString(),
                    Country_Capital = result["Country_Capital"].ToString(),
                    Country_Population = long.Parse(result["Country_Population"].ToString()),
                    Country_Area = double.Parse(result["Country_Area"].ToString()),
                    Country_Flag = result["Country_Flag"].ToString(),
                    Country_Description = result["Country_Description"].ToString()
                };
                countries.Add(c);
            }

            cn.CloseConnection();
            return countries;
        }

        // إضافة دولة جديدة
        public static int AddNewCountry(Country c)
        {
            string sql = $"INSERT INTO Countries " +
                         "(Continent_Id, Country_Name, Country_Capital, Country_Population, Country_Area, Country_Flag, Country_Description) " +
                         $"VALUES ({c.Continent_Id}, '{c.Country_Name}', '{c.Country_Capital}', {c.Country_Population}, {c.Country_Area}, '{c.Country_Flag}', '{c.Country_Description}')";
            Connector cn = new Connector(configs.databaselocation2);
            int rowsAffected = cn.RunUpdateInsertDelete(sql);
            cn.CloseConnection();
            return rowsAffected;
        }

        // حذف دولة حسب Id
        public static int DeleteCountryById(int Country_Id)
        {
            string sql = "DELETE FROM Countries WHERE Country_Id=" + Country_Id;
            Connector cn = new Connector(configs.databaselocation2);
            int x = cn.RunUpdateInsertDelete(sql);
            cn.CloseConnection();
            return x;
        }

        // تحديث دولة
        public static int UpdateCountry(Country c)
        {
            string sql = $"UPDATE Countries SET Continent_Id={c.Continent_Id}, Country_Name='{c.Country_Name}', Country_Capital='{c.Country_Capital}', " +
                         $"Country_Population={c.Country_Population}, Country_Area={c.Country_Area}, Country_Flag='{c.Country_Flag}', Country_Description='{c.Country_Description}' " +
                         $"WHERE Country_Id={c.Country_Id}";
            Connector cn = new Connector(configs.databaselocation2);
            int rowsAffected = cn.RunUpdateInsertDelete(sql);
            cn.CloseConnection();
            return rowsAffected;
        }

        // جلب دولة حسب Id
        public static Country GetCountryById(int Country_Id)
        {
            string sql = "SELECT * FROM Countries WHERE Country_Id=" + Country_Id;
            Connector cn = new Connector(configs.databaselocation2);
            OleDbDataReader result = cn.RunSelect(sql);

            if (result == null)
                return null;

            Country c = null;
            if (result.Read())
            {
                c = new Country
                {
                    Country_Id = int.Parse(result["Country_Id"].ToString()),
                    Continent_Id = int.Parse(result["Continent_Id"].ToString()),
                    Country_Name = result["Country_Name"].ToString(),
                    Country_Capital = result["Country_Capital"].ToString(),
                    Country_Population = long.Parse(result["Country_Population"].ToString()),
                    Country_Area = double.Parse(result["Country_Area"].ToString()),
                    Country_Flag = result["Country_Flag"].ToString(),
                    Country_Description = result["Country_Description"].ToString()
                };
            }

            cn.CloseConnection();
            return c;
        }

        // البحث عن الدول حسب الاسم أو العاصمة
        public static List<Country> SearchByNameOrCapital(string keyword)
        {
            string sql = $"SELECT * FROM Countries WHERE Country_Name LIKE '%{keyword}%' OR Country_Capital LIKE '%{keyword}%'";
            List<Country> countries = new List<Country>();
            Connector cn = new Connector(configs.databaselocation2);
            OleDbDataReader result = cn.RunSelect(sql);

            if (result == null)
                return null;

            while (result.Read())
            {
                Country c = new Country
                {
                    Country_Id = int.Parse(result["Country_Id"].ToString()),
                    Continent_Id = int.Parse(result["Continent_Id"].ToString()),
                    Country_Name = result["Country_Name"].ToString(),
                    Country_Capital = result["Country_Capital"].ToString(),
                    Country_Population = long.Parse(result["Country_Population"].ToString()),
                    Country_Area = double.Parse(result["Country_Area"].ToString()),
                    Country_Flag = result["Country_Flag"].ToString(),
                    Country_Description = result["Country_Description"].ToString()
                };
                countries.Add(c);
            }

            cn.CloseConnection();
            return countries;
        }

        // ==================== إضافة الدالة المطلوبة ====================
        // جلب دولة حسب اسم المستخدم وكلمة المرور
        public static Country GetCountryByUsernameAndPassword(string username, string password)
        {
            string sql = $"SELECT * FROM Countries WHERE Country_Username='{username}' AND Country_Password='{password}'";
            Connector cn = new Connector(configs.databaselocation2);
            OleDbDataReader result = cn.RunSelect(sql);

            if (result == null)
                return null;

            Country c = null;
            if (result.Read())
            {
                c = new Country
                {
                    Country_Id = int.Parse(result["Country_Id"].ToString()),
                    Continent_Id = int.Parse(result["Continent_Id"].ToString()),
                    Country_Name = result["Country_Name"].ToString(),
                    Country_Capital = result["Country_Capital"].ToString(),
                    Country_Population = long.Parse(result["Country_Population"].ToString()),
                    Country_Area = double.Parse(result["Country_Area"].ToString()),
                    Country_Flag = result["Country_Flag"].ToString(),
                    Country_Description = result["Country_Description"].ToString()
                };
            }

            cn.CloseConnection();
            return c;
        }
    }
}
