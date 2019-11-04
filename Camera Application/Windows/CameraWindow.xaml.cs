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
        // Camera object
        Camera sc = null;

        // Camera features
        FeatureCollection features = null;
        Feature feature = null;

        public CameraWindow(CameraModel camera)
        {
            InitializeComponent();

            // Setup UI
            setupUI();
            
            // Open specified camera and start capture
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

                    // Read camera features
                    readFeatures();
                }
            }
            finally
            {
                sys.Shutdown();
            }
        }

        private void readFeatures()
        {
            try
            {
                features = sc.Features;
                foreach (Feature f in sc.Features)
                {
                    Console.WriteLine("Detected Feature: " + f.Name);
                }
            }
            catch (VimbaException ve)
            {
                Console.WriteLine("Feature error: " + ve.Message);
            }
        }

        private void startCapture()
        {
            try
            {
                long payloadSize;
                AVT.VmbAPINET.Frame[] frameArray = new AVT.VmbAPINET.Frame[3];

                sc.OnFrameReceived += new Camera.OnFrameReceivedHandler(onFrameReceived);
                feature = features["PayloadSize"];
                payloadSize = feature.IntValue;

                for (int index = 0; index < frameArray.Length; ++index)
                {
                    frameArray[index] = new AVT.VmbAPINET.Frame(payloadSize);
                    sc.AnnounceFrame(frameArray[index]);
                }

                sc.StartCapture();

                for (int index = 0; index < frameArray.Length; ++index)
                {
                    sc.QueueFrame(frameArray[index]);
                }

                feature = features["AcquisitionMode"];
                feature.EnumValue = "Continuous";

                feature = features["AcquisitionStart"];
                feature.RunCommand();
            }
            catch (VimbaException ve)
            {
                MessageBox.Show("An error occured: " + ve.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void stopCapture()
        {
            try
            {
                feature = features["AcquisitionStop"];
                feature.RunCommand();

                sc.EndCapture();
                sc.FlushQueue();
                sc.RevokeAllFrames();
                sc.Close();
            }
            catch (VimbaException ve)
            {
                MessageBox.Show("An error occured: " + ve.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void onFrameReceived(AVT.VmbAPINET.Frame frame)
        {
            //if (InvokeRequired) // if not from this thread invoke it in our context
            //{
                // In case of a separate thread (e.g. GUI ) use BeginInvoke to avoid a deadlock
                Dispatcher.Invoke(new Camera.OnFrameReceivedHandler(onFrameReceived), frame);
            //}
            if (VmbFrameStatusType.VmbFrameStatusComplete == frame.ReceiveStatus)
            {
                Console.WriteLine("Frame status complete");
            }
            sc.QueueFrame(frame);
        }

        private void startCaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            // Start capturing
            startCapture();
        }

        private void stopCaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            // Stop capturing
            stopCapture();
        }
    }
}
