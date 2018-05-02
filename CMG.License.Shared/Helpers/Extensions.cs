using System.IO;
using System.Reflection;

namespace CMG.License.Shared.Helpers
{
    public static class Extensions
    {
        public static bool ExtractResourceFile(this string resourceFileName, string filePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var t = assembly.GetManifestResourceNames();
            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{resourceFileName}"))
            {
                using (var fileStream = File.Create(filePath+$@"\{resourceFileName}"))
                {
                    if (stream != null)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.CopyTo(fileStream);
                        return true;
                    }
                    return false;
                };
            }
        }
    }
}