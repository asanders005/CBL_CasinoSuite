using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages
{
    public class IndexModel : PageModel
    {
        public IDal dbDal { get; private set; }

        public IndexModel(IDal dal)
        {
            dbDal = dal;
        }

        public void OnGet()
        {
            //User newUser = new User("Newothan McUser", "NOTpassword1234", new GameList());
            //dbDal.AddUser(newUser);
        }
    }
}
