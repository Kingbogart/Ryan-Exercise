using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coding_Exercise.Models
{
    //Data types selected based on data provided in WeatherData.json
    //Internal representation of our data model for WeatherData. Currently matches WeatherRequest, but could be altered at a later date
    //for various reasons such as extending for more data, internal data points, differing property names etc.
    public class WeatherData
    {
        public DateTime date { get; set; }
        public decimal temperature { get; set; }
        public decimal humidity { get; set; }
        public decimal pressure { get; set; }
        public decimal wind { get; set; }

        public decimal windDirection { get; set; }
        public decimal rain { get; set; }
    }
}
