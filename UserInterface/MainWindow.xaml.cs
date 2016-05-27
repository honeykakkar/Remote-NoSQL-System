//////////////////////////////////////////////////////////////////////////////////////////////
// MainWindow.xaml.cs - It is the userinterface of the application. It                      //
//                      contains code which defines lements and action associated with them //
//                      It can send and receive message stream to and from client as well as//
//                      server. It can also be considered as tool to manipulate query       //
//                      messages                                              ////////////////
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
 * This package defines how an user interface should look like and behave. 
 *
 * Contains action on button clicks. It has its own sender and receiver to communicate with
 * other endpoints
 *
 * It contains methods to setup channels, sending and receiving streams, processing result streams.
 *
 * Maintenance:
 * ------------
 * Required Files: ICommunicationManager.cs, Receiver.cs, Sender.cs, MessageGenerator.cs, UtilityMethods.cs, Starter.cs
 *
 * Build Process:  devenv RemoteNoSQL.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 18 Nov 2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace RemoteNoSQL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XDocument MessageXML = new XDocument(); // Xdocument instance to send and retrieve XML messages to and from other sources.
        static bool firstConnect = true; 
        static Receiver UIReceiver = null; // Receiver implemented from Receiver package. UserInterface, just like others
        static WPFSender UISender = null;  // has its own sender and receiver. 
        string LocalAddress = "localhost";
        string LocalPort = "8081";
        string RemoteAddress = "localhost";
        string RemotePort = "8080";
        static int RCMessageID = 0; // Used to assign ids to a message being sent from ReadClient tab to a reader
        static int WCMessageID = 0; // Used to assign ids to a message being sent from WriteClient tab to a writer

        static List<TimeSpan> TravelTime = new List<TimeSpan>(); // Used to store travel time or round trip time of each message. 
                                                                 // It is not much used right now, but idea is that when WPF connecting directly to server may need it.

        public class WPFSender : Sender
        {
            TextBox lStat_ = null;  // reference to UIs local status textbox
            System.Windows.Threading.Dispatcher dispatcher_ = null; //Dispatcher used to send results or text to UI thread

            public WPFSender(TextBox lStat, System.Windows.Threading.Dispatcher dispatcher)
            {
                dispatcher_ = dispatcher;  // use to send results action to main UI thread. 
                lStat_ = lStat;
            }
            public override void sendMsgNotify(string msg)                                    ////////
            {                                                                                       //
                Action act = () => { lStat_.Text = msg; };                                          //
                dispatcher_.Invoke(act);                                                            //
                                                                                                    // 
            }                                                                                       // Methods used to know the status of connection using dispatcher
            public override void sendExceptionNotify(Exception ex, string msg = "")                 //
            {                                                                                       //
                Action act = () => { lStat_.Text = ex.Message; };                                   //
                dispatcher_.Invoke(act);                                                            //
            }                                                                                       //
            public override void sendAttemptNotify(int attemptNumber)                               //
            {                                                                              ///////////
                Action act = null;
                act = () => { lStat_.Text = String.Format("Attempt to send in #{0} try", attemptNumber); };
                dispatcher_.Invoke(act);
            }
        }

        // This is the entry point of UI. Initializes the maini window.
        public MainWindow()
        {
            InitializeComponent();
            ServerPerformance.Focus();
            Title = "UserInterface";
            setupChannel();
            XElement MessageStream = new XElement("MessageStream");  // An element is added in the above defined XMLDoc to initiate a message stream being sent.
            MessageXML.Add(MessageStream);                           // Client adds children to it. Children are queries along with the numbers and required paramaeters
        }
        //----< trim off leading and trailing white space >------------------

        string trim(string msg)
        {
            StringBuilder sb = new StringBuilder(msg);
            for (int i = 0; i < sb.Length; ++i)
                if (sb[i] == '\n')
                    sb.Remove(i, 1);
            return sb.ToString().Trim();
        }
        //----< indirectly used by child receive thread to post results >----
        // This method is used to process the XML message received from server which contains all necessary performance related information
        // It contains average time. Throughput is calculated here. 
        public void postPerformanceMsg(string content)
        {
            StringBuilder PerformanceResult = new StringBuilder();          //
            XDocument PerformanceResultDOC = XDocument.Parse(content);      // This method publishes the result to Listbox in "Summary" tab of WPF.
            foreach(var type in PerformanceResultDOC.Root.Elements())       //
            {
                if (type.Name == "AverageofADD")
                    PerformanceResult.Append(String.Format("\n Total queries to ADD: {0}\n Average Time taken by server to process ADD queries: {1} microseconds\n Inferring that server can process {2} ADD queries per second", type.LastAttribute.Value, type.Value, (1000000/ (ulong.Parse(type.Value)))));
                if (type.Name == "AverageofDELETE")
                    PerformanceResult.Append(String.Format("\n\n Total queries to DELETE: {0}\n Average Time taken by server to process DELETE queries: {1} microseconds\n Inferring that server can process {2} DELETE queries per second", type.LastAttribute.Value, type.Value, (1000000 / (ulong.Parse(type.Value)))));
                if (type.Name == "AverageofMODIFY")
                    PerformanceResult.Append(String.Format("\n\n Total queries to MODIFY: {0}\n Average Time taken by server to process MODIFY queries: {1} microseconds\n Inferring that server can process {2} MODIFY queries per second", type.LastAttribute.Value, type.Value, (1000000 / (ulong.Parse(type.Value)))));
                if (type.Name == "AverageofRETRIEVE")
                    PerformanceResult.Append(String.Format("\n\n Total queries to RETRIEVE are: {0}\n Average Time taken by server to process RETRIEVE queries: {1} microseconds\n Inferring that server can process {2} RETRIEVE queries per second", type.LastAttribute.Value, type.Value, (1000000 / (ulong.Parse(type.Value)))));
            }
            ulong TotalQ = 0;
            foreach (var type in PerformanceResultDOC.Root.Elements())
                TotalQ += ulong.Parse(type.LastAttribute.Value);
            PerformanceResult.Append(String.Format("\n Total Queries performed: {0}", TotalQ)); // for displaying total number of queries performed by server.
            ServerPerformance.Items.Insert(0, PerformanceResult);
        }

        // This method contains code for action to be taken when an XML containg query stream is fed to receiver's Q. 
        public void postRcvMsg(string content, string id)
        {
            if (content.StartsWith("<MessageStream>"))                                      // Message of querystreams is received only when server and clients are done with processing the message
            {                                                                               // and wants to publish the results to WPF. Results are published under respective client tab.
                XDocument ReceivedDoc = XDocument.Parse(content);
                int NumberofQ = ReceivedDoc.Element("MessageStream").Elements().Count();
                var Queries = ReceivedDoc.Element("MessageStream").Elements();
                foreach (var Q in Queries)
                {
                    List<string> Results = new List<string>();
                    foreach( var Q1 in Q.Elements("Result"))
                        Results.Add(Q1.Value.ToString());
                    StringBuilder SendResult = new StringBuilder();
                    foreach (string result in Results.Distinct())
                        SendResult.Append(result + " | ");                                      // Results from received back messagestream are being retreived from XML messages and then displayed
                    if(id.StartsWith("RC"))
                        RCResults.Items.Insert(0, SendResult);
                    if (id.StartsWith("WC"))
                        WCResults.Items.Insert(0, SendResult);
                }                                                                               // Summary of server is also provided when a message is originally sent from WPF
                StringBuilder Performance = new StringBuilder();                                // along with main summary. It contains information about when a message stream was sent, received.
                Performance.Append("Message stream with message id: " + id + " was processed and sent by read client to server.\nIt was sent at " + TimeSent + 
                    " and received back at " + TimeReceived + " \nafter processing of each messsage query included in the stream.\nThe time, message stream spent on communication channels and server is total " + (TimeReceived - TimeSent).TotalMilliseconds + " milliseconds");

                if (id.StartsWith("RC"))
                    Read_Client_Performance.Items.Insert(0, Performance);
                if (id.StartsWith("WC"))
                    Write_Client_Performance.Items.Insert(0, Performance);
            }
        }
        //----< used by main thread >----------------------------------------

        DateTime TimeSent;
        DateTime TimeReceived;

        // Method contains code to be executed upon sending of a message. Currently, as per requirements, it is not much used except storing TimeSent when a message is sent
        public void postSndMsg(string content)
        {
            TimeSent = DateTime.Now;
        }
        //----< get Receiver and Sender running >----------------------------

        void setupChannel()
        {
            UIReceiver = new Receiver(LocalPort, LocalAddress);  // A erceiver for UI is created using the local port and local address. A receiver is defined in Receiver package.
            Action serviceAction = () =>                            // ServiceAction is the backbone of WCF communication. It defines the behaviour of WCF
            {                                                       // as it contains code that maniplute XML messages.
                try
                {
                    Message rmsg = null;
                    while (true)
                    {
                        rmsg = UIReceiver.getMessage();                 // As a message is received, it stores its time to
                        TimeReceived = DateTime.Now;                     // TimeReceived attribute of the message      
                        rmsg.TimeReceived = DateTime.Now;
                        rmsg.TravelTime = (rmsg.TimeReceived - rmsg.TimeSent);      // Difference of times when a message is received and sent. It contains round-trip time.
                        TravelTime.Add(rmsg.TravelTime);
                        if (rmsg.MessageContent.StartsWith("<MessageStream"))                      /////// 
                        {                                                                               //
                            Action act = () => { postRcvMsg(rmsg.MessageContent, rmsg.MessageID); };    // There are two different actions which are taken depending on the type of 
                            Dispatcher.Invoke(act);                                                     // received message. If message contains preformance info, it is handled by
                                                                                                        //   different method
                        }                                                                               //
                        if (rmsg.MessageContent.StartsWith("<Per"))                                 //////
                        {                                                                           
                            {
                                Action act = () => { postPerformanceMsg(rmsg.MessageContent); };
                                Dispatcher.Invoke(act);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Action act = () => { lStat.Text = ex.Message; };
                    Dispatcher.Invoke(act);
                }
            };

            if (UIReceiver.StartService())                   // It starts the service pf receiver defined in serviceaction
                UIReceiver.doService(serviceAction);

            UISender = new WPFSender(lStat, this.Dispatcher);
        }
        //----< set up channel after entering ports and addresses >----------

        RadioButton CheckedParRB;   //
        RadioButton CheckedDBRB;    // References to check which Parameter is passed to query, and on which database to operate on
        RadioButton CheckedParRB1;  //
        RadioButton CheckedDBRB1;   //

        // It is associated with AddMessage of ReadCLient tab. An XML is created based upon the user input. XML is not sent until send button is clicked
        private void RCAddMessage_Click(object sender, RoutedEventArgs e)
        {
            // For this code analyzer shows it to have a size of 297 and CC of 38. Please comment if I can improve this code in terms of size. Thank you.
            int NumberofQ = 0;
            int k;
            if (RCQueryType.SelectedItem == null || ((bool)RCMetadata.IsChecked && RCMetadataDB.SelectedItem == null) || RCParameter.Text == null || RCParameter.Text == "" || CheckedDBRB == null || CheckedParRB == null || !Int32.TryParse(RCNumber.Text, out NumberofQ) || (CheckedDBRB.Content.ToString() == "Int-String" && CheckedParRB.Content.ToString() == "Key" && !Int32.TryParse(RCParameter.Text, out k)))
            {
                    MessageBoxResult alert = MessageBox.Show("Query is not well-formatted", "Inappropriate query", MessageBoxButton.OK);        //MessageBox appears on the screen
            }                                                                                                                                   // if query is not well-defined or if a parameter is missing
            else
            {
                StringBuilder Params = new StringBuilder();
                Params.Append(CheckedParRB.Content.ToString());
                if (CheckedParRB.Content.ToString() == "Metadata")
                    Params.Append(((ComboBoxItem)RCMetadataDB.SelectedValue).Content.ToString());
                NumberofQ = Int32.Parse(RCNumber.Text);
                string Parameter = RCParameter.Text;
                MessageXML.Element("MessageStream").Add(new XElement("QueryType", new XAttribute("DBType", CheckedDBRB.Content.ToString()), new XAttribute("NumberofQ", NumberofQ), RCQueryType.Text.ToString(), new XElement("Parameters", new XElement(Params.ToString() , Parameter))));

                string message = NumberofQ + ": " + RCQueryType.Text.ToString()+ " from " + CheckedDBRB.Content.ToString() + " where " + CheckedParRB.Content.ToString() + " =  " + Parameter;
                RCMessages.Items.Insert(0, message);
                StatusLabel.Content = "Message added to the message stream";    // There is one main XML document containing root only. Every time this button is clicked
            }                                                                   // root's children are created which contain query information
        }

        private void RCSendList_Click(object sender, RoutedEventArgs e)             // In ReadCLient tab, XML Message stream containing all queries is sent to its target address upn the click
        {                                                                           // of this button. Message ID is generated based on value of RCMessageID value declared at the start.
            MessageGenerator maker = new MessageGenerator();
            Message RCMessageStream = maker.MessageCreator(UtilityMethods.makeUrl(LocalAddress, LocalPort), UtilityMethods.makeUrl(RemoteAddress, RemotePort));
            RCMessageStream.MessageContent = MessageXML.ToString();
            RCMessageStream.MessageID = String.Concat("RC", RCMessageID.ToString());
            StatusLabel.Content = "Sending to " + RCMessageStream.ToURL;
            try
            {
                RCMessageStream.TimeSent = DateTime.Now;
                if (UISender.sendMessage(RCMessageStream))       // Cuurent date time is stored when a message is sent
                    StatusLabel.Content = "Message stream sent";
                else
                    StatusLabel.Content = "Failed to send";
            }
            catch (Exception EX)
            {
                StatusLabel.Content = EX.Message;
            }
            MessageXML.Save("MessageStream.xml");                          // XML Message is saved before it is sent across
            MessageXML.Element("MessageStream").Elements().Remove();        // Elements are removed as to make the same XML document for the query results received after processing. 
            RCMessages.Items.Clear();                                       // Those would be added in the same XML message to avoid losing any property required to measure the performance 
        }

        // Used to check the parameter selected on read  tab
        private void RCPar_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton Checked = sender as RadioButton;
            if ((bool)Checked.IsChecked)
                CheckedParRB = Checked;
        }

        //Used to know the database for which query is designed at ReadClient tab
        private void RCDB_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton Checked = sender as RadioButton;
            if ((bool)Checked.IsChecked)
                CheckedDBRB = Checked;
        }

        //Used to know the database for which query is designed at WriteClient tab
        private void WCDB_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton Checked = sender as RadioButton;
            if ((bool)Checked.IsChecked)
                CheckedDBRB1 = Checked;
        }

        // Used to check the parameter selected on write client
        private void WCPar_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton Checked = sender as RadioButton;
            if ((bool)Checked.IsChecked)
                CheckedParRB1 = Checked;

            if (Checked.Content.ToString() == "Metadata")
                WCMetadataDB.IsEnabled = true;
        }

        // In WriteCLient tab, XML Message stream containing all queries is sent to its target address upon the click of this button
        private void WCSendList_Click(object sender, RoutedEventArgs e)
        {
            MessageXML.Save("MessageStream.xml");
            MessageGenerator maker = new MessageGenerator();
            Message WCMessageStream = maker.MessageCreator(UtilityMethods.makeUrl(LocalAddress, LocalPort), UtilityMethods.makeUrl(RemoteAddress, RemotePort));
            WCMessageStream.MessageContent = MessageXML.ToString();
            WCMessageStream.MessageID = String.Concat("WC", WCMessageID.ToString());
            StatusLabel1.Content = "Sending to " + WCMessageStream.ToURL;
            try
            {
                WCMessageStream.TimeSent = DateTime.Now;
                if (UISender.sendMessage(WCMessageStream))  // The stream is sent across at this line.
                    StatusLabel1.Content = "Message stream sent";
                else
                    StatusLabel1.Content = "Failed to send";
            }
            catch (Exception EX)
            {
                StatusLabel1.Content = EX.Message;
            }
            MessageXML.Save("MessageStream.xml");
            MessageXML.Element("MessageStream").Elements().Remove();
            WCMessages.Items.Clear();
        }

        // It is associated with AddMessage of WriteCLient tab. An XML is created based upon the user input. XML is not sent until send button is clicked. This application keeps on 
        // adding the queries upon its click and sent all of them across when sent is clicked.
        private void WCAddMessage_Click(object sender, RoutedEventArgs e)
        {
            // For this code analyzer shows it to have a size of 297. Please comment if I can improve this code in terms of size. Thank you.
            int NumberofQ = 0, k;
            if (WCQueryType.SelectedItem == null || ((bool)WCMetadata.IsChecked && WCMetadataDB.SelectedItem == null) || CheckedDBRB1 == null || !Int32.TryParse(WCNumber.Text, out NumberofQ))
            {
                MessageBoxResult alert = MessageBox.Show("Query is not well-formatted", "Inappropriate query", MessageBoxButton.OK);
            }
            else
            {
                string message = "";
                NumberofQ = Int32.Parse(WCNumber.Text);
                if (WCQueryType.SelectedIndex == 0)
                {   MessageXML.Element("MessageStream").Add(new XElement("QueryType", new XAttribute("DBType", CheckedDBRB1.Content.ToString()), new XAttribute("NumberofQ", NumberofQ), WCQueryType.Text.ToString()));
                    message = NumberofQ + ": " + WCQueryType.Text.ToString() + " in " + CheckedDBRB1.Content.ToString();
                    WCMessages.Items.Insert(0, message);
                    StatusLabel1.Content = "Message added to the message stream";
                }
                
                if (WCQueryType.SelectedIndex == 1)
                {
                    if(WCKey.Text == null || WCKey.Text == "" || (CheckedDBRB1.Content.ToString() == "Int-String" && !Int32.TryParse(WCKey.Text, out k)))
                    {
                        MessageBoxResult alert = MessageBox.Show("Query is not well-formatted", "Inappropriate query", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageXML.Element("MessageStream").Add(new XElement("QueryType", new XAttribute("DBType", CheckedDBRB1.Content.ToString()), new XAttribute("NumberofQ", NumberofQ), WCQueryType.Text.ToString(), new XElement("Parameters", new XElement("Key", WCKey.Text))));
                        message = NumberofQ + ": " + WCQueryType.Text.ToString() + " in " + CheckedDBRB1.Content.ToString() + " where Key = " + WCKey.Text.ToString();
                        WCMessages.Items.Insert(0, message);
                        StatusLabel1.Content = "Message added to the message stream";
                    }
                }
                StringBuilder Params = new StringBuilder();
                string Parameter;
                if (WCQueryType.SelectedIndex == 2)
                {   if (WCKey.Text == null || WCKey.Text == "" || CheckedParRB1 == null || (CheckedDBRB1.Content.ToString() == "Int-String" && !Int32.TryParse(WCKey.Text, out k)))
                    {
                        MessageBoxResult alert = MessageBox.Show("Query is not well-formatted", "Inappropriate query", MessageBoxButton.OK);
                    }
                    else
                    {
                        Parameter = WCParameter.Text;
                        Params.Append(CheckedParRB1.Content.ToString());
                        if (CheckedParRB1.Content.ToString() == "Metadata")
                            Params.Append(((ComboBoxItem)WCMetadataDB.SelectedValue).Content.ToString());
                        MessageXML.Element("MessageStream").Add(new XElement("QueryType", new XAttribute("DBType", CheckedDBRB1.Content.ToString()), new XAttribute("NumberofQ", NumberofQ), WCQueryType.Text.ToString(), new XElement("Parameters", new XElement("Key", WCKey.Text)), new XElement(Params.ToString(), WCParameter.Text.ToString())));
                        message = NumberofQ + ": " + WCQueryType.Text.ToString() + " in " + CheckedDBRB1.Content.ToString() + " where Key = " + WCKey.Text.ToString();
                        WCMessages.Items.Insert(0, message);
                        StatusLabel1.Content = "Message added to the message stream";
                    }
                    WCResults.Items.Clear();
                }
            }
        }

        //It contains the code which enables or disables the UI controls based on the query selected in readclient tab
        private void RCQueryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((e.AddedItems[0] as ComboBoxItem).Content.ToString().Equals("Get data of a key") || (e.AddedItems[0] as ComboBoxItem).Content.ToString().Equals("Get children of a key"))
            {
                RCPattern.IsEnabled = false;
                RCMetadata.IsEnabled = false;
                RCKey.IsEnabled = true;
                RCKey.IsChecked = true;
                RCMetadataDB.IsEnabled = false;
                RCMetadataDB.SelectedItem = null;
            }

            if ((e.AddedItems[0] as ComboBoxItem).Content.ToString().Equals("Get all keys based on a metadata"))
            {
                RCPattern.IsEnabled = false;
                RCMetadata.IsEnabled = true;
                RCKey.IsEnabled = false;
                RCMetadata.IsChecked = true;
                RCMetadataDB.IsEnabled = true;
                RCMetadataDB.SelectedItem = null;
            }

            if ((e.AddedItems[0] as ComboBoxItem).Content.ToString().Equals("Get all keys based on time interval"))
            {
                RCPattern.IsEnabled = false;
                RCMetadata.IsEnabled = true;
                RCKey.IsEnabled = false;
                RCMetadata.IsChecked = true;
                RCMetadataDB.IsEnabled = true;
                RCMetadataDB.SelectedItem = RCMetadataDB.Items[2];
            }

            if ((e.AddedItems[0] as ComboBoxItem).Content.ToString().Equals("Get all keys matching a pattern"))
            {
                RCKey.IsEnabled = false;
                RCMetadata.IsEnabled = false;
                RCPattern.IsEnabled = true;
                RCPattern.IsChecked = true;
                RCMetadataDB.IsEnabled = false;
                RCMetadataDB.SelectedItem = null;
            }
        }
        
        //It contains the code which enables or disables the UI controls based on the query selected in Writeclient tab
        private void WCQueryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems[0] as ComboBoxItem).Content.ToString().Equals("Add pre-defined elements"))
            {
                WCKey.IsEnabled = false;
                WCMetadata.IsEnabled = false;
                WCMetadataDB.IsEnabled = false;
                WCValue.IsEnabled = false;
                WCParameter.IsEnabled = false;
            }

            if ((e.AddedItems[0] as ComboBoxItem).Content.ToString().Equals("Delete an element based on key"))
            {
                WCKey.IsEnabled = true;
                WCMetadata.IsEnabled = false;
                WCMetadataDB.IsEnabled = false;
                WCValue.IsEnabled = false;
                WCParameter.IsEnabled = false;
            }

            if ((e.AddedItems[0] as ComboBoxItem).Content.ToString().Equals("Modify an element based on key"))
            {
                WCMetadata.IsEnabled = true;
                WCValue.IsEnabled = true;
                WCParameter.IsEnabled = true;
                WCKey.IsEnabled = true;
            }
        }
        
        // It launches the instances of readers and writers based on input given by user. It uses Starter project to start the instances
        private void Launcher(object sender, RoutedEventArgs e)
        {
            try
            {
                int R = 0, W = 0;
                string ARG4 = "";
                if (CheckedLogging == null || CheckedLogging.Content.ToString() == "No")
                    ARG4 = "F";
                else
                    ARG4 = "T";
                if (((Readers.Text.Trim() != null || Readers.Text.Trim() != "") && (bool)int.TryParse(Readers.Text.Trim(), out R)) && ((Writers.Text != null || Writers.Text.Trim() != "") && (bool)int.TryParse(Writers.Text.Trim(), out W)))
                {
                    string[] args = new string[] { "ReadClient", R.ToString(), "WriteClient", W.ToString(), ARG4 };
                    Starter.StartClientsfromWPF(args);
                    Message NoOfClients = new Message();
                    NoOfClients.FromURL = UtilityMethods.makeUrl(LocalAddress, LocalPort); NoOfClients.ToURL = UtilityMethods.makeUrl(RemoteAddress, RemotePort);
                    NoOfClients.TimeSent = DateTime.Now;
                    XDocument ClientInfo = new XDocument();
                    int T = R + W;
                    ClientInfo.Add(new XElement("NoOfClient", T));
                    NoOfClients.MessageContent = ClientInfo.ToString();
                    UISender.sendMessage(NoOfClients);
                }
                else
                    Status.Items.Insert(0, "Couldn't start the clients. Enter valid inputs.\nEnter 0 against the client you don't wish to start");
            }
            catch(Exception)
            {
                MessageBoxResult alert = MessageBox.Show("Couldn't start the clients. Enter valid inputs");

            }
        }

        // It is the event associated with connect button under LaunchTesters tab. It connects to the port mentioned in textbox. WPF then considers that port as remote port.
        private void Connecter(object sender, RoutedEventArgs e)
        {
            RemotePort = rPort.Text;
            if (firstConnect)
            {
                firstConnect = false;                                   // Connect must be hit first to get going as it establishes the connection with port mentioned
                if (UIReceiver != null)
                    UIReceiver.shutDown();
                setupChannel();
            }
            lStat.Text = "Connected";
            Status.Items.Insert(0, "Connected to " + rPort.Text);
            Send.IsEnabled = true;
        }

        // It is the event associated with Send button under LaunchTesters tab. It sends messages to the port mentioned in textbox upon the connection. WPF then considers that port as remote port.
        private void Sender(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!RemotePort.Equals(rPort.Text))
                    RemotePort = rPort.Text;
                MessageGenerator maker = new MessageGenerator();
                Message msg = maker.MessageCreator(UtilityMethods.makeUrl(LocalAddress, LocalPort), UtilityMethods.makeUrl(RemoteAddress, RemotePort));
                lStat.Text = "Sending to" + msg.ToURL;
                UISender.LocalURL = msg.FromURL;
                UISender.RemoteURL = msg.ToURL;
                msg.TimeSent = DateTime.Now;
                TimeSent = DateTime.Now;
                lStat.Text = "Attempting to connect";
                if (UISender.sendMessage(msg))
                    lStat.Text = "Connected";
                else
                    lStat.Text = "Connection failed";
                postSndMsg(msg.MessageContent);
            }
            catch (Exception ex)
            {
                lStat.Text = ex.Message;
            }
        }

        RadioButton CheckedLogging;
        private void CheckLogging(object sender, RoutedEventArgs e)
        {
            RadioButton Checked = sender as RadioButton;
            if ((bool)Checked.IsChecked)
                CheckedLogging = Checked;
        }
    }
}
         