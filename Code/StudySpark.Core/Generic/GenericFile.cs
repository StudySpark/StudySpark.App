using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.Core.FileManager {
    public class GenericFile {
        private string? path;
        private string? targetname;
        private string? type;
        private string? image;
        private int? id;

        public int? Id { get { return id; } set { id = value; } }
        public string? Path { get { return path; } set { path = value; } }
        public string? TargetName { get { return targetname; } set { targetname = value; } }
        public string? Type { get { return type; } set { type = value; } }
        public string? Image { get { return image; } set { image = value; } }
        public GenericFile(int id, string path, string targetname, string type, string image) 
        {
            this.id = id;
            this.path = path;
            this.targetname = targetname;
            this.type = type;
            this.image = image;
        }


    }
}
