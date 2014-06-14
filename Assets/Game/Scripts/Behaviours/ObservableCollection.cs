using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace KXSGCodec
{
    public class ObservableCollection<T> : ICollection<T>, INotifyPropertyChanged
    {
        private readonly ICollection<T> mCollection;

        public ObservableCollection(IEnumerable<T> collection)
        {
            mCollection = new List<T>(collection);
        }

        public ObservableCollection(int capacity)
        {
            mCollection = new List<T>(capacity);
        }

        public ObservableCollection()
        {
            mCollection = new List<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return mCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return mCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            mCollection.CopyTo(array, arrayIndex);
        }

        public void Add(T item)
        {
            mCollection.Add(item);
            OnPropertyChanged(this, "");
        }

        public void Clear()
        {
            mCollection.Clear();
            OnPropertyChanged(this, "");
        }

        public bool Remove(T item)
        {
            bool remove = mCollection.Remove(item);
            if(remove)
            {
                OnPropertyChanged(this, "");
            }
            return remove;
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(object sender, string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}


