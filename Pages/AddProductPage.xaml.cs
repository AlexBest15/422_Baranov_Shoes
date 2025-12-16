using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _422_Baranov_Shoes.Pages
{
    public partial class AddProductPage : Page
    {
        public AddProductPage()
        {
            InitializeComponent();
            LoadReferenceData();
        }

        private void LoadReferenceData()
        {
            try
            {
                using (var context = new ShoeStoreEntities())
                {
                    // Загружаем производителей
                    cmbManufacturer.Items.Clear();
                    cmbManufacturer.Items.Add("-- Выберите производителя --");
                    foreach (var manufacturer in context.Manufacturers.ToList())
                    {
                        cmbManufacturer.Items.Add(manufacturer);
                    }
                    cmbManufacturer.SelectedIndex = 0;

                    // Загружаем категории
                    cmbCategory.Items.Clear();
                    cmbCategory.Items.Add("-- Выберите категорию --");
                    foreach (var category in context.Categories.ToList())
                    {
                        cmbCategory.Items.Add(category);
                    }
                    cmbCategory.SelectedIndex = 0;

                    // Загружаем поставщиков
                    cmbSupplier.Items.Clear();
                    cmbSupplier.Items.Add("-- Выберите поставщика --");
                    foreach (var supplier in context.Suppliers.ToList())
                    {
                        cmbSupplier.Items.Add(supplier);
                    }
                    cmbSupplier.SelectedIndex = 0;

                    // Загружаем единицы измерения
                    cmbUnit.Items.Clear();
                    cmbUnit.Items.Add("-- Выберите единицу --");
                    foreach (var unit in context.Units.ToList())
                    {
                        cmbUnit.Items.Add(unit);
                    }
                    cmbUnit.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке справочников: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных
                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    MessageBox.Show("Введите название товара", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtProductName.Focus();
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Введите корректную цену", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPrice.Focus();
                    return;
                }

                if (!int.TryParse(txtDiscount.Text, out int discount) || discount < 0 || discount > 100)
                {
                    MessageBox.Show("Скидка должна быть от 0 до 100%", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDiscount.Focus();
                    return;
                }

                if (!int.TryParse(txtStockQuantity.Text, out int stockQuantity) || stockQuantity < 0)
                {
                    MessageBox.Show("Введите корректное количество", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtStockQuantity.Focus();
                    return;
                }

                // Получаем выбранные значения
                var selectedManufacturer = cmbManufacturer.SelectedItem as Manufacturers;
                var selectedCategory = cmbCategory.SelectedItem as Categories;
                var selectedSupplier = cmbSupplier.SelectedItem as Suppliers;
                var selectedUnit = cmbUnit.SelectedItem as Units;

                if (selectedManufacturer == null || cmbManufacturer.SelectedIndex == 0)
                {
                    MessageBox.Show("Выберите производителя", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создаем новый товар
                using (var context = new ShoeStoreEntities())
                {
                    var newProduct = new Products
                    {
                        ArticleNumber = txtArticleNumber.Text,
                        Description = txtDescription.Text,
                        Price = price,
                        CurrentDiscount = discount, 
                        StockQuantity = stockQuantity,
                        ManufacturerID = selectedManufacturer.ManufacturerID 
                    };

                    if (selectedCategory != null && cmbCategory.SelectedIndex > 0)
                    {
                        newProduct.CategoryID = selectedCategory.CategoryID;
                    }

                    if (selectedSupplier != null && cmbSupplier.SelectedIndex > 0)
                    {
                        newProduct.SupplierID = selectedSupplier.SupplierID;
                    }

                    if (selectedUnit != null && cmbUnit.SelectedIndex > 0)
                    {
                        newProduct.UnitID = selectedUnit.UnitID;
                    }

                    var productName = new ProductNames
                    {
                        ProductName = txtProductName.Text
                    };
                    context.ProductNames.Add(productName);
                    context.SaveChanges();

                    newProduct.ProductNameID = productName.ProductNameID;

                    context.Products.Add(newProduct);
                    context.SaveChanges();

                    MessageBox.Show("Товар успешно добавлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении товара: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}