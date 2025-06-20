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
using CmlLib.Core.ModLoaders.FabricMC;
using CmlLib.Core.ModLoaders.QuiltMC;
using CmlLib.Core.Installer.Forge.Versions;
using Avalonia.Platform.Storage;
using CmlLib.Core.Installer.Forge;

namespace CL;

public partial class Window1 : Avalonia.Controls.Window
{

    #region �������� ���������
    public ListBox _ListAccount => this.FindControl<ListBox>("ListAccount")!;
    public ListBox _VersionVanil => this.FindControl<ListBox>("VersionList")!;
    public ListBox _VersionListVanila => this.FindControl<ListBox>("VersionListVanila")!;
    public ListBox _VersionListMod => this.FindControl<ListBox>("VersionListMod")!;
    public ListBox _VersionMinecraftChangeLog => this.FindControl<ListBox>("VersionMinecraftChangeLog")!;
    public Border _SelectVersion => this.FindControl<Border>("SelectVersion")!;
    public Border _SelectVersionMod => this.FindControl<Border>("SelectVersionMod")!;
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
    public Label _PlayTXTMinecraft => this.FindControl<Label>("PlayTXT")!;
    public Label _VersionVanillaLatest => this.FindControl<Label>("VersionVanillaLatest")!;
    public Label _VersionFabricMC => this.FindControl<Label>("VersionFabricMC")!;
    public Label _VersionFabricLoader => this.FindControl<Label>("VersionFabricLoader")!;
    public Label _VersionForgeMC => this.FindControl<Label>("VersionForgeMC")!;
    public Label _VersionForgeLoader => this.FindControl<Label>("VersionForgeLoader")!;
    public Label _VersionQuiltMC => this.FindControl<Label>("VersionQuiltMC")!;
    public Label _VersionQuiltLoader => this.FindControl<Label>("VersionQuiltLoader")!;
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

    // ��� ������� �����
    string? type;
    // ˳����� �� ������
    private bool? MicosoftAccount;
    JELoginHandler? loginHandler;
    MSession? session;
    
