using System.Net;

namespace NetOverview
{
    internal class TCPEndpoint
    {
        public IPAddress Address { get; private set; }
        public TCPEndpoint(IPAddress address)
        {
            Address = address;
        }
    }
}