using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace _422_Baranov_Shoes.Pages
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }

        private void lblLoginHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtbxLogin.Focus();
        }

        private void lblPasswordHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            passBxPassword.Focus();
        }

        private void lblConfirmPasswordHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            passBxConfirmPassword.Focus();
        }

        private void lblFullNameHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtbxFullName.Focus();
        }

        private void txtbxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblLoginHint.Visibility = Visibility.Visible;
            if (txtbxLogin.Text.Length > 0)
            {
                lblLoginHint.Visibility = Visibility.Hidden;
            }
        }

        private void txtbxFullName_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblFullNameHint.Visibility = Visibility.Visible;
            if (txtbxFullName.Text.Length > 0)
            {
                lblFullNameHint.Visibility = Visibility.Hidden;
            }
        }

        private void passBxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPasswordHint.Visibility = Visibility.Visible;
            if (passBxPassword.Password.Length > 0)
            {
                lblPasswordHint.Visibility = Visibility.Hidden;
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

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            // Проверка заполнения полей
            if (string.IsNullOrEmpty(txtbxLogin.Text) ||
                string.IsNullOrEmpty(txtbxFullName.Text) ||
                string.IsNullOrEmpty(passBxPassword.Password) ||
                string.IsNullOrEmpty(passBxConfirmPassword.Password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
                return;
            }

            // Проверка существования пользователя
            using (var context = new ShoeStoreEntities())
            {
                var existingUser = context.Users
                    .FirstOrDefault(u => u.Login == txtbxLogin.Text);

                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    isValid = false;
                    return;
                }
            }

            // Проверка пароля
            if (passBxPassword.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            // Проверка совпадения паролей
            if (passBxPassword.Password != passBxConfirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            // Проверка пароля на сложность
            if (!Regex.IsMatch(passBxPassword.Password, @"^(?=.*[a-zA-Z])(?=.*\d).+$"))
            {
                MessageBox.Show("Пароль должен содержать буквы и цифры!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (isValid)
            {
                try
                {
                    using (var context = new ShoeStoreEntities())
                    {
                        var clientRole = context.Roles.FirstOrDefault(r => r.RoleName == "Клиент");

                        int roleId = 3; 
                        if (clientRole != null)
                        {
                            roleId = clientRole.RoleID;
                        }

                        var newUser = new Users
                        {
                            Login = txtbxLogin.Text,
                            Password = passBxPassword.Password,
                            FullName = txtbxFullName.Text,
                            RoleID = roleId
                        };

                        context.Users.Add(newUser);
                        context.SaveChanges();

                        MessageBox.Show("Регистрация прошла успешно!\nТеперь вы можете войти в систему.",
                            "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                        txtbxLogin.Clear();
                        txtbxFullName.Clear();
                        passBxPassword.Clear();
                        passBxConfirmPassword.Clear();

                        btnBack_Click(sender, e);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при регистрации: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToPage(new AuthPage());
        }
    }
}