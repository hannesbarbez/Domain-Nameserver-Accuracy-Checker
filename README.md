https://www.barbez.eu/

# Domain Nameserver Accuracy Checker
Simply verify whether a list of domain name servers (DNS servers) finds the domains they should, but not those they should not. Domain Nameserver Accuracy Checker checks this automatically and removes those that fail the tests.

# Features
- Load a server list from a CSV or TXT file;
- Save the result to a TXT file;
- Multi-Threaded Tests; the interface does not hang during testing;
- Built based on Three-Tier Model, which reduces the risk for bugs compared to a monolithically structured app.

# Pre-requisites
"Domain Nameserver Accuracy Checker" uses the excellent JHSoftware.DnsClient library for the actual checking. Since JHSoftware.DnsClient.dll is not open source, download it from http://simpledns.com/ and add it to /Domain-Nameserver-Accuracy-Checker/assets in order for the project to build and run.

# Disclaimer
**Do not set the "Time between DNS queries" slider too low, as on some networks this may result in an apparent (temporary) loss of internet connection. The author is definitely NOT responsible for any misuse of this application and of its code.**

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
