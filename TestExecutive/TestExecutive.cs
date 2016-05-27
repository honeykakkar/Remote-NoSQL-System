////////////////////////////////////////////////////////////////////////////////////////
// TestExective.cs - It contains all the methods required to demonstrate that all     //
//                   requirements have been met by implementing generic               //
//                   DatabaseDictionary and Elements.                                 //
// Application: NoSQL database implementation, Project 2, CSE681-SMA         ///////////
// Language:    C#, Framework 4.5.2, Visual Studio 2015 (Community Edt.)     //
// Platform:    Dell Inspiron 13 7000, Core-i7, Windows 10                   //
// Author:      Honey Kakkar, Computer Engineering, SU                       //
//              hkakkar@syr.edu                                              //
///////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 *
 * It contains methods to required to demonstrate that all requirements of project #2 have been met
 * Maintenance:
 * ------------
 * Required Files: DatabaseDictionary.cs, Element.cs, QueryMethods.cs, ExtensionMethod.cs, XMLManager.cs, ImmutableDictionary.cs
 *
 * Build Process:  devenv Project2Demonstration.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 7 Oct 2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.IO;

namespace RemoteNoSQL
{

    public class TestExecutive
    {
        //Creating instances of two different types of dictionaries, their elements
        private static DatabaseDictionary<int, Element<int, string>> IntKeyDB = new DatabaseDictionary<int, Element<int, string>>();
        private static DatabaseDictionary<string, Element<string, List<string>>> StringKeyDB = new DatabaseDictionary<string, Element<string, List<string>>>();
        private Element<int, string> IKElement1 = new Element<int, string>();
        private Element<string, List<String>> SKElement1 = new Element<string, List<string>>();
        static int DBKey = 0;

        // Method to display XML file related to last persisted database
        public void DisplayXMLfile()
        {
            XmlDocument XDoc = new XmlDocument();
            try
            {
                string Xfile = @"\PersistedIntDB.xml";
                StringBuilder Path = new StringBuilder();
                string directory = Directory.GetCurrentDirectory();
                Path.Append(directory);
                Path.Append(Xfile);
                XDoc.Load(Path.ToString());
                XDoc.Save(Console.Out);
                Console.WriteLine();
            }

            catch (XmlException E)
            {
                Console.WriteLine("\n  " + E.Message + "\n");
            }
        }

        void DemoR2()
        {
            "\n  Demonstrating Requirement #2".Wrap();
            "  Persisting previously created databases from XML files".Wrap();
            "  Displaying contents of XML file of persisted database of type Int-String".Wrap();
            DisplayXMLfile();
            "  Displaying current state of the database".Wrap();
            IntKeyDB = XMLtoIntDB();
            Console.WriteLine(IntKeyDB.DisplayKVP<int, Element<int, string>, string>());
        }

        void DemoR3()
        {
            "\n  Demonstrating Requirement #3".Wrap();
            "  Using CommunicationManager class and the interface to use WCF services".Wrap();
            Console.WriteLine("  Messages are being sent in-between all clients and server in an XML format. \n  All communication channels use WCF services by exploiting Basic Http binding and contracts to send and receive messages.\n");
        }
        
        void DemoR4()
        {
            "\n  Demonstrating Requirement #4".Wrap();
            Console.WriteLine("  All dependent and necessary packages have been imported from Project #2 to support all previously designed database operations\n  which includes adding, deleting, modifing an element and retrieving any information from any database");
            Console.WriteLine("  All DB operations can be performed by respective clients with the help of UserInterface (WPF) in their respective tabs.\n  XML message (message stream containing user-defined queries) is sent to clients on their startups by WPF application");
        }

        void DemoR5and7()
        {
            "\n  Demonstrating Requirement #5 and #7".Wrap();
            Console.WriteLine("  Clients are initiated upon the startup of project. Number is determined by user input. \n  Each client uses a console window in order to display the processing of queries");
            Console.WriteLine("  All types of clients have their respective user interface in order to allow customised queries for databases\n  XML Message stream can be found in the respective locations of clients in project directory");
        }


        void DemoR6()
        {
            "\n  Demonstrating requirement #6".Wrap();
            Console.WriteLine("  In the argument array of string of clients, last argument decided whether or not to log the messages to console as they are sent.\n  T means true which would log messages to console.By default, it is set to F.");
        }

