using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace NICE_LOGGER
{
    public enum LogMode
    {
        All = 0,
        Error = 1,
        Warning = 2,
        Info = 3
    }

    public enum LogType
    {
        Error = 1,
        Warning = 2,
        Info = 3
    }

    public enum WindowType
    {
        TextBox = 0,
        RichTextBox = 1,
        ListBox = 2,
        ComboBox = 3,
        None = 4
    }

    public class NiceLogger
    {
        LogMode Mode;
        object Window;
        FileStream fs;
        string aktdate = "";
        bool FileIsOpen = false;
        WindowType WType;
        public NiceLogger(object LogWindow, LogMode mode, WindowType type)
        {
            Window = LogWindow;
            Mode = mode;
            WType = type;
            OpenLogFile();
        }

        public NiceLogger(LogMode mode)
        {
            Mode = mode;
            WType = WindowType.None;
            OpenLogFile();
        }

        void OpenLogFile()
        {
            DateTime DT = DateTime.Today;
            string FileName = DT.ToString("dd.MM.yyyy");
            if (aktdate != FileName)
            {
                if (File.Exists(Application.StartupPath + "\\Logs\\" + aktdate + ".log") && FileIsOpen)
                {
                    while (FileIsOpen)
                    {
                        if (fs.CanWrite)
                        {
                            fs.Close();
                            FileIsOpen = false;
                        }
                    }
                }
            }
            aktdate = FileName;
            if (File.Exists(Application.StartupPath + "\\Logs\\" + FileName + ".log"))
            {
                fs = File.Open(Application.StartupPath + "\\Logs\\" + FileName + ".log", FileMode.Append);
                FileIsOpen = true;
            }
            else
            {
                fs = File.Open(Application.StartupPath + "\\Logs\\" + FileName + ".log", FileMode.CreateNew);
                FileIsOpen = true;
            }
        }

        public void WriteToLog(LogType LT, string LogMessage)
        {
            try
            {
                DateTime DT = DateTime.Today;
                string Datum = DT.ToString("dd.MM.yyyy");

                string CompMessage = "";
                Form Frm;
                bool loglevel = true;

                switch (LT)
                {
                    case LogType.Error:
                        CompMessage = "FEHLER: " + LogMessage + "\r\n";
                        if (Mode != LogMode.Error && Mode != LogMode.All)
                            loglevel = false;
                        break;
                    case LogType.Warning:
                        CompMessage = "WARNUNG: " + LogMessage + "\r\n";
                        if (Mode != LogMode.Warning && Mode != LogMode.All)
                            loglevel = false;
                        break;
                    case LogType.Info:
                        CompMessage = "INFO: " + LogMessage + "\r\n";
                        if (Mode != LogMode.Info && Mode != LogMode.All)
                            loglevel = false;
                        break;
                }



                if (Datum != aktdate)
                {
                    OpenLogFile();
                }

                if (fs.CanWrite)
                {
                    if (WType != WindowType.None)
                    {
                        switch (WType)
                        {
                            case WindowType.RichTextBox:
                                RichTextBox RTB = (RichTextBox)Window;
                                //this.Invoke((MethodInvoker)(() => richTextBox1.Text = richTextBox1.Text + CompMessage));
                                Frm = RTB.FindForm();
                                Frm.Invoke((MethodInvoker)(() => RTB.Text = RTB.Text + CompMessage));
                                break;
                            case WindowType.TextBox:
                                TextBox TB = (TextBox)Window;
                                Frm = TB.FindForm();
                                Frm.Invoke((MethodInvoker)(() => TB.Text = TB.Text + CompMessage));
                                break;
                            case WindowType.ComboBox:
                                ComboBox CB = (ComboBox)Window;
                                Frm = CB.FindForm();
                                Frm.Invoke((MethodInvoker)(() => CB.Items.Add(CompMessage)));
                                break;
                            case WindowType.ListBox:
                                ListBox LB = (ListBox)Window;
                                Frm = LB.FindForm();
                                Frm.Invoke((MethodInvoker)(() => LB.Items.Add(CompMessage)));
                                break;
                            case WindowType.None:

                                break;
                        }
                    }

                    if (loglevel)
                    {
                        fs.WriteAsync(Encoding.ASCII.GetBytes(CompMessage), 0, CompMessage.Length);
                        fs.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (fs.CanWrite)
                {
                    fs.WriteAsync(Encoding.ASCII.GetBytes(ex.Message.ToString()), 0, ex.Message.Length);
                    fs.FlushAsync();
                }
            }
        }

        public void CloseLogFile()
        {
            while (FileIsOpen)
            {
                if (fs.CanWrite)
                {
                    fs.FlushAsync();
                    fs.Close();
                    FileIsOpen = false;
                }
            }
        }
    }


}
