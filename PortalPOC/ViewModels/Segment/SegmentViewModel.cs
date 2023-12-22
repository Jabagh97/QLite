namespace PortalPOC.ViewModels.Segment
{
    internal class SegmentViewModel
    {
        public string? Account { get; set; }

        public string? Name { get; set; }

        public bool? Default { get; set; }

        public bool? IsParent { get; set; }

        public string? Parent { get; set; }

        public string? Prefix { get; set; }
    }
}