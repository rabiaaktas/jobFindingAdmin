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
    
    public partial class company_logo
    {
        public company_logo()
        {
            this.company = new HashSet<company>();
        }
    
        public int companyLogoId { get; set; }
        public byte[] companyLogo { get; set; }
    
        public virtual ICollection<company> company { get; set; }
    }
}