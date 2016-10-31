using DynamicSerializer.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Tests
{
    public class Utility
    {
        public static void ConsoleWriter(MemoryStream ms)
        {
            ms.Position = 0;

            var br = new SmartBinaryReader(ms);
            var br2 = new BinaryReader(ms);

            StringBuilder sbBinary = new StringBuilder();
            StringBuilder sbHex = new StringBuilder();

            //Debug.WriteLine("Ref lista mérete: " + br.ReadInt32());

            var numoftypes = br.ReadInt32();
            Debug.WriteLine("Típusok száma: " + numoftypes);

            for (int i = 0; i < numoftypes; i++)
            {
                Debug.WriteLine("Tipus[" + (i + 15) + "]: " + br.ReadString());
            }
            while (ms.Position != ms.Length)
            {
                var myByte = br2.ReadByte();
                sbBinary.Append(Convert.ToString(myByte, 2).PadLeft(8, '0') + " ");
                sbHex.Append(myByte.ToString("X").PadLeft(2, '0') + " ");
            }
            Debug.WriteLine(sbBinary.ToString());
            Debug.WriteLine(sbHex.ToString());
            Debug.WriteLine("");
            ms.Position = 0;
        }
    }
}
