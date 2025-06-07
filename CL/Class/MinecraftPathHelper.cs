using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Class
{
    public static class MinecraftPathHelper
    {
        public static string GetDefaultExtractPath()
        {
            string path;

            if (OperatingSystem.IsWindows())
            {
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CL(Launcher)");
                MessageBoxManager.GetMessageBoxStandard("Увага", $"Windows: {path}").ShowAsync();
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "CLLauncher");
                MessageBoxManager.GetMessageBoxStandard("Увага", $"Linux або MacOS: {path}").ShowAsync();
            }
            else
            {
                _ = MessageBoxManager.GetMessageBoxStandard("Помилка", "Невідома операційна система.").ShowAsync();
                return string.Empty;
            }

            return path;
        }
        public static string? FindJavaPath()
        {
            // 1. JAVA_HOME
            string? javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrWhiteSpace(javaHome))
            {
                var javaFromHome = Path.Combine(javaHome, "bin", GetJavaExecutableName());
                if (File.Exists(javaFromHome))
                    return javaFromHome;
            }

            // 2. Перевірити у PATH
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "which",
                        Arguments = "java",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                if (OperatingSystem.IsWindows())
                {
                    process.StartInfo.FileName = "where";
                }

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(output))
                {
                    var javaPath = output.Split('\n', '\r').FirstOrDefault(x => x.EndsWith(GetJavaExecutableName()));
                    if (File.Exists(javaPath))
                        return javaPath;
                }
            }
            catch { }

            return null;
        }

        public static string GetJavaExecutableName() =>
            OperatingSystem.IsWindows() ? "java.exe" : "java";

    }
}
