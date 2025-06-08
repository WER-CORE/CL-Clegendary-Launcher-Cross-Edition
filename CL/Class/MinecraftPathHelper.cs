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
    // / Клас для отримання шляху до папки Minecraft, де зберігаються ванільні версії гри і не тількі
    public static class MinecraftPathHelper
    {
        /// <summary>
        /// Отримує шлях до папки Minecraft, де зберігаються ванільні версії гри. І залежить від ОС
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultExtractPath()
        {
                string basePath;

                if (OperatingSystem.IsWindows())
                {
                    basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".ClMinecraft");
                }
                else if (OperatingSystem.IsLinux())
                {
                    basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".clminecraft");
                }
                else if (OperatingSystem.IsMacOS())
                {
                    basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "CLMinecraft");
                }
                else
                {
                    basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".clminecraft");
                }
                MessageBoxManager.GetMessageBoxStandard("Шлях встановлено", $"Новий шлях до лаунчера: {basePath}").ShowAsync();
            return basePath;
        }
    }
}
