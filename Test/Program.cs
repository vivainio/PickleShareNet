using PickleShareNet;
using System;
using System.Linq;
using NUnit.Framework;

namespace PickleShareNet.Test
{
    class TestType
    {
        public string foo;
        public string[] bar;

    }
    [TestFixture]
    class Tests
    {
        [Test]
        [Ignore("only for manual stress testing")]
        public static void Stress()
        {
            var db = new PickleShareDb("c:/t/.testdb");
            var testArr = Enumerable.Repeat(0xdeadbeef, 10000).ToArray();
            int counter = 0;
            while (true)
            {
                var testid = $"longobject{counter % 4}";
                db.Set(testid, testArr);
                var (o, ok) = db.TryGet<uint[]>(testid);
                counter++;
                if (counter % 1000 == 0)
                {
                    Console.Write($"{counter},");
                }
            }
        }

        [Test]
        public static void SimpleTest()
        {
            var db = new PickleShareDb("c:/t/.testdb");

           
            var dummytype = new
            {
                foo = 11,
                bar = 12

            };
            db.Set("test1", dummytype);
            db.SetByType("12", dummytype);
            var tt = new TestType
            {
                foo = "hello",
                bar = new[] { "world", "again" }
            };
            db.SetByType("13", tt);
            var (gotval, ok) = db.GetByType<TestType>("13");
            Assert.IsTrue(gotval.foo == "hello");

            db.Set("long/path", 11);
            var (r, ok2) = db.TryGet<int>("long/path");
            Assert.IsTrue(ok2);
            Assert.IsTrue(r == 11);
            var keys = db.Keys("long");
            Assert.IsTrue(keys.Length == 1 && keys[0] == "long/path");
        }
    }
}
