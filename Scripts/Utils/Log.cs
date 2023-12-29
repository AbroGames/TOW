﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOW.Scripts.Services.ModLoader;
using TOW.Scripts.Utils.Loggers;

namespace TOW.Scripts.Utils;

internal enum PrefixType
{
    Debug,
    Info,
    Warning,
    Error,
    Critical
}

public static class Log
{
    
    private static readonly HashSet<ILogger> _loggers = [];
    private static readonly string[] _prefixes;

    static Log()
    {
        var rawPrefixes = Enum.GetNames<PrefixType>();
        var prefixes = new List<string>();
        
        var longestPrefix = rawPrefixes.Max(p => p.Length);
        
        foreach (var rawPrefix in rawPrefixes)
        {
            var sb = new StringBuilder();
            sb.Append(rawPrefix.ToUpper());
            while (sb.Length < longestPrefix)
            {
                sb.Append(' ');
            }
            prefixes.Add(sb.ToString());
        }

        _prefixes = prefixes.ToArray();
        
        AddLogger(new DefaultLogger());
    }

    public static void AddLogger(ILogger logger)
    {
        if (logger is ModLogger) throw new ArgumentException("Can't use ModLogger as a global logger because of recursive calls");
        _loggers.Add(logger);
    }
    
    public static ILogger GetModLogger(Mod mod)
    {
        return new ModLogger(mod);
    }

    public static void Debug(object msg = null)
    {
        foreach (var logger in _loggers) logger.Debug(Format(msg, PrefixType.Debug));
    }

    public static void Info(object msg = null)
    {
        foreach (var logger in _loggers) logger.Info(Format(msg, PrefixType.Info));
    }

    public static void Warning(object msg = null)
    {
        foreach (var logger in _loggers) logger.Warning(Format(msg, PrefixType.Warning));
    }

    public static void Error(object msg = null)
    {
        foreach (var logger in _loggers) logger.Error(Format(msg, PrefixType.Error));
    }

    public static void Critical(object msg = null)
    {
        foreach (var logger in _loggers) logger.Critical(Format(msg, PrefixType.Critical));
    }

    private static string Format(object msg = null, PrefixType prefix = PrefixType.Info)
    {
        if (msg is null) return null;
        string text = msg.ToString();
        
        var now = DateTime.Now;
        return $"{now.ToShortDateString()} {now.ToLongTimeString()} {_prefixes[(int)prefix]} {text}";
    }
}



public interface ILogger
{
    void Debug(object msg = null);
    void Info(object msg = null);
    void Warning(object msg = null);
    void Error(object msg = null);
    void Critical(object msg = null);
}