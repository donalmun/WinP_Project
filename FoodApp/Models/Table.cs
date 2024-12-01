using System.ComponentModel;

namespace FoodApp
{
    public class Table : INotifyPropertyChanged
    {
        private int _id;
        private string _tableName;
        private int _capacity;
        private byte _status;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Table_Name
        {
            get => _tableName;
            set
            {
                _tableName = value;
                OnPropertyChanged(nameof(Table_Name));
            }
        }

        public int Capacity
        {
            get => _capacity;
            set
            {
                _capacity = value;
                OnPropertyChanged(nameof(Capacity));
            }
        }

        public byte Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}