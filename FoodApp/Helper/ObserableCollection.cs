using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp
{
    public sealed class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        public ObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        public ObservableCollection(IEnumerable<T> pItems) : this()
        {
            foreach (var item in pItems)
            {
                this.Add(item);
            }
        }

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is T changedItem)
            {
                int index = IndexOf(changedItem);
                if (index >= 0 && index < Count)
                {
                    // Safely raise the Replace event only if the index is valid
                    NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace,
                        changedItem,
                        changedItem,
                        index
                    );
                    OnCollectionChanged(args);
                }
                else
                {
                    // Optional: Log or handle the scenario where the item is not found
                    Console.WriteLine($"ItemPropertyChanged: Item at index {index} is out of range.");
                }
            }
        }
    }
}