using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using WINFORMS = System.Windows.Forms;
using WPF = System.Windows.Controls;
using mshtml;

namespace All_Log.net
{
        public class Logger
        {
            private bool logon = false;
            private bool isdebug = false;
            private bool extralog = false;
            private bool logtoobject = false;
            private bool logtimestamp = false;
            public bool LogTimestamp
            {
                get { return this.logtimestamp; }
                set { this.logtimestamp = value; }
            }

            private bool logtostdout = false;
            public bool LogToStdOut
            {
                get { return this.logtostdout; }
                set { this.logtostdout = value; }
            }

            private string LogPath = AppDomain.CurrentDomain.BaseDirectory;
            private object logobject = null;
            private Type logobjecttype = null;

            public enum LogType : uint
            {
                Debug = 0,
                Warn = 2,
                Error = 4,
                Verbose = 6,
                Info = 8
            }

            /// <summary>
            /// Constructor simply logs all passed output to Application Directory
            /// Logfile if WritePermission exists
            /// </summary>
            /// <param name="LogOn">Boolean to enable Logging</param>
            /// <param name="isDebug">Boolean to enable Debug Logging
            ///  extremely usefull if special Output in Log is needed</param>
            public Logger(bool LogOn, bool isDebug)
            {
                this.logon = LogOn;
                this.isdebug = isDebug;
            }

            /// <summary>
            /// Like previous Constructor just with extra LogPath
            /// </summary>
            /// <param name="LogOn">Boolean to enable Logging</param>
            /// <param name="isDebug">Boolean to enable Debug Logging
            ///  extremely usefull if special Output in Log is needed</param>
            /// <param name="Path">Path to where Logfiles are stored</param>
            public Logger(bool LogOn, bool isDebug, string Path)
            {
                this.logon = LogOn;
                this.isdebug = isDebug;
                this.LogPath = Path;
            }

            /// <summary>
            /// Constructor to configure Logger with Object Logging
            /// </summary>
            /// <param name="LogOn">Boolean to enable Logging</param>
            /// <param name="isDebug">Boolean to enable Debug Logging
            ///  extremely usefull if special Output in Log is needed</param>
            /// <param name="LogObject">the Object to log to</param>
            /// <param name="ObjectType">the Type of the logging Object
            /// needed to determine Logging style for loggin to RTF or Listbox
            /// </param>
            public Logger(bool LogOn, bool isDebug, object LogObject, Type ObjectType)
            {
                this.logon = LogOn;
                this.isdebug = isDebug;
                this.logtoobject = true;
                this.logobject = LogObject;
                this.logobjecttype = ObjectType;
            }

            /// <summary>
            /// like privious Constructor but with extra Log to Logfile in App
            /// directory if Writepermission exists
            /// </summary>
            /// <param name="LogOn">Boolean to enable Logging</param>
            /// <param name="isDebug">Boolean to enable Debug Logging
            ///  extremely usefull if special Output in Log is needed</param>
            /// <param name="LogObject">the Object to log to</param>
            /// <param name="ObjectType">the Type of the logging Object
            /// needed to determine Logging style for loggin to RTF or Listbox
            /// </param>
            /// <param name="extraLog">Boolean to enable extra Log to
            /// LogFile</param>
            public Logger(bool LogOn, bool isDebug, object LogObject, Type ObjectType, bool extraLog)
            {
                this.logon = LogOn;
                this.isdebug = isDebug;
                this.logtoobject = true;
                this.logobject = LogObject;
                this.logobjecttype = ObjectType;
                this.extralog = extraLog;
            }
            /// <summary>
            /// Like Privious Constructor, just with extra Path where Logfiles are
            /// stored
            /// </summary>
            /// <param name="LogOn">Boolean to enable Logging</param>
            /// <param name="isDebug">Boolean to enable Debug Logging
            ///  extremely usefull if special Output in Log is needed</param>
            /// <param name="LogObject">the Object to log to</param>
            /// <param name="ObjectType">the Type of the logging Object
            /// needed to determine Logging style for loggin to RTF or Listbox
            /// </param>
            /// <param name="extraLog">Boolean to enable extra Log to
            /// LogFile</param>
            /// <param name="extraLogPath">Path to where the Logfiles are
            /// stored</param>
            public Logger(bool LogOn, bool isDebug, object LogObject, Type ObjectType, bool extraLog, string extraLogPath)
            {
                this.logon = LogOn;
                this.isdebug = isDebug;
                this.logtoobject = true;
                this.logobject = LogObject;
                this.logobjecttype = ObjectType;
                this.extralog = extraLog;
                this.LogPath = extraLogPath;
            }

        /// <summary>
        /// Method to Handle Log Command from other Project
        /// </summary>
        /// <param name="LogType">The Type of Logmessage e.g. Error, Warm etc.</param>
        /// <param name="Message">Message which will be written to Log</param>
            public void Log(Logger.LogType LogType, string Message)
            {

                CreateLog(LogType, Message, false);
            }

        /// <summary>
        /// Method to Handle Log Command from Other Project
        /// </summary>
        /// <param name="LogType">The Type of Logmessage e.g. Error, Warm etc.</param>
        /// <param name="Message">Message which will be written to Log</param>
        /// <param name="withColor">Color which the Type will display if Color is useable, e.g. Graphical Console, RichTextBox</param>
            public void Log(Logger.LogType LogType, string Message, bool withColor)
            {
                CreateLog(LogType, Message, withColor);
            }

