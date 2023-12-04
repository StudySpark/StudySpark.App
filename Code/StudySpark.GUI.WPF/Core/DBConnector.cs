using StudySpark.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.GUI.WPF.Core {
    public class DBConnector {
        public static DBRepository Database { get; } = new DBRepository();
    }
}
