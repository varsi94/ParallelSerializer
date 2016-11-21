using DemoModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParallelSerializer.Generator;
using ParallelSerializer.Tests.ModelExt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Tests
{
    [TestClass]
    public class ParallelSerializerTests
    {
        private ParallelSerializer serializer;

        [TestInitialize]
        public void SetUp()
        {
            serializer = new ParallelSerializer(new TplScheduler());
        }

        [TestMethod]
        public void TestSimpleObject()
        {
            using (var ms = new MemoryStream())
            {
                var input = new Category {Name = "asd", ID = 5};
                serializer.Serialize(input, ms);
                Utility.ConsoleWriter(ms);
                var result = (Category) serializer.Deserialize(ms);
                Assert.AreNotSame(input, result);
                Assert.AreEqual(input.Name, result.Name);
                Assert.AreEqual(input.ID, result.ID);
                Assert.IsNull(result.Products);
            }

            using (var ms = new MemoryStream())
            {
                string x = "asd";
                serializer.Serialize(x, ms);
                Utility.ConsoleWriter(ms);
                var result = (string) serializer.Deserialize(ms);
                Assert.AreEqual(x, result);
            }
        }

        [TestMethod]
        public void TestCompositeObject()
        {
            using (var ms = new MemoryStream())
            {
                var input = new Category
                {
                    Name = "asd",
                    ID = 5,
                    Products = Enumerable.Range(1, 100).Select(x => new Product {Name = "asd" + x, ID = 2, Count = x}).ToList()
                };
                serializer.Serialize(input, ms);
                ms.Position = 0;
                Utility.ConsoleWriter(ms);
                Category result = (Category) serializer.Deserialize(ms);
                Assert.AreNotSame(input, result);
                Assert.AreEqual(input.Name, result.Name);
                Assert.AreEqual(input.ID, result.ID);
                Assert.AreEqual(input.Products.Count, result.Products.Count);
                for (int i = 0; i < input.Products.Count; i++)
                {
                    Assert.AreEqual(input.Products[i].Name, result.Products[i].Name);
                    Assert.AreEqual(input.Products[i].ID, result.Products[i].ID);
                    Assert.AreEqual(input.Products[i].Count, result.Products[i].Count);
                }
            }
        }

        [TestMethod]
        public void TestList()
        {
            using (var ms = new MemoryStream())
            {
                var list = Enumerable.Range(1, 5).Select(x => new Product {Name = "asd", ID = x, Count = 1}).ToList();
                serializer.Serialize(list, ms);
                Utility.ConsoleWriter(ms);
                var result = (List<Product>)serializer.Deserialize(ms);
                Assert.AreNotSame(list, result);
                Assert.AreEqual(list.Count, result.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    Assert.AreEqual(list[i].Name, result[i].Name);
                    Assert.AreEqual(list[i].ID, result[i].ID);
                    Assert.AreEqual(list[i].Count, result[i].Count);
                }
            }
        }

        [TestMethod]
        public void TestDictionary()
        {
            using (var ms = new MemoryStream())
            {
                var dict = Enumerable.Range(1, 5)
                    .ToDictionary(x => x, x => new Product {Name = "asd", ID = x, Count = 1});
                serializer.Serialize(dict, ms);
                Utility.ConsoleWriter(ms);
                var result = (Dictionary<int, Product>)serializer.Deserialize(ms);
                Assert.AreNotSame(dict, result);
                Assert.AreEqual(dict.Count, result.Count);
                foreach (int i in dict.Keys)
                {
                    Assert.AreEqual(dict[i].Name, result[i].Name);
                    Assert.AreEqual(dict[i].ID, result[i].ID);
                    Assert.AreEqual(dict[i].Count, result[i].Count);
                }
            }
        }

        [TestMethod]
        public void TestPolymorphism()
        {
            using (var ms = new MemoryStream())
            {
                var category = new Category
                {
                    Name = "asd",
                    Products = new List<Product> {new ProductExt {Name = "asd2", AddedProp = "dummy"}}
                };
                serializer.Serialize(category, ms);
                Utility.ConsoleWriter(ms);
                Category result = (Category) serializer.Deserialize(ms);
                Assert.AreNotSame(category.Products.First(), result.Products.First());
                Assert.AreEqual(true, category.Products.First() is ProductExt);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            TaskGenerator.GenerateAssembly();
        }
    }
}
