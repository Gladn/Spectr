using System;

namespace Spectr.Model
{
    internal class Device
    {
        public int DeviceID { get; set; }
        public string SerialNumber { get; set; }
        public string Type { get; set; }
        public string Company { get; set; }
        public string Model { get; set; }
        public int ManufactureYear { get; set; }

    }
}
