using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProjectModul.Models
{
    public class Composition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Calories { get; set; }
        public DateTime Created { get; set; }
    }
}
