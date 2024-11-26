using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp;

public class Order : INotifyPropertyChanged
{
    public int Id { get; set; }
    public DateTime Order_Date { get; set; }
    public float Total_Amount { get; set; }
    public byte Status { get; set; }
    public int? Customer_Id { get; set; }
    public int? Table_Id { get; set; }
    public DateTime Created_At { get; set; }

    public Customer Customer { get; set; }
    public Table Table { get; set; }
    public ICollection<Detail> Details { get; set; }
    public ICollection<Payment> Payments { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}