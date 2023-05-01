using Social_website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Social_website.Controllers
{
    public class AnswerController : Controller
    {
        AskContext db = new AskContext();

        public ActionResult index()
        {
            if (Session["user_id"] != null)
            {
                users myuser = db.Users.Find((int)Session["user_id"]);
                return View(myuser);
            }
            return Redirect("/authentication/login");

        }

        [HttpPost]
        public ActionResult index(string answer, int id)
        {
            answers myanswer = db.Answers.Find(id);
            myanswer.Content = answer;
            myanswer.CreatedAt = DateTime.Now;
            db.SaveChanges();

            return Redirect("/answer");
        }


        public ActionResult deleteAnswer(int id)
        {
            if (Session["user_id"] != null)
            {
                answers ans = db.Answers.Find(id);
                if (ans != null)
                {
                    db.Answers.Remove(ans);
                    db.SaveChanges();
                }
                return Redirect("/user/profile");
            }
            return Redirect("/authentication/login");

        }



    }
}