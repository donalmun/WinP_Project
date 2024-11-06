using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.UI.ViewManagement;
using WinRT.Interop;

namespace FoodApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window m_window;

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new Window();
            Frame rootFrame = new Frame();
            m_window.Content = rootFrame;

            rootFrame.Navigate(typeof(FoodApp.Views.LoginPage), args.Arguments);
            m_window.Activate();

            // Set the window to maximized mode
            var hwnd = WindowNative.GetWindowHandle(m_window);
            ShowWindow(hwnd, SW_MAXIMIZE);
        }

        // Method to get the window handle
        public IntPtr GetWindowHandle()
        {
            return WindowNative.GetWindowHandle(m_window);
        }

        // P/Invoke declaration for ShowWindow
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_MAXIMIZE = 3;
    }
}
