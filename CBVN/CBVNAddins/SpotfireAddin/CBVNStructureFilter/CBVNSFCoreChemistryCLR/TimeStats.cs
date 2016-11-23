using System.Diagnostics;

namespace CBVNStructureFilter
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeStats
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger("LeadDiscovery");
        private Stopwatch _watch;
        private string _processName;
        
        private long _minTime;
        private string _minRepString;
        private long _maxTime;
        private string _maxRepString;
        private long _lastCalcTime;
        
        private int _lastLogNumCompleted;
        private long _lastLogTime;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="processNameIn"></param>
        public TimeStats(string processNameIn)
        {
            InitClass(processNameIn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processNameIn"></param>
        public void ResetTimeStats(string processNameIn)
        {
            InitClass(processNameIn);
        }

        private void InitClass(string processNameIn)
        {
            _watch = new Stopwatch();
            _processName = processNameIn;

            _minTime = -1;
            _maxTime = -1;
            _lastCalcTime = -1;

            _lastLogNumCompleted = -1;
            _lastLogTime = 0;

            _log.DebugFormat("Time Stats: Started Time Stats for : {0}", _processName);

            _watch.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replicateString"></param>
        public void CalculateMinimumMaximum(string replicateString)
        {
            long elapsedTime = _lastCalcTime > 0 ? _watch.ElapsedMilliseconds - _lastCalcTime : _watch.ElapsedMilliseconds;
            
            _lastCalcTime = _watch.ElapsedMilliseconds;

            if ((_minTime == -1) || (_minTime > elapsedTime))
            {
                _minTime = elapsedTime;
                _minRepString = replicateString;
            }
            if ((_maxTime == -1) || (_maxTime < elapsedTime))
            {
                _maxTime = elapsedTime;
                _maxRepString = replicateString;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalNumberCompleted"></param>
        public void LogInterval(int totalNumberCompleted)
        {
            int totalThisSet = _lastLogNumCompleted > 0 ? totalNumberCompleted - _lastLogNumCompleted : totalNumberCompleted;
            long elapsedThisSet = _watch.ElapsedMilliseconds - _lastLogTime;

            _lastLogNumCompleted = totalNumberCompleted;
            _lastLogTime = _watch.ElapsedMilliseconds;
            _log.DebugFormat("     Time Stats: {0}: {1} operations took {2} ms", _processName, totalThisSet, elapsedThisSet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalCompleted"></param>
        /// <param name="extendedInfo"></param>
        public void LogTotals(int totalCompleted, bool extendedInfo)
        {
            long totalTime = _watch.ElapsedMilliseconds;
            _log.DebugFormat("Time Stats: {0} finished: Completed {1} in {2} ms", _processName, totalCompleted, totalTime);
            if (extendedInfo)
            {
                if (_minTime != -1) _log.DebugFormat("     Minimum time: {0} ({1})", _minTime, _minRepString);
                if (_maxTime != -1) _log.DebugFormat("     Maximum time: {0} ({1})", _maxTime, _maxRepString);
                if (totalCompleted > 0) _log.DebugFormat("     Average time: {0})", totalTime / totalCompleted);
            }
        }
    }
}
