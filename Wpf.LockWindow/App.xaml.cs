using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Wpf.LockWindow
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        public static TaskbarIcon TaskbarIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            TaskbarIcon = (TaskbarIcon)FindResource("Taskbar");

        }

        private void TaskbarIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null)
            {
                MainWindow = new MainWindow();
            }
            MainWindow.Show();
            MainWindow.WindowState = WindowState.Maximized;
        }

        private void showItem_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null)
            {
                MainWindow = new MainWindow();
            }
            MainWindow.Show();
            MainWindow.WindowState = WindowState.Maximized;
        }

        private void closeItem_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null)
            {
                MainWindow = new MainWindow();
            }
            MainWindow.Hide();
        }

        private void quitItem_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
            //Application.Current.Shutdown();
        }
    }
}
