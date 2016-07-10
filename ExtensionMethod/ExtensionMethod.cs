////////////////////////////////////////////////////////////////////////////////////////
// ExtensionMethod.cs - It defines all the extension methods required by TestExecutive //
//                     DatabaseDictionary and Elements.                               //
// Application: NoSQL database implementation, Project 2, CSE681-SMA         ///////////
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
 * It holds good amount of functionality as many actions like displaying
 * elements and databases to console depend on it.
 *
 * Maintenance:
 * ------------
 * Required Files: DatabaseDictionary.cs, Element.cs
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
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


namespace RemoteNoSQL
{
    public static class DatabaseExtensionMethods
    {
        public static string DisplayMD<Key, Data>(this Element<Key, Data> element)  // Method to display metadata of an element if Data is not inemurable
        {
            StringBuilder displaymd = new StringBuilder();
            bool first = true;

            if (element.NAME != null)
                displaymd.Append(string.Format("  Name: {0}", element.NAME));
            else
                displaymd.Append("  Name: None");

            if(element.DESCRIPTION != null)
                displaymd.Append(string.Format("\n  Description: {0}", element.DESCRIPTION));
            else
                displaymd.Append("\n  Description: None");

            if (element.TIMESTAMP != null)
                displaymd.Append(string.Format("\n  Timestamp: {0}", element.TIMESTAMP));
            else
                displaymd.Append("\n  Timestamp: None");

            displaymd.Append("\n  Relations: ");

            if (element.RELATIONS != null)
            {
                foreach (Key k in element.RELATIONS)
                {
                    if (first)
                    {
                        displaymd.Append(string.Format("{0}", k.ToString()));
                        first = false;
                    }
                    else
                        displaymd.Append(string.Format(" , {0}", k.ToString()));
                }
            }
            else
                displaymd.Append(" None");
            return displaymd.ToString();
        }

        public static string DisplayMD<Key, Data, T>(this Element<Key, Data> element) where Data : IEnumerable<T>  // Method to display metadata of an element if Data is inemurable
        {
            StringBuilder displaymd = new StringBuilder();
            bool first = true;
            if (element.NAME != null)
                displaymd.Append(string.Format("  Name: {0}", element.NAME));
            else
                displaymd.Append("  Name: None");

            if (element.DESCRIPTION != null)
                displaymd.Append(string.Format("\n  Description: {0}", element.DESCRIPTION));
            else
                displaymd.Append("\n  Description: None");

            if (element.TIMESTAMP != null)
                displaymd.Append(string.Format("\n  Timestamp: {0}", element.TIMESTAMP));
            else
                displaymd.Append("\n  Timestamp: None");

            displaymd.Append("\n  Relations: ");

            if (element.RELATIONS != null)
            {
                foreach (Key k in element.RELATIONS)
                {
                    if (first)
                    {
                        displaymd.Append(string.Format("{0}", k.ToString()));
                        first = false;
                    }
                    else
                        displaymd.Append(string.Format(" , {0}", k.ToString()));
                }
            }
            else
                displaymd.Append(" None");
            return displaymd.ToString();
        }   

        public static string DisplayData<Key, Data>(this Element<Key, Data> element)  // Method to display Data of an element if Data is not inemurable
        {
            StringBuilder displaydata = new StringBuilder();
            displaydata.Append(element.DisplayMD());
            if (element.DATA != null)
            {
                displaydata.Append(string.Format("\n  Data: {0}", element.DATA.ToString()));
            }
            else
                displaydata.Append(string.Format("\n  Data: Null"));
            return displaydata.ToString();
        }

        public static string DisplayData<Key, Data, T>(this Element<Key, Data> element) where Data : IEnumerable<T>    // Method to display metadata of an element if Data is inemurable
        {
            StringBuilder displayenumerable = new StringBuilder();
            bool first = true;
            displayenumerable.Append(element.DisplayMD());
            displayenumerable.Append(String.Format("\n  Data: "));
            if (element.DATA != null)
            {
                if (element.DATA as IEnumerable<object> == null)
                {
                    displayenumerable.Append(String.Format("{0}", element.DATA.ToString()));
                }
                else
                {
                    foreach (var item in element.DATA)
                    {
                        if (first)
                        {
                            displayenumerable.Append(String.Format("{0}", item.ToString()));
                            first = false;
                        }
                        else
                            displayenumerable.Append(String.Format(" , {0}", item.ToString()));
                    }
                }
            }
            else
                displayenumerable.Append(" None");
            //Console.WriteLine(displayenumerable.ToString());
            return displayenumerable.ToString();
        }

        public static string DisplayKVP<Key, Value, Data>(this DatabaseDictionary<Key, Value> TempDB)   // Method to display the state of a DB if Data is not inemurable
        {
            StringBuilder KVP = new StringBuilder();
            foreach (Key k in TempDB.Keys())
            {
                KVP.Append("  Key: " + k);
                Value v = TempDB.RetrieveValue(k);
                Element<Key, Data> Tempelement = v as Element<Key, Data>;
                KVP.Append("\n" + Tempelement.DisplayData<Key,Data>());
                KVP.Append("\n\n");
            }
            return KVP.ToString();
        }

        public static string DisplayKVP<Key, Value, Data, T>(this DatabaseDictionary<Key, Value> TempDB) where Data : IEnumerable<T>   // Method to display the state of a DB if Data is inemurable
        {
            StringBuilder KVP = new StringBuilder();
            foreach (Key k in TempDB.Keys())
            {
                KVP.Append("  Key: " + k);
                Value V = TempDB.RetrieveValue(k); 
                Element<Key, Data> Tempelement = V as Element<Key, Data>;
                KVP.Append("\n" + Tempelement.DisplayData<Key,Data,T>());
                KVP.Append("\n\n");
            }
            return KVP.ToString();
        }
    }

#if (TEST_DATABASEEXTENSIONS)
    class TestDatabaseExtensions
    {
        static void Main(string[] args)
        {

        }
    }
#endif
}
