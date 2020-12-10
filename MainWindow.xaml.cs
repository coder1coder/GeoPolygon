using GeoPolygon.Core.Repository;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace GeoPolygon
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var sdlg = new SaveFileDialog
            {
                DefaultExt = "txt",
                Filter = "Text files (*.txt)|*.txt*"
            };

            if (sdlg.ShowDialog() == true)
            {
                var polygons = new OpenStreetMapRepository().GetMultiPolygonByAddress(SearchBox.Text, 1);

                var output = "";

                foreach(var polygon in polygons)
                { 
                    foreach (var p in polygon.Points)
                        output += p.X + " " + p.Y + "\r\n";

                    output += "\r\n";
                }
                
                try
                {
                    File.WriteAllText(sdlg.FileName, output);
                    MessageBox.Show("Saved");
                }
                catch(IOException)
                {
                    MessageBox.Show("Can't save result to file");
                }

            }
            
        }
    }
}
