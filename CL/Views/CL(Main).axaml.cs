using Avalonia.Controls;
using CL.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using MsBox.Avalonia;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Diagnostics;
using CL.CustomItem;
using CmlLib.Core.Auth.Microsoft;
using Newtonsoft.Json;
using MsBox.Avalonia.Enums;
using Avalonia.Input;
using System.Text.RegularExpressions;

namespace CL;

public partial class Window1 : Avalonia.Controls.Window
{

    #region Елементи керування
    public ListBox _ListAccount => this.FindControl<ListBox>("ListAccount")!;
    public ListBox _VersionVanil => this.FindControl<ListBox>("VersionList")!;
    public ListBox _VersionMinecraftChangeLog => this.FindControl<ListBox>("VersionMinecraftChangeLog")!;
    public Border _SelectVersion => this.FindControl<Border>("SelectVersion")!;
    public Grid _SelectVersionTypeGird => this.FindControl<Grid>("SelectVersionTypeGird")!;
    public Grid _GirdFormAccountAdd => this.FindControl<Grid>("GirdFormAccountAdd")!;
    public Grid _GirdSelectAccountType => this.FindControl<Grid>("GirdSelectAccountType")!;
    public Grid _GirdOnlineMode => this.FindControl<Grid>("GirdOnlineMode")!;
    public Grid _GirdOfflineMode => this.FindControl<Grid>("GirdOfflineMode")!;
    public Border _SelectNow => this.FindControl<Border>("PanelSelectNow")!;
    public Border _SelectVersionVanila => this.FindControl<Border>("SelectVersionVanila")!;
    public Border _SelectVersionFabric => this.FindControl<Border>("SelectVersionFabric")!;
    public Border _SelectVersionForge => this.FindControl<Border>("SelectVersionForge")!;
    public Border _SelectVersionQuilt => this.FindControl<Border>("SelectVersionQuilt")!;

    public Border _CreateAccount_Offline => this.FindControl<Border>("CreateAccount_Offline")!;
    public Border _CreateAccount_Online => this.FindControl<Border>("CreateAccount_Online")!;
    public Image _BackMainWindow => this.FindControl<Image>("BackMainWindow")!;
    public Image _IconAccount => this.FindControl<Image>("IconAccount")!;
    public Image _CheckMarkAccount => this.FindControl<Image>("CheckMarkAccount")!;
    public Image _OfflineAccount => this.FindControl<Image>("OfflineAccount")!;
    public Image _OnlineAccount => this.FindControl<Image>("MicrosoftAccount")!;
    public Label _PlayTXT => this.FindControl<Label>("PlayTXTPanelSelect")!;
    public TextBlock _PlayTXTMinecraft => this.FindControl<TextBlock>("PlayTXT")!;
    public Label _ModBuild => this.FindControl<Label>("modbuilds")!;
    public Label _ModsTXT => this.FindControl<Label>("ModsTXTPanelSelect")!;
    public Label _ServerTXT => this.FindControl<Label>("ServerTXTPanelSelect")!;
    public Label _TextBlockAccountName => this.FindControl<Label>("NameNik")!;
    public Label _AddProfile => this.FindControl<Label>("AddProfile")!;
    public TextBox _SearchSystemTXT1 => this.FindControl<TextBox>("SearchSystemTXT1")!;
    public TextBox _SearchSystemTXT2 => this.FindControl<TextBox>("SearchSystemTXT2")!;
    public TextBox _NameNikManeger => this.FindControl<TextBox>("NameNikManeger")!;
    public Panel _PanelManegerAccount => this.FindControl<Panel>("PanelManegerAccount")!;
    public CheckBox _Snapshot => this.FindControl<CheckBox>("Snapshots")!;
    public CheckBox _Relesed => this.FindControl<CheckBox>("Relesed")!;
    public CheckBox _Beta => this.FindControl<CheckBox>("Beta")!;
    public CheckBox _Alpha => this.FindControl<CheckBox>("Alpha")!;
    #endregion

    // Ліцензія
    private bool MicosoftAccount;
    JELoginHandler loginHandler;
    MSession session;
    
