using StudySpark.GUI.WPF.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    internal class FilesViewModel {

        private readonly FilesModel _model;

        public FilesViewModel() {
            _model = new FilesModel();
        }
    }
}
