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

        }
    }


}