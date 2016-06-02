using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogger
{
    public class Event
    {
        public string EventName;
        public DateTime EventTime;
        public string EventOccuredAt;
        public bool EventPassed;
        public string EventTriggeredBy;
    }
}
