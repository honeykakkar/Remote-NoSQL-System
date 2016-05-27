/////////////////////////////////////////////////////////////////////////////////
// CommunicationManager.cs - Implementation of WCF message-passing service     //
//                                                                             //
// Author: Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////////////
/*
 * Additions to the C# Console Wizard code:
 * - added reference to System.ServiceModel
 * - added using System.ServiceModel
 */
/* Required files: UtilityMethods.cs, ICommunicationManager.cs
 *
 * Maintenance History:
 * --------------------
 * ver 2.0 : 18 Nov 2015
 * Added IsQEmpty() method
 *
 * ver 1.0 : 18 Oct 2015
 * - first release
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace RemoteNoSQL
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]  // Service keeps on running for a session. Time lease is extended based on the current status.
    public class CommunicationManager : ICommunicationManager
    {
        // static rcvrQueue is shared by all instances of this class
        private static MessageQueue<Message> ReceivingQueue = new MessageQueue<Message>();

        public void sendMessage(Message message)  // To send message across, one must enqueue the message to the message queue.
        {
            ReceivingQueue.enqueue(message);
        }

        //----< called by server, blocks caller while empty >----------------
        /*
         * Note: this is NOT a service method - see interface definition
         */

        public Message getMessage()  // To retrieve message at head from the Q
        {
            return ReceivingQueue.dequeue();
        }

        public bool IsQEmpty()  // determines whether Q is empty
        {
            return ReceivingQueue.IsEMPTYQ();
        }
    }
}
