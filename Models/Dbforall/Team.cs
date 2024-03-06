using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackFlow.Models.dbforall
{
    [Table("Teams")]
    public partial class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TeamID { get; set; }

        public long? ShiftID { get; set; }

        public Shift Shift { get; set; }
        [Required]
        public string TeamName { get; set; }

        public ICollection<AspNetUser> AspNetUsers { get; set; }

    }
}