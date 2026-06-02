using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WebApplication4.Models;

namespace WebApplication4.Data
{
    public static class PlayerRepository
    {
        // =========================
        // مسار قاعدة البيانات
        // =========================
        private static readonly string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\AppData\GameDB.accdb;Persist Security Info=False;";

        // =========================
        // Hash لكلمة المرور
        // =========================
        public static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // =========================
        // التحقق من قوة كلمة المرور
        // =========================
        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;
            return true;
        }

        // =========================
        // إضافة لاعب جديد (SignUp)
        // =========================
        public static bool AddPlayer(Player player)
        {
            Connector cn = new Connector(configs.DatabaseLocationGame);

            // صياغة SQL بنفس نمط AddContinentToDB
            string sql = "INSERT INTO Players ([Username], [Email], [Password], [Score], [pLevel]) " +
                         "VALUES ('" + player.Username.Replace("'", "''") + "', " +
                         "'" + player.Email.Replace("'", "''") + "', " +
                         "'" + player.Password.Replace("'", "''") + "', " +
                         player.Score + ", " +    // رقم بدون علامات اقتباس
                         player.pLevel + ")";     // رقم بدون علامات اقتباس

            int result = cn.RunUpdateInsertDelete(sql);
            cn.CloseConnection();
            return result > 0; // true إذا تم الإدخال بنجاح
        }


        // =========================
        // التحقق من تسجيل الدخول (Login)
        // =========================
        public static Player GetPlayer(string username, string password)
        {
            Connector cn = new Connector(configs.DatabaseLocationGame);
            string sql = "SELECT * FROM Players WHERE Username='" + username.Replace("'", "''") + "' AND Password='" + password.Replace("'", "''") + "'";
            OleDbDataReader reader = cn.RunSelect(sql);

            Player player = null;

            if (reader.Read())
            {
                player = new Player
                {
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Password = reader.GetString(reader.GetOrdinal("Password")),
                    Score = reader.GetInt32(reader.GetOrdinal("Score")),
                    pLevel = reader.GetInt32(reader.GetOrdinal("pLevel"))
                };
            }

            reader.Close();
            cn.CloseConnection();

            return player; // null إذا لم يتم العثور على لاعب
        }

        // =========================
        // الحصول على لاعب بواسطة ID
        // =========================
        public static Player GetPlayerByID(int id)
        {
            try
            {
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Players WHERE ID=@ID";
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapReaderToPlayer(reader);
                        }
                    }
                    conn.Close();
                }
            }
            catch { }
            return null;
        }

        // =========================
        // الحصول على لاعب بواسطة Username
        // =========================
        public static Player GetPlayerByUsername(string username)
        {
            Player player = null;

            // صياغة SQL
            string sql = "SELECT * FROM Players WHERE Username='" + username.Replace("'", "''") + "'";

            // إنشاء الاتصال عبر Connector
            Connector cn = new Connector(configs.DatabaseLocationGame);

            // تنفيذ الاستعلام
            OleDbDataReader reader = cn.RunSelect(sql);

            // قراءة البيانات إذا وجدت
            if (reader != null && reader.Read())
            {
                player = MapReaderToPlayer(reader);
                reader.Close();
            }

            // إغلاق الاتصال
            cn.CloseConnection();

            return player; // null إذا لم يتم العثور على لاعب
        }


        // =========================
        // الحصول على كل اللاعبين
        // =========================
        public static List<Player> GetAllPlayers()
        {
            List<Player> players = new List<Player>();
            try
            {
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Players";
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                players.Add(MapReaderToPlayer(reader));
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch { }
            return players;
        }

        // =========================
        // تعديل بيانات اللاعب (Username, Email, Password)
        // =========================
        public static bool UpdatePlayer(Player player)
        {
            try
            {
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE Players SET Username=@Username, Email=@Email, Password=@Password WHERE ID=@ID";
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", player.Username);
                        cmd.Parameters.AddWithValue("@Email", player.Email);
                        cmd.Parameters.AddWithValue("@Password", player.Password);
                        cmd.Parameters.AddWithValue("@ID", player.Id);

                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return true;
            }
            catch { return false; }
        }

        // =========================
        // تعديل Score و PLevel
        // =========================
        public static bool UpdateScoreAndLevel(int playerId, int score, int pLevel)
        {
            try
            {
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE Players SET Score=@Score, pLevel=@pLevel WHERE ID=@ID";
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Score", score);
                        cmd.Parameters.AddWithValue("@pLevel", pLevel);
                        cmd.Parameters.AddWithValue("@ID", playerId);

                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return true;
            }
            catch { return false; }
        }

        // =========================
        // حذف لاعب
        // =========================
        public static bool DeletePlayer(int playerId)
        {
            try
            {
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM Players WHERE ID=@ID";
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", playerId);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        // =========================
        // دالة مساعدة لتحويل بيانات القارئ إلى Player
        // =========================
        private static Player MapReaderToPlayer(OleDbDataReader reader)
        {
            return new Player
            {
                Id = (int)reader["ID"],
                Username = reader["Username"].ToString(),
                Email = reader["Email"].ToString(),
                Password = reader["Password"].ToString(),
                Score = reader["Score"] != DBNull.Value ? (int)reader["Score"] : 0,
                pLevel = reader["pLevel"] != DBNull.Value ? (int)reader["pLevel"] : 0
            };
        }
        public static bool AddPointsToPlayer(int playerId, int points)
        {
            try
            {
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // الحصول على النقاط الحالية
                    string sqlSelect = "SELECT Score FROM Players WHERE ID=@ID";
                    int currentScore = 0;

                    using (var cmd = new OleDbCommand(sqlSelect, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", playerId);
                        var result = cmd.ExecuteScalar();
                        if (result != null) currentScore = Convert.ToInt32(result);
                    }

                    // تحديث النقاط والمستوى
                    int newScore = currentScore + points;
                    int newLevel = newScore / 10000;

                    string sqlUpdate = "UPDATE Players SET Score=@Score, pLevel=@pLevel WHERE ID=@ID";
                    using (var cmd = new OleDbCommand(sqlUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@Score", newScore);
                        cmd.Parameters.AddWithValue("@pLevel", newLevel);
                        cmd.Parameters.AddWithValue("@ID", playerId);

                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
