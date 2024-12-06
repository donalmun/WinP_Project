// EditTableControl.xaml.cs
using Microsoft.UI.Xaml.Controls;

namespace FoodApp.Service.Controls
{
    public sealed partial class EditTableControl : UserControl
    {
        public Table UpdatedTable { get; private set; }

        public EditTableControl(Table table)
        {
            this.InitializeComponent();
            UpdatedTable = table;
            this.DataContext = UpdatedTable;
            
        }
    }
}
