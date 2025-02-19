using CBL_CasinoSuite.Data.Interfaces;
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
            dbDal.AddUser();
        }

        public void OnGet()
        {

        }
    }
}
