using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace _422_Baranov_Shoes.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }

        private void lblLoginHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtbxLogin.Focus();
        }

        private void lblNewPasswordHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            passBxNewPassword.Focus();
        }

        private void lblConfirmPasswordHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            passBxConfirmPassword.Focus();
        }

        private void txtbxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblLoginHint.Visibility = Visibility.Visible;
            if (txtbxLogin.Text.Length > 0)
            {
                lblLoginHint.Visibility = Visibility.Hidden;
            }
        }

        private void passBxNewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblNewPasswordHint.Visibility = Visibility.Visible;
            if (passBxNewPassword.Password.Length > 0)
            {
                lblNewPasswordHint.Visibility = Visibility.Hidden;
            }
        }

        private void passBxConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblConfirmPasswordHint.Visibility = Visibility.Visible;
            if (passBxConfirmPassword.Password.Length > 0)
            {
                lblConfirmPasswordHint.Visibility = Visibility.Hidden;
            }
        }

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string login = txtbxLogin.Text.Trim();
            string newPassword = passBxNewPassword.Password;
            string confirmPassword = passBxConfirmPassword.Password;

            if (string.IsNullOrEmpty(login) ||
                string.IsNullOrEmpty(newPassword) ||
                string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Regex.IsMatch(newPassword, @"^(?=.*[a-zA-Z])(?=.*\d).+$"))
            {
                MessageBox.Show("Пароль должен содержать буквы и цифры!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new ShoeStoreEntities())
                {
                    var user = context.Users.FirstOrDefault(u => u.Login == login);

                    if (user != null)
                    {
                        user.Password = newPassword;
                        context.SaveChanges();

                        MessageBox.Show("Пароль успешно изменен!", "Успешно",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        txtbxLogin.Clear();
                        passBxNewPassword.Clear();
                        passBxConfirmPassword.Clear();

                        btnBack_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Пользователь с таким логином не найден!",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при смене пароля: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToPage(new AuthPage());
        }
    }
}