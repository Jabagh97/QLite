using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Dtos;
using Quavis.QorchLite.Hwlib;
using Quavis.QorchLite.Hwlib.Display;
using Quavis.QorchLite.Hwlib.Printer;
using System.Security.Cryptography;
using KioskApp.Helpers;
using QLite.DesignComponents;
using KioskApp.Services;
using QLite.Data.CommonContext;

namespace KioskApp.Controllers
{
    public class TicketController : Controller
    {
        HwManager _hwman;
        private readonly HttpService _httpService;

        public TicketController(HttpService httpService, HwManager hwman)
        {
            _httpService = httpService;

            _hwman = hwman;


        }
      


        [HttpPost]
        public IActionResult PrintTicket([FromBody] TicketViewModel viewModel)
        {
            try
            {
                //var html = Helpers.Helpers.PrepareTicket(viewModel.Html);

             
                _hwman.Print(viewModel.Html);

                return Ok("Print successful");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Printing failed: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult DisplayTicket([FromBody] QueNumData queNumData)
        {
            try
            {
               

                _hwman.Display(queNumData);

                return Ok("Print successful");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Printing failed: {ex.Message}");
            }
        }

      
    }
}
public class TicketViewModel
{
    public string Html { get; set; }
}