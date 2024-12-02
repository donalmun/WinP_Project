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
    public partial class OrderPage : Page
    {
        public OrderViewModel ViewModel { get; }

        public OrderPage()
        {
            this.InitializeComponent();
            ViewModel = new OrderViewModel();
            this.DataContext = ViewModel;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Product product)
            {
                ViewModel.AddToDetail(product);
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Detail detail)
            {
                if (detail.Quantity > 1)
                {
                    detail.Quantity--;
                }
                else
                {
                    ViewModel.RemoveFromDetail(detail);
                }
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Detail detail)
            {
                detail.Quantity++;
            }
        }

        private void RemoveDetail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Detail detail)
            {
                ViewModel.RemoveFromDetail(detail);
            }
        }

        private void FontIcon_Click(object sender, RoutedEventArgs e)
        {
            PopupContainer.Visibility = Visibility.Visible;
            DiscountPopup.IsOpen = true;

            Button button = sender as Button;
            ViewModel.SelectedDetailItem = button?.Tag as Detail;
        }

        private void SaveOrder_Click(object sender, RoutedEventArgs e)
        {
            PopupContainer.Visibility = Visibility.Visible;
            PaymentOptionsPopup.IsOpen = true;
        }

        private async void PaymentOptionsControl_PaymentConfirmed(object sender, RoutedEventArgs e)
        {
            var paymentControl = sender as Service.Controls.PaymentOptionsControl;
            PaymentOptionsPopup.IsOpen = false;

            // Optionally, you can use the selected payment method
            string paymentMethod = paymentControl.SelectedPaymentMethod;

            // Proceed with saving the order
            await ViewModel.SaveOrderAsync();

            // Optionally, show a success message
            var dialog = new ContentDialog
            {
                Title = "Thành công",
                Content = "Đơn hàng đã được lưu thành công.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot // Set the XamlRoot here
            };
            await dialog.ShowAsync(); ;
        }
        private void PaymentOptionsControl_PaymentCanceled(object sender, RoutedEventArgs e)
        {
            PaymentOptionsPopup.IsOpen = false; // Close the popup
            PopupContainer.Visibility = Visibility.Collapsed;
        }

        private void DiscountPopup_Opened(object sender, object e)
        {
            PopupContainer.Visibility = Visibility.Visible;
        }

        private void DiscountPopup_Closed(object sender, object e)
        {
            PopupContainer.Visibility = Visibility.Collapsed;
        }

        private async void GenerateDetailFile_Click(object sender, RoutedEventArgs e)
        {
            var invoiceItems = (this.DataContext as OrderViewModel)?.Details;
            if (invoiceItems == null) return;

            double totalInvoice = invoiceItems.Sum(item => item.Sub_Total); // Changed to double for compatibility

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

                    var table = new iText.Layout.Element.Table(4).UseAllAvailableWidth();
                    table.AddHeaderCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Tên món").SetFont(font)));
                    table.AddHeaderCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Số lượng").SetFont(font)));
                    table.AddHeaderCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Đơn giá").SetFont(font)));
                    table.AddHeaderCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Thành tiền").SetFont(font)));

                    var cultureInfo = new CultureInfo("vi-VN");
                    foreach (var item in invoiceItems)
                    {
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
                        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(item.Unit_Price.ToString("C0", cultureInfo)).SetFont(font)));
                        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(item.Sub_Total.ToString("C0", cultureInfo)).SetFont(font)));
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
                if (ViewModel.SelectedDetailItem != null)
                {
                    ViewModel.SelectedDetailItem.DiscountType = selectedItem.Tag.ToString();
                }
            }
        }

        private void SurchargeCalculationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                // Update SurchargeType based on selection
                if (ViewModel.SelectedDetailItem != null)
                {
                    ViewModel.SelectedDetailItem.SurchargeType = selectedItem.Tag.ToString();
                }
            }
        }

        private async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var viewModel = DataContext as OrderViewModel;
                await viewModel.SearchCustomersAsync(sender.Text);
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var selectedCustomer = args.SelectedItem as Customer;
            var viewModel = DataContext as OrderViewModel;
            if (selectedCustomer != null)
            {
                viewModel.PhoneNumber = selectedCustomer.Phone;
                viewModel.SelectedCustomer = selectedCustomer;
            }
        }

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

        private void GoToMembershipPage_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to RegisterMembership
            this.Frame.Navigate(typeof(CustomerManagementPage));
        }
    }
}