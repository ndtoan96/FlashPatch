using DotNet.Globbing;

namespace FlashPatch
{
    internal static class GlobUtils
    {
        public static IEnumerable<string> GetFiles(string root, List<string> includes, List<string> excludes)
        {
            var allFiles = new DirectoryInfo(root).GetFiles("*", SearchOption.AllDirectories);
            return from file in allFiles
                   where IsMatch(file.FullName, includes, excludes)
                   select file.FullName;
        }

        public static bool IsMatch(string path, List<string> includes, List<string> excludes)
        {
            foreach (string exclude in excludes)
            {
                if (Glob.Parse(exclude).IsMatch(path))
                {
                    return false;
                }
            }

            foreach (string include in includes)
            {
                if (Glob.Parse(include).IsMatch(path))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
