using System.Collections.Generic;

namespace DemoModel
{
    public class Category
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}