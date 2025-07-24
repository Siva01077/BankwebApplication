using BankAccountDAL;
using LoginsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankwebApplication.Controllers
{
    public class EmployeeDetailsController : Controller
    {
        // GET: EmployeeDetails
        public ActionResult Dashboard()
        {
            var username = Session["Username"] ;
            ViewBag.Username = username;
            return View();
        }

        [HttpGet]
        public ActionResult Employeelogout()
        {
            return RedirectToAction("EmployeeLogin", "LoginAccounts");

        }

        [HttpGet]
        public ActionResult Applications()
        {
            DAL dal = new DAL();
            var applications = dal.GetPendingApplications();
            return View(applications);
        }

        [HttpPost]
        public ActionResult ApproveApplication(string name, string email)
        {
            DAL dal = new DAL();
            var application = dal.GetApplicationDetailsByEmail(email);

            if (application != null && application.Status == "Rejected")
            {
                return Json(new { success = false, message = "Cannot approve a rejected application." });
            }


            dal.UpdateApplicationStatus("Accepted",email, "");

            Random rnd = new Random();
            long accountNo = rnd.Next(110000000, 999999999);
            string username = name.Split(' ')[0].ToLower() + rnd.Next(10, 99);
            string password = "hdfc@" + rnd.Next(100, 999);

            dal.InsertUserLogin(new UserLoginDTO
            {
                Name = name,
                Email = email,
                AccountNumber = accountNo,
                Username = username,
                Password = password
            });

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RejectApplication(string email, string remarks)
        {
            DAL dal = new DAL();
            dal.UpdateApplicationStatus("Rejected",email, remarks);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateApplicationInline(BankAccountRegistrationDTO data)
        {
            DAL db = new DAL();
            bool isUpdated = db.UpdateRegistrationData(data);
            return Json(new { success = isUpdated });
        }

    }
}