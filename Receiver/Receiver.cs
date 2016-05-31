/////////////////////////////////////////////////////////////////////////
// Receiver.cs - CommunicationManager Receiver listens for messages    //
// ver 2.1                                                             //
// Author: Dr. Jim Fawcett, CSE681 - Software Modeling and Analysis    //
//         Project #4                                                  //
/////////////////////////////////////////////////////////////////////////
/*
 * Receiver:
 * - listens for incoming connection requests
 * - provides sendMessage() for Senders to post messages
 * - provides serviceAction to determine what happens to received messages
 */
/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to System.ServiceModel
 * - Added using System.ServiceModel
 * - Added using System.ServiceModel.Description
 * - Added reference to ICommunicationManager
 * - Added reference to CommunicationManager
 */
/*
 * Maintenance History:
 * --------------------
 * ver 2.1 : 29 Oct 2015
 * - added comment prologue to shutDown()
 * ver 2.0 : 24 Oct 2015
 * - Provided mechanism to define new Receiver msg processing.
 *   See methods defaultServiceAction, serverProcessMessage, and doService.
 * - to see more detail about what is going on in Sender and Receiver

 * ver 1.0 : 18 Oct 2015
 * - first release
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;

namespace RemoteNoSQL
{
    public class Receiver
    {
        public string address { get; set; }
        /*
         * Port should be greater than 1024.
         * If you choose a port already in use the listener will throw an exception.
         */
        public string port { get; set; }

        CommunicationManager ReceivingQ = null;
        ServiceHost Host = null;

        //----< constructor sets listening endpoint >------------------------

        public Receiver(string Port = "8080", string Address = "localhost")
        {
            address = Address;
            port = Port;
        }

        //----< creates listener but does not start it >---------------------

        public ServiceHost CreateListener()
        {
            string url = "http://" + this.address + ":" + this.port + "/CommunicationManager";
            BasicHttpBinding binding = new BasicHttpBinding();
            Uri address = new Uri(url);
            Type service = typeof(CommunicationManager);
            ServiceHost Host = new ServiceHost(service, address);
            Host.AddServiceEndpoint(typeof(ICommunicationManager), binding, address);
            return Host;
        }
        //----< Create CommunicationManager and listener and start it >---------------

        public bool StartService()
        {
            if (UtilityMethods.verbose)
                Console.Write("\n  Receiver starting service");
            try
            {
                Host = CreateListener();
                Host.Open();
                ReceivingQ = new CommunicationManager(); // to get messages from Q
            }
            catch (Exception ex)
            {
                Console.Write("\n\n --- creation of Receiver listener failed ---\n");
                Console.Write("\n    {0}", ex.Message);
                Console.Write("\n    exiting\n\n");
                return false;
            }
            return true;
        }
        //----< serviceAction defines what happens to received messages >----
        /*
         * - Default service action is to display each received message.
         * - serverProcessMessage(message) does nothing, but can be overridden
         *   to provide additional server processing.
         */
        public Action defaultServiceAction() // being used for demonstration
        {
            Action serviceAction = () =>
            {
                if (UtilityMethods.verbose)
                    Console.Write("\n  starting Receiver.defaultServiceAction");
                Message message = null;
                while (true)
                {
                    message = getMessage();   // note use of non-service method to deQ messages
                    Console.Write("\n  Received message:");
                    Console.Write("\n  sender is {0}", message.FromURL);
                    Console.Write("\n  MessageContent is {0}\n", message.MessageContent);
                    if (message.MessageContent == "closeReceiver")
                        break;
                }
            };
            return serviceAction;
        }
        
        //----< run the service action >-------------------------------------
        /*
         * - Provides a mechanism for applications to define service operations.
         *   Look at test stub for an example of how to define the serviceAction.
         * - Runs asynchronously
         */
        public void doService(Action serviceAction)
        {
            ThreadStart ts = () =>
            {
                if (UtilityMethods.verbose)
                    Console.Write("\n  doService thread started");
                serviceAction.Invoke();  // usually has while loop that runs until closed
            };
            Thread t = new Thread(ts); // Otherwise, main thread would be blocked. 
            t.IsBackground = true;
            t.Start();
        }
        //----< runs defaultServiceAction >----------------------------------

        public void doService()
        {
            doService(defaultServiceAction());
        }
        //----< application hosting Receiver calls this method >-------------

        public Message getMessage()
        {
            if (UtilityMethods.verbose)
                Console.Write("\n  calling CommunicationManager.getMessage()");
            Message message = ReceivingQ.getMessage();
            if (UtilityMethods.verbose)
                Console.Write("\n  returned from CommunicationManager.getMessage()");
            return message;
        }
        //----< send closeReceiver message to local Receiver >---------------

        public void shutDown()
        {
            Console.Write("\n  local receiver shutting down");
            Message message = new Message();
            message.MessageContent = "closeReceiver";
            message.ToURL = UtilityMethods.makeUrl(address, port);
            message.FromURL = message.ToURL;
            message.TimeSent = DateTime.Now;
            UtilityMethods.showMessage(message);
            ReceivingQ.sendMessage(message);
            Host.Close();
        }

        public bool IsEMPTY()
        {
            return ReceivingQ.IsQEmpty();
        }
        //----< quick way to grab ports and addresses from commandline >-----

        public void ProcessCommandLine(string[] args)
        {
            if (args.Length > 0)
                port = args[0];
            if (args.Length > 1)
                address = args[1];
        }
        //----< Test Stub >--------------------------------------------------

#if (TEST_RECEIVER)

    static void Main(string[] args)
    {
      UtilityMethods.verbose = true;

      Console.Title = "CommunicationManager Receiver";
      Console.Write("\n  Starting CommunicationManager Receiver");
      Console.Write("\n ===============================\n");

      Receiver rcvr = new Receiver();
      rcvr.ProcessCommandLine(args);

      Console.Write("\n  Receiver url = {0}\n", UtilityMethods.makeUrl(rcvr.address, rcvr.port));

      // serviceAction defines what the server does with received messages

      if (rcvr.StartService())
        rcvr.doService(rcvr.defaultServiceAction());  // equivalent to rcvr.doService()

      Console.Write("\n  press any key to exit: ");
      Console.ReadKey();
      Console.Write("\n\n");
    }
#endif
    }
}
