using System;

namespace Spectr.Model
{
    internal class Device
    {
        public int DeviceID { get; private set; }
        public string SerialNumber { get; private set; }
        public string Type { get; private set; }
        public string Company { get; private set; }
        public string Model { get; private set; }
        public int ManufactureYear { get; private set; }
    }
}
