using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackFlow.Models.dbforall
{
    [Table("Shifts")]
    public partial class Shift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ShiftID { get; set; }

        public string ShiftName { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public ICollection<Team> Teams { get; set; }

    }
}