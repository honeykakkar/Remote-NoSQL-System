/////////////////////////////////////////////////////////////////////////////////////////////////
// WriteClient.cs - It is the write client of the application which sends queries to server   //
//                 to modify any particular information in the database which is hosted      //
//                 on server itself. Its many instances can be created using starter package//
//                 server.                                                    ///////////////
// Application: Remote NoSQL database implementation, Project 4, CSE681-SMA  //
// Language:    C#, Framework 4.5.2, Visual Studio 2015 (Community Edt.)     //
// Platform:    Dell Inspiron 13 7000, Core-i7, Windows 10                   //
// Author:      Honey Kakkar, Computer Engineering, SU                       //
//              hkakkar@syr.edu                                              //
// Source:      Jim Fawcett, CST 4-187, Syracuse University                  //
//              (315) 443-3948, jfawcett@twcny.rr.com                        //
///////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package has a receiver and send to communicate across endpoints. 
 *
 * Command arguments can be used to preset its localURL, RemoteURL and switch which determines whether messages to be logged on console or GUI.
 *
 * It contains methods to process sending and receiving streams, processing result queries from server.
 *
 * This client contains query methods only to update information in database residing on server
 *
 * Maintenance:
 * ------------
 * Required Files: ICommunicationManager.cs, Receiver.cs, Sender.cs, UtilityMethods.cs
 *
 * Build Process:  devenv RemoteNoSQL.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 3.0 : 18 Nov 2015
 * Changed service action of receiver from default service action
 * Added methods to process received messages 
 *
 * 
 * ver 2.0 : 20 Oct 2015
 * - replaced almost all functionality with a Sender instance
 * - added Receiver to retrieve Server echo messages.
 * 
 * ver 1.0 : 18 Oct 2015
 * - first release
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;

namespace RemoteNoSQL
{                             //NOTE :
    public class WriteClient  // If WPF wants to send user-defined queries to any client, he/she may turn off automatic processing of queries from load of XML message stream at startup
    {
        string LocalURL { get; set; } = "http://localhost:8083/CommunicationManager";
        string RemoteURL { get; set; } = "http://localhost:8080/CommunicationManager";
        bool LogMessages = false;
        //----< retrieve urls from the CommandLine if there are any >--------

        public void processCommandLine(string[] args)
        {
            if (args.Length == 0)
                return;
            LocalURL = UtilityMethods.processCommandLineForLocal(args, LocalURL);
            RemoteURL = UtilityMethods.processCommandLineForRemote(args, RemoteURL);
            if (args[2].ToUpper() == "T")
                LogMessages = true;         // 
            if (args[2].ToUpper() == "F")  // sets the LogMessages according to commandline argument. If yes, sent messages are displayed on console.
                LogMessages = false;
        }

        // It process a message stream which contains a number of queries
        // It splits the message stream into individual query messages which are sent one by one to server to be processed.

