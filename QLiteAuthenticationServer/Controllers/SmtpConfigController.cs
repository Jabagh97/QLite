using Microsoft.AspNetCore.Mvc;
using QLiteAuthenticationServer.Context;
using QLiteAuthenticationServer.Helpers;

namespace QLiteAuthenticationServer.Controllers
{
    //public class SmtpConfigController : Controller
    //{
    //    private readonly ApplicationDbContext context;
    //    public SmtpConfigController(ApplicationDbContext context) => this.context = context;


    //    [HttpGet]
    //    public async Task<IActionResult> Index()
    //    {
    //        if (await context.SmtpConfig.AnyAsync())
    //            return View(Utils.DecryptSmtpConfiguration(context.SmtpConfig.FirstOrDefault()));
    //        else
    //            return View();
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> Update(SmtpConfiguration model)
    //    {
    //        try
    //        {
    //            if (ModelState.IsValid)
    //            {
    //                var encryptedModel = Utils.EncryptSmtpConfiguration(model);

    //                if (await context.SmtpConfig.AnyAsync()) // if there is an existing record
    //                {
    //                    var existingSettings = await context.SmtpConfig.FirstAsync();

    //                    await context.SmtpConfig.ExecuteUpdateAsync(setter => setter
    //                        .SetProperty(x => x.fromName, b => encryptedModel.fromName)
    //                        .SetProperty(x => x.fromAddress, b => encryptedModel.fromAddress)
    //                        .SetProperty(x => x.smtpPassword, b => encryptedModel.smtpPassword)
    //                        .SetProperty(x => x.smtpServer, b => encryptedModel.smtpServer)
    //                        .SetProperty(x => x.smtpUsername, b => encryptedModel.smtpUsername)
    //                        .SetProperty(x => x.smtpPort, b => encryptedModel.smtpPort)
    //                        );

    //                    await context.SaveChangesAsync();
    //                }
    //                else // if this is the first time saving
    //                {
    //                    await context.SmtpConfig.AddAsync(encryptedModel);
    //                    await context.SaveChangesAsync();
    //                }

    //                return Json(new { success = true });
    //            }
    //            else
    //            {
    //                // If model state is not valid, return validation errors as JSON
    //                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
    //                return Json(new { success = false, errors = errors });
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // TODO logla
    //            return Json(new
    //            {
    //                success = false,
    //                error = "An unexpected error occured."
    //            });
    //        }
    //    }

    //}

}