        /// <summary>
        /// the Method to handle all Info passed to Log Method,
        /// firstly the Console Color is captured for later use. Switch Statement checks 
        /// which type of Log will be written so the right Prefix gets written
        /// to Variable LType. if Timestamp should be saved. this is saved too.
        /// finaly Method checks where to write the log to and fires the 
        /// corresponding Method.
        /// </summary>
        /// <param name="LogType"></param>
        /// <param name="Message"></param>
        /// <param name="withColor"></param>
            private void CreateLog(Logger.LogType LogType, String Message, bool withColor)
            {
                string LType = "";
                ConsoleColor col = ConsoleColor.White;
                switch (LogType)
                {
                    case LogType.Debug:
                        LType = "[DEBUG]";
                        col = ConsoleColor.Yellow;
                        break;
                    case LogType.Error:
                        LType = "[ERROR]";
                        col = ConsoleColor.Red;
                        break;
                    case LogType.Info:
                        LType = "[INFO]";
                        col = ConsoleColor.Green;
                        break;
                    case LogType.Verbose:
                        LType = "[VERBOSE]";
                        col = ConsoleColor.White;
                        break;
                    case LogType.Warn:
                        LType = "[WARN]";
                        col = ConsoleColor.Magenta;
                        break;
                }
                if (this.logtimestamp)
                {
                    DateTime dt = DateTime.Now;
                    Message = dt.ToString() + ":" + dt.Millisecond + LType + ":" + Message;
                }
                else
                {
                    Message = LType + ":" + Message;
                }

                if (this.logtoobject)
                {
                    //Methode aufrufen für LogToObject
                    WriteLogToObject(Message);
                }
                else
                {
                    // Methode aufrufen um Log in Datei zu schreiben
                    WriteLogToFile(Message);
                }
                if (this.extralog)
                {
                    //Methode aufrufen um Extra Log in Datei zu schreiben
                    WriteLogToFile(Message);
                }
                if (this.logtostdout)
                {
                    // methode aufrufen um Meldung auf stdout auszugeben.

                    if (withColor)
                    {
                        WriteLogToStdOut(col, Message);
                    }
                    else
                    {
                        WriteLogToStdOut(Message);
                    }
                }
            }

            /// <summary>
            /// Method to simply write to console Window
            /// </summary>
            /// <param name="Message">The Messasge that will be written 
            /// to console</param>
            private void WriteLogToStdOut(string Message)
            {
                Console.WriteLine(Message);
            }
            /// <summary>
            /// Method to write to console Window, but with selected Color
            /// </summary>
            /// <param name="color">The Color of the written Message</param>
            /// <param name="Message">The Message that will be written to console</param>
            private void WriteLogToStdOut(ConsoleColor color, string Message)
            {
                ConsoleColor c = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(Message);
                Console.ForegroundColor = c;
            }
            /// <summary>
            /// Method to write Message to File
            /// </summary>
            /// <param name="Message">the Message that will be written</param>
            private void WriteLogToFile(string Message)
            {
                FileStream fs = File.OpenWrite(this.LogPath + "Logfile");
                fs.Write(System.Text.Encoding.ASCII.GetBytes(Message), 0, Message.Length);
                fs.Flush();
                fs.Close();
            }
            /// <summary>
            /// Method to write Log Message to passed Object.
            /// Switch statement selects the propper steps to write
            /// the Message to the Object correctly
            /// </summary>
            /// <param name="Message">The Message that will be written</param>
            private void WriteLogToObject(string Message)
            {
                switch (this.logobjecttype.ToString())
                {
                    //wpf
                    case "System.Windows.Controls.ListBox":
                    WPF.ListBox wlb = (WPF.ListBox)this.logobject;
                    WPF.ListBoxItem item = new WPF.ListBoxItem();
                    item.Content = Message;
                    wlb.Items.Add(item);
                        break;
                    case "System.Windows.Controls.TextBox":
                    WPF.TextBox wtb = (WPF.TextBox)this.logobject;
                    wtb.Text += Message;
                        break;
                    case "System.Windows.Controls.RichTextBox":
                    WPF.RichTextBox wrtb = (WPF.RichTextBox)this.logobject;
                    System.Windows.Documents.Run rtbtext = new System.Windows.Documents.Run();
                    System.Windows.Documents.Paragraph p = new System.Windows.Documents.Paragraph();
                    rtbtext.Text = Message;
                    p.Inlines.Add(rtbtext);
                    wrtb.Document.Blocks.Add(p);
                        break;
                    //winforms
                    case "System.Windows.Forms.ListBox":
                        WINFORMS.ListBox lb = (WINFORMS.ListBox)this.logobject;
                        lb.Items.Add(Message);
                        break;
                    case "System.Windows.Forms.TextBox":
                    WINFORMS.TextBox tb = (WINFORMS.TextBox)this.logobject;
                    tb.Text += Message;
                        break;
                    case "System.Windows.Forms.RichTextBox":
                    WINFORMS.RichTextBox rtb = (WINFORMS.RichTextBox)this.logobject;
                    rtb.Text += "\r\n" + Message;
                        break;
                    default:

                        break;

                }
            }
        }
}
