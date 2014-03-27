using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace NFCReader
{
    public partial class SectionTitle : UserControl
    {


        private string _title = "";

        public string Title{
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    TitleText.Text = _title;
                }
                
            }
        }

        public SectionTitle()
        {
            InitializeComponent();
        }

        public void DoOpenAnimation(double d)
        {      
            var duration = new Duration(TimeSpan.FromMilliseconds(d));
            var sb = new Storyboard();
            sb.Duration = duration;

            var da = new DoubleAnimation();
            da.Duration = duration;

            sb.Children.Add(da);

            var rt = new RotateTransform();

            Storyboard.SetTarget(da, rt);
            Storyboard.SetTargetProperty(da, new PropertyPath("Angle"));
            da.From = 0;
            da.To = 90;


            Triangle.RenderTransform = rt;
            Triangle.RenderTransformOrigin = new Point(0.5, 0.5);
            sb.Begin();
        }

        public void DoCloseAnimation(double d) 
        {
            var duration = new Duration(TimeSpan.FromMilliseconds(d));
            var sb = new Storyboard();
            sb.Duration = duration;

            var da = new DoubleAnimation();
            da.Duration = duration;

            sb.Children.Add(da);

            var rt = new RotateTransform();

            Storyboard.SetTarget(da, rt);
            Storyboard.SetTargetProperty(da, new PropertyPath("Angle"));
            da.From = 90;
            da.To = 0;

            Triangle.RenderTransform = rt;
            Triangle.RenderTransformOrigin = new Point(0.5, 0.5);
            sb.Begin();
        }

        
    }
}
