namespace PortalPOC.ViewModels.MacroRule
{
    public class MacroRuleViewModel
    {
        public string? Macro { get; set; }

        public string? ServiceType { get; set; }

        public string? Segment { get; set; }

        public bool? Transfer { get; set; }

        public int? ToThisDesk { get; set; }

        //public int? MaxWaitingTime { get; set; }

        //public int? MinWaitingTime { get; set; }

        public int? Sequence { get; set; }

        public string? Description { get; set; }
    }
}
