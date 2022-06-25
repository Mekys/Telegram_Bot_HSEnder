using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot_tg
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Para
    {
        public Para(string _day, string _time, string _description)
        {
            day = _day;
            time = _time;
            description = _description;
        }
        public string day { get; set; }
        public string time { get; set; }
        public string description { get; set; }
        public override string ToString()
        {
            return $"День недели:{day} Время:{time} пара: {description}";
        }
        public override bool Equals(object obj)
        {
            var other = obj as Para;
            return other.day == day && other.time == time && other.description == description;
        }
    }
}
