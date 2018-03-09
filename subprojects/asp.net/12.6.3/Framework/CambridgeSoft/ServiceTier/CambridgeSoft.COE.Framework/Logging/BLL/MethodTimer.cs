using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.COELoggingService
{
    [Serializable]
    public class MethodTimer
    {
        
        private int _StartTime;
		private int _StopTime;

		private int StartTime
		{
			get
			{
				return this._StartTime;
			}
			set
			{
				this._StartTime = value;
			}
		}

		private int StopTime
		{
			get
			{
				return this._StopTime;
			}
			set
			{
				this._StopTime = value;
			}
		}

		/// 
		/// Initializes the StopWatch to 0.
		/// 
        public MethodTimer()
		{
			StartTime = 0;
			StopTime = 0;
		}
		
		/// 
		/// Starts the Stopwatch.
		/// 
		public void Start()
		{
			StartTime = 
				DateTime.Now.Hour * 60 * 60 * 1000 +
				DateTime.Now.Minute * 60 * 1000 +
				DateTime.Now.Second * 1000 + 
				DateTime.Now.Millisecond;
			//Console.WriteLine("StartTime: " + StartTime);
		}

		/// 
		/// Stops the StopWatch.
		/// 
		public void Stop()
		{
			StopTime = 
				DateTime.Now.Hour * 60 * 60 * 1000 +
				DateTime.Now.Minute * 60 * 1000 +
				DateTime.Now.Second * 1000 + 
				DateTime.Now.Millisecond;
			//Console.WriteLine("StopTime: " + StopTime);
		}

		/// 
		/// Resets the Stopwatch to 0.
		/// 
		public void Reset()
		{
			StartTime = DateTime.Now.Millisecond;
			StopTime = DateTime.Now.Millisecond;
		}

		/// 
		/// Returns a string containing the elasped time since the Start
		/// of the StopWatch.
		/// 
		/// (If Called after the Stop Method)
		/// Returns a string containing the elasped time between the Start
		/// of the StopWatch and the Stop of the  StopWatch
		/// 
		public string GetTime()
		{
			int CurrentTime;
			float Elasped;

			CurrentTime = 
				DateTime.Now.Hour * 60 * 60 * 1000 +
				DateTime.Now.Minute * 60 * 1000 +
				DateTime.Now.Second * 1000 + 
				DateTime.Now.Millisecond;

			if(StopTime == 0)
				Elasped = (CurrentTime - StartTime)/(float)1000;
			else
				Elasped = (StopTime - StartTime)/(float)1000;
			return Elasped.ToString();
		}
	}

    }

