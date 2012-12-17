using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3.Controls
{
    public class DataList<T> : ICollection<T>
    {
        public event EventHandler<DataListItemEventArgs<T>> ItemAdded = delegate { };
        public event EventHandler<DataListItemEventArgs<T>> ItemRemoved = delegate { };
        public event EventHandler ListCleared = delegate { };

        private List<T> items;

        public DataList()
        {
            items = new List<T>();
        }

        public new void Add(T item)
        {
            items.Add(item);

            ItemAdded(this, new DataListItemEventArgs<T>(item));
        }

        public new void Remove(T item)
        {
            items.Remove(item);

            ItemRemoved(this, new DataListItemEventArgs<T>(item));
        }

        public new void Clear()
        {
            items.Clear();

            ListCleared(this, EventArgs.Empty);
        }


        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class DataListItemEventArgs<T> : EventArgs
    {
        public DataListItemEventArgs(T item)
        {
            this.Item = item;
        }

        public T Item { get; set; }
    }
}
