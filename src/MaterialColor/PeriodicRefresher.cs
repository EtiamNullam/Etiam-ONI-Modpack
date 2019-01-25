using MaterialColor.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MaterialColor.IO
{
    public class PeriodicRefresher : IRender1000ms
    {
        void IRender1000ms.Render1000ms(float dt)
        {
            if (State.ConfigChanged)
            {
                Painter.Refresh();
                State.ConfigChanged = false;
            }
        }
    }
}
