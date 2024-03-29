namespace KioskApp.Constants
{
    public class Errors
    {
        public static string KioskError = "Kiosk Error :";

        public static string AuthenticatioError = "The kiosk is either not registered in the system or the server is currently unavailable.";

        public static string AuthenticatioSolution = "Please verify the following steps:<br />" +
                             "1. Check the 'KioskID' value in the 'appsettings.json' file located in the kiosk application folder to ensure it matches the system records.<br />" +
                             "2. Contact your system administrator to confirm that the kiosk is registered in the system and to check the server's status.<br />" +
                             "3. Restart the Kiosk App.";

        public static string KioskTypeError = "The Kiosk Type configuration is missing.";

        public static string KioskTypeSolution = "To resolve this issue, please follow these steps:<br />" +
                             "1. Navigate to the Admin Portal and ensure the Kiosk type is correctly set and not left as 'None'.<br />" +
                             "2. Once verified or updated, please restart the Kiosk application for the changes to take effect.";


        public static string DefaultSegmentError = "A Default Segment is not assigned. Note: When opting for a 'Services Only' workflow, assigning a default segment is necessary.";


        public static string DefaultSegmentSolution = "To address this requirement, please carry out the following steps:<br />" +
                          "1. Access the 'Segments' page within the Admin Portal and ensure a default segment is designated by selecting the 'Default' checkbox for the desired segment.<br />" +
                          "2. Save your changes to confirm the default segment selection.<br />" +
                          "3. To apply these settings, restart the Kiosk application.";


        public static string SegmentStepError = "Segments not detected.";


        public static string SegmentStepSolution = "To resolve this issue, ensure segments are properly configured:<br />" +
                           "1. Visit the 'Segments' section in the Admin Portal. Verify that at least one segment exists. Ideally, set one as the default segment.<br />" +
                           "2. After adjustments, save the changes to confirm.<br />" +
                           "3. After adjustments, Check languages page and add a default languege if there is none.<br />" +

                           "4. Restart the Kiosk application to reflect the changes.";


        public static string ServiceStepError = "There are no services available at the moment. Please try again later.";


        public static string ServiceStepSolution = "If the issue persists,<br />" +
                             "check the service configuration and if there is a default language in the Admin Portal or contact technical support.<br />";




        public static string TicketStepError = "Encountered an issue with ticket generation or data retrieval.";


        public static string TicketStepSolution = "Please attempt the process again later. If the problem continues,<br />" +
            "consider restarting the Kiosk application or consult with the support team for further assistance.<br />";


    }
}
