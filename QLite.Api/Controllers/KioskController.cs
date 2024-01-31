using Microsoft.AspNetCore.Mvc;
using Quavis.QorchLite.Hwlib.Printer;

namespace Quavis.QorchLite.Hwhost
{
    public class KioskController: ControllerBase
    {
        EmsePrinter _printer;
        public KioskController(EmsePrinter printer)
        {
            _printer = printer;
        }

        [HttpPost]
        [Route("/qlhw/print")]
        public virtual IActionResult PrintTicket([FromBody] string html)
        {
            _printer.Send(html);
            return Ok();
        }
    }
}
