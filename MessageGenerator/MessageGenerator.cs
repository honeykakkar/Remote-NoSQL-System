/////////////////////////////////////////////////////////////////////////
// MessageGenerator.cs - Construct ICommService Messages               //
// ver 1.0                                                             //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Purpose:
 *----------
 * This is a placeholder for application specific message construction
 *
 * Additions to C# Console Wizard generated code:
 * - references to ICommunicationManager and UtilityMethods
 */
/* Required Files: UtilityMethods.cs, ICommunicationManager.cs
/*
 * Maintenance History:
 * --------------------
 * ver 1.0 : 29 Oct 2015
 * - first release
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteNoSQL
{
    public class MessageGenerator
    {
        public static int msgCount { get; set; } = 0;
        public Message MessageCreator(string FromURL, string ToURL)
        {
            Message message = new Message();
            message.FromURL = FromURL;
            message.ToURL = ToURL;
            message.MessageContent = String.Format("\n  message #{0}", ++msgCount);
            return message;
        }

#if (TEST_MessageGenerator)
    static void Main(string[] args)
    {
      MessageGenerator mm = new MessageGenerator();
      Message message = mm.MessageCreator("fromFoo", "toBar");
      UtilityMethods.showMessage(message);
      Console.Write("\n\n");
    }
#endif
    }
}