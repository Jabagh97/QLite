using KioskApp.HardwareManager.Display.Protocols;
using KioskApp.HardwareManager.Display.Settings;
using KioskApp.HardwareManager.Hardware;
using QLite.Data.Dtos;
using QLite.Kio;
using QLite.KioskLibrary.Hardware;
using Quavis.QorchLite.Hwlib.Display;
using Quavis.QorchLite.Hwlib.Hardware;
using Serilog;
using System.Drawing;
using System.Xml.Linq;
using static QLite.Data.Models.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KioskApp.HardwareManager.Display
{
    public class EmseDisplayEthernet : IDisplay
    {
        private readonly IProtocolEthernet _protocol;
        private IReadOnlyList<VCTerminalDisplay> _displays;
        private VCDisplaySettingsNetwork _settings = new();
        public Type SettingsType => typeof(VCDisplaySettingsNetwork);
        private readonly Dictionary<string, (string ticket, int rowId)[]> _lastDisplayedTickets = new();
        private readonly Dictionary<byte, (int slotNo, NetworkDevice device)> _networkDevices = new();

        private const byte SOP = 0x03;
        private const byte PARAMETER_ID = 0xBF;
        private const byte PARAMETER_DATA_LENGTH = 0x02;
        private const byte DISCOVER_DEVICE_ID_MIN = 0x31;
        private const byte DISCOVER_DEVICE_ID_MAX = 0xFE;


        public NetworkDevice Device;

        RealDeviceBase IDisplay.Device => Device;


        public EmseDisplayEthernet(NetworkDevice dev)
        {
            Device = dev;
            _protocol = new ProtocolEthernet();

        }

        public HwStatusDto GetHwStatus()
        {
            var connected = Device.Connected != false;
            var notok = !connected;
            return new HwStatusDto
            {
                Device = QLDevice.Display,
                Connected = connected,
                Ok = !notok
            };
        }

        public async void Initialize(object settings)
        {
            _settings = settings as VCDisplaySettingsNetwork;

            ConfigVCForAddionalSettings();

            if (Device.Initialize(_settings.RealDeviceSettings.First()))
            {
                Log.Debug($"NetWork Device initialized");

                await ResetDispalysAsync();
                await DiscoverDisplaysAsync();
                await ApplySettingsAsync();

                Device.DeviceConnectionEvent += Device_DeviceConnectionEvent;

            }
        }

        private void Device_DeviceConnectionEvent(object sender, bool? e)
        {
        }
        protected void ConfigVCForAddionalSettings()
        {
            var displays = new List<VCTerminalDisplay>();

            foreach (var main in _settings.MainDisplays ?? Enumerable.Empty<VCMainDisplay>())
            {
                main.Terminals.ForEach(x => { x.RowId = x.RowId; x.DisplayNo += ProtocolHDot.BaseTerminalID; });
                foreach (var row in main.RowIds)
                {
                    main.Settings ??= new VCTerminalSettings();
                    main.Settings.BreakMessage = row.BreakMessage;

                    displays.Add(new VCTerminalDisplay
                    {
                        IsMain = true,
                        RowId = row.RowId,
                        DisplayNo = (byte)(row.RowId + ProtocolHDot.BaseMainID), // TODO: overflow durumu?
                        Direction = (int)DisplayArrowDirection.NoArrow,
                        Settings = GetValueOrDefault(main.Settings),
                    });
                }

                var ticketNumbers = new (string, int)[main.RowIds.Count];
                if (!_lastDisplayedTickets.TryAdd(main.Name, ticketNumbers))
                {
                    throw new Exception("MainDisplay names mustbe unique");
                }
            }
            _settings.Terminals?.ForEach(x =>
            {
                var settings = GetValueOrDefault(x.Settings);
                byte displayNo = (byte)(x.RowId + ProtocolHDot.BaseTerminalID); // TODO: overflow durumu?
                x.DisplayNo = displayNo;
                x.Settings = settings;
                displays.Add(x);
            });

            _displays = displays;
        }

        private VCTerminalSettings GetValueOrDefault(VCTerminalSettings settings) => new()
        {
            DotHeight = settings?.DotHeight ?? _settings.DefaultSettings?.DotHeight ?? 0,
            DotWidth = settings?.DotWidth ?? _settings.DefaultSettings?.DotWidth ?? 0,
            ArabicDelayTime = settings?.ArabicDelayTime ?? _settings.DefaultSettings?.ArabicDelayTime ?? 0,
            DimmingTime = settings?.DimmingTime ?? _settings.DefaultSettings?.DimmingTime ?? 0,
            FlashingCount = settings?.FlashingCount ?? _settings.DefaultSettings?.FlashingCount ?? 0,
            MessageIdleTime = settings?.MessageIdleTime ?? _settings.DefaultSettings?.MessageIdleTime ?? 0,
            BreakMessage = !System.String.IsNullOrEmpty(settings?.BreakMessage) ? settings?.BreakMessage : _settings.DefaultSettings?.BreakMessage,
            BreakMessageArabic = !System.String.IsNullOrEmpty(settings?.BreakMessageArabic) ? settings?.BreakMessageArabic : _settings.DefaultSettings?.BreakMessageArabic,
            Font = new VCTerminalSettingsFont
            {
                FontName = !System.String.IsNullOrEmpty(settings?.Font?.FontName) ? settings?.Font?.FontName : _settings.DefaultSettings?.Font?.FontName,
                FontSize = settings?.Font?.FontSize ?? _settings.DefaultSettings?.Font?.FontSize ?? 0,
                FontStyle = settings?.Font?.FontStyle ?? _settings.DefaultSettings?.Font?.FontStyle ?? FontStyle.Regular,
                FontWeight = settings?.Font?.FontWeight ?? _settings.DefaultSettings?.Font?.FontWeight ?? FontWeightType.Thin,
                BitmapContrast = settings?.Font?.BitmapContrast ?? _settings.DefaultSettings?.Font?.BitmapContrast ?? 0,
                BitmapStrech = settings?.Font?.BitmapStrech ?? _settings.DefaultSettings?.Font?.BitmapStrech ?? false,
            },
        };

        private async Task ResetDispalysAsync()
        {
            var rowId = 1;

           
                for (int slot = 0; slot < 6 && rowId <= _settings.Terminals.Count; slot++)
                {
                    DisplayResetMessage(
                        rowId + ProtocolEthernet.BaseTerminalID,
                        "Emse",
                        rowId,
                        (DisplayArrowDirection)5,
                        _settings.Terminals[rowId - 1].Settings,
                        Device,
                        slot
                    );

                    await Task.Delay(500);
                    rowId++;
                }
            

        }
        private bool DisplayResetMessage(int displayNo, string ticketNo, int ticketDisplayNo, DisplayArrowDirection direction, VCTerminalSettings settings, NetworkDevice device, int slotNo)
        {
            var ticket = new TicketInfo
            {
                SlotNumber = slotNo,
                DisplayNo = displayNo,
                Direction = direction,
                TicketNumber = ticketNo,
                TicketDisplayNo = ticketDisplayNo,
                Settings = settings
            };

            var buffer = _protocol.ProduceTicketMessage(ticket);

            return device.Write(buffer);
        }
        private async Task DiscoverDisplaysAsync()
        {
            var tasks = DiscoverDisplaysAsync(Device);
            await Task.WhenAll(tasks);
        }

        private async Task DiscoverDisplaysAsync(NetworkDevice device)
        {
            await Task.Delay(100);
            try
            {
                byte[] buffer = { SOP, PARAMETER_ID, PARAMETER_DATA_LENGTH, DISCOVER_DEVICE_ID_MIN, DISCOVER_DEVICE_ID_MAX };
                var response = device.WriteAndWaitResponse(buffer, 0);

                while (response[1] == 144)
                {
                    await Task.Delay(100);
                    response = device.Read();
                }

                int startIndex = 3;
                const int RANGE_SIZE = 32;
                for (int slot = 0; slot < 6; slot++)
                {
                    var range = new Range(startIndex, startIndex + RANGE_SIZE);
                    var displayIds = response.Take(range).Where(x => x != 0).ToList();
                    foreach (var id in displayIds)
                    {
                        _networkDevices[id] = (slot, device);
                        Log.Information("Device with ID : " + id + " Found in Port : " + slot);
                    }

                    startIndex += RANGE_SIZE;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);

            }
        }


        private async Task ApplySettingsAsync()
        {
            if (_displays == null) return;

            foreach (var row in _displays)
            {
                await ApplySettingsForRowAsync(row);
            }

            await ShowMarqueeAsync();
        }

        private async Task ApplySettingsForRowAsync(VCTerminalDisplay row)
        {
            byte[] buffer;

            if (_networkDevices.ContainsKey((byte)row.DisplayNo))
            {
                var (slotNo, device) = _networkDevices[(byte)row.DisplayNo];

                // Settings
                buffer = _protocol.ProduceSettingsMessage(slotNo, row.DisplayNo, row.Settings.FlashingCount ?? 0, row.Settings.DimmingTime ?? 0);
                device.Write(buffer);
                await Task.Delay((int)((buffer.Length * 1.1) + 250));

                // Arabic delay time
                buffer = _protocol.ProduceArabicDelayTime(slotNo, row.DisplayNo, row.Settings.ArabicDelayTime ?? 0);
                device.Write(buffer);
                await Task.Delay((int)((buffer.Length * 1.1) + 250));

                // İlk açışılta display no göster
                buffer = _protocol.ProduceDisplayNumberMessage(row.DisplayNo, row.RowId, slotNo, row.Settings.DotHeight ?? 0, row.Settings.DotWidth ?? 0);
                device.Write(buffer);
                await Task.Delay((int)((buffer.Length * 1.1) + 250));

            }
        }

        private async Task ShowMarqueeAsync()
        {
            if (_settings.InitialMarqueeDelay > 0)
            {

                await Task.Delay(_settings.InitialMarqueeDelay * 1000);

                foreach (var row in _displays)
                {
                    if (_networkDevices.ContainsKey((byte)row.DisplayNo))
                    {
                        var (slotNo, device) = _networkDevices[(byte)row.DisplayNo];

                        var buffer = _protocol.ProduceMarquee(slotNo, row.DisplayNo, row.Settings.BreakMessage, row.Settings.MessageIdleTime ?? 0);

                        await Task.Delay((int)((buffer.Length * 1.1) + 250));

                        device.Write(buffer);
                    }

                }
            }
        }

        public void Send(QueNumData qnumDataObject)
        {

            int.TryParse(qnumDataObject.DisplayNo, out int rowId);
            var terminal = _settings.Terminals?.FirstOrDefault(x => x.RowId == rowId);
            if (terminal is not null)
            {
                // Desk / Terminal / Counter ticket gösterme
                DisplayTicket(terminal.DisplayNo, qnumDataObject.TicketNo, terminal.RowId, (DisplayArrowDirection)terminal.Direction, terminal.Settings);

                // Ana ekranda göster?
                if (qnumDataObject.SendToMain && _settings.MainDisplays != null)
                {
                    // mainDisplays: Tanımlı ana ekran(lar)
                    foreach (var main in _settings.MainDisplays)
                    {
                        // Ana ekranda o terminalin tanımlı ok yönüne göre gösterim yapılmalı
                        var terminalOfMainDisplay = main.Terminals.FirstOrDefault(x => x.RowId == rowId);
                        if (terminalOfMainDisplay is not null)
                        {
                            // Ana ekranda satırların bir alta kaydırılması
                            var newDisplayedTickets = ShiftTicketNumbers(main.Name, qnumDataObject.TicketNo, terminal.RowId);

                            for (int i = 0; i < main.RowIds.Count; i++)
                            {
                                var mainRow = main.RowIds[i];
                                var (ticket, ticketRowId) = newDisplayedTickets[i];
                                var mainDisplay = _displays.FirstOrDefault(x => x.RowId == mainRow.RowId);
                                DisplayTicket(mainDisplay.DisplayNo, ticket, ticketRowId, (DisplayArrowDirection)terminalOfMainDisplay.Direction, mainDisplay.Settings);
                            }
                        }
                    }
                }



            }
        }

        private bool DisplayTicket(int displayNo, string ticketNo, int ticketDisplayNo, DisplayArrowDirection direction, VCTerminalSettings settings)
        {
            var (slotNo, device) = _networkDevices[(byte)displayNo];

            var ticket = new TicketInfo
            {
                SlotNumber = slotNo,
                DisplayNo = displayNo,
                Direction = direction,
                TicketNumber = ticketNo,
                TicketDisplayNo = ticketDisplayNo,
                Settings = settings
            };

            var buffer = _protocol.ProduceTicketMessage(ticket);

            return device.Write(buffer);
        }

        private (string Ticket, int RowId)[] ShiftTicketNumbers(string mainDisplayName, string ticketNo, int rowId)
        {
            var lastDisplayedTickets = _lastDisplayedTickets[mainDisplayName];
            var newDisplayedTickets = new (string, int)[lastDisplayedTickets.Length];
            Array.Copy(lastDisplayedTickets, 0, newDisplayedTickets, 1, lastDisplayedTickets.Length - 1);
            newDisplayedTickets[0] = (ticketNo, rowId); // yeni gösterilen ticket en üstte
            _lastDisplayedTickets[mainDisplayName] = newDisplayedTickets;
            return newDisplayedTickets;
        }




    }
}
