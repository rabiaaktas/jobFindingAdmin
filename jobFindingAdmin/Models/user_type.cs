//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace jobFindingAdmin.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class user_type
    {
        public user_type()
        {
            this.user_account = new HashSet<user_account>();
        }
    
        public int userTypeId { get; set; }
        public string user_type_name { get; set; }
    
        public virtual ICollection<user_account> user_account { get; set; }
    }
}
