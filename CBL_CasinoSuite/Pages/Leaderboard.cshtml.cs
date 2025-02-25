using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using CBL_CasinoSuite.Data.NavConstraints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages;

public class Leaderboard : PageModel {
    public List<User> Users { get; private set; }
    public User CurrentUser { get; set; }

    [BindProperty(SupportsGet = true)]
    public string pageNumber { get; set; } = "page1";
    public int PageNum { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string filter { get; set; } = "None";
    public LeaderboardFilter LbFilter { get; private set; }

    public Leaderboard(IDal dal, IUser user)
    {
        _dal = dal;
        _userSingleton = user;
    }

    public void OnGet() {
        if (int.TryParse(pageNumber, out int count))
        {
            PageNum = count;
        }

        string subRoute = pageNumber.Substring(0, 4);
        if (subRoute.ToLower() == "page")
        {
            string pageNumber = this.pageNumber.Substring(4);
            if (int.TryParse(pageNumber, out int page))
            {
                PageNum = page;
            }
        }

        if (Enum.TryParse(filter, out LeaderboardFilter lbFilter))
        {
            LbFilter = lbFilter;
        }

        switch (LbFilter)
        {
            //case LeaderboardFilter.Blackjack:
            //    Users = _dal.GetUsers().OrderByDescending(u => u.GameStatistics.FirstOrDefault(stat => stat._GameName == "Blackjack")(stat => stat.TotalWinnings - stat.TotalLosings)).ToList();
            //    break;
            default:
                Users = _dal.GetUsers().OrderByDescending(u => u.GameStatistics.Sum(stat => stat.TotalWinnings - stat.TotalLosings)).ToList();
                break;
        }
        CurrentUser = _userSingleton.GetUser();
    }

    private IDal _dal;
    private IUser _userSingleton;
}