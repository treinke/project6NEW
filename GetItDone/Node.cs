using System;

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
    public Node(string header, long s, long e)
    {
        title = header;
        start = new DateTime(s);
        end = new DateTime(e);
    }
    public Node(string header, string content, long s, long e)
    {
        title = header;
        description = content;
        start = new DateTime(s);
        end = new DateTime(e);
    }
    public Node(string header, long s, long e, int t, string extra)
    {
        title = header;
        start = new DateTime(s);
        end = new DateTime(e);
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
    public Node(string header, string content, long s, long e, int t, string extra)
    {
        title = header;
        description = content;
        start = new DateTime(s);
        end = new DateTime(e);
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
    public string getTitle()
    {
        return title;
    }
    public string getDescription()
    {
        return description;
    }
    public long getStart()
    {
        return start.ToBinary();
    }
    public long getEnd()
    {
        return end.ToBinary();
    }
    public string getHTML()
    {
        return html;
    }
    public string getGPS()
    {
        return gps;
    }
    public long getPhone()
    {
        return phone;
    }
    public bool getRepeat()
    {
        return repeat;
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
        long num = start.ToBinary();
        info = string.Concat(info, num.ToString());
        info = string.Concat(info, "/*/");
        num = end.ToBinary();
        info = string.Concat(info, num.ToString());
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
    public bool checkRepeat(long s, long e)
    {
        DateTime oStart = new DateTime(s);
        DateTime oEnd = new DateTime(e);
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