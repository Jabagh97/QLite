using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.ViewModels
{
    public class ErrorViewModel
    {
        public ErrorViewModel()
        {
        }

        public ErrorViewModel(string error, string simpleErrorMessage)
        {
            Error = new ErrorMessage { Error = error };
            this.simpleErrorMessage = simpleErrorMessage;
        }

        public string simpleErrorMessage { get; set; } = string.Empty;
        public ErrorMessage Error { get; set; }
    }
}
