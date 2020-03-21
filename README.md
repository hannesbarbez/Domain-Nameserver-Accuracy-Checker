https://www.barbez.eu/

# Domain Nameserver Accuracy Checker
Simply verify whether a list of domain name servers (DNS servers) finds the domains they should, but not those they should not. Domain Nameserver Accuracy Checker checks this automatically and removes those that fail the tests.

# Features
- Load a server list from a CSV or TXT file;
- Save the result to a TXT file;
- Multi-Threaded Tests; the interface does not hang during testing;
- Built based on Three-Tier Model, which reduces the risk for bugs compared to a monolithically structured app.

# Pre-requisites
"Domain Nameserver Accuracy Checker" uses the excellent JHSoftware.DnsClient library for the actual checking. Since JHSoftware.DnsClient.dll is not open source, download it from http://simpledns.com/dns-client-lib.aspx and add it to /Domain-Nameserver-Accuracy-Checker/assets in order for the project to build and run.

# Disclaimer
** !! Do not set the "Time between DNS queries" slider too low, as on some networks this may result in an apparent (temporary) loss of internet connection !! **

The author is definitely NOT responsible for any misuse of this application and of its code.