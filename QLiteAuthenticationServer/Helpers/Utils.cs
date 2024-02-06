using QLite.Data.Models.Auth;
using QLiteAuthenticationServer.Services;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace QLiteAuthenticationServer.Helpers
{
    public static class Utils
    {
        private static QuavisEncryptionService _encService;
        private static readonly Random rnd = new Random();
        private static readonly TimeSpan TOKEN_EXPIRATION = TimeSpan.FromDays(3);

        // pseudo constructor
        public static void Initialize(QuavisEncryptionService encService)
            =>
            _encService = encService;


        #region CUSTOM TOKEN MANAGEMENT

        /// <summary>
        /// Generates a custom password change token with non-DI aware symmetric encryption.
        /// </summary>
        /// <param name="userId">ID of the user who will recieve the token.</param>
        /// <param name="currentPassword">The current password of the user (hashed version).</param>
        /// <returns>The generated token, or empty string if an error occured.</returns>
        public static async Task<string> GeneratePasswordChangeToken(string userId, string currentPassword)
            => await Task.Run(() => // offload the CPU-bound work to a thread pool thread using `Task.Run`
            {
                try
                {
                    DateTime expiration = DateTime.UtcNow + TOKEN_EXPIRATION; // determine when the token will expire
                    string payload = $"{currentPassword}|{expiration:O}"; // append expiration date to payload
                    return _encService.GetProtector(userId).Protect(payload); // get protector with the user id and encrypt the payload with it
                }
                catch (Exception ex)
                {
                    // TODO logla
                    return string.Empty; // you must handle this response at the calling method
                }
            });

        /// <summary>
        /// Validates a custom password change token with non-DI aware symmetric encryption.
        /// </summary>
        /// <param name="userId">ID of the user who is using the token to change his password.</param>
        /// <param name="currentPassword">The current password of the user (hashed version). NOT the new password!!</param>
        /// <returns>The generated token, or empty string if an error occured.</returns>
        public static async Task<bool> ValidatePasswordChangeToken(string tokenToValidate, string userId, string currentPassword)
            => await Task.Run(() => // offload the CPU-bound work to a thread pool thread using `Task.Run`
            {
                try
                {
                    string decrypted = _encService.GetProtector(userId).Unprotect(tokenToValidate);

                    string[] parts = decrypted.Split('|');
                    if (parts.Length != 2)
                        return false;

                    string decryptedPassword = parts[0];
                    if (!DateTime.TryParseExact(parts[1], "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime expiration))
                        return false;

                    TimeSpan remainingTime = expiration - DateTime.UtcNow;
                    if (remainingTime <= TimeSpan.Zero)
                        return false;

                    return currentPassword == decryptedPassword;
                }
                catch (Exception ex)
                {
                    // TODO log the exception
                    return false;
                }
            });

        /// <summary>
        /// Generates a custom email change token with non-DI aware symmetric encryption.
        /// </summary>
        /// <param name="userId">ID of the user who will recieve the token.</param>
        /// <param name="currentEmail">The current email of the user.</param>
        /// <returns>The generated token, or empty string if an error occured.</returns>
        public static async Task<string> GenerateEmailChangeToken(string userId, string currentEmail)
            => await Task.Run(() => // offload the CPU-bound work to a thread pool thread using `Task.Run`
            {
                try
                {
                    DateTime expiration = DateTime.UtcNow + TOKEN_EXPIRATION; // determine when the token will expire
                    string payload = $"{currentEmail}|{expiration:O}"; // append expiration date to payload
                    return _encService.GetProtector(userId).Protect(payload); // get protector with the user id and encrypt the payload with it
                }
                catch (Exception ex)
                {
                    // TODO logla
                    return string.Empty; // you must handle this response at the calling method
                }
            });

        /// <summary>
        /// Validates a custom email change token with non-DI aware symmetric encryption.
        /// </summary>
        /// <param name="userId">ID of the user who is using the token to change his password.</param>
        /// <param name="currentEmail">The current email of the user.</param>
        /// <returns>The generated token, or empty string if an error occured.</returns>
        public static async Task<bool> ValidateEmailChangeToken(string tokenToValidate, string userId, string currentEmail)
            => await Task.Run(() => // offload the CPU-bound work to a thread pool thread using `Task.Run`
            {
                try
                {
                    string decrypted = _encService.GetProtector(userId).Unprotect(tokenToValidate);

                    // split the decrypted payload into email and expiration date
                    string[] parts = decrypted.Split('|');
                    if (parts.Length != 2)
                        return false; // malformed token

                    string decryptedEmail = parts[0];
                    if (!DateTime.TryParseExact(parts[1], "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime expiration))
                        return false; // invalid expiration date format

                    // check expiration
                    TimeSpan remainingTime = expiration - DateTime.UtcNow;
                    if (remainingTime <= TimeSpan.Zero)
                        return false; // token expired

                    // get protector with the user id, unprotect the token with it, and compare it against current password
                    return currentEmail == decryptedEmail;
                }
                catch (Exception ex)
                {
                    // TODO logla
                    return false;
                }
            });

        #endregion


        #region SMTP SECRET ENCRYPTION & DECRYPTION

        /// <summary>
        /// Handles the encryption of an entire `SmtpConfiguration` object.
        /// </summary>
        /// <param name="model">The model with plaintext properties.</param>
        /// <returns>The model with all of its properties encrypted.</returns>
        public static SmtpConfiguration EncryptSmtpConfiguration(SmtpConfiguration model)
            =>
            new SmtpConfiguration()
            {
                fromAddress = EncryptSmtpSecret(model.fromAddress),
                fromName = EncryptSmtpSecret(model.fromName),
                smtpPassword = EncryptSmtpSecret(model.smtpPassword),
                smtpServer = EncryptSmtpSecret(model.smtpServer),
                smtpUsername = EncryptSmtpSecret(model.smtpUsername),
                smtpPort = model.smtpPort
            };

        /// <summary>
        /// Instantiates a protector with relevant purpose string and encrypts the payload with it
        /// </summary>
        private static string EncryptSmtpSecret(string payload)
            =>
            _encService.GetProtector(_encService.encryptionPurposeString).Protect(payload);

        /// <summary>
        /// Handles the decryption of an entire `SmtpConfiguration` object.
        /// </summary>
        /// <param name="model">The model with encrypted properties.</param>
        /// <returns>The model with all of its properties decrypted.</returns>
        public static SmtpConfiguration DecryptSmtpConfiguration(SmtpConfiguration model)
        {
            if (model == null)
                return new SmtpConfiguration();
            else
                return new SmtpConfiguration()
                {
                    fromAddress = DecryptSmtpSecret(model.fromAddress),
                    fromName = DecryptSmtpSecret(model.fromName),
                    smtpPassword = DecryptSmtpSecret(model.smtpPassword),
                    smtpServer = DecryptSmtpSecret(model.smtpServer),
                    smtpUsername = DecryptSmtpSecret(model.smtpUsername),
                    smtpPort = model.smtpPort
                };
        }

        /// <summary>
        /// Instantiates a protector with relevant purpose string and decrypts the payload with it
        /// </summary>
        private static string DecryptSmtpSecret(string payload)
            => _encService.GetProtector(_encService.encryptionPurposeString).Unprotect(payload);

        #endregion


        #region TOTP

        /// <summary>
        /// Temporarily generates a QR image with the user's unique two factor secret into 'AppData/Local/quavis-temp', Base64 encodes it into memory, deletes the image from disc, and returns the encoded image.
        /// </summary>
        public static async Task<string> GenerateQRImage(ApplicationUser _user)
            => await Task.Run(() => // offload the CPU-bound work to a thread pool thread using `Task.Run`
            {
                try
                {
                    string _base32code = Base32.ToBase32String(Encoding.ASCII.GetBytes(_user.TwoFactorSecret));
                    string _code = "otpauth://totp/" + _user.UserName + "?secret=" + _base32code + "&issuer=Quavis%20Auth%20Server&accountname=" + _user.Email;
                    QRCodeEncoderLibrary.QREncoder q = new QRCodeEncoderLibrary.QREncoder
                    {
                        ErrorCorrection = QRCodeEncoderLibrary.ErrorCorrection.H,
                        ModuleSize = 20
                    };
                    q.Encode(_code);

                    #region create folder if it doesn't already exist
                    string tempPath = System.IO.Path.Combine(System.Environment.GetEnvironmentVariable("LOCALAPPDATA"), "quavis-temp");
                    string tempFilePath = System.IO.Path.Combine(tempPath, Guid.NewGuid().ToString() + ".png");
                    if (!System.IO.Directory.Exists(tempPath))
                        System.IO.Directory.CreateDirectory(tempPath);
                    #endregion

                    q.SaveQRCodeToPngFile(tempFilePath);
                    FileStream stream = System.IO.File.Open(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                    BinaryReader br = new BinaryReader(stream);

                    byte[] qr = br.ReadBytes((int)stream.Length);
                    var encodedImg = "data:image/jpg;base64, " + Convert.ToBase64String(qr.ToArray());
                    System.IO.File.Delete(tempFilePath); // we should delete the actual image after loading it into memory
                    return encodedImg;
                }
                catch (Exception ex)
                {
                    // todo logla
                    return string.Empty;
                }
            });

        /// <summary>
        /// Validates OTP code for given user two factor secret.
        /// Every user has a unique two factor secret value in user table.
        /// </summary>
        public static bool ValidateOTP(string userTwoFactorSecret, string code)
        {
            try
            {
                return code == new OtpNet.Totp(Encoding.ASCII.GetBytes(userTwoFactorSecret)).ComputeTotp();
            }
            catch (Exception ex)
            {
                // TODO logla
                return false;
            }
        }

        #endregion


        #region RANDOM PASSWORD GENERATION

        /// <summary>
        /// Generates a random password containing 2 lowercase characters, 2 uppercase characters, 2 numbers and 2 non-alphanumeric characters and returns the shuffled result.
        /// </summary>
        public static async Task<string> GenerateRandomPassword()
            => await Task.Run(() => // offload the CPU-bound work to a thread pool thread using `Task.Run`
            {
                var lowercases = "abcdefghijklmnopqrstuvwxyz";
                var uppercases = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var numbers = "0123456789";
                var specials = "!@#$%^&*()-_+=";

                StringBuilder password = new StringBuilder();

                password.Append(lowercases[rnd.Next(0, lowercases.Length)]);
                password.Append(lowercases[rnd.Next(0, lowercases.Length)]);

                password.Append(uppercases[rnd.Next(0, uppercases.Length)]);
                password.Append(uppercases[rnd.Next(0, uppercases.Length)]);

                password.Append(numbers[rnd.Next(0, numbers.Length)]);
                password.Append(numbers[rnd.Next(0, numbers.Length)]);

                password.Append(specials[rnd.Next(0, specials.Length)]);
                password.Append(specials[rnd.Next(0, specials.Length)]);

                return ShuffleString(password.ToString());
            });

        /// <summary>
        /// Fisher-Yates shuffle algorithm. Shuffles the letters in a given string.
        /// </summary>
        private static async Task<string> ShuffleString(string input)
            => await Task.Run(() => // offload the CPU-bound work to a thread pool thread using `Task.Run`
            {
                char[] chars = input.ToCharArray();

                for (int i = chars.Length - 1; i > 0; i--)
                {
                    int j = rnd.Next(0, i + 1);

                    // Swap characters at positions i and j
                    char temp = chars[i];
                    chars[i] = chars[j];
                    chars[j] = temp;
                }

                return new string(chars);
            });

        #endregion


        #region OTHER

        // unused?
        public static X509Certificate2 GetCertificateFromStore(string certName)
        {

            // Get the certificate store for the current user.
            X509Store store = new X509Store(StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);

                // Place all certificates in an X509Certificate2Collection object.
                X509Certificate2Collection certCollection = store.Certificates;
                // If using a certificate with a trusted root you do not need to FindByTimeValid, instead:
                // currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, true);
                X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, false);
                if (signingCert.Count == 0)
                    return null;
                // Return the first certificate in the collection, has the right name and is current.
                return signingCert[0];
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// Checks whether a provided string is a valid email address using regular expression.
        /// </summary>
        public static bool IsValidEmail(string email)
            => Regex.IsMatch(email, @"^[\w\.-]+@[\w\.-]+\.\w+$");

        /// <summary>
        /// The fingerprint is checked on client app upon every request to prevent cookie theft attacks
        /// </summary>
        /// <returns>The constructed fingerprint, or empty string if an error occured.</returns>
        public static string GenerateFingerprint(HttpContext context)
        {
            try
            {
                string ipAddress = context.Connection.RemoteIpAddress?.ToString(); // acquire client ip address. you will get `::1` when debugging locally

                var fullUA = context.Request.Headers["User-Agent"].ToString(); // acquire user agent

                if (string.IsNullOrEmpty(fullUA) || string.IsNullOrEmpty(ipAddress))
                    return "";

                // parse user agent into smaller segments that are still useful
                var lengthOfUA = fullUA.Length;
                var last25CharsOfUA = fullUA.Substring(lengthOfUA - 25);

                return $"{ipAddress}-{last25CharsOfUA}-{lengthOfUA}";
            }
            catch (Exception ex)
            {
                // TODO logla
                return "";
            }
        }


        /// <summary>
        /// 2FA icin her kullanicinin unique bir keyi olmasi gerekiyor, bu method kullanici olusturulurken bu keyi set etmek icin kullaniliyor.
        /// </summary>
        public static string GenerateTwoFactorKey()
            => Guid.NewGuid().ToString().ToLower().Replace("-", "").Substring(0, 8);

        /// <summary>
        /// Creating an X509Certificate2 object directly in C# without having to use PowerShell or any other external tool.
        /// </summary>
        /// <param name="subjectName">Can be the name of the company using the app, e.g., "Quavis", "Pegasus", "THY", etc.</param>
        /// <param name="certPassword">Password for the certificate being generated.</param>
        /// <returns></returns>
        public static X509Certificate2 GenerateSelfSignedCertificate(string subjectName, string certPassword)
        {
            using (var rsa = RSA.Create(2048)) // Generate a new 2048-bit RSA key
            {
                var request = new CertificateRequest($"cn={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                // Set the certificate validity period
                var startDate = DateTimeOffset.UtcNow;
                var endDate = startDate.AddYears(5);

                // Create the self-signed certificate
                var certificate = request.CreateSelfSigned(startDate, endDate);

                // Export the certificate with a private key (optional)
                // You can set a password for the exported PFX
                var password = new SecureString();
                foreach (char c in certPassword) password.AppendChar(c);

                var pfxBytes = certificate.Export(X509ContentType.Pfx, password);
                return new X509Certificate2(pfxBytes, password, X509KeyStorageFlags.Exportable);
            }
        }

        /// <summary>
        /// Username valdiaiton icin kullaniliyor
        /// </summary>
        public static bool ContainsNonEnglishCharacters(string input)
            => new Regex(@"[^a-zA-Z]").IsMatch(input);

        #endregion
    }

}
