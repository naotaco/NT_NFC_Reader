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

            if (record.id.Length < 1)
            {
                IDTitle.Visibility = System.Windows.Visibility.Collapsed;
                ID.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ID.Text = record.id;
            }

            if (record.type.Length < 1)
            {
                TypeTitle.Visibility = System.Windows.Visibility.Collapsed;
                Type.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                Type.Text = record.type;
            }

            if (record.payload.Length < 1)
            {
                PayloadTitle.Visibility = System.Windows.Visibility.Collapsed;
                Payload_01.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                Payload_01.Text = record.payload;
            }

            if (record.SonyPayload.Count == 0)
            {
                SonyPayloadTitle.Visibility = System.Windows.Visibility.Collapsed;
                SonyPayload.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                var sb = new StringBuilder();
                foreach (String s in record.SonyPayload)
                {
                    sb.Append(s);
                    sb.Append(System.Environment.NewLine);
                }
                SonyPayload.Text = sb.ToString();
            }
        }

    }
}
