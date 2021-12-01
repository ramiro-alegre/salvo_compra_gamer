using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Models
{
    public class ShipHitDTO
    {
        public String Type { get; set; }
        public List<String> Hits { get; set; }
    }
}
