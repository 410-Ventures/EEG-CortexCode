using System;
using System.IO;
using System.Text;
using System.Timers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using CortexAccess;

namespace EEGWriter
{
    class MyProgram
    {
        const string Username = "eegdroneftw";
        const string Password = "Eegdroneftw123";
        const string LicenseId = "316d02a3-025a-41b3-8fbe-edb854926e3b";
        const int DebitNumber = 2; // default number of debit

        const string OutFilePath = @"EEGLogger.csv";
        const string OutFilePath2 = @"MotionLogger.csv";
        private static FileStream OutFileStream;
        private static FileStream OutFileStream2;

        // timer
        private static System.Timers.Timer aTimer;
        public static DateTime startingtime;

        static void Main(string[] args)
        {
            Console.WriteLine("EEG WRITER");
            Console.WriteLine("Please wear Headset with good signal!!!");

            // Delete Output file if existed
            if (File.Exists(OutFilePath))
            {
                File.Delete(OutFilePath);
            }
            if (File.Exists(OutFilePath2))
            {
                File.Delete(OutFilePath2);
            }
            OutFileStream = new FileStream(OutFilePath, FileMode.Append, FileAccess.Write);
            OutFileStream2 = new FileStream(OutFilePath2, FileMode.Append, FileAccess.Write);

            CortexAccess.Process p = new Process();

            // Register Event
            p.OnEEGDataReceived += OnEEGDataReceived;
            p.SessionCtr.OnSubcribeEEGOK += OnEEGDataReceived;
            p.OnMotionDataReceived += OnMotionDataReceived;
            p.SessionCtr.OnSubcribeMotionOK += OnMotionDataReceived;

            Thread.Sleep(1000); //wait for querrying user login, query headset
            if (String.IsNullOrEmpty(p.GetUserLogin()))
            {
                p.Login(Username, Password);
                Thread.Sleep(1000); //wait for logining
            }
            // Show username login
            Console.WriteLine("Username :" + p.GetUserLogin());

            if (p.AccessCtr.IsLogin)
            {
                // Send Authorize
                p.Authorize();
                Thread.Sleep(1000); //wait for authorizing
            }
            if (!p.IsHeadsetConnected())
            {
                p.QueryHeadset();
                Thread.Sleep(1000); //wait for querying headset and create session
            }
            if (!p.IsCreateSession)
            {
                p.CreateSession();
                Thread.Sleep(1000);
            }
            if (p.IsCreateSession)
            {
                // Subcribe Motion data

                Thread.Sleep(1000);
            }
            if (p.IsCreateSession)
            {
                // Subcribe EEG data
                startingtime = DateTime.Now;
                aTimer = new System.Timers.Timer(0012.8);

                Console.WriteLine(startingtime);
                Console.WriteLine("Start time: {0:HH:mm:ss.fff}",
                              startingtime);

                // Hook up the Elapsed event for the timer.
                aTimer.Elapsed += OnTimedEvent;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
                
                p.SubcribeData("mot");
                p.SubcribeData("eeg");
            }

            Console.WriteLine("Press Enter to exit");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }

            aTimer = new System.Timers.Timer(0012.8);
            // Unsubcribe stream
            p.UnSubcribeData("eeg");
            p.UnSubcribeData("mot");
            Thread.Sleep(1000);
            // Close Out Stream
            OutFileStream.Dispose();
            OutFileStream2.Dispose();
            aTimer.Stop();
            aTimer.Dispose();
        }

        // Write Header and Data to File
        private static void WriteDataToFile(ArrayList data)
        {
            int i = 0;
            for (; i < data.Count - 1; i++)
            {
                byte[] val = Encoding.UTF8.GetBytes(data[i].ToString() + ", ");

                if (OutFileStream != null)
                    OutFileStream.Write(val, 0, val.Length);
                else
                    break;
            }
            // Last element
            byte[] lastVal = Encoding.UTF8.GetBytes(data[i].ToString() + "\n");
            if (OutFileStream != null)
                OutFileStream.Write(lastVal, 0, lastVal.Length);
        }
        private static void WriteDataToFile2(ArrayList data)
        {
            int i = 0;
            for (; i < data.Count - 1; i++)
            {
                byte[] val = Encoding.UTF8.GetBytes(data[i].ToString() + ", ");
                if (OutFileStream2 != null)
                    OutFileStream2.Write(val, 0, val.Length);
                else
                    break;
            }
            // Last element
            byte[] lastVal = Encoding.UTF8.GetBytes(data[i].ToString() + "\n");
            if (OutFileStream2 != null)
                OutFileStream2.Write(lastVal, 0, lastVal.Length);
        }

        private static void OnMotionDataReceived(object sender, ArrayList motionData)
        {
            WriteDataToFile(motionData);
        }
        private static void OnEEGDataReceived(object sender, ArrayList eegData)
        {
            WriteDataToFile(eegData);
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine(e.SignalTime.Subtract(startingtime));
            // Console.WriteLine("Signal Time: {0:HH:mm:ss.fff}", e.SignalTime);
        }
    }
}
