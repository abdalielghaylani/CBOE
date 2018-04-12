using System;
using System.Configuration;
using System.Web;
using CambridgeSoft.ChemFlow;
using System.IO;

/// <summary>
/// Summary description for ServiceLog
/// </summary>
public class ServiceLog
{
    string logFolder = null;
    int expireDays = 30;
    bool disabled = false;
    bool debug = false;

    string logFilename = null;
    Log log = null;

    public Log Log { get { return log; } }

    public string Folder
    {
        get { return logFolder; }
        set { logFolder = value; }
    }

    public int ExpireDays
    {
        get { return expireDays; }
        set { expireDays = value; }
    }

    public bool Disabled
    {
        get { return disabled; }
        set { disabled = value; }
    }

    public bool Debug
    {
        get { return debug; }
        set { debug = value; }
    }

    public bool Add(string ip, Log.Level lvl, string msg)
    {
        if (disabled)
            return false;

        if (log != null)
        {
            FileInfo fi = new FileInfo(logFilename);
            if (fi.Exists)
            {
                DateTime now = DateTime.Now;
                if (fi.CreationTime < new DateTime(now.Year, now.Month, now.Day))
                {
                    log.Close();
                    log = null;
                }
            }
            else
            {
                log = null;
            }
        }

        if (log == null)
        {
            string folder = GetFolder();
            if (folder == null)
                return false;

            DeleteExpiredFiles(folder);

            logFilename = Path.Combine(folder, GetTodayFilename() + ".log");
            log = new Log(Path.Combine(folder, logFilename), false);
        }

        if (log != null)
        {
            log.Add(" [" + ip + "] " + msg, lvl);
            log.Close();
        }

        return log != null;
    }

    string GetTodayFilename()
    {
        return DateTime.Now.ToString("yyyyMMdd");
    }

    void DeleteExpiredFiles(string folder)
    {
        if (expireDays <= 0)
            return;

        DateTime dt = DateTime.Now;
        dt = (new DateTime(dt.Year, dt.Month, dt.Day)) - (new TimeSpan(expireDays, 0, 0, 0));

        string[] files = Directory.GetFiles(folder, "????????.log");
        try
        {
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.CreationTime < dt)
                    fi.Delete();
            }
        }
        catch
        {
        }
    }

    string GetFolder()
    {
        string folder = logFolder;
        if (folder == null || !Directory.Exists(folder))
        {
            try
            {
                Directory.CreateDirectory(folder);
            }
            catch
            {
            }

            if (!Directory.Exists(folder))
            {
                folder = Path.GetPathRoot(Path.GetTempFileName());
                folder = Path.Combine(folder, "ChemScriptLog");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
        }

        return Directory.Exists(folder) ? folder : null;
    }
}
