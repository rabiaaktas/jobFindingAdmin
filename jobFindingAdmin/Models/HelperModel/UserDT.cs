using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jobFindingAdmin.Models.HelperModel
{
    public class UserDT
    {
        public int userAccountId { get; set; }
        public string userEmail { get; set; }
        public string stufirstName { get; set; }
        public string stulastName { get; set; }
        public string userPhone { get; set; }
        public string userAddress { get; set; }
        public string userIsActive { get; set; }
        public string userIsConfirmed { get; set; }
        public int userTypeId { get; set; }
        public string user_type_name { get; set; }

    }
}