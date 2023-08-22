using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMeThatPage3.Helpers.State
{
    public class CSSState : State
    {
        public bool needsRenaming { get; set; }
        public bool isRenamed { get; set; }
        public bool needsContentChanged { get; set; }
        public bool isContentChanged { get; set; }
    }
}
