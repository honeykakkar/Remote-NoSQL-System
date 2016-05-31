/////////////////////////////////////////////////////////////////////////////////
// Starter.cs - It is responsible for creating instances of readers and writers//
// Application: Remote NoSQL database implementation, Project 4, CSE681-SMA  //
// Language:    C#, Framework 4.5.2, Visual Studio 2015 (Community Edt.)     //
// Platform:    Dell Inspiron 13 7000, Core-i7, Windows 10                   //
// Author:      Honey Kakkar, Computer Engineering, SU                       //
//              hkakkar@syr.edu                                              //
///////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package is capable of starting readers and writers from WPF as well as Command-Prompt. 
 *
 * Maintenance:
 * ------------
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
using System.Diagnostics;
using System.Reflection;

namespace RemoteNoSQL
{
    // To start the instances of read clients and write clients
    public class Starter
    {
        static uint LocalPort = 8085;   // Base port of a client. 

        static public void StartClients(string[] args)  // this function is called when clients are initiated using any of the means including Console, Run.batch, Test Executive
        {
            // Arguments to be fed to the process being started. It would act as set of arguments for read clients and write clients
            // T or F in order to demonstrate Req. #6

            string BaseArgument = "/R http://localhost:8080/CommunicationManager /L http://localhost:8085/CommunicationManager F";
            try
            {

                int NumberofReadClients = int.Parse(args[1]);
                int NumberofWriteClients = int.Parse(args[3]);
                string CurrentLocation = Environment.CurrentDirectory;
                string SolutionPath = CurrentLocation.Substring(0, CurrentLocation.LastIndexOf("NoSQL Implementation") + 20);
                string ReadClientApp = String.Concat(SolutionPath, "\\", args[0], "\\bin\\debug\\", args[0], ".exe");
                string WriteClientApp = String.Concat(SolutionPath, "\\", args[2], "\\bin\\debug\\", args[2], ".exe");

                for (int i = 0; i < NumberofWriteClients; ++i)
                {
                    ProcessStartInfo PSI = new ProcessStartInfo();
                    PSI.UseShellExecute = true;
                    PSI.FileName = WriteClientApp;
                    string Argument = BaseArgument.Replace("8085", (++LocalPort).ToString());
                    PSI.Arguments = Argument.Replace("F", args[4].ToString());
                    Process.Start(PSI);
                    ("\n  Starting WriteClient #" + (i+1)).Wrap();
                    Console.WriteLine("  Press any key to start another instance of client......");   // Wait for user input to start another client instance
                    Console.ReadKey();
                }

                for (int i = 0; i < NumberofReadClients; ++i)
                {
                    ProcessStartInfo PSI = new ProcessStartInfo();
                    PSI.UseShellExecute = true;
                    PSI.FileName = ReadClientApp;
                    string Argument = BaseArgument.Replace("8085", (++LocalPort).ToString());
                    PSI.Arguments = Argument.Replace("F", args[4].ToString());
                    Process.Start(PSI);
                    ("\n  Starting ReadClient #" + (i+1)).Wrap();
                    Console.WriteLine("  Press any key to start another instance of client......");    // Wait for user input to start another client instance
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static public void StartClientsfromWPF(string[] args)  // this function is called when clients are initiated using WPF
        {
            // Arguments to be fed to the process being started. It would act as set of arguments for read clients and write clients
            // T or F in order to demonstrate Req. #6

            string BaseArgument = "/R http://localhost:8080/CommunicationManager /L http://localhost:8085/CommunicationManager F";
            try
            {

                int NumberofReadClients = int.Parse(args[1]);
                int NumberofWriteClients = int.Parse(args[3]);
                string CurrentLocation = Environment.CurrentDirectory;
                string SolutionPath = CurrentLocation.Substring(0, CurrentLocation.LastIndexOf("NoSQL Implementation") + 20);
                string ReadClientApp = String.Concat(SolutionPath, "\\", args[0], "\\bin\\debug\\", args[0], ".exe");
                string WriteClientApp = String.Concat(SolutionPath, "\\", args[2], "\\bin\\debug\\", args[2], ".exe");

                for (int i = 0; i < NumberofWriteClients; ++i)
                {
                    ProcessStartInfo PSI = new ProcessStartInfo();
                    PSI.UseShellExecute = true;
                    PSI.FileName = WriteClientApp;
                    string Argument = BaseArgument.Replace("8085", (++LocalPort).ToString());
                    PSI.Arguments = Argument.Replace("F", args[4].ToString());
                    Process.Start(PSI);
                }

                for (int i = 0; i < NumberofReadClients; ++i)
                {
                    ProcessStartInfo PSI = new ProcessStartInfo();
                    PSI.UseShellExecute = true;
                    PSI.FileName = ReadClientApp;
                    string Argument = BaseArgument.Replace("8085", (++LocalPort).ToString());
                    PSI.Arguments = Argument.Replace("F", args[4].ToString());
                    Process.Start(PSI);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
            // Starter takes arguments as ProcessName NumberOfInstances ProcessName NumberOfInstances
            // For E.G. ReadClient 10 WriteClient 8
        {
           
            //StartClientsFromCommand(args);
            StartClients(args);
        }
    }
}
