using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtgLifeCounter
{
    public delegate void LifeChangedEventHandler(object sender, LifeChangedEventArgs e);

    public class LifeChangedEventArgs : EventArgs
    {
        public int Lifechanged { get; private set; }
        public LifeChangedEventArgs(int change)
        {
            Lifechanged = change;
        }
    }
}
