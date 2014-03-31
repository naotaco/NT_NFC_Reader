using System;
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
        public NdefPivotItem(int number, NdefUtils.NdefRecord record)
        {
            InitializeComponent();
            setRecord(number, record);
        }

        public void setRecord(int number, NdefUtils.NdefRecord record)
        {
            PivotItemTitle.Text = "NDEF Record #" + number;

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



            if (record.SonyPayload.Count > 0)
            {
                var sonyPayloadSection = new Section(AppResources.SonyRecordTitle, record.SonyPayload)
                 {
                     Margin = new Thickness(0),
                 };
                sonyPayloadSection.Open();
                LayoutRoot.Children.Add(sonyPayloadSection);
            }

            var payloadSection = CreateSection(AppResources.Payload, record.payload);
            LayoutRoot.Children.Add(payloadSection);

            var hexPayloadSection = new Section(AppResources.HexPayload, CreateHexAsciiStrigCorrection(record.RawPayload))
            {
                Margin = new Thickness(0),
                FontFamily = new System.Windows.Media.FontFamily("Courier New"),
                FontSize = (double)Application.Current.Resources["PhoneFontSizeSmall"],
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
            var hex = new StringBuilder();
            var ascii = new StringBuilder();
            var ret = new List<string>();

            var list = splitByteCorrection(input, 8);

            foreach (List<byte> line in list)
            {
                foreach (byte b in line)
                {
                    //hex
                    if (b < 16)
                    {
                        hex.Append('0');
                    }
                    hex.Append(Convert.ToString(b, 16));
                    hex.Append(" ");

                    // ascii
                    // if possible to map to character
                    if ((int)b >= 0x20 && (int)b <= 0x7e)
                    {
                        ascii.Append((char)b);
                    }
                    else
                    {
                        ascii.Append(".");
                    }

                }

                string s = hex.ToString() + " " + ascii.ToString() + System.Environment.NewLine;
                Debug.WriteLine("line: " + s);
                ret.Add(s);
                hex.Clear();
                ascii.Clear();

            }

            return ret;
        }

        private List<List<byte>> splitByteCorrection(List<byte> input, int length)
        {
            var ret = new List<List<byte>>();

            if (length < 1)
            {
                return null;
            }

            int count = 0;

            while (count + length - 1 < input.Count)
            {
                var list = new List<byte>();
                list = input.GetRange(count, length);
                ret.Add(list);

                count += length;
            }

            if (input.Count > count)
            {
                // if there're some remaining bytes,
                ret.Add(input.GetRange(count, input.Count - count));
            }

            return ret;
        }

    }


}
