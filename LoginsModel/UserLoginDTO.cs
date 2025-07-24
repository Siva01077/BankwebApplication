using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace LoginsModel
{
   public class UserLoginDTO
    {
        public int ApplicationId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter username")]
        public string Username { get; set; }
        public string Status { get; set; }
        public long? AccountNumber { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Remarks { get; set; }
    }

    public class EmployeeLoginDTO
    {
        [Required(ErrorMessage = "Please enter username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public long? AccountNumber { get; set; }
        
    }

    public class BankAccountRegistrationDTO
    {
        public string ApplicationId { get; set; }
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public long? Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string AadharNumber { get; set; }

        public string PanNumber { get; set; }  // Optional

        [Required]
        public string AccountType { get; set; }

        [Required]
        public string ModeOfOperation { get; set; }

        // File uploads:

        [Required(ErrorMessage = "Aadhaar proof file is required")]
        public string AadharProof { get; set; }

        // Pan proof file is optional, no Required attribute
        public string PanProof { get; set; }

       

        [Required(ErrorMessage = "Photo file is required")]
        public string Photo { get; set; }
        public  string Status { get; set; }
        public long? AccountNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Remarks { get; set; }
    }
}
