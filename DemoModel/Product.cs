using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoModel
{
    public class Product
    {
        public string Name { get; set; }

        public int ID { get; set; }

        public int Count { get; set; }

        public Category Category { get; set; }
    }
}
