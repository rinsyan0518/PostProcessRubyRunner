
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace PostProcessScriptRunner
{
    public class RubyRunner
    {
        private const string kScriptPath = "Assets/PostProcessRubyRunner/Editor/ruby_runner";

        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            var proc = new System.Diagnostics.Process();
            var current = Directory.GetCurrentDirectory();

            var args = new string[] {
                path,
                buildTarget.ToString(),
                Debug.isDebugBuild.ToString(),
                PlayerSettings.productName
            };

            proc.StartInfo.FileName = Path.Combine(current, kScriptPath);
            proc.StartInfo.Arguments = string.Join(" ", args);
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardInput = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;

            var homePath = proc.StartInfo.EnvironmentVariables["HOME"];
            var rbenvDir = string.Format("{0}/.rbenv/shims:{0}/.rbenv/bin", homePath);
            var newPath =
                string.Format("{0}:/usr/local/sbin:/usr/local/bin:{1}",
                    rbenvDir,
                    proc.StartInfo.EnvironmentVariables["PATH"]);
            proc.StartInfo.EnvironmentVariables["PATH"] = newPath;

            proc.Start();

            var so = proc.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(so))
                Debug.Log(so);

            var se = proc.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(se))
                Debug.LogError(se);

            proc.WaitForExit();
        }
    }
}
