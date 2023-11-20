using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.Core.FileManager {
    public class File {
        private string? path;
        private string? targetname;
        private string? type;
        private string? image;

        public string? Path { get { return path; } set { path = value; } }
        public string? TargetName { get { return targetname; } set { targetname = value; } }
        public string? Type { get { return type; } set { type = value; } }
        public string? Image { get { return image; } set { image = value; } }


    }
}
