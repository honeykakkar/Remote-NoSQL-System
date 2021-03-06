﻿// Package contains code to generate dead-letter queues (DLQs) 
// used for storing messages that could not be delivered during 
// communication between client and server

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteNoSQL
{
    public class ProxyAndDeadLetterQ
    {
        ICommunicationManager Proxy;
        public ICommunicationManager PROXY { get { return Proxy; } set { Proxy = value; } }
        MessageQueue<Message> DeadLetterQueue;
        public MessageQueue<Message> DEADLETTERQUEUE { get { return DeadLetterQueue; } set { DeadLetterQueue = value; } }

        public ProxyAndDeadLetterQ()
        {
            Proxy = null;
            DeadLetterQueue = new MessageQueue<Message>();
        }
    }
}
