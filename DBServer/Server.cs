/////////////////////////////////////////////////////////////////////////
// Server.cs - CommunicationManager Server                             //
// ver 2.2                                                             //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to ICommunicationManager, Sender, Receiver, Utilities
 *
 * Note:
 * - This server receives and then sends back processed messages after its operations on databases.
 * It contains processing for messages containing individual queries and as well as Message stream (just in case user wants to test DB using WPF client only).
 */
// Required Files: ICommunicationManager.cs, Sender.cs, Receiver.cs, UtilityMethods.cs, DatabaseDictionary.cs, Element.cs, HiResTimer.cs, TestExecutive.cs
/*
 * Maintenance History:
 * --------------------
 * ver 2.3 : 29 Oct 2015
 * - added handling of special messages: 
 *   "connection start message", "done", "closeServer"
 * ver 2.2 : 25 Oct 2015
 * - minor changes to display
 *
 * ver 2.0 : 20 Oct 2015
 * - Defined Receiver and used that to replace almost all of the
 *   original Server's functionality.
 * ver 1.0 : 18 Oct 2015
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;

namespace RemoteNoSQL
{
    class Server
    {
        XDocument XMLDoc = new XDocument();
        List<ulong> TimeforRetrievals = new List<ulong>(); // List contains all elements as time interval taken by server to process queries of retrieval of any information
        List<ulong> TimeforAdditions = new List<ulong>(); // List contains all elements as time interval taken by server to process queries of addition of new elements
        List<ulong> TimeforDeletions = new List<ulong>(); // List contains all elements as time interval taken by server to process queries of deletion of any element
        static TestExecutive Test = new TestExecutive();  // Not public, but CodeAnalyzer shows it to be
        List<ulong> TimeforModfications = new List<ulong>();  // List contains all elements as time interval taken by server to process queries of modification of any element
        static int TotalQueries = 0;  // Server keeps track of total queries received
        string ServerAddress { get; set; } = "localhost";
        string ServerPort { get; set; } = "8080";

        //----< quick way to grab ports and addresses from commandline >-----

        public void ProcessCommandLine(string[] args)
        {
            if (args.Length > 0)
            {
                ServerPort = args[0];
            }
            if (args.Length > 1)
            {
                ServerAddress = args[1];
            }
        }

        // Method to load databases from XML file
        public void LoadDB()
        {
            Test.XMLProcessing();
            Test.CreateStringDB();
            Test.DisplayDBs();
        }

        // This method is not used when WPF communicates via clients but it would come handy when user wishes WPF to communicate directly to server instead of any client in-between
        // It basically processes whole streams instead of individual messages
        public XDocument ProcessMessageStream(string XMLMessage)
        {
            XMLDoc = XDocument.Parse(XMLMessage);
            int NumberofQ = XMLDoc.Element("MessageStream").Elements().Count();
            var Queries = XMLDoc.Element("MessageStream").Elements();
            string QueryType = null;
            string DBType = null;

            List<string> Results = new List<string>();
            foreach (var Query in Queries)
            {
                List<string> ParameterSelected = new List<string>();
                List<string> ParameterValue = new List<string>();

                if (Query.Name.ToString().Equals("QueryType"))
                {
                    QueryType = Query.Nodes().OfType<XText>().FirstOrDefault().Value;
                    DBType = Query.FirstAttribute.Value.ToString();

                    foreach (var Param in Query.Element("Parameters").Descendants())
                    {
                        ParameterSelected.Add(Param.Name.ToString());
                        ParameterValue.Add(Param.Value.ToString());
                    }
                    Results = TestExecutive.QueryProcessing(DBType, QueryType, ParameterSelected, ParameterValue);
                }
                foreach (string result in Results)
                    Query.Add(new XElement("Result", result));
            }
            XMLDoc.Save("Results.xml");
            return XMLDoc;
        }

        // Method is used to process and send the query for operation on databases as it is received by receiver of server
        public void ProcessMessageQuery(ref Message ServerQuery, ref Sender ServerSender, ref HiResTimer HRTimer)
        {
            XDocument XMLDoc = XDocument.Parse(ServerQuery.MessageContent);
            var Queries = XMLDoc.Element("QueryType").Elements();                   //////
            string QueryType = null, DBType = null;                                     //
            List<string> Results = new List<string>();                                  //
            XElement Query = XMLDoc.Root;                                               //
            List<string> ParameterSelected = new List<string>();                        // Query is forwarded as string of XML message to server from client
            List<string> ParameterValue = new List<string>();                           // It fetches paramaters and values required to process the query on DB.
            QueryType = Query.Nodes().OfType<XText>().FirstOrDefault().Value;           //
            DBType = Query.FirstAttribute.Value.ToString();                             //
            if (Query.Elements("Parameters").Any())                                     //
            {                                                                   //////////
                foreach (var Param in Query.Element("Parameters").Descendants())
                {
                    ParameterSelected.Add(Param.Name.ToString());
                    ParameterValue.Add(Param.Value.ToString());
                }
            }
            HRTimer.Start();            // Timer is started as it is forwarded to process the query on database
            Results = TestExecutive.QueryProcessing(DBType, QueryType, ParameterSelected, ParameterValue);
            HRTimer.Stop();             // Timer is stopped as database is done with the processing of query
            ServerQuery.ServerProcessTime = HRTimer.ElapsedMicroseconds;
            TimeKeeper(QueryType, HRTimer.ElapsedMicroseconds);
            foreach (string result in Results)
                Query.Add(new XElement("Result", result));      // Results of query are added in the same received message
            UtilityMethods.swapUrls(ref ServerQuery);           // Same message is sent back with no update on TimeSent or TimeReceived property in order to
            ServerQuery.MessageContent = XMLDoc.ToString();     // calculate the round-trip time of the message, and that's the reason why same message is used instead of creating a new one
            ServerSender.sendMessage(ServerQuery);
        }

        // Keeps tracks of time taken by database to process each type of query 
        void TimeKeeper(string QueryType, ulong ElapsedTime)
        {
            if (QueryType.StartsWith("Get"))
                TimeforRetrievals.Add(ElapsedTime);
            if (QueryType.StartsWith("Add"))
                TimeforAdditions.Add(ElapsedTime);
            if (QueryType.StartsWith("Delete"))
                TimeforDeletions.Add(ElapsedTime);
            if (QueryType.StartsWith("Modify"))
                TimeforModfications.Add(ElapsedTime);
        }

        // This method measures the performance of server and database based on results received from TimeKeeper method.
        // It computes average time for each time of message 
        XDocument PerformanceMeasurer()
        {
            XDocument Performance = new XDocument();
            Performance.Add(new XElement("Performance"));
            if (TimeforAdditions.Any())
            {
                ulong AddTime = 0;
                foreach (var item in TimeforAdditions)
                    AddTime += item;
                ulong AverageofADD = AddTime / (ulong)TimeforAdditions.Count;
                Performance.Element("Performance").Add(new XElement("AverageofADD", AverageofADD, new XAttribute("NumberofADDQ", TimeforAdditions.Count)));
            }

            if (TimeforDeletions.Any())
            {
                ulong DeleteTime = 0;
                foreach (var item in TimeforDeletions)
                    DeleteTime += item;
                ulong AverageofDELETE = DeleteTime / (ulong)TimeforDeletions.Count;
                Performance.Element("Performance").Add(new XElement("AverageofDELETE", AverageofDELETE, new XAttribute("NumberofDELETEQ", TimeforDeletions.Count)));
            }

            if (TimeforModfications.Any())
            {
                ulong ModifyTime = 0;
                foreach (var item in TimeforModfications)
                    ModifyTime += item;
                ulong AverageofMODIFY = ModifyTime / (ulong)TimeforModfications.Count;
                Performance.Element("Performance").Add(new XElement("AverageofMODIFY", AverageofMODIFY, new XAttribute("NumberofMODIFYQ", TimeforModfications.Count)));
            }

            if (TimeforRetrievals.Any())
            {
                ulong RetrieveTime = 0;
                foreach (var item in TimeforRetrievals)
                    RetrieveTime += item;
                ulong AverageofRETRIEVE = RetrieveTime / (ulong)TimeforRetrievals.Count;
                Performance.Element("Performance").Add(new XElement("AverageofRETRIEVE", AverageofRETRIEVE, new XAttribute("NumberofRETRIEVEQ", TimeforRetrievals.Count)));
            }
            return Performance;
        }

        static void Main(string[] args)
        {
            Server server = new Server();
            (String.Format("\n  Starting Server where databases reside")).Wrap();
            server.ProcessCommandLine(args);
            Console.Title = "Server";
            Sender ServerSender = new Sender(UtilityMethods.makeUrl(server.ServerAddress, server.ServerPort));
            Receiver ServerReceiver = new Receiver(server.ServerPort, server.ServerAddress);
            Console.WriteLine("  Listening on ServerPort {0}\n", server.ServerPort);
            server.LoadDB();
            HiResTimer HRTimer = new HiResTimer();
            bool first = true;
            int TotalNOC = 0, NOC = 0;  // To know number of clients connected to server
            Action serviceAction = () =>
            {
                Message ServerQuery = null;
                while (true)
                {
                    bool a = ServerReceiver.IsEMPTY(); // checks whether receiving Q is empty
                    if (!(first == false && a == true)) // This check helps in determining the time when server is done with the processing of all queries
                    {
                        ServerQuery = ServerReceiver.getMessage();// note use of non-service method to deQ messages
                        first = false;
                        if (ServerQuery.MessageContent != "closeReceiver" && !ServerQuery.MessageContent.StartsWith("<NoOfClient"))
                        {
                            String.Format("\n  Received new message from {0}", ServerQuery.FromURL).Wrap();
                            Console.Write("\n  MessageContent is\n  {0}", ServerQuery.MessageContent);
                            Console.Write("\n  Message was sent on {0}", ServerQuery.TimeSent);
                            Console.Write("\n  Message was received on {0}", DateTime.Now);
                            Console.Write("\n  Message took {0} milliseconds or {1} microseconds on communication channel\n", (DateTime.Now - ServerQuery.TimeSent).TotalMilliseconds, (DateTime.Now.Ticks / 10 - ServerQuery.TimeSent.Ticks / 10));
                        }
                        if (ServerQuery.MessageContent == "connection start message")
                            continue; // don't send back start message

                        if (ServerQuery.MessageContent == "closeReceiver")
                        {
                            Console.Write("  Received closeReceiver");
                            break;
                        }
                        if (ServerQuery.MessageContent.StartsWith("<QueryType"))
                        {
                            ++TotalQueries;
                            server.ProcessMessageQuery(ref ServerQuery, ref ServerSender, ref HRTimer);
                            Thread.Sleep(100);
                        }

                        if (ServerQuery.MessageContent.StartsWith("<MessageStream"))
                        {
                            ++TotalQueries;
                            server.ProcessMessageStream(ServerQuery.MessageContent);
                            Thread.Sleep(100);
                        }

                        if ((ServerQuery.MessageContent.StartsWith("<NoOfClient")))
                            TotalNOC = int.Parse(XDocument.Parse(ServerQuery.MessageContent).Root.Value);

                        if (ServerQuery.MessageContent.StartsWith("DONE"))  // Increment counter as a client is done. If number of elements in ProxyDB is equal to this, it means all clients are done.
                        {
                            ++NOC;  // Increment it every time a client is done
                            "\n\n  Please wait while performance is calculated. It may take some time as performance is being calculated at a very granular level".Wrap();
                            "  Almost there. Just a peek of performance in XML format".Wrap();
                            ServerSender.Connect(UtilityMethods.makeUrl("localhost", "8081"));              ////  WPF Port Address. Change it if you change WPF local port.
                            XDocument SendPerformance = server.PerformanceMeasurer();                         //
                            Message PM = new Message();                                                       // If yes, send the performance results to WPF
                            PM.MessageContent = SendPerformance.ToString();                                   //
                            PM.FromURL = UtilityMethods.makeUrl(server.ServerAddress, server.ServerPort);     //
                            PM.ToURL = UtilityMethods.makeUrl("localhost", "8081");
                            Console.WriteLine("\n" + SendPerformance.ToString() + "\n" + "\n  One may check the summary of server hosting the database under 'Summary' tab of UserInterface.");
                            ServerSender.sendMessage(PM);
                        }
                    }
                    else
                    {
                        //TotalNOC = TestExecutive.NoOfClients;
                        if (NOC != 0 && TotalNOC != 0 && TotalNOC == NOC)   // Check if all clients are done sending messages to server AND server is done sending all messages. If yes, shutdown receiver. // Requirement odf shutting down is met when clients are started using WPF
                            ServerReceiver.shutDown();
                    }
                }
            };

            if (ServerReceiver.StartService())
                ServerReceiver.doService(serviceAction); // This serviceAction asynchronous, so the call doesn't block.
            // Test.DisplayDBafterProcessing();  // When readers and writers are initiated by WPF, it asks whether to display DBs after processing
            UtilityMethods.waitForUser();
        }
    }
}