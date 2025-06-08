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
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }
    public void DowloadProgressBarVersion(int progress, object version)
    {
        _VersionTXT.Content = "Завантажується версія " + version;
        _ProgressDowloadVersion.Value = progress;
        _ProgressDowloadTXT.Content = progress + "%";
    }
    public void DowloadProgressBarFileTask(int filedowload, int filetotaldowload, string namefile)
    {
        _FileTXTName.Content = $"{namefile}";
        _FileTXT.Content = $"Завантажено {filetotaldowload} з {filedowload}";
    }
    public void DowloadProgressBarFile(int progress)
    {
        _ProgressDowloadFile.Value = progress;
        _ProgressFileDowloadTXT.Content = progress + "%";
    }

}