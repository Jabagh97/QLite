using MailKit.Security;
using MimeKit;
using QLiteAuthenticationServer.Context;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace QLiteAuthenticationServer.Helpers
{
    #region OTHER CLASSES

    /// <summary>
    /// Gets read from database every time a job is added.
    /// </summary>
    public class SmtpConfiguration
    {
        public int Id { get; set; } // this is needed for change tracker to work

        [Display(Name = "SMTP Server")]
        [Required]
        public string smtpServer { get; set; }

        [Display(Name = "SMTP Port")]
        [Required]
        public int smtpPort { get; set; }

        [Display(Name = "SMTP Username")]
        [Required]
        public string smtpUsername { get; set; }

        [Display(Name = "SMTP Password")]
        [Required]
        public string smtpPassword { get; set; }

        [Display(Name = "From Name")]
        [Required]
        public string fromName { get; set; }

        [Display(Name = "From Address")]
        [Required]
        public string fromAddress { get; set; }
    }

    /// <summary>
    /// Defined during job registration.
    /// </summary>
    public class Email
    {
        public string toName { get; set; }
        public string toAddress { get; set; }
        public string subject { get; set; }
        public TextPart body { get; set; }
        public SmtpConfiguration smtpConfig { get; set; }
    }

    /// <summary>
    /// Instantiated internally. Represents the in-memory job that is added to the queue.
    /// </summary>
    public class EmailJob
    {
        public Email email { get; set; }
        public int retryCount { get; set; } = 0;
    }

    #endregion

    public class EmailSender
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<EmailSender> logger;
        private SemaphoreSlim semaphore = new SemaphoreSlim(50); // max 50 tasks at once

        public EmailSender(
            IServiceProvider ServiceProvider,
            ILogger<EmailSender> Logger
            )
        {
            serviceProvider = ServiceProvider;
            logger = Logger;
        }

        /// <summary>
        /// Responsible for taking jobs and scheduling them to be concurrently handled in a controlled manner.
        /// </summary>
        //public async Task AddJobAsync(Email email)
        //{
        //    logger.LogInformation("`AddJob` invoked");

        //    // VALIDATE EMAIL
        //    if (!ValidateEmail(ref email))
        //    {
        //        logger.LogInformation("Invalid email parameter");
        //        throw new ArgumentException("Invalid email parameter");
        //    }

        //    try
        //    {
        //        // when accessing a scoped service (ApplicationDbContext) from a singleton service (EmailSender), a scope must be manually created to avoid memory leaking (captive dependancy)
        //        // we read SMTP settings from database and inject it into the email
        //        using (IServiceScope scope = serviceProvider.CreateScope())
        //        {
        //            ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //            email.smtpConfig = Utils.DecryptSmtpConfiguration(context.SmtpConfig.FirstOrDefault());
        //        }

        //        // VALIDATE SMTP CONFIG
        //        if (!ValidateSmtpConfiguration(email.smtpConfig))
        //        {
        //            logger.LogInformation("Invalid SMTP configuration");
        //            throw new Exception("Invalid SMTP configuration");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // TODO logla
        //        throw new Exception("An unexpected error happened");
        //    }

        //    // Asynchronously execute `DoJob` within a semaphore-controlled block.
        //    // The semaphore ensures that a limited number of `DoJob` tasks can run in parallel.
        //    // If the semaphore is full, WaitAsync will pause until it's safe to proceed.
        //    // The semaphore is released after `DoJob` completes, regardless of success or failure.
        //    await semaphore.WaitAsync();
        //    try
        //    {
        //        await DoJobAsync(new EmailJob() { email = email });
        //    }
        //    finally
        //    {
        //        semaphore.Release();
        //    }
        //}

        /// <summary>
        /// Sends an email concurrently.
        /// </summary>
        //public async Task DoJobAsync(EmailJob job)
        //{
        //    logger.LogInformation("`StartWorking` invoked");

        //    if (job.retryCount >= 6)
        //    {
        //        logger.LogInformation("Job queue is empty or retry count exceeded, stopping.");
        //        return;
        //    }

        //    try
        //    {
        //        #region PREPARE EMAIL

        //        var emailMessage = new MimeMessage();
        //        emailMessage.Body = job.email.body;
        //        emailMessage.From.Add(new MailboxAddress(job.email.smtpConfig.fromName, job.email.smtpConfig.fromAddress));
        //        emailMessage.To.Add(new MailboxAddress(name: job.email.toName, address: job.email.toAddress));
        //        emailMessage.Subject = job.email.subject;

        //        #endregion

        //        // no need to run this asyncronously, as the method itself is already running asynchronously in a non-blocking manner.
        //        using (var client = new SmtpClient())
        //        {
        //            client.Connect(job.email.smtpConfig.smtpServer, job.email.smtpConfig.smtpPort, SecureSocketOptions.StartTls);
        //            client.Authenticate(job.email.smtpConfig.smtpUsername, job.email.smtpConfig.smtpPassword);
        //            client.Send(emailMessage);
        //            client.Disconnect(true);
        //        }

        //        logger.LogInformation("Email sent");
        //    }
        //    catch (Exception ex)
        //    {
        //        // wait a bit
        //        logger.LogInformation("Failed to send email, waiting a bit to reattempt later. Error details:" + ex.Message);
        //        await Task.Delay(TimeSpan.FromSeconds(10)); // use `Task.Delay` and not `Thread.Sleep` for non-blocking delays!

        //        // reattempt after waiting
        //        ++job.retryCount;
        //        logger.LogInformation("Reattempting a failed job. Retry Count: " + job.retryCount);

        //        // Fire and forget. Continue executing DoJob() on the same logical thread, as we are already in an asynchronous context.
        //        // Makes absolutely sure that no context is captured for the continuation, with `ConfigureAwait(false)`.
        //        // `ConfigureAwait(false)` here will make sure that when DoJob is awaited internally, and that it won't attempt to marshal the continuation back to the original context. 
        //        // This can be beneficial to potentially avoid deadlocks or context-related issues in certain situations.
        //        _ = DoJobAsync(job).ConfigureAwait(false);
        //    }
        //}


        #region VALIDATION METHODS

        /// <summary>
        /// Validates an entire `Email` object.
        /// </summary>
        /// <returns>True if email is valid, false otherwise.</returns>
        private static bool ValidateEmail(ref Email email)
        {
            if (string.IsNullOrWhiteSpace(email.toAddress) || !Utils.IsValidEmail(email.toAddress))
                return false;

            if (string.IsNullOrWhiteSpace(email.toName))
                email.toName = email.toAddress; // if no recipient name is provided, set email as name.

            if (string.IsNullOrWhiteSpace(email.subject))
                return false;

            if (email.body == null)
                return false;

            return true;
        }

        /// <summary>
        /// Validates a `SmtpConfiguration` object. Used by `ValidateEmail` method.
        /// </summary>
        private static bool ValidateSmtpConfiguration(SmtpConfiguration config)
        {
            // Check if 'smtpServer' is not null or empty
            if (string.IsNullOrWhiteSpace(config.smtpServer))
                return false;

            // Check if 'smtpPort' is within the valid range of port numbers
            // Common SMTP ports are 25, 465, 587. Adjust the range as necessary for your application.
            if (config.smtpPort <= 0 || config.smtpPort > 65535)
                return false;

            // Check if 'smtpUsername' and 'smtpPassword' are not null or empty
            // Assuming that your SMTP server requires authentication
            if (string.IsNullOrWhiteSpace(config.smtpUsername) || string.IsNullOrWhiteSpace(config.smtpPassword))
                return false;

            // Check if 'fromName' and 'fromAddress' are not null or empty
            if (string.IsNullOrWhiteSpace(config.fromName) || string.IsNullOrWhiteSpace(config.fromAddress))
                return false;

            // Check if 'fromAddress' is in a valid email format
            if (!Utils.IsValidEmail(config.fromAddress))
                return false;

            return true;
        }

        #endregion
    }
}
