using System;
using CortexAccess;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Text;
using System.Timers;

namespace EEGLogger
{
	class Program
	{
		const string Username = "eegdroneftw";
		const string Password = "Eegdroneftw123";
		const string LicenseId = "316d02a3-025a-41b3-8fbe-edb854926e3b";
		const int DebitNumber = 2; // default number of debit

		const string OutFilePath = @"EEGLogger.csv";
		private static FileStream OutFileStream;
		private static System.Timers.Timer aTimer;
		//public static DateTime startingtime = DateTime.Now;
		public static DateTime startingtime;

		static void Main(string[] args)
		{
			Console.WriteLine("EEG LOGGER");
			Console.WriteLine("Please wear Headset with good signal!!!");

			// Delete Output file if existed
			if (File.Exists(OutFilePath))
			{
				File.Delete(OutFilePath);
			}
			OutFileStream = new FileStream(OutFilePath, FileMode.Append, FileAccess.Write);


			CortexAccess.Process p = new CortexAccess.Process();

			// Register Event
			p.OnEEGDataReceived += OnEEGDataReceived;
			p.SessionCtr.OnSubcribeEEGOK += OnEEGDataReceived;

			Thread.Sleep(10000); //wait for querrying user login, query headset
			if (String.IsNullOrEmpty(p.GetUserLogin()))
			{
				p.Login(Username, Password);
				Thread.Sleep(5000); //wait for logining
			}
			// Show username login
			Console.WriteLine("Username :" + p.GetUserLogin());

			if (p.AccessCtr.IsLogin)
			{
				// Send Authorize
				p.Authorize(LicenseId, DebitNumber);
				Thread.Sleep(5000);

				//DateTime startingtime = DateTime.Now;
				//aTimer = new System.Timers.Timer(2000);
				//Console.WriteLine(startingtime);
				//Console.WriteLine("Start time: {0:HH:mm:ss.fff}",
				//             startingtime);
				//// Hook up the Elapsed event for the timer.
				//aTimer.Elapsed += OnTimedEvent;
				//aTimer.AutoReset = true;
				//aTimer.Enabled = true;
				//wait for authorizing
			}
			if (!p.IsHeadsetConnected())
			{
				p.QueryHeadset();
				Thread.Sleep(10000); //wait for querying headset and create session
			}
			if (!p.IsCreateSession)
			{
				p.CreateSession();
				Thread.Sleep(5000);
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
				p.SubcribeData("eeg");
				//Thread.Sleep(5000);
			}

			Console.WriteLine("Press Enter to exit");
			while (Console.ReadKey().Key != ConsoleKey.Enter) { }

			// Unsubcribe stream
			p.UnSubcribeData("eeg");
			Thread.Sleep(3000);
			// Close Out Stream
			OutFileStream.Dispose();
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

		private static void OnEEGDataReceived(object sender, ArrayList eegData)
		{
			WriteDataToFile(eegData);
		}

		private static void OnTimedEvent(Object source, ElapsedEventArgs e)
		{
			//Console.WriteLine("Time passed: {0:HH:mm:ss.fff}",
			//                 e.SignalTime);
			//Console.WriteLine(e.SignalTime);
			Console.WriteLine(e.SignalTime.Subtract(startingtime));
			// Console.WriteLine("Signal Time: {0:HH:mm:ss.fff}", e.SignalTime);
		}

		// TODO: For subcrible All stream
		//private static void OnMotionDataReceived(object sender, ArrayList eegData)
		//{
		//    //WriteDataToFile(eegData);
		//}
		//private static void OnDevDataReceived(object sender, ArrayList eegData)
		//{
		//    //WriteDataToFile(eegData);
		//}
		//private static void OnMetDataReceived(object sender, ArrayList eegData)
		//{
		//    //WriteDataToFile(eegData);
		//}

	}
}
