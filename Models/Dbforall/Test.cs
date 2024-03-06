using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackFlow.Models.dbforall
{
    [Table("test")]
    public partial class Test
    {
        public long? Field1 { get; set; }

        public long? Field2 { get; set; }

    }
}