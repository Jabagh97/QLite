using KioskApp.Constants;
using KioskApp.Helpers;
using KioskApp.Models;
using KioskApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.CommonContext;
using QLite.Data.Dtos;
using QLite.Data.Services;
using QLite.DesignComponents;
using Quavis.QorchLite.Hwlib;
using Quavis.QorchLite.Hwlib.Display;
using Serilog;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography;
using static QLite.Data.Models.Enums;

namespace KioskApp.Controllers
{
    /// <summary>
    /// Manages kiosk main operations including displaying segments, services, hardware requests, and handling tickets.
    /// </summary>
    public class KioskController : Controller
    {
        private readonly ApiService _apiService;
        private readonly HwManager _hwman;

        public KioskController(ApiService httpService, HwManager hwman)
        {
            _hwman = hwman;
            _apiService = httpService;

        }
        /// <summary>
        /// Serves as the entry point for the kiosk's main interface, loading the initial homepage view with necessary data.
        /// </summary>
        /// <returns>The main homepage view for the kiosk, populated with design data and other initial setup information.</returns>
        /// <remarks>
        /// This method attempts to initialize the homepage by gathering necessary data through <see cref="InitHomepage"/>.
        /// If successful, it returns the main view populated with the relevant model data. In the case of an exception,
        /// it logs the error and returns a 500 Internal Server Error status code.
        /// </remarks>
        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = await InitHomepage();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading the Kiosk page.");
                return StatusCode(500, "An internal server error has occurred.");
            }
        }

        /// <summary>
        /// Initializes the homepage by fetching necessary design data, language options, and resources based on the kiosk's hardware ID.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the home page's view model, populated with design data, available languages, and resources.</returns>
        /// <remarks>
        /// This method aggregates data needed for the kiosk's homepage, including design specifics, language options, and other resources.
        /// It sets the global context for languages, resources, and the current language based on the first available language option or a default value.
        /// The resulting <see cref="HomeAndDesPageDataViewModel"/> is then used to populate the homepage view.
        /// </remarks>
        private async Task<HomeAndDesPageDataViewModel> InitHomepage()
        {
            var viewModel = new HomeAndDesPageDataViewModel
            {
                DesPageData = await GetDesignData(KioskContext.KioskHwId, Step.WelcomePage.ToString()),
                KioskHwId = KioskContext.KioskHwId
            };

            KioskContext.Languages = await GetLanguageList();
            KioskContext.Resources = await GetResourceList();
            KioskContext.CurrentLanguage = KioskContext.Languages.FirstOrDefault()?.Oid ?? Guid.Empty;

            Session.homeAndDesPageData = viewModel;
            return viewModel;
        }

        /// <summary>
        /// Retrieves and displays the segment view for a given kiosk based on the hardware ID, potentially as the initial view.
        /// </summary>
        /// <param name="hwId">The hardware ID of the kiosk for which segments are being retrieved.</param>
        /// <param name="asFirstPage">Indicates whether this view should be treated as the initial page, affecting timeout settings.</param>
        /// <returns>A View or PartialView with the segments available for the specified kiosk.</returns>
        /// <remarks>
        /// This method fetches design data and segment options for the given kiosk. If this operation is intended
        /// as the first interaction (asFirstPage = true), the method adjusts the timeout to ensure an uninterrupted experience.
        /// Upon failure, it logs the exception and returns a 500 Internal Server Error.
        /// </remarks>
        public async Task<IActionResult> GetSegmentView(string hwId, bool asFirstPage = false)
        {
            try
            {
                var viewModel = new SegmentsAndDesignModel
                {
                    DesignData = await GetDesignData(hwId, Step.SegmentSelection.ToString()),
                    Segments = await GetSegmentList()
                };

                Session.segmentsAndDesignModel = viewModel;

                if (asFirstPage)
                {

                    KioskContext.Languages = await GetLanguageList();
                    KioskContext.Resources = await GetResourceList();
                    KioskContext.CurrentLanguage = KioskContext.Languages.FirstOrDefault()?.Oid ?? Guid.Empty;

                    //Cancel Timeout if first page 
                    viewModel.DesignData.PageTimeOut = 0;
                    return View("Views/Kiosk/Segments.cshtml", viewModel);

                }

                return PartialView("~/Views/Kiosk/Segments.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load segments view.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves and displays the service selection view for a specified segment of a kiosk, optionally as the first page.
        /// </summary>
        /// <param name="segmentOid">The unique identifier of the segment for which services are being retrieved.</param>
        /// <param name="hwId">The hardware ID of the kiosk, used for fetching design data specific to the kiosk.</param>
        /// <param name="asFirstPage">Indicates whether this view should be treated as the initial page, affecting timeout settings.</param>
        /// <returns>A View or PartialView displaying the services available for the specified segment and kiosk.</returns>
        /// <remarks>
        /// Fetches design data and services for a given segment of the kiosk. Adjusts the page timeout if indicated
        /// as the first page. On failure, logs the exception details and returns a 500 Internal Server Error status.
        /// </remarks>
        public async Task<IActionResult> GetServiceView(Guid segmentOid, string hwId, bool asFirstPage = false)
        {

            try
            {
                var viewModel = new ServicesAndDesignModel
                {
                    DesignData = await GetDesignData(hwId, Step.ServiceTypeSelection.ToString()),
                    Services = await GetServiceList(segmentOid)
                };

                Session.selectedSegment = segmentOid;

                Session.servicesAndDesignModel = viewModel;

                if (asFirstPage)
                {

                    KioskContext.Languages = await GetLanguageList();
                    KioskContext.Resources = await GetResourceList();
                    KioskContext.CurrentLanguage = KioskContext.Languages.FirstOrDefault()?.Oid ?? Guid.Empty;

                    //Cancel Timeout if first page 
                    viewModel.DesignData.PageTimeOut = 0;

                    return View("Views/Kiosk/Services.cshtml", viewModel);

                }

                return PartialView("Views/Kiosk/Services.cshtml", viewModel);


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load services.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        /// <summary>
        /// Processes a ticket request for selected services and displays the ticket information, including a QR code.
        /// </summary>
        /// <param name="ticketRequest">The data transfer object containing the request details for ticket generation.</param>
        /// <returns>A PartialView displaying the generated ticket information.</returns>
        /// <remarks>
        /// Submits a ticket request based on selected services and returns a view with the ticket details. The method
        /// fetches additional design data for ticket presentation. On failure to retrieve ticket data or any exception,
        /// it logs the issue and returns a 500 Internal Server Error.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> GetTicketView([FromBody] TicketRequestDto ticketRequest)
        {
            try
            {
                ticketRequest.SegmentId = Session.selectedSegment;

                var ticket = await _apiService.PostGenericRequest<Ticket>("api/Kiosk/GetTicket", ticketRequest);

                if (ticket == null) return StatusCode(500, "Failed to retrieve ticket data");

                var hwId = KioskContext.Config.GetValue<string>("KioskID");

                var designData = await GetDesignData(hwId, Step.PagePrint.ToString());

                var model = new TicketAndDesPageDataViewModel
                {
                    Ticket = ticket,
                    DesPageData = designData
                };

                Session.ticketAndDesPageData = model;


                return PartialView("Views/Kiosk/Ticket.cshtml", model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching ticket.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        /// <summary>
        /// Fetches a list of resources available for the kiosk interface from the API.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of resources.</returns>
        /// <remarks>
        /// This method asynchronously calls the API to retrieve a list of resources such as images or texts used within the kiosk's UI.
        /// The resources are expected to be customized for the specific hardware ID of the kiosk.
        /// </remarks>
        private async Task<List<Resource>> GetResourceList()
        {
            return await _apiService.GetDesignResponse<List<Resource>>($"api/Kiosk/GetResourceList");

        }


        /// <summary>
        /// Asynchronously retrieves a list of languages supported by the kiosk.
        /// </summary>
        /// <returns>A task that results in a list of languages available for selection in the kiosk interface.</returns>
        /// <remarks>
        /// This method makes an asynchronous API call to obtain a list of languages that the user can select for the kiosk interface,
        /// enabling localization and personalization of the user experience based on language preference.
        /// </remarks>
        private async Task<List<Language>> GetLanguageList()
        {
            return await _apiService.GetDesignResponse<List<Language>>($"api/Kiosk/GetLanguageList");
        }


        /// <summary>
        /// Retrieves design-related data for a specific step of the kiosk's operation, based on the kiosk's hardware ID.
        /// </summary>
        /// <param name="hwId">The hardware ID of the kiosk, identifying the specific device.</param>
        /// <param name="step">The step of the operation (e.g., WelcomePage, SegmentSelection) for which design data is needed.</param>
        /// <returns>A task that results in the design data needed for the specified step of the kiosk's operation.</returns>
        /// <remarks>
        /// This method fetches design configurations such as themes, layouts, and other UI elements tailored for a specific step in the kiosk's workflow,
        /// enhancing the user experience by providing a consistent look and feel across different parts of the interaction process.
        /// </remarks>
        private async Task<DesPageData> GetDesignData(string hwId, string step)
        {
            return await _apiService.GetDesignResponse<DesPageData>($"api/Kiosk/GetDesignByKiosk/{step}/{hwId}");
        }

        /// <summary>
        /// Retrieves a list of segments that the user can navigate through in the kiosk interface.
        /// </summary>
        /// <returns>A task that results in a list of segment DTOs representing the different sections or categories available in the kiosk UI.</returns>
        /// <remarks>
        /// Segments are major parts or categories of services or information the kiosk provides to the user,
        /// and this method fetches them as a preparatory step for displaying to the user.
        /// </remarks>
        private async Task<List<SegmentDto>> GetSegmentList()
        {
            return await _apiService.GetGenericResponse<List<SegmentDto>>("api/Kiosk/GetSegments");
        }

        /// <summary>
        /// Fetches a list of service types available within a specific segment of the kiosk's offerings.
        /// </summary>
        /// <param name="segmentOid">The unique identifier for the segment whose service types are being requested.</param>
        /// <returns>A task that results in a list of service type DTOs relevant to the specified segment.</returns>
        /// <remarks>
        /// This method provides the detailed breakdown of services available under a specific segment,
        /// allowing for further user interaction and selection within the kiosk interface.
        /// </remarks>
        private async Task<List<ServiceTypeDto>> GetServiceList(Guid segmentOid)
        {

            return await _apiService.GetGenericResponse<List<ServiceTypeDto>>($"api/Kiosk/GetServiceTypeList/{segmentOid}");

        }

        /// <summary>
        /// Changes the current language of the kiosk interface, updating the session and reloading the current view with the selected language.
        /// </summary>
        /// <param name="LangID">The unique identifier of the language to switch to.</param>
        /// <param name="step">The current step of the kiosk operation, determining which view to reload with the new language.</param>
        /// <returns>A PartialView corresponding to the current operation step, rendered in the selected language.</returns>
        /// <remarks>
        /// This method allows for dynamic language switching within the kiosk interface, enhancing accessibility and user satisfaction by supporting multiple languages.
        /// In case of an unrecognized step, it returns a 404 Not Found response.
        /// </remarks>
        [HttpPost]
        public IActionResult ChangeLanguage(Guid LangID, string step)
        {
            try
            {
                KioskContext.CurrentLanguage = LangID;

                switch (step)
                {
                    case "WelcomePage":
                        return PartialView("~/Views/Kiosk/Index.cshtml", Session.homeAndDesPageData);

                    case "SegmentSelection":
                        return PartialView("~/Views/Kiosk/Segments.cshtml", Session.segmentsAndDesignModel);

                    case "ServiceTypeSelection":
                        return PartialView("~/Views/Kiosk/Services.cshtml", Session.servicesAndDesignModel);

                    default:
                        return NotFound("The specified step is not recognized.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        #region Hardware Requests

        [HttpPost]
        public IActionResult PrintTicket([FromBody] TicketViewModel viewModel)
        {
            try
            {
                var copies = Session.ticketAndDesPageData.Ticket.CopyNumber;

                if (copies.HasValue)
                {
                    // Execute the loop the number of times specified by copies.
                    for (int i = 0; i < copies.Value; i++)
                    {
                        _hwman.Print(viewModel.Html);
                    }
                }


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
        public IActionResult CheckKiosk()
        {
            return Ok(_hwman.GetKioskHwStatus());
        }
        #endregion

    }
}