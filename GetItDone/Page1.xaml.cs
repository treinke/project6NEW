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
using Microsoft.Phone.Controls;
using System.Windows.Controls.Primitives;
using System.IO.IsolatedStorage;
using System.IO;

namespace GetItDone
{
    public partial class listPage : PhoneApplicationPage
    {
        IsolatedStorageFile contentFile = IsolatedStorageFile.GetUserStoreForApplication();
        string listName;
        //Constructor
        public listPage()
        {
            InitializeComponent();

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            listName = NavigationContext.QueryString["name"];
            PageTitle.Text = listName;
            if (!contentFile.FileExists(listName + ".txt"))//Create a file for storing list names if it doesn't exist
            {
                IsolatedStorageFileStream dataFile = contentFile.CreateFile(listName + ".txt");
                dataFile.Close();
            }
            //Call function to get list items from the server here
            StreamReader reader = new StreamReader(new IsolatedStorageFileStream(listName + ".txt", FileMode.Open, contentFile));
            string rawData = reader.ReadToEnd();
            reader.Close();

            string[] sep = new string[] { "\r\n" };//Seperate by new line
            string[] arrData = rawData.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            foreach (var d in arrData)
            {
                addElement(d);
            }

        }

        private void addElement(string d)
        {
            Button btn = new Button();
            btn.Height = 72;
            btn.Width = 427;
            btn.Margin = new Thickness(5);
            btn.Content = d;
            btn.Name = d;
            btn.IsEnabled = true;
            btn.Background = new SolidColorBrush(Colors.Blue);
            btn.Foreground = new SolidColorBrush(Colors.Yellow);
            btn.Opacity = 10;
            //Add the element to the list page
            listElementContainer.Children.Add(btn);
            //Add event handlers for elements
            btn.Click += new RoutedEventHandler(generalElementClick);//Need to check for and prevent duplicate element names
            btn.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(generalElementClick);
        }

        private void addElementButton_Click(object sender, RoutedEventArgs e)
        {
            //Add elements
            if (newItemBox.Text.ToString() != "")
            {
                string newItem = newItemBox.Text;
                //Create a new list element
                Button btn = new Button();
                btn.Height = 72;
                btn.Width = 427;
                btn.Margin = new Thickness(5);
                btn.Content = newItem;
                btn.Name = newItem;
                btn.IsEnabled = true;
                btn.Background = new SolidColorBrush(Colors.Blue);
                btn.Foreground = new SolidColorBrush(Colors.Yellow);
                btn.Opacity = 10;
                //Add the element to the list page
                listElementContainer.Children.Add(btn);
                //Add event handlers for elements
                btn.Click += new RoutedEventHandler(generalElementClick);//Need to check for and prevent duplicate element names
                btn.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(generalElementClick);
                //Write element to file
                StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(listName + ".txt", FileMode.Append, contentFile));
                writer.WriteLine(newItem);
                writer.Close();
                newItemBox.Text = "Add";
            }
        }

        //When a list element is pressed
        private void generalElementClick(object sender, RoutedEventArgs e)
        {
            //Remove element and delete the entry from the file
            String elementName = ((Button)sender).Name;
            //Remove the list entry from the local file
            StreamReader reader = new StreamReader(new IsolatedStorageFileStream(listName + ".txt", FileMode.Open, contentFile));
            List<String> fileLines = new List<string>();
            while (!reader.EndOfStream)
            {
                fileLines.Add(reader.ReadLine());
            }
            reader.Close();
            fileLines.Remove(elementName);
            StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(listName + ".txt", FileMode.Truncate, contentFile));
            foreach (var d in fileLines)
            {
                writer.WriteLine(d);
            }
            writer.Close();

            //Remove the list button from the main page
            listElementContainer.Children.Remove(((Button)sender));

        }

        //When a list element is held
        private void generalElementHold(object sender, RoutedEventArgs e)
        {

        }

        private void newItemBox_GotFocus(object sender, RoutedEventArgs e)
        {
            newItemBox.Text = "";
        }

        private void newItemBox_Lostfocus(object sender, RoutedEventArgs e)
        {
            if (newItemBox.Text == "")
            {
                newItemBox.Text = "Add";
            }
        }

    }

}