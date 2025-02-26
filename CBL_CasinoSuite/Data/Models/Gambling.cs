using CBL_CasinoSuite.Data.Interfaces;

namespace CBL_CasinoSuite.Data.Models
{
    public static class Gambling
    {
        public static void Bet(float betAmount, ref IDal dal, string username, string gameName)
        {
            User workingUser = dal.GetUser(username);

            GameStats tempStats = workingUser.GameStatistics.First(g => g._GameName == gameName);
            if (tempStats == null) tempStats = new GameStats(gameName);

            tempStats.TotalLosings += betAmount;

            dal.UpdateUserStatistics(workingUser.Username, tempStats);
            dal.UpdateUserBalance(workingUser.Username, workingUser.CurrentBalance - betAmount);
        }

        public static void Win(float betAmount, ref IDal dal, string username, string gameName)
        {
            User workingUser = dal.GetUser(username);

            GameStats tempStats = workingUser.GameStatistics.First(g => g._GameName == gameName);
            if (tempStats == null) tempStats = new GameStats(gameName);

            tempStats.TotalLosings -= betAmount;
            tempStats.TotalWinnings += (betAmount);
            tempStats.TotalWins += 1;

            dal.UpdateUserStatistics(workingUser.Username, tempStats);
            dal.UpdateUserBalance(workingUser.Username, workingUser.CurrentBalance + (2 * betAmount));
        }

        public static void Lose(ref IDal dal, string username, string gameName)
        {
            User workingUser = dal.GetUser(username);

            GameStats tempStats = workingUser.GameStatistics.First(g => g._GameName == gameName);
            if (tempStats == null) tempStats = new GameStats(gameName);

            tempStats.TotalLosses += 1;

            dal.UpdateUserStatistics(workingUser.Username, tempStats);
        }
    }
}
