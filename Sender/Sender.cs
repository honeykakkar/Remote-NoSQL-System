/////////////////////////////////////////////////////////////////////////
// Sender.cs - CommunicationManager Sender connects and sends messages  //
// ver 2.1                                                             //
// Author: Jim Fawcett, CSE681 - Software Modeling and Analysis        //
//         Project #4                                                  //
/////////////////////////////////////////////////////////////////////////
/*
 * - has a dedicated sendThread that reads from application queue,
 *   looks at message.toUrl and finds or creates a proxy for that
 *   destination, then sends message
 */
/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to System.ServiceModel
 * - Added using System.ServiceModel
 * - Added references to ICommunicationManager, MessageQueue, and UtilityMethods
 */
/* Required Files: ICommunicationManager.cs, MessageQueue.cs, and UtilityMethods.cs

 * Maintenance History:
 * --------------------
 * ver 2.1 : 29 Oct 2015
 * - added statement to store proxy in connect
 * - moved ProxyDB to data member instead of local variable
 * - renamed svc to proxy
 * - moved message creation out of connect attempt loop
 * - added overridable notifiers
 * - added shutDown()
 * ver 2.0 : 24 Oct 2015
 * - added sender queue and thread
 * - now, user just uses sendMessage(msg).  The sendThread examines
 *   msg destination and routes to the appropriate proxy, creating
 *   one if necessary.
 * - several helper functions added
 * ver 1.0 : 20 Oct 2015
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ServiceModel;
using System.Threading;

namespace RemoteNoSQL
{
    public class Sender
    {
        public string LocalURL { get; set; } = "http://localhost:8081/CommunicationManager";
        public string RemoteURL { get; set; } = "http://localhost:8080/CommunicationManager";
        public int MaxConnectAttempts { get; set; } = 10;

        ICommunicationManager Proxy = null;
        MessageQueue<Message> SendingQueue = null;
        Dictionary<string, ProxyAndDeadLetterQ> ProxyDB = new Dictionary<string, ProxyAndDeadLetterQ>();
        Action sendAction = null;

        //----< define send thread processing and start thread >-------------

        public Sender(string LocalUrl = "http://localhost:8081/CommServer")
        {
            LocalURL = LocalUrl;
            SendingQueue = defineSendProcessing(); // defines thread, creates Proxy and get it going
            startSender();
        }
        //----< Proxy implements the service interface >---------------------
        /*
         * An instance of the Proxy is what the client uses to make
         * calls on the server's remote service instance.  
         */
        ICommunicationManager CreateProxy(string RemoteURL)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress address = new EndpointAddress(RemoteURL);
            ChannelFactory<ICommunicationManager> factory = new ChannelFactory<ICommunicationManager>(binding, address);
            return factory.CreateChannel();
        }
        //----< is sender connected to the specified url? >------------------

        public bool isConnected(string url) // for reusing already created Proxy
        {
            return ProxyDB.ContainsKey(url);
        }
        //----< Connect repeatedly tries to send messages to service >-------

        public bool Connect(string RemoteURL)
        {
            if (UtilityMethods.verbose)
                sendMsgNotify("attempting to connect");
            if (isConnected(RemoteURL))
                return true;
            Proxy = CreateProxy(RemoteURL);
            int attemptNumber = 0;
            Message startMsg = new Message();
            startMsg.FromURL = LocalURL;
            startMsg.ToURL = RemoteURL;
            startMsg.TimeSent = DateTime.Now;
            startMsg.MessageContent = "connection start message";
            while (attemptNumber < MaxConnectAttempts)
            {
                try
                {
                    Proxy.sendMessage(startMsg);                                          // will throw if server isn't listening yet
                    ProxyAndDeadLetterQ ProxyAndDLQ = new ProxyAndDeadLetterQ();
                    ProxyAndDLQ.PROXY = Proxy;
                    ProxyDB[RemoteURL] = ProxyAndDLQ;  // remember this Proxy


                    if (UtilityMethods.verbose)
                        sendMsgNotify("connected");
                    return true;
                }
                catch
                {
                    ++attemptNumber;
                    sendAttemptNotify(attemptNumber);
                    Thread.Sleep(100);
                }
            }
            return false;
        }
        //----< overridable message annunciator >----------------------------

        public virtual void sendMsgNotify(string message)
        {
            Console.Write("\n  {0}\n", message);
        }
        //----< overridable attemptHandler >---------------------------------

        public virtual void sendAttemptNotify(int attemptNumber)
        {
            Console.Write("\n  connection attempt #{0}", attemptNumber);
        }
        //----< close connection - not used in this demo >-------------------

        public void CloseConnection()
        {
            Proxy = null;
        }
        //----< set send action >--------------------------------------------
        /*
         * Installs send thread action in sender.
         */
        public void setAction(Action sendAct)
        {
            sendAction = sendAct;
        }
        //----< send messages to remote Receivers >--------------------------

        public void startSender()
        {
            sendAction.Invoke();
        }
        //----< send a message to remote Receiver >--------------------------

        public bool sendMessage(Message message)
        {
            SendingQueue.enqueue(message);
            return true;
        }
        //----< defines SendThread and its operations >----------------------
        /*
         * - asynchronous function defines Sender sendThread processing
         * - creates MessageQueue<Message> to use inside Sender.sendMessage()
         * - creates and starts a thread executing that processing
         * - uses message.ToURL to find or create a Proxy for url destination
         */
        public virtual MessageQueue<Message> defineSendProcessing()
        {
            MessageQueue<Message> SendingQueue = new MessageQueue<Message>();
            Action sendAction = () =>
            {
                ThreadStart sendThreadProc = () =>
                {
                    while (true)
                    {
                        try
                        {
                            Message SendingMessage = SendingQueue.dequeue();
                            if (SendingMessage.MessageContent == "closeSender")
                            {
                                Console.Write("\n  send thread quitting\n\n");
                                break;
                            }
                            if (ProxyDB.ContainsKey(SendingMessage.ToURL))
                                ProxyDB[SendingMessage.ToURL].PROXY.sendMessage(SendingMessage);
                            else
                            {
                                ProxyAndDeadLetterQ ProxyAndDLQ1 = new ProxyAndDeadLetterQ();
                                // create new Proxy with Connect, save it, and use it
                                if (this.Connect(SendingMessage.ToURL))  // if Connect succeeds it will set Proxy and send start msg
                                {
                                    ProxyAndDLQ1.PROXY = Proxy;
                                    ProxyDB[RemoteURL] = ProxyAndDLQ1;
                                    ProxyDB[SendingMessage.ToURL] = ProxyAndDLQ1;  // save Proxy
                                    Proxy.sendMessage(SendingMessage);
                                }
                                else
                                {
                                    sendMsgNotify(String.Format("could not connect to {0}\n", SendingMessage.ToURL));
                                    ProxyAndDLQ1.DEADLETTERQUEUE.enqueue(SendingMessage);
                                    continue;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sendExceptionNotify(ex);
                            continue;
                        }
                    }
                };
                Thread t = new Thread(sendThreadProc);  // start the sendThread
                t.IsBackground = true;
                t.Start();
            };
            this.setAction(sendAction);
            return SendingQueue;
        }
        //----< overridable exception annunciator >--------------------------

        public virtual void sendExceptionNotify(Exception ex, string message = "")
        {
            Console.Write("\n --- {0} ---\n", ex.Message);
        }
        //----< sets urls from CommandLine if defined there >----------------

        public void processCommandLine(string[] args)
        {
            if (args.Length > 0)
            {
                LocalURL = UtilityMethods.processCommandLineForLocal(args, LocalURL);
                RemoteURL = UtilityMethods.processCommandLineForRemote(args, RemoteURL);
            }
        }
        //----< send closeSender message to local sender >-------------------

        public void shutdown()
        {
            Message sdmsg = new Message();
            sdmsg.FromURL = LocalURL;
            sdmsg.ToURL = LocalURL;
            sdmsg.MessageContent = "closeSender";
            Console.Write("\n  shutting down local sender");
            sendMessage(sdmsg);
        }
        //----< Test Stub >--------------------------------------------------

#if (TEST_SENDER)
    static void Main(string[] args)
    {
      UtilityMethods.verbose = false;

      Console.Write("\n  starting CommunicationManager Sender");
      Console.Write("\n =============================\n");

      Console.Title = "CommunicationManager Sender";

      Sender sndr = new Sender("http://localhost:8081/CommunicationManager");

      sndr.processCommandLine(args);

      int numMsgs = 5;
      int counter = 0;
      Message message = null;
      while (true)
      {
        message = new Message();
        message.FromURL = sndr.LocalURL;
        message.ToURL = sndr.RemoteURL;
        message.MessageContent = "Message #" + (++counter).ToString();
        Console.Write("\n  sending {0}", message.MessageContent);
        sndr.sendMessage(message);
        Thread.Sleep(30);
        if (counter >= numMsgs)
          break;
      }

      message = new Message();
      message.FromURL = sndr.LocalURL;
      message.ToURL = "http://localhost:9999/CommunicationManager";
      message.MessageContent = "no listener for this message";
      Console.Write("\n  sending {0}", message.MessageContent);
      sndr.sendMessage(message);
      message = new Message();
      message.FromURL = sndr.LocalURL;
      message.ToURL = sndr.RemoteURL;
      message.MessageContent = "Message #" + (++counter).ToString();
      Console.Write("\n  sending {0}", message.MessageContent);
      sndr.sendMessage(message);
      message = new Message();
      message.FromURL = sndr.LocalURL;
      message.ToURL = sndr.RemoteURL;
      message.MessageContent = "closeSender";  // message for self and Receiver
      Console.Write("\n  sending {0}", message.MessageContent);
      sndr.sendMessage(message);
    }
#endif
    }
}
