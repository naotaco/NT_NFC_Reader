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
        public NdefPivotItem(SonyNdefUtils.SonyNdefRecord record)
        {
            InitializeComponent();
            setRecord(record);
        }

        public void setRecord(SonyNdefUtils.SonyNdefRecord record)
        {
            txtAlertName.Text = record.type;
        }

    }
}