        void DemoR8()
        {
            "\n  Demonstrating requirement #8".Wrap();
            Console.WriteLine("  Although this requirement is met at a fine-grain level when processing message streams via console window\n  but results can be clearly seen under Performance Assessment tab of WPF which stores this result of each message stream sent when it is sent using WPF client");
        }

        //Method converts last persisted database XML file to a database
        public DatabaseDictionary<int, Element<int, string>> XMLtoIntDB()
        {
            XDocument IntDBXML = XDocument.Load("PersistedIntDB.xml");
            var Query = from KV in IntDBXML.Elements("KeyValuePairs").Elements("KeyValuePair").Descendants() select KV;
            foreach (var KVP in Query)
            {
                if (KVP.Name.ToString() == "Key")
                    DBKey = int.Parse(KVP.FirstAttribute.Value);
                if (KVP.Name.ToString() == "Value")
                {
                    Element<int, string> DBElement = new Element<int, string>();
                    try
                    {
                        foreach (var Desc in KVP.Descendants())
                        {
                            if (Desc.Name.ToString() == "Name")
                                DBElement.NAME = Desc.Value.ToString();
                            if (Desc.Name.ToString() == "Description")
                                DBElement.DESCRIPTION = Desc.Value.ToString();
                            if (Desc.Name.ToString() == "Timestamp")
                            {
                                DateTime timestamp = DateTime.Parse(Desc.Value);
                                DBElement.TIMESTAMP = timestamp;
                            }
                            if (Desc.Name.ToString() == "Relations")
                            {
                                if (Desc.Descendants() == null)
                                    DBElement.RELATIONS = null;
                                List<int> Relations = new List<int>();
                                foreach (var Relation in Desc.Descendants())
                                    Relations.Add(int.Parse(Relation.Value));
                                DBElement.RELATIONS = Relations;
                            }
                            if (Desc.Name.ToString() == "Data")
                                DBElement.DATA = Desc.Value;
                        }
                    }
                    catch (Exception M)
                    {
                        Console.WriteLine(M.Message);
                    }

                    IntKeyDB.AddPair(DBKey, DBElement);
                }
            }
            return IntKeyDB;
        }

        // Method to retrieve database from XML file 
        public void XMLProcessing()
        {
            "  Retrieving key value pairs from the persisted database XML file and adding them back to <int, string> database".Wrap();
            IntKeyDB = XMLtoIntDB();
            "  Adding elements back to the dictionary from XML file".Wrap();
            "  Displaying current state of the databases".Wrap();     
        }

        //String data base is created initially in case user wants to perform read actions first
        public void CreateStringDB()
        {
            Element<string, List<string>> EL1 = new Element<string, List<string>>();
            List<string> Data = new List<string>(); List<string> relations = new List<string>();
            Data.Add("Wish you were here"); relations.Add("Key3");
            EL1.NAME = "Pink Floyd"; EL1.DATA = Data; EL1.DESCRIPTION = "Rockband"; EL1.RELATIONS = relations; EL1.TIMESTAMP = DateTime.Now;
            StringKeyDB.AddPair("Key1", EL1);
            Data.Add("Beautiful day");
            EL1.NAME = "U2"; EL1.DATA = Data; EL1.DESCRIPTION = "Rockband"; EL1.RELATIONS = relations; EL1.TIMESTAMP = DateTime.Now;
            StringKeyDB.AddPair("Key2", EL1);
        }

        //Shows current state of databases
        public void DisplayDBs()
        {
            "\n  Displaying current state of database".Wrap();
            Console.WriteLine(IntKeyDB.DisplayKVP<int, Element<int,string>, string>());
            Console.WriteLine(StringKeyDB.DisplayKVP<string, Element<string, List<string>>, List<string>, string>());
        }

        //It processes all queries related to retrieval of keys based on a data of key
        public static void GetValue(ref string DBType, ref List<string> ParameterValue, ref List<string> Results)
        {
            if (DBType == "Int-String")
            {
                int Key = int.Parse(ParameterValue.First());
                String.Format("\n  Retrieving data of Key: " + Key + " in <int, string> database").Wrap();
                string R = IntKeyDB.RetrieveValue(Key).DATA.ToString();
                if (R != null || R != "")
                    Results.Add(R);
                else
                    Results.Add("No value found");
            }

            if (DBType == "String-List(String)")
            {
                String.Format("\n  Retrieving data of Key: " + ParameterValue.First() + " in <int, string> database").Wrap();
                List<string> R = StringKeyDB.RetrieveValue(ParameterValue.First()).DATA;
                if ( R == null || R.Count == 0)
                    Results.Add("No value found");
                else
                {
                    foreach (var item in R)
                        Results.Add(item.ToString());
                }
            }
        }

