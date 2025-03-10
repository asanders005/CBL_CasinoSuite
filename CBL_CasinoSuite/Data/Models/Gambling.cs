using CBL_CasinoSuite.Data.Interfaces;

namespace CBL_CasinoSuite.Data.Models
{
    public static class Gambling
    {
        public enum EndState
        {
            Won,
            Lost,
            Tied
        }

        public static string NegativeBalanceToString(float bal)
        {
            if (bal < 0) return "-" + Math.Abs(bal).ToString("C2");
            else return bal.ToString("C2");
        }

        public static void Bet(float betAmount, ref IDal dal, string username, string gameName)
        {
            User workingUser = dal.GetUser(username);

            GameStats tempStats = workingUser.GameStatistics.FirstOrDefault(g => g._GameName == gameName);
            if (tempStats == null) tempStats = new GameStats(gameName);

            tempStats.TotalLosings += betAmount;

            dal.UpdateUserStatistics(workingUser.Username, tempStats);
            dal.UpdateUserBalance(workingUser.Username, workingUser.CurrentBalance - betAmount);
        }

        public static void Win(float betAmount, ref IDal dal, string username, string gameName, float winningsModifier = 1.0f)
        {
            User workingUser = dal.GetUser(username);

            GameStats tempStats = workingUser.GameStatistics.First(g => g._GameName == gameName);
            if (tempStats == null) tempStats = new GameStats(gameName);

            tempStats.TotalLosings -= betAmount;
            tempStats.TotalWinnings += (betAmount * winningsModifier);
            tempStats.TotalWins += 1;

            dal.UpdateUserStatistics(workingUser.Username, tempStats);
            dal.UpdateUserBalance(workingUser.Username, workingUser.CurrentBalance + betAmount + (betAmount * winningsModifier));
        }

        public static void Lose(ref IDal dal, string username, string gameName)
        {
            User workingUser = dal.GetUser(username);

            GameStats tempStats = workingUser.GameStatistics.FirstOrDefault(g => g._GameName == gameName);
            if (tempStats == null) tempStats = new GameStats(gameName);

            tempStats.TotalLosses += 1;

            dal.UpdateUserStatistics(workingUser.Username, tempStats);
        }

        public static void Tie(float betAmount, ref IDal dal, string username, string gameName)
        {
            User workingUser = dal.GetUser(username);

            GameStats tempStats = workingUser.GameStatistics.First(g => g._GameName == gameName);
            if (tempStats == null) tempStats = new GameStats(gameName);

            tempStats.TotalLosings -= betAmount;

            dal.UpdateUserStatistics(workingUser.Username, tempStats);
            dal.UpdateUserBalance(workingUser.Username, workingUser.CurrentBalance + betAmount);
        }
    }
}
