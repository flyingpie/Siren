using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace B3.Controls
{
    public class DataListView<T> : ListView
    {
        private DataList<T> list;

        public DataListView()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        public Func<T, string[]> ViewFunc { get; set; }

        public void Bind(DataList<T> list)
        {
            this.list = list;

            list.ItemAdded += OnItemAdded;
            list.ItemRemoved += OnItemRemoved;
            list.ListCleared += OnListCleared;
        }

        public void Unbind()
        {
            list.ItemAdded -= OnItemAdded;
            list.ItemRemoved -= OnItemRemoved;
            list.ListCleared -= OnListCleared;
        }

        private void OnItemAdded(object sender, DataListItemEventArgs<T> e)
        {
            this.UIThread(delegate
            {
                if (ViewFunc != null)
                {
                    Items.Add(new ListViewItem(ViewFunc(e.Item)) { Tag = e.Item });
                }
                else
                {
                    Items.Add(new ListViewItem(e.Item.ToString()) { Tag = e.Item });
                }
            });
        }

        private void OnItemRemoved(object sender, DataListItemEventArgs<T> e)
        {

        }

        private void OnListCleared(object sender, EventArgs e)
        {

        }

        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }

    static class FormExtensions
    {
        static public void UIThread(this Control control, MethodInvoker code)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(code);
                return;
            }
            code.Invoke();
        }
    }
}
