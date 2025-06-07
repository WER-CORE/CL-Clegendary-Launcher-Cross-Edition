using Avalonia.Controls;
using CL.Script;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;

namespace CL.Views
{
    public partial class MainWindow : Window
    {
        private Random _random = new Random();
        private readonly List<string> RandomPhrases = new List<string>
        {
    "Сонце сяє на повну потужність...",
    "Запасаємося морозивом для спекотних днів...",
    "Плануємо ідеальний літній відпочинок...",
    "Homka ловить сонячні промені на пляжі...",
    "Deeplay показує лише літні картинки 🌴...",
    "Готуємо мангал — час пікніка!",
    "Запускаємо вентилятори на максимум...",
    "Збираємося з друзями на озері...",
    "Насолоджуємось літніми заходами сонця...",
    "Поливаємо клумби під палючим сонцем...",
    "Данило купає ноги в холодній воді...",
    "Проводимо вечори з лимонадом на балконі...",
    "WER_Clegendary мріє про літні канікули...",
    "Пляж, вода і трошки Minecraft — ідеальне літо!",
    "Влаштовуємо вечірній кінопоказ просто неба...",
    "Відкриваємо вікна, запускаємо цикад і вітерець...",
    "Слухаємо ледь чутне «дз-з-з» комарів... і тікаємо!",
        };


        public MainWindow()
        {
            InitializeComponent();
            CheckUpdate();
        }
        private async void CheckUpdate()
        {
            string versionLauncher = Assembly
                .GetExecutingAssembly()
                .GetName()
                .Version
                .ToString();

            VersionLauncherTXT.Content = versionLauncher + "-Beta";
            try
            {
                using (HttpClient cl = new HttpClient())
                {
                    string versionToInternet = LoadFileVersion("https://drive.usercontent.google.com/u/0/uc?id=1ZjUJGOhNcPXonXPQ4Hks8M5cHxN2CBmv&export=download");

                    if (versionLauncher != versionToInternet)
                    {
                        UpdateWindow updater = new UpdateWindow();
                        updater.Show();
                        this.Hide();
                    }
                    if (versionLauncher == versionToInternet)
                    {
                        await DiscordController.Initialize($"В віконці завантаження");
                        StartLoadingAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Помилка", "Помилка завантаження" + ex.Message).ShowAsync();
                this.Hide();
            }
        }
        private async void StartLoadingAsync()
        {
            LoadingProgressBar.Value = 0;

            for (int i = 0; i <= 100; i++)
            {
                double previousValue = LoadingProgressBar.Value;
                double targetValue = i;

                await AnimateProgressBarAsync(previousValue, targetValue, TimeSpan.FromMilliseconds(100));

                if (i % 20 == 0)
                {
                    string randomPhrase = RandomPhrases[_random.Next(RandomPhrases.Count)];
                    RandomPhraseText.Text = randomPhrase;
                }

                await Task.Delay(_random.Next(50, 100));
            }

            OpenMainWindow();
        }
        private void OpenMainWindow()
        {
            var cl_main = new Window1();
            cl_main.Show();

            if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                desktopLifetime.MainWindow = cl_main;
            }

            this.Close();
            DiscordController.Deinitialize();
        }
        public static string LoadFileVersion(string url)
        {
            using var client = new HttpClient();
            var content = client.GetStringAsync(url).Result;
            return content;
        }
        private async Task AnimateProgressBarAsync(double from, double to, TimeSpan duration)
        {
            int steps = 10;
            double stepValue = (to - from) / steps;
            int stepTime = (int)(duration.TotalMilliseconds / steps);

            for (int i = 1; i <= steps; i++)
            {
                LoadingProgressBar.Value = from + (stepValue * i);
                await Task.Delay(stepTime);
            }
        }


    }
}