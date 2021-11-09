using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.FBNMobile.DTOs
{
    public class Customer : ErrorResponse
    {
        public bool IsSuccessful { get; set; }
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string AccountNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool HasAccount { get; set; }

        public List<Transactions> Transactions { get; set; }
        public List<UserActivity> UserActivities { get; set; }

        public Customer(bool isSuccessful)
        {
            this.IsSuccessful = isSuccessful;
            Transactions = new List<Transactions>();
            UserActivities = new List<UserActivity>();
        }
    }
}
