using Microsoft.AspNetCore.DataProtection;

namespace QLiteAuthenticationServer.Services
{
    public class QuavisEncryptionService
    {
        // PROPERTIES
        private IDataProtectionProvider dataProtectionProvider;
        public readonly string encryptionPurposeString = @"

                   ____                    _     
                  / __ \                  (_)    
                 | |  | |_   _  __ ___   ___ ___ 
                 | |  | | | | |/ _` \ \ / / / __|
                 | |__| | |_| | (_| |\ V /| \__ \
                  \___\_\\__,_|\__,_| \_/ |_|___/
                                         
        ".Trim(); // !!!!! DON'T CHANGE THE PURPOSE STRING !!!!!

        // CTOR
        public QuavisEncryptionService(IDataProtectionProvider dataProtectionProvider)
            =>
            this.dataProtectionProvider = dataProtectionProvider;

        /// <summary>
        /// Returns a protector for the provided purpose string
        /// </summary>
        public IDataProtector GetProtector(string purposeString)
            =>
            this.dataProtectionProvider.CreateProtector(purposeString);

        /// <summary>
        /// Encrypts the given payload with default purpose string
        /// </summary>
        /// <param name="payload">Payload to encrypt</param>
        /// <returns>Encrypted payload</returns>
        public string Encrypt(string payload)
            =>
            GetProtector(encryptionPurposeString).Protect(payload);

        /// <summary>
        /// Decrypts the given payload with default purpose string
        /// </summary>
        /// <param name="payload">Payload to decrypt</param>
        /// <returns>Decrypted payload</returns>
        public string Decrypt(string payload)
            =>
            GetProtector(encryptionPurposeString).Unprotect(payload);

    }

}
