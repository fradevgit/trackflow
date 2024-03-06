using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackFlow.Models.dbforall
{
    [Table("ActivityRecords")]
    public partial class ActivityRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ActivityID { get; set; }

        public string UserID { get; set; }

        public AspNetUser AspNetUser { get; set; }

        public string ShiftStartTime { get; set; }

        public string ShiftEndTime { get; set; }

        public string Break1StartTime { get; set; }

        public string Break1EndTime { get; set; }

        public string Break2StartTime { get; set; }

        public string Break2EndTime { get; set; }

        public string Break3StartTime { get; set; }

        public string Break3EndTime { get; set; }

        public string Break4StartTime { get; set; }

        public string Break4EndTime { get; set; }

        public string LunchStartTime { get; set; }

        public string LunchEndTime { get; set; }

    }
}