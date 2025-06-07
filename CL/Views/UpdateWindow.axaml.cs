using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.IO;
using System.Net;
using System;
using System.Reflection;
using MsBox.Avalonia;
using System.Threading.Tasks;
using System.Net.Http;

namespace CL;

public partial class UpdateWindow : Window
{
    //Елементи
    private ProgressBar ProgreesBarDowloadObj;
    private Label DowoloadMBUpdateObj;
    private Label VersionObj;
    public UpdateWindow()
    {
        InitializeComponent();
        this.Loaded += (s, e) =>
        {
            CheckUpdate();
        };
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        ProgreesBarDowload = this.FindControl<ProgressBar>("ProgreesBarDowload");
        DowoloadMBUpdate = this.FindControl<Label>("DowoloadMBUpdate");
        Version = this.FindControl<Label>("Version");
    }

    private async void CheckUpdate()
    {
        try
        {
            using HttpClient httpClient = new HttpClient();
            string versionToInternet = await httpClient.GetStringAsync("https://drive.usercontent.google.com/u/0/uc?id=1ZjUJGOhNcPXonXPQ4Hks8M5cHxN2CBmv&export=download");

            Version.Content = "Оновлення: " + versionToInternet;

            string extractPath = GetDefaultExtractPath();
            string downloadPath = GetDownloadPath();

            if (!Directory.Exists(extractPath))
            {
                Directory.CreateDirectory(extractPath);
            }

            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(10);

            var response = await client.GetAsync("https://www.dropbox.com/scl/fo/wzxsj7jx3njfalb40cgin/h?rlkey=1ptkt5qi2s86kl38317gbq4bm&dl=1", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var canReportProgress = totalBytes != -1;

            using (var contentStream = await response.Content.ReadAsStreamAsync())
            using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var buffer = new byte[81920];
                long totalRead = 0;
                int read;
                while ((read = await contentStream.ReadAsync(buffer)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, read));
                    totalRead += read;

                    if (canReportProgress)
                    {
                        double progress = (double)totalRead / totalBytes * 100;

                        ProgreesBarDowload.Value = progress;

                        double mbDownloaded = totalRead / (1024.0 * 1024.0);
                        double mbTotal = totalBytes / (1024.0 * 1024.0);
                        DowoloadMBUpdate.Content = $"Завантажено: {mbDownloaded:F2} MB / {mbTotal:F2} MB";
                    }
                }
            }

            Extract(downloadPath, extractPath);
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard("Помилка", "Помилка завантаження оновлення\n" + ex.Message).ShowAsync();
        }
    }
    string GetDefaultExtractPath()
    {
        if (OperatingSystem.IsWindows())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CL(Launcher)");
        }
        else if (OperatingSystem.IsLinux())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "CLLauncher");
        }
        else if (OperatingSystem.IsMacOS())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "CLLauncher");
        }
        else
        {
            MessageBoxManager.GetMessageBoxStandard("!","Невідома операційна система.").ShowAsync();
            return null;
        }
    }
    string GetDownloadPath()
    {
        return Path.Combine(Path.GetTempPath(), "update_cl_launcher.rar");
    }
    async void Extract(string archivePath, string extractPath)
    {
        try
        {
            using (var archive = ArchiveFactory.Open(archivePath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(extractPath, new ExtractionOptions
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }
            File.Delete(archivePath);
            await MessageBoxManager.GetMessageBoxStandard("Успіх", $"Оновлення успішно встановлено! {extractPath}").ShowAsync();
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard("Помилка", "Не вдалося розпакувати оновлення " + ex.Message).ShowAsync();
        }
    }
}