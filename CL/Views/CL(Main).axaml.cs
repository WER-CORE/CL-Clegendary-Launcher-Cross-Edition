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
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Diagnostics;
using CL.CustomItem;
using CmlLib.Core.Auth.Microsoft;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;
using CL.Views;
using System.Security.Cryptography;

namespace CL;

public partial class Window1 : Avalonia.Controls.Window
{

    #region Елементи керування
    public ListBox _VersionVanil => this.FindControl<ListBox>("VersionList")!;
    public ListBox _VersionMinecraftChangeLog => this.FindControl<ListBox>("VersionMinecraftChangeLog")!;
    public Border _SelectVersion => this.FindControl<Border>("SelectVersion")!;
    public Grid _SelectVersionTypeGird => this.FindControl<Grid>("SelectVersionTypeGird")!;
    public Border _SelectNow => this.FindControl<Border>("PanelSelectNow")!;
    public Border _SelectVersionVanila => this.FindControl<Border>("SelectVersionVanila")!;
    public Image _BackMainWindow => this.FindControl<Image>("BackMainWindow")!;
    public Label _PlayTXT => this.FindControl<Label>("PlayTXTPanelSelect")!;
    public TextBlock _PlayTXTMinecraft => this.FindControl<TextBlock>("PlayTXT")!;
    public Label _ModBuild => this.FindControl<Label>("modbuilds")!;
    public Label _ModsTXT => this.FindControl<Label>("ModsTXTPanelSelect")!;
    public Label _ServerTXT => this.FindControl<Label>("ServerTXTPanelSelect")!;
    public Label _TextBlockAccountName => this.FindControl<Label>("NameNik")!;
    public Label _AddProfile => this.FindControl<Label>("AddProfile")!;
    public Image _CheckMarkAccount => this.FindControl<Image>("CheckMarkAccount")!;
    public Panel _PanelManegerAccount => this.FindControl<Panel>("PanelManegerAccount")!;
    #endregion
    
    // Ліцензія
    private bool MicosoftAccount;
    JELoginHandler loginHandler;
    MSession session;
    
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
        _VersionMinecraftChangeLog.SelectionChanged += _VersionMinecraftChangeLog_SelectionChanged;
        LoadChangeLogMinecraft(); // Завантаження списку версій Minecraft(changelog) для відображення в ListBox
    }

    private async void _VersionMinecraftChangeLog_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
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
                MessageBoxManager.GetMessageBoxStandard("!","Помилка при завантаженні даних про версію: " + ex.Message);
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
    public async void LoadProfilesAndAddToListBox()
    {

        string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        string profilesManagerPath = Path.Combine(directoryPath, "ProfilesManeger.json");

        if (!File.Exists(profilesManagerPath))
        {
            Settings1.Default.SelectIndexAccount = -1;
            Settings1.Default.Save();
            if (!Directory.Exists(directoryPath))
            { Directory.CreateDirectory(directoryPath); File.Create(profilesManagerPath).Close(); }
            return;
        }

        try
        {
            var encryptedData = File.ReadAllBytes(profilesManagerPath);
            var decryptedJson = DecryptData(encryptedData);
            var profiles = JsonConvert.DeserializeObject<List<ProfileItem>>(decryptedJson) ?? new();

            ListAccount.Items.Clear();

            for (byte i = 0; i < profiles.Count; i++)
            {
                var profile = profiles[i];
                var item = CreateProfileItem(profile, i);

                item.DeleteAccount.MouseDown += (s, e) =>
                {
                    if (MessageBox.Show("Ви впевненні?", "!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                    { DeleteProfile(profiles, profile, item); }
                };
                item.ClickSelectAccount.MouseDown += async (s, e) => await SelectProfile(profile, (byte)i);

                ListAccount.Items.Add(item);
            }

        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard("!",$"Помилка при зчитуванні профілів: {ex.Message}");
        }
    }
    private ItemManegerProfile CreateProfileItem(ProfileItem profile, int index)
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.UriSource = new Uri(profile.ImageUrl);
        image.EndInit();

        return new ItemManegerProfile
        {
            NameAccount2 = profile.NameAccount,
            UUID = profile.UUID,
            AccessToken = profile.AccessToken,
            ImageUrl = profile.ImageUrl,
            OfficalAccount = profile.OfficalAccount,
            index = index,
            IconAccountType = { Source = image },
            NameAccount = { Text = profile.NameAccount }
        };
    }

    private void DeleteProfile(List<ProfileItem> profiles, ProfileItem profile, ItemManegerProfile item)
    {
        MicosoftAccount = false;
        Settings1.Default.Microsoft = false;
        Settings1.Default.Save();

        if (Settings1.Default.SelectIndexAccount == ListAccount.SelectedIndex)
        {
            Settings1.Default.SelectIndexAccount = -1;
            Settings1.Default.Save();
        }

        _TextBlockAccountName.Content = "Відсутній акаунт";
        IconAccount.Source = null;

        profiles.Remove(profile);
        SaveProfiles(profiles);
        ListAccount.Items.Remove(item);
    }

    private async Task SelectProfile(ProfileItem profile, byte index)
    {
        MicosoftAccount = profile.OfficalAccount;
        Settings1.Default.Microsoft = profile.OfficalAccount;

        if (profile.OfficalAccount)
            session = await loginHandler.AuthenticateSilently() ?? await loginHandler.Authenticate();
        else
            session = MSession.CreateOfflineSession(profile.NameAccount);

        Settings1.Default.SelectIndexAccount = index;
        Settings1.Default.Save();

        _TextBlockAccountName.Content = profile.NameAccount;
        IconAccount.Source = new BitmapImage(new Uri(profile.ImageUrl));
    }
    public void SaveProfiles(List<ProfileItem> profiles)
    {
        string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        string profilesManegerPath = Path.Combine(directoryPath, "ProfilesManeger.json");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var jsonToWrite = JsonConvert.SerializeObject(profiles, Formatting.Indented);
        var encryptedData = EncryptData(jsonToWrite);

        File.WriteAllBytes(profilesManegerPath, encryptedData);

        LoadProfilesAndAddToListBox();
    }

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
            var encryptedData = File.ReadAllBytes(profilesManegerPath);
            var decryptedJson = DecryptData(encryptedData);
            var profiles = JsonConvert.DeserializeObject<List<ProfileItem>>(decryptedJson) ?? new List<ProfileItem>();

            return profiles;
        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard("!", $"Помилка при зчитуванні профілів: {ex.Message}");
            return new List<ProfileItem>();
        }
    }
    private byte[] EncryptData(string plainText)
    {
        byte[] key = GetEncryptionKey();
        byte[] iv = new byte[16];

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return ms.ToArray();
                }
            }
        }
    }
    private string DecryptData(byte[] cipherText)
    {
        byte[] key = GetEncryptionKey();
        byte[] iv = new byte[16]; 

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                using (var ms = new MemoryStream(cipherText))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
    private byte[] GetEncryptionKey()
    {
        string base64Key = Settings1.Default.EncryptKey;

        if (string.IsNullOrEmpty(base64Key) || Convert.FromBase64String(base64Key).Length != 16)
        {
            byte[] newKey = new byte[16];
            new Random().NextBytes(newKey);
            base64Key = Convert.ToBase64String(newKey);

            Settings1.Default.EncryptKey = base64Key;
            Settings1.Default.Save();

            return newKey;
        }
        return Convert.FromBase64String(base64Key);
    }

}