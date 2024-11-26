using FoodApp.Helper;
using FoodApp.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using System.IO;
using iText.Layout.Properties;
using System.Collections.Generic;
using System.Globalization;
using FoodApp.Views;

namespace FoodApp
{
    public partial class OrderPage : global::Microsoft.UI.Xaml.Controls.Page
    {
        public MainViewModel ViewModel { get; }

        // Constructor
        public OrderPage()
        {
            this.InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Product product)
            {
                ViewModel.AddToInvoice(product);
            }
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle checkout button click event here
            DisplayCheckoutMessage();
        }

        private void DisplayCheckoutMessage()
        {
            var dialog = new ContentDialog
            {
                Title = "Thanh toán",
                Content = "Bạn đã nhấn nút thanh toán.",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot // Set the XamlRoot property
            };

            _ = dialog.ShowAsync();
        }

        private void GoToLoginPage_Click(object sender, RoutedEventArgs e)
        {
            // Handle login page navigation here
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is InvoiceItem item)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                }
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is InvoiceItem item)
            {
                item.Quantity++;
            }
        }

        private void FontIcon_Click(object sender, RoutedEventArgs e)
        {
            PopupContainer.Visibility = Visibility.Visible;
            DiscountPopup.IsOpen = true;

            Button button = sender as Button;
            ViewModel.SelectedInvoiceItem = button?.Tag as InvoiceItem;
        }

        private void ShowDiscountPopup()
        {
            PopupContainer.Visibility = Visibility.Visible;
            DiscountPopup.IsOpen = true;
        }

        private void DiscountPopup_Opened(object sender, object e)
        {
            PopupContainer.Visibility = Visibility.Visible;
        }

        private void DiscountPopup_Closed(object sender, object e)
        {
            PopupContainer.Visibility = Visibility.Collapsed;
        }

        private async void GenerateInvoiceFile_Click(object sender, RoutedEventArgs e)
        {
            var invoiceItems = (this.DataContext as MainViewModel)?.InvoiceItems;
            if (invoiceItems == null) return;

            decimal totalInvoice = (decimal)invoiceItems.Sum(item => (double)item.TotalPrice);

            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("PDF", new List<string> { ".pdf" });

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            savePicker.SuggestedFileName = $"HoaDon_{timestamp}";

            var hwnd = ((App)Application.Current).GetWindowHandle();
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    var writer = new iText.Kernel.Pdf.PdfWriter(stream);
                    var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
                    var document = new iText.Layout.Document(pdf);

                    string fontPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "arial.ttf");
                    var fontProgram = iText.IO.Font.FontProgramFactory.CreateFont(fontPath, true);
                    var font = iText.Kernel.Font.PdfFontFactory.CreateFont(fontProgram, iText.IO.Font.PdfEncodings.IDENTITY_H);

                    var titleParagraph = new iText.Layout.Element.Paragraph("HÓA ĐƠN")
                        .SetFont(font)
                        .SetFontSize(22)
                        .SetBold()
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    document.Add(titleParagraph);

                    var dateParagraph = new iText.Layout.Element.Paragraph($"Ngày lập hóa đơn: {DateTime.Now:dd/MM/yyyy}")
                        .SetFont(font)
                        .SetFontSize(12)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                    document.Add(dateParagraph);

                    var table = new iText.Layout.Element.Table(5).UseAllAvailableWidth();
                    table.AddHeaderCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("STT").SetFont(font)));
                    table.AddHeaderCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Tên món").SetFont(font)));
                    table.AddHeaderCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Số lượng").SetFont(font)));
                    table.AddHeaderCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Đơn giá").SetFont(font)));
                    table.AddHeaderCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Thành tiền").SetFont(font)));

                    int index = 1;
                    var cultureInfo = new CultureInfo("vi-VN");
                    foreach (var item in invoiceItems)
                    {
                        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(index.ToString()).SetFont(font)));

                        var productParagraph = new iText.Layout.Element.Paragraph(item.Product.Name)
                            .SetFont(font)
                            .SetFontSize(12)
                            .SetBold();

                        if (!string.IsNullOrEmpty(item.Note))
                        {
                            productParagraph.Add("\n")
                                .Add(new iText.Layout.Element.Text($"Ghi chú: {item.Note}")
                                .SetFontSize(8)
                                .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK));
                        }

                        if (item.Discount > 0)
                        {
                            productParagraph.Add("\n")
                                .Add(new iText.Layout.Element.Text($"Giảm giá: {item.Discount} {item.DiscountType}")
                                .SetFontSize(8)
                                .SetFontColor(iText.Kernel.Colors.ColorConstants.BLUE));
                        }

                        if (item.Surcharge > 0)
                        {
                            productParagraph.Add("\n")
                                .Add(new iText.Layout.Element.Text($"Phụ phí: {item.Surcharge} {item.SurchargeType}")
                                .SetFontSize(8)
                                .SetFontColor(iText.Kernel.Colors.ColorConstants.RED));
                        }

                        table.AddCell(new iText.Layout.Element.Cell().Add(productParagraph));
                        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(item.Quantity.ToString()).SetFont(font)));

                        // Corrected line with VND formatting
                        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(item.UnitPrice.ToString("C0", cultureInfo)).SetFont(font)));

                        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(item.TotalPrice.ToString("C0", cultureInfo)).SetFont(font)));
                        index++;
                    }

                    document.Add(table);

                    var totalParagraph = new iText.Layout.Element.Paragraph($"Tổng cộng: {totalInvoice.ToString("C0", cultureInfo)}")
                        .SetFont(font)
                        .SetFontSize(14)
                        .SetBold()
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                        .SetMarginTop(20);
                    document.Add(totalParagraph);

                    document.Close();
                }
            }
        }



        private void DiscountCalculationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                // Update DiscountType based on selection
                if (ViewModel.SelectedInvoiceItem != null)
                {
                    ViewModel.SelectedInvoiceItem.DiscountType = selectedItem.Tag.ToString();
                }
            }
        }

        private void SurchargeCalculationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                // Update SurchargeType based on selection
                if (ViewModel.SelectedInvoiceItem != null)
                {
                    ViewModel.SelectedInvoiceItem.SurchargeType = selectedItem.Tag.ToString();
                }
            }
        }

        //private void RemoveItem_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Button button && button.Tag is InvoiceItem item)
        //    {
        //        ViewModel.RemoveFromInvoice(item.Product);
        //    }
        //}

        private async void FilterByCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out int categoryId))
            {
                await ViewModel.FilterProductsByCategoryAsync(categoryId);
            }
        }


        private void GoToRevenueView_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to RevenueView
            this.Frame.Navigate(typeof(RevenueView));
        }

        private void GoToManagementPage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ProductManagementPage)); 
        }
    }
}