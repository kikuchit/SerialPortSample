using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace SerialPortSample
{
    class Program
    {
        static void Main(string[] args)
        {
            SerialPortControl spc = new SerialPortControl();
            spc.Run();
        }
    }
}
