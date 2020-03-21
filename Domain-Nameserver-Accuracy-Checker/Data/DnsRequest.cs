using System.Net;

namespace Domain_Nameserver_Accuracy_Checker.Data
{
    public struct DnsRequest
    {
        public string Url { get; set; }
        public IPAddress[] DnsServerIP { get; set; }
    }
}
