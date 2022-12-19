using Tomlyn;

namespace FlashPatch
{
    class Program
    {
        public static void Main(string[] args)
        {

            string configText = File.ReadAllText(@"D:\Code\cshaftprojects\FlashPatch\FlashPatch\patch.toml");
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
                File.WriteAllText(Path.Join(outDir, Path.GetFileName(file)), content);
                Console.WriteLine($"Apply patches to file {file}");
            }
        }
    }
}