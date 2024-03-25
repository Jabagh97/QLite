using QLite.Data;
using QLite.DesignComponents;
using System.Text;

namespace KioskApp.Helpers
{
    public class ViewHelper
    {

        public static string RenderTicketOrDisplayComponent(DesCompDataText comp, Ticket ticket )
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"<div id=\"{comp.Id}\" ");
            sb.Append($"data-comp-id=\"{comp.Id}\" ");

            sb.Append("style=\"position: absolute; ");
            sb.AppendFormat("width:{0}; ", comp.Width);
            sb.AppendFormat("height:{0}; ", comp.Height);
            sb.AppendFormat("left: {0}; top:{1}; ", comp.PosX, comp.PosY);

            if (!string.IsNullOrEmpty(comp.CustomCss))
            {
                sb.Append(comp.CustomCss);
            }

            sb.Append("\"> ");
            if (ticket != null)
            {
                if (comp.InfoType == TicketInfoType.ServiceCode)
                {
                    sb.Append(ticket.ServiceCode);

                }
                if (comp.InfoType == TicketInfoType.Number)
                {
                    sb.Append(ticket.Number);

                }
                if (comp.InfoType == TicketInfoType.WaitingTickets)
                {
                    sb.Append(ticket.WaitingTickets);

                }
                if (comp.InfoType == TicketInfoType.ServiceTypeName)
                {
                    sb.Append(ticket.ServiceTypeName);

                }
                if (comp.InfoType == TicketInfoType.Segment)
                {
                    sb.Append(ticket.SegmentName);

                }
            }

            sb.Append("</div>");

            return sb.ToString();
        }


        public static string RenderGenericHtmlComponent(DesCompDataGenericHtml comp)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"<div id=\"{comp.Id}\" ");
            sb.Append($"data-comp-id=\"{comp.Id}\" ");
            sb.Append("style=\"position: absolute;");
            sb.AppendFormat("width:{0}; ", comp.Width);
            sb.AppendFormat("height:{0}; ", comp.Height);
            sb.AppendFormat("left: {0}; top:{1}; ", comp.PosX, comp.PosY);


            if (comp.GenCompType == HtmlCompType.Image)
            {
                // Apply additional styles for background image
                sb.Append("background-image: url(");
                sb.Append(comp.BgImageUrl);
                sb.Append("); ");
                sb.Append("background-size: cover; ");
                sb.Append("background-position: center; ");
                sb.Append("background-repeat: no-repeat; ");
            }

            if (!string.IsNullOrEmpty(comp.CustomCss))
            {
                sb.Append(comp.CustomCss);
            }

            sb.Append("\"> ");


            if (comp.GenCompType == HtmlCompType.YoutubeVideo)
            {
                string videoId = comp.YoutubeUrl.Split("/").Last().Split("?").First();
                // Build the iframe HTML string
                sb.Append("<div style=\"position: relative;padding-bottom: 56.25%; height: 0;\">");
                sb.Append("<iframe style=\"position: absolute; top: 0; left: 0; width: 100%; height: 100%; border: none;\"");
                sb.Append($"src=\"{comp.YoutubeUrl}?controls=0&mute=1&showinfo=0&rel=0&autoplay=1&loop=1&playlist={videoId}\">");
                sb.Append("</iframe>");
                sb.Append("</div>");
            }
            if (comp.GenCompType == HtmlCompType.Text)
            {
                sb.Append(comp.ButtonText);

            }

            if (comp.GenCompType == HtmlCompType.Date)
            {
                sb.Append(DateTime.Now.ToShortDateString());

            }

            sb.Append("</div>");

            return sb.ToString();
        }


        public static string RenderSegmentComponent(DesCompDataSegment comp, List<Segment> segments)
        {
            // Check if comp's SegmentID exists in the segments list
            if (segments == null || !segments.Any(seg => seg.Oid == comp.SegmentID))
            {
                return null; // or handle the case where SegmentID doesn't exist in segments
            }

            // Prepare CSS styles
            string cssStyles = string.IsNullOrEmpty(comp.CustomCss) ? "" : comp.CustomCss;

            // Construct the button HTML using StringBuilder
            StringBuilder sb = new StringBuilder();
            sb.Append($"<button id=\"{comp.Id}\" ");
            sb.Append($"data-comp-id=\"{comp.Id}\" ");
            sb.Append($"data-x=\"{comp.PosX}\" ");
            sb.Append($"data-y=\"{comp.PosY}\" ");
            sb.Append("class=\"resize-drag\" ");
            sb.Append("style=\"position: absolute; ");
            sb.AppendFormat("width:{0}; ", comp.Width);
            sb.AppendFormat("height:{0}; ", comp.Height);
            sb.AppendFormat("transform: translate({0}, {1}); ", comp.PosX, comp.PosY);
            sb.Append(cssStyles);
            sb.Append("\" onclick=\"loadServiceView('");
            sb.Append(comp.SegmentID);
            sb.Append("')\"> ");
            sb.Append(comp.ButtonText);
            sb.Append("</button>");

            return sb.ToString();
        }

        public static string RenderWfButtonComponent(DesCompDataWfButton comp)
        {
            string buttonText = comp.ButtonText;
            string buttonType = comp.BtnType.ToString().ToLower();
            return $"<button style=\"position: absolute; top: {comp.PosY}px; left: {comp.PosX}px;\" type=\"{buttonType}\">{buttonText}</button>";
        }


        public static string RenderServiceButton(DesCompDataServiceButton comp, List<ServiceType> services)
        {
            // Check if comp's SegmentID exists in the segments list
            if (services == null || !services.Any(seg => seg.Oid == comp.ServiceTypeOid))
            {
                return null; // or handle the case where SegmentID doesn't exist in segments
            }

            // Prepare CSS styles
            string cssStyles = string.IsNullOrEmpty(comp.CustomCss) ? "" : comp.CustomCss;

            // Construct the button HTML using StringBuilder
            StringBuilder sb = new StringBuilder();
            sb.Append($"<button id=\"{comp.Id}\" ");
            sb.Append($"data-comp-id=\"{comp.Id}\" ");
            sb.Append($"data-x=\"{comp.PosX}\" ");
            sb.Append($"data-y=\"{comp.PosY}\" ");
            sb.Append("class=\"resize-drag\" ");
            sb.Append("style=\"position: absolute; ");
            sb.AppendFormat("width:{0}; ", comp.Width);
            sb.AppendFormat("height:{0}; ", comp.Height);
            sb.AppendFormat("transform: translate({0}, {1}); ", comp.PosX, comp.PosY);
            sb.Append(cssStyles);
            sb.Append("\" onclick=\"svcTypeSelected('");
            sb.Append(comp.ServiceTypeOid);
            sb.Append("')\"> ");
            sb.Append(comp.ButtonText);
            sb.Append("</button>");

            return sb.ToString();

        }
    }
}
