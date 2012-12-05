using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using System.Windows.Controls.Primitives;
using System.IO.IsolatedStorage;
using System.IO;
using System.Text;
using System.Windows.Ink;
using System.Threading;

namespace GetItDone
{
    public partial class MainPage : PhoneApplicationPage
    {
        IsolatedStorageFile listFile = IsolatedStorageFile.GetUserStoreForApplication();
        string sFile = "ListNames.txt";
        IsolatedStorageFile eventFile = IsolatedStorageFile.GetUserStoreForApplication();
        string eFile = "Events.txt";
        EList remList;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Application.Current.Host.Settings.EnableFrameRateCounter = false;
            if (!listFile.FileExists(sFile))//Create a file for storing list names if it doesn't exist
            {
                IsolatedStorageFileStream dataFile = listFile.CreateFile(sFile);
                dataFile.Close();
            }
            if (!eventFile.FileExists(eFile))//Create a file for storing event details if it doesn't exist
            {
                IsolatedStorageFileStream temp = eventFile.CreateFile(eFile);
                temp.Close();
            }
            //Read listNames from the file
            StreamReader reader = new StreamReader(new IsolatedStorageFileStream(sFile, FileMode.Open, listFile));
            string rawData = reader.ReadToEnd();
            reader.Close();

            string[] sep = new string[] { "\r\n" };//Seperate by new line
            string[] arrData = rawData.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            foreach (var d in arrData)
            {
                loadListButton(d);//Add the lists to the main screen
            }
            //Read the events in here, add buttons to the display
            reader = new StreamReader(new IsolatedStorageFileStream(eFile, FileMode.Open, eventFile));
            string data = reader.ReadToEnd();
            reader.Close();
            remList = new EList();
            remList.Recreate(data);
            loadEvents();

        }

        //Create the buttons on the main page for the events
        public void loadEvents()
        {
            LinkedList<Node>.Enumerator looper = remList.loopHelp();
            while (looper.MoveNext())
            {
                HyperlinkButton eventButton = new HyperlinkButton();
                eventButton.Height = 72;
                eventButton.Width = 427;
                eventButton.Margin = new Thickness(5);
                eventButton.Background = new SolidColorBrush(Colors.Red);
                eventButton.Foreground = new SolidColorBrush(Colors.Black);
                string buttonContent1 = looper.Current.getTitle();
                eventButton.Name = buttonContent1;
                string buttonContent2 = looper.Current.getStart();
                string buttonContentFinal = buttonContent1 + " " + buttonContent2;
                eventButton.Content = buttonContentFinal;
                listPanel.Children.Add(eventButton);
                //Add action handler to display information about event when clicked
                eventButton.Click += new RoutedEventHandler(eventButtonGeneralClick);
                //Add action handler to remove the event
                eventButton.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(eventButtonGeneralHold);
            }
        }

        //Create a new list
        private void newListButton_Click(object sender, RoutedEventArgs e)
        {
            //Display a pop up asking for a name for the new list
            Popup getNamePopUp = new Popup();
            getNamePopUp.Height = 300;
            getNamePopUp.Width = 400;
            getNamePopUp.VerticalOffset = 100;
            PopUpUserControl control = new PopUpUserControl();
            getNamePopUp.Child = control;
            getNamePopUp.IsOpen = true;
            control.btnOK.Click += (s, args) =>
                {
                    if (control.tbx.Text.ToString() != "")
                    {
                        //Create a button to show the new list
                        createListButton(control.tbx.Text);
                        getNamePopUp.IsOpen = false;
                    }

                    

                };
            control.btnCancel.Click += (s, args) =>
                {
                    getNamePopUp.IsOpen = false;
                };


        }

        //Create a new reminder
        private void newReminderButton_Click(object sender, RoutedEventArgs e)
        {
            Popup container = new Popup();
            container.Height = 300;
            container.Width = 400;
            container.VerticalOffset = 100;
            WindowsPhoneControl4 content = new WindowsPhoneControl4();
            container.Child = content;
            container.IsOpen = true;
            content.btnOK.Click += (s, args) =>
                {
                    Reminder rem = new Reminder(content.nameTxt.Text.ToString());
                    int day = int.Parse(content.daysTxt.Text);
                    int hours = int.Parse(content.hoursTxt.Text);
                    int minutes = int.Parse(content.minutesTxt.Text);
                    int seconds = int.Parse(content.secondsTxt.Text);
                    rem.Content = content.nameTxt.Text;
                    TimeSpan span = new TimeSpan(day, hours, minutes, seconds);
                    rem.BeginTime = DateTime.Now.Add(span);
                    ScheduledActionService.Add(rem);
                    container.IsOpen = false;                };
            content.btnCancel.Click += (s, args) =>
                {
                    container.IsOpen = false;
                };

        }

