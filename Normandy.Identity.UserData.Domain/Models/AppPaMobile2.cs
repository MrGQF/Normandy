using System;
using System.Collections.Generic;

#nullable disable

namespace Normandy.Identity.UserData.Domain.Models
{
    public partial class AppPaMobile2
    {
        public int Id { get; set; }
        public int Userid { get; set; }
        public DateTime Ctime { get; set; }
        public DateTime Mtime { get; set; }
        public string Mobile { get; set; }
        public int Country { get; set; }
    }
}