    public Window1()
    {
        InitializeComponent(); // Ініціалізація компонентів(завантаження XAML)

        _CheckMarkAccount.PointerPressed += _CheckMarkAccount_PointerPressed; // Обробка натискання на кнопку "CheckMarkAccount" для управління панеллю облікових записів
        _BackMainWindow.PointerPressed += _BackMainWindow_PointerPressed; // Обробка натискання на кнопку "BackMainWindow" для управління панеллю вибору версії
        
        _SelectVersionVanila.PointerPressed += _SelectVersionVanila_PointerPressed; // Обробка натискання на кнопку "SelectVersionVanila" для управління панеллю вибору версії ванільної гри
        _VersionVanil.SelectionChanged += _VersionVanil_SelectionChanged; ; // Обробка натискання на список версій ванільної гри
        _SearchSystemTXT1.TextChanged += (s, e) => ShowVanillaVersionListAsync(); // Обробка зміни тексту в полі пошуку версій ванільної гри

        _Snapshot.IsCheckedChanged += ChangeCheckBoxVersionFilter_IsCheckedChanged; ; // Обробка наведення курсора на чекбокси фільтрації версій
        _Relesed.IsCheckedChanged += ChangeCheckBoxVersionFilter_IsCheckedChanged; // Обробка наведення курсора на чекбокси фільтрації версій
        _Beta.IsCheckedChanged += ChangeCheckBoxVersionFilter_IsCheckedChanged; // Обробка наведення курсора на чекбокси фільтрації версій
        _Alpha.IsCheckedChanged += ChangeCheckBoxVersionFilter_IsCheckedChanged; // Обробка наведення курсора на чекбокси фільтрації версій

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

        _PlayTXT.PointerPressed +=  (s, e) => { AnimationHelper.AnimateBorder(0, 0, _SelectNow); }; // Анімація для переміщення бордера до кнопки "PlayTXTPanelSelect"
        _ModBuild.PointerPressed += (s, e) => { AnimationHelper.AnimateBorder(80, 0, _SelectNow); }; // Анімація для переміщення бордера до кнопки "modbuilds"
        _ModsTXT.PointerPressed += (s, e) => { AnimationHelper.AnimateBorder(160, 0, _SelectNow); }; // Анімація для переміщення бордера до кнопки "ModsTXTPanelSelect"
        _ServerTXT.PointerPressed += (s, e) => { AnimationHelper.AnimateBorder(223, 0, _SelectNow); }; // Анімація для переміщення бордера до кнопки "ServerTXTPanelSelect"
        
        _VersionMinecraftChangeLog.SelectionChanged += _VersionMinecraftChangeLog_SelectionChanged; // Обробка зміни вибору версії Minecraft(changelog) в ListBox

        _GirdFormAccountAdd.PointerPressed += _GirdFormAccountAdd_PointerPressed; // Обробка натискання на форму додавання профілю для її закриття
        _AddProfile.PointerPressed += _AddProfile_PointerPressed; // Обробка натискання на кнопку "AddProfile" для відкриття форми додавання профілю

        _CreateAccount_Offline.PointerPressed += _CreateAccount_Offline_PointerPressed; // Обробка натискання на кнопку "CreateAccount_Offline" перевірка поля з ніком і створення офлайн-акаунту
        _CreateAccount_Online.PointerPressed += _CreateAccount_Online_PointerPressed; ; // Обробка натискання на кнопку "CreateAccount_Online" відкриття форми для підключення до онлайн-акаунту
        _OnlineAccount.PointerPressed += _OnlineAccount_PointerPressed; // Обробка натискання на кнопку "OnlineAccount" для перемикання на онлайн-акаунт
        _OfflineAccount.PointerPressed += _OfflineAccount_PointerPressed; // Обробка натискання на кнопку "OfflineAccount" для перемикання на офлайн-акаунт
        
        LoadChangeLogMinecraft(); // Завантаження списку версій Minecraft(changelog) для відображення в ListBox
        LoadProfilesAndAddToListBox(); // Завантаження профілів з файлу і додавання їх до ListBox
    }

