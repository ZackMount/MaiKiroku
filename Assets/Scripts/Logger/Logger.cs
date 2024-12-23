using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;


public sealed class Logger : IDisposable
{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    private static readonly Lazy<Logger> _instance = new Lazy<Logger>(() => new Logger());
    public static Logger Instance => _instance.Value;

    private readonly string _logFilePath;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly Task _backgroundTask;
    private readonly BlockingCollection<LogEntry> _logQueue = new BlockingCollection<LogEntry>();

    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Debug;

    private Logger()
    {
        var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        Directory.CreateDirectory(logDirectory);
        _logFilePath = Path.Combine(logDirectory, $"application_{DateTime.UtcNow:yyyyMMdd}.log");

        _backgroundTask = Task.Factory.StartNew(ProcessLogQueue, TaskCreationOptions.LongRunning);
    }

    public void Log(
        LogLevel level,
        string message,
        object data = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        if (level < MinimumLogLevel)
            return;

        var logEntry = new LogEntry
        {
            Timestamp = DateTime.UtcNow,
            Level = level,
            Message = message,
            Data = data,
            MemberName = memberName,
            FilePath = filePath,
            LineNumber = lineNumber,
            ThreadId = Thread.CurrentThread.ManagedThreadId
        };

        _logQueue.Add(logEntry);
    }

    private async Task ProcessLogQueue()
    {
        foreach (var logEntry in _logQueue.GetConsumingEnumerable(_cancellationTokenSource.Token))
        {
            string formattedMessage = FormatLogEntry(logEntry);
            WriteToConsole(logEntry.Level, formattedMessage);
            await WriteToFileAsync(formattedMessage);

            if (logEntry.Level == LogLevel.Fatal)
            {
                Console.Beep();
                Console.WriteLine("Process has exited due to a fatal error.");
                Environment.Exit(1);
            }
        }
    }

    private string FormatLogEntry(LogEntry entry)
    {
        var sb = new StringBuilder();
        sb.Append($"[{entry.Timestamp:yyyy-MM-ddTHH:mm:ss.fffZ}] ");
        sb.Append($"[{entry.Level.ToString().ToUpper()}] ");
        sb.Append($"[Thread:{entry.ThreadId}] ");
        sb.Append($"[{Path.GetFileNameWithoutExtension(entry.FilePath)}.{entry.MemberName}() Line:{entry.LineNumber}] ");
        sb.Append(entry.Message);

        if (entry.Data != null)
        {
            sb.Append(" | ");
            sb.Append(FormatData(entry.Data));
        }

        return sb.ToString();
    }

    private string FormatData(object data)
    {
        if (data is Exception ex)
        {
            return $"Exception=\"{ex}\"";
        }
        else if (data is IDictionary<string, object> dict)
        {
            return FormatDictionary(dict);
        }
        else if (data is IEnumerable enumerable && !(data is string))
        {
            return FormatEnumerable(enumerable);
        }
        else if (IsSimpleType(data.GetType()))
        {
            return $"Data=\"{data}\"";
        }
        else
        {
            return FormatObjectProperties(data);
        }
    }

    private string FormatDictionary(IDictionary<string, object> dict)
    {
        var sb = new StringBuilder();
        bool first = true;
        foreach (var key in dict.Keys)
        {
            if (!first)
            {
                sb.Append(", ");
            }
            sb.Append($"{key}=\"{dict[key]}\"");
            first = false;
        }
        return sb.ToString();
    }

    private string FormatEnumerable(IEnumerable enumerable)
    {
        var sb = new StringBuilder();
        sb.Append("Items=[");
        bool first = true;
        foreach (var item in enumerable)
        {
            if (!first)
            {
                sb.Append(", ");
            }
            sb.Append($"\"{item}\"");
            first = false;
        }
        sb.Append("]");
        return sb.ToString();
    }

