using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace SerialPortSample
{
    class SerialPortControl
    {
        class SerialCode
        {
            public const byte Stx = 0x02;
            public const byte Etx = 0x03;
        }

        public delegate void ExecuteCommand(byte[] data, int endPos);

        byte[] buf;
        int readByte;
        bool isReading;
        SerialPort port;
        ExecuteCommand executeCommand;

        public SerialPortControl(ExecuteCommand command)
        {
            this.buf = new byte[1024];
            this.readByte = 0;
            this.isReading = false;
            this.port = new SerialPort("COM5", 2400, Parity.Even, 8, StopBits.Two);
            this.port.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceived);
            this.executeCommand = new ExecuteCommand(command);
        }

        public void Run()
        {
            try
            {
                this.port.Open();
                this.port.DtrEnable = true;
                this.port.RtsEnable = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
            Console.ReadLine();
            this.port.Close();
            this.port.Dispose();
        }

        private void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (this.port.BytesToRead != 0)
            {
                byte b = (byte)this.port.ReadByte();
                if (this.isReading)
                {
                    if (b == SerialCode.Etx)
                    {
                        this.executeCommand(buf, this.readByte);
                        this.Initialize();
                        continue;
                    }

                    this.Append(b);
                    continue;
                }

                if (b == SerialCode.Stx)
                {
                    this.isReading = true;
                    continue;
                }
            }
        }

        private void Append(byte b)
        {
            if (this.readByte + 1 > this.buf.Length)
            {
                throw new IndexOutOfRangeException("読み込みバッファ超過");
            }
            this.buf[this.readByte] = b;
            this.readByte++;
        }

        private void Initialize()
        {
            Array.Clear(this.buf, 0, this.buf.Length);
            this.readByte = 0;
            this.isReading = false;
        }
    }
}
