using Social_website.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Social_website.Controllers
{
    public class authenticationController : Controller
    {

        AskContext db = new AskContext();

        public ActionResult signup()
        {
            return View();

        }

        [HttpPost]
        public ActionResult signup(users user, HttpPostedFileBase profilePicture)
        {
            if (checkEmail(user.Email))
            {

                if (ModelState.IsValid)
                {
                    profilePicture.SaveAs(Server.MapPath($"~/attach/{profilePicture.FileName}"));
                    user.ProfilePicture = profilePicture.FileName;
                    db.Users.Add(user);
                    db.SaveChanges();
                    Session["user_id"] = user.Id;
                    return Redirect("/");

                }
                ModelState.AddModelError("", "Invalid email or password");
                return View();
            }
            ModelState.AddModelError("email", "alreay exist");
            return View();



        }

        public ActionResult login()
        {
            return View();

        }
        [HttpPost]
        public ActionResult login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                users myuser = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password );
                if(myuser != null)
                {
                    Session["user_id"] = myuser.Id;
                    return Redirect("/");
                }
                ModelState.AddModelError("", "Invalid email or password");

            }
            return View();

        }

        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            Session["user_id"] = null;
            return RedirectToAction("Login", "authentication");

        }

        private bool checkEmail(string Email)
        {

            users user = db.Users.Where(n => n.Email == Email).FirstOrDefault();
            if (user == null)
                return true;
            return false;
            
        }
    }
}