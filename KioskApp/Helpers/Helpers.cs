namespace KioskApp.Helpers
{
    public static class Helpers
    {

        public static string PrepareTicket(string html)
        {

           
            string htmlWithBase64Image = html.Replace("/media/logos/qlogo.png", $"data:image/png;base64,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");

            string styledHtml = @"
        <style>
                .card .card-header {
                    display: flex;
                    justify-content: space-between;
                    align-items: stretch;
                    flex-wrap: wrap;
                    min-height: 70px;
                    padding: 0 2.25rem;
                    color: var(--bs-card-cap-color);
                    background-color: var(--bs-card-cap-bg);
                    border-bottom: 1px solid var(--bs-card-border-color);
                }
            .mb-3 {
                margin-bottom: 0.75rem !important;
            }
            .align-items-center {
                align-items: center !important;
            }
            .justify-content-center {
                justify-content: center !important;
            }
            .d-flex {
                display: flex !important;
            }
            .card.card-dashed {
                font-size: 2rem;
                box-shadow: 0 2px 5px rgba(0, 0, 0, 0.4);
            }

           .w-100 {
    width: 100% !important;
}

            .card-body {
                padding: 1rem;
            }
img {
    overflow-clip-margin: content-box;
    overflow: clip;
}
          .img-fluid {
                max-width: 100%;
                height: auto;
                max-height: 40px;
                object-fit: contain;
            }

            .card-title {
                font-size: 1.5rem; /* Change font size as needed */
                margin-bottom: 1rem;
            }

            .font-weight-bold {
                font-weight: bold;
            }

            .text-center {
                text-align: center;
            }

            .card-footer {
                padding: 1rem;
            }
        </style>";

            html = htmlWithBase64Image + styledHtml;

            return html;
        }

    }


    public class TicketViewModel
    {
        public string Html { get; set; }
    }
}
