using _422_Baranov_Shoes.Pages;
using System;
using System.Windows;
using System.Windows.Controls;

namespace _422_Baranov_Shoes
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigateToAuthPage();
        }

        public void NavigateToPage(Page page)
        {
            MainFrame.Navigate(page);
        }

        public void NavigateToUserPage(string role)
        {
            NavigateToPage(new ProductPage(role));
        }

        public void NavigateToAuthPage()
        {
            NavigateToPage(new AuthPage());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTimeNow.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }
    }
}