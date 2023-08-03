using Avalonia.Collections;
using Emulator_ARM.Models;
using ReactiveUI;
using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Emulator_ARM.ViewModels
{
    public class RecieveMessage : ReactiveObject
    {
        private byte[]? _message;
        private string? _dateTime;

        public byte[]? message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public string? dateTime
        {
            get => _dateTime;
            set => this.RaiseAndSetIfChanged(ref _dateTime, value);
        }

        public RecieveMessage(byte[] data)
        {
            message = data;

            dateTime = DateTime.Now.ToLongTimeString();
        }
    }

    public class ViewModelBase : ReactiveObject
    {
        private bool _continue;
        public SerialPort _serialPort = new SerialPort();
        private string _selectedCom;
        public string selectedCom
        {
            get => _selectedCom;
            set => this.RaiseAndSetIfChanged(ref _selectedCom, value);
        }
        private AvaloniaList<string> _coms = new();
        public AvaloniaList<string> coms
        {
            get => _coms;
            set => this.RaiseAndSetIfChanged(ref _coms, value);
        }

        private AvaloniaList<RecieveMessage> _recieveMsg = new();
        public AvaloniaList<RecieveMessage> recieveMsg
        {
            get => _recieveMsg;
            set => this.RaiseAndSetIfChanged(ref _recieveMsg, value);
        }

        private AvaloniaList<RecieveMessage> _transmitMsg = new();
        public AvaloniaList<RecieveMessage> transmitMsg
        {
            get => _transmitMsg;
            set => this.RaiseAndSetIfChanged(ref _transmitMsg, value);
        }

        private PULD _puld = new();
        public PULD puld
        {
            get => _puld;
            set => this.RaiseAndSetIfChanged(ref _puld, value);
        }

        public ViewModelBase()
        {
            string[] str_coms = SerialPort.GetPortNames();
            foreach (string str in str_coms) { coms.Add(str); }
            selectedCom = str_coms[0];
        }

        public void uart_connect()
        {
            _serialPort.PortName = selectedCom;
            _serialPort.BaudRate = 115200;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.NewLine = 0x0D.ToString();

            _serialPort.ReadTimeout = 2000;
            _serialPort.WriteTimeout = 500;

            _serialPort.Open();
            _continue = true;


            wait_status_tick();
            get_status();


        }


        private int byteToInt(byte data1, byte data2, byte data3, byte data4)
        {
            int state = 0;
            state += data1;
            state <<= 8;
            state += data2;
            state <<= 8;
            state += data3;
            state <<= 8;
            state += data4;
            return state;
        }

        private void getStateBits(AvaloniaList<stateBit> list, int data, int bit)
        {
            for(int i = 0; i < 32; i++)
            {
                if(i < list.Count-1)
                {
                    if ((data & (1 << i)) > 0)
                    {
                        if (bit == 1) list[i].bit1 = 1;
                        else if (bit == 2) list[i].bit2 = 1;
                    }
                    else
                    {
                        if (bit == 1) list[i].bit1 = 0;
                        else if (bit == 2) list[i].bit2 = 0;
                    }
                }
            }
        }
        
        public void changeMode_standby()
        {
            byte[] sendBytes = { 0x1, 0, 0, 0, 0, 0x1 };

            _serialPort.Write(sendBytes, 0, 6);
            transmitMsg.Insert(0, new(sendBytes));
            if (transmitMsg.Count > 3600) transmitMsg.RemoveAt(transmitMsg.Count - 1);
            _serialPort.DiscardOutBuffer();
        }
        public void changeMode_preparation()
        {
            byte[] sendBytes = { 0x1, 0, 0, 0, 1, 0x2 };

            _serialPort.Write(sendBytes, 0, 6);
            transmitMsg.Insert(0, new(sendBytes));
            if (transmitMsg.Count > 3600) transmitMsg.RemoveAt(transmitMsg.Count - 1);
            _serialPort.DiscardOutBuffer();
        }

        public void changeMode_operation()
        {
            byte[] sendBytes = { 0x1, 0, 0, 0, 2, 0x3 };

            _serialPort.Write(sendBytes, 0, 6);
            transmitMsg.Insert(0, new(sendBytes));
            if (transmitMsg.Count > 3600) transmitMsg.RemoveAt(transmitMsg.Count - 1);
            _serialPort.DiscardOutBuffer();
        }

        private void parseMsg(byte[] arr)
        {
            getStateBits(puld.main_status, byteToInt(arr[1], arr[2], arr[3], arr[4]), 1);
            getStateBits(puld.ppu_status, byteToInt(arr[5], arr[6], arr[7], arr[8]), 1);
            getStateBits(puld.ppu_status, byteToInt(arr[9], arr[10], arr[11], arr[12]), 2);

            puld.main_status.Add(new("")); puld.main_status.RemoveAt(puld.main_status.Count - 1);
            puld.ppu_status.Add(new("")); puld.ppu_status.RemoveAt(puld.ppu_status.Count - 1);
        }

        
        #pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
        public async Task get_status()
        {
            Task.Run(() =>
            {
                // отправить запрос статуса
                while(true)
                {
                    byte[] sendBytes = { 0x2, 0, 0, 0, 0, 0x2 };

                    _serialPort.Write(sendBytes, 0, 6);
                    transmitMsg.Insert(0, new(sendBytes));
                    if (transmitMsg.Count > 3600) transmitMsg.RemoveAt(transmitMsg.Count - 1);
                    _serialPort.DiscardOutBuffer();

                    Task.Delay(1000).Wait();
                }
            });
        }

        public async Task wait_status_tick()
        {
            Task.Run(() =>
            {
                byte[] arr = new byte[16];

                while (true)
                {
                    if (_serialPort.IsOpen)
                    {
                        try
                        {
                            if (_serialPort.BytesToRead >= 16)
                            {
                                _serialPort.Read(arr, 0, 16);
                                recieveMsg.Insert(0, new(arr));
                                if (recieveMsg.Count > 3600) recieveMsg.RemoveAt(recieveMsg.Count - 1);
                                _serialPort.DiscardInBuffer();
                                parseMsg(arr);
                            }
                        }
                        catch (TimeoutException) { }
                    }
                }
            });
            #pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
        }
    }
}