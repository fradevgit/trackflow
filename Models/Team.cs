using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace TrackFlow.Models
    {
        public class Team
        {
            public int TeamID {get;set;}
            public string TeamName {get;set;}

            public ICollection<ApplicationUser> Users {get;set;}
        }
        

    }