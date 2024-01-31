namespace Quavis.QorchLite.Hwlib.Call
{
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Quavis.QorchLite.Common;
    using Quavis.QorchLite.Data.Dto;
    using Quavis.QorchLite.Data.Dto.Kapp;
    using Quavis.QorchLite.Hwlib.Display;
    using Quavis.QorchLite.Hwlib.Hardware;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using static Quavis.QorchLite.Data.Dto.Enums;

    public class EmseCall : ICallDevice
    {
        public string Name { get; } = "EmseCall";

        public Type SettingsType => typeof(VCPhysicalTerminalSettings);

        private readonly SortedList<char, int> PossibleChars = new SortedList<char, int>() { { 'A', 111 }, { 'B', 122 }, { 'C', 51 }, { 'D', 124 }, { 'E', 115 }, { 'F', 99 }, { 'H', 110 }, { 'J', 28 }, { 'L', 50 }, { 'N', 47 }, { 'P', 103 }, { 'R', 96 }, { 'U', 62 }, { 'Y', 94 }, { '1', 12 }, { '2', 117 }, { '3', 93 }, { '4', 78 }, { '5', 91 }, { '6', 123 }, { '7', 13 }, { '8', 127 }, { '9', 95 }, { '0', 63 }, { '-', 64 } };

        public Encoding DefaultProtocolEncoding { get; set; } = ASCIIEncoding.GetEncoding(28591);

        public EmseUsbQueNumDevice Device;
        RealDeviceBase ICallDevice.Device => Device;


        VCPhysicalTerminalSettings VCSettings;
        List<string> PatternsTerminal = new List<string>();
        public List<PhysicalTerminal> Terminals = new List<PhysicalTerminal>();
        public readonly static List<byte> EndOfProtocol = new List<byte>() { 10, 13 };


        IConfiguration _config;
        public EmseCall(IConfiguration config, EmseUsbQueNumDevice dev)
        {
            _config = config;
            Device = dev;
            Device.DeviceConnectionEvent += Device_DeviceConnectionEvent;
            //RealDeviceComManager.RealDevices.Add(Device);
        }

        private void Device_DeviceConnectionEvent(object sender, bool? e)
        {
            if (e != true)
            {
                StopListenTerminals();
            }
            else if (e == true && cts == null)
            {
                StartListenTerminals();
            }

        }

        Dictionary<int, QLTerminalModel> QLTermianlList = new Dictionary<int, QLTerminalModel>();  //token ve status
        public void Initialize(object settings)
        {
            VCSettings = settings as VCPhysicalTerminalSettings;

            LoggerAdapter.Debug($"{Name} initializing:" + JsonConvert.SerializeObject(VCSettings));

            #region AdvancedTerminalPatterns
            string AdvancedTerminalPattern1 = "^.[";
            char[] atpProtocol = DefaultProtocolEncoding.GetChars(new byte[] { 130, 134 });

            foreach (char item in atpProtocol)
            {
                AdvancedTerminalPattern1 += item;
            }
            AdvancedTerminalPattern1 += "]$";

            string AdvancedTerminalPattern2 = "^." + DefaultProtocolEncoding.GetChars(new byte[] { 128 })[0] + "[0-9]{3,13}$";

            PatternsTerminal = new List<string>() { "^.N$", "^.[fedcab].{5}[LR]$", AdvancedTerminalPattern1, AdvancedTerminalPattern2 };
            #endregion



            StartListenTerminals();
        }

        CancellationTokenSource cts;
        void StopListenTerminals()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

        private void StartListenTerminals()
        {
            StopListenTerminals();
            cts = new CancellationTokenSource();
            var ct = cts.Token;
            Task.Run(() =>
            {

                Thread.Sleep(2000);
                for (int i = 0; i < VCSettings.Terminals.Count; i++)
                {

                    Terminals.Add(new PhysicalTerminal() { TerminalNo = i + 1, CustomerIsAvailable = false, TerminalIsActive = true, CommunicationLineNo = 1 });
                    var t = VCSettings.Terminals[i];

                    var jwt = HttpCaller.GetHttpReqAsync<JwtToken>($"terminalauth/token/{CommonCtx.KioskHwId}/{t.ToString()}", null).Result;
                    if (jwt == null)
                    {
                        LoggerAdapter.Warning($"terminal token can not be created, terminalNo:{t}");
                        continue;
                    }
                    if (!QLTermianlList.ContainsKey(t))
                        QLTermianlList.Add(t, new QLTerminalModel { Status = DeskActivityStatus.Open, Token = jwt.access_token, TokenExpTime = jwt.expires_in });
                }

                while (true)
                {
                    if (ct.IsCancellationRequested)
                        break;
                    StartListenTeminals(ct);
                    Thread.Sleep(10);
                }

            }, ct);
        }

        private async void DeviceQueryResponse(InterfacePackage package, CancellationToken ct)
        {
            LoggerAdapter.Debug($"terminal action -{package.PTerminalAction}- from terminal {package.TerminalNo} ");
            if (QLTermianlList.TryGetValue(package.TerminalNo, out QLTerminalModel terminal))
            {
                if (terminal.TokenExpTime < DateTime.UtcNow)
                {
                    var jwt = HttpCaller.GetHttpReqAsync<JwtToken>($"terminalauth/token/{CommonCtx.KioskHwId}/{package.TerminalNo}", null).Result;
                    if (jwt == null)
                    {
                        LoggerAdapter.Warning($"terminal token can not be created, terminalNo:{package.TerminalNo}");
                        return;
                    }

                    terminal.Token = jwt.access_token;
                }
                switch (package.PTerminalAction)
                {
                    case PhysicalTerminalAction.NewCallButton:

                        var ticket = await HttpCaller.GetHttpReqAsync<TicketStateDto>($"callticket/0?autocall=false", terminal.Token);
                        LoggerAdapter.Debug("EmseCallDevice Ticket:" + ticket?.TicketNumber);
                        if (ticket == null)
                        {
                            SendCb(package, "0", ct);
                            return;
                        }
                        terminal.LastTicketNumber = ticket.Prefix + ticket.TicketNumber;
                        SendCb(package, terminal.LastTicketNumber, ct);
                        break;
                    case PhysicalTerminalAction.WaitingButton:
                        var wl = await HttpCaller.GetHttpReqAsync<List<TicketDto>>($"getWaitingList", terminal.Token);
                        SendCb(package, wl?.Count.ToString() ?? "0", ct);
                        break;
                    case PhysicalTerminalAction.SleepModeButton:
                        var status = terminal.Status == DeskActivityStatus.Open ? DeskActivityStatus.Paused : DeskActivityStatus.Open;
                        await HttpCaller.GetHttpReqAsync<string>($"changestatus/{status}", terminal.Token);
                        terminal.Status = status;
                        SendCb(package, null, ct);
                        break;
                    case PhysicalTerminalAction.ProgramButton:
                        var resultString = Regex.Match(package.TicketNo, @"\d+").Value;
                        if (!int.TryParse(resultString, out int ticketNo))
                            return;

                        var calledTicket = await HttpCaller.GetHttpReqAsync<TicketStateDto>($"callticket/{ticketNo}?autocall=false", terminal.Token);
                        if (calledTicket == null)
                            return;
                        SendCb(package, calledTicket?.Prefix + calledTicket?.TicketNumber, ct);
                        break;
                    case PhysicalTerminalAction.ServiceTransferButton:
                        if (!int.TryParse(package.TicketNo, out int seqNo))
                            return;

                        TransferTicketDto transferService = new()
                        {
                            ServiceNo = seqNo,
                            TicketNote = "PhysicalTerminal",
                        };

                        var trRes = await HttpCaller.PostHttpReqAsync<TicketStateDto>($"transfer", transferService, terminal.Token);
                        if (trRes == null)
                        {
                            LoggerAdapter.Warning($"transfer request failed serviceSıraNo : {seqNo}");
                            SendCb(package, terminal.LastTicketNumber, ct);
                        }
                        else
                        {
                            SendBlinking(package, ct);
                        }

                        break;
                    case PhysicalTerminalAction.TerminalTransferButton:
                        if (!int.TryParse(package.TicketNo, out int deskNo))
                            return;

                        TransferTicketDto transferTicket = new()
                        {
                            TransferDeskNo = deskNo,
                            TicketNote = "PhysicalTerminal",
                        };

                        var res = await HttpCaller.PostHttpReqAsync<TicketStateDto>($"transfer", transferTicket, terminal.Token);
                        if (res == null)
                        {
                            LoggerAdapter.Warning($"transfer request failed targetDesk : {deskNo}");
                            SendCb(package, terminal.LastTicketNumber, ct);
                        }
                        else
                        {
                            SendBlinking(package, ct);
                        }

                        break;
                    default:
                        SendBlinking(package, ct);
                        return;
                }

            }
        }

        private void SendBlinking(InterfacePackage package, CancellationToken ct)
        {
            SendCb(package, "****", ct);
            Thread.Sleep(200);
            SendCb(package, "----", ct);
            Thread.Sleep(200);
            SendCb(package, "****", ct);
            Thread.Sleep(200);
            SendCb(package, "----", ct);
        }

        HttpCaller _httpCaller;

        HttpCaller HttpCaller
        {
            get
            {
                if (_httpCaller == null)
                {
                    var http = new HttpClient();
                    http.BaseAddress = new Uri("http://localhost:5000");
                    _httpCaller = new HttpCaller(http);
                }
                return _httpCaller;
            }
        }


        public void Send(InterfacePackage data)
        {
            SendQueryResponse(data, CancellationToken.None);
        }

        public void SendCb(InterfacePackage data, string ticketNo, CancellationToken ct)
        {
            var cbData = new InterfacePackage
            {
                PackageTime = DateTime.Now,
                PTerminalAction = data.PTerminalAction,
                ReceivedDeviceType = data.ReceivedDeviceType,
                TerminalNo = data.TerminalNo,
                TicketNo = ticketNo
            };

            SendQueryResponse(cbData, ct);
        }

        private void StartListenTeminals(CancellationToken ct)
        {
            if (Device.Connected != true)
                return;

            var endOfProtocol = DefaultProtocolEncoding.GetString(EndOfProtocol.ToArray(), 0, EndOfProtocol.Count);
            foreach (var terminal in Terminals)
            {
                if (ct.IsCancellationRequested)
                    break;
                var package = SendTerminalQuery(terminal);
                var res = Device.WriteAndWaitResponse(package, ct, 1000);
                //var res = new byte[] { };
                var received = DefaultProtocolEncoding.GetString(res);
                var dataArray = received.Split(endOfProtocol);
                //CommonContext.EmseLogger.Debug("PhysicalTerminal data:" + JsonConvert.SerializeObject(dataArray));
                if (string.IsNullOrEmpty(received) || dataArray == null || dataArray.Length < 1)
                {
                    //terminal.TerminalIsActive = false;
                    continue;
                }

                foreach (string dataitem in dataArray)
                {
                    if (string.IsNullOrEmpty(dataitem))
                        continue;

                    foreach (string patternitem_Terminal in PatternsTerminal)
                    {
                        Match match = Regex.Match(dataitem, patternitem_Terminal);

                        if (match.Success && match.Groups.Count == 1)
                        {

                            InterfacePackage NewReceivedPackage = new InterfacePackage();

                            NewReceivedPackage.PackageContent = match.Value;
                            NewReceivedPackage.PackageTime = DateTime.Now;
                            NewReceivedPackage.TerminalNo = Convert.ToInt32(Convert.ToByte(match.Value[0])) - 48;
                            NewReceivedPackage.ReceivedDeviceType = DeviceType.PhysicalTerminal;
                            if (match.Value.Length > 4)
                            {
                                NewReceivedPackage.TicketNo = match.Value.Substring(2, 1) + match.Value.Substring(4, 3);
                            }
                            switch (match.Value[1])
                            {
                                case 'N':
                                    terminal.TerminalIsActive = true;
                                    //DeviceQueryResponse(NewReceivedPackage);
                                    break;
                                case 'f':
                                    NewReceivedPackage.PTerminalAction = PhysicalTerminalAction.NewCallButton;
                                    DeviceQueryResponse(NewReceivedPackage, ct);
                                    break;
                                case 'e':
                                    NewReceivedPackage.PTerminalAction = PhysicalTerminalAction.WaitingButton;
                                    DeviceQueryResponse(NewReceivedPackage, ct);
                                    break;
                                case 'd':
                                    NewReceivedPackage.PTerminalAction = PhysicalTerminalAction.SleepModeButton;
                                    DeviceQueryResponse(NewReceivedPackage, ct);
                                    break;
                                case 'c':
                                    NewReceivedPackage.PTerminalAction = PhysicalTerminalAction.ProgramButton;
                                    DeviceQueryResponse(NewReceivedPackage, ct);
                                    break;
                                case 'a':
                                    NewReceivedPackage.PTerminalAction = PhysicalTerminalAction.ServiceTransferButton;
                                    DeviceQueryResponse(NewReceivedPackage, ct);
                                    break;
                                case 'b':
                                    NewReceivedPackage.PTerminalAction = PhysicalTerminalAction.TerminalTransferButton;
                                    DeviceQueryResponse(NewReceivedPackage, ct);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }



        public byte[] SendTerminalQuery(PhysicalTerminal Terminal)
        {
            List<byte> PackageToSend = new List<byte>();
            int CustomerAvailable = 0;
            if (Terminal.TerminalIsActive && Terminal.TerminalNo > 0 && Terminal.TerminalNo < 100)
            {
                PackageToSend.Add(Convert.ToByte(48 + Terminal.TerminalNo));
                PackageToSend.Add(Convert.ToByte('R'));
                if (Terminal.CustomerIsAvailable)
                {
                    CustomerAvailable = 1;
                }
                PackageToSend.Add(Convert.ToByte(48 + CustomerAvailable));
                PackageToSend.AddRange(EndOfProtocol);
            }
            return PackageToSend.ToArray();
        }

        public void SendQueryResponse(InterfacePackage package, CancellationToken ct)
        {
            if (package.ReceivedDeviceType != DeviceType.PhysicalTerminal)
                return;

            List<byte> PackageToSend = new List<byte>();
            switch (package.PTerminalAction)
            {
                case PhysicalTerminalAction.NewCallButton:

                    if (string.IsNullOrEmpty(package.TicketNo))
                    {
                        PackageToSend.Add(Convert.ToByte(48 + package.TerminalNo));
                        PackageToSend.Add(Convert.ToByte('X'));
                        PackageToSend.AddRange(EndOfProtocol);

                    }
                    else
                    {
                        PackageToSend.Add(Convert.ToByte(48 + package.TerminalNo));
                        PackageToSend.Add(Convert.ToByte('A'));
                        PackageToSend.AddRange(TerminalResponseProtocolH(package.TicketNo));
                        PackageToSend.AddRange(EndOfProtocol);
                    }

                    break;
                case PhysicalTerminalAction.WaitingButton:
                    if (!string.IsNullOrEmpty(package.TicketNo))
                    {
                        PackageToSend.Add(Convert.ToByte(48 + package.TerminalNo));
                        PackageToSend.Add(Convert.ToByte('L'));
                        PackageToSend.Add(49);

                        string waitingno = CheckNo(package.TicketNo);
                        PackageToSend.AddRange(ToDigits(waitingno));

                        PackageToSend.AddRange(EndOfProtocol);
                    }
                    else
                    {
                        PackageToSend.Add(Convert.ToByte(48 + package.TerminalNo));
                        PackageToSend.Add(Convert.ToByte('L'));
                        PackageToSend.AddRange(new byte[4] { 49, 48, 48, 48 });
                        PackageToSend.AddRange(EndOfProtocol);
                    }

                    break;
                case PhysicalTerminalAction.SleepModeButton:
                    break;
                case PhysicalTerminalAction.ProgramButton:

                    if (!string.IsNullOrEmpty(package.TicketNo))
                    {
                        PackageToSend.Add(Convert.ToByte(48 + package.TerminalNo));
                        PackageToSend.Add(Convert.ToByte('A'));
                        PackageToSend.AddRange(TerminalResponseProtocolH(package.TicketNo));
                        PackageToSend.AddRange(EndOfProtocol);
                    }
                    else
                    {
                        PackageToSend.Add(Convert.ToByte(48 + package.TerminalNo));
                        PackageToSend.Add(Convert.ToByte('L'));
                        PackageToSend.AddRange(new byte[4] { 49, 48, 48, 48 });
                        PackageToSend.AddRange(EndOfProtocol);
                    }
                    break;
                case PhysicalTerminalAction.ServiceTransferButton:

                    if (!string.IsNullOrEmpty(package.TicketNo))
                    {
                        PackageToSend.Add(Convert.ToByte(48 + package.TerminalNo));
                        PackageToSend.Add(Convert.ToByte('A'));
                        PackageToSend.AddRange(TerminalResponseProtocolH(package.TicketNo));
                        PackageToSend.AddRange(EndOfProtocol);
                    }
                    break;
                case PhysicalTerminalAction.TerminalTransferButton:

                    if (!string.IsNullOrEmpty(package.TicketNo))
                    {
                        PackageToSend.Add(Convert.ToByte(48 + package.TerminalNo));
                        PackageToSend.Add(Convert.ToByte('A'));
                        PackageToSend.AddRange(TerminalResponseProtocolH(package.TicketNo));
                        PackageToSend.AddRange(EndOfProtocol);
                    }
                    break;
                case PhysicalTerminalAction.LoginRequest:

                    if (string.IsNullOrEmpty(package.TicketNo))
                    {
                        PackageToSend.Add(Convert.ToByte(48 + package.TerminalNo));
                        PackageToSend.Add(129);
                        PackageToSend.Add(Convert.ToByte('T'));
                        PackageToSend.AddRange(EndOfProtocol);
                    }
                    else
                    {
                        PackageToSend.Add(Convert.ToByte(48 + package.TerminalNo));
                        PackageToSend.Add(129);
                        PackageToSend.Add(Convert.ToByte('F'));
                        PackageToSend.AddRange(EndOfProtocol);
                    }

                    break;


                case PhysicalTerminalAction.LoginNeeded:

                    if (string.IsNullOrEmpty(package.TicketNo))
                    {
                        PackageToSend.Add(Convert.ToByte(48 + package.TerminalNo));
                        PackageToSend.Add(129);
                        PackageToSend.Add(Convert.ToByte('F'));
                        PackageToSend.AddRange(EndOfProtocol);
                    }

                    break;

                default:
                    return;
            }

            var res = Device.Write(PackageToSend.ToArray());
        }
        protected byte[] ToDigits(string value)
        {
            if (value == null)
            {
                return null;
            }

            char[] digits = value.ToCharArray();
            byte[] output = DefaultProtocolEncoding.GetBytes(digits);

            return output;
        }

        private string CheckNo(string number)
        {
            if (string.IsNullOrEmpty(number))
                return "000";

            if (number.Length > 3)
            {
                return number.Substring(number.Length - 3);
            }
            else if (number.Length < 3)
            {
                return number.PadLeft(3, '0');
            }
            else
                return number;

        }

        private List<byte> TerminalResponseProtocolH(string TicketNo)
        {
            string HexValue = null;
            List<byte> Packet = new List<byte>();

            if (string.IsNullOrEmpty(TicketNo)) { TicketNo = " "; }

            if (TicketNo.Length == 4) { TicketNo = TicketNo.Substring(0, 1) + " " + TicketNo.Substring(1, 3); }

            while (TicketNo.Length < 5) { TicketNo = " " + TicketNo; }

            foreach (char TicketNoItem in TicketNo)
            {
                if (PossibleChars.ContainsKey(TicketNoItem))
                {
                    HexValue = Convert.ToString(this.PossibleChars[TicketNoItem], 16);
                }
                else
                {
                    HexValue = Convert.ToString(0, 16);
                }

                if (HexValue.Length == 1) { HexValue = "0" + HexValue; }

                foreach (var HexValueItem in HexValue.ToCharArray())
                {
                    string tmp = "3" + HexValueItem.ToString();
                    Packet.Add(Byte.Parse(tmp, System.Globalization.NumberStyles.HexNumber));
                }

            }

            return Packet;
        }


        public HwStatusDto GetHwStatus()
        {
            var connected = Device.Connected != false;
            var notok = !connected;
            return new HwStatusDto
            {
                Device = QLDevice.Terminal,
                Connected = connected,
                Ok = !notok
            };

        }
    }
}
