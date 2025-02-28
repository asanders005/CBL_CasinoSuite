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
    public string PageNumber { get; set; } = "page1";
    public int PageNum { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string Filter { get; set; } = "None";
    public EGameList LbFilter { get; private set; } = EGameList.None;

    public Leaderboard(IDal dal, IUser user)
    {
        _dal = dal;
        _userSingleton = user;
    }

    public void OnGet()
    {
        if (int.TryParse(PageNumber, out int count))
        {
            PageNum = count;
        }
        else if (PageNumber.Length >= 5)
        {
            string subRoute = PageNumber.Substring(0, 4);
            if (subRoute.ToLower() == "page")
            {
                string pageNumber = this.PageNumber.Substring(4);
                if (int.TryParse(pageNumber, out int page))
                {
                    PageNum = page;
                }
            }
        }

        if (Enum.TryParse(Filter, true, out EGameList lbFilter))
        {
            LbFilter = lbFilter;
        }

        if (LbFilter != EGameList.None)
        {
            Users = _dal.GetUsers()
                .Where(u => u.GameStatistics.Contains(u.GameStatistics.Find(stat => stat._GameName == LbFilter.ToString())))
                .Select(u => new { User = u, Stat = u.GameStatistics.Find(stat => stat._GameName == LbFilter.ToString()) })
                .OrderByDescending(u => u.Stat.TotalWinnings - u.Stat.TotalLosings)
                .Select(u => u.User)
                .ToList();
        }
        else
        {
            Users = _dal.GetUsers().OrderByDescending(u => u.GameStatistics.Sum(stat => stat.TotalWinnings - stat.TotalLosings)).ToList();
        }
        CurrentUser = _userSingleton.GetUser();

        int maxPage = ((Users.Count - 1) / 10) + 1;
        PageNum = (PageNum < 1) ? 1 : (PageNum > maxPage) ? maxPage : PageNum;
    }

    public IActionResult OnPostUpdateFilter()
    {
        if (Filter != null)
        {
            return RedirectToAction("Get", new { PageNumber = "page1", Filter = Filter });
        }

        return RedirectToAction("Get");
    }

    private IDal _dal;
    private IUser _userSingleton;
}