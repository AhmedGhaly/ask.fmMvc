using Social_website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Social_website.Controllers
{
    public class QuestionController : Controller
    {
        AskContext db = new AskContext();
        public ActionResult index(int user_id)
        {
            List<questions> myQuestions = db.Users.Find(user_id).Questions.ToList();
            return View(myQuestions);
        }
    }
}