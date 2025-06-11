using System;
using System.IO;
using Newtonsoft.Json;

// Шаблон файлу settings.json
public class UserSettings
{
    public string EncryptedPassword { get; set; } = string.Empty;
    public string Theme { get; set; } = "Dark";
    public string Language { get; set; } = "ua";
    public bool MicrosoftAccount { get; set; } = false;
    public int SelectIndex { get; set; } = -1;
}

// Цей класс керуе настройками користувача, зберігаючи їх у файлі JSON а також завантажує
public static class SettingsManager
{
    // Шлях до папки де зберігаеться файл налаштувань
    public static string SettingsPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "settings.json");
    // Динамічний об'єкт для зберігання налаштувань
    public static dynamic Settings = string.Empty;

    // Метод для завантаження налаштувань з файлу
    public static void LoadSettings()
    {
        Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));

        if (!File.Exists(SettingsPath))
        {
            File.Create(SettingsPath).Close();

            var settings = new UserSettings(); 
            string jsonFile = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsManager.SettingsPath, jsonFile);
        }

        string json = File.ReadAllText(SettingsPath);
        Settings = JsonConvert.DeserializeObject(json);
    }
    // Метод для збереження налаштувань у файл
    public static void SaveSettings()
    {
        string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
        File.WriteAllText(SettingsPath, json);
    }
}
