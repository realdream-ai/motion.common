using System.Collections;
using System.IO;
using UnityEngine;

namespace RealDream.AI
{
    public partial class LogUtil
    {
        static string _lastLine = "";
        public static bool IsNeedBreak = false;

        public static IEnumerator MonitorFile(string filePath, string prefix = "")
        {
            while (!File.Exists(filePath))
            {
                yield return null;
            }

            FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(filePath))
            {
                Filter = Path.GetFileName(filePath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            watcher.Changed += (s, e) =>
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length > 1)
                {
                    var line = lines[lines.Length - 1];
                    if (line != _lastLine)
                    {
                        _lastLine = line;
                        Debug.Log(prefix + " " + _lastLine);
                    }
                }
            };

            watcher.EnableRaisingEvents = true;

            while (true)
            {
                if (IsNeedBreak)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                    break;
                }
                yield return null;
            }
        }
    }
}