        //Create a new list button for a list
        private void createListButton(string listName)
        {
            HyperlinkButton newListDisplay = new HyperlinkButton();
            newListDisplay.Height = 72;
            newListDisplay.Width = 427;
            newListDisplay.Margin = new Thickness(5);
            newListDisplay.Content = listName;
            newListDisplay.Name = listName;
            newListDisplay.IsEnabled = true;
            newListDisplay.Background = new SolidColorBrush(Colors.Blue);
            newListDisplay.Foreground = new SolidColorBrush(Colors.Yellow);
            //Add the button to the main page
            listPanel.Children.Add(newListDisplay);
            //Set its click event handler
            newListDisplay.Click += new RoutedEventHandler(generalButtonClick);
            newListDisplay.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(deleteList);
            //Write the list name to the persistent storage
            StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(sFile, FileMode.Append, listFile));
            writer.WriteLine(listName);
            writer.Close();
             
        }

        //Create list buttons for lists that are stored when the app is launched
        private void loadListButton(string listName)
        {
            HyperlinkButton newListDisplay = new HyperlinkButton();
            newListDisplay.Height = 72;
            newListDisplay.Width = 427;
            newListDisplay.Margin = new Thickness(5);
            newListDisplay.Content = listName;
            newListDisplay.Name = listName;
            newListDisplay.IsEnabled = true;
            newListDisplay.Background = new SolidColorBrush(Colors.Blue);
            newListDisplay.Foreground = new SolidColorBrush(Colors.Yellow);
            //Add the button to the main page
            listPanel.Children.Add(newListDisplay);
            //Set its click event handler
            newListDisplay.Click += new RoutedEventHandler(generalButtonClick);
            newListDisplay.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(deleteList);
        }

        //When a list is selected, show the contents of that list
        private void generalButtonClick(object sender, System.EventArgs e)
        {
            String listName = ((HyperlinkButton)sender).Name;
            //Open Page1.xaml and pass it the listName parameter
            NavigationService.Navigate(new Uri("/GetItDone;component/Page1.xaml?name=" + listName, UriKind.Relative));
        }

        //When a list is held, delete the list
        private void deleteList(object sender, System.EventArgs e)
        {
            //Prompt user to verify
            Popup container = new Popup();
            container.VerticalOffset = 100;
            verifyDelete control = new verifyDelete();
            container.Child = control;
            control.textBlock1.Text = "Are you sure you want to delete this list?";
            container.IsOpen = true;
            container.Opacity = 10;

            control.yesButton.Click += (s, args) =>
            {
                String listName = ((HyperlinkButton)sender).Name;
                //Remove the list entry from the local file
                StreamReader reader = new StreamReader(new IsolatedStorageFileStream(sFile, FileMode.Open, listFile));
                List<String> fileLines = new List<string>();
                while (!reader.EndOfStream)
                {
                    fileLines.Add(reader.ReadLine());
                }
                reader.Close();
                fileLines.Remove(listName);
                StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(sFile, FileMode.Truncate, listFile));
                foreach (var d in fileLines)
                {
                    writer.WriteLine(d);
                }
                writer.Close();
                //Remove the list from the server here

                //Remove the list button from the main page
                listPanel.Children.Remove(((HyperlinkButton)sender));
                container.IsOpen = false;
            };
            control.noButton.Click += (s, args) =>
            {
                container.IsOpen = false;
            };
            



        }

        //Show the about page as a pop-up
        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            Popup container = new Popup();
            AboutControl temp = new AboutControl();
            container.Child = temp;
            container.IsOpen = true;
            container.VerticalOffset = 100;
            String aboutInfo = "Authors : \r\n      Theo Reinke\r\n      Chris King\r\n      Larry Schneck\r\n      Jordan Hardwick\r\n";
            temp.infoBox.Text = aboutInfo;
            //Close the about page when the x button is clicked
            temp.closeButton.Click += (s, args) =>
                {
                    container.IsOpen = false;
                };
            
        }

        //Create a new event
        private void newEventButton_Click(object sender, RoutedEventArgs e)
        {
            //error check number
            int error = 0;
            //Get the details from a popup, add the event to the linked list, write the string to the file
            Popup container = new Popup();
            container.Height = 300;
            container.Width = 400;
            container.VerticalOffset = 100;
            WindowsPhoneControl5 temp = new WindowsPhoneControl5();
            container.Child = temp;
            container.IsOpen = true;
            
            temp.OKbtn.Click += (s, args) =>
                {
                    
                    //Create a new node and add it to the LinkedList
                    String title = temp.titleTxt.Text;  //error code 1
                    if(title == null)
                    {
                        error = error * 10;
                        error = error + 1;
                    }
                    else
                    {
                        if(title.Length < 1)
                        {
                            error = error * 10;
                            error = error + 1;
                        }
                    }
                    String date = temp.dateTxt.Text;
                    String startTimeString = temp.startTimeTxt.Text;
                    String endTimeString = temp.endTimeTxt.Text;
                    DateTime eventDate = new DateTime();
                    try
                    {
                        eventDate = DateTime.ParseExact(date, "MM/dd/yyyy", null);  //error code 2
                    }
                    catch (Exception ex)
                    {
                        error = error * 10;
                        error = error + 2; 
                    }
                    DateTime startTime = new DateTime();
                    try
                    {
                        startTime = DateTime.ParseExact(startTimeString, "HH:mm", null);  //error code 3
                    }
                    catch (Exception ex)
                    {
                        error = error * 10;
                        error = error + 3;
                    }
                    DateTime endTime = new DateTime();
                    try
                    {
                        endTime = DateTime.ParseExact(endTimeString, "HH:mm", null);  //error code 4
                    }
                    catch (Exception ex)
                    {
                        error = error * 10;
                        error = error + 4;
                    }
                    DateTime startTimeDate = new DateTime(eventDate.Year, eventDate.Month, eventDate.Day, startTime.Hour, startTime.Minute, 0);
                    DateTime endTimeDate = new DateTime(eventDate.Year, eventDate.Month, eventDate.Day, endTime.Hour, endTime.Minute, 0);
                    String extra = temp.extraTxt.Text;  //error code 5
                    String detail = temp.extraTxt.Text;  //error code 6
                    System.Console.Write("extra: " + extra);
                    System.Console.Write("detail: " + detail);

                    if(error != 0)
                    {
                    Popup fixError = new Popup();
                    AboutControl page = new AboutControl();
                    fixError.Height = 300;
                    fixError.Width = 400;
                    fixError.VerticalOffset = 100;
                    fixError.Child = page;
                    fixError.IsOpen = true;
                    string errorMessage = "";
                    int code;
                    while (error > 0)
                    {
                        code = error % 10;
                        error = error / 10;
                        switch (code)
                        {
                            case 1:
                                errorMessage = String.Concat("Please enter a title\n", errorMessage);
                                break;
                            case 2:
                                errorMessage = String.Concat("Invalid date\n", errorMessage);
                                break;
                            case 3:
                                errorMessage = String.Concat("Invalid start time\n", errorMessage);
                                break;
                            case 4:
                                errorMessage = String.Concat("Invalid end time\n", errorMessage);
                                break;
                            case 5:
                                errorMessage = String.Concat("Invalid info\n", errorMessage);
                                break;
                            case 6:
                                errorMessage = String.Concat("Invalid description\n", errorMessage);
                                break;
                            default:
                                errorMessage = String.Concat("Unkown error\n", errorMessage);
                                break;
                        }
                    }
                    page.infoBox.Text = errorMessage;
                    page.closeButton.Click += (s1, args1) =>
                        {
                            fixError.IsOpen = false;
                        };
                    container.IsOpen = false;
                    return;
                    }
                    
                            

                    //Add the event to the EList linked list
                    if (remList == null)
                    {
                        remList = new EList();
                    }
                    remList.addEvent((startTimeDate.ToString()), endTimeDate.ToString(), title, 0, extra, detail);
                    
                    //Write backupString to a file for persistent storage
                    string backupString = remList.returnAll();
                    StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(eFile, FileMode.Truncate, eventFile));
                    writer.Write(backupString);
                    writer.Close();
                    //add button on main screen for event, display title and start time
                    HyperlinkButton eventButton = new HyperlinkButton();
                    eventButton.Name = title;
                    eventButton.Height = 72;
                    eventButton.Width = 427;
                    eventButton.Margin = new Thickness(5);
                    eventButton.Content = title + " " + startTimeDate.ToString();
                    eventButton.Background = new SolidColorBrush(Colors.Red);
                    eventButton.Foreground = new SolidColorBrush(Colors.Black);
                    listPanel.Children.Add(eventButton);
                    //Add action handler to display information about event when clicked
                    eventButton.Click+= new RoutedEventHandler(eventButtonGeneralClick);
                    eventButton.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(eventButtonGeneralHold);
                    //create a reminder for the event
                    Reminder rem = new Reminder(title);
                    rem.BeginTime = new DateTime(eventDate.Year, eventDate.Month, eventDate.Day, startTime.Hour, startTime.Minute, 0);
                    rem.Content = detail;
                    ScheduledActionService.Add(rem);
                    container.IsOpen = false;
                };
            temp.cancelBtn.Click += (s, args) =>
                {
                    container.IsOpen = false;
                };
        }
        
        //Remove an event 
        private void eventButtonGeneralHold(object sender, System.EventArgs e)
        {
            string tempy = ((HyperlinkButton)sender).Content.ToString();
            tempy = tempy.Substring(tempy.IndexOf(' ')+1);
            remList.removeEvent(DateTime.Parse(tempy));
            if (ScheduledActionService.Find(((HyperlinkButton)sender).Name.ToString()) != null)
            {
                ScheduledActionService.Remove(((HyperlinkButton)sender).Name.ToString());
            }
            String backup = remList.returnAll();
            StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(eFile, FileMode.Truncate, eventFile));
            writer.Write(backup);
            writer.Close();
            listPanel.Children.Remove(((HyperlinkButton)sender));

        }

        //Show details about event when clicked
        private void eventButtonGeneralClick(object sender, System.Windows.RoutedEventArgs e)
        {
            string tempy = ((HyperlinkButton)sender).Content.ToString();
            tempy = tempy.Substring(tempy.IndexOf(' ')+1);
            string[] tempy1 = remList.returnEventSeg(DateTime.Parse(tempy));
            Popup container = new Popup();
            AboutControl temp = new AboutControl();
            container.Child = temp;
            container.IsOpen = true;
            container.VerticalOffset = 100;
            String aboutInfo = "Title:" + tempy1[0] + "\nDescription:" + tempy1[1]+ "\nStarts:"+tempy1[2]+"\nEnds:"+tempy1[3];
            temp.infoBox.Text = aboutInfo;
            //Close the about page when the x button is clicked
            temp.closeButton.Click += (s, args) =>
                {
                    container.IsOpen = false;
                };
        }

        //Sync events with the server
        private void syncButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string address = "sslab01.cs.purdue.edu";
            const int BACKUP_PORT = 7272;
            SocketClient client = new SocketClient();
            string tempy;
            client.Connect(address, BACKUP_PORT);
            tempy = client.Send("GetInfo|larry|pass");
            tempy = client.Receive();
            remList.Recreate(tempy);
            client.Close();
            //recreate the reminders and write to file
            Reminder rem;
            LinkedList<Node>.Enumerator looper = remList.loopHelp();
            while (looper.MoveNext())
            {
                rem = new Reminder(looper.Current.getTitle());
                rem.BeginTime = DateTime.Parse(looper.Current.getStart());
                rem.Content = looper.Current.getDescription();
                ScheduledActionService.Add(rem);
            }
            string bString = remList.returnAll();
            StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(eFile, FileMode.Truncate, eventFile));
            writer.Write(bString);
            writer.Close();
        }
        private void uploadButton_Click(object sender, RoutedEventArgs e)
        {
            string address = "sslab01.cs.purdue.edu";
            const int BACKUP_PORT = 7272;
            SocketClient client = new SocketClient();
            client.Connect(address, BACKUP_PORT);
            client.Send("UpdateInfo|larry|pass|");
            client.Send(remList.returnAll());
            client.Close();
            //delete local content after pushing to server
            LinkedList<Node>.Enumerator looper = remList.loopHelp();
            while (looper.MoveNext())
            {
                if (ScheduledActionService.Find(looper.Current.getTitle()) != null)
                {
                    ScheduledActionService.Remove(looper.Current.getTitle());
                }
            }
            remList.Recreate("");
            string bString = "";
            StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(eFile, FileMode.Truncate, eventFile));
            writer.Write(bString);
            writer.Close();
        }
    }
    //Node Class
    public class Node
    {
        private DateTime start;
        private DateTime end;
        private string title;
        private string description = null;
        private int type = 0;
        private string html = null;
        private string gps = null;
        private long phone = 0;
        private bool repeat = false;
        private TimeSpan duration;
        public Node(string header, string s, string e)
        {
            title = header;
            start = DateTime.Parse(s);
            end = DateTime.Parse(e);
        }
        public Node(string header, string content, string s, string e)
        {
            title = header;
            description = content;
            start = DateTime.Parse(s);
            end = DateTime.Parse(e);
        }
        public Node(string header, string s, string e, int t, string extra)
        {
            title = header;
            start = DateTime.Parse(s);
            end = DateTime.Parse(e);
            type = t;
            switch (type)
            {
                case 1:
                    html = extra;
                    break;
                case 2:
                    gps = extra;
                    break;
                case 3:
                    phone = Convert.ToInt64(extra);
                    break;
                default:
                    type = 0;
                    break;
            }
        }
        public Node(string header, string content, string s, string e, int t, string extra)
        {
            title = header;
            description = content;
            start = DateTime.Parse(s);
            end = DateTime.Parse(e);
            type = t;
            switch (type)
            {
                case 1:
                    html = extra;
                    break;
                case 2:
                    gps = extra;
                    break;
                case 3:
                    phone = Convert.ToInt64(extra);
                    break;
                default:
                    type = 0;
                    break;
            }
        }
        public Node(string info)
        {
            try
            {
                int x = info.IndexOf("/*/");
                title = info.Substring(0, x);
                info = info.Substring(x + 3);
                x = info.IndexOf("/*/");
                if (x == 0)
                {
                    description = null;
                }
                else
                {
                    description = info.Substring(0, x);
                }
                info = info.Substring(x + 3);
                x = info.IndexOf("/*/");
                start = DateTime.Parse(info.Substring(0, x));
                info = info.Substring(x + 3);
                x = info.IndexOf("/*/");
                end = DateTime.Parse(info.Substring(0, x));
                info = info.Substring(x + 3);
                x = info.IndexOf("/*/");
                type = Convert.ToInt32(info.Substring(0, x));
                info = info.Substring(x + 3);
                x = info.IndexOf("/*/");
                switch (type)
                {
                    case 1:
                        html = info.Substring(0, x);
                        break;
                    case 2:
                        gps = info.Substring(0, x);
                        break;
                    case 3:
                        phone = Convert.ToInt64(info.Substring(0, x));
                        break;
                    default:
                        type = 0;
                        break;
                }
                info = info.Substring(x + 3);
                x = info.IndexOf("/*/");
                repeat = Convert.ToBoolean(info.Substring(0, x));
                info = info.Substring(x + 3);
                x = info.IndexOf("/***/");
                if (repeat)
                {
                    duration = TimeSpan.Parse(info.Substring(0, x));
                }
            }
            catch (Exception e)
            {
                throw (e);
            }

        }
        public string getTitle()
        {
            return title;
        }
        public string getDescription()
        {
            return description;
        }
        public string getStart()
        {
            return start.ToString();
        }
        public string getEnd()
        {
            return end.ToString();
        }
        public int getType()
        {
            return type;
        }
        public string getHTML()
        {
            return html;
        }
        public string getGPS()
        {
            return gps;
        }
        public string getPhone()
        {
            return phone.ToString();
        }
        public bool getRepeat()
        {
            return repeat;
        }
        public int getDuration()
        {
            return Convert.ToInt32(duration.TotalHours);
        }
        public string getInfo()
        {
            string info = string.Copy(title);
            info = string.Concat(info, "/*/");
            if (description != null)
            {
                info = string.Concat(info, description);
            }
            info = string.Concat(info, "/*/");
            info = string.Concat(info, start.ToString());
            info = string.Concat(info, "/*/");
            info = string.Concat(info, end.ToString());
            info = string.Concat(info, "/*/");
            switch (type)
            {
                case 0:
                    info = string.Concat(info, "0/*/");
                    break;
                case 1:
                    info = string.Concat(info, "1/*/");
                    info = string.Concat(info, html);
                    break;
                case 2:
                    info = string.Concat(info, "2/*/");
                    info = string.Concat(info, gps);
                    break;
                case 3:
                    info = string.Concat(info, "3/*/");
                    info = string.Concat(info, phone.ToString());
                    break;
                default:
                    info = string.Concat(info, "/*/");
                    break;
            }
            info = string.Concat(info, "/*/");
            if (repeat)
            {
                info = string.Concat(info, "true");
                info = string.Concat(info, "/*/");
                info = string.Concat(info, duration.ToString());
            }
            else
            {
                info = string.Concat(info, "false");
                info = string.Concat(info, "/*/");
            }
            info = string.Concat(info, "/***/");
            return info;
        }
        public void setRepeat(int d)
        {
            //d id duration in hours
            repeat = true;
            duration = new TimeSpan(d, 0, 0);

        }
        public bool checkRepeat(string s, string e)
        {
            DateTime oStart = DateTime.Parse(s);
            DateTime oEnd = DateTime.Parse(e);
            if (repeat == false)
            {
                int check1, check2, check3, check4;
                int test = DateTime.Compare(start, oStart);
                if (test == 0)
                {
                    return false;
                }
                else if (test > 0)
                {
                    check1 = 1;
                }
                else
                {
                    check1 = -1;
                }
                test = DateTime.Compare(end, oEnd);
                if (test == 0)
                {
                    return false;
                }
                else if (test > 0)
                {
                    check2 = 1;
                }
                else
                {
                    check2 = -1;
                }
                test = DateTime.Compare(start, oEnd);
                if (test == 0)
                {
                    check3 = 0;
                }
                else if (test > 0)
                {
                    check3 = 1;
                }
                else
                {
                    check3 = -1;
                }
                test = DateTime.Compare(end, oStart);
                if (test == 0)
                {
                    check4 = 0;
                }
                else if (test > 0)
                {
                    check4 = 1;
                }
                else
                {
                    check4 = -1;
                }
                if (check1 * check2 == -1 || check1 * check3 == -1 || check1 * check4 == -1)
                {
                    return false;
                }
                return true;
            }
            else
            {
                TimeSpan span = oStart.Subtract(start);
                double hours = span.TotalHours;
                double check1 = hours / duration.TotalHours;
                check1 = Math.Floor(check1);
                span = oEnd.Subtract(end);
                hours = span.TotalHours;
                double check2 = hours / duration.TotalHours;
                check2 = Math.Floor(check2);
                span = oStart.Subtract(end);
                hours = span.TotalHours;
                double check3 = hours / duration.TotalHours;
                check3 = Math.Floor(check3);
                span = oEnd.Subtract(start);
                hours = span.TotalHours;
                double check4 = hours / duration.TotalHours;
                check4 = Math.Floor(check4);
                if (check1 != check2 || check1 != check3 || check1 != check4)
                {
                    return false;
                }
                return true;
            }
        }
        public Node activateRepeat()
        {
            if (repeat == false)
            {
                return null;
            }
            start = start.AddHours(duration.TotalHours);
            end = end.AddHours(duration.TotalHours);
            return this;
        }
    }
    //List Class
    public class EList
{
    //List Class
    //Coded by lschneck, meant to implement a linked list of event nodes
    //could break if linked list is circular
    //or if move next doesn't return null when i expect
    private LinkedList<Node> eventList;
    private DateTime trash1;
    private DateTime trash2;
    public EList()
    {
        eventList = new LinkedList<Node>();
    }
    public int addEvent(string title)
    {
        Node temp = new Node(title);
        //add the event into list sorted by startTime
        if (eventList.Count != 0)
        {
            int j = 0;
            LinkedList<Node>.Enumerator e = eventList.GetEnumerator();
            while (e.MoveNext() && j != 1)
            {
                trash1 = DateTime.Parse(temp.getStart());
                trash2 = DateTime.Parse(e.Current.getStart());
                if (trash1 <= trash2)
                {
                    //check the end time
                    if (!e.Current.checkRepeat(temp.getStart(), temp.getEnd()))
                    {
                        return -1;
                    }
                    //add to non-first and non-last point in list
                    trash1 = DateTime.Parse(e.Current.getStart());
                    trash2 = DateTime.Parse(eventList.First.Value.getStart());
                    if (trash1 != trash2)
                    {
                        eventList.AddBefore(eventList.Find(e.Current), temp);
                        return 1;
                    }
                    else
                    {
                        //add to first point in list
                        eventList.AddFirst(temp);
                        return 1;
                    } 
                }
                j = 1;
            }
            if (j != 1)
            {
                //add to end of list and check the end time
                if (!e.Current.checkRepeat(temp.getStart(), temp.getEnd()))
                {
                    return -1;
                }
                eventList.AddLast(temp);
                return 1;
            }
        }
        eventList.AddFirst(temp);
        return 1; 
    }
    public int addEvent(string istartTime, string iendTime, string title, int type, string extra, string detail)
    {
        DateTime startTime = DateTime.Parse(istartTime);
        DateTime endTime = DateTime.Parse(iendTime);
        //create event based on given data
        trash1 = new DateTime(1, 1, 1, 1, 1, 1);
        trash2 = new DateTime(9, 9, 9, 9, 9, 9);
        Node temp = new Node("EXAMPLE", trash1.ToString(), trash2.ToString());
        if (detail == null && type == 0)
        {
            temp = new Node(title, startTime.ToString(), endTime.ToString());
        }
        else if (type == 0)
        {
            temp = new Node(title, detail, startTime.ToString(), endTime.ToString());
        }
        else if (detail == null)
        {
            temp = new Node(title, startTime.ToString(), endTime.ToString(), type, extra);
        }
        else
        {
            temp = new Node(title, detail, startTime.ToString(), endTime.ToString(), type, extra);
        }
        //add the event into list sorted by startTime
        if (eventList.Count != 0)
        {
            int j = 0;
            LinkedList<Node>.Enumerator e = eventList.GetEnumerator();
            while (e.MoveNext() && j != 1)
            {
                trash1 = DateTime.Parse(temp.getStart());
                trash2 = DateTime.Parse(e.Current.getStart());
                if (trash1 <= trash2)
                {
                    //check the end time
                    if (!e.Current.checkRepeat(temp.getStart(), temp.getEnd()))
                    {
                        return -1;
                    }
                    //add to non-first and non-last point in list
                    trash1 = DateTime.Parse(e.Current.getStart());
                    trash2 = DateTime.Parse(eventList.First.Value.getStart());
                    if (trash1 != trash2)
                    {
                        eventList.AddBefore(eventList.Find(e.Current), temp);
                        return 1;
                    }
                    else
                    {
                        //add to first point in list
                        eventList.AddFirst(temp);
                        return 1;
                    }
                }
                j = 1;
            }
            if (j == 1)
            {
                //add to end of list and check the end time
                if (!e.Current.checkRepeat(temp.getStart(), temp.getEnd()))
                {
                    return -1;
                }
                eventList.AddLast(temp);
                return 1;
            }
        }
        else
        {
            eventList.AddFirst(temp);
            return 1;
        }
        return 1;
    }
    public int removeEvent(DateTime startTime)
    {
        LinkedList<Node>.Enumerator e = eventList.GetEnumerator();
        while (e.MoveNext())
        {
            trash1 = DateTime.Parse(e.Current.getStart());
            if (trash1 == startTime)
            {
                eventList.Remove(e.Current);
                return 1;
            }
        }
        return -1;
    }
    public string returnEvent(DateTime startTime)
    {
        LinkedList<Node>.Enumerator e = eventList.GetEnumerator();
        while (e.MoveNext())
        {
            trash1 = DateTime.Parse(e.Current.getStart());
            if (trash1 == startTime)
            {
                return e.Current.getInfo();
            }
        }
        return "FAILED";
    }
    public string[] returnEventSeg(DateTime startTime)
    {
        //similar to return event but it gives formated output in string array
        LinkedList<Node>.Enumerator e = eventList.GetEnumerator();
        string[] temp = new string[8];
        while (e.MoveNext())
        {
            trash1 = DateTime.Parse(e.Current.getStart());
            if (trash1 == startTime)
            {
                temp[0]=e.Current.getTitle();
                temp[1]=e.Current.getDescription();
                temp[2]=e.Current.getStart();
                temp[3]=e.Current.getEnd();
                temp[4]=e.Current.getType().ToString();
                temp[5]=getExtra(e.Current);
                //if repeats is 1 it repeats else it does not
                if((e.Current.getRepeat()) == true)
                {
                    temp[6]="yes";
                }else
                {
                    temp[6]="no";
                }
                temp[7]=e.Current.getDuration().ToString();
                return temp;
            }
        }
        temp[0]="failed";
        return temp;   
    }
    public string returnAll()
    {
        string temp = "";
        LinkedList<Node>.Enumerator e = eventList.GetEnumerator();
        while (e.MoveNext())
        {
            temp = string.Concat(temp, e.Current.getInfo());
        }
        return temp;
    }
    private Node returnNode(DateTime startTime){
        LinkedList<Node>.Enumerator e = eventList.GetEnumerator();
        while (e.MoveNext())
        {
            trash1 = DateTime.Parse(e.Current.getStart());
            if (trash1 == startTime)
            {
                return e.Current;
            }
        }
        return null;
    }
    public void setRepeat(DateTime startTime, int interval)
    {
        //accepts only interval in hours
        Node temp = returnNode(startTime);
        temp.setRepeat(interval);
        return;
    }
    public void Recreate(string backup)
    {
        //use this to parse the backups into a list again
        string temp = "";
        eventList = new LinkedList<Node>();
        while(backup.IndexOf("/***/") != -1)
        {
            temp = backup.Substring(0,(backup.IndexOf("/***/")+5));
            this.addEvent(temp);
            backup = backup.Remove(0,(backup.IndexOf("/***/")+5));
        }
        return;
    }
    public LinkedList<Node>.Enumerator loopHelp(){
        return eventList.GetEnumerator();
    }
    private string getExtra(Node temp)
    {
        if (temp.getType() == 1)
        {
            return temp.getHTML();
        }
        if (temp.getType() == 2)
        {
            return temp.getGPS();
        }
        if (temp.getType() == 3)
        {
            return temp.getPhone().ToString();
        }
        if (temp.getType() == 0)
        {
            return "none";
        }
        return "none";
    }
    }
    public class SocketClient
    {
        Socket _socket = null;
        static ManualResetEvent _clientDone = new ManualResetEvent(false);
        const int TIMEOUT_MILLISECONDS = 5000;
        const int MAX_BUFFER_SIZE = 2048;
        public SocketClient(){
        }
        public string Receive()
        {
            string response = "Operation Timeout";

            // We are receiving over an established socket connection
            if (_socket != null)
            {
                // Create SocketAsyncEventArgs context object
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = _socket.RemoteEndPoint;

                // Setup the buffer to receive the data
                socketEventArg.SetBuffer(new Byte[MAX_BUFFER_SIZE], 0, MAX_BUFFER_SIZE);

                // Inline event handler for the Completed event.
                // Note: This even handler was implemented inline in order to make this method self-contained.
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
                {
                    if (e.SocketError == SocketError.Success)
                    {
                        // Retrieve the data from the buffer
                        response = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                        response = response.Trim('\0');
                    }
                    else
                    {
                        response = e.SocketError.ToString();
                    }

                    _clientDone.Set();
                });

                // Sets the state of the event to nonsignaled, causing threads to block
                _clientDone.Reset();

                // Make an asynchronous Receive request over the socket
                _socket.ReceiveAsync(socketEventArg);

                // Block the UI thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
                // If no response comes back within this time then proceed
                _clientDone.WaitOne(TIMEOUT_MILLISECONDS);
            }
            else
            {
                response = "Socket is not initialized";
            }

            return response;
        }

        /// <summary>
        /// Closes the Socket connection and releases all associated resources
        /// </summary>
        public void Close()
        {
            if (_socket != null)
            {
                _socket.Close();
            }
        }
        public string Send(string data)
        {
            string response = "Operation Timeout";

            // We are re-using the _socket object that was initialized in the Connect method
            if (_socket != null)
            {
                // Create SocketAsyncEventArgs context object
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();

                // Set properties on context object
                socketEventArg.RemoteEndPoint = _socket.RemoteEndPoint;
                socketEventArg.UserToken = null;

                // Inline event handler for the Completed event.
                // Note: This event handler was implemented inline in order to make this method self-contained.
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
                {
                    response = e.SocketError.ToString();

                    // Unblock the UI thread
                    _clientDone.Set();
                });

                // Add the data to be sent into the buffer
                byte[] payload = Encoding.UTF8.GetBytes(data);
                socketEventArg.SetBuffer(payload, 0, payload.Length);

                // Sets the state of the event to nonsignaled, causing threads to block
                _clientDone.Reset();

                // Make an asynchronous Send request over the socket
                _socket.SendAsync(socketEventArg);

                // Block the UI thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
                // If no response comes back within this time then proceed
                _clientDone.WaitOne(TIMEOUT_MILLISECONDS);
            }
            else
            {
                response = "Socket is not initialized";
            }

            return response;
        }

        public string Connect(string hostName, int portNumber)
        {
            string result = string.Empty;

            // Create DnsEndPoint. The hostName and port are passed in to this method.
            DnsEndPoint hostEntry = new DnsEndPoint(hostName, portNumber);

            // Create a stream-based, TCP socket using the InterNetwork Address Family. 
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Create a SocketAsyncEventArgs object to be used in the connection request
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.RemoteEndPoint = hostEntry;

            // Inline event handler for the Completed event.
            // Note: This event handler was implemented inline in order to make this method self-contained.
            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
            {
                // Retrieve the result of this request
                result = e.SocketError.ToString();

                // Signal that the request is complete, unblocking the UI thread
                _clientDone.Set();
            });

            // Sets the state of the event to nonsignaled, causing threads to block
            _clientDone.Reset();

            // Make an asynchronous Connect request over the socket
            _socket.ConnectAsync(socketEventArg);

            // Block the UI thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
            // If no response comes back within this time then proceed
            _clientDone.WaitOne(TIMEOUT_MILLISECONDS);

            return result;
        }
    }
}












//ALL THIS SHIT IS OUTDATED!!!!!!!!!!!!!!!!!!

/* NOTES and TO-DO
 * -used larry's linked list class to save events
 * -Add sync button
 *  -Check server for pending events
 *  -Prompt user if they want to add pending events
 *      -yes: getInfo() from server, re-create linked list
 *      -no: do nothing
 *      
 * 
Usage:
CreateNewUser|user|password
DeleteUser|user|password
UpdateInfo|user|password|info -When user creates a new event, send entire list string
UpdateInfoWithUser|user|password|info|userToAdd
PendingInfo|user|password -Returns yes or no
AddPendingInfo|user|password -If user clicks yes they want to add pending events to their events
GetInfo|user|password
Exit


*/