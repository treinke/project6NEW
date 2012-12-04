using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GetItDone
{
    public partial class WindowsPhoneControl5 : UserControl
    {
        public WindowsPhoneControl5()
        {
            InitializeComponent();
        }

        private void OKbtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void titleTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void titleTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Text = "";
        }

        private void titleTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(titleTxt) && ((TextBox)sender).Text == "")
            {
                titleTxt.Text = "Title";
            }
            if (sender.Equals(startTimeTxt) && ((TextBox)sender).Text == "")
            {
                startTimeTxt.Text = "Start Time(24hr, hh:mm)";
            }
            if (sender.Equals(endTimeTxt) && ((TextBox)sender).Text == "")
            {
                endTimeTxt.Text = "Start Time(24hr, hh:mm)";
            }
            if (sender.Equals(dateTxt) && ((TextBox)sender).Text == "")
            {
                dateTxt.Text = "Date mm/dd/yyyy";
            }
            if (sender.Equals(extraTxt) && ((TextBox)sender).Text == "")
            {
                extraTxt.Text = "Extra";
            }
            if (sender.Equals(detailTxt) && ((TextBox)sender).Text == "")
            {
                detailTxt.Text = "Details";
            }
        }

        private void detailTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
