using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlashPatch
{
    internal class PatchConfig
    {
        public byte NumThreads { get; set; } = 1;
        public string Root { get; set; } = ".";
        public string OutDir { get; set; } = "./out";
        public List<string> GlobalIncludes { get; set; } = new List<string>();
        public List<string> GlobalExcludes { get; set; } = new List<string>();
        public List<Patch> Patches { get; set; } = new List<Patch>();
    }
}