        //It processes all queries related to retrieval of keys where search is based on any element in children of keys
        public static void GetChildren(ref string DBType, ref List<string> ParameterValue, ref List<string> Results)
        {
            if (DBType == "Int-String")
            {
                int Key1 = int.Parse(ParameterValue.First());
                String.Format("\n  Retrieving children of Key: " + Key1 + " in <int, string> database").Wrap();
                List<int> R = IntKeyDB.RetrieveValue(Key1).RELATIONS;
                if (R == null || R.Count == 0)
                    Results.Add("No child found");
                else
                {
                    foreach (var item in R)
                        Results.Add(item.ToString());
                }
            }

            if (DBType == "String-List(String)")
            {
                String.Format("\n  Retrieving children of Key: " + ParameterValue.First() + " in <int, string> database").Wrap();
                List<string> R = StringKeyDB.RetrieveValue(ParameterValue.First()).RELATIONS;
                if (R == null || R.Count == 0)
                    Results.Add("No child found");
                else
                {
                    foreach (var item in R)
                        Results.Add(item.ToString());
                }
            }
        }

        //It processes all queries related to retrieval of keys based on a certain metadata value
        public static void MetadataKeys(ref string DBType, ref List<string> ParameterSelected, ref List<string> ParameterValue, ref List<string> Results)
        {
            if (DBType == "Int-String")
            {
                string Param = ParameterSelected.First().Substring(8);
                String.Format("\n  Retrieving all keys in <int, string> database where " + Param + " = " + ParameterValue.First().ToString()).Wrap();
                List<int> R = IntKeyDB.FindKeysbyMetadata(Param, ParameterValue.First().ToString());
                if (R == null || R.Count == 0)
                    Results.Add("No such key found");
                else
                {
                    foreach (var item in R)
                        Results.Add(item.ToString());
                }
            }

            if (DBType == "String-List(String)")
            {
                string Param = ParameterSelected.First().Substring(8);
                String.Format("\n  Retrieving all keys in <string, List<string>> database where " + Param + " = " + ParameterValue.First().ToString()).Wrap();
                List<string> R = StringKeyDB.FindKeysbyMetadata(Param, ParameterValue.First().ToString());
                if (R == null || R.Count == 0)
                    Results.Add("No such key found");
                else
                {
                    foreach (var item in R)
                        Results.Add(item.ToString());
                }
            }
        }

        //It processes all queries related to retrieval of keys based on a certain pattern
        public static void PatternKeys(ref string DBType, ref List<string> ParameterValue, ref List<string> Results)
        {
            if (DBType == "Int-String")
            {
                String.Format("\n  Retrieving all keys in <int, string> database matching the pattern = " + ParameterValue.First().ToString()).Wrap();
                List<int> R = IntKeyDB.FindMatchingKeys(ParameterValue.First().ToString());
                if (R == null || R.Count == 0)
                    Results.Add("No such key found");
                else
                {
                    foreach (var item in R)
                        Results.Add(item.ToString());
                }
            }
            if (DBType == "String-List(String)")
            {
                String.Format("\n  Retrieving all keys in <string, List<string>> database matching the pattern = " + ParameterValue.First().ToString()).Wrap();
                List<string> R = StringKeyDB.FindMatchingKeys(ParameterValue.First().ToString());
                if (R == null || R.Count == 0)
                    Results.Add("No such key found");
                else
                {
                    foreach (var item in R)
                        Results.Add(item.ToString());
                }
            }
        }

        //It processes all queries related to addition of new elements in any database
        public static void AddElements(ref string DBType, ref List<string> Results)
        {
            if (DBType == "Int-String")
            {
                String.Format("\n  Adding element in <int, string> database").Wrap();
                Results.Add(IntKeyDB.AddElementIS().ToString());
            }

            if (DBType == "String-List(String)")
            {
                String.Format("\n  Adding element in <string, List<string>> database").Wrap();
                Results.Add(StringKeyDB.AddElementSL().ToString());
            }
        }
        
        //It processes all queries related to deletion of elements in any database
        public static void DeleteKey(ref string DBType, ref string Key, ref List<string> Results)
        {
            if (DBType == "Int-String")
            {
                int key = int.Parse(Key);
                String.Format("\n  Deleting element in <int, string> database where key: " + Key ).Wrap();
                Results.Add(IntKeyDB.RemoveKVPair(key).ToString());
            }

            if (DBType == "String-List(String)")
            {
                String.Format("\n  Deleting element in <string, List<string>> database where key: " + Key ).Wrap();
                Results.Add(StringKeyDB.RemoveKVPair(Key).ToString());
            }
        }

