using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Dtos;
using QLiteDataApi.Context;
using QLiteDataApi.SignalR;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Controllers.Kiosk
{
    public class KioskController : Controller
    {

        private readonly ApplicationDbContext _context;


        public KioskController(ApplicationDbContext context)
        {
            //_hubContext = hubContext;
            _context = context;
        }

        [HttpPost]
        [Route("api/Kiosk/GetTicket")]
        public async Task<IActionResult> GetNewTicketAsync(TicketRequestDto req)
        {
            try
            {
                // Fetch necessary data from the database
                var svcType = await _context.ServiceTypes.FindAsync(req.ServiceTypeId);
                var segment = await _context.Segments.FindAsync(req.SegmentId);
                var ticketPool = await _context.TicketPools
                    .FirstOrDefaultAsync(x => x.ServiceType == req.ServiceTypeId && x.Segment == req.SegmentId && x.Oid == req.TicketPoolId);

                // Perform validation checks

                // Generate ticket number
                var lastTicket = await _context.Tickets
                    .OrderByDescending(t => t.Number)
                    .FirstOrDefaultAsync(t => t.ServiceType == req.ServiceTypeId && t.Segment == req.SegmentId && t.TicketPool == ticketPool.Oid);

                var retNumber = lastTicket != null ? lastTicket.Number + 1 : ticketPool.RangeStart;

                // Create new ticket entity
                var newTicket = new Ticket
                {
                    // Populate ticket properties
                    // Oid = Guid.NewGuid().ToString(),
                    ServiceType = svcType.Oid,
                    Segment = segment.Oid,
                    // Populate other properties...
                    Number = retNumber
                };

                // Create new ticket state entity
                var newTicketState = new TicketState
                {
                    // Populate ticket state properties
                    // Oid = Guid.NewGuid().ToString(),
                    Ticket = newTicket.Oid,
                    // Populate other properties...
                };

                // Save changes to the database
                _context.Tickets.Add(newTicket);
                _context.TicketStates.Add(newTicketState);
                await _context.SaveChangesAsync();



                return Ok(newTicket);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                // Log the exception
                throw; // Rethrow the exception or return an appropriate response
            }
        }

        [HttpGet]
        [Route("api/Kiosk/GetServiceTypeList")]
        public IActionResult GetServiceTypeList(Guid segmentId)
        {


            // First query
            var lst = (from st in _context.ServiceTypes
                       join tp in _context.TicketPools on st.Oid equals tp.ServiceType
                       where st.Gcrecord == null
                       select st).ToList();


            // Filter further based on time conditions in memory
            //lst = lst.Where(x => x.ServiceStartTime.TimeOfDay < DateTime.Now.TimeOfDay &&
            //                     x.ServiceEndTime.AddSeconds(-1).TimeOfDay > DateTime.Now.TimeOfDay &&
            //                    !(x.BreakStartTime.TimeOfDay < DateTime.Now.TimeOfDay &&
            //                      x.BreakEndTime.TimeOfDay > DateTime.Now.TimeOfDay))
            //         .ToList();

            return Ok(lst);

        }

        [HttpGet]
        [Route("api/Kiosk/GetSegments")]
        public IActionResult GetSegments()
        {
            var segments = _context.Segments.Where(seg => seg.Gcrecord == null)
                .ToList();

            return Ok(segments);

        }
    }
}
