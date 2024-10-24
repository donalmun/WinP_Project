﻿using System;
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
    public float Cost { get; set; }
    public string Status { get; set; }
    public int User_Id { get; set; }
    public DateTime Created_At { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}