    private string FormatObjectProperties(object obj)
    {
        var sb = new StringBuilder();
        bool first = true;
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            if (!first)
            {
                sb.Append(", ");
            }
            var value = prop.GetValue(obj, null) ?? "null";
            sb.Append($"{prop.Name}=\"{value}\"");
            first = false;
        }
        return sb.ToString();
    }

    private bool IsSimpleType(Type type)
    {
        return
            type.IsPrimitive ||
            new Type[]
            {
                typeof(string),
                typeof(decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
            }.Contains(type) ||
            Convert.GetTypeCode(type) != TypeCode.Object;
    }

    private void WriteToConsole(LogLevel level, string message)
    {
        lock (_semaphore)
        {
            SetConsoleColor(level);
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    private async Task WriteToFileAsync(string message)
    {
        try
        {
            await _semaphore.WaitAsync();
            await File.AppendAllTextAsync(_logFilePath, message + Environment.NewLine);
        }
        catch (Exception ex)
        {
            lock (_semaphore)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
                Console.ResetColor();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void SetConsoleColor(LogLevel level)
    {
        switch (level)
        {
            case LogLevel.Debug:
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                break;
            case LogLevel.Info:
                Console.ResetColor();
                break;
            case LogLevel.Warn:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogLevel.Fatal:
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Red;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.White;
                break;
        }
    }

    public static void Debug(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Debug, message, null, memberName, filePath, lineNumber);

    public static void Debug(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Debug, message, data, memberName, filePath, lineNumber);

    public static void Debug(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        Instance.Log(LogLevel.Debug, message, null, memberName, filePath, lineNumber);
    }

    public static void Info(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Info, message, null, memberName, filePath, lineNumber);

    public static void Info(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Info, message, data, memberName, filePath, lineNumber);

    public static void Info(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        Instance.Log(LogLevel.Info, message, null, memberName, filePath, lineNumber);
    }

    public static void Warn(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Warn, message, null, memberName, filePath, lineNumber);

    public static void Warn(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Warn, message, data, memberName, filePath, lineNumber);

    public static void Warn(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        Instance.Log(LogLevel.Warn, message, null, memberName, filePath, lineNumber);
    }

    public static void Error(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Error, message, null, memberName, filePath, lineNumber);

    public static void Error(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Error, message, data, memberName, filePath, lineNumber);

    public static void Error(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        Instance.Log(LogLevel.Error, message, null, memberName, filePath, lineNumber);
    }

    public static void Fatal(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Fatal, message, null, memberName, filePath, lineNumber);

    public static void Fatal(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Fatal, message, data, memberName, filePath, lineNumber);

    public static void Fatal(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        Instance.Log(LogLevel.Fatal, message, null, memberName, filePath, lineNumber);
    }

    public void Dispose()
    {
        _logQueue.CompleteAdding();
        _cancellationTokenSource.Cancel();
        try
        {
            _backgroundTask.Wait();
        }
        catch (AggregateException)
        {

        }
        _semaphore.Dispose();
        _cancellationTokenSource.Dispose();
        _logQueue.Dispose();
    }

    private class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public object Data { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public int ThreadId { get; set; }
    }

#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
    private static readonly Lazy<Logger> _instance = new Lazy<Logger>(() => new Logger());
    public static Logger Instance => _instance.Value;

    private readonly string _logFilePath;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly Task _backgroundTask;
    private readonly BlockingCollection<LogEntry> _logQueue = new BlockingCollection<LogEntry>();

    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Debug;

    private Logger()
    {
        // 与 ConsoleHelper 的日志文件路径保持一致
        _logFilePath = Path.Combine(Application.persistentDataPath, "unity_console.log");

        // 确保日志文件存在
        Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));
        using (FileStream fs = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write))
        {
            // 仅确保文件被创建
        }

        _backgroundTask = Task.Factory.StartNew(ProcessLogQueue, TaskCreationOptions.LongRunning);
    }

    public void Log(
        LogLevel level,
        string message,
        object data = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        if (level < MinimumLogLevel)
            return;

        var logEntry = new LogEntry
        {
            Timestamp = DateTime.UtcNow,
            Level = level,
            Message = message,
            Data = data,
            MemberName = memberName,
            FilePath = filePath,
            LineNumber = lineNumber,
            ThreadId = Thread.CurrentThread.ManagedThreadId
        };

        _logQueue.Add(logEntry);
    }

    private async Task ProcessLogQueue()
    {
        foreach (var logEntry in _logQueue.GetConsumingEnumerable(_cancellationTokenSource.Token))
        {
            string formattedMessage = FormatLogEntry(logEntry);
            // 在 macOS 上，ConsoleHelper 已经将 Console 输出重定向到日志文件，因此可以选择不写入 Console
            // 但为了保持一致性，这里仍然写入 Console，内容将写入日志文件
            WriteToConsole(logEntry.Level, formattedMessage);
            await WriteToFileAsync(formattedMessage);

            if (logEntry.Level == LogLevel.Fatal)
            {
                // 在 macOS 上没有 Console.Beep，因此可以使用 Unity 的 Debug 提示
                // Debug.Break();
                // Debug.LogError("Process has exited due to a fatal error.");
                // 退出应用程序
                Environment.Exit(1);
            }
        }
    }

    private string FormatLogEntry(LogEntry entry)
    {
        var sb = new StringBuilder();
        sb.Append($"[{entry.Timestamp:yyyy-MM-ddTHH:mm:ss.fffZ}] ");
        sb.Append($"[{entry.Level.ToString().ToUpper()}] ");
        sb.Append($"[Thread:{entry.ThreadId}] ");
        sb.Append($"[{Path.GetFileNameWithoutExtension(entry.FilePath)}.{entry.MemberName}() Line:{entry.LineNumber}] ");
        sb.Append(entry.Message);

        if (entry.Data != null)
        {
            sb.Append(" | ");
            sb.Append(FormatData(entry.Data));
        }

        return sb.ToString();
    }

    private string FormatData(object data)
    {
        if (data is Exception ex)
        {
            return $"Exception=\"{ex}\"";
        }
        else if (data is IDictionary<string, object> dict)
        {
            return FormatDictionary(dict);
        }
        else if (data is IEnumerable enumerable && !(data is string))
        {
            return FormatEnumerable(enumerable);
        }
        else if (IsSimpleType(data.GetType()))
        {
            return $"Data=\"{data}\"";
        }
        else
        {
            return FormatObjectProperties(data);
        }
    }

    private string FormatDictionary(IDictionary<string, object> dict)
    {
        var sb = new StringBuilder();
        bool first = true;
        foreach (var key in dict.Keys)
        {
            if (!first)
            {
                sb.Append(", ");
            }
            sb.Append($"{key}=\"{dict[key]}\"");
            first = false;
        }
        return sb.ToString();
    }

    private string FormatEnumerable(IEnumerable enumerable)
    {
        var sb = new StringBuilder();
        sb.Append("Items=[");
        bool first = true;
        foreach (var item in enumerable)
        {
            if (!first)
            {
                sb.Append(", ");
            }
            sb.Append($"\"{item}\"");
            first = false;
        }
        sb.Append("]");
        return sb.ToString();
    }

    private string FormatObjectProperties(object obj)
    {
        var sb = new StringBuilder();
        bool first = true;
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            if (!first)
            {
                sb.Append(", ");
            }
            var value = prop.GetValue(obj, null) ?? "null";
            sb.Append($"{prop.Name}=\"{value}\"");
            first = false;
        }
        return sb.ToString();
    }

    private bool IsSimpleType(Type type)
    {
        return
            type.IsPrimitive ||
            new Type[]
            {
                typeof(string),
                typeof(decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
            }.Contains(type) ||
            Convert.GetTypeCode(type) != TypeCode.Object;
    }

    private void WriteToConsole(LogLevel level, string message)
    {
        // macOS 上的 Console 已被 ConsoleHelper 重定向到日志文件，因此这里只需写入 Console
        lock (_semaphore)
        {
            // 由于 Console 的颜色可能无法正确显示，可以选择不设置颜色
            Console.WriteLine(message);
        }
    }

    private async Task WriteToFileAsync(string message)
    {
        try
        {
            await _semaphore.WaitAsync();
            await File.AppendAllTextAsync(_logFilePath, message + Environment.NewLine);
        }
        catch (Exception ex)
        {
            lock (_semaphore)
            {
                // 在 macOS 上，无法像 Windows 一样设置 Console 颜色，可以使用 Unity 的 Debug 输出错误
                // Debug.LogError($"Failed to write to log file: {ex.Message}");
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public static void Debug(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Debug, message, null, memberName, filePath, lineNumber);

    public static void Debug(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Debug, message, data, memberName, filePath, lineNumber);

    public static void Debug(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        Instance.Log(LogLevel.Debug, message, null, memberName, filePath, lineNumber);
    }

    public static void Info(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Info, message, null, memberName, filePath, lineNumber);

    public static void Info(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Info, message, data, memberName, filePath, lineNumber);

    public static void Info(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        Instance.Log(LogLevel.Info, message, null, memberName, filePath, lineNumber);
    }

    public static void Warn(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Warn, message, null, memberName, filePath, lineNumber);

    public static void Warn(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Warn, message, data, memberName, filePath, lineNumber);

    public static void Warn(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        Instance.Log(LogLevel.Warn, message, null, memberName, filePath, lineNumber);
    }

    public static void Error(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Error, message, null, memberName, filePath, lineNumber);

    public static void Error(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Error, message, data, memberName, filePath, lineNumber);

    public static void Error(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        Instance.Log(LogLevel.Error, message, null, memberName, filePath, lineNumber);
    }

    public static void Fatal(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Fatal, message, null, memberName, filePath, lineNumber);

    public static void Fatal(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Instance.Log(LogLevel.Fatal, message, data, memberName, filePath, lineNumber);

    public static void Fatal(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        Instance.Log(LogLevel.Fatal, message, null, memberName, filePath, lineNumber);
    }

    public void Dispose()
    {
        _logQueue.CompleteAdding();
        _cancellationTokenSource.Cancel();
        try
        {
            _backgroundTask.Wait();
        }
        catch (AggregateException)
        {

        }
        _semaphore.Dispose();
        _cancellationTokenSource.Dispose();
        _logQueue.Dispose();
    }

    private class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public object Data { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public int ThreadId { get; set; }
    }

#else
    public static void Debug(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        UnityEngine.Debug.Log($"DEBUG: {message} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Debug(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        UnityEngine.Debug.Log($"DEBUG: {message} | {FormatData(data)} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Debug(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        UnityEngine.Debug.Log($"DEBUG: {message} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Info(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        UnityEngine.Debug.Log($"INFO: {message} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Info(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        UnityEngine.Debug.Log($"INFO: {message} | {FormatData(data)} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Info(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        UnityEngine.Debug.Log($"INFO: {message} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Warn(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        UnityEngine.Debug.LogWarning($"WARN: {message} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Warn(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        UnityEngine.Debug.LogWarning($"WARN: {message} | {FormatData(data)} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Warn(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        UnityEngine.Debug.LogWarning($"WARN: {message} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Error(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        UnityEngine.Debug.LogError($"ERROR: {message} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Error(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        UnityEngine.Debug.LogError($"ERROR: {message} | {FormatData(data)} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Error(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        UnityEngine.Debug.LogError($"ERROR: {message} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Fatal(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        UnityEngine.Debug.LogError($"FATAL: {message} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Fatal(string message, object data,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        UnityEngine.Debug.LogError($"FATAL: {message} | {FormatData(data)} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    public static void Fatal(string format,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        string message = string.Format(format, args);
        UnityEngine.Debug.LogError($"FATAL: {message} (at {Path.GetFileNameWithoutExtension(filePath)}.{memberName}() Line:{lineNumber})");
    }

    private static string FormatData(object data)
    {
        if (data is Exception ex)
        {
            return $"Exception=\"{ex}\"";
        }
        else if (data is IDictionary<string, object> dict)
        {
            return FormatDictionary(dict);
        }
        else if (data is IEnumerable enumerable && !(data is string))
        {
            return FormatEnumerable(enumerable);
        }
        else if (IsSimpleType(data.GetType()))
        {
            return $"Data=\"{data}\"";
        }
        else
        {
            return FormatObjectProperties(data);
        }
    }

    private static string FormatDictionary(IDictionary<string, object> dict)
    {
        var sb = new StringBuilder();
        bool first = true;
        foreach (var key in dict.Keys)
        {
            if (!first)
            {
                sb.Append(", ");
            }
            sb.Append($"{key}=\"{dict[key]}\"");
            first = false;
        }
        return sb.ToString();
    }

    private static string FormatEnumerable(IEnumerable enumerable)
    {
        var sb = new StringBuilder();
        sb.Append("Items=[");
        bool first = true;
        foreach (var item in enumerable)
        {
            if (!first)
            {
                sb.Append(", ");
            }
            sb.Append($"\"{item}\"");
            first = false;
        }
        sb.Append("]");
        return sb.ToString();
    }

    private static string FormatObjectProperties(object obj)
    {
        var sb = new StringBuilder();
        bool first = true;
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            if (!first)
            {
                sb.Append(", ");
            }
            var value = prop.GetValue(obj, null) ?? "null";
            sb.Append($"{prop.Name}=\"{value}\"");
            first = false;
        }
        return sb.ToString();
    }

    private static bool IsSimpleType(Type type)
    {
        return
            type.IsPrimitive ||
            new Type[]
            {
                typeof(string),
                typeof(decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
            }.Contains(type) ||
            Convert.GetTypeCode(type) != TypeCode.Object;
    }
    public void Dispose()
    {

    }
#endif
}
