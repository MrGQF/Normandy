using System;

namespace Normandy.Identity.UserData.Domain.Models
{
    public partial class AcBase
    {
        public long Thsid { get; set; }
        public int? Userid { get; set; }
        public string Appid { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public byte? Status { get; set; }
        public byte? Deleted { get; set; }
        public string Note { get; set; }
        public string Identifier { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
    }
}
