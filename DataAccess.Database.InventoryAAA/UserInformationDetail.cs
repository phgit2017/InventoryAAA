namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserInformationDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }

        [StringLength(32)]
        public string FirstName { get; set; }

        [StringLength(32)]
        public string LastName { get; set; }

        [ForeignKey("UserID")]
        public virtual UserDetail UserDetail { get; set; }
    }
}
