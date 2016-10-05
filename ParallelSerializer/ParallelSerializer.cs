using DemoModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class ParallelSerializer
    {
        public void Serialize(object obj, Stream output)
        {
        }

        public Product Deserialize(Stream input)
        {
            using (var br = new BinaryReader(input))
            {
                int id = br.ReadInt32();
                if (id != 15)
                {
                    throw new InvalidOperationException();
                }

                return null;
            }
        }

        private void Serialize(Product product, Stream output)
        {
            using (var bw = new BinaryWriter(output))
            {
                if (product == null)
                {
                    bw.Write(-1);
                    return;
                }
                bw.Write(15);
                bw.Write((product.Category == null) ? -1 : 16);
                if (product.Category != null)
                {
                    
                }
            }
        }
    }
}
