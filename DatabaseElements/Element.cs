///////////////////////////////////////////////////////////////////////////////
// Element.cs - It defines elements for noSQL database (DatabaseDictionary)  //
// Application: NoSQL database implementation, Project 2, CSE681-SMA         //
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
 * This package implements the Element<Key, Data> type, used by 
 * DatabaseDictionary<Key, Value> where Value is Element<Key, Data>
 *
 * Element<Key, Data> state consists of metadata and an instance of the Data type.
 * It also contains methods required to edit any data or metadata of an element.
 *
 * Maintenance:
 * ------------
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
using System.Threading.Tasks;

namespace RemoteNoSQL
{
    public class Element<Key, Data>
    {
        //Metada
        //Data members are private while properties are public in order to make the private data members accessible to other classes.

        private string name {  get; set; }
        public string NAME { get { return name; } set { name = value; } }

        private string description { get; set; }
        public string DESCRIPTION { get { return description; } set { description = value; } }

        private DateTime timestamp { get; set; }
        public DateTime TIMESTAMP { get { return timestamp; } set { timestamp = value; } }

        private List<Key> relations { get; set; }
        public List<Key> RELATIONS { get { return relations; } set { relations = value; } }
        
        //Data
        private Data data { get; set; }
        public Data DATA { get { return data; } set { data = value; } }

        public Element()
        {
            name = "DEFAULT";
            description = "DEFAULT";
            relations = new List<Key>();
        }

        public Element(string aname, string adescription)
        {
            name = aname; description = adescription; timestamp = DateTime.Now; relations = new List<Key>(); data = default(Data);
        }

        public void EditData(Data Temp)
        {
            data = Temp;
        }

        public void EditRelations(List<Key> LK)
        {
            relations = LK;
        }

        public void EditNameMetada(string N)
        {
            name = N;
        }

        public void EditDescriptionMetada(string N)
        {
            description = N;
        }
    }

//Test stub : Used for testing the components of this class. Other tests would be performed in DatabaseElementsTest package

#if(TEST_ELEMENT)
    public class TestElement : Element<int, string>
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Creating an element for <int, string> database");
            Element<int, string> element = new Element<int, string>();
            element.NAME = "Book"; element.DESCRIPTION = "C# Reference Book"; element.TIMESTAMP = DateTime.Now; element.RELATIONS = new List<int> { 2 }; element.DATA = "C# in a Nutshell";
            StringBuilder relation = new StringBuilder();
            foreach ( int k in element.RELATIONS)
            {
                relation.Append(k.ToString());
            }
            Console.WriteLine("\nDisplaying metadata and data of the element\n");
            Console.WriteLine("Name: {0}\tDescription: {1}\tTimestamp: {2}\tRelations: {3}\tData: {4}", element.NAME, element.DESCRIPTION, element.TIMESTAMP, relation , element.DATA);
        }
    }
#endif    
}
