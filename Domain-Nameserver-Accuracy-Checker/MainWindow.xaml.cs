using Domain_Nameserver_Accuracy_Checker.Logic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Domain_Nameserver_Accuracy_Checker
{
    public partial class MainWindow : Window
    {
        private Thread dnsTesterThread;
        private List<string> goodNs = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void lblAbout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Generic.OpenDefaultBrowser("http://" + lblAbout.Content);
        }

        #region Nameserver Maintenance
        private void btnCleanNameserversNotKnowingGoogle_Click(object sender, RoutedEventArgs e)
        {
            string nsToTest = tbNs.Text;
            ToggleNameServerGUIElements();
            BarbezDnsAccuracyChecker bdac = new BarbezDnsAccuracyChecker(sldWaitTimeBetweenDnsQuestions.Value, nsToTest);
            this.pbNameserverCleaner.Maximum = bdac.GetAmountOfNameServers();
            string serverToResolve = tbCleanNameserversThatDontKnowExistingDomain.Text;

            this.dnsTesterThread = new Thread(() =>
            {
                DelegateTestReporter dtr = new DelegateTestReporter(KeepNsKnowingExistingDomainName);
                bool success = bdac.PerformTestAsync(serverToResolve, dtr);
                Dispatcher.Invoke((Action)(() =>
                {
                    if (!success) MessageBox.Show("Please only use correct IP addresses. Verify your input, then try again." + Environment.NewLine + "You can seperate IP adresses using a new line or a comma.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    ToggleNameServerGUIElements();
                }));
            });
            this.dnsTesterThread.Start();
        }

        private void ToggleNameServerGUIElements()
        {
            if (btnCleanNameserversThatDontKnowExistingDomain.IsEnabled)
            {
                this.goodNs.Clear();

                btnCleanNameserversThatDontKnowExistingDomain.IsEnabled = false;
                btnCleanNameserversThatKnowNonExistingDomain.IsEnabled = false;
                tbCleanNameserversThatDontKnowExistingDomain.IsEnabled = false;
                tbCleanNameserversThatKnowNonExistingDomain.IsEnabled = false;

                tbNs.IsEnabled = false;
                btnSave.IsEnabled = false;
                btnClear.IsEnabled = false;
                btnPaste.IsEnabled = false;
                btnLoadFromTxtFile.IsEnabled = false;
                sldWaitTimeBetweenDnsQuestions.IsEnabled = false;
            }
            else
            {
                btnCleanNameserversThatDontKnowExistingDomain.IsEnabled = true;
                btnCleanNameserversThatKnowNonExistingDomain.IsEnabled = true;
                tbCleanNameserversThatDontKnowExistingDomain.IsEnabled = true;
                tbCleanNameserversThatKnowNonExistingDomain.IsEnabled = true;

                tbNs.IsEnabled = true;
                btnSave.IsEnabled = true;
                btnClear.IsEnabled = true;
                btnPaste.IsEnabled = true;
                btnLoadFromTxtFile.IsEnabled = true;
                sldWaitTimeBetweenDnsQuestions.IsEnabled = true;
            }
        }

        private void btnCleanNameserversKnowingNonExistingServer_Click(object sender, RoutedEventArgs e)
        {
            string nsToTest = tbNs.Text;
            ToggleNameServerGUIElements();
            BarbezDnsAccuracyChecker bdac = new BarbezDnsAccuracyChecker(sldWaitTimeBetweenDnsQuestions.Value, nsToTest);
            this.pbNameserverCleaner.Maximum = bdac.GetAmountOfNameServers();
            string serverToResolve = tbCleanNameserversThatKnowNonExistingDomain.Text;

            this.dnsTesterThread = new Thread(() =>
            {
                DelegateTestReporter dtr = new DelegateTestReporter(KeepNsNotKnowingNonExistingDomainName);
                bool success = bdac.PerformTestAsync(serverToResolve, dtr);

                Dispatcher.Invoke((Action)(() =>
                {
                    if (!success) MessageBox.Show("Please only use correct IP addresses. Verify your input, then try again." + Environment.NewLine + "You can seperate IP adresses using a new line or a comma.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    ToggleNameServerGUIElements();
                }));
            });
            this.dnsTesterThread.Start();
        }

        private void KeepNsNotKnowingNonExistingDomainName(string s)
        {
            string[] data = s.Split(';');

            if (data.Length == 2)
            {
                this.goodNs.Add(data[1]); //index 1 holds the ns ip here, so we want to keep that
                //if length is 2 it means that only an ns_ip and the requested hostname were returned. (3 if successful b/c resulting IP)
                //Here we want to delete the ones that return 3 values.
            }
            try
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    pbNameserverCleaner.Value++;
                    if (pbNameserverCleaner.Value == pbNameserverCleaner.Maximum)
                    {
                        DeleteDefectiveNameservers();
                        pbNameserverCleaner.Value = 0;
                    }
                }));
            }
            catch
            {
               
            }
        }

        private void KeepNsKnowingExistingDomainName(string s)
        {
            string[] data = s.Split(';');

            if (data.Length == 3)
            {
                this.goodNs.Add(data[2]); //index 2 holds the ns ip here, so we want to keep that
                //if length is 2 it means that only an ns_ip and the requested hostname were returned. (3 if successful b/c resulting IP)
            }
            try
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    pbNameserverCleaner.Value++;
                    if (pbNameserverCleaner.Value == pbNameserverCleaner.Maximum)
                    {
                        DeleteDefectiveNameservers();
                        pbNameserverCleaner.Value = 0;
                    }
                }));
            }
            catch { }
        }

        private void DeleteDefectiveNameservers()
        {
            if (this.dnsTesterThread != null)
                if (this.dnsTesterThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin | this.dnsTesterThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    this.dnsTesterThread.Abort();
                    while (this.dnsTesterThread.ThreadState != System.Threading.ThreadState.Aborted) { }
                    this.pbNameserverCleaner.Value = 0;
                }

            if (this.goodNs != null)
                if (this.goodNs.Count() > 0)
                {
                    bool succesful = DisplayCleanedUpNsList(this.goodNs);
                    if (succesful) MessageBox.Show("Succesfully removed unreliable nameservers.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                    {
                        MessageBoxResult nbr = MessageBox.Show("Could not clean out *all* of the unreliable nameservers. \r\n\r\n Want to try to clean them again (this will skip testing so should be faster).", "Fail", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (nbr == MessageBoxResult.Yes) DeleteDefectiveNameservers();
                    }
                }
        }

        private bool DisplayCleanedUpNsList(List<string> defectiveNameServers)
        {
            try
            {
                tbNs.Clear();
                foreach (string s in defectiveNameServers)
                    tbNs.Text += s + Environment.NewLine;
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            tbNs.Clear();
        }

        private void btnLoadFromTxtFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV or TXT file|*.txt;*.csv|TXT|*.txt|CSV|*.csv|All files (*.*)|*.*";
                bool? opened = openFileDialog.ShowDialog();
                string fn = openFileDialog.FileName;
                string s = "";
                Thread t = new Thread(() =>
                {
                    if (opened == true)
                    {
                        s = File.ReadAllText(fn);
                        Dispatcher.Invoke((Action)(() =>
                        {
                            tbNs.Text = s;
                        }));
                    }
                });
                t.Start();
            }
            catch
            {
                MessageBox.Show("Something went wrong. Verify if you have the necessary rights to open the file, then try again." + Environment.NewLine + "We could not open the file.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            tbNs.Text = Clipboard.GetText();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                    using (StreamWriter sw = new StreamWriter(File.OpenWrite(saveFileDialog.FileName)))
                        sw.Write(tbNs.Text);
            }
            catch
            {
                MessageBox.Show("Something went wrong. Verify if you have the necessary rights to save the file to this location, then try again." + Environment.NewLine + "We could not save the file, maybe try saving to another location.",
                     "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}