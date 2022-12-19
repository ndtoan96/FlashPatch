using FlashPatch;
using Tomlyn;

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
    tasks.Add(Task.Run(() =>
    {

        string content = File.ReadAllText(file);
        bool fileChanged = false;
        foreach (Patch patch in config.Patches)
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
            File.WriteAllText(Path.Join(config.OutDir, Path.GetFileName(file)), content);
            Console.WriteLine($"Apply patches to file {file}");
        }
    }));
}

Task.WaitAll(tasks.ToArray());