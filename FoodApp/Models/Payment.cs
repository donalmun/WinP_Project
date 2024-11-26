using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FoodApp;

public class Payment : INotifyPropertyChanged
{
    public int Id { get; set; }
    public int Order_Id { get; set; }
    public string Payment_Method { get; set; }
    public DateTime? Payment_Date { get; set; }
    public decimal Amount_Paid { get; set; }

    public Order Order { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}
