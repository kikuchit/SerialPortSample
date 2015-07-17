using System;
using System.Collections.Generic;
using System.Text;

namespace SerialPortSample
{
    class OperationFactory {
        private OperationFactory() { }

        public static Operation MakeOperation(byte[] data) {
            if (data[0] != 0x02) {
                throw new ArgumentException();
            }

            Operation op = null;
            switch (data[1]) {
                case 0x42:
                    op = new Clear();
                    break;
                case 0x44:
                    op = new Display();
                    break;
                default:
                    throw new ArgumentException();
            }

            op.Make(data);

            return op;
        }
    }

    abstract class Operation
    {
        abstract public void Make(byte[] data);
    }

    class Clear : Operation {
        public string OperationCode { private set; get; }

        public Clear() {
            this.OperationCode = null;
        }

        public override void Make(byte[] data) {
            this.OperationCode = Encoding.ASCII.GetString(data, 1, 1);
        }

        public override string ToString() {
            return "OperationCode=" + this.OperationCode + "\r\n";
        }
    }

    class Display : Operation {
        public string Trial { private set; get; }
        public string Number { private set; get; }
        private string RecordIntegerPart { set; get; }
        private string RecordDecimalPart { set; get; }
        public string Result { private set; get; }
        public string Record {
            get {
                return this.RecordIntegerPart + "." + this.RecordDecimalPart;
            }
        }

        public Display() {
            this.Trial = null;
            this.Number = null;
            this.RecordIntegerPart = null;
            this.RecordDecimalPart = null;
            this.Result = null;
        }

        public override void Make(byte[] data)
        {
            this.Trial = Encoding.ASCII.GetString(data, 2, 1);
            this.Number = Encoding.ASCII.GetString(data, 3, 4);
            this.RecordIntegerPart = Encoding.ASCII.GetString(data, 7, 2);
            this.RecordDecimalPart = Encoding.ASCII.GetString(data, 9, 2);
            this.Result = Encoding.ASCII.GetString(data, 11, 1);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Trial=" + this.Trial + "\r\n");
            sb.Append("Number=" + this.Number + "\r\n");
            sb.Append("Record=" + this.Record + "\r\n");
            sb.Append("result=" + this.Result + "\r\n");
            return sb.ToString();
        }
    }
}
