using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.KappWorkflow
{
    internal class KappWorkflowViewModel
    {
        [Required]

        public string? Name { get; set; }

        public string? SessionType { get; set; }

        public string? DesignData { get; set; }

        public string? RestartProfile { get; set; }

    }
}