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
            ViewBag.Message = "To find out more about Clarifier check out the presentation below:";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "If you have any doubts, questions or suggestions on how to make Clarifier better, please don't hesitate to contact us!";
            ViewBag.Message2 = "And if you want to meet face to face, here is where you might find us:";

            return View();
        }
    }
}