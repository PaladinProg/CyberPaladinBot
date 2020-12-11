using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPaladinBot
{
    class WeatherResponse//отвечает за струкуру 1 вложенности json ответа
    {
        public TemperatureInfo Main { get; set; }

        public string Name { get; set; }
    }
}
