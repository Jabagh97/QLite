using KioskApp.HardwareManager.Display.Settings;
using Quavis.QorchLite.Hwlib.Hardware;
using Serilog;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace KioskApp.HardwareManager.Hardware
{
    public class NetworkDevice : RealDeviceBase
    {

        public override string Name => IpAddress;
        public string IpAddress => Settings.IpAddress;

        byte[] _buffer = null;
        NetworkSettings Settings;
        NetworkStream _stream = null;
        TcpClient _client = null;
        CancellationTokenSource readTaskCTS;

        public bool Initialize(object settings)
        {

            if (Initialized)
            {
                Log.Warning($"RealDevice - {Name} is already initialized.");
                return true;
            }
            Settings = settings as NetworkSettings;


            return Initialize();
        }

        private bool Initialize()
        {
            if (_stream != null && _stream.CanWrite)
                return true;

            try
            {
                _stream?.Dispose();
                if (!int.TryParse(Settings.Port, out int port))
                    port = 10000;

                _client = new TcpClient();
                _client.Connect(Settings.IpAddress, port); // Using Connect with handling
                _buffer = new byte[_client.ReceiveBufferSize];
                _stream = _client.GetStream();
                Initialized = true;
            }
            catch (SocketException ex)
            {
                Log.Error($"Connection failed: {ex.Message}", "NetworkDevice GetStream failed");
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "NetworkDevice GetStream failed");
                return false;
            }

            return true;
        }


        private void RunReadFromStreamJob()
        {
            readTaskCTS = new CancellationTokenSource();

            Task.Run(() =>
            {
                while (true)
                {
                    if (readTaskCTS.IsCancellationRequested)
                        break;

                    byte[] d = null;
                    try
                    {
                        if (_stream != null && _stream.DataAvailable)
                        {
                            ClearBuffer();
                            d = Read();
                            var data = Encoding.ASCII.GetString(d);
                            PublishDeviceData(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "asenkron read from NetworkDevice, dev:" + this.Name);
                    }

                    Thread.Sleep(10);
                }
            }, readTaskCTS.Token);
        }

        private void ClearBuffer()
        {
            for (int i = 0; i < _client.ReceiveBufferSize; i++)
            {
                _buffer[i] = 0x00;
            }
        }

        protected override void OnListenersRegistered()
        {
            if (_stream != null && _stream.CanRead)
                RunReadFromStreamJob();
        }

        public bool IsItYou(object settings)
        {
            var sett = settings as NetworkSettings;
            if (sett == null)
                return false;

            return sett.IpAddress == Settings?.IpAddress && sett.Port == Settings?.Port;
        }

        public byte[] Read()
        {
            try
            {
                _stream.Read(_buffer, 0, _buffer.Length);
                return _buffer;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "NetworkDevice Read failed");
                Initialize();
            }
            return null;
        }

        public bool Write(byte[] data)
        {
            try
            {
                if (_stream == null || _stream.CanWrite == false)
                    return false;
                _stream.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "NetworkDevice Write failed");
                return false;
            }
        }

        public byte[] WriteAndWaitResponse(byte[] data, int timeout = 100)
        {
            try
            {
                Write(data);
                Thread.Sleep(timeout);
                return Read();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public void Dispose()
        {
        }

        public bool CheckConnection()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var options = new PingOptions();
                    options.DontFragment = true;
                    // Sending a buffer of 32 bytes, which is the default size of ping:
                    byte[] buffer = new byte[32];
                    var reply = ping.Send(Settings.IpAddress, 1000, buffer, options);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }


    }

}
