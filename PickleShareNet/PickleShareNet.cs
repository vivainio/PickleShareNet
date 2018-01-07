using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading;

namespace PickleShareNet
{

    static class FileHelpers
    {
        public static string SanitizeFileName(string filename) =>
            string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));

        public static string EnsureDir(string path)
        {
            var full = Path.GetFullPath(path);
            if (!Directory.Exists(full))
            {
                Directory.CreateDirectory(full);
            }
            return full;
        }

        public static string SafeRead(string path)
        {
            int counter = 0;
            while (counter < 100)
            {
                try
                {
                    var txt = File.ReadAllText(path);
                    return txt;
                }
                catch (IOException e)
                {
                    Thread.Sleep(1);
                    counter++;
                }
            }
            return null;
        }
        public static bool SafeWrite(string path, string cont)
        {
            while (true)
            {

                try
                {
                    File.WriteAllText(path, cont);
                    return true;

                }
                catch (DirectoryNotFoundException)
                {
                    EnsureDir(Path.GetDirectoryName(path));
                }

                catch (IOException e)
                {
                    if (e.HResult != -2147024864)
                    {
                        throw;
                    }
                    return false;

                }
            }
        }


    }
    public class PickleShareDb
    {
        string _root;
        public PickleShareDb(string root)
        {
            _root = FileHelpers.EnsureDir(root);

        }

        private string PathTo(string tail)
        {
            return Path.Combine(_root, tail);
        }
        private string PathToId(string tail, string id)
        {
            var dirName = FileHelpers.EnsureDir(Path.Combine(_root, tail));

            return Path.Combine(dirName, id);
        }
        public (T value, bool ok) TryGet<T>(string path)
        {
            var fname = Path.Combine(_root, path);
            if (!File.Exists(fname))
            {
                return (default(T), false);
            }
            var cont = FileHelpers.SafeRead(fname);
            var val = JsonConvert.DeserializeObject<T>(cont);
            return (val, true);
        }

        public bool Set(string path, object val)
        {
            var fname = PathTo(path);
            var serialized = JsonConvert.SerializeObject(val);
            return FileHelpers.SafeWrite(fname, serialized);
        }

        public bool SetByType<T>(string id, T val)
        {
            var pathFromType = FileHelpers.SanitizeFileName(typeof(T).Name);
            var tgt = PathToId(pathFromType, id);
            return Set(tgt, val);
        }
        public (T value, bool ok) GetByType<T>(string id)
        {
            var pathFromType = FileHelpers.SanitizeFileName(typeof(T).Name);
            var tgt = PathToId(pathFromType, id);
            return TryGet<T>(tgt);
        }

        public string[] Keys(string path) => Directory.GetFiles(PathTo(path)).Select(p => path + "/" + Path.GetFileName(p)).ToArray();

    }
}
