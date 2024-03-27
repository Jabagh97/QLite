using QLite.Data.CustomAttribute;
using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.Segment
{
    internal class SegmentViewModel
    {
        [Required]

        public string? Account { get; set; }
        [Required]

        public string? Name { get; set; }


        //[Boolean]
       // public bool? Default { get; set; }
       // [Boolean]

       // public bool? IsParent { get; set; }

       //// public string? Parent { get; set; }

       // public string? Prefix { get; set; }
    }
}