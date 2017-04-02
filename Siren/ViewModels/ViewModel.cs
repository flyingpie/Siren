using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siren.ViewModels
{
    public abstract class ViewModel
    {
        public event EventHandler Changed = delegate { };

        public ViewModel()
        {

        }

        protected void SetChanged()
        {
            Changed(this, EventArgs.Empty);
        }
    }
}
