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
            SerialPortControl spc = new SerialPortControl(new SerialPortControl.ExecuteCommand(Command));
            spc.Run();
        }

        private static void Command(byte[] data, int endPos)
        {
            byte[] buf = new byte[1024];
            Array.Copy(data, 0, buf, 0, endPos);

            Operation op = OperationFactory.MakeOperation(buf);
            Console.WriteLine(op.ToString());
        }
    }
}
