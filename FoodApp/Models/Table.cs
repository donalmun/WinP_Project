using System.ComponentModel;

namespace FoodApp
{
    public class Table : INotifyPropertyChanged
    {
        private int _id;
        private string _tableName;
        
        private int _status;

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

        

        public int Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}