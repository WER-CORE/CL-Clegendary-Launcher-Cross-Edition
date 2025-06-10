using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CL;

public partial class DowloadProgress : Window
{
    // Елементи управління для відображення прогресу завантаження
    public Label _ProgressFileDowloadTXT => this.FindControl<Label>("ProgressFileDowloadTXT");
    public Label _ProgressDowloadTXT => this.FindControl <Label>("ProgressDowloadTXT");
    public Label _VersionTXT => this.FindControl<Label>("VersionTXT");
    public Label _FileTXT => this.FindControl<Label>("FileTXT");
    public Label _FileTXTName => this.FindControl<Label>("FileTXTName");
    public ProgressBar _ProgressDowloadVersion => this.FindControl<ProgressBar>("ProgressDowloadVersion");
    public ProgressBar _ProgressDowloadFile => this.FindControl<ProgressBar>("ProgressDowloadFile");

    public DowloadProgress()
    {
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Метод для оновлення прогресу завантаження версії
    public void DowloadProgressBarVersion(int progress, object version)
    {
        _VersionTXT.Content = "Завантажується версія " + version;
        _ProgressDowloadVersion.Value = progress;
        _ProgressDowloadTXT.Content = progress + "%";
    }
    // Метод для оновлення прогресу завантаження файлів
    public void DowloadProgressBarFileTask(int filedowload, int filetotaldowload, string namefile)
    {
        _FileTXTName.Content = $"{namefile}";
        _FileTXT.Content = $"Завантажено {filetotaldowload} з {filedowload}";
    }
    // Метод для оновлення прогресу завантаження файлів у відсотках
    public void DowloadProgressBarFile(int progress)
    {
        _ProgressDowloadFile.Value = progress;
        _ProgressFileDowloadTXT.Content = progress + "%";
    }

}