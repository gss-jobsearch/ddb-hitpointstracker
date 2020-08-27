using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HitPointsTracker.Tests
{
    public static class Resources
    {
        private static readonly Assembly TestAssembly =
            typeof(Resources).Assembly;

        public static Stream? GetResourceStream(
            string filename)
        {
            var path = TestAssembly.FullName!.Split(',')
               .Take(1)
               .Concat(filename.Split('/', '\\'));

            return TestAssembly.GetManifestResourceStream(
                string.Join('.', path));
        }

        public static string? GetResourceString(
            string filename,
            Encoding? encoding = null)
        {
            using Stream? stream = GetResourceStream(filename);
            if (stream == null) return null;
            using TextReader reader = new StreamReader(stream,
                encoding ?? Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
