/////////////////////////////////////////////////////////////////////////
// Logger.cs - Prototype for test logger                               //
//                                                                     //
// Jim Fawcett, CSE784 - Software Modeling & Analysis, Fall 2008       //
/////////////////////////////////////////////////////////////////////////
/*
 * Limitations:
 * ============
 * Only supports file logging
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace EventLogger
{
    public class EventLoggerClass : IEventLogger
    {
        public void LogEvent(ref XDocument XD, ref EventLogger.Event E)
        {
            XD.Root.Add(new XElement("EventLog" , new XElement("EventName", E.EventName), new XElement("EventTime", E.EventTime), new XElement("EventOccuredAt", E.EventOccuredAt), new XElement("EventPassed", E.EventPassed), new XElement("EventTriggeredBy", E.EventTriggeredBy)));
        }
    }

    public interface IEventLogger
    {
        void LogEvent(ref XDocument XD, ref Event E);
    }
}