    public Window1()
    {
        InitializeComponent(); // ����������� ����������(������������ XAML)
        System.Net.ServicePointManager.DefaultConnectionLimit = 1000000;
        _CheckMarkAccount.PointerPressed += _CheckMarkAccount_PointerPressed; // ������� ���������� �� ������ "CheckMarkAccount" ��� ��������� ������� �������� ������
        _BackMainWindow.PointerPressed += _BackMainWindow_PointerPressed; // ������� ���������� �� ������ "BackMainWindow" ��� ��������� ������� ������ ����
        
        _SelectVersionVanila.PointerPressed += _SelectVersionVanila_PointerPressed; // ������� ���������� �� ������ "SelectVersionVanila" ��� ��������� ������� ������ ���� ������� ���
        _SelectVersionFabric.PointerPressed += _SelectVersionFabric_PointerPressed; // ������� ���������� �� ������ "SelectVersionFabric" ��� ��������� ������� ������ ���� Fabric
        _SelectVersionForge.PointerPressed += _SelectVersionForge_PointerPressed; // ������� ���������� �� ������ "SelectVersionForge" ��� ��������� ������� ������ ���� Forge;
        _SelectVersionQuilt.PointerPressed += _SelectVersionQuilt_PointerPressed; // ������� ���������� �� ������ "SelectVersionQuilt" ��� ��������� ������� ������ ���� Quilt

        _VersionListMod.SelectionChanged += _VersionListMod_SelectionChanged; // ������� ���������� �� ������ ������� �����;
        _VersionListVanila.SelectionChanged += _VersionListVanila_SelectionChanged; // ������� ���������� �� ������ �������� �����

        _VersionVanil.SelectionChanged += _VersionVanil_SelectionChanged; ; // ������� ���������� �� ������ ����� ������� ���
        _SearchSystemTXT1.TextChanged += (s, e) => ShowVanillaVersionListAsync(); // ������� ���� ������ � ��� ������ ����� ������� ���

        _Snapshot.IsCheckedChanged += ChangeCheckBoxVersionFilter_IsCheckedChanged; ; // ������� ��������� ������� �� �������� ���������� �����
        _Relesed.IsCheckedChanged += ChangeCheckBoxVersionFilter_IsCheckedChanged; // ������� ��������� ������� �� �������� ���������� �����
        _Beta.IsCheckedChanged += ChangeCheckBoxVersionFilter_IsCheckedChanged; // ������� ��������� ������� �� �������� ���������� �����
        _Alpha.IsCheckedChanged += ChangeCheckBoxVersionFilter_IsCheckedChanged; // ������� ��������� ������� �� �������� ���������� �����

        _PlayTXTMinecraft.PointerPressed += _PlayTXTMinecraft_PointerPressed; // ������� ���������� �� ������ "PlayTXTMinecraft" ��� ������� ��� � ������� ����� �� ����� �������������

        _PlayTXT.PointerPressed +=  (s, e) => { AnimationHelper.AnimateBorder(0, 0, _SelectNow); }; // ������� ��� ���������� ������� �� ������ "PlayTXTPanelSelect"
        _ModBuild.PointerPressed += (s, e) => { AnimationHelper.AnimateBorder(80, 0, _SelectNow); }; // ������� ��� ���������� ������� �� ������ "modbuilds"
        _ModsTXT.PointerPressed += (s, e) => { AnimationHelper.AnimateBorder(160, 0, _SelectNow); }; // ������� ��� ���������� ������� �� ������ "ModsTXTPanelSelect"
        _ServerTXT.PointerPressed += (s, e) => { AnimationHelper.AnimateBorder(223, 0, _SelectNow); }; // ������� ��� ���������� ������� �� ������ "ServerTXTPanelSelect"
        
        _VersionMinecraftChangeLog.SelectionChanged += _VersionMinecraftChangeLog_SelectionChanged; // ������� ���� ������ ���� Minecraft(changelog) � ListBox

        _GirdFormAccountAdd.PointerPressed += _GirdFormAccountAdd_PointerPressed; // ������� ���������� �� ����� ��������� ������� ��� �� ��������
        _AddProfile.PointerPressed += _AddProfile_PointerPressed; // ������� ���������� �� ������ "AddProfile" ��� �������� ����� ��������� �������

        _CreateAccount_Offline.PointerPressed += _CreateAccount_Offline_PointerPressed; // ������� ���������� �� ������ "CreateAccount_Offline" �������� ���� � ���� � ��������� ������-�������
        _CreateAccount_Online.PointerPressed += _CreateAccount_Online_PointerPressed; ; // ������� ���������� �� ������ "CreateAccount_Online" �������� ����� ��� ���������� �� ������-�������
        _OnlineAccount.PointerPressed += _OnlineAccount_PointerPressed; // ������� ���������� �� ������ "OnlineAccount" ��� ����������� �� ������-������
        _OfflineAccount.PointerPressed += _OfflineAccount_PointerPressed; // ������� ���������� �� ������ "OfflineAccount" ��� ����������� �� ������-������
        
        LoadChangeLogMinecraft(); // ������������ ������ ����� Minecraft(changelog) ��� ����������� � ListBox
        LoadProfilesAndAddToListBox(); // ������������ ������� � ����� � ��������� �� �� ListBox
    }

    private async void _PlayTXTMinecraft_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var selectedVersion = _VersionListVanila.SelectedItem as string;
        var selectVersionMod = type != "Vanil" ? _VersionListMod.SelectedItem as string : null;

