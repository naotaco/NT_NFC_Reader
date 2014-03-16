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
using SonyNdefUtils;
using System.Text;

namespace NFCReader
{
    public partial class MainPage : PhoneApplicationPage
    {


        private ProximityDevice _proximitiyDevice;
        private long _subscriptionIdNdef;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            initNFC();
        }


        private void initNFC()
        {
            // Initialize NFC
            _proximitiyDevice = ProximityDevice.GetDefault();

            if (_proximitiyDevice == null)
            {
                Debug.WriteLine("It seems this is not NFC available device");
                return;
            }

            _subscriptionIdNdef = _proximitiyDevice.SubscribeForMessage("NDEF", NFCMessageReceivedHandler);
            

        }

        private void NFCMessageReceivedHandler(ProximityDevice sender, ProximityMessage message)
        {
            var parser = new SonyNdefParser(message);
            List<SonyNdefRecord> ndefRecords = new List<SonyNdefRecord>();
            ndefRecords = parser.Parse();

            int i = 0;

            foreach (SonyNdefRecord r in ndefRecords)
            {
                var sb = new StringBuilder();
                sb.Append("===== record [");
                sb.Append(i);
                sb.Append("] =====");
                sb.Append(System.Environment.NewLine);
                sb.Append("id: ");
                sb.Append(r.id);
                sb.Append(System.Environment.NewLine);
                sb.Append("type: ");
                sb.Append(r.type);
                sb.Append(System.Environment.NewLine);
                if (r.SonyPayload.Count > 0)
                {
                    int ii = 0;
                    foreach (String s in r.SonyPayload)
                    {
                        sb.Append("   sonyRecord ");
                        sb.Append(ii);
                        sb.Append(": ");
                        sb.Append(System.Environment.NewLine);
                        sb.Append("    ");
                        sb.Append(s);
                        sb.Append(System.Environment.NewLine);
                        ii++;
                    }
                }
                sb.Append("  --- payload raw data start ---");
                sb.Append(System.Environment.NewLine);
                sb.Append(r.payload);
                sb.Append(System.Environment.NewLine);
                sb.Append("  --- payload raw data end   ---");
                sb.Append(System.Environment.NewLine);
                i++;

                Dispatcher.BeginInvoke(() =>
                { 
                    var textBlock = new TextBlock { 
                        Text = sb.ToString(),
                        
                };
                    ValuesPanel.Children.Add(textBlock);
                }
                );
               
                    
            }

            


        }
    }


}