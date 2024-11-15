using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPaladinBot
{
    class TemperatureInfo//отвечает конкретно за свойство Main
    {
        public float Temp { get; set; }//выделение нужного нам свойства для показа температуры
        public float Pressure { get; set; }//давление
        public float Humidity { get; set; }//влажность
    }
}
