using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Hide();

            // Tạo cửa sổ đăng nhập
            Login loginWindow = new Login();

            // Hiển thị cửa sổ đăng nhập
            if (loginWindow.ShowDialog() == true)
            {
                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}
