using ReScanVisualizer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Diagnostics;

#nullable enable

namespace ReScanVisualizer.Models.Pipes
{
    public class UDPPipe : PipeBase
    {
        private readonly ushort _port;
        public ushort Port => _port;

        private readonly MainViewModel _mainViewModel;
        private readonly Task _task;
        private UdpClient? _udpClient;
        private IPEndPoint _remoteEP;

        public event EventHandler<Exception>? ErrorThrowed;

        public UDPPipe(MainViewModel mainViewModel, ushort port)
        {
            _mainViewModel = mainViewModel;
            _port = port;
            _task = new Task(Run);
            _udpClient = null;
            _remoteEP = new IPEndPoint(IPAddress.Any, 0);
        }

        /// <summary>
        /// Try to start a UDP client on the specified port.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="SocketException"></exception>
        public override void Start()
        {
            if (!_isStarted)
            {
                if (Tools.IsPortInUse(_port, ProtocolType.Tcp) ||
                    Tools.IsPortInUse(_port, ProtocolType.Udp))
                {
                    throw new InvalidOperationException($"The port {_port} is already used.");
                }
#if DEBUG
                Trace.WriteLine($"UDP Pipe ({_port}) starting...");
#endif
                _udpClient = new UdpClient(_port);
#if DEBUG
                Trace.WriteLine($"UDP Pipe ({_port}) started");
#endif
                base.Start();
                _task.Start();
            }
        }

        /// <summary>
        /// Try to stop the UDP client on the specified port.
        /// </summary>
        /// <exception cref="SocketException"></exception>
        public override void Stop()
        {
            if (_isStarted)
            {
#if DEBUG
                Trace.WriteLine($"UDP Pipe ({_port}) closing...");
#endif
                _udpClient?.Close();
                _udpClient?.Dispose();
#if DEBUG
                Trace.WriteLine($"UDP Pipe ({_port}) closed");
#endif
                base.Stop();
                _udpClient = null;
            }
        }

        protected override void Run()
        {
            while (_isStarted && _udpClient != null)
            {
                try
                {
#if DEBUG
                    Trace.WriteLine($"UDP Pipe ({_port}) waiting...");
#endif
                    // Réception des données et adresse IP de l'expéditeur
                    byte[] data = _udpClient.Receive(ref _remoteEP);

                    // Convertir les données en chaîne de caractères
                    string message = Encoding.UTF8.GetString(data);
#if DEBUG
                    Trace.WriteLine($"UDP Pipe ({_port}) received: {message}");
#endif

                    string[] args = message.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    _mainViewModel.ModifierPipe.Pipe(args);
                }
                catch (Exception ex)
                {
                    OnErrorThrowed(ex);
                }
            }
        }

        private void OnErrorThrowed(Exception e)
        {
            ErrorThrowed?.Invoke(this, e);
        }
    }
}
