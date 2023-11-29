using StudySpark.Core.Repositories;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    internal class GitViewModel : ObservableObject
    {
        GitRepository repository = new GitRepository();
        private void SelectRepository()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                string selectedPath = Path.GetDirectoryName(openFileDialog.FileName);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    bool result = repository.InsertData(selectedPath, "repo");
                    if (!result)
                    {
                        System.Windows.MessageBox.Show("Er is iets fout gegaan!");
                        return;
                    }
                }
                System.Windows.MessageBox.Show("Map succesvol toegevoegd!");
            }
            else
            {
                System.Windows.MessageBox.Show("Er is iets fout gegaan!");
            }
        }
    }
}
