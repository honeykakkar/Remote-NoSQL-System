/////////////////////////////////////////////////////////////////////////////
// ITest.cs - Defines ITest, ILogger, and ITestVectorGenerator interfaces  //
//                                                                         //
// Jim Fawcett, CSE784 - Software Modeling & Analysis, Fall 2008           //
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace EventLogger
{
  public interface IEventLogger
  {
    void LogEvent(ref XDocument XD, ref Event E);
  }
}
