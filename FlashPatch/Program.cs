using System.Text;
using Tomlyn;

namespace FlashPatch
{
    class Program
    {
        public static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Please provide the path of config file!");
                return;
            }

            string configText = File.ReadAllText(args[0], Encoding.UTF8);
            var config = Toml.ToModel<PatchConfig>(configText);

            DirectoryInfo outDir = Directory.CreateDirectory(config.OutDir);
            foreach (var child in outDir.GetFiles())
            {
                child.Delete();
            }

            List<Task> tasks = new();

            foreach (string file in GlobUtils.GetFiles(config.Root, config.GlobalIncludes, config.GlobalExcludes))
            {
                tasks.Add(Task.Run(() => ApplyPatches(file, config.Patches, config.OutDir)));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static void ApplyPatches(string file, IEnumerable<Patch> patches, string outDir)
        {

            string content = File.ReadAllText(file);
            bool fileChanged = false;

            string destPath = Path.Join(outDir, Path.GetFileName(file));

            // If destination file already exists, stop further processing
            if (Directory.Exists(destPath)) {
                return;
            }

            foreach (Patch patch in patches)
            {
                if (patch.IsMatch(file))
                {
                    (bool applied, string? newContent) = patch.ApplyPatch(content);
                    if (applied)
                    {
                        content = newContent!;
                        fileChanged = true;
                    }
                }
            }
            if (fileChanged)
            {
                File.WriteAllText(destPath, content);
                Console.WriteLine($"Apply patches to file {file}");
            }
        }
    }
}