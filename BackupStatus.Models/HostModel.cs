namespace BackupStatus.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    public class HostModel : DbContext
    {
        // Your context has been configured to use a 'HostModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'BackupStatus.WebUI.Models.HostModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'HostModel' 
        // connection string in the application configuration file.
        public HostModel()
            : base("name=HostModel")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Host> Hosts { get; set; }
    }

    public enum StatusCode
    {
        OK,
        WARNING,
        ERROR,
        UNKNOWN
    }

    public class Host
    {
        public int Id { get; set; }

        [DisplayName("Nome")]
        [Column(TypeName ="VARCHAR")]
        [StringLength(15)]
        [Index(IsUnique =true)]
        [Required]
        public string Name { get; set; }

        [DisplayName("Endereço IP")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        [Index(IsUnique = true)]
        [Required]
        public string Address { get; set; }

        [DisplayName("Status")]
        [EnumDataType(typeof(StatusCode))]
        public StatusCode ReturnCode { get; set; }

        [DisplayName("Ultima atualização")]
        public DateTime? LastStatusUpdate { get; set; }
    }
}