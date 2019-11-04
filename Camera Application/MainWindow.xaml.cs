using AVT.VmbAPINET;
using Camera_Application.Models;
using Camera_Application.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Camera_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            // Setup UI
            setupUI();

            // Setup table
            setupTable();

            // Detect cameras
            try
            {
                detectCameras();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void setupUI()
        {
            this.button.IsEnabled = false;
        }

        private void setupTable()
        {
            var gridView = new GridView();
            this.listView.View = gridView;

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Camera ID",
                DisplayMemberBinding = new Binding("cameraID")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Camera Name",
                DisplayMemberBinding = new Binding("cameraName")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Camera Model",
                DisplayMemberBinding = new Binding("cameraModel")
            });

            // Populate list
            //this.listView.Items.Add(new MyItem { Id = 1, Name = "David" });
        }

        private void detectCameras()
        {
            Vimba sys = new Vimba();
            CameraCollection cameras;

            string strCameraID;
            string strCameraName;
            string strCameraModel;

            try
            {
                sys.Startup();                      // Initialize the Vimba API
                Console.WriteLine("Vimba .NET API Version {0:D}.{1:D}.{2:D}", sys.Version.major, sys.Version.minor, sys.Version.patch);
                cameras = sys.Cameras;              // Fetch all cameras known to Vimba

                Console.WriteLine("Cameras found: " + cameras.Count);
                Console.WriteLine();

                // Query all static details of all known cameras and print them out.
                // We don't have to open the cameras for that.
                foreach (Camera camera in cameras)
                {
                    try
                    {
                        strCameraID = camera.Id;
                    }
                    catch (VimbaException ve)
                    {
                        strCameraID = ve.Message;
                    }

                    try
                    {
                        strCameraName = camera.Name;
                    }
                    catch (VimbaException ve)
                    {
                        strCameraName = ve.Message;
                    }

                    try
                    {
                        strCameraModel = camera.Model;
                    }
                    catch (VimbaException ve)
                    {
                        strCameraModel = ve.Message;
                    }

                    this.listView.Items.Add(new CameraModel { cameraID = strCameraID, cameraName = strCameraName, cameraModel = strCameraModel });

                    Console.WriteLine("/// Camera Name: " + strCameraName);
                    Console.WriteLine("/// Model Name: " + strCameraModel);
                    Console.WriteLine("/// Camera ID: " + strCameraID);
                    Console.WriteLine();
                }
            }
            finally
            {
                sys.Shutdown();
            }
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.button.IsEnabled = true;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                CameraModel selectedCamera = (CameraModel) listView.SelectedItems[0];
                Console.WriteLine("selected item: " + selectedCamera.cameraID);
                CameraWindow cameraWindow = new CameraWindow(selectedCamera);
                cameraWindow.Show();
                this.Close();
            }
        }
    }
}
