using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NFCReader.Resources;
using Windows.Networking.Proximity;
using System.Diagnostics;
using NdefUtils;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Microsoft.Phone.Tasks;

namespace NFCReader
{
    public partial class MainPage : PhoneApplicationPage
    {


        private ProximityDevice _proximitiyDevice;
        private long _subscriptionIdNdef;
        private StringBuilder stringBuilder;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            initNFC();

            stringBuilder = new StringBuilder();
        }


        private void initNFC()
        {
            // Initialize NFC
            _proximitiyDevice = ProximityDevice.GetDefault();

            if (_proximitiyDevice == null)
            {
                Debug.WriteLine("It seems this is not NFC-available device");
                MessageBox.Show(AppResources.Error_NFC_NotSupported, AppResources.Title_error, MessageBoxButton.OK);
                return;
            }

            _subscriptionIdNdef = _proximitiyDevice.SubscribeForMessage("NDEF", NFCMessageReceivedHandler);


        }

        private void NFCMessageReceivedHandler(ProximityDevice sender, ProximityMessage message)
        {
            var parser = new NdefParser(message);
            List<NdefRecord> ndefRecords = new List<NdefRecord>();
            ndefRecords = parser.Parse();
            InitializePivot();

            if (ndefRecords.Count == 0)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    // there's no NDEF record.
                    NFCMessage.Text = AppResources.NFC_Message_error;
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    // NDEF records found.
                    NFCMessage.Text = AppResources.NFC_Message_detected;
                    ApplicationBar = CreateAppBar();
                });
            }

            int i = 0;

            foreach (NdefRecord r in ndefRecords)
            {

                i++;

                Dispatcher.BeginInvoke(() =>
                {
                    var newPivotItem = new PivotItem()
                    {
                        Margin = new Thickness(12, 0, 12, 0),
                    };

                    var content = new NdefPivotItem(MyPivot.Items.Count, r);
                    newPivotItem.Content = content;

                    stringBuilder.Append(content.getAppendString());

                    // ValuesPanel.Children.Add(textBlock);
                    MyPivot.Items.Add(newPivotItem);
                });
            }

        }

        private void InitializePivot()
        {

            Dispatcher.BeginInvoke(() =>
            {
                while (MyPivot.Items.Count > 1)
                {
                    MyPivot.Items.RemoveAt(1);
                }
            });

        }

        private ApplicationBar CreateAppBar()
        {
            var appBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["appbar"];

            var shareButton = new ApplicationBarIconButton();
            shareButton.Text = AppResources.Button_share;
            shareButton.IconUri = new Uri("/Assets/shareButton.png", UriKind.Relative);
            appBar.Buttons.Add(shareButton);
            shareButton.Click += new EventHandler(ShareButton_click);

            return appBar;
        }

        private void ShareButton_click(object sender, EventArgs e)
        {
            ShareText(stringBuilder.ToString());
        }

        private void ShareText(string s)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = AppResources.Title_shareText;
            emailComposeTask.Body = s;
            emailComposeTask.Show();
        }
    }


}