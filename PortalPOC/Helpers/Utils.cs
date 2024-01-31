using System.Reflection;
using System.Security.Claims;

namespace PortalPOC.Helpers
{
    public static class Utils
    {
        /// <summary>
        /// Used for file handling. Takes a user-submitted file and turns it into a byte array in Base64 binary-to-text encoding scheme.
        /// </summary>
        /// <param name="uploadedFile">The file ueer uploaded, of IFormFile? type</param>
        /// <returns>Returns byte array in Base64 format, or null if no file is provided</returns>
        public static byte[]? ImageFileHandler(IFormFile? uploadedFile)
        {
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    uploadedFile.CopyTo(memoryStream);
                    return memoryStream.ToArray(); // Now you have the logo data as a byte array (logoData)
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Used by the fingerprint middleware to generate a fingerprint with request info.
        /// </summary>
        /// <returns>The constructed fingerprint, or empty string if an error occured.</returns>
        public static string GenerateFingerprint(HttpContext context, bool isBehindLoadBalancer)
        {
            try
            {
                // acquire client ip address accordingly based on whether or not the server is behind a load balancer
                string ipAddress;
                if (isBehindLoadBalancer)
                    ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
                else
                    ipAddress = context.Connection.RemoteIpAddress?.ToString();

                // acquire user agent
                var fullUA = context.Request.Headers["User-Agent"].ToString();

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

        public static string GetCurrentUserName(ClaimsPrincipal user)
            =>
            !string.IsNullOrWhiteSpace(user?.Identity?.Name) && user.Identity.IsAuthenticated
            ? user.Identity.Name
            : "";
    }
}
