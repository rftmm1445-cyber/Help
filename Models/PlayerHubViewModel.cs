using System;
using System.Collections.Generic;

namespace WebApplication4.Models
{
    public class PlayerHubViewModel
    {
        // بيانات اللاعب
        public string Username { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
        public int Rank { get; set; }

        // Leaderboard (Top 5)
        public List<LeaderboardRow> TopPlayers { get; set; } = new();

        /* ======================================================
           🎯 Wheel of Fortune (Daily Tasks)
           ====================================================== */

        // قائمة المهام اليومية (8 مهام للعجلة)
        public List<string> WheelTasks { get; set; } = new();

        // هل يمكن للاعب لف العجلة اليوم؟
        public bool CanSpinWheel { get; set; } = true;

        // المهمة المختارة بعد الدوران (index)
        public int SelectedTaskIndex { get; set; } = -1;

        // نص المهمة المختارة
        public string SelectedTaskText { get; set; } = null;
    }

    public class LeaderboardRow
    {
        public int Position { get; set; }
        public string Username { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
    }
}
