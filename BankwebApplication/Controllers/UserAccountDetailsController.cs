using BankAccountDAL;
using LoginsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BankwebApplication.Controllers
{
    public class UserAccountDetailsController : Controller
    {
        // GET: UserAccountDetails

        DAL dal = new DAL();
        public ActionResult UserDashboard()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            Session["Balance"] = dal.GetCurrentBalance(userId);
            Session["AccountNumber"] = dal.GetAccountNumber(userId);
            Session["UserName"] = dal.GetUserName(userId);
            ViewBag.Transactions = dal.GetRecentTransactions(userId);
            return View();
        }

        [HttpGet]
        public ActionResult Userlogout()
        {
            return RedirectToAction("UserLogin", "LoginAccounts");

        }
           
        public ActionResult UpdatePassword(string password)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Deposit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Deposit(TransactionModel model)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            decimal currentBalance = dal.GetCurrentBalance(userId);
            decimal newBalance = currentBalance + model.Credit;

            dal.SaveTransaction(
                userId,
                "Deposit",
                model.SenderName,
                null,
                model.SenderAccount.ToString(),
                null,
                model.Credit,
                0,
                newBalance
            );

            return Json(new { success = true });
        }


        [HttpGet]
        public ActionResult Withdraw()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Withdraw(TransactionModel model)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            decimal currentBalance = dal.GetCurrentBalance(userId);

            if (model.Debit > currentBalance)
            {
                return Json(new { success = false, message = "Insufficient balance." }, JsonRequestBehavior.AllowGet);
            }

            decimal newBalance = currentBalance - model.Debit;
            string senderName = Convert.ToString(Session["UserName"]);

            dal.SaveTransaction(
                userId,
                "Withdraw",
                senderName,
                model.RecieverName,
                model.SenderAccount.ToString(),
                model.ReceiverAccount.ToString(),
                0,
                model.Debit,
                newBalance
            );

            return Json(new { success = true });
        }


        [HttpGet]
        public ActionResult Transfer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Transfer(string ReceiverName, long ReceiverAccount, decimal Debit)
        {
            int senderId = Convert.ToInt32(Session["UserId"]);
            string senderName = Convert.ToString(Session["UserName"]);
            string senderAccount = Convert.ToString(Session["AccountNumber"]);

          

            // Get Receiver Id
            int receiverId = dal.GetUserIdByUsernameAndAccount(ReceiverName, ReceiverAccount);
            if (receiverId == 0)
            {
                return Json(new { success = false, message = "Invalid receiver details." });
            }

            decimal currentBalance = dal.GetCurrentBalance(senderId);
            if (Debit > currentBalance)
            {
                return Json(new { success = false, message = "Insufficient balance." });
            }

            // Get Receiver Details
            string receiverName = dal.GetUserName(receiverId);
            string receiverAccount = dal.GetAccountNumber(receiverId);

            // Update Sender Transaction
            decimal newSenderBalance = currentBalance - Debit;
            dal.SaveTransaction(senderId, "Transfer", senderName, receiverName, senderAccount, receiverAccount, 0, Debit, newSenderBalance);

            // Update Receiver Transaction
            decimal receiverBalance = dal.GetCurrentBalance(receiverId) + Debit;
            

            dal.SaveTransaction(receiverId, "Received", senderName, receiverName, senderAccount, receiverAccount, Debit, 0, receiverBalance);

            return Json(new { success = true });
        }


    }
}