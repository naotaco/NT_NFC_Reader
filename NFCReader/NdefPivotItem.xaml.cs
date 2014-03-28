﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using System.Text;
using NFCReader.Resources;
using System.Diagnostics;

namespace NFCReader
{
    public partial class NdefPivotItem : UserControl
    {
        public NdefPivotItem(int number, SonyNdefUtils.SonyNdefRecord record)
        {
            InitializeComponent();
            setRecord(number, record);
        }

        public void setRecord(int number, SonyNdefUtils.SonyNdefRecord record)
        {
            PivotItemTitle.Text = "Record #" + number;

            var idSection = CreateSection(AppResources.ID, record.id);
            LayoutRoot.Children.Add(idSection);

            // Type name format
            string tnf = "";
            switch (record.typeNameFormat)
            {
                case 0x0:
                    tnf = "Empty";
                    break;
                case 0x1:
                    tnf = "NFC Forum well-known type";
                    break;
                case 0x2:
                    tnf = "MIME, Media-type (RFC 2046)";
                    break;
                case 0x3:
                    tnf = "Absolute URI (RFC 3986)";
                    break;
                case 0x4:
                    tnf = "NFC Forum external type";
                    break;
                case 0x5:
                    tnf = "Unknown";
                    break;
                case 0x6:
                    tnf = "Unchanged";
                    break;
                case 0x7:
                    tnf = "Reserved";
                    break;
            }

            var recordTypeSection = CreateSection(AppResources.TypeNameFormat, "0x0" + record.typeNameFormat.ToString("x") + " (" + tnf + ")");
            LayoutRoot.Children.Add(recordTypeSection);

            var typeSection = CreateSection(AppResources.Type, record.type);
            LayoutRoot.Children.Add(typeSection);

            var payloadSection = CreateSection(AppResources.Payload, record.payload);
            LayoutRoot.Children.Add(payloadSection);

            if (record.SonyPayload.Count > 0)
            {
                var sonyPayloadSection = new Section(AppResources.SonyRecordTitle, record.SonyPayload)
                 {
                     Margin = new Thickness(0),
                 };
                sonyPayloadSection.Open();
                LayoutRoot.Children.Add(sonyPayloadSection);
            }

            var hexPayloadSection = new Section(AppResources.HexPayload, CreateHexAsciiStrigCorrection(record.RawPayload))
            {
                Margin = new Thickness(0),
            };
            hexPayloadSection.Close();
            LayoutRoot.Children.Add(hexPayloadSection);
           

        }

        private Section CreateSection(String title, String text)
        {
            var section = new Section(title, text)
            {
                Margin = new Thickness(0),
            };

            if (text.Length < 1)
            {
                section.SetText("(None)");
                section.Close();
            }
            else
            {
                section.Open();
            }

            return section;
        }

        private string CreateStringFromByteCollection(List<byte> input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in input)
            {
                if (b < 16)
                {
                    sb.Append('0');
                }
                sb.Append(Convert.ToString(b, 16));
                sb.Append(" ");
            }

            return sb.ToString();
        }

        private List<string> CreateHexAsciiStrigCorrection(List<byte> input)
        {
            int count = 1;
            var hex = new StringBuilder();
            var ascii = new StringBuilder();
            var ret = new List<string>();

            foreach (byte b in input)
            {
                //hex
                if (b < 16)
                {
                    hex.Append('0');
                }
                hex.Append(Convert.ToString(b, 16));
                hex.Append(" ");

                // ascii
                // if ascii
                if ((int)b >= 0x20 && (int)b <= 0x7e)
                {
                    ascii.Append((char)b);
                }
                else
                {
                    ascii.Append(".");
                }

                if (count % 8 == 0)
                {
                    string s = hex.ToString() + " " + ascii.ToString() + System.Environment.NewLine;
                    Debug.WriteLine(s);
                    ret.Add(s);
                    hex.Clear();
                    ascii.Clear();
                }

                count++;
            }

            return ret;
        }

    }

    
}