    private void ChangeCheckBoxVersionFilter_IsCheckedChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ShowVanillaVersionListAsync();
    }

    private async void _GirdFormAccountAdd_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        await AnimationHelper.FadeOutAsync(_GirdOfflineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdOnlineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdFormAccountAdd, 0.2); await AnimationHelper.FadeOutAsync(_GirdSelectAccountType, 0.2);
    }

    // Обробка натискання на кнопку "CreateAccount_Online" відкриття форми для підключення до онлайн-акаунту
    private async void _CreateAccount_Online_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        try
        {
            loginHandler = JELoginHandlerBuilder.BuildDefault();
            var sessionMicrosoft = await loginHandler.Authenticate();

            MicosoftAccount = true;
            SettingsManager.Settings.MicrosoftAccount = MicosoftAccount;
            SettingsManager.SaveSettings();

            ProfileItem profileItem = new ProfileItem
            {
                NameAccount = sessionMicrosoft.Username,
                UUID = sessionMicrosoft.UUID,
                ImageUrl = $"https://mc-heads.net/avatar/{sessionMicrosoft.UUID}".ToString(),
                AccessToken = sessionMicrosoft.AccessToken,
                OfficalAccount = true,
            };
            await AnimationHelper.FadeOutAsync(_GirdOfflineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdOnlineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdFormAccountAdd, 0.2); await AnimationHelper.FadeOutAsync(_GirdSelectAccountType, 0.2);
            SaveProfile(profileItem);
            LoadProfilesAndAddToListBox();
        }
        catch (Exception ex)
        {
            MicosoftAccount = false;
            SettingsManager.Settings.MicrosoftAccount = MicosoftAccount;
            SettingsManager.SaveSettings();

            MessageBoxManager.GetMessageBoxStandard("!",$"Помилка входу в Microsoft {ex.Message}",ButtonEnum.Ok,MsBox.Avalonia.Enums.Icon.Error);
            await AnimationHelper.FadeOutAsync(_GirdOfflineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdOnlineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdFormAccountAdd, 0.2); await AnimationHelper.FadeOutAsync(_GirdSelectAccountType, 0.2);
        }
    }
    // Обробка натискання на кнопку "CreateAccount_Offline" перевірка поля з ніком і створення офлайн-акаунту
    private async void _CreateAccount_Offline_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        try
        {
            string uuid = Guid.NewGuid().ToString();

            ProfileItem profileItem = new ProfileItem
            {
                NameAccount = _NameNikManeger.Text,
                UUID = uuid,
                AccessToken = "-",
                ImageUrl = $"https://mc-heads.net/avatar/".ToString(),
                OfficalAccount = false
            };
            await AnimationHelper.FadeOutAsync(_GirdOfflineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdOnlineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdFormAccountAdd, 0.2); await AnimationHelper.FadeOutAsync(_GirdSelectAccountType, 0.2);
            SaveProfile(profileItem);
            LoadProfilesAndAddToListBox();
        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard("!", $"Помилка створення офлайн акаунту {ex.Message}", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            await AnimationHelper.FadeOutAsync(_GirdOfflineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdOnlineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdFormAccountAdd, 0.2); await AnimationHelper.FadeOutAsync(_GirdSelectAccountType, 0.2);
        }
    }

    // Обробка натискання на кнопку "OfflineAccount" для перемикання на офлайн-акаунт
    private async void _OfflineAccount_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        await AnimationHelper.FadeOutAsync(_GirdOnlineMode, 0.2);
        await AnimationHelper.FadeInAsync(_GirdOfflineMode, 0.5);
    }
    // Обробка натискання на кнопку "OnlineAccount" для перемикання на онлайн-акаунт
    private async void _OnlineAccount_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        await AnimationHelper.FadeOutAsync(_GirdOfflineMode, 0.2);
        await AnimationHelper.FadeInAsync(_GirdOnlineMode, 0.5);
    }

    // Обробка натискання на кнопку "AddProfile" для відкриття форми додавання профілю
    private async void _AddProfile_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        await AnimationHelper.FadeInAsync(_GirdFormAccountAdd, 0.3); await AnimationHelper.FadeInAsync(_GirdOfflineMode, 0.3); await AnimationHelper.FadeInAsync(_GirdSelectAccountType, 0.3);

        string directoryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        string profilesManegerPath = System.IO.Path.Combine(directoryPath, "ProfilesManeger.json");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(profilesManegerPath) == false)
        {
            using (FileStream fs = File.Create(profilesManegerPath))
            {
                fs.Close();
            }
        }

    }

    // Обробка натискання на кнопку "VersionMinecraftChangeLog" для відкриття вікі сттаті за версію Minecraft(changelog)
    private async void _VersionMinecraftChangeLog_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_VersionMinecraftChangeLog.IsPointerOver)
            return;

        if (_VersionMinecraftChangeLog.SelectedItem is string selectedVersion && _VersionMinecraftChangeLog != null)
        {
            try
            {
                string wikiUrl = selectedVersion.StartsWith("a") ? $"https://minecraft.wiki/w/Java_Edition_Alpha_v{selectedVersion.Replace("a", "")}" :
                                      selectedVersion.StartsWith("b") ? $"https://minecraft.wiki/w/Java_Edition_Beta_{selectedVersion.Replace("b", "")}" :
                                      $"https://minecraft.wiki/w/Java_Edition_{selectedVersion}";

                string url = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
                using HttpClient httpClient = new HttpClient();
                string json = await httpClient.GetStringAsync(url);

                JObject manifest = JObject.Parse(json);
                JArray versions = (JArray)manifest["versions"];

                if (selectedVersion.StartsWith("1.7"))
                {
                    StartLinkChrom(wikiUrl);
                }
                else
                {
                    var selected = versions.FirstOrDefault(v => v["id"]?.ToString() == selectedVersion);
                    if (selected != null)
                    {
                        string versionUrl = selected["url"]?.ToString();
                        string versionJson = await httpClient.GetStringAsync(versionUrl);
                        JObject versionData = JObject.Parse(versionJson);
                    }
                    StartLinkChrom(wikiUrl);
                }
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("!", "Помилка при завантаженні даних про версію: " + ex.Message);
            }
        }
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
        if (_SelectVersion.IsVisible)
        {
            await AnimationHelper.FadeOutAsync(_SelectVersion, 0.3);
            return;
        }
        else
        {
            ShowVanillaVersionListAsync();
            await AnimationHelper.FadeInAsync(_SelectVersion, 0.3);
        }
    }

    // Обробка натискання на кнопку "BackMainWindow" для приховання/показу панелі вибору версії і приховання панелі вибору версії ванільної гри
    private async void _BackMainWindow_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (_SelectVersionTypeGird.IsVisible)
        {
            if (_SelectVersion.IsVisible) { await AnimationHelper.FadeOutAsync(_SelectVersion, 0.2); }
            await AnimationHelper.FadeOutAsync(_SelectVersionTypeGird, 0.3);
            return;
        }
        else
        {
            await AnimationHelper.FadeInAsync(_SelectVersionTypeGird, 0.3);
        }
    }

    // Обробка натискання на кнопку "CheckMarkAccount" для приховання/показу панеллю облікових записів
    private async void _CheckMarkAccount_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (_PanelManegerAccount.IsVisible)
        {
            await AnimationHelper.FadeOutAsync(_PanelManegerAccount, 0.3);
            return;
        }
        else
        {
            await AnimationHelper.FadeInAsync(_PanelManegerAccount, 0.3);
            LoadProfilesAndAddToListBox();
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
        try
        {
            _VersionVanil.Items?.Clear();

            string searchText = _SearchSystemTXT1.Text.ToLower().Trim();
            string pattern = string.IsNullOrEmpty(searchText) ? ".*" : searchText.Replace("*", ".*");
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            var path = new MinecraftPath(MinecraftPathHelper.GetDefaultExtractPath());
            var launcher = new MinecraftLauncher(path);
            var versions = await launcher.GetAllVersionsAsync();

            foreach (var item in versions)
            {
                if (item.Type == "release" && regex.IsMatch(item.Name) && _Relesed.IsChecked == true)
                    _VersionVanil.Items.Add(item.Name);
                if (item.Type == "snapshot" && regex.IsMatch(item.Name) && _Snapshot.IsChecked == true)
                    _VersionVanil.Items.Add(item.Name);
                if (item.Type == "old_beta" && regex.IsMatch(item.Name) && _Beta.IsChecked == true)
                    _VersionVanil.Items.Add(item.Name);
                if (item.Type == "old_alpha" && regex.IsMatch(item.Name) && _Alpha.IsChecked == true)
                    _VersionVanil.Items.Add(item.Name);
            }
        }
        catch (Exception ex)
        {
        }
    }
    // Метод для запуску Minecraft з обраною версією
    private async void LaunchMinecraft(string version)
    {
        try
        {
            if (_SelectVersion.IsVisible) { await AnimationHelper.FadeOutAsync(_SelectVersion, 0.2); }
            await AnimationHelper.FadeOutAsync(_SelectVersionTypeGird, 0.3);

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
                Session = session,
                MaximumRamMb = 2048
            });
            process.Start();

            this.WindowState = WindowState.Minimized;
            dowloadProgress.Close();
        }
        catch (Exception)
        {
        }
        //await DiscordController.UpdatePresence($"Грає версію {version}");
    }
    private async void LoadChangeLogMinecraft()
    {
        try
        {
            string url = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
            using HttpClient httpClient = new HttpClient();
            string json = await httpClient.GetStringAsync(url);

            JObject manifest = JObject.Parse(json);
            JArray versions = (JArray)manifest["versions"];

            _VersionMinecraftChangeLog.Items?.Clear();

            foreach (var version in versions)
            {
                string id = version["id"].ToString();
                _VersionMinecraftChangeLog.Items?.Add(id);

                if (id == "a1.0.4") break;
            }

            if (_VersionMinecraftChangeLog.Items.Count > 0)
                _VersionMinecraftChangeLog.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard("!","Не вдалося завантажити changelog: " + ex.Message);
        }
    }
    // Метод для відкриття посилання в браузері
    public void StartLinkChrom(string url)
    {
        try
        {
            if (url == "-")
                return;

            if (OperatingSystem.IsWindows())
            {
                string[] browsers = { "chrome.exe", "msedge.exe", "firefox.exe", "opera.exe" };
                bool launched = false;

                foreach (string browser in browsers)
                {
                    if (IsBrowserInstalled(browser))
                    {
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = "powershell.exe";
                            process.StartInfo.Arguments = $"start {browser} {url}";
                            process.StartInfo.CreateNoWindow = true;
                            process.Start();
                        }
                        launched = true;
                        break;
                    }
                }

                if (!launched)
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
            }
            else if (OperatingSystem.IsMacOS())
            {
                Process.Start("open", url);
            }
            else if (OperatingSystem.IsLinux())
            {
                Process.Start("xdg-open", url);
            }
            else
            {
                MessageBoxManager.GetMessageBoxStandard("!", "Невідома ОС. Не вдалося відкрити посилання.").ShowAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard("!", $"Помилка: {ex.Message}");
        }
    }
    // Перевірка наявності браузера в системі
    private bool IsBrowserInstalled(string browser)
    {
        if (OperatingSystem.IsWindows())
        {
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            return File.Exists(Path.Combine(programFiles, "Google\\Chrome\\Application", browser)) ||
                   File.Exists(Path.Combine(programFilesX86, "Google\\Chrome\\Application", browser)) ||
                   File.Exists(Path.Combine(programFiles, "Mozilla Firefox", browser)) ||
                   File.Exists(Path.Combine(programFilesX86, "Mozilla Firefox", browser)) ||
                   File.Exists(Path.Combine(programFiles, "Microsoft\\Edge\\Application", browser)) ||
                   File.Exists(Path.Combine(programFilesX86, "Microsoft\\Edge\\Application", browser)) ||
                   File.Exists(Path.Combine(programFiles, "Opera", browser)) ||
                   File.Exists(Path.Combine(programFilesX86, "Opera", browser));
        }
        else if (OperatingSystem.IsMacOS())
        {
            // Для MacOS перевіряємо наявність .app у стандартних шляхах
            string[] appPaths = {
                "/Applications/Google Chrome.app",
                "/Applications/Firefox.app",
                "/Applications/Microsoft Edge.app",
                "/Applications/Opera.app"
            };
            return appPaths.Any(Directory.Exists);
        }
        else if (OperatingSystem.IsLinux())
        {
            // Для Linux перевіряємо наявність браузера у PATH
            string? path = Environment.GetEnvironmentVariable("PATH");
            if (path == null) return false;
            foreach (var dir in path.Split(':'))
            {
                if (File.Exists(Path.Combine(dir, browser)))
                    return true;
            }
            return false;
        }
        return false;
    }
    // Метод для завантаження профілів з файлу і додавання їх до ListBox
    public void LoadProfilesAndAddToListBox()
    {
        string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        string profilesManagerPath = Path.Combine(directoryPath, "ProfilesManeger.json");

        if (!File.Exists(profilesManagerPath))
        {
            SettingsManager.Settings.SelectIndex = -1;
            SettingsManager.SaveSettings();

            if (!Directory.Exists(directoryPath))
            { Directory.CreateDirectory(directoryPath); File.Create(profilesManagerPath).Close(); }

            return;
        }

        try
        {
            var json = File.ReadAllText(profilesManagerPath);
            if (string.IsNullOrWhiteSpace(json))
                return;
            var profiles = JsonConvert.DeserializeObject<List<ProfileItem>>(json) ?? new();
            _ListAccount?.Items.Clear();

            for (byte i = 0; i < profiles.Count; i++)
            {
                var profile = profiles[i];
                var item = CreateProfileItem(profile, i);

                item._DeleteAccount.PointerPressed += async (s, e) =>
                {
                    var result = await MessageBoxManager.GetMessageBoxStandard(
                        "Ви впевненні?",
                        "!",
                        MsBox.Avalonia.Enums.ButtonEnum.YesNo,
                        MsBox.Avalonia.Enums.Icon.Info
                    ).ShowAsync();

                    if (result == ButtonResult.Yes)
                    {
                        DeleteProfile(profiles, profile, item);
                    }
                };
                item._ClickSelectAccount.PointerPressed += async (s, e) => await SelectProfile(profile, (byte)i);

                _ListAccount?.Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard("!", $"Помилка при зчитуванні профілів: {ex.Message}");
        }
    }
    // Метод для створення профілю в ListBox
    private ItemManegerProfile CreateProfileItem(ProfileItem profile, int index)
    {
        return new ItemManegerProfile
        {
            NameAccount2 = profile.NameAccount,
            UUID = profile.UUID,
            AccessToken = profile.AccessToken,
            ImageUrl = profile.ImageUrl,
            OfficalAccount = profile.OfficalAccount,
            index = index,
            _NameAccount = { Text = profile.NameAccount }
        };
    }
    // Метод для видалення профілю з ListBox і файлу
    public void DeleteProfile(List<ProfileItem> profiles, ProfileItem profile, ItemManegerProfile item)
    {
        MicosoftAccount = false;
        SettingsManager.Settings.MicrosoftAccount = false;
        SettingsManager.SaveSettings();

        if (SettingsManager.Settings.SelectIndex == _ListAccount.SelectedIndex)
        {
            SettingsManager.Settings.SelectedIndex = -1;
            SettingsManager.SaveSettings();
        }

        _TextBlockAccountName.Content = "Відсутній акаунт";
        _IconAccount.Source = null;

        profiles.Remove(profile);
        SaveProfiles(profiles);
        _ListAccount.Items.Remove(item);
    }

    // Метод для вибору профілю з ListBox
    public async Task SelectProfile(ProfileItem profile, byte index)
    {
        MicosoftAccount = profile.OfficalAccount;
        SettingsManager.Settings.MicrosoftAccount = profile.OfficalAccount;

        if (profile.OfficalAccount)
            session = await loginHandler.AuthenticateSilently() ?? await loginHandler.Authenticate();
        else
            session = MSession.CreateOfflineSession(profile.NameAccount);

        SettingsManager.Settings.SelectIndex = index;
        SettingsManager.SaveSettings();

        _TextBlockAccountName.Content = profile.NameAccount;
        //_IconAccount.Source = new Bitmap(profile.ImageUrl);
    }
    // Метод для збереження профілів у файл
    public void SaveProfiles(List<ProfileItem> profiles)
    {
        string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        string profilesManegerPath = Path.Combine(directoryPath, "ProfilesManeger.json");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var jsonToWrite = JsonConvert.SerializeObject(profiles, Formatting.Indented);
        File.WriteAllText(profilesManegerPath, jsonToWrite);

        LoadProfilesAndAddToListBox();
    }

    // Метод для збереження профілю в файл
    public void SaveProfile(ProfileItem profileItem)
    {
        var profiles = LoadProfiles();

        if (profiles.Any(p => p.NameAccount == profileItem.NameAccount))
        {
            MessageBoxManager.GetMessageBoxStandard("!","Цей профіль вже існує!");
            return;
        }

        profiles.Add(profileItem);
        SaveProfiles(profiles); 
    }
    // Метод для завантаження профілів з файлу
    public List<ProfileItem> LoadProfiles()
    {
        string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        string profilesManegerPath = Path.Combine(directoryPath, "ProfilesManeger.json");

        if (!File.Exists(profilesManegerPath))
        {
            return new List<ProfileItem>();
        }

        try
        {
            var json = File.ReadAllText(profilesManegerPath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<ProfileItem>();
            var profiles = JsonConvert.DeserializeObject<List<ProfileItem>>(json) ?? new List<ProfileItem>();
            return profiles;
        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard("!", $"Помилка при зчитуванні профілів: {ex.Message}");
            return new List<ProfileItem>();
        }
    }

}