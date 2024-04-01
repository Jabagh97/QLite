$(document).ready(function () {
    const table = $('#ticketStateTable').DataTable({
        responsive: true,
        dom: 'Bfrtipl',
        columns: [
            { data: 'DeskName' },
            { data: 'CalledTickets' },
            { data: 'TransferedTickets' },
            { data: 'Parked' },
            { data: 'WaitingTime' },
            { data: 'ServiceTime' },
        ],

    });

    // Function to fetch data
    function fetchData_ticketStates(startDate, endDate) {
        return $.ajax({
            url: `Reports/GetTicketStateReport?StartDate=${startDate}&EndDate=${endDate}`,
            method: 'GET',
            dataType: 'json'
        });
    }

    function updateTableWithTicketStates(startDate, endDate) {
        fetchData_ticketStates(startDate, endDate)
            .done(function (response) {
                const { Report, ServiceTypeCounts } = response;

                table.clear(); // Clear the table before adding new data
                table.rows.add(Report); // Add new data from 'Report'
                table.draw(); // Redraw the table

                initCharts(ServiceTypeCounts); // Initialize charts with 'ServiceTypeCounts'
            })
            .fail(function (jqXHR, textStatus) {
                console.error('Failed to fetch ticket states: ' + textStatus);
            });
    }


    // Event Listeners and Initial Data Load
    // Automatically update the table for today's date when the page loads
    const today = new Date().toISOString().split('T')[0];
    updateTableWithTicketStates(today, today);
    $('#dateRangeDropdown').text('Today'); 

    $('.dropdown-item').on('click', function (e) {
        e.preventDefault();
        const range = $(this).data('range');
        const rangeText = $(this).text(); // Get the text of the clicked dropdown item
        let startDate, endDate;

        switch (range) {
            case 'today':
                startDate = endDate = new Date().toISOString().split('T')[0];
                break;
            case 'this-week':
                const currentDate = new Date();
                const diff = currentDate.getDate() - currentDate.getDay();
                const firstDayOfPastWeek = new Date(currentDate.setDate(diff - 6)).toISOString().split('T')[0];
                const lastDayOfPastWeek = new Date(currentDate.setDate(diff)).toISOString().split('T')[0];
                startDate = firstDayOfPastWeek;
                endDate = lastDayOfPastWeek;
                break;
            case 'last-30-days':
                endDate = new Date().toISOString().split('T')[0];
                startDate = new Date(new Date().setDate(new Date().getDate() - 30)).toISOString().split('T')[0];
                break;
            case 'this-year':
                const yearStart = new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0];
                const yearEnd = new Date().toISOString().split('T')[0];
                startDate = yearStart;
                endDate = yearEnd;
                break;
            case 'custom':
                $('#customDateRange').show();
                $('#dateRangeDropdown').text(rangeText); // Update the button text for "Custom Pick"
                return; // Return early so the table doesn't update immediately for custom range
            default:
                console.log('Invalid selection');
                return;
        }

        // Update the table and dropdown button text for non-custom selections
        updateTableWithTicketStates(startDate, endDate);
        $('#dateRangeDropdown').text(rangeText);

        // Hide custom date range inputs if not selected
        if (range !== 'custom') {
            $('#customDateRange').hide();
        }
    });

    const onDateChange = () => {
        const startDateInput = document.getElementById('startDateTS');
        const endDateInput = document.getElementById('endDateTS');
        const startDate = startDateInput.value;
        const endDate = endDateInput.value;
        if (startDate && endDate) {
            updateTableWithTicketStates(startDate, endDate);
            $('#dateRangeDropdown').text('Custom Range Selected');
        }
    };

    document.getElementById('startDateTS').addEventListener('change', onDateChange);
    document.getElementById('endDateTS').addEventListener('change', onDateChange);


    function initCharts(serviceTypeCounts) {
        let serviceTypeData = serviceTypeCounts.map(item => ({
            name: item.ServiceTypeName,
            y: item.Count
        }));


        const themeMode = localStorage.getItem('data-bs-theme-mode');
        const isDarkMode = themeMode === 'dark';

        // Define colors based on the theme
        const chartBackgroundColor = isDarkMode ? '#15171c' : '#FFFFFF'; 
        const textColor = isDarkMode ? '#FFFFFF' : '#343a40'; 

        // Generate the pie chart
        Highcharts.chart('serviceTypePieChart', { 
            chart: {
                type: 'pie',
                backgroundColor: chartBackgroundColor,
                plotShadow: true,
                style: {
                    fontFamily: 'Arial'
                }

            },
            title: {
                text: 'Service Type Allocation Among Tickets',
                style: {
                    color: textColor // Adjust title color based on theme
                }
            },
            tooltip: {
                pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        format: '<b>{point.name}</b>: {point.y} ({point.percentage:.1f}%)',
                        connectorColor: 'silver'
                    }
                }
            },
            series: [{
                name: 'Service Types',
                colorByPoint: true,
                data: serviceTypeData
            }]
        });
    }



});