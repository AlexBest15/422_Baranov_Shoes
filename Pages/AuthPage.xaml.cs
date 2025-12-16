using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _422_Baranov_Shoes.Pages
{
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void txtbxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblLoginHint.Visibility = string.IsNullOrEmpty(txtbxLogin.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void passBxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPasswordHint.Visibility = string.IsNullOrEmpty(passBxPassword.Password)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtbxLogin.Text.Trim();
            string password = passBxPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new ShoeStoreEntities())
                {
                    var user = context.Users
                        .FirstOrDefault(u => u.Login == login && u.Password == password);

                    if (user != null)
                    {
                        string roleName = "Клиент";
                        var role = context.Roles.FirstOrDefault(r => r.RoleID == user.RoleID);
                        if (role != null)
                        {
                            roleName = role.RoleName;
                        }

                        MessageBox.Show($"Добро пожаловать, {user.FullName}!\nРоль: {roleName}",
                            "Успешная авторизация", MessageBoxButton.OK, MessageBoxImage.Information);

                        var mainWindow = Application.Current.MainWindow as MainWindow;
                        mainWindow?.NavigateToUserPage(roleName);
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль", "Ошибка авторизации",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении к базе данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGuest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы вошли как гость. Доступен только просмотр товаров.",
                "Гостевой вход", MessageBoxButton.OK, MessageBoxImage.Information);

            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.NavigateToUserPage("Гость");
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.NavigateToPage(new RegPage());
        }

        private void btnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.NavigateToPage(new ChangePassPage());
        }
    }
}