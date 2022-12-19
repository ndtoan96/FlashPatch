using System.Text.RegularExpressions;

namespace FlashPatch
{
    internal class Patch
    {
        public List<string> Includes { get; set; } = new List<string> { "**/*" };
        public List<string> Excludes { get; set; } = new List<string>();
        public Predecessor Predecessor { get; set; } = Predecessor.None;
        public List<string> PredecessorParmams { get; set; } = new List<string>();
        public bool IsRegex { get; set; } = false;
        public bool ReplaceFirst { get; set; } = false;
        public string Find { get; set; } = "";
        public string Replace { get; set; } = "";
        public string? InsertTop { get; set; } = null;
        public string? InsertBottom { get; set; } = null;

        public bool IsMatch(string path)
        {
            return GlobUtils.IsMatch(path, Includes, Excludes);
        }

        public (bool applied, string? newContent) ApplyPatch(string content)
        {
            bool applied = false;
            string? newContent = null;

            Regex pattern;
            if (IsRegex)
            {
                pattern = new Regex(Find);
            }
            else
            {
                pattern = new Regex(Regex.Escape(Find));
            }

            if (pattern.IsMatch(content))
            {
                if (ReplaceFirst)
                {
                    content = pattern.Replace(content, Replace, 1);
                }
                else
                {
                    content = pattern.Replace(content, Replace);
                }
                applied = true;
                newContent = content;
            }


            if (applied)
            {
                if (InsertTop != null)
                {
                    newContent = InsertTop + Environment.NewLine + newContent;
                }
                if (InsertBottom != null)
                {
                    newContent += InsertBottom + Environment.NewLine;
                }
            }
            return (applied, newContent);
        }
    }

    internal enum Predecessor
    {
        None,
        InnerRemover,
    }
}
