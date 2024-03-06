using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackFlow.Models.dbforall
{
    [Table("Fines")]
    public partial class Fine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FineID { get; set; }

        public long? RecordID { get; set; }

        public long? ViolationType { get; set; }

        public ViolationType ViolationType1 { get; set; }

        [Required]
        public string UserID { get; set; }

        public AspNetUser AspNetUser { get; set; }

        public string Description { get; set; }

    }
}