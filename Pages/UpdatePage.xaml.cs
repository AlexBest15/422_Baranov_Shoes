using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _422_Baranov_Shoes.Pages
{
    public partial class UpdatePage : Page
    {
        private int _productId = 0;
        private bool _isNewProduct = false;

        public UpdatePage()
        {
            InitializeComponent();
            _isNewProduct = true;
            txtTitle.Text = "Добавление нового товара";
            LoadReferenceData();
        }

        public UpdatePage(int productId)
        {
            InitializeComponent();
            _productId = productId;
            _isNewProduct = false;
            txtTitle.Text = "Редактирование товара";
            LoadReferenceData();
            LoadProductData();
        }

        private void LoadProductData()
        {
            try
            {
                using (var context = new ShoeStoreEntities())
                {
                    var product = context.Products.Find(_productId);
                    if (product != null)
                    {
                        txtProductName.Text = GetProductName(product);
                        txtArticleNumber.Text = product.ArticleNumber ?? "";
                        txtDescription.Text = product.Description ?? "";
                        txtPrice.Text = product.Price.ToString("0.##");
                        txtDiscount.Text = product.CurrentDiscount?.ToString("0.##") ?? "0";
                        txtStockQuantity.Text = product.StockQuantity.ToString();

                        SetComboBoxSelection(cmbManufacturer, product.ManufacturerID);
                        SetComboBoxSelection(cmbCategory, product.CategoryID);
                        SetComboBoxSelection(cmbSupplier, product.SupplierID);
                        SetComboBoxSelection(cmbUnit, product.UnitID);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных товара: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetProductName(Products product)
        {
            if (product.ProductNames != null)
                return product.ProductNames.ProductName;

            if (product.ProductNameID > 0)
            {
                using (var context = new ShoeStoreEntities())
                {
                    var name = context.ProductNames.Find(product.ProductNameID);
                    if (name != null)
                        return name.ProductName;
                }
            }

            return product.ArticleNumber ?? "Товар без названия";
        }

        private void SetComboBoxSelection(ComboBox comboBox, int? id)
        {
            if (id == null || id == 0)
            {
                comboBox.SelectedIndex = 0;
                return;
            }

            foreach (var item in comboBox.Items)
            {
                if (item is Manufacturers manufacturer && manufacturer.ManufacturerID == id)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
                else if (item is Categories category && category.CategoryID == id)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
                else if (item is Suppliers supplier && supplier.SupplierID == id)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
                else if (item is Units unit && unit.UnitID == id)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }

            comboBox.SelectedIndex = 0;
        }

        private void LoadReferenceData()
        {
            try
            {
                using (var context = new ShoeStoreEntities())
                {
                    cmbManufacturer.Items.Clear();
                    cmbManufacturer.Items.Add(new Manufacturers { ManufacturerID = 0, ManufacturerName = "-- Выберите --" });
                    foreach (var manufacturer in context.Manufacturers.ToList())
                    {
                        cmbManufacturer.Items.Add(manufacturer);
                    }
                    cmbManufacturer.SelectedIndex = 0;

                    cmbCategory.Items.Clear();
                    cmbCategory.Items.Add(new Categories { CategoryID = 0, CategoryName = "-- Выберите --" });
                    foreach (var category in context.Categories.ToList())
                    {
                        cmbCategory.Items.Add(category);
                    }
                    cmbCategory.SelectedIndex = 0;

                    cmbSupplier.Items.Clear();
                    cmbSupplier.Items.Add(new Suppliers { SupplierID = 0, SupplierName = "-- Выберите --" });
                    foreach (var supplier in context.Suppliers.ToList())
                    {
                        cmbSupplier.Items.Add(supplier);
                    }
                    cmbSupplier.SelectedIndex = 0;

                    cmbUnit.Items.Clear();
                    cmbUnit.Items.Add(new Units { UnitID = 0, UnitName = "-- Выберите --" });
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

                var selectedManufacturer = cmbManufacturer.SelectedItem as Manufacturers;
                var selectedCategory = cmbCategory.SelectedItem as Categories;
                var selectedSupplier = cmbSupplier.SelectedItem as Suppliers;
                var selectedUnit = cmbUnit.SelectedItem as Units;

                if (selectedManufacturer == null || selectedManufacturer.ManufacturerID == 0)
                {
                    MessageBox.Show("Выберите производителя", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var context = new ShoeStoreEntities())
                {
                    if (_isNewProduct)
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

                        if (selectedCategory != null && selectedCategory.CategoryID > 0)
                            newProduct.CategoryID = selectedCategory.CategoryID;

                        if (selectedSupplier != null && selectedSupplier.SupplierID > 0)
                            newProduct.SupplierID = selectedSupplier.SupplierID;

                        if (selectedUnit != null && selectedUnit.UnitID > 0)
                            newProduct.UnitID = selectedUnit.UnitID;

                        var productName = new ProductNames
                        {
                            ProductName = txtProductName.Text
                        };
                        context.ProductNames.Add(productName);
                        context.SaveChanges();
                        newProduct.ProductNameID = productName.ProductNameID;

                        context.Products.Add(newProduct);
                        context.SaveChanges();

                        MessageBox.Show("Товар успешно добавлен", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var product = context.Products.Find(_productId);
                        if (product != null)
                        {
                            product.ArticleNumber = txtArticleNumber.Text;
                            product.Description = txtDescription.Text;
                            product.Price = price;
                            product.CurrentDiscount = discount;
                            product.StockQuantity = stockQuantity;
                            product.ManufacturerID = selectedManufacturer.ManufacturerID;

                            if (selectedCategory != null && selectedCategory.CategoryID > 0)
                            {
                                product.CategoryID = selectedCategory.CategoryID;
                            }
                            else
                            {

                                product.CategoryID = 0;
                            }

                            if (selectedSupplier != null && selectedSupplier.SupplierID > 0)
                            {
                                product.SupplierID = selectedSupplier.SupplierID;
                            }
                            else
                            {
                                product.SupplierID = 0;
                            }

                            if (selectedUnit != null && selectedUnit.UnitID > 0)
                            {
                                product.UnitID = selectedUnit.UnitID;
                            }
                            else
                            {
                                product.UnitID = 0;
                            }

                            if (product.ProductNameID > 0)
                            {
                                var productName = context.ProductNames.Find(product.ProductNameID);
                                if (productName != null)
                                {
                                    productName.ProductName = txtProductName.Text;
                                }
                            }
                            else
                            {
                                var productName = new ProductNames
                                {
                                    ProductName = txtProductName.Text
                                };
                                context.ProductNames.Add(productName);
                                context.SaveChanges();
                                product.ProductNameID = productName.ProductNameID;
                            }

                            context.SaveChanges();

                            MessageBox.Show("Товар успешно обновлен", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }

                NavigationService.GoBack();
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