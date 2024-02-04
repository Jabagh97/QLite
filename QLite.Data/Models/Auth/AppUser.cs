using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Models.Auth
{
    public class AppUser : IdentityUser
    {
        public Guid Oid { get; set; }

        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? CreatedDateUtc { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? ModifiedDateUtc { get; set; }

        public Guid? Account { get; set; }

        public Guid? Branch { get; set; }

        public Guid? Desk { get; set; }

        public virtual Account? AccountNavigation { get; set; }


        public virtual Branch? BranchNavigation { get; set; }

        public virtual Desk? DeskNavigation { get; set; }


        public virtual ICollection<DeskMacroSchedule> DeskMacroSchedules { get; } = new List<DeskMacroSchedule>();

        public int? Gcrecord { get; set; }

    }
}