        //It processes all queries related to modification of elements in any database
        public static void ModifyElement(ref string DBType, ref List<string> ParameterSelected, ref List<string> ParameterValue, ref List<string> Results)
        {
            if (DBType == "Int-String")
            {
                String.Format("\n  Editing element in <int, string> database where key: " + ParameterValue.First().ToString()).Wrap();
                Results.Add(IntKeyDB.EditElementIS(ParameterSelected, ParameterValue).ToString());
            }

            if (DBType == "String-List(String)")
            {
                String.Format("\n  Editing element in <string, List<string>> database where key: " + ParameterValue.First().ToString()).Wrap();
                Results.Add(StringKeyDB.EditElementSL(ParameterSelected, ParameterValue).ToString());
            }
        }

        // To persist int-string database
        public static void IntKeyDBtoXML(ref DatabaseDictionary<int, Element<int, string>> TempDB, ref List<string> Results)
        {
            "Persisting the database".Wrap();
            XDocument IntDBXML = new XDocument();
            IntDBXML.Declaration = new XDeclaration("1.0", "UTF-8", "yes");
            XComment comment = new XComment("Persisting int-string database");
            IntDBXML.Add(comment);
            XElement Root = new XElement("KeyValuePairs");
            IntDBXML.Add(Root);
            foreach (int K in TempDB.Keys())
            {
                XElement KVP = new XElement("KeyValuePair");
                Root.Add(KVP);
                Element<int, string> Val = TempDB.RetrieveValue(K);
                XElement Key = new XElement("Key", new XAttribute("IntKey", K.ToString()));
                KVP.Add(Key);
                XElement Value = new XElement("Value", new XAttribute("DatabaseElement", K.ToString()));
                KVP.Add(Value);
                XElement Name = new XElement("Name", Val.NAME);
                Value.Add(Name);
                XElement Description = new XElement("Description", Val.DESCRIPTION.ToString());
                Value.Add(Description);
                XElement Timestamp = new XElement("Timestamp", Val.TIMESTAMP.ToString());
                Value.Add(Timestamp);
                XElement Relations = new XElement("Relations");
                if (Val.RELATIONS != null)
                {
                    foreach (int R in Val.RELATIONS)
                    {
                        XElement Relation = new XElement("Relation", R.ToString());
                        Relations.Add(Relation);
                    }
                    Value.Add(Relations);
                }
                XElement Data = new XElement("Data", Val.DATA);
                Value.Add(Data);
            }
            "Persisting the contents of the dictionary into an XML file".Wrap();
            IntDBXML.Save("PersistedDB.xml");
            Results.Add("Database has been persisted");
        }

