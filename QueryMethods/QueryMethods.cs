////////////////////////////////////////////////////////////////////////////////////////
// QuertyMethods.cs - It defines all the query methods required by TestExecutive      //
//                     DatabaseDictionary and Elements.                               //
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
 * It contains methods to get a set of keys relating to a particular valuE:\SU Documents\Ist Semester\SMA\My Projects\Project2Demonstration\QueryMethods\QueryMethods.cse or a data in a value, in order to satisy requirements, particularly #7.
 *
 * Maintenance:
 * ------------
 * Required Files: DatabaseDictionary.cs, Element.cs
 *
 * Build Process:  devenv RemoteNoSQL.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 2.0 : Added new methods to support random generated element queries, Better code for editing elements
 * ver 1.0 : 7 Oct 2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteNoSQL
{
    public static class QueryMethods
    {
        // Code Analyzer shows that its complexity is 12 but it doesn't look like to me. Please comment if there is any scope of reducing the complexity
        public static string AddElementIS(this DatabaseDictionary<int, Element<int, string>> Temp) //Method to add a random generated elment in a DB 
        {
            try
            {
                int Key = Temp.IKeygenerator();
                Random Random = new Random();
                List<string> Names = new List<string> {"Megadeath" , "Pink Floyd", "Porcupine Tree", "Bahramji", "RHCP" };
                List<List<int>> Relation = new List<List<int>> { new List<int> { 1 } , new List<int> { 2, 3 }, new List<int> { 3,1 }, new List<int> { 4 }, new List<int> { 2,5 } };
                List<string> Data = new List<string> { "Opeth", "Animals", "Sunset", "Chillout", "New Era" };
                if (Key != 0)
                {
                    Element<int, string> TempE = new Element<int, string>();
                    TempE.NAME = Names[Random.Next(0, Names.Count)];
                    TempE.DESCRIPTION = "Rockband";
                    TempE.RELATIONS = Relation[Random.Next(0, Relation.Count)];
                    TempE.DATA = Data[Random.Next(0,Data.Count)];
                    Temp.AddPair(Key, TempE);
                    TempE.TIMESTAMP = DateTime.Now;
                    Console.WriteLine("  New element added");
                    return "New element added";
                }
                Console.WriteLine("  New element was not added");
                return "Element was not added";
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
        }

        // Code Analyzer shows that its complexity is 17 but it doesn't look like to me. Please comment if there is any scope of reducing the complexity
        public static string AddElementSL(this DatabaseDictionary<string, Element<string, List<string>>> Temp) //Method to add a random generated elment in a DB
        {
            try
            {
                string Key = Temp.SKeygenerator();
                Random Random = new Random();
                List<string> Names = new List<string> { "Megadeath", "Pink Floyd", "Porcupine Tree", "Bahramji", "RHCP" };
                List<List<string>> Relation = new List<List<string>> { new List<string> { "Key1" }, new List<string> { "Key2", "Key3" }, new List<string> { "Key3", "Key1" }, new List<string> { "Key4" }, new List<string> { "Key4", "Key2" } };
                List<List<string>> Data = new List<List<string>> { new List<string> { "Opeth" } , new List<string> { "Animals" }, new List<string> { "Sunset" }, new List<string> { "Chillout" }, new List<string> { "New Era" } };
                if (Key != "" || Key!=null)
                {
                    Element<string, List<string>> TempE = new Element<string, List<string>>();
                    TempE.NAME = Names[Random.Next(0, Names.Count)];
                    TempE.DESCRIPTION = "Rockband";
                    TempE.TIMESTAMP = DateTime.Now;
                    TempE.RELATIONS = Relation[Random.Next(0, Relation.Count)];
                    TempE.DATA = Data[Random.Next(0, Data.Count)];
                    Temp.AddPair(Key, TempE);
                    Console.WriteLine("  New element added");
                    return "New element added";
                }
                Console.WriteLine("  New element was not added");
                return "Element was not added";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
        }

        public static string EditElementIS(this DatabaseDictionary<int, Element<int, string>> Temp, List<string> ParameterSelected, List<string> ParameterValue)   //Method to edit any information in an elment of DB
        {
            try
            {
                int Key = int.Parse(ParameterValue.First());
                string Param;
                if (ParameterSelected.Last().StartsWith("Metadata"))
                    Param = ParameterSelected.Last().Substring(8);
                else
                    Param = ParameterSelected.Last();
                if (Key != 0)
                {
                    Element<int, string> TempE;
                    if (Temp.RetrieveValue(Key, out TempE))
                    {
                        if (Param == "Name")
                            TempE.NAME = ParameterValue[1];
                        if (Param == "Description")
                            TempE.DESCRIPTION = ParameterValue[1];
                        if (Param == "Children")
                        {
                            List<int> relations = new List<int>();
                            relations.Add(int.Parse(ParameterValue[1]));
                            TempE.RELATIONS = relations;
                        }
                        if (Param == "Data")
                            TempE.DATA = ParameterValue[1];
                    }
                    Console.WriteLine("  Modified the element");
                    return "Modified the element";
                }
                else
                {
                    Console.WriteLine("  Couldn't modify the element");
                    return "Couldn't modify the element. Key not found";
                }
            }
            catch (Exception)
            {
                Console.WriteLine("  Couldn't modify the element");
                return "Couldn't process the query";
            }
        }

        public static string EditElementSL(this DatabaseDictionary<string, Element<string, List<string>>> Temp, List<string> ParameterSelected, List<string> ParameterValue)  //Method to edit any information in an elment of DB
        {
            try
            {
                string Param;
                if (ParameterSelected.Last().StartsWith("Metadata"))
                    Param = ParameterSelected.Last().Substring(8);
                else
                    Param = ParameterSelected.Last();

                string Key = ParameterValue.First();
                if (Key != null)
                {
                    Element<string, List<string>> TempE;
                    if (Temp.RetrieveValue(Key, out TempE))
                    {
                        if (Param == "Name")
                            TempE.NAME = ParameterValue[1];
                        if (Param == "Description")
                            TempE.DESCRIPTION = ParameterValue[1];
                        if (Param == "Children")
                        {
                            List<string> relations = new List<string>();
                            relations.Add(ParameterValue[1]);
                            TempE.RELATIONS = relations;
                        }
                        if (Param == "Data")
                        {
                            List<string> values = new List<string>();
                            values.Add(ParameterValue[1]);
                            TempE.DATA = values;
                        }
                    }
                    return "Modified the element";
                }
                else
                    return "Couldn't modify the element. Key not found";
            }
            catch (Exception)
            {

                return "Couldn't process the query";
            }
        }

        public static List<string> FindKeysbyMetadata(this DatabaseDictionary<string, Element<string, List<string>>> Temp, string Parameter, string V)  //Method to retrieve keys using a metadata
        {
            List<string> LK = new List<string>();
            foreach (var k in Temp.Keys())
            {
                if (Parameter == "Name")
                {
                    if (Temp.RetrieveValue(k).NAME.Equals(V))
                        LK.Add(k);
                }

                if (Parameter == "Description")
                {
                    if (Temp.RetrieveValue(k).DESCRIPTION.Equals(V))
                        LK.Add(k);
                }

                if (Parameter == "Timestamp")
                {
                    try
                    {
                        DateTime V1 = Convert.ToDateTime(V);
                        if ((Temp.RetrieveValue(k).TIMESTAMP.CompareTo(V1) >= 0) && (Temp.RetrieveValue(k).TIMESTAMP.CompareTo(DateTime.Now) <= 0))
                            LK.Add(k);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in converting datetime. Enter correct format" + e.Message);
                    }
                }

                if (Parameter == "Children")
                {
                    if (Temp.RetrieveValue(k).RELATIONS.Contains(V))
                        LK.Add(k);
                }
            }
            return LK;
        }

        public static List<int> FindKeysbyMetadata(this DatabaseDictionary<int, Element<int, string>> Temp, string Parameter, string V)  ////Method to retrieve keys using a metadata parameter
        {
            List<int> LK = new List<int>();

            foreach (var k in Temp.Keys())
            {
                if (Parameter == "Name")
                {
                    if (Temp.RetrieveValue(k).NAME.Equals(V))
                        LK.Add(k);
                }

                if (Parameter == "Description")
                {
                    if (Temp.RetrieveValue(k).DESCRIPTION.Equals(V))
                        LK.Add(k);
                }

                if (Parameter == "Timestamp")
                {
                    try
                    {
                        DateTime V1 = Convert.ToDateTime(V);
                        if ((Temp.RetrieveValue(k).TIMESTAMP.CompareTo(V1) >= 0) && (Temp.RetrieveValue(k).TIMESTAMP.CompareTo(DateTime.Now) <= 0))
                            LK.Add(k);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in converting datetime. Enter correct format" + e.Message);
                    }
                }

                if (Parameter == "Children")
                {
                    if (Temp.RetrieveValue(k).RELATIONS.Contains(int.Parse(V)))
                        LK.Add(k);
                }
            }
            return LK;
        }

        //Method to get a set of keys containing the string as their data in values or among other strings in the data of respective values
        public static List<string> FindKeysbyData(this DatabaseDictionary<string, Element<string, List<string>>> Temp, string V) //Method to retrieve keys using data as a parameter
        {
            List<string> LK1 = new List<string>();
            List<string> result = new List<string>();
            List<List<string>> LK = new List<List<string>>();

            foreach (string k in Temp.Keys())
            {
                LK.Add(Temp.RetrieveValue(k).DATA);
                foreach (List<string> TempL in LK)
                {
                    if (TempL.Contains(V))
                        result.Add(k);
                }
            }
            return result;
        }

        public static List<int> FindKeysbyData(this DatabaseDictionary<int, Element<int, string>> Temp, string V)  ////Method to retrieve keys using data parameter
        {
            List<int> LK = new List<int>();
            foreach (var k in Temp.Keys())
            {
                if (Temp.RetrieveValue(k).DATA.Contains(V))
                    LK.Add(k);
            }
            return LK;
        }

        public static List<string> FindMatchingKeys(this DatabaseDictionary<string, Element<string, List<string>>> Temp, string K)  //Method to retrieve keys using a pattern
        {
            List<string> result = new List<string>();
            foreach(var key in Temp.Keys().ToList())
            {
                if (key.Contains(K))
                    result.Add(key);
            }
            return result;
        }

        public static List<int> FindMatchingKeys(this DatabaseDictionary<int, Element<int, string>> Temp, string K) //Method to retrieve keys using a pattern
        {
            List<int> result = new List<int>();
            foreach (var key in Temp.Keys().ToList())
            {
                if (key.ToString().Contains(K))
                    result.Add(key);
            }
            return result;
        }
    }

#if(TEST_QUERIES)
    class TestQueryMethods
    {
        static void Main(string[] args)
        {
            DatabaseDictionary<int, Element<int, string>> IntKeyDB = new DatabaseDictionary<int, Element<int, string>>();
            Element<int, string> IKElement1 = new Element<int, string>();
            IKElement1.NAME = "The Beatles"; IKElement1.DESCRIPTION = "Rockband"; IKElement1.TIMESTAMP = DateTime.Now; IKElement1.RELATIONS = new List<int> { 5, 2 }; IKElement1.DATA = "Revolver";
            Element<int, string> IKElement2 = new Element<int, string>();
            IKElement2.NAME = "Pink Floyd"; IKElement2.DESCRIPTION = "Rockband"; IKElement2.TIMESTAMP = DateTime.Now; IKElement2.RELATIONS = new List<int> { 1, 2 }; IKElement2.DATA = "Wish you were here";
            IntKeyDB.AddPair(1, IKElement2);
            IntKeyDB.AddPair(2, IKElement1);
            Console.Write("Following key or keys contain THE BEATLES as the NAME in their values (Elements)\nKey: ");
            Console.Write("\n\nFollowing key or keys contain WISH YOU WERE HERE as the DATA in their values (Elements)\nKey: ");
            foreach (int a in IntKeyDB.FindKeysbyData("Wish you were here"))
                Console.Write("{0} ", a.ToString());
            Console.Write("\n\nFollowing key or keys have been updated in the database today till now\nKey: ");
            Console.WriteLine();
        }
    }
#endif
}
