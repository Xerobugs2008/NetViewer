using NetTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetOverview
{
    public partial class Form1 : Form
    {
        List<TCPEndpoint> AddressList = new List<TCPEndpoint>(256);
        public Form1()
        {
            InitializeComponent();
        }

        public void MonitorRange(IPAddress start, IPAddress end)
        {
            var range = new IPAddressRange(start, end);
            // enumerate all ip addresses in the specified range and add them to potential list of addresses
            // for polling
            foreach (var ip in range)
            {
                AddressList.Add(new TCPEndpoint(ip));
                TestEndpoint tp = new TestEndpoint(ip,5000);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MonitorRange(new IPAddress(new byte[] { 192, 168, 0, 1 }), new IPAddress(new byte[] { 192, 168, 0, 254 }));
        }
    }
}