        public void ProcessStream(ref Message ReceivedMessage, ref WriteClient Writeclient, ref Sender WCSender)
        {
            XDocument XMLDoc = XDocument.Parse(ReceivedMessage.MessageContent);
            Console.WriteLine("\n  Latest message stream to WriteClient has been currently saved in the directory\n");
            "  XML Message stream being sent to the server for processing".Wrap();
            var Queries = XMLDoc.Element("MessageStream").Elements();
            try
            {
                foreach (var Query in Queries)
                {
                    int NumberofQ = int.Parse(Query.LastAttribute.Value);   // Last attribute contains information about number of queries of this particular type in stream
                    Query.LastAttribute.Remove();                           // It is removed as every message is sent to server as a standalone entity 
                    for (int i = 1; i <= NumberofQ; ++i)
                    {
                        Message MSG = new Message();
                        MSG.FromURL = ReceivedMessage.ToURL;
                        MSG.ToURL = Writeclient.RemoteURL;
                        MSG.MessageContent = Query.ToString();
                        MSG.TimeSent = DateTime.Now;
                        if (Writeclient.LogMessages == true)                // Requirement 6 is met here as sent messages are logged to console only if command prompt argument says so
                        {
                            String.Format("  Sending a message to {0}", MSG.ToURL).Wrap();
                            Console.Write("\n  Sender is {0}", MSG.FromURL);
                            Console.Write("\n  MessageContent is\n  {0}", MSG.MessageContent);
                        }
                        WCSender.sendMessage(MSG);   // message is sent across using sender of write client
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n  " + e.Message + "\n");
            }
        }

        static void Main(string[] args)
        {
            "\n  Starting Write Client or writer for database".Wrap();
            Console.Title = "WriteClient";
            WriteClient Writeclient = new WriteClient();
            Writeclient.processCommandLine(args);
            String.Format("  Url of Write Client is {0}", Writeclient.LocalURL).Wrap();
            string localPort = UtilityMethods.urlPort(Writeclient.LocalURL);
            string localAddr = UtilityMethods.urlAddress(Writeclient.LocalURL);
            Receiver WCReceiver = new Receiver(localPort, localAddr);
            Sender WCSender = new Sender(Writeclient.LocalURL);
            int NumberofQ = 0;
            XDocument XMLDoc = new XDocument();
            Message MessageStream = new Message();

            // New service action is defined for receiver which handles the way receiver behaves on received message in WriteClient

            Action WCserviceAction = () =>
            {
                try
                {
                    Message ReceivedMessage = new Message();
                    while (true)
                    {
                        ReceivedMessage = WCReceiver.getMessage();   // note use of non-service method to deQ messages
                        if (ReceivedMessage.ToURL != ReceivedMessage.FromURL)
                        {
                            ReceivedMessage.TimeReceived = DateTime.Now;
                            ReceivedMessage.TravelTime = DateTime.Now - ReceivedMessage.TimeSent;
                            "\n  Received a new message".Wrap();
                            Console.Write("\n  Sender is {0}", ReceivedMessage.FromURL);
                            Console.Write("\n  MessageContent is\n  {0}", ReceivedMessage.MessageContent);
                            Console.Write("\n  Message was sent on {0}", ReceivedMessage.TimeSent);
                            Console.Write("\n  Message was received on {0}", DateTime.Now);                                                 //Demonstarting another way to convert milliseconds to microseconds. Although, all server measurments of time are done using high resolution timer.
                            Console.Write("\n  Message took {0} milliseconds or {1} microseconds on communication channel (round-trip, including its processing)\n", (DateTime.Now - ReceivedMessage.TimeSent).TotalMilliseconds, (DateTime.Now.Ticks / 10 - ReceivedMessage.TimeSent.Ticks / 10));
                            if (ReceivedMessage.FromURL == Writeclient.RemoteURL)
                                Console.Write("\n  Server took {0} microseconds to process this query)\n", ReceivedMessage.ServerProcessTime);
                        }
                        if (ReceivedMessage.MessageContent.StartsWith("<MessageStream>"))
                        {
                            Writeclient.ProcessStream(ref ReceivedMessage, ref Writeclient, ref WCSender);
                            "XML Message stream have been sent to the server for processing".Wrap();
                            MessageStream = ReceivedMessage;
                            XMLDoc = XDocument.Parse(ReceivedMessage.MessageContent);
                            foreach (var element in XMLDoc.Element("MessageStream").Elements())         // To know the total number of queries in original message
                                NumberofQ += int.Parse(element.LastAttribute.Value);
                            XMLDoc.Element("MessageStream").Elements().Remove();                        // As client would be receiving two types of messages.
                        }                                                                               // One is MessageStream XML message and other is Reply messages from server

                        if (ReceivedMessage.MessageContent.StartsWith("<QueryType"))
                        {
                            XElement QueryResult = XElement.Parse(ReceivedMessage.MessageContent);
                            XMLDoc.Root.Add(QueryResult);
                            --NumberofQ;                                                                    // Reference for received message stream is saved in order to send the same message back to its source
                            if (NumberofQ == 0)                                                             // NumberofQ counted the number of queries. When all are processed,
                            {                                                                               // server is notified about that.
                                Message Final = new Message();
                                Final.ToURL = Writeclient.RemoteURL;
                                Final.FromURL = Writeclient.LocalURL;
                                Final.MessageContent = "DONE !! All queries from this client have been processed.";
                                Final.TimeSent = DateTime.Now;
                                WCSender.sendMessage(Final);                                        
                                MessageStream.MessageContent = XMLDoc.ToString();
                                XMLDoc.Save("Results.xml");
                                if (MessageStream.ToURL != MessageStream.FromURL)
                                {
                                    UtilityMethods.swapUrls(ref MessageStream);
                                    WCSender.sendMessage(MessageStream);
                                }
                            }
                        }
                        if (ReceivedMessage.MessageContent == "closeReceiver")
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("  " + e.Message);
                }
            };

            if (WCReceiver.StartService())
                WCReceiver.doService(WCserviceAction);              // Receiver for write client is started to perform the action defined

            // Comment following lines till 209 to enable this client to be able to process user defined query
            // comment following lines in order to disable this client to perform queries automatically

            Message LoadXMLMessage = new Message();
            LoadXMLMessage.ToURL = Writeclient.LocalURL;
            LoadXMLMessage.FromURL = Writeclient.LocalURL;
            string ID = UtilityMethods.urlPort(Writeclient.LocalURL).Substring(2);
            LoadXMLMessage.MessageID = "WC" + ID;
            LoadXMLMessage.MessageContent = XDocument.Load("WCMessageStream.xml").ToString();           // It is used to load the previously created message stream. It is fed to its processing queue.
            MessageStream = LoadXMLMessage;                                           // Reference is saved in order to avoid any loss of information.
            WCSender.sendMessage(LoadXMLMessage);
            Thread.Sleep(100);
            UtilityMethods.waitForUser();
            Console.Write("\n\n");
        }
    }
}
