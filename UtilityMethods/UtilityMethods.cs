using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace RemoteNoSQL
{
    static public class UtilityMethods
    {
        public static void Wrap(this string input)
        {
            Console.Write("{0}\n{1}\n", input, new string('-', input.Length + 1));
        }

        static public string processCommandLineForLocal(string[] args, string localUrl)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                if ((args.Length > i + 1) && (args[i] == "/l" || args[i] == "/L"))
                {
                    localUrl = args[i + 1];
                }
            }
            return localUrl;
        }

        static public string processCommandLineForRemote(string[] args, string remoteUrl)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                if ((args.Length > i + 1) && (args[i] == "/r" || args[i] == "/R"))
                {
                    remoteUrl = args[i + 1];
                }
            }
            return remoteUrl;
        }

        //----< helper functions to construct url strings >------------------
        public static string makeUrl(string address, string port)
        {
            return "http://" + address + ":" + port + "/CommunicationManager";
        }
        public static string urlPort(string url)
        {
            int posColon = url.LastIndexOf(':');
            int posSlash = url.LastIndexOf('/');
            string port = url.Substring(posColon + 1, posSlash - posColon - 1);
            return port;
        }
        public static string urlAddress(string url)
        {
            int posFirstColon = url.IndexOf(':');
            int posLastColon = url.LastIndexOf(':');
            string port = url.Substring(posFirstColon + 3, posLastColon - posFirstColon - 3);
            return port;
        }

        public static void swapUrls(ref Message message)
        {
            string temp = message.FromURL;
            message.FromURL = message.ToURL;
            message.ToURL = temp;
        }

        public static bool verbose { get; set; } = false;

        public static void waitForUser()
        {
            Thread.Sleep(200);
            Console.Write("\n  press any key to quit: ");
            Console.ReadKey();
        }

        public static void showMessage(Message message)
        {
            Console.Write("\n  message.FromURL: {0}", message.FromURL);
            Console.Write("\n  message.ToURL:   {0}", message.ToURL);
            Console.Write("\n  message.MessageContent: {0}", message.MessageContent);
        }

        static void Main(string[] args)
        {
            "testing utilities".Wrap();
            Console.WriteLine();

            "testing makeUrl".Wrap();
            string localUrl = makeUrl("localhost", "7070");
            string remoteUrl = makeUrl("localhost", "7071");
            Console.Write("\n  localUrl  = {0}", localUrl);
            Console.Write("\n  remoteUrl = {0}", remoteUrl);
            Console.WriteLine();

            "testing url parsing".Wrap();
            string port = urlPort(localUrl);
            string addr = urlAddress(localUrl);
            Console.Write("\n  local port = {0}", port);
            Console.Write("\n  local addr = {0}", addr);
            Console.WriteLine();

            "testing processCommandLine".Wrap();
            localUrl = processCommandLineForLocal(args, localUrl);
            remoteUrl = processCommandLineForRemote(args, remoteUrl);
            Console.Write("\n  localUrl  = {0}", localUrl);
            Console.Write("\n  remoteUrl = {0}", remoteUrl);
            Console.WriteLine();

            "testing swapUrls(ref Message message)".Wrap();
            Message message = new Message();
            message.ToURL = "http://localhost:8080/CommunicationManager";
            message.FromURL = "http://localhost:8081/CommunicationManager";
            message.MessageContent = "swapee";
            showMessage(message);
            Console.WriteLine();

            swapUrls(ref message);
            showMessage(message);
            Console.Write("\n\n");
        }
    }
}