        // This function is entry point for processing the queries received at server end. 
        //It further delegates the work to above defined functions in order to decrease cyclomatic complexity
        public static List<string> QueryProcessing(string DBType, string QueryType, List<string> ParameterSelected, List<string> ParameterValue)
        {
            List<string> Results = new List<string>(); ;
            try
            { if (QueryType != null && ParameterValue != null)
                {
                    switch (QueryType)
                    {
                        case "Get data of a key":
                            GetValue(ref DBType, ref ParameterValue, ref Results);
                            break;
                        case "Get children of a key":
                            GetChildren(ref DBType, ref ParameterValue, ref Results);
                            break;
                        case "Get all keys based on a metadata":
                            MetadataKeys(ref DBType, ref ParameterSelected, ref ParameterValue, ref Results);
                            break;
                        case "Get all keys based on time interval":
                            MetadataKeys(ref DBType, ref ParameterSelected, ref ParameterValue, ref Results);
                            break;
                        case "Get all keys matching a pattern":
                            PatternKeys(ref DBType, ref ParameterValue, ref Results);
                            break;
                        case "Add pre-defined elements":
                            AddElements(ref DBType, ref Results);
                            break;
                        case "Delete an element based on key":
                            string Key = ParameterValue.First();
                            DeleteKey(ref DBType, ref Key, ref Results);
                            break;
                        case "Modify an element based on key":
                            ModifyElement(ref DBType, ref ParameterSelected, ref ParameterValue, ref Results);
                            break;
                        case "Persist database":
                            IntKeyDBtoXML(ref IntKeyDB, ref Results);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("  Error in processing the query");
                    Results.Add("Error in processing the query");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("  Error in processing the query. Query not well formatted\n{0}", e.Message);
            }
            return Results;
        }

        void DemoR10()
        {
            
            "\n  Demonstrating requirement #10 through Test Executive".Wrap();
            Console.WriteLine("\n  If readers and writers have not been already asked to start automatically, user can manually start them");
            Console.WriteLine("\n  User can inititate any number of readers and writers either through Test Executive, or command-line, or through WPF");
            string[] args = new string[5];
            args[0] = "ReadClient";
            Console.Write("\n  Enter number of readers to be initiated : ");
            args[1] = Console.ReadLine().ToString();
            Console.Write("\n  Enter number of writers to be initiated : ");
            args[2] = "WriteClient";
            args[3] = Console.ReadLine().ToString();
            Console.Write("\n  Enter T if you want to log messages to console, else enter F: ");
            args[4] = Console.ReadLine();
            Starter.StartClients(args);
        }

        public DatabaseDictionary<int, Element<int, string>> XMLtoDBAfterProcessing()
        {
            string CurrentLocation = Environment.CurrentDirectory;
            string SolutionPath = CurrentLocation.Substring(0, CurrentLocation.LastIndexOf("NoSQL Implementation") + 20);
            XDocument IntDBXML = XDocument.Load(SolutionPath + "\\PersistedDB.xml");
            var Query = from KV in IntDBXML.Elements("KeyValuePairs").Elements("KeyValuePair").Descendants() select KV;
            foreach (var KVP in Query)
            {
                if (KVP.Name.ToString() == "Key")
                    DBKey = int.Parse(KVP.FirstAttribute.Value);
                if (KVP.Name.ToString() == "Value")
                {
                    Element<int, string> DBElement = new Element<int, string>();
                    try
                    {
                        foreach (var Desc in KVP.Descendants())
                        {
                            if (Desc.Name.ToString() == "Name")
                                DBElement.NAME = Desc.Value.ToString();
                            if (Desc.Name.ToString() == "Description")
                                DBElement.DESCRIPTION = Desc.Value.ToString();
                            if (Desc.Name.ToString() == "Timestamp")
                            {
                                DateTime timestamp = DateTime.Parse(Desc.Value);
                                DBElement.TIMESTAMP = timestamp;
                            }
                            if (Desc.Name.ToString() == "Relations")
                            {
                                if (Desc.Descendants() == null)
                                    DBElement.RELATIONS = null;
                                List<int> Relations = new List<int>();
                                foreach (var Relation in Desc.Descendants())
                                    Relations.Add(int.Parse(Relation.Value));
                                DBElement.RELATIONS = Relations;
                            }
                            if (Desc.Name.ToString() == "Data")
                                DBElement.DATA = Desc.Value;
                        }
                    }
                    catch (Exception M)
                    {
                        Console.WriteLine(M.Message);
                    }

                    IntKeyDB.AddPair(DBKey, DBElement);
                }
            }
            return IntKeyDB;
        }

        public void DisplayDBafterProcessing()
        {
            "\n\n  Database is persisted at the end of every read client automatically.".Wrap();
            "\n  Although order and number of queries to persist database can be changed by editing XML file of MessageStream in the respective client folders".Wrap();
            "\n  Once all processing is done, XML of persisted database is created in the parent folder of project titled as PersistedDB.xml".Wrap();
            "\n  In order to demonstrate the requirement to persist and reload database from XML, this project persists the latest state of database and reloads it".Wrap();
            "\n  Press any key to retrieve the last state of processed database".Wrap();
            Console.ReadKey();
            DatabaseDictionary<int, Element<int, string>> AfterProcessing = new DatabaseDictionary<int, Element<int, string>>();
            AfterProcessing = XMLtoDBAfterProcessing();
            Console.WriteLine(AfterProcessing.DisplayKVP<int, Element<int, string>, string>());
        }

        static void Main(string[] args)
        {
            Console.Title = "Test Executive";
            "  Project 2: Implementation of Remote NoSQL database".Wrap();
            TestExecutive T = new TestExecutive();
            T.DemoR10();
            T.DemoR2();
            T.DemoR3();
            T.DemoR4();
            T.DemoR5and7();
            T.DemoR6();
            T.DemoR8();
            T.DisplayDBafterProcessing();
            Console.ReadKey();
        }
    }
}
