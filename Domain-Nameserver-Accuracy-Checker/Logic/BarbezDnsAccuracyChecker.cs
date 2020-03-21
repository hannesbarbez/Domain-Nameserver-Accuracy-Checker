using Domain_Nameserver_Accuracy_Checker.Data;
using JHSoftware;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Domain_Nameserver_Accuracy_Checker.Logic
{
    internal class BarbezDnsAccuracyChecker
    {
        private DelegateTestReporter dtr;
        private AsyncCallback callBack;
        private List<string> lns = new List<string>();
        string[] splitChars = new string[] { ",", ";", " ", Environment.NewLine, "\r", "\n" };

        internal double WaitTimeBetweenQueryingDns { get; set; }

        internal BarbezDnsAccuracyChecker(double waitTimeBetweenQueringDns, string nsToTest)
        {
            this.WaitTimeBetweenQueryingDns = waitTimeBetweenQueringDns;
            PopulateNameServerList(nsToTest);
        }

        private void ProcessTestAnswersOfDnsAsync(IAsyncResult ar)
        {
            DnsRequest dnsRequest = (DnsRequest)ar.AsyncState;
            IPAddress resolvedIP;

            try
            {
                IPAddress[] ips = DnsClient.EndLookupHost(ar);
                resolvedIP = ips[0];

                this.dtr(dnsRequest.Url + ";" + resolvedIP.ToString() + ";" + dnsRequest.DnsServerIP[0].ToString());
            }
            catch
            {
                this.dtr(dnsRequest.Url + ";" + dnsRequest.DnsServerIP[0].ToString());
            }
        }

        internal int GetAmountOfNameServers()
        {
            return this.lns.Count;
        }

        private void PopulateNameServerList(string nsToTest)
        {
            this.lns.AddRange(nsToTest.Split(this.splitChars, StringSplitOptions.RemoveEmptyEntries));
        }

        internal bool PerformTestAsync(string serverToResolve, DelegateTestReporter dtr)
        {
            try
            {
                this.dtr = dtr;
                this.callBack = new AsyncCallback(ProcessTestAnswersOfDnsAsync);

                foreach (string ns in this.lns)
                {
                    DnsClient.RequestOptions ro = new DnsClient.RequestOptions();
                    IPAddress result = IPAddress.Loopback;
                    IPAddress.TryParse(ns, out result);

                   
                    if (result == null) result = IPAddress.Loopback;

                    ro.DnsServers = new IPAddress[] { result };

                    DnsRequest dnsRequest = new DnsRequest();
                    dnsRequest.DnsServerIP = ro.DnsServers;
                    dnsRequest.Url = serverToResolve;

                    Thread.Sleep(Int32.Parse(Math.Ceiling(this.WaitTimeBetweenQueryingDns).ToString()));
                    DnsClient.BeginLookupHost(serverToResolve, DnsClient.IPVersion.IPv4, ro, callBack, dnsRequest);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}