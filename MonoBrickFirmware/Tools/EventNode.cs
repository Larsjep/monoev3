using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoBrickFirmware.Tools
{
    public class EventNode
    {
        private Delegate EventToRaise;
        private Object[] Parameters;

        public EventNode(Delegate Event, Object[] Parameters)
        {
            this.EventToRaise = Event;
            this.Parameters = Parameters;
        }

        public Delegate eventToRaise {
            get { return this.EventToRaise; }
        }
        public Object[] parameters {
            get { return this.Parameters; }
        }
    }
}
