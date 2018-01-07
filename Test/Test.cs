using PickleShareNet;
using System;
using System.Linq;
using NUnit.Framework;

namespace PickleShareNet.Test
{
    class TestType
    {
        public string Foo;
        public string[] Bar;

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

            // Root entry point is PickleShareDb instance
            var db = new PickleShareDb("c:/t/.testdb");

            // All data stored needs to have a type, obviously
            var tt = new TestType
            {
                Foo = "hello",
                Bar = new[] { "world", "again" }
            };

            db.Set("straightset", tt);

            // basic reading is done by TryGet, which returns tuple (result, ok)

            Assert.IsTrue(db.TryGet<TestType>("straightset").value.Foo == "hello");
            Assert.IsTrue(db.TryGet<TestType>("doesnotexist").ok == false);

            // SetByType puts the data in directory name indicated by type. E.g. this
            // one will go to "TestType/13". Use this to "database up" your directory layout

            db.SetByType("13", tt);

            // GetByType reads it from, you guessed it, "TestType/13"
            var (gotval, ok) = db.GetByType<TestType>("13");
            Assert.IsTrue(gotval.Foo == "hello");

            // you can have a full path in the object id. It will appear in respective place under file system
            db.Set("long/path", 11);
            var (r, ok2) = db.TryGet<int>("long/path");
            Assert.IsTrue(ok2);
            Assert.IsTrue(r == 11);

            // you can use "Keys" to get all legal keys under directory. 
            var keys = db.Keys("long");
            Assert.IsTrue(keys.Length == 1 && keys[0] == "long/path");
        }
    }
}
