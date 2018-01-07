# PickleShareNet

This is an implementation of the basic idea of PickleShare python package as netstandard2 library.

Basic idea is that:

- Database is under some "root directory"
- The objects are stored as files under the root directory, serialized and deserialized through JSON.Net
- Basic concurrent access is supported through exception handling and retries (with okay throughput even
with contested database)
- The API is not async (turned out to be much faster as a synchronous version)
- Caveat emptor. This probably still has bugs, but it's simple and short so you can fix them as you go

Typical use cases:

- Store configuration or incremental state in command line apps. It can be tedious to implement this manually
- Backing store for small web apps with light throughput requirements (e.g. ones you run on localhost)

Example use (see Test.cs for more):

```csharp
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


```

## FAQ

Q: Why did you write this in the first place?

A: I'm using pickleshare (the python version) in tons of hand-rolled command line apps, and wanted similar experience for .net

Q: Why not use SQLITE?

A: It sucks to maintain SQL schema for simple apps. Also, this is a 7kb DLL currently with only NewtonSoft.Json as dependency

Q: Why C# instead of F#?

A: It was trivial enough to remain "fun" in C#. I'm using it in F# apps myself.


## License

MIT

## Copyright

Copyright (c) 2018 by Ville M. Vainio
