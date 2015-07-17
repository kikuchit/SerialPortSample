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

        byte[] buf = new byte[1024];
        int readByte = 0;
        bool isReading = false;

        public void Run()
        {
            SerialPort port = new SerialPort("COM5", 2400, Parity.Even, 8, StopBits.Two);
            port.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            try
            {
                port.Open();
                port.DtrEnable = true;
                port.RtsEnable = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
            Console.ReadLine();
            port.Close();
            port.Dispose();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            while (true)
            {
                if (port.BytesToRead == 0)
                {
                    break;
                }

                byte b = (byte)port.ReadByte();
                if (isReading)
                {
                    if (b == SerialCode.Etx)
                    {
                        ExecuteCommand(buf, readByte);
                        readByte = 0;
                        isReading = false;
                        continue;
                    }

                    buf[readByte] = b;
                    readByte++;
                    continue;
                }

                if (b == SerialCode.Stx)
                {
                    isReading = true;
                    continue;
                }
            }
        }

        private void ExecuteCommand(byte[] data, int endPos)
        {
            string message = Encoding.ASCII.GetString(data, 0, endPos);
            Console.WriteLine(message);
        }
    }
}
