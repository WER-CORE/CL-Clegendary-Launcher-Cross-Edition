using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using CL.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using MsBox.Avalonia;
using System.Collections;
using Avalonia.Platform.Storage;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using CL.Script;
using CmlLib.Core.Version;

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
    public TextBlock _PlayTXTMinecraft => this.FindControl<TextBlock>("PlayTXT");
    public Label _ModBuild => this.FindControl<Label>("modbuilds");
    public Label _ModsTXT => this.FindControl<Label>("ModsTXTPanelSelect");
    public Label _ServerTXT => this.FindControl<Label>("ServerTXTPanelSelect");
    public Image _CheckMarkAccount => this.FindControl<Image>("CheckMarkAccount");
    public Panel _PanelManegerAccount => this.FindControl<Panel>("PanelManegerAccount");
    public TextBlock _TextBlockAccountName => this.FindControl<TextBlock>("NameNik");

    public Window1()
    {
        InitializeComponent(); // Ініціалізація компонентів(завантаження XAML)

        AnimationHelper animationHelper = new AnimationHelper(); // Ініціалізація анімаційного помічника    
        _CheckMarkAccount.PointerPressed += _CheckMarkAccount_PointerPressed; // Обробка натискання на кнопку "CheckMarkAccount" для управління панеллю облікових записів
        _BackMainWindow.PointerPressed += _BackMainWindow_PointerPressed; // Обробка натискання на кнопку "BackMainWindow" для управління панеллю вибору версії
        _SelectVersionVanila.PointerPressed += _SelectVersionVanila_PointerPressed; // Обробка натискання на кнопку "SelectVersionVanila" для управління панеллю вибору версії ванільної гри
        _VersionVanil.SelectionChanged += _VersionVanil_SelectionChanged; ; // Обробка натискання на список версій ванільної гри

        _PlayTXTMinecraft.PointerPressed += async (s, e) =>
        {
            var selectedVersion = _VersionVanil.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedVersion))
            {
                MessageBoxManager.GetMessageBoxStandard("Помилка", "Оберіть версію для запуску.");
                return;
            }

            LaunchMinecraft(selectedVersion);
        }; // Обробка натискання на кнопку "PlayTXT" для запуску Minecraft з обраною версією

        _PlayTXT.PointerPressed +=  (s, e) => { animationHelper.AnimateBorder(0, 0, _SelectNow); }; // Анімація для переміщення бордера до кнопки "PlayTXTPanelSelect"
        _ModBuild.PointerPressed += (s, e) => { animationHelper.AnimateBorder(80, 0, _SelectNow); }; // Анімація для переміщення бордера до кнопки "modbuilds"
        _ModsTXT.PointerPressed += (s, e) => { animationHelper.AnimateBorder(160, 0, _SelectNow); }; // Анімація для переміщення бордера до кнопки "ModsTXTPanelSelect"
        _ServerTXT.PointerPressed += (s, e) => { animationHelper.AnimateBorder(223, 0, _SelectNow); }; // Анімація для переміщення бордера до кнопки "ServerTXTPanelSelect"
    }

    // Обробка зміни вибору версії ванільної гри в ListBox
    private void _VersionVanil_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_VersionVanil.SelectedItem != null)
        {
            var selectedVersion = _VersionVanil.SelectedItem as string;
            _PlayTXTMinecraft.Text = $"ГРАТИ В ({selectedVersion})";
        }
    }
    // Обробка натискання на кнопку "SelectVersionVanila" для приховання/показу панелі вибору версії ванільної гри
    private async void _SelectVersionVanila_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var animHelper = new AnimationHelper();

        if (_SelectVersion.IsVisible)
        {
            await animHelper.FadeOutAsync(_SelectVersion, 0.3);
            return;
        }
        else
        {
            ShowVanillaVersionListAsync();
            await animHelper.FadeInAsync(_SelectVersion, 0.3);
        }
    }

    // Обробка натискання на кнопку "BackMainWindow" для приховання/показу панелі вибору версії і приховання панелі вибору версії ванільної гри
    private async void _BackMainWindow_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var animHelper = new AnimationHelper();

        if (_SelectVersionTypeGird.IsVisible)
        {
            if (_SelectVersion.IsVisible) { await animHelper.FadeOutAsync(_SelectVersion, 0.2); }
            await animHelper.FadeOutAsync(_SelectVersionTypeGird, 0.3);
            return;
        }
        else
        {
            await animHelper.FadeInAsync(_SelectVersionTypeGird, 0.3);
        }
    }

    // Обробка натискання на кнопку "CheckMarkAccount" для приховання/показу панеллю облікових записів
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
    // Ініціалізація компонентів(завантаження XAML)
    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }
    // Метод для отримання списку ванільних версій Minecraft і їх відображення в ListBox
    private async void ShowVanillaVersionListAsync()
    {
        _VersionVanil.Items?.Clear();
        var path = new MinecraftPath(MinecraftPathHelper.GetDefaultExtractPath());
        var launcher = new MinecraftLauncher(path);
        var versions = await launcher.GetAllVersionsAsync();

        foreach (var item in versions)
        {
            _VersionVanil.Items.Add(item.Name);
        }
    }
    // Метод для запуску Minecraft з обраною версією
    private async void LaunchMinecraft(string version)
    {
        DowloadProgress dowloadProgress = new DowloadProgress();
        dowloadProgress.Show();

        var path = new MinecraftPath(MinecraftPathHelper.GetDefaultExtractPath());
        var launcher = new MinecraftLauncher(path);
        System.Net.ServicePointManager.DefaultConnectionLimit = 1000000;

        launcher.FileProgressChanged += (sender, args) =>
        {
            int fileProgress = args.TotalTasks > 0 ? (int)((double)args.ProgressedTasks / args.TotalTasks * 100) : 0;
            dowloadProgress.DowloadProgressBarFileTask(args.TotalTasks, args.ProgressedTasks, args.Name);
            dowloadProgress.DowloadProgressBarVersion(fileProgress, version);
        };

        launcher.ByteProgressChanged += (sender, args) =>
        {
            int byteProgress = args.TotalBytes > 0 ? (int)((double)args.ProgressedBytes / args.TotalBytes * 100) : 0;
            dowloadProgress.DowloadProgressBarFile(byteProgress);
        };

        await launcher.InstallAsync(version);        
        var process = await launcher.InstallAndBuildProcessAsync(version, new MLaunchOption
        {
            Session = MSession.CreateOfflineSession("Gamer123"),
            MaximumRamMb = 2048
        });
        process.Start();

        this.WindowState = WindowState.Minimized;
        dowloadProgress.Close();
        //await DiscordController.UpdatePresence($"Грає версію {version}");
    }
}