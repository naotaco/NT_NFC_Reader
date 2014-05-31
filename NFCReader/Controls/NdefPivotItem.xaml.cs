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
using NtNfcLib;


namespace NFCReader
{
    public partial class NdefPivotItem : UserControl
    {

        private StringBuilder stringBuilder;

        public NdefPivotItem(int number, NtNfcLib.NdefRecord record)
        {
            stringBuilder = new StringBuilder();
            InitializeComponent();
            setRecord(number, record);
        }

        public void setRecord(int number, NtNfcLib.NdefRecord record)
        {
            PivotItemTitle.Text = "NDEF Record #" + number;

            AppendToSharingTextAsTitle("-- " + PivotItemTitle.Text + " --");

            // NDEF header in binary
            var ndefHeaderSection = CreateNdefHeaderSection(AppResources.NDEFHeader, Convert.ToString(record.ndefHeader, 2).PadLeft(8, '0'));
            LayoutRoot.Children.Add(ndefHeaderSection);

            // id (if exist)
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

            var tnfTitle = "0x0" + record.typeNameFormat.ToString("x") + " (" + tnf + ")";
            var recordTypeSection = CreateSection(AppResources.TypeNameFormat, tnfTitle);
            LayoutRoot.Children.Add(recordTypeSection);

            // type
            var typeSection = CreateSection(AppResources.Type, record.type);
            LayoutRoot.Children.Add(typeSection);

            // payload as sony's camera's records
            if (record.SonyPayload.Count > 0)
            {
                var sonyPayloadSection = new Section(AppResources.SonyRecordTitle, record.SonyPayload)
                 {
                     Margin = new Thickness(0),
                 };
                sonyPayloadSection.Open();
                LayoutRoot.Children.Add(sonyPayloadSection);
                AppendToSharingTextAsTitle(AppResources.SonyRecordTitle);
                foreach (string s in record.SonyPayload)
                {
                    AppendToSharingText(s);
                }
            }

            // payload in ASCII
            var payloadSection = CreateSection(AppResources.Payload, record.payload);
            LayoutRoot.Children.Add(payloadSection);

            // hex payload
            var hexPayloadSection = new Section(AppResources.HexPayload, CreateHexAsciiStrigCorrection(record.RawPayload))
            {
                Margin = new Thickness(0),
                FontFamily = new System.Windows.Media.FontFamily("Courier New"),
                FontSize = (double)Application.Current.Resources["PhoneFontSizeSmall"],
            };
            hexPayloadSection.Close();
            LayoutRoot.Children.Add(hexPayloadSection);
            AppendToSharingTextAsTitle(AppResources.HexPayload);
            foreach (string s in CreateHexAsciiStrigCorrection(record.RawPayload))
            {
                AppendToSharingText(s);
            }


        }

        private Section CreateSection(String title, String text)
        {
            bool open = false;

            if (text.Length < 1)
            {
                text = "(None)";
                open = false;
            }
            else
            {
                open = true;
            }

            return CreateSection(title, text, open, false);
        }

        private Section CreateSection(string title, string text, bool isOpened)
        {
            if (text.Length < 1)
            {
                text = "(None)";
            }

            return CreateSection(title, text, isOpened, false);
        }

        private Section CreateSection(string title, string text, bool isOpened, bool isMonoSpaceFont)
        {
            Section section;

            if (isMonoSpaceFont)
            {
                section = new Section(title, text)
                {
                    Margin = new Thickness(0),
                    FontFamily = new System.Windows.Media.FontFamily("Courier New"),
                };
            }
            else
            {
                section = new Section(title, text)
                {
                    Margin = new Thickness(0),
                };
            }

            if (isOpened)
            {
                section.Open();
            }
            else
            {
                section.Close();
            }

            AppendToSharingTextAsTitle(title);
            AppendToSharingText(text);

            return section;
        }

        private Section CreateNdefHeaderSection(string title, string text)
        {
            Section section;


            section = new Section(title, text, Section.SpecifiedSectionType.NdefHeader)
            {
                Margin = new Thickness(0),
                FontFamily = new System.Windows.Media.FontFamily("Courier New"),
            };

            section.Open();

            AppendToSharingTextAsTitle(title);
            AppendToSharingText(text);

            return section;
        }

        private string CreateStringFromByteCollection(List<byte> input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in input)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
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
                int count = 0;
                foreach (byte b in line)
                {
                    //hex                    
                    if (count != 0)
                    {
                        hex.Append(" ");
                    }
                    hex.Append(Convert.ToString(b, 16).PadLeft(2, '0'));

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

                    count++;

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

        private void AppendToSharingText(string s)
        {
            stringBuilder.Append(s);
            stringBuilder.Append(System.Environment.NewLine);
        }

        private void AppendToSharingTextAsTitle(string s)
        {
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append(" ** ");
            stringBuilder.Append(s);
            stringBuilder.Append(" **");
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append(System.Environment.NewLine);
        }

        public string getAppendString()
        {
            return stringBuilder.ToString();
        }
    }


}
