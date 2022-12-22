using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace FlashPatch
{
    internal class Patch
    {
        public string Name { get; set; } = "anonymous";
        public string Description { get; set; } = "";
        public List<string> Includes { get; set; } = new List<string> { "**/*" };
        public List<string> Excludes { get; set; } = new List<string>();
        public PreprocessorType Preprocessor { get; set; } = PreprocessorType.NONE;
        public List<string> PreprocessorParams { get; set; } = new List<string>();
        [DataMember(Name = "regex")]
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

            string tmpContent = content;

            switch (Preprocessor)
            {
                case PreprocessorType.NONE:
                    break;
                case PreprocessorType.INNER_REMOVER:
                    string prefix = PreprocessorParams[0];
                    char openChar = Char.Parse(PreprocessorParams[1]);
                    char closeChar = Char.Parse(PreprocessorParams[2]);
                    InnerRemover remover = new InnerRemover(prefix, openChar, closeChar);
                    tmpContent = remover.Process(tmpContent);
                    break;
            }

            Regex pattern;
            if (IsRegex)
            {
                pattern = new Regex(Find);
            }
            else
            {
                pattern = new Regex(Regex.Escape(Find));
            }

            if (pattern.IsMatch(tmpContent))
            {
                if (ReplaceFirst)
                {
                    tmpContent = pattern.Replace(tmpContent, Replace, 1);
                }
                else
                {
                    tmpContent = pattern.Replace(tmpContent, Replace);
                }
                applied = true;
                newContent = tmpContent;
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

    internal enum PreprocessorType
    {
        NONE,
        INNER_REMOVER,
    }
}
