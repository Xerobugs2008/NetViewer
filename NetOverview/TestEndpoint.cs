using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Threading;
using System.Net;
using NetTools;
using SnmpSharpNet;

namespace NetOverview
{
    public class TestEndpoint
    {
        public IPAddress Address { get; private set; }
        public int Timeout { get; private set; }
        public int Retries { get; private set; }
        public int SeenCount { get; private set; }

        // Create a buffer of data to be transmitted.
        byte[] buffer = Encoding.ASCII.GetBytes("greengage discovery ping");

        // Set options for transmission:
        // The data can go through 64 gateways or routers
        // before it is destroyed, and the data packet
        // cannot be fragmented.
        PingOptions options = new PingOptions(64, true);

        // Ping operation
        Ping pingSender = new Ping();

        public TestEndpoint(IPAddress address, int timeout)
        {
            Address = address;
            Timeout = timeout;
            SeenCount = 0;
            // When the PingCompleted event is raised,
            // the PingCompletedCallback method is called.
            pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);
            Retries = 10;

            DoSomething();
        }

        private static string GetMachineNameFromIPAddress(string ipAdress)
        {
            string machineName = string.Empty;
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAdress);

                machineName = hostEntry.HostName;
            }
            catch (Exception ex)
            {
                // Machine not found...
            }
            return machineName;
        }

        void DoSomething()
        {
            if (Retries > 0)
            {
                Retries--;
                // Send the ping asynchronously.
                // Use the waiter as the user token.
                // When the callback completes, it can wake up this thread.
                if (SeenCount==0)
                    pingSender.SendAsync(Address, Timeout, buffer, options, this);
                else
                {
                    // try probing for other information
                    GetSNMPStuff();
                }
            }
        }

        private void GetSNMPStuff()
        {
            SimpleSnmp snmpVerb = new SimpleSnmp(Address.ToString(), 161, "public");
            if (!snmpVerb.Valid)
            {
                Console.WriteLine("Unable to access community");
                return;
            }
            Dictionary< Oid, AsnType> x = snmpVerb.Walk(SnmpVersion.Ver2, ".1.3.6.1.4.1.2021.11.10.0");
            if (x.Count>0)
            {
                foreach(Oid xx in x.Keys)
                {
                    Console.WriteLine(xx);
                }
            }
        }

        /// <summary>
        /// whether successful or not we get a ping response here
        /// If successful then the endpoint can be logged as present and can then potentially be monitored for futher information 
        /// via SNMP or whatever
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            PingReply reply = e.Reply;
            if (reply.Status == IPStatus.Success)
            {
                SeenCount++;
                Console.Write(Address);
                Console.WriteLine(" Ping Success " + GetMachineNameFromIPAddress(Address.ToString()));
            }
            else if (reply.Status==IPStatus.TimedOut)
            {
                Console.WriteLine(Address + " Timeout");
            }
            DoSomething();
        }
    }
}
