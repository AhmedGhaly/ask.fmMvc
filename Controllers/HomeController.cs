using Social_website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Social_website.Controllers
{
    public class HomeController : Controller
    {
        AskContext db = new AskContext();
        public ActionResult Index()
        {
            if(Session["user_id"] != null)
            {
                int user_id = (int)Session["user_id"];
                users myuser = db.Users.Find(user_id);
                List<answers> followAnswers = new List<answers>();

                foreach (var item in myuser.follow)
                {
                    followAnswers.AddRange(item.Answers);
                    followAnswers.OrderBy(x => x.CreatedAt);
                }

                return View(followAnswers);
            }
            return Redirect("/authentication/login");
        }

        
    }
}