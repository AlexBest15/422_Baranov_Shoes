using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace _422_Baranov_Shoes.Pages
{
    public partial class ProductPage : Page
    {
        private ObservableCollection<ProductViewModel> _products;
        private ObservableCollection<ProductViewModel> _filteredProducts;
        private string _userRole;

        public class ProductViewModel
        {
            public int ProductID { get; set; }
            public string ArticleNumber { get; set; }
            public string ProductName { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public decimal Discount { get; set; }
            public string ManufacturerName { get; set; }
            public string CategoryName { get; set; }
            public int StockQuantity { get; set; }
            public string UnitName { get; set; }
            public string SupplierName { get; set; }

            // Свойства для изображения
            public string ImagePath { get; set; }
            public bool HasImage { get; set; }

            public bool HasDiscount => Discount > 0;

            public decimal DiscountedPrice => Price * (1 - Discount / 100);

            public string PriceText
            {
                get
                {
                    if (HasDiscount)
                    {
                        return $"Старая цена: {Price:C}\nНовая цена: {DiscountedPrice:C}";
                    }
                    return $"Цена: {Price:C}";
                }
            }

            public string DiscountText => HasDiscount ? $"Скидка {Discount}%!" : "";

            public Visibility DiscountVisibility => HasDiscount ? Visibility.Visible : Visibility.Collapsed;

            public string ShortDescription
            {
                get
                {
                    if (string.IsNullOrEmpty(Description))
                        return "Нет описания";

                    if (Description.Length > 60)
                        return Description.Substring(0, 57) + "...";

                    return Description;
                }
            }
        }

        public ProductPage(string userRole = "Гость")
        {
            InitializeComponent();
            _userRole = userRole;
            txtUserRole.Text = $"Роль: {userRole}";
            LoadProducts();
            SetupControls();
        }

        private void SetupControls()
        {
            // Показываем панель управления только для Менеджера и Администратора
            bool showControls = _userRole == "Менеджер" || _userRole == "Администратор";
            pnlControls.Visibility = showControls ? Visibility.Visible : Visibility.Collapsed;

            if (showControls)
            {
                LoadFilters();
            }

            // Настраиваем видимость кнопок в зависимости от роли
            if (_userRole == "Администратор")
            {
                btnAddProduct.Visibility = Visibility.Visible;
               
            }
            else if (_userRole == "Менеджер")
            {
                btnAddProduct.Visibility = Visibility.Collapsed;
               
            }
            else
            {
                btnAddProduct.Visibility = Visibility.Collapsed;
               
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (var context = new ShoeStoreEntities())
                {
                    var dbProducts = context.Products.ToList();

                    _products = new ObservableCollection<ProductViewModel>();
                    _filteredProducts = new ObservableCollection<ProductViewModel>();

                    foreach (var product in dbProducts)
                    {
                        var productVM = new ProductViewModel
                        {
                            ProductID = product.ProductID,
                            ArticleNumber = product.ArticleNumber ?? "Без артикула",
                            ProductName = GetProductName(product),
                            Description = product.Description ?? "Описание отсутствует",
                            Price = product.Price,
                            Discount = product.CurrentDiscount ?? 0,
                            ManufacturerName = GetManufacturerName(product),
                            CategoryName = GetCategoryName(product),
                            SupplierName = GetSupplierName(product),
                            UnitName = GetUnitName(product),
                            StockQuantity = product.StockQuantity,
                            ImagePath = GetProductImagePath(product.ProductID, GetCategoryName(product)),
                            HasImage = CheckImageExists(product.ProductID, GetCategoryName(product))
                        };

                        _products.Add(productVM);
                        _filteredProducts.Add(productVM);
                    }

                    ItemsProducts.ItemsSource = _filteredProducts;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetProductImagePath(int productId, string categoryName = null)
        {
            string imagesPath = @"C:\Users\mosco\source\repos\422_Baranov_Shoes\Pages\Images\";

            if (!Directory.Exists(imagesPath))
            {
                imagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!Directory.Exists(imagesPath))
                {
                    return null;
                }
            }

            string[] imageFormats = { ".jpg", ".jpeg", ".png", ".bmp" };

            foreach (var format in imageFormats)
            {
                string fileName = $"{productId}{format}";
                string fullPath = Path.Combine(imagesPath, fileName);

                if (File.Exists(fullPath))
                {
                    return $"file:///{fullPath.Replace('\\', '/')}";
                }
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                string safeCategoryName = GetSafeFileName(categoryName);
                foreach (var format in imageFormats)
                {
                    string fileName = $"{safeCategoryName}{format}";
                    string fullPath = Path.Combine(imagesPath, fileName);

                    if (File.Exists(fullPath))
                    {
                        return $"file:///{fullPath.Replace('\\', '/')}";
                    }
                }
            }

            string noImagePath = Path.Combine(imagesPath, "no-image.png");
            if (File.Exists(noImagePath))
            {
                return $"file:///{noImagePath.Replace('\\', '/')}";
            }

            return null;
        }

        private string GetSafeFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName.ToLower();
        }

        private bool CheckImageExists(int productId, string categoryName = null)
        {
            string imagesPath = @"C:\Users\mosco\source\repos\422_Baranov_Shoes\Pages\Images\";

            if (!Directory.Exists(imagesPath))
            {
                imagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!Directory.Exists(imagesPath))
                {
                    return false;
                }
            }

            string[] imageFormats = { ".jpg", ".jpeg", ".png", ".bmp" };

            foreach (var format in imageFormats)
            {
                string fileName = $"{productId}{format}";
                string fullPath = Path.Combine(imagesPath, fileName);

                if (File.Exists(fullPath))
                {
                    return true;
                }
            }

            string imagePath = GetProductImagePath(productId, categoryName);
            if (!string.IsNullOrEmpty(imagePath) && imagePath.Contains("no-image.png"))
            {
                return false;
            }

            return !string.IsNullOrEmpty(imagePath);
        }

        private string GetProductName(Products product)
        {
            if (product.ProductNames != null)
            {
                return product.ProductNames.ProductName;
            }

            if (product.ProductNameID > 0)
            {
                using (var context = new ShoeStoreEntities())
                {
                    var name = context.ProductNames
                        .FirstOrDefault(p => p.ProductNameID == product.ProductNameID);
                    if (name != null)
                    {
                        return name.ProductName;
                    }
                }
            }

            return product.ArticleNumber ?? "Товар без названия";
        }

        private string GetManufacturerName(Products product)
        {
            if (product.Manufacturers != null)
            {
                return product.Manufacturers.ManufacturerName;
            }

            if (product.ManufacturerID > 0)
            {
                using (var context = new ShoeStoreEntities())
                {
                    var manufacturer = context.Manufacturers
                        .FirstOrDefault(m => m.ManufacturerID == product.ManufacturerID);
                    if (manufacturer != null)
                    {
                        return manufacturer.ManufacturerName;
                    }
                }
            }

            return "Неизвестно";
        }

        private string GetCategoryName(Products product)
        {
            if (product.Categories != null)
            {
                return product.Categories.CategoryName;
            }

            if (product.CategoryID > 0)
            {
                using (var context = new ShoeStoreEntities())
                {
                    var category = context.Categories
                        .FirstOrDefault(c => c.CategoryID == product.CategoryID);
                    if (category != null)
                    {
                        return category.CategoryName;
                    }
                }
            }

            return "Неизвестно";
        }

        private string GetSupplierName(Products product)
        {
            if (product.Suppliers != null)
            {
                return product.Suppliers.SupplierName;
            }

            if (product.SupplierID > 0)
            {
                using (var context = new ShoeStoreEntities())
                {
                    var supplier = context.Suppliers
                        .FirstOrDefault(s => s.SupplierID == product.SupplierID);
                    if (supplier != null)
                    {
                        return supplier.SupplierName;
                    }
                }
            }

            return "Неизвестно";
        }

        private string GetUnitName(Products product)
        {
            if (product.Units != null)
            {
                return product.Units.UnitName;
            }

            if (product.UnitID > 0)
            {
                using (var context = new ShoeStoreEntities())
                {
                    var unit = context.Units
                        .FirstOrDefault(u => u.UnitID == product.UnitID);
                    if (unit != null)
                    {
                        return unit.UnitName;
                    }
                }
            }

            return "шт.";
        }

        private void LoadFilters()
        {
            try
            {
                using (var context = new ShoeStoreEntities())
                {
                    cmbCategory.Items.Clear();
                    cmbCategory.Items.Add("Все категории");
                    foreach (var category in context.Categories.ToList())
                    {
                        cmbCategory.Items.Add(category);
                    }
                    cmbCategory.SelectedIndex = 0;

                    cmbManufacturer.Items.Clear();
                    cmbManufacturer.Items.Add("Все производители");
                    foreach (var manufacturer in context.Manufacturers.ToList())
                    {
                        cmbManufacturer.Items.Add(manufacturer);
                    }
                    cmbManufacturer.SelectedIndex = 0;

                    cmbSort.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке фильтров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterAndSortProducts()
        {
            try
            {
                var filtered = _products.ToList();

                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    string search = txtSearch.Text.ToLower();
                    filtered = filtered.Where(p =>
                        p.ProductName.ToLower().Contains(search) ||
                        p.ArticleNumber.ToLower().Contains(search) ||
                        p.Description.ToLower().Contains(search) ||
                        p.ManufacturerName.ToLower().Contains(search) ||
                        p.CategoryName.ToLower().Contains(search)
                    ).ToList();
                }

                if (cmbCategory.SelectedIndex > 0)
                {
                    if (cmbCategory.SelectedItem is Categories selectedCategory)
                    {
                        filtered = filtered.Where(p => p.CategoryName == selectedCategory.CategoryName).ToList();
                    }
                }

                if (cmbManufacturer.SelectedIndex > 0)
                {
                    if (cmbManufacturer.SelectedItem is Manufacturers selectedManufacturer)
                    {
                        filtered = filtered.Where(p => p.ManufacturerName == selectedManufacturer.ManufacturerName).ToList();
                    }
                }

                if (cmbSort.SelectedItem is ComboBoxItem sortItem)
                {
                    string tag = sortItem.Tag as string;
                    switch (tag)
                    {
                        case "PriceAsc":
                            filtered = filtered.OrderBy(p => p.Price).ToList();
                            break;
                        case "PriceDesc":
                            filtered = filtered.OrderByDescending(p => p.Price).ToList();
                            break;
                        case "Name":
                            filtered = filtered.OrderBy(p => p.ProductName).ToList();
                            break;
                        case "Stock":
                            filtered = filtered.OrderByDescending(p => p.StockQuantity).ToList();
                            break;
                    }
                }

                _filteredProducts.Clear();
                foreach (var item in filtered)
                {
                    _filteredProducts.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterAndSortProducts();
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterAndSortProducts();
        }

        private void cmbManufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterAndSortProducts();
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterAndSortProducts();
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                if (int.TryParse(button.Tag.ToString(), out int productId))
                {
                    ShowProductDetails(productId);
                }
            }
        }

        private void ShowProductDetails(int productId)
        {
            var product = _products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                string details = $"Артикул: {product.ArticleNumber}\n" +
                               $"Название: {product.ProductName}\n" +
                               $"Производитель: {product.ManufacturerName}\n" +
                               $"Категория: {product.CategoryName}\n" +
                               $"Поставщик: {product.SupplierName}\n" +
                               $"{product.PriceText}\n" +
                               $"Единица измерения: {product.UnitName}\n" +
                               $"В наличии: {product.StockQuantity} {product.UnitName}\n" +
                               $"\nОписание:\n{product.Description}";

                if (product.StockQuantity <= 0)
                {
                    details += $"Товар временно отсутствует на складе";
                }

                // Для администратора показываем кастомное окно
                if (_userRole == "Администратор")
                {
                    ShowAdminProductDialog(productId, details);
                }
                else
                {
                    MessageBox.Show(details, "Информация о товаре",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ShowAdminProductDialog(int productId, string details)
        {
            var dialog = new Window
            {
                Title = "Информация о товаре",
                Width = 550,
                Height = 550,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.CanResize,
                Background = Brushes.White,
                MinWidth = 500,
                MinHeight = 450
            };

            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            mainGrid.Margin = new Thickness(10);

            // Заголовок
            var header = new TextBlock
            {
                Text = "Управление товаром",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(header, 0);
            mainGrid.Children.Add(header);

            // Информация о товаре в TextBox с прокруткой
            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var detailsBox = new TextBox
            {
                Text = details,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 13,
                Padding = new Thickness(5),
                IsReadOnly = true,
                Background = Brushes.WhiteSmoke,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.LightGray,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 300 
            };

            scrollViewer.Content = detailsBox;
            Grid.SetRow(scrollViewer, 1);
            mainGrid.Children.Add(scrollViewer);

            // Кнопки управления
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var editButton = new Button
            {
                Content = "Редактировать",
                Width = 120,
                Height = 32,
                Margin = new Thickness(5),
                Background = Brushes.Orange,
                Foreground = Brushes.White,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold
            };
            editButton.Click += (s, args) =>
            {
                dialog.DialogResult = true;
                dialog.Close();
                NavigationService.Navigate(new UpdatePage(productId));
            };

            var deleteButton = new Button
            {
                Content = "Удалить",
                Width = 80,
                Height = 32,
                Margin = new Thickness(5),
                Background = Brushes.Red,
                Foreground = Brushes.White,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold
            };
            deleteButton.Click += (s, args) =>
            {
                dialog.DialogResult = true;
                dialog.Close();
                DeleteProduct(productId);
            };

            var closeButton = new Button
            {
                Content = "Закрыть",
                Width = 80,
                Height = 32,
                Margin = new Thickness(5),
                Background = Brushes.Gray,
                Foreground = Brushes.White,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold
            };
            closeButton.Click += (s, args) =>
            {
                dialog.DialogResult = false;
                dialog.Close();
            };

            buttonPanel.Children.Add(editButton);
            buttonPanel.Children.Add(deleteButton);
            buttonPanel.Children.Add(closeButton);

            Grid.SetRow(buttonPanel, 2);
            mainGrid.Children.Add(buttonPanel);

            dialog.Content = mainGrid;
            dialog.ShowDialog();
        }
        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole == "Администратор")
            {
                NavigationService.Navigate(new AddProductPage());
            }
            else
            {
                MessageBox.Show("У вас нет прав для добавления товаров", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole == "Администратор")
            {
                MessageBox.Show("Для редактирования товара нажмите на кнопку 'Подробнее' у нужного товара",
                    "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("У вас нет прав для редактирования товаров", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole == "Администратор")
            {
                MessageBox.Show("Для удаления товара нажмите на кнопку 'Подробнее' у нужного товара",
                    "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("У вас нет прав для удаления товаров", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
       

        private void DeleteProduct(int productId)
        {
            if (MessageBox.Show($"Вы уверены, что хотите удалить товар ID: {productId}?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new ShoeStoreEntities())
                    {
                        var product = context.Products.Find(productId);
                        if (product != null)
                        {
                            context.Products.Remove(product);
                            context.SaveChanges();

                            var productToRemove = _products.FirstOrDefault(p => p.ProductID == productId);
                            if (productToRemove != null)
                            {
                                _products.Remove(productToRemove);
                                _filteredProducts.Remove(productToRemove);
                            }

                            MessageBox.Show("Товар успешно удален", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении товара: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
