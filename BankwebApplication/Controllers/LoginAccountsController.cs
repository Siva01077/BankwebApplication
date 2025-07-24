using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using LoginsModel;
using BankAccountDAL;

namespace BankwebApplication.Controllers
{
    public class LoginAccountsController : Controller
    {
        // GET: LoginAccounts
        
        public ActionResult Logins()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetApplicationDetails()
        {
            DAL dal = new DAL();
            List<BankAccountRegistrationDTO> Applicationlist = dal.GetRegistrationDetails();
            return PartialView("_pApplicationDetails", Applicationlist);
        }

        [HttpGet]
        public ActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserLogin(UserLoginDTO user)
        {
            DAL dal = new DAL();
            var loggedInUser = dal.GetUserLogin(user); // Now gets full user details

            if (loggedInUser != null)
            {
                Session["UserId"] = loggedInUser.ApplicationId;

                Session["UserName"] = loggedInUser.Username;
                Session["AccountNumber"] = loggedInUser.AccountNumber;

                return RedirectToAction("UserDashboard", "UserAccountDetails");
            }
            else
            {
                ViewBag.Msg = "Invalid Credentials";
                return View();
            }
        }



        [HttpGet]
        public ActionResult EmployeeLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EmployeeLogin(EmployeeLoginDTO employee)
        {

            DAL dal = new DAL();
            bool result = dal.GetEmployeeLogin(employee);

            if (result)
            {
                Session["Username"] = employee.Username;
                return RedirectToAction("Dashboard", "EmployeeDetails");
            }
            else
            {
                ViewBag.Msg = "Invalid Credentials";
                return View();
            }
        }

      

        [HttpGet]
        public ActionResult AccountRegistration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AccountRegistration(BankAccountRegistrationDTO model)
        {
            DAL dal = new DAL();
            
                bool result = dal.InsertAccountRegistration(model);
                
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ApplicationStatus()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ApplicationStatus(string email)
        {
            DAL dal = new DAL();
            var result = dal.GetApplicationDetailsByEmail(email);

            if (result == null)
            {
                ViewBag.Message = "No record found for this email.";
                return View();
            }

            return View(result);
        }
    }
}