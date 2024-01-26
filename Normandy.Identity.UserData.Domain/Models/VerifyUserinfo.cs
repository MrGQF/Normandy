using System;
using System.Collections.Generic;

#nullable disable

namespace Normandy.Identity.UserData.Domain.Models
{
    public partial class VerifyUserinfo
    {
        public int Userid { get; set; }
        public byte[] Account { get; set; }
        public string Passwd { get; set; }
        public int Userclass { get; set; }
        public DateTime Regtime { get; set; }
        public int Msgcode { get; set; }
        public int Qsid { get; set; }
        public int? Qsuserclass { get; set; }
        public string Nocase { get; set; }
    }
}
