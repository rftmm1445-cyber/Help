using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using WebApplication4.Data;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult PlayerHub()
        {
            /* =====================================
             * 1) حماية الصفحة - تسجيل دخول
             * ===================================== */
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrWhiteSpace(username))
            {
                TempData["Message"] = "يرجى تسجيل الدخول أولاً.";
                return RedirectToAction("Login", "Account");
            }

            /* =====================================
             * 2) جلب اللاعب الحالي
             * ===================================== */
            var currentPlayer = PlayerRepository.GetPlayerByUsername(username);
            if (currentPlayer == null)
            {
                HttpContext.Session.Clear();
                TempData["Message"] = "تعذر العثور على بيانات اللاعب.";
                return RedirectToAction("Login", "Account");
            }

            /* =====================================
             * 3) حساب الترتيب (Leaderboard)
             * ===================================== */
            var allPlayers = PlayerRepository.GetAllPlayers();

            var ordered = allPlayers
                .OrderByDescending(p => p.Score)
                .ThenByDescending(p => p.pLevel)
                .ThenBy(p => p.Username, StringComparer.OrdinalIgnoreCase)
                .ToList();

            int rank = ordered.FindIndex(p =>
                p.Username.Equals(currentPlayer.Username, StringComparison.OrdinalIgnoreCase)
            ) + 1;

            if (rank <= 0) rank = 999;

            /* =====================================
             * 4) منطق عجلة الحظ (Daily Wheel)
             * ===================================== */

            // المهام اليومية (8 مهام = 8 أقسام في العجلة)
            var dailyTasks = new List<string>
            {
                "اذهب إلى الموقع الصحيح",
                "احصل على 50 نقطة",
                "أكمل مهمة واحدة",
                "اكتشف موقع جديد",
                "أجب عن سؤال صحيح",
                "افتح مكافأة",
                "اربح +1 مستوى",
                "لا توجد جائزة اليوم 😅"
            };

            // هل لف العجلة اليوم؟
            string lastSpin = HttpContext.Session.GetString("LastWheelSpinDate");
            bool canSpin = lastSpin != DateTime.Today.ToString("yyyy-MM-dd");

            // اختيار مهمة (إذا لم يلف اليوم)
            int selectedIndex = -1;
            string selectedTask = null;

            if (canSpin)
            {
                var rnd = new Random();
                selectedIndex = rnd.Next(dailyTasks.Count);
                selectedTask = dailyTasks[selectedIndex];

                // تسجيل أن اللاعب لف اليوم
                HttpContext.Session.SetString(
                    "LastWheelSpinDate",
                    DateTime.Today.ToString("yyyy-MM-dd")
                );
            }

            /* =====================================
             * 5) بناء ViewModel
             * ===================================== */
            var vm = new PlayerHubViewModel
            {
                // بيانات اللاعب
                Username = currentPlayer.Username,
                Score = currentPlayer.Score,
                Level = currentPlayer.pLevel,
                Rank = rank,

                // Leaderboard
                TopPlayers = ordered.Take(5).Select((p, i) => new LeaderboardRow
                {
                    Position = i + 1,
                    Username = p.Username,
                    Score = p.Score,
                    Level = p.pLevel
                }).ToList(),

                // Wheel of Fortune
                CanSpinWheel = canSpin,
                WheelTasks = dailyTasks,
                SelectedTaskIndex = selectedIndex,
                SelectedTaskText = selectedTask
            };

            return View(vm);
        }
    }
}

