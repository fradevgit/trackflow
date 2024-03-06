using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackFlow.Models.dbforall
{
    [Table("ViolationTypes")]
    public partial class ViolationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ViolationID { get; set; }

        [Required]
        public string ViolationName { get; set; }

        public long? Amount { get; set; }

        public string Description { get; set; }

        public ICollection<Fine> Fines { get; set; }

    }
}