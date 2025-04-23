﻿using IniParser;
using IniParser.Model;

namespace R6Downloader.CLI;

public class ConfigINI
{
    /// <summary>
    /// Reading the <paramref name="section"/> and the <paramref name="key"/> from <paramref name="filename"/>
    /// </summary>
    /// <param name="filename">FileName to Read</param>
    /// <param name="section">INI Section</param>
    /// <param name="key">INI Key</param>
    /// <returns>Readed value or <see cref="string.Empty"/></returns>
    public static string Read(string filename, string section, string key)
    {
        if (!File.Exists(filename))
            return string.Empty;
        FileIniDataParser parser = new();
        IniData data = parser.ReadFile(filename, System.Text.Encoding.UTF8);
        if (!data.Sections.ContainsSection(section))
            return string.Empty;
        if (!data[section].ContainsKey(key))
            return string.Empty;
        return data[section][key];
    }

    /// <summary>
    /// Reading a <typeparamref name="T"/> type in a <paramref name="section"/> and the <paramref name="key"/> from <paramref name="filename"/>
    /// </summary>
    /// <typeparam name="T">A Type that has <see cref="IConvertible"/> and <see cref="IParsable{TSelf}"/></typeparam>
    /// <param name="filename">FileName to Read</param>
    /// <param name="section">INI Section</param>
    /// <param name="key">INI Key</param>
    /// <returns>Readed value or default</returns>
    public static T? Read<T>(string filename, string section, string key) where T : IConvertible, IParsable<T>
    {
        string readed = Read(filename, section, key);
        if (string.IsNullOrEmpty(readed))
            return default;
        T? value = default;
        T.TryParse(readed, null, out value);
        return value;
    }

    public static bool Exists(string filename, string section, string key)
    {
        string readed = Read(filename, section, key);
        return !string.IsNullOrEmpty(readed);
    }

    /// <summary>
    /// Write a <paramref name="value"/> to the <paramref name="filename"/> with a Key of <paramref name="key"/> and a Section as <paramref name="section"/>
    /// </summary>
    /// <param name="filename">FileName to write to</param>
    /// <param name="section">INI Section</param>
    /// <param name="key">INI Key</param>
    /// <param name="value">The Value</param>
    public static void Write(string filename, string section, string key, string? value)
    {
        if (!File.Exists(filename))
        {
            string? path = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(filename, ";TMP");
        }
        FileIniDataParser parser = new();
        IniData data = parser.ReadFile(filename, System.Text.Encoding.UTF8);
        data[section][key] = value;
        parser.WriteFile(filename, data, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// Write a <paramref name="value"/> of Type <typeparamref name="T"/> to the <paramref name="filename"/> with a Key of <paramref name="key"/> and a Section as <paramref name="section"/>
    /// </summary>
    /// <typeparam name="T">A Type that has <see cref="IConvertible"/></typeparam>
    /// <param name="filename">FileName to write to</param>
    /// <param name="section">INI Section</param>
    /// <param name="key">INI Key</param>
    /// <param name="value">The Value</param>
    public static void Write<T>(string filename, string section, string key, T value) where T : IConvertible
    {
        Write(filename, section, key, value.ToString());
    }
}
