using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using NFCReader.Resources;

namespace NFCReader
{
    public partial class Section : UserControl
    {
        public enum SectionStatus
        {
            Closed,
            Opened,
        };

        public enum SpecifiedSectionType
        {
            NdefHeader,
        };

        private SectionStatus status;
        private const double AnimationDuration = 300;

        public Section(String title, String text)
        {
            init();
            Title.Title = title;
            var block = CreateTextBlock(text);
            block.Tap += SetTextToClipboard;
            ContentList.Children.Add(block);
        }

        public Section(String title, List<String> text)
        {
            init();
            Title.Title = title;
            foreach (string s in text)
            {
                var block = CreateTextBlock(s);
                block.Tap += SetTextToClipboard;
                ContentList.Children.Add(block);
            }
        }

        public Section(String title, String text, SpecifiedSectionType type)
        {
            init();
            Title.Title = title;

            switch (type)
            {
                case SpecifiedSectionType.NdefHeader:
                    var block = CreateNdefHeaderGrid(text);
                    block.Tap += SetTextToClipboard;
                    ContentList.Children.Add(block);
                    break;
            }

        }

        public void SetText(String s)
        {
            ContentList.Children.Clear();
            var block = CreateTextBlock(s);
            block.Tap += SetTextToClipboard;
            ContentList.Children.Add(block);
        }

        public void Open()
        {
            Open(1);
        }

        public void Close()
        {
            Close(1);
        }

        private void init()
        {   
            InitializeComponent();

            Title.Tap += LayoutRoot_Tap;

            Close();
        }

        private void Open(double duration)
        {
            status = SectionStatus.Opened;
            Title.DoOpenAnimation(duration);
            ContentList.Visibility = System.Windows.Visibility.Visible;
        }

        private void Close(double duration)
        {
            status = SectionStatus.Closed;
            Title.DoCloseAnimation(duration);
            ContentList.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (status == SectionStatus.Closed)
            {
                Open(AnimationDuration);
            }
            else
            {
                Close(AnimationDuration);
            }
        }

        private TextBlock CreateTextBlock(String text)
        {
            var block = new TextBlock()
            {
                Text = text,
                TextWrapping = System.Windows.TextWrapping.Wrap,
                Margin = new Thickness(48, 6, 12, 6),
            };

            return block;
        }

        private Grid CreateNdefHeaderGrid(String text)
        {
            string[] description = {
                                       "MB",
                                       "ME",
                                       "CF",
                                       "SR",
                                       "IL",
                                       "TNF",
                                   };
    
            var block = new Grid()
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                Margin = new Thickness(40, 3, 40, 3),
                
            };

            block.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(1, GridUnitType.Star),
            });
            block.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(1, GridUnitType.Star),
            });

            for (int i = 0; i < 8; i++)
            {
                block.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star),
                });

                var headerChar = new TextBlock()
                {
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Margin = new Thickness(0,6,0,6),
                    Text = text.ElementAt(i).ToString(),
                };
                block.Children.Add(headerChar);
                Grid.SetRow(headerChar, 1);
                Grid.SetColumn(headerChar, i);
            }
            int column = 0;
            foreach (string str in description)
            {
                var descriptionChar = new TextBlock()
                {
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Margin = new Thickness(0, 6, 0, 6),
                    Text = str
                };
                block.Children.Add(descriptionChar);
                Grid.SetRow(descriptionChar, 0);
                Grid.SetColumn(descriptionChar, column);
                if (str == "TNF")
                {
                    Grid.SetColumnSpan(descriptionChar, 3);
                }
                column++;
            }

            return block;
        }

        private void SetTextToClipboard(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var textBlock = sender as TextBlock;
            Debug.WriteLine("tap: " + textBlock.Text);
            Clipboard.SetText(textBlock.Text);
            MessageBox.Show(AppResources.Message_Copied + System.Environment.NewLine + System.Environment.NewLine + textBlock.Text);
        }
    }
}
