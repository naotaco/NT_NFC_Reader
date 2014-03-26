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
            PivotItemTitle.Text = "Record " + number;
            ID.Text = record.id;
            Type.Text = record.type;
            Payload_01.Text = record.payload;
        }

    }
}
