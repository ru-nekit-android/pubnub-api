﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace PubNub_Messaging
{
    public class Pubnub_Example
    {
        static public Pubnub pubnub;

        static public bool deliveryStatus = false;
        static public string channel = "";

        static public void Main()
        {
            Console.WriteLine("HINT: TO TEST RE-CONNECT AND CATCH-UP,");
            Console.WriteLine("      DISCONNECT YOUR MACHINE FROM NETWORK/INTERNET AND ");
            Console.WriteLine("      RE-CONNECT YOUR MACHINE AFTER SOMETIME.");
            Console.WriteLine();
            Console.WriteLine("      IF NO NETWORK BEFORE MAX RE-TRY CONNECT,");
            Console.WriteLine("      NETWORK ERROR MESSAGE WILL BE SENT");
            Console.WriteLine();

            Console.WriteLine("ENTER Channel Name");
            channel = Console.ReadLine();

            Console.WriteLine(string.Format("Channel = {0}",channel));
            Console.WriteLine();

            Console.WriteLine("Enable SSL? ENTER Y for Yes, else N");
            string enableSSL = Console.ReadLine();
            if (enableSSL.Trim().ToLower() == "y")
            {
                Console.WriteLine("SSL Enabled");
            }
            else
            {
                Console.WriteLine("SSL NOT Enabled");
            }
            Console.WriteLine();

            Console.WriteLine("ENTER cipher key for encryption feature.");
            Console.WriteLine("If you don't want to avail at this time, press ENTER.");
            string cipheryKey = Console.ReadLine();
            if (cipheryKey.Trim().Length > 0)
            {
                Console.WriteLine("Cipher key provided.");
            }
            else
            {
                Console.WriteLine("No Cipher key provided");
            }
            Console.WriteLine();

            pubnub = new Pubnub("demo", "demo", "", cipheryKey,
                (enableSSL.Trim().ToLower() == "y") ? true : false);

            Console.WriteLine("ENTER 1 FOR Subscribe");
            Console.WriteLine("ENTER 2 FOR Publish");
            Console.WriteLine("ENTER 3 FOR Presence");
            Console.WriteLine("ENTER 4 FOR Detailed History");
            Console.WriteLine("ENTER 5 FOR Here_Now");
            Console.WriteLine("ENTER 6 FOR Unsubscribe");
            Console.WriteLine("ENTER 7 FOR Presence-Unsubscribe");
            Console.WriteLine("ENTER 8 FOR Time");
            Console.WriteLine("ENTER 0 FOR EXIT OR QUIT");

            bool exitFlag = false;

            Console.WriteLine("");
            while (!exitFlag)
            {
                string userinput = Console.ReadLine();
                switch (userinput)
                {
                    case "0":
                        exitFlag = true;
                        break;
                    case "1":
                        Console.WriteLine("Running subscribe()");
                        pubnub.subscribe<string>(channel, DisplayReturnMessage);
                        //System.Threading.Tasks.Task subtask = System.Threading.Tasks.Task.Factory.StartNew(() => pubnub.subscribe<string>(channel, DisplayReturnMessage));
                        //pubnub.subscribe<object>(channel, DisplayReturnMessage);
                        //pubnub.subscribe(channel, DisplayReturnMessage);
                        break;
                    case "2":
                        Console.WriteLine("Running publish()");
                        Console.WriteLine("Enter the message for publish. To exit loop, enter QUIT");
                        string publishMsg = Console.ReadLine();
                        pubnub.publish<string>(channel, publishMsg, DisplayReturnMessage);
                        break;
                    case "3":
                        Console.WriteLine("Running presence()");
                        pubnub.presence<string>(channel, DisplayReturnMessage);
                        //System.Threading.Tasks.Task pretask = System.Threading.Tasks.Task.Factory.StartNew(() => pubnub.presence<string>(channel, DisplayReturnMessage));
                        //pubnub.presence<object>(channel, DisplayReturnMessage);
                        break;
                    case "4":
                        Console.WriteLine("Running detailed history()");
                        pubnub.detailedHistory<string>(channel, 100, DisplayReturnMessage);
                        //pubnub.detailedHistory<object>(channel, 100, DisplayReturnMessage);
                        break;
                    case "5":
                        Console.WriteLine("Running Here_Now()");
                        pubnub.here_now<string>(channel, DisplayReturnMessage);
                        //pubnub.here_now<object>(channel, DisplayReturnMessage);
                        break;
                    case "6":
                        Console.WriteLine("Running unsubscribe()");
                        pubnub.unsubscribe<string>(channel, DisplayReturnMessage);
                        //pubnub.unsubscribe<object>(channel, DisplayReturnMessage);
                        break;
                    case "7":
                        Console.WriteLine("Running presence-unsubscribe()");
                        pubnub.presence_unsubscribe<string>(channel, DisplayReturnMessage);
                        break;
                    case "8":
                        Console.WriteLine("Running time()");
                        pubnub.time<string>(DisplayReturnMessage);
                        break;
                    default:
                        Console.WriteLine("INVALID CHOICE.");
                        break;
                }
            }

            Console.WriteLine("\nPress any key to confirm exit.\n\n");
            Console.ReadLine();

        }

        static void DisplayReturnMessage(string result)
        {
            Console.WriteLine(result);
        }

        static void DisplayReturnMessage(object result)
        {
            IList<object> message = result as IList<object>;

            if (message != null && message.Count >= 1)
            {
                for (int index = 0; index < message.Count; index++)
                {
                    ParseObject(message[index], 1);
                }
            }
            else
            {
                Console.WriteLine("unable to parse data");
            }
        }

        static void ParseObject(object result, int loop)
        {
            if (result is object[])
            {
                object[] arrResult = (object[])result;
                foreach (object item in arrResult)
                {
                    if (item != null)
                    {
                        if (!item.GetType().IsGenericType)
                        {
                            if (!item.GetType().IsArray)
                            {
                                Console.WriteLine(item.ToString());
                            }
                            else
                            {
                                ParseObject(item, loop + 1);
                            }
                        }
                        else
                        {
                            ParseObject(item, loop + 1);
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                   }
                }
            }
            else if (result.GetType().IsGenericType && (result.GetType().Name == typeof(Dictionary<,>).Name))
            {
                Dictionary<string, object> itemList = (Dictionary<string, object>)result;
                foreach (KeyValuePair<string, object> pair in itemList)
                {
                    Console.WriteLine(string.Format("key = {0}", pair.Key));
                    if (pair.Value is object[])
                    {
                        Console.WriteLine("value = ");
                        ParseObject(pair.Value, loop);
                    }
                    else
                    {
                        Console.WriteLine(string.Format("value = {0}", pair.Value));
                    }
                }
            }
            else
            {
                Console.WriteLine(result.ToString());
            }

        }

    }
}
