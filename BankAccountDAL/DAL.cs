using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using LoginsModel;
using System.Data;

namespace BankAccountDAL
{
    public class DAL
    {
        string ConnectionString = ConfigurationManager.AppSettings["conn"];
        public UserLoginDTO GetUserLogin(UserLoginDTO user)
        {
            UserLoginDTO resultUser = null;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string query = "SELECT ApplicationId, UserName, AccountNumber FROM UserLogin WHERE UserName = @username AND Password = @password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@password", user.Password);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    resultUser = new UserLoginDTO
                    {
                        ApplicationId = Convert.ToInt32(dr["ApplicationId"]),
                        Username = dr["UserName"].ToString(),
                        AccountNumber = Convert.ToInt64(dr["AccountNumber"]),

                    };
                }
            }

            return resultUser;
        }


        public bool GetEmployeeLogin(EmployeeLoginDTO employee)
        {
            bool isValid = false;
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            string query = "select * from EmployeeLogin where UserName= @username and Password=@password";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@username", employee.Username);
            cmd.Parameters.AddWithValue("@password", employee.Password);
            SqlDataReader dr = cmd.ExecuteReader();
            isValid = dr.HasRows;
            return isValid;
        }

        public bool InsertAccountRegistration(BankAccountRegistrationDTO userdata)
        {
            bool isInserted = false;

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            string Query = "sp_InsertAccountRegistration";
            SqlCommand cmd = new SqlCommand(Query, con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Name", userdata.Name);
            cmd.Parameters.AddWithValue("@Email", userdata.Email);
            cmd.Parameters.AddWithValue("@Phone", userdata.Phone);
            cmd.Parameters.AddWithValue("@Address", userdata.Address);
            cmd.Parameters.AddWithValue("@DateOfBirth", userdata.DateOfBirth);
            cmd.Parameters.AddWithValue("@Gender", userdata.Gender);
            cmd.Parameters.AddWithValue("@Photo", userdata.Photo);
            cmd.Parameters.AddWithValue("@AadharNumber", userdata.AadharNumber);
            cmd.Parameters.AddWithValue("@AadharProof", userdata.AadharProof);
            cmd.Parameters.AddWithValue("@PanNumber", userdata.PanNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PanProof", userdata.PanProof ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@AccountType", userdata.AccountType);
            cmd.Parameters.AddWithValue("@ModeOfOperation", userdata.ModeOfOperation);


            int roweffected = cmd.ExecuteNonQuery();

            isInserted = roweffected > 0;


            return isInserted;
        }


        public List<BankAccountRegistrationDTO> GetRegistrationDetails()
        {
            List<BankAccountRegistrationDTO> registrations = new List<BankAccountRegistrationDTO>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = "SELECT * FROM AccountRegistration";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    BankAccountRegistrationDTO dto = new BankAccountRegistrationDTO
                    {
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone = Convert.ToInt64(reader["Phone"]),
                        Address = reader["Address"].ToString(),
                        DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                        Gender = reader["Gender"].ToString(),
                        Photo = reader["Photo"].ToString(),
                        AadharNumber = reader["AadharNumber"].ToString(),
                        AadharProof = reader["AadharProof"].ToString(),
                        PanNumber = reader["PanNumber"] == DBNull.Value ? null : reader["PanNumber"].ToString(),
                        PanProof = reader["PanProof"] == DBNull.Value ? null : reader["PanProof"].ToString(),
                        AccountType = reader["AccountType"].ToString(),
                        ModeOfOperation = reader["ModeOfOperation"].ToString(),
                        Status = reader["Status"].ToString()
                    };

                    registrations.Add(dto);
                }
                reader.Close();
            }

            return registrations;
        }


        public UserLoginDTO GetApplicationDetailsByEmail(string email)
        {
            UserLoginDTO user = null;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
            SELECT 
                ar.Name, ar.Email, ar.Status, ar.Remarks,
                ul.AccountNumber, ul.Username, ul.Password
            FROM AccountRegistration ar
            LEFT JOIN UserLogin ul ON ar.Email = ul.Email
            WHERE ar.Email = @Email", con);

                cmd.Parameters.AddWithValue("@Email", email);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    string status = dr["Status"].ToString();

                    user = new UserLoginDTO
                    {
                        Name = dr["Name"].ToString(),
                        Status = status,
                        Email = dr["Email"].ToString(),
                        Remarks = dr["Remarks"] != DBNull.Value ? dr["Remarks"].ToString()  :"No Remarks Available"    
                    };

                     if (status.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
                    {
                        user.Email = dr["Email"].ToString();
                        user.AccountNumber = dr["AccountNumber"] != DBNull.Value ? Convert.ToInt64(dr["AccountNumber"]) : 0;
                        user.Username = dr["Username"]?.ToString();
                        user.Password = dr["Password"]?.ToString();
                    }
                }

                return user;
            }
        }



        public List<BankAccountRegistrationDTO> GetPendingApplications()
        {
            List<BankAccountRegistrationDTO> list = new List<BankAccountRegistrationDTO>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM AccountRegistration", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new BankAccountRegistrationDTO
                    {

                        Name = dr["Name"].ToString(),
                        Email = dr["Email"].ToString(),
                        Phone = Convert.ToInt64(dr["Phone"]),
                        Address = dr["Address"].ToString(),
                        Gender = dr["Gender"].ToString(),
                        DateOfBirth = Convert.ToDateTime(dr["DateOfBirth"]),
                        AadharNumber = dr["AadharNumber"].ToString(),
                        PanNumber = dr["PanNumber"].ToString(),
                        AccountType = dr["AccountType"].ToString(),
                        ModeOfOperation = dr["ModeOfOperation"].ToString(),
                        AadharProof = dr["AadharProof"].ToString(),
                        PanProof = dr["PanProof"].ToString(),
                        Photo = dr["Photo"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }

            return list;
        }

        public void UpdateApplicationStatus(string status, string email, string remarks)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                if (status == "Rejected")
                {
                    // Update Status in AccountRegistration
                    SqlCommand cmd1 = new SqlCommand("UPDATE AccountRegistration SET Status = @Status , Remarks =@Remarks WHERE Email = @Email", con);
                    cmd1.Parameters.AddWithValue("@Status", status);
                    cmd1.Parameters.AddWithValue("@Email", email);
                    cmd1.Parameters.AddWithValue("@Remarks", remarks);
                    cmd1.ExecuteNonQuery();

                    
               
                }
                else
                {
                    // For non-rejected status, only update Status in AccountRegistration
                    SqlCommand cmd = new SqlCommand("UPDATE AccountRegistration SET Status = @Status WHERE Email = @Email", con);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertUserLogin(UserLoginDTO dto)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO UserLogin (Name,Email, AccountNumber, Username, Password) VALUES (@Name, @Email, @AccountNumber, @Username, @Password)", con);
                cmd.Parameters.AddWithValue("@Name", dto.Name);
                cmd.Parameters.AddWithValue("@Email", dto.Email);
                cmd.Parameters.AddWithValue("@AccountNumber", dto.AccountNumber);
                cmd.Parameters.AddWithValue("@Username", dto.Username);
                cmd.Parameters.AddWithValue("@Password", dto.Password);
                 cmd.ExecuteNonQuery();

            }

        }

        public bool UpdateRegistrationData(BankAccountRegistrationDTO data)
        {
            bool isUpdated = false;

            string query = @"UPDATE AccountRegistration 
                     SET Name = @Name, Phone = @Phone, Address = @Address, 
                         Gender = @Gender, DateOfBirth = @DateOfBirth 
                     WHERE Email = @Email";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", data.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", data.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", data.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", data.Gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfBirth", data.DateOfBirth ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", data.Email);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    isUpdated = rows > 0;
                }
            }

            return isUpdated;
        }

        public void Update_Password(string password)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                    
                    SqlCommand cmd1 = new SqlCommand("UPDATE UserLogin SET Password = @Password  WHERE Email = @Email");
                    cmd1.Parameters.AddWithValue("@Password", password);
                   
                    cmd1.ExecuteNonQuery();



       
                
            }
        }


        public string GenerateTransactionReference(string HolderName)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string random = Guid.NewGuid().ToString("N").Substring(0, 2).ToUpper();
            string AccountName = HolderName?.Split(' ')[0]?.Trim() ?? "User";
            return $"TXN{timestamp}{random}/{AccountName}";
        }

        public void SaveTransaction(int userId, string type, string senderName, string receiverName, string senderAccount, string receiverAccount, decimal credit, decimal debit, decimal balanceAfter)
        {
            string nameForRef;

            if (type == "Deposit" || type == "Withdraw")
                nameForRef = senderName;
            else if (type == "Transfer")  // Sender transaction
                nameForRef = receiverName;
            else if (type == "Received")  // Receiver transaction
                nameForRef = senderName;
            else
                nameForRef = senderName;
            string reference = GenerateTransactionReference(nameForRef);

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = @"INSERT INTO Transactions 
        (UserId, Date, Type, Reference, SenderName, ReceiverName, SenderAccount, ReceiverAccount, Credit, Debit, BalanceAfter)
        VALUES (@UserId, @Date, @Type, @Reference, @SenderName, @ReceiverName, @SenderAccount, @ReceiverAccount, @Credit, @Debit, @BalanceAfter)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Reference", reference);
                cmd.Parameters.AddWithValue("@SenderName", senderName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReceiverName", receiverName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SenderAccount", senderAccount ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReceiverAccount", receiverAccount ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Credit", credit);
                cmd.Parameters.AddWithValue("@Debit", debit);
                cmd.Parameters.AddWithValue("@BalanceAfter", balanceAfter);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }




        public List<TransactionModel> GetRecentTransactions(int userId)
        {
            List<TransactionModel> list = new List<TransactionModel>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT  * FROM Transactions WHERE UserId = @UserId ORDER BY Date DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
               
                while (reader.Read())
                {

                    TransactionModel transaction = new TransactionModel();
                    transaction.Date = Convert.ToDateTime(reader["Date"]);
                    transaction.Type = reader["Type"].ToString();
                    transaction.Reference = reader["Reference"].ToString();
                    transaction.Credit = Convert.ToDecimal(reader["Credit"]);
                    transaction.Debit = Convert.ToDecimal(reader["Debit"]);
                    transaction.BalanceAfter = Convert.ToDecimal(reader["BalanceAfter"]);

                    list.Add(transaction);

                }
                return list;
            }

            
        }

        public int GetUserIdByUsernameAndAccount(string username, long accountNumber)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = "SELECT ApplicationId FROM UserLogin WHERE UserName = @username AND AccountNumber = @accountNumber";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@accountNumber", accountNumber);
                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        public decimal GetCurrentBalance(int userId)
        {
            decimal balance = 0;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT  ISNULL(SUM(Credit), 0) - ISNULL(SUM(Debit), 0) FROM Transactions WHERE UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        balance = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                    }
                }
            }

            return balance;
        }


        public string GetAccountNumber(int userId)
        {
            string accountNumber = null;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT AccountNumber FROM UserLogin WHERE ApplicationId = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        accountNumber = reader["AccountNumber"].ToString();
                    }
                }
            }

            return accountNumber;
        }


        public string GetUserName(int userId)
        {
            string userName = null;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT UserName FROM UserLogin WHERE ApplicationId = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userName = reader["UserName"].ToString();
                    }
                }
            }

            return userName;
        }




    }
}

