//using System;
//using System.Collections.Generic;
//using System.Data.OleDb;
//using WebApplication4; // للوصول إلى Configs

//namespace WebApplication4.Models
//{
//    public class Player
//    {
//        public int ID { get; set; }
//        public string Username { get; set; }
//        public string Email { get; set; }
//        public string Password { get; set; }
//        public int Score { get; set; }
//        public int pLevel { get; set; }
//    }



//   }
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.OleDb;
using WebApplication4.GameModels;

namespace WebApplication4.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Score { get; set; }
        public int pLevel { get; set; }


        // =========================================================================
        // العملية الجديدة: استقبال الإحداثيات وإنشاء كائن Location مرتبط بهذا اللاعب
        // =========================================================================
        public Location CreatePlayerLocation(double latitude, double longitude)
        {
            // بناء كائن الـ Location الجديد
            // نمرر 0 للـ LocationId لأن قاعدة البيانات ستعطيه رقماً تلقائياً عند الحفظ
            // ونضع الاسم فارغاً "" ليقوم الذكاء الاصطناعي بملئه لاحقاً
            // ونمرر this.Id وهو معرف اللاعب الحالي الذي استدعى الدالة
            Location newLocation = new Location(0, "", latitude, longitude, this.Id);

            return newLocation;
        }

        // نص الاتصال بقاعدة بيانات MS Access - قم بتعديل المسار لملفك الحقيقي
        private static readonly string ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\AppData\GameDB.accdb;Persist Security Info=False;";

        // جلب بيانات اللاعب بواسطة اسم المستخدم (لإدارة الجلسة)
        public static Player GetPlayerByUsername(string username)
        {
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                string query = "SELECT * FROM Players WHERE Username = @User";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@User", username);

                conn.Open();
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Player
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Username = reader["Username"].ToString(),
                            Email = reader["Email"].ToString(),
                            Password = reader["Password"].ToString(),
                            Score = Convert.ToInt32(reader["Score"]),
                            pLevel = Convert.ToInt32(reader["pLevel"])
                        };
                    }
                }
            }
            return null;
        }


        // دالة موحدة لتحديث النقاط والمستوى في قاعدة البيانات
        public static bool SavePlayerProgress(int playerId, int newScore, int newLevel)
        {
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                string query = "UPDATE Players SET Score = @Score, pLevel = @Level WHERE Id = @Id";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@Score", newScore);
                cmd.Parameters.AddWithValue("@Level", newLevel);
                cmd.Parameters.AddWithValue("@Id", playerId);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }
    }
}