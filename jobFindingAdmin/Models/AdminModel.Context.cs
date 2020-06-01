﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class AdminEntities : DbContext
    {
        public AdminEntities()
            : base("name=AdminEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<admin_log> admin_log { get; set; }
        public DbSet<business_stream> business_stream { get; set; }
        public DbSet<company> company { get; set; }
        public DbSet<countries> countries { get; set; }
        public DbSet<job_location> job_location { get; set; }
        public DbSet<job_post> job_post { get; set; }
        public DbSet<job_post_activity> job_post_activity { get; set; }
        public DbSet<job_type> job_type { get; set; }
        public DbSet<languages> languages { get; set; }
        public DbSet<universities> universities { get; set; }
        public DbSet<user_account> user_account { get; set; }
        public DbSet<user_admin> user_admin { get; set; }
        public DbSet<user_education> user_education { get; set; }
        public DbSet<user_experinence_detail> user_experinence_detail { get; set; }
        public DbSet<user_language_skill> user_language_skill { get; set; }
        public DbSet<user_log> user_log { get; set; }
        public DbSet<user_student> user_student { get; set; }
        public DbSet<user_teacher> user_teacher { get; set; }
        public DbSet<user_type> user_type { get; set; }
        public DbSet<departments> departments { get; set; }
    
        public virtual ObjectResult<Sp_Admin_Login_Result> Sp_Admin_Login(string mail, string pass)
        {
            var mailParameter = mail != null ?
                new ObjectParameter("mail", mail) :
                new ObjectParameter("mail", typeof(string));
    
            var passParameter = pass != null ?
                new ObjectParameter("pass", pass) :
                new ObjectParameter("pass", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Admin_Login_Result>("Sp_Admin_Login", mailParameter, passParameter);
        }
    }
}
