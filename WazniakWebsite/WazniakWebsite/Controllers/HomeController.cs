using System.Web.Mvc;

namespace WazniakWebsite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This page is yet to be updated. Stay tunned for fresh updates on the work progress!";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "If you have any doubts, questions or suggestions on how to make Clarifier better, please don't hesitate to contact us!";
            ViewBag.Message2 = "Here is where you might find us:";
            ViewBag.Message3 = "And here is some other useful contact info:";
            return View();
        }
    }
}