        switch (type)
        {
            case "Vanil":
                InstallVanilAndPlay(selectedVersion);
                break;
            case "Forge":
                InstallForgeAndPlay(selectedVersion, selectVersionMod);
                break;
            case "Fabric":
                InstallFabricAndPlay(selectedVersion, selectVersionMod);
                break;
            case "Quilt":
                InstallQuiltAndPlay(selectedVersion, selectVersionMod);
                break;
            default:
                await MessageBoxManager
                    .GetMessageBoxStandard("�������", $"�������� ��� �������������: {type}")
                    .ShowAsync();
                break;
        }
    }
    // ����� ��� ��������� ������ ����� ������� ��� ��� ��� �������(��������)
    private void ChangeCheckBoxVersionFilter_IsCheckedChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ShowVanillaVersionListAsync();
    }
    // ����� ���� ���������� �� ��� ����� ��������� �������, ��� ������� �����
    private async void _GirdFormAccountAdd_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        await AnimationHelper.FadeOutAsync(_GirdOfflineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdOnlineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdFormAccountAdd, 0.2); await AnimationHelper.FadeOutAsync(_GirdSelectAccountType, 0.2);
    }

    // ������� ���������� �� ������ "CreateAccount_Online" �������� ����� ��� ���������� �� ������-�������
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
                NameAccount = sessionMicrosoft.Username ?? throw new InvalidOperationException("�'�� ����������� null."),
                UUID = sessionMicrosoft.UUID ?? throw new InvalidOperationException("UUID null."),
                ImageUrl = $"https://mc-heads.net/avatar/{sessionMicrosoft.UUID}",
                AccessToken = sessionMicrosoft.AccessToken ?? throw new InvalidOperationException("AccessToken null."),
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

            MessageBoxManager.GetMessageBoxStandard("!",$"������� ����� � Microsoft {ex.Message}",ButtonEnum.Ok,MsBox.Avalonia.Enums.Icon.Error);
            await AnimationHelper.FadeOutAsync(_GirdOfflineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdOnlineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdFormAccountAdd, 0.2); await AnimationHelper.FadeOutAsync(_GirdSelectAccountType, 0.2);
        }
    }
    // ������� ���������� �� ������ "CreateAccount_Offline" �������� ���� � ���� � ��������� ������-�������
    private async void _CreateAccount_Offline_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        try
        {
            string uuid = Guid.NewGuid().ToString();

            ProfileItem profileItem = new ProfileItem
            {
                NameAccount = _NameNikManeger.Text ?? "null",
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
            MessageBoxManager.GetMessageBoxStandard("!", $"������� ��������� ������ ������� {ex.Message}", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            await AnimationHelper.FadeOutAsync(_GirdOfflineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdOnlineMode, 0.2); await AnimationHelper.FadeOutAsync(_GirdFormAccountAdd, 0.2); await AnimationHelper.FadeOutAsync(_GirdSelectAccountType, 0.2);
        }
    }

    // ������� ���������� �� ������ "OfflineAccount" ��� ����������� �� ������-������
    private async void _OfflineAccount_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        await AnimationHelper.FadeOutAsync(_GirdOnlineMode, 0.2);
        await AnimationHelper.FadeInAsync(_GirdOfflineMode, 0.5);
    }
    // ������� ���������� �� ������ "OnlineAccount" ��� ����������� �� ������-������
    private async void _OnlineAccount_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        await AnimationHelper.FadeOutAsync(_GirdOfflineMode, 0.2);
        await AnimationHelper.FadeInAsync(_GirdOnlineMode, 0.5);
    }

    // ������� ���������� �� ������ "AddProfile" ��� �������� ����� ��������� �������
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

    // ������� ���������� �� ������ "VersionMinecraftChangeLog" ��� �������� �� ����� �� ����� Minecraft(changelog)
    private async void _VersionMinecraftChangeLog_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_VersionMinecraftChangeLog.IsPointerOver)
            return;

        if (_VersionMinecraftChangeLog.SelectedItem is string selectedVersion && _VersionMinecraftChangeLog != null)
        {
            try
            {
               string baseWikiTitleUA = selectedVersion.StartsWith("a") ? $"Alpha_v{selectedVersion.Replace("a", "")}_(Java_Edition)" :
                                         selectedVersion.StartsWith("b") ? $"Beta_{selectedVersion.Replace("b", "")}_(Java_Edition)" :
                                         $"{selectedVersion}_(Java_Edition)";
                string fandomUkUrl = $"https://uk.minecraft.wiki/w/{Uri.EscapeDataString(baseWikiTitleUA)}";

                StartLinkChrom(fandomUkUrl);
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("!", "������� ��� ����������� ����� ��� �����: " + ex.Message);
            }
        }
    }
    private void _VersionListMod_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_VersionListVanila.SelectedItem != null && _VersionListMod.SelectedItem != null)
        {
            var selectedVersion = _VersionListVanila.SelectedItem as string;
            var selectedVersionMod = _VersionListMod.SelectedItem as string;
            _PlayTXTMinecraft.Content = $"����� � ({selectedVersion} : {selectedVersionMod})";

            if (type == "Forge")
            {
                _VersionForgeLoader.Content = _VersionListMod.SelectedItem;
            }
            else if (type == "Fabric")
            {
                _VersionFabricLoader.Content = _VersionListMod.SelectedItem;
            }
            if (type == "Quilt")
            {
                _VersionQuiltLoader.Content = _VersionListMod.SelectedItem;
            }
        }
    }
    // ������� ���� ������ ���� ������� ��� � ����� �������� ������� ����� � ListBox
    private void _VersionListVanila_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_VersionListVanila.SelectedItem != null)
        {
            ShowModsVersion(type);
            if (type == "Forge")
            {
                _VersionForgeMC.Content = _VersionListVanila.SelectedItem;
            }
            else if (type == "Fabric")
            {
                _VersionFabricMC.Content = _VersionListVanila.SelectedItem;
            }
            if (type == "Quilt")
            {
                _VersionQuiltMC.Content = _VersionListVanila.SelectedItem;
            }
        }
    }
    // ������� ���� ������ ���� ������� ��� � ListBox
    private void _VersionVanil_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_VersionVanil.SelectedItem != null)
        {
            var selectedVersion = _VersionVanil.SelectedItem as string;
            _VersionVanillaLatest.Content = selectedVersion;
            _PlayTXTMinecraft.Content = $"����� � ({selectedVersion})";
        }
    }
    // ������� ���������� �� ������ "SelectVersionVanila" ��� ����������/������ ����� ������ ���� ������� ���
    private async void _SelectVersionVanila_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (_SelectVersion.IsVisible)
        {
            await AnimationHelper.FadeOutAsync(_SelectVersion, 0.3);
            return;
        }
        else
        {
            type = "Vanil";
            if (_SelectVersionMod.IsVisible) { await AnimationHelper.FadeOutAsync(_SelectVersionMod, 0.2); }
            await AnimationHelper.FadeInAsync(_SelectVersion, 0.3);
            ShowVanillaVersionListAsync();
        }
    }
    // ������� ���������� �� ������ "SelectVersionFabric" ��� ����������/������ ����� ������ ���� Fabric
    private async void _SelectVersionFabric_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_SelectVersionMod.IsVisible)
        {
            await AnimationHelper.FadeOutAsync(_SelectVersionMod, 0.3);
            return;
        }
        else
        {
            if (_SelectVersion.IsVisible) { await AnimationHelper.FadeOutAsync(_SelectVersion, 0.2); }
            await AnimationHelper.FadeInAsync(_SelectVersionMod, 0.3);
            ShowModsVanilVersion("Fabric");
        }
    }
    // ������� ���������� �� ������ "SelectVersionForge" ��� ����������/������ ����� ������ ���� Forge
    private async void _SelectVersionForge_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_SelectVersionMod.IsVisible)
        {
            await AnimationHelper.FadeOutAsync(_SelectVersionMod, 0.3);
            return;
        }
        else
        {
            if (_SelectVersion.IsVisible) { await AnimationHelper.FadeOutAsync(_SelectVersion, 0.2); }
            await AnimationHelper.FadeInAsync(_SelectVersionMod, 0.3);
            ShowModsVanilVersion("Forge");
        }
    }
    // ������� ���������� �� ������ "SelectVersionQuilt" ��� ����������/������ ����� ������ ���� Quilt
    private async void _SelectVersionQuilt_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_SelectVersionMod.IsVisible)
        {
            await AnimationHelper.FadeOutAsync(_SelectVersionMod, 0.3);
            return;
        }
        else
        {
            if (_SelectVersion.IsVisible) { await AnimationHelper.FadeOutAsync(_SelectVersion, 0.2); }
            await AnimationHelper.FadeInAsync(_SelectVersionMod, 0.3);
            ShowModsVanilVersion("Quilt");
        }
    }
    // ������� ���������� �� ������ "BackMainWindow" ��� ����������/������ ����� ������ ���� � ���������� ����� ������ ���� ������� ���
    private async void _BackMainWindow_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (_SelectVersionTypeGird.IsVisible)
        {
            if (_SelectVersion.IsVisible) { await AnimationHelper.FadeOutAsync(_SelectVersion, 0.2); }
            if (_SelectVersionMod.IsVisible) { await AnimationHelper.FadeOutAsync(_SelectVersionMod, 0.2); }

            await AnimationHelper.FadeOutAsync(_SelectVersionTypeGird, 0.3);
            return;
        }
        else
        {
            await AnimationHelper.FadeInAsync(_SelectVersionTypeGird, 0.3);
        }
    }

    // ������� ���������� �� ������ "CheckMarkAccount" ��� ����������/������ ������� �������� ������
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
    // ����������� ����������(������������ XAML)
    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }
    // ����� ��� ��������� ������ �������� ����� Minecraft � �� ����������� � ListBox
    private async void ShowVanillaVersionListAsync()
    {
        try
        {
            _VersionVanil.Items?.Clear();

            string? searchText = _SearchSystemTXT1?.Text?.ToLower().Trim();
            string pattern = string.IsNullOrEmpty(searchText) ? ".*" : searchText.Replace("*", ".*");
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            var path = new MinecraftPath(MinecraftPathHelper.GetDefaultExtractPath());
            var launcher = new MinecraftLauncher(path);
            var versions = await launcher.GetAllVersionsAsync();

            foreach (var item in versions)
            {
                if (item.Type == "release" && regex.IsMatch(item.Name) && _Relesed.IsChecked == true)
                    _VersionVanil?.Items?.Add(item.Name);
                if (item.Type == "snapshot" && regex.IsMatch(item.Name) && _Snapshot.IsChecked == true)
                    _VersionVanil?.Items?.Add(item.Name);
                if (item.Type == "old_beta" && regex.IsMatch(item.Name) && _Beta.IsChecked == true)
                    _VersionVanil?.Items?.Add(item.Name);
                if (item.Type == "old_alpha" && regex.IsMatch(item.Name) && _Alpha.IsChecked == true)
                    _VersionVanil?.Items?.Add(item.Name);
            }
        }
        catch (Exception)
        {
        }
    }
    // ����� ��� ������������ ������� ����� �� ���������(Fabric, Forge, Quilt) � �� ����������� � ListBox ������ ����
    public async void ShowModsVanilVersion(string type)
    {
        _VersionListMod.Items?.Clear();
        _VersionListVanila.Items?.Clear();

        this.type = type;
        try
        {
            if (type == "Fabric")
            {
                var fabricInstaller = new FabricInstaller(new HttpClient());
                var versions = await fabricInstaller.GetSupportedVersionNames();

                foreach (var version in versions)
                {
                    _VersionListVanila?.Items?.Add(version);
                }
            }
            if (type == "Forge")
            {
                var path = new MinecraftPath(MinecraftPathHelper.GetDefaultExtractPath());
                var launcher = new MinecraftLauncher(path);
                var versions = await launcher.GetAllVersionsAsync();

                foreach (var version in versions)
                {
                    if(version.Type == "release")
                    {
                        _VersionListVanila?.Items?.Add(version.Name);
                    }
                }
            }
            if (type == "Quilt")
            {
                var quiltInstaller = new QuiltInstaller(new HttpClient());
                var versions = await quiltInstaller.GetSupportedVersionNames();

                foreach (var version in versions)
                {
                    _VersionListVanila?.Items?.Add(version);
                }
            }
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard("!", $"������� ��� ����������� �����: {ex.Message}", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
        }
    }
    // ����� ��� ������������ ������� ����� �� ���������(Fabric, Forge, Quilt) � �� ����������� � ListBox ����� ����
    public async void ShowModsVersion(string type)
    {
        _VersionListMod.Items?.Clear();

        type = this.type ?? "Forge";
        
        try
        {
            if (type == "Fabric")
            {
                var fabricInstaller = new FabricInstaller(new HttpClient());
                var versions = await fabricInstaller.GetLoaders($"{_VersionListVanila.SelectedItem}");

                foreach (var version in versions)
                {
                    _VersionListMod?.Items?.Add(version.Version);
                }
            }
            if (type == "Forge")
            {
                var versionLoader = new ForgeVersionLoader(new HttpClient());
                var versions = await versionLoader.GetForgeVersions($"{_VersionListVanila.SelectedItem}");

                foreach (var version in versions)
                {
                    _VersionListMod?.Items?.Add(version.ForgeVersionName);
                }
            }
            if (type == "Quilt")
            {
                var quiltInstaller = new QuiltInstaller(new HttpClient());
                var versions = await quiltInstaller.GetLoaders($"{_VersionListVanila.SelectedItem}");

                foreach (var version in versions)
                {
                    _VersionListMod?.Items?.Add(version.Version);
                }
            }
        }
        catch (Exception ex)
        {
           await MessageBoxManager.GetMessageBoxStandard("!", $"������� ��� ����������� �����: {ex.Message}", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
        }
    }

    // ����� ��� ������� Minecraft � ������� �����
    private async void InstallVanilAndPlay(string version)
    {
        try
        {
            if (_SelectVersion.IsVisible) { await AnimationHelper.FadeOutAsync(_SelectVersion, 0.2); }
            await AnimationHelper.FadeOutAsync(_SelectVersionTypeGird, 0.3);

            DowloadProgress dowloadProgress = new DowloadProgress();
            dowloadProgress.Show();

            var path = new MinecraftPath(MinecraftPathHelper.GetDefaultExtractPath());
            var launcher = new MinecraftLauncher(path);

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
        catch (Exception ex)
        {
           await MessageBoxManager.GetMessageBoxStandard("�������", $"�� ������� ���������� ����� Minecraft. �������� ���������� �� ��������� ��� ������� ���� �����. {ex}", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
        }
        //await DiscordController.UpdatePresence($"��� ����� {version}");
    }
    private async void InstallFabricAndPlay(string selectedVersion, string selectVersionMod)
    {
        try
        {
            if (_SelectVersionMod.IsVisible) { await AnimationHelper.FadeOutAsync(_SelectVersionMod, 0.2); }
            await AnimationHelper.FadeOutAsync(_SelectVersionTypeGird, 0.3);

            DowloadProgress dowloadProgress = new DowloadProgress();
            dowloadProgress.Show();

            var path = new MinecraftPath(MinecraftPathHelper.GetDefaultExtractPath());
            var launcher = new MinecraftLauncher(path);

            var fabricInstaller = new FabricInstaller(new HttpClient());
            var versionName = await fabricInstaller.Install(selectedVersion, selectVersionMod, path);

            launcher.FileProgressChanged += (sender, args) =>
            {
                int fileProgress = args.TotalTasks > 0 ? (int)((double)args.ProgressedTasks / args.TotalTasks * 100) : 0;
                dowloadProgress.DowloadProgressBarFileTask(args.TotalTasks, args.ProgressedTasks, args.Name);
                dowloadProgress.DowloadProgressBarVersion(fileProgress, versionName);
            };

            launcher.ByteProgressChanged += (sender, args) =>
            {
                int byteProgress = args.TotalBytes > 0 ? (int)((double)args.ProgressedBytes / args.TotalBytes * 100) : 0;
                dowloadProgress.DowloadProgressBarFile(byteProgress);
            };

            var process = await launcher.InstallAndBuildProcessAsync(versionName, new MLaunchOption
            {
                Session = session,
                MaximumRamMb = 2048
            });
            process.Start();
            this.WindowState = WindowState.Minimized;
            dowloadProgress.Close();
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard("�������", $"�� ������� ���������� ����� Minecraft. �������� ���������� �� ��������� ��� ������� ���� �����. {ex}", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
        }
    }

    private async void InstallForgeAndPlay(string selectedVersion, string selectVersionMod)
    {
        try
        {
            if (_SelectVersionMod.IsVisible) { await AnimationHelper.FadeOutAsync(_SelectVersionMod, 0.2); }
            await AnimationHelper.FadeOutAsync(_SelectVersionTypeGird, 0.3);

            DowloadProgress dowloadProgress = new DowloadProgress();
            dowloadProgress.Show();

            var path = new MinecraftPath(MinecraftPathHelper.GetDefaultExtractPath());
            var launcher = new MinecraftLauncher(path);

            var forge = new ForgeInstaller(launcher);

            var versionName = await forge.Install(selectedVersion, selectVersionMod);

            launcher.FileProgressChanged += (sender, args) =>
            {
                int fileProgress = args.TotalTasks > 0 ? (int)((double)args.ProgressedTasks / args.TotalTasks * 100) : 0;
                dowloadProgress.DowloadProgressBarFileTask(args.TotalTasks, args.ProgressedTasks, args.Name);
                dowloadProgress.DowloadProgressBarVersion(fileProgress, versionName);
            };

            launcher.ByteProgressChanged += (sender, args) =>
            {
                int byteProgress = args.TotalBytes > 0 ? (int)((double)args.ProgressedBytes / args.TotalBytes * 100) : 0;
                dowloadProgress.DowloadProgressBarFile(byteProgress);
            };

            var process = await launcher.InstallAndBuildProcessAsync(versionName, new MLaunchOption
            {
                Session = session,
                MaximumRamMb = 2048
            });
            process.Start();
            this.WindowState = WindowState.Minimized;
            dowloadProgress.Close();
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard($"�������", $"�� ������� ���������� ����� Minecraft. �������� ���������� �� ��������� ��� ������� ���� �����. {ex}", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
        }
    }
    private async void InstallQuiltAndPlay(string selectedVersion, string selectVersionMod)
    {
        try
        {
            if (_SelectVersionMod.IsVisible) { await AnimationHelper.FadeOutAsync(_SelectVersionMod, 0.2); }
            await AnimationHelper.FadeOutAsync(_SelectVersionTypeGird, 0.3);

            DowloadProgress dowloadProgress = new DowloadProgress();
            dowloadProgress.Show();

            var path = new MinecraftPath(MinecraftPathHelper.GetDefaultExtractPath());
            var launcher = new MinecraftLauncher(path);

            var quiltInstaller = new QuiltInstaller(new HttpClient());

            var versionName = await quiltInstaller.Install(selectedVersion, selectVersionMod, path);

            launcher.FileProgressChanged += (sender, args) =>
            {
                int fileProgress = args.TotalTasks > 0 ? (int)((double)args.ProgressedTasks / args.TotalTasks * 100) : 0;
                dowloadProgress.DowloadProgressBarFileTask(args.TotalTasks, args.ProgressedTasks, args.Name);
                dowloadProgress.DowloadProgressBarVersion(fileProgress, versionName);
            };

            launcher.ByteProgressChanged += (sender, args) =>
            {
                int byteProgress = args.TotalBytes > 0 ? (int)((double)args.ProgressedBytes / args.TotalBytes * 100) : 0;
                dowloadProgress.DowloadProgressBarFile(byteProgress);
            };

            var process = await launcher.InstallAndBuildProcessAsync(versionName, new MLaunchOption
            {
                Session = session,
                MaximumRamMb = 2048
            });
            process.Start();
            this.WindowState = WindowState.Minimized;
            dowloadProgress.Close();
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard($"�������", $"�� ������� ���������� ����� Minecraft. �������� ���������� �� ��������� ��� ������� ���� �����. {ex}", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
        }
    }

    private async void LoadChangeLogMinecraft()
    {
        try
        {
            string url = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
            using HttpClient httpClient = new HttpClient();
            string json = await httpClient.GetStringAsync(url);

            JObject manifest = JObject.Parse(json);
            JArray? versions = (JArray?)manifest["versions"] as JArray;
            if (versions == null)
                throw new InvalidOperationException("����� ���������� changelog");

            _VersionMinecraftChangeLog.Items?.Clear();

            foreach (var version in versions)
            {
                string? id = version["id"]?.ToString();
                _VersionMinecraftChangeLog.Items?.Add(id);

                if (id == "a1.0.4") break;
            }

            if (_VersionMinecraftChangeLog?.Items?.Count > 0)
                _VersionMinecraftChangeLog.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard("!","�� ������� ����������� changelog: " + ex.Message);
        }
    }
    // ����� ��� �������� ��������� � �������
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
                MessageBoxManager.GetMessageBoxStandard("!", "������� ��. �� ������� ������� ���������.").ShowAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard("!", $"�������: {ex.Message}");
        }
    }
    // �������� �������� �������� � ������
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
            // ��� MacOS ���������� �������� .app � ����������� ������
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
            // ��� Linux ���������� �������� �������� � PATH
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
    // ����� ��� ������������ ������� � ����� � ��������� �� �� ListBox
    public void LoadProfilesAndAddToListBox()
    {
        string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        string profilesManagerPath = Path.Combine(directoryPath, "ProfilesManeger.json");

        if (!File.Exists(profilesManagerPath))
        {
            SettingsManager.Settings.SelectIndex = -1;
            SettingsManager.SaveSettings();

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            File.Create(profilesManagerPath).Close();
            return;
        }

        try
        {
            var json = File.ReadAllText(profilesManagerPath);
            if (string.IsNullOrWhiteSpace(json))
                return;

            var profiles = JsonConvert.DeserializeObject<List<ProfileItem>>(json) ?? new();
            _ListAccount?.BeginInit();
            _ListAccount?.Items.Clear();

            foreach (var (profile, index) in profiles.Select((p, i) => (p, (byte)i)))
            {
                var item = CreateProfileItem(profile, index);

                if (item._DeleteAccount is Image deleteButton)
                {
                    deleteButton.PointerPressed += async (s, e) =>
                    {
                        var result = await MessageBoxManager.GetMessageBoxStandard(
                            "�� �������?",
                            "��� ������� ���� ��������.",
                            MsBox.Avalonia.Enums.ButtonEnum.YesNo,
                            MsBox.Avalonia.Enums.Icon.Info
                        ).ShowAsync();

                        if (result == ButtonResult.Yes)
                        {
                            DeleteProfile(profiles, profile, item);
                        }
                    };
                }

                if (item._ClickSelectAccount is Grid selectButton)
                {
                    selectButton.PointerPressed += async (s, e) =>
                    {
                        await SelectProfile(profile, index);
                    };
                }

                _ListAccount?.Items.Add(item);
            }

            _ListAccount?.EndInit();
        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard("!", $"������� ��� ��������� �������: {ex.Message}").ShowAsync();
        }
    }
    // ����� ��� ��������� ������� � ListBox
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
    // ����� ��� ��������� ������� � ListBox � �����
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

        _TextBlockAccountName.Content = "³������ ������";
        _IconAccount.Source = null;

        profiles.Remove(profile);
        SaveProfiles(profiles);
        _ListAccount.Items.Remove(item);
    }

    // ����� ��� ������ ������� � ListBox
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
    // ����� ��� ���������� ������� � ����
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

    // ����� ��� ���������� ������� � ����
    public void SaveProfile(ProfileItem profileItem)
    {
        var profiles = LoadProfiles();

        if (profiles.Any(p => p.NameAccount == profileItem.NameAccount))
        {
            MessageBoxManager.GetMessageBoxStandard("!","��� ������� ��� ����!");
            return;
        }

        profiles.Add(profileItem);
        SaveProfiles(profiles); 
    }
    // ����� ��� ������������ ������� � �����
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
            MessageBoxManager.GetMessageBoxStandard("!", $"������� ��� ��������� �������: {ex.Message}");
            return new List<ProfileItem>();
        }
    }

}