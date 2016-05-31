//////////////////////////////////////////////////////////////////////////////////
// ICommunicationManager.cs - Contract for WCF message-passing service          //
// ver 2.0                                                                      //
// Author: Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
////////////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to System.ServiceModel
 * - Added using System.ServiceModel
 * - Added reference to System.Runtime.Serialization
 * - Added using System.Runtime.Serialization
 */
/*
 * Maintenance History:
 * --------------------
 * ver 2.0 : 18 Nov 2015
 * Added new parameters to store information about the round trip, process time etc.

 * ver 1.1 : 29 Oct 2015
 * - added comment in data contract
 * ver 1.0 : 18 Oct 2015
 * - first release
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace RemoteNoSQL
{
    [ServiceContract(Namespace = "RemoteNoSQL")]
    public interface ICommunicationManager
    {
        [OperationContract(IsOneWay = true)]
        void sendMessage(Message message);
    }

    [DataContract]
    public class Message
    {
        [DataMember]
        public string FromURL { get; set; }
        [DataMember]
        public string ToURL { get; set; }
        [DataMember]
        public string MessageContent { get; set; }  // will hold XML defining message information
        [DataMember]
        public string MessageID { get; set; }   // Every message sent is defined a unique ID to distinguish it from rest
        [DataMember]
        public DateTime TimeSent { get; set; }   // It stores the information when a message is sent across.                     //
        [DataMember]                                                                                                             //
        public DateTime TimeReceived { get; set; }   // It stores the information when a message is received on the other end    //
        [DataMember]                                                                                                             // Having these parameters saves a lot of trouble for HiResTimer
        public TimeSpan TravelTime { get; set; }   //   stores the roundtrip time for this message                               // But it must be set very carefully in order to measure the 
        [DataMember]                                                                                                             // right round-trip time.
        public ulong ServerProcessTime { get; set; }       // If it is a query, it is set to the time it takes for a   
    }                                                      // server to process it                         
}