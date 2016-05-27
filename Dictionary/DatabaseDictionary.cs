///////////////////////////////////////////////////////////////////////////////
// DatabaseDictionary.cs - It is the backbone of the application as it       //
//                         contains code which define a dictionary.          //
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
 * This package defines how a dictionary should look like and behave. Dictionay is a key-value pair 
 * data structure which holds value associated with a unique key assigned to it.
 * This package holds methods for adding, removing a key-value pair
 *
 * It also contains methods to get a set of all keys or values existing in database.
 *
 * Maintenance:
 * ------------
 * Required Files: Element.cs
 *
 * Build Process:  devenv Project2Demonstration.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 2.0 : 18 Nov 2015 :: Created a duplicate RemoveKVP to support overloading of method, slightly modified key generators
 * ver 1.0 : 7 Oct 2015
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RemoteNoSQL
{
    // Database dictionary 
   public class DatabaseDictionary<Key, Value>
    {
        private Dictionary<Key, Value> database;
        public Dictionary<Key, Value> DATABASE { get { return database; } set { database = value; } } //Property to access the dictionary

        public DatabaseDictionary()
        {
            database = new Dictionary<Key, Value>();
        }

        public void UpdateValue(Key K, Value V) //Requires two arguments: one key- where value is to be updated and other is value which is to be updated.
        {
            if (database.ContainsKey(K) == false)
                Console.WriteLine("Cannot update the database element. Key doesn't exist in the dictionary.");
            else
                database[K] = V;   
        }

        public bool AddPair(Key K, Value V)
        {
            if (database.ContainsKey(K) == true)
                return false;
            else
                database[K] = V;
            return true;
        }

        public string RemoveKVPair(List <Key> LK) // Requires a list of keys of Key-Value pairs to be removed from the dictionary, thus avoiding calling
        {                                       // the multiple times if user requires to delete multiple kVPs at once.
            string result = null;
            if (LK !=null)
            {
                foreach (Key k in LK)
                {
                    if(database.ContainsKey(k))
                    {
                        database.Remove(k);
                        result = String.Format("Key: {0} was deleted from database" + "\n", k.ToString());
                    }
                    else
                        result = String.Format("Key: {0} was not found in database" + "\n", k.ToString());
                }
            }
            else
                result = String.Format("No Key Value pair was deleted." + "\n");
            return result;
        }

        public string RemoveKVPair(Key k) // Requires a list of keys of Key-Value pairs to be removed from the dictionary, thus avoiding calling
        {                                       // the multiple times if user requires to delete multiple kVPs at once.
            string result = null;
            if (k != null)
            {

                if (database.ContainsKey(k))
                {
                    database.Remove(k);
                    result = String.Format("Key: {0} was deleted from database" + "\n", k.ToString());
                }
                else
                    result = String.Format("Key: {0} was not found in database" + "\n", k.ToString());
            }

            else
                result = String.Format("No Key Value pair was deleted." + "\n");
            return result;
        }

        public bool RetrieveValue(Key K, out Value V)
        {
            if (database.ContainsKey(K) == false)
            {
                V = default(Value);
                return false;
            }
            else
                V = database[K];
            return true;
        }

        public Value RetrieveValue(Key K)
        {
            Value V = default(Value);
            if (database.ContainsKey(K) == false)
                return V;
            else
                V = database[K];
            return V;
        }

        public IEnumerable<Key> Keys()
        {
            return database.Keys;
        }

        public IEnumerable<Value> Values()
        {
            return database.Values;
        }

        public List<Key> FindKeys(Value V) //Finds the keys that contain a partcular value
        {
            List<Key> LK = new List<Key>();
            if (V != null)
            {                
                foreach (Key k in database.Keys)
                {
                    if (database.ContainsValue(V))
                        LK.Add(k);
                    else
                        Console.WriteLine("No key found with this value");
                }
            }
            else
                Console.WriteLine("Null Value");
            return LK;
        }

        static int IntKey = 2;

        //Key generators generate  with the help of above integer (k1) for both types of databases as user only provides value or Element to be inserted. 
        public int IKeygenerator()
        {
            return ++IntKey;
        }

        public string SKeygenerator()
        {
            ++IntKey;
            StringBuilder finalk = new StringBuilder();
            finalk.Append("Key");
            finalk.Append(IntKey.ToString());
            return finalk.ToString();
        }
    }

#if(TEST_DICTIONARY)
    class TestDictionary
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing Database Dictionary class" + "\n");
            DatabaseDictionary<int, Element<int, string>> d = new DatabaseDictionary<int, Element<int, string>>();
            Element<int, string> IKElement1 = new Element<int, string>();
            IKElement1.NAME = "Pink Floyd"; IKElement1.DESCRIPTION = "Rockband"; IKElement1.TIMESTAMP = DateTime.Now; IKElement1.RELATIONS = new List<int> { 1, 2 }; IKElement1.DATA = "Wish you were here";
            d.AddPair(1, IKElement1);
            //Above code is there to show how coding of adding an element in a database would look like.

            Console.WriteLine("Databases can't be tested in this project as most of the testing depends on ExtensionMethod which contain all extension methods.");
            Console.WriteLine("To avoid circular dependency, all tests are performed in DatabaseDictionaryTest project");
        }
    }
#endif
}