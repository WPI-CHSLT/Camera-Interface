using AVT.VmbAPINET;
using Camera_Application.Models;
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
using System.Windows.Shapes;

namespace Camera_Application.Windows
{
    /// <summary>
    /// Interaction logic for CameraWindow.xaml
    /// </summary>
    public partial class CameraWindow : Window
    {

        // Camera features
        FeatureCollection features = null;
        Feature feature = null;

        public CameraWindow(CameraModel camera)
        {
            InitializeComponent();

            // Setup UI
            setupUI();
            
            // Open specified camera
            openCamera(camera);
        }

        private void setupUI()
        {
            
        }

        private void openCamera(CameraModel camera)
        {
            Vimba sys = new Vimba();

            try
            {
                sys.Startup();

                CameraCollection cameras = sys.Cameras;
                Camera sc = null;
                foreach (Camera c in cameras)
                {
                    if (c.Id.Equals(camera.cameraID))
                    {
                        sc = c;
                        break;
                    }
                }
                if (sc != null)
                {
                    try
                    {
                        sys.OpenCameraByID(camera.cameraID, VmbAccessModeType.VmbAccessModeFull);
                    }
                    catch (VimbaException ve)
                    {
                        MessageBox.Show("Error opening camera: " + ve.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    Console.WriteLine("Camera opened: " + camera.cameraID);
                    readFeatures(sc);
                }
            }
            finally
            {
                sys.Shutdown();
            }
        }

        private void readFeatures(Camera camera)
        {
            try
            {
                foreach (Feature f in camera.Features)
                {
                    Console.WriteLine("Detected Feature: " + f.Name);
                }
            }
            catch (VimbaException ve)
            {
                Console.WriteLine("Feature error: " + ve.Message);
            }
        }

        private void captureImage()
        {

        }
    }
}
