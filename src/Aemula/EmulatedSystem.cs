using System.Collections.Generic;
using System.Linq;
using Aemula.UI;

namespace Aemula
{
    public abstract class EmulatedSystem
    {
        public virtual void Reset()
        {
            
        }

        public virtual IEnumerable<DebuggerWindow> CreateDebuggerWindows()
        {
            return Enumerable.Empty<DebuggerWindow>();
        }
    }
}
