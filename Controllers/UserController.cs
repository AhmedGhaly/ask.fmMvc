using Social_website.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Social_website.Controllers
{
    public class UserController : Controller
    {
        AskContext db = new AskContext();
        public ActionResult Index(string id)
        {
            if (Session["user_id"] != null)
            {
                
                ViewBag.id = id;
                int userId = (int)Session["user_id"];
                ViewBag.myUser = db.Users.Where(u => u.Id == userId).FirstOrDefault(); 
                return View(db.Users.Where(n => n.userName_id == id).FirstOrDefault());
            }
            return Redirect("/authentication/login");

        }
        [HttpPost]
        public ActionResult Index(string content, string id, string unkown)
        {


            questions questoins = new questions();
            questoins.Content = content;
            questoins.UserId = (int)Session["user_id"];
            questoins.CreatedAt = DateTime.Now;
            questoins.unkown = unkown;
            db.Questoins.Add(questoins);
            


            answers answers = new answers();
            answers.UserId = db.Users.Where(n => n.userName_id == id).Select(n => n.Id).FirstOrDefault();
            answers.Questions = questoins;
            answers.CreatedAt = DateTime.Now;
            db.Answers.Add(answers);


            db.SaveChanges();
            return Redirect("/home");
        }

        
        public ActionResult search()
        {
            if (Session["user_id"] != null)
             return View();

            return Redirect("/authentication/login");
        }

        public ActionResult getSearchUser(string user)
        {
            if (Session["user_id"] != null)
            { 
                List<users> allusers = new List<users>();
                if (user.Contains("@") )

                    allusers = db.Users.Where(n => n.userName_id.Contains(user.Remove(0,1))).ToList();
                else
                    allusers = db.Users.Where(n => n.Username.Contains(user)).ToList();

                int myId = (int)Session["user_id"];
                users myUser = db.Users.Where(n => n.Id == myId).FirstOrDefault();
                var isExist = allusers.Contains(myUser);
                if (isExist)
                    allusers.Remove(myUser);

                ViewBag.users = allusers;

                return PartialView("searchresult");
            }
            return Redirect("/authentication/login");

        }


        public ActionResult checkUserNameId(string userName_id) 
        {
            if (Session["user_id"] != null)
            {
                users user = db.Users.Where(n => n.userName_id == userName_id).FirstOrDefault();
                if (user == null)
                    return Json(true, JsonRequestBehavior.AllowGet);
                return Json(false, JsonRequestBehavior.AllowGet);

            }
            return Redirect("/authentication/login");

        }

        public ActionResult addFollow(string id)
        {
            if (Session["user_id"] != null)
            { 
            
                int user_id = (int)Session["user_id"];
                users myUser = db.Users.Where(n => n.userName_id == id).FirstOrDefault();
                users us = db.Users.Where(u => u.Id == user_id).FirstOrDefault();

                myUser.ConfirmPassword = myUser.Password;
                us.follow.Add(myUser);
                myUser.followers++;

                try
                {
                    // Save changes to the database
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Get the validation errors
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the error messages into a single string
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Throw a new exception with the error message
                    throw new Exception(fullErrorMessage, ex);
                }

                return RedirectToAction("/index/" + id);

            }
            return Redirect("/authentication/login");

        }


        public ActionResult removeFollow(string id)
        {
            if (Session["user_id"] != null)
            {

                int user_id = (int)Session["user_id"];
                users myUser = db.Users.Where(n => n.userName_id == id).FirstOrDefault();
                users us = db.Users.Where(u => u.Id == user_id).FirstOrDefault();
                myUser.ConfirmPassword = myUser.Password;
                us.follow.Remove(myUser);
                myUser.followers--;

                try
                {
                    // Save changes to the database
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Get the validation errors
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the error messages into a single string
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Throw a new exception with the error message
                    throw new Exception(fullErrorMessage, ex);
                }

                return RedirectToAction("/index/" + id);
            }
            return Redirect("/authentication/login");

        }

        public ActionResult profile()
        {
            if (Session["user_id"] != null)
            {
                int user_id = (int)Session["user_id"];
                return View(db.Users.Where(u => u.Id == user_id).FirstOrDefault());
            }
            return Redirect("/authentication/login");
        }

        public ActionResult follow(int id)
        {
            if (Session["user_id"] != null)
            {
                return View(db.Users.Where(u => u.Id == id).FirstOrDefault().follow.ToList());
            }
            return Redirect("/authentication/login");

        }
        public ActionResult editeUser(int id)
        {
            if (Session["user_id"] != null) { 
                return View(db.Users.Find(id));
            } 
            return Redirect("/authentication/login");  
        }

        [HttpPost]
        public ActionResult editeUser(users user, HttpPostedFileBase profilePicture)
        {

            if (Session["user_id"] != null)
            {

                users us = db.Users.Find(user.Id);
                if (us != null)
                {
                    if(profilePicture != null)
                    {
                        System.IO.File.Delete(Server.MapPath($"~/attach/{us.ProfilePicture}"));
                        profilePicture.SaveAs(Server.MapPath($"~/attach/{profilePicture.FileName}"));
                        us.ProfilePicture = profilePicture.FileName;

                    }
                    us.ProfilePicture = db.Users.Find((int)Session["user_id"]).ProfilePicture;
                    us.ConfirmPassword = us.Password;
                    us.Username = user.Username;
                    us.Bio = user.Bio;
                    // Save changes to the database
                    db.SaveChanges();
                    return Redirect("profile");
                }

            }
            return Redirect("/authentication/login");  
        }

        public ActionResult block(string id)
        {
            if (Session["user_id"] != null)
            {
                users us = db.Users.Where(u => u.userName_id == id).FirstOrDefault();
                users myUser = db.Users.Find((int)Session["user_id"]);
                myUser.block_users.Add(us);
                myUser.follow.Remove(us);
                //myUser.ConfirmPassword = myUser.Password;
                us.ConfirmPassword = us.Password;
                try
                {
                    // Save changes to the database
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Get the validation errors
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the error messages into a single string
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Throw a new exception with the error message
                    throw new Exception(fullErrorMessage, ex);
                }
                return RedirectToAction("/index/" + id);
            }
            return Redirect("/authentication/login");

        }

        public ActionResult unblock(string id)
        {
            if (Session["user_id"] != null)
            {
                users us = db.Users.Where(u => u.userName_id == id).FirstOrDefault();
                users myUser = db.Users.Find((int)Session["user_id"]);
                myUser.block_users.Remove(us);
                us.ConfirmPassword = us.Password;
                try
                {
                    // Save changes to the database
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Get the validation errors
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the error messages into a single string
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Throw a new exception with the error message
                    throw new Exception(fullErrorMessage, ex);
                }
                return RedirectToAction("/index/" + id);
            }
            return Redirect("/authentication/login");

        }
    }
    
}