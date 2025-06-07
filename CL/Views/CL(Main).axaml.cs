using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using CL.Class;
using ProjBobcat.DefaultComponent.Launch;
using ProjBobcat.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ProjBobcat.Class.Model;
using ProjBobcat.DefaultComponent;
using System.Linq;
using MsBox.Avalonia;
using ProjBobcat.DefaultComponent.Authenticator;
using ProjBobcat.Class;

namespace CL;

public partial class Window1 : Avalonia.Controls.Window
{
    //Елементи
    public ListBox _VersionVanil => this.FindControl<ListBox>("VersionList");
    public Border _SelectVersion => this.FindControl<Border>("SelectVersion");
    public Grid _SelectVersionTypeGird => this.FindControl<Grid>("SelectVersionTypeGird");
    public Border _SelectNow => this.FindControl<Border>("PanelSelectNow");
    public Border _SelectVersionVanila => this.FindControl<Border>("SelectVersionVanila");
    public Image _BackMainWindow => this.FindControl<Image>("BackMainWindow");
    public Label _PlayTXT => this.FindControl<Label>("PlayTXTPanelSelect");
    public Label _PlayTXTMinecraft => this.FindControl<Label>("PlayTXT");
    public Label _ModBuild => this.FindControl<Label>("modbuilds");
    public Label _ModsTXT => this.FindControl<Label>("ModsTXTPanelSelect");
    public Label _ServerTXT => this.FindControl<Label>("ServerTXTPanelSelect");
    public Image _CheckMarkAccount => this.FindControl<Image>("CheckMarkAccount");
    public Panel _PanelManegerAccount => this.FindControl<Panel>("PanelManegerAccount");
    public TextBlock _TextBlockAccountName => this.FindControl<TextBlock>("NameNik");


    public Window1()
    {
        InitializeComponent();
        _CheckMarkAccount.PointerPressed += _CheckMarkAccount_PointerPressed;
        _BackMainWindow.PointerPressed += _BackMainWindow_PointerPressed;
        _SelectVersionVanila.PointerPressed += _SelectVersionVanila_PointerPressed;

        _PlayTXTMinecraft.PointerPressed += async (s, e) => {
            var selectedVersion = _VersionVanil.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedVersion))
            {
                MessageBoxManager.GetMessageBoxStandard("Помилка", "Оберіть версію для запуску.");
                return;
            }

            LaunchMinecraft(selectedVersion);
        };
        _PlayTXT.PointerPressed += (s, e) => { AnimateBorder(0, 0, _SelectNow); };
        _ModBuild.PointerPressed += (s, e) => { AnimateBorder(80, 0, _SelectNow); };
        _ModsTXT.PointerPressed += (s, e) => { AnimateBorder(160, 0, _SelectNow); };
        _ServerTXT.PointerPressed += (s, e) => { AnimateBorder(223, 0, _SelectNow); };
    }

    private async void _SelectVersionVanila_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var animHelper = new AnimationHelper();

        if (_SelectVersion.IsVisible)
        {
            await animHelper.FadeOutAsync(_SelectVersion, 0.3);
            ShowVanillaVersionListAsync();
            return;
        }
        else
        {
            await animHelper.FadeInAsync(_SelectVersion, 0.3);
        }
    }

    private async void _BackMainWindow_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var animHelper = new AnimationHelper();

        if (_SelectVersionTypeGird.IsVisible)
        {
            await animHelper.FadeOutAsync(_SelectVersionTypeGird, 0.3);
            return;
        }
        else
        {
            await animHelper.FadeInAsync(_SelectVersionTypeGird, 0.3);
        }
    }

    private async void _CheckMarkAccount_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var animHelper = new AnimationHelper();

        if (_PanelManegerAccount.IsVisible)
        {
            await animHelper.FadeOutAsync(_PanelManegerAccount, 0.3);
            return;
        }
        else
        {
            await animHelper.FadeInAsync(_PanelManegerAccount, 0.3);
        }
    }
    private IVersionLocator _versionLocator;

    private async Task<List<string>> LoadVanillaVersionsAsync()
    {
        string minecraftPath = MinecraftPathHelper.GetDefaultExtractPath();
        _versionLocator = new DefaultVersionLocator(minecraftPath, Guid.NewGuid())
        {

        };

        var versions = await Task.Run(() => _versionLocator.GetAllGames());

        var vanillaVersions = versions
            .Where(v => v.Id.StartsWith("1.") && !v.Id.Contains("forge") && !v.Id.Contains("fabric"))
            .Select(v => v.Id)
            .ToList();

        return vanillaVersions;
    }
    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);

    }
    private void AnimateBorder(double targetX, double targetY, Control border)
    {
        const int durationMs = 300;
        const int fps = 60;
        int frameCount = durationMs * fps / 1000;
        int currentFrame = 0;

        if (border.RenderTransform is not TranslateTransform transform)
        {
            transform = new TranslateTransform();
            border.RenderTransform = transform;
        }

        double startX = transform.X;
        double startY = transform.Y;
        double deltaX = targetX - startX;
        double deltaY = targetY - startY;

        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1000.0 / fps)
        };

        timer.Tick += (_, _) =>
        {
            currentFrame++;
            double t = (double)currentFrame / frameCount;

            t = 1 - Math.Pow(1 - t, 4);

            transform.X = startX + deltaX * t;
            transform.Y = startY + deltaY * t;

            if (currentFrame >= frameCount)
                timer.Stop();
        };

        timer.Start();
    }
    private async void ShowVanillaVersionListAsync()
    {
        var versions = await LoadVanillaVersionsAsync();
        _VersionVanil.ItemsSource = versions;
    }
    private async void LaunchMinecraft(string version)
    {
        string? javaPath = MinecraftPathHelper.FindJavaPath();
        if (string.IsNullOrWhiteSpace(javaPath))
        {
            await MessageBoxManager.GetMessageBoxStandard("Java не знайдено", "Будь ласка, оберіть правильний виконуваний файл Java.").ShowAsync();
            return;
        }

        string mcPath = MinecraftPathHelper.GetDefaultExtractPath();

        var launchOptions = new LaunchSettings
        {
            Version = version,
            GamePath = mcPath,
            VersionInsulation = false,
            GameResourcePath = mcPath,
            Authenticator = new OfflineAuthenticator
            {
                Username = "Player"
            }
        };

        var core = new ProjBobcat.Class.Launch.GameCore(launchOptions);
        core.Launch();
    }

}