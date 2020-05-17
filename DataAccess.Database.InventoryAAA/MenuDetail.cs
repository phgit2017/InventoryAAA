namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MenuDetail
    {
        public MenuDetail()
        {
            UserMenuRoleDetails = new HashSet<UserMenuRoleDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MenuID { get; set; }

        [Required]
        [StringLength(16)]
        public string MenuName { get; set; }

        public virtual ICollection<UserMenuRoleDetail> UserMenuRoleDetails { get; set; }
    }
}
