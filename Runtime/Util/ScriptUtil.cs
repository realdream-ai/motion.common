using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace RealDream.AI
{
    public static class ScriptUtil
    {
        public static IEnumerator Execute(string rootDir,string scriptPath,string extraPath,Dictionary<string,object> envs, params string[] args)
        {
            string argsString = string.Join(" ", args);
            var fullScriptPath = PathUtil.GetFullPath(scriptPath);
            var workPath = PathUtil.GetFullPath(rootDir);
            Debug.Log($"workdir={workPath} \nrun " + fullScriptPath + " " + argsString);
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fullScriptPath,
                    Arguments = $"{argsString}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = workPath
                }
            };
            string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
            path += extraPath;
            process.StartInfo.EnvironmentVariables["PATH"] = path;
            if (envs != null)
            {
                foreach (var item in envs)
                {
                    process.StartInfo.EnvironmentVariables[item.Key] = item.Value.ToString();
                }
            }

             

            process.OutputDataReceived += (_, args) => Debug.Log(args.Data);
            process.ErrorDataReceived += (_, args) => Debug.LogError(args.Data);
            process.Start();
            Debug.Log("process.Id=" + process.Id);
            process.BeginOutputReadLine();
            while (!process.HasExited)
            {
                yield return null;
            }
            Debug.Log("[Unity] script done");
        }
    }
}