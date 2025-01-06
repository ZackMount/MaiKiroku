using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Diagnostics;

public static class ConsoleHelper
{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    // Import necessary Win32 API functions
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AttachConsole(int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleOutputCP(uint wCodePageID);

    private const int ATTACH_PARENT_PROCESS = -1;
    private const int STD_OUTPUT_HANDLE = -11;

    // Check if a console is already attached
    public static bool IsConsoleAttached()
    {
        IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        return stdHandle != IntPtr.Zero && stdHandle != new IntPtr(-1);
    }

    // Allocate a new console
    public static bool AllocateConsole()
    {
        bool result = AllocConsole();
        if (!result)
        {
            UnityEngine.Debug.LogError("AllocConsole failed, error code: " + Marshal.GetLastWin32Error());
        }
        return result;
    }

    // Free/Detach the console
    public static bool FreeConsoleWrapper()
    {
        return FreeConsole();
    }

    // Attempt to attach to the parent process's console
    public static bool AttachToParentProcess()
    {
        bool result = AttachConsole(ATTACH_PARENT_PROCESS);
        if (!result)
        {
            UnityEngine.Debug.LogError("AttachConsole failed, error code: " + Marshal.GetLastWin32Error());
        }
        return result;
    }

    // Redirect standard output and error streams to the console
    public static void RedirectStandardStreams()
    {
        IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        if (stdHandle == IntPtr.Zero || stdHandle == new IntPtr(-1))
        {
            UnityEngine.Debug.LogError("Failed to get standard output handle. Please ensure the console is correctly allocated or attached.");
            return;
        }

        var safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, false);
        if (!safeFileHandle.IsInvalid)
        {
            try
            {
                var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                var encoding = System.Text.Encoding.UTF8;
                var writer = new StreamWriter(fileStream, encoding) { AutoFlush = true };
                Console.SetOut(writer);
                Console.SetError(writer);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("An exception occurred while redirecting standard streams: " + ex.Message);
            }
        }
        else
        {
            UnityEngine.Debug.LogError("SafeFileHandle is invalid.");
        }
    }
    public static void SetConsoleCodePage()
    {
        const uint CP_UTF8 = 65001;
        SetConsoleOutputCP(CP_UTF8);
    }

#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
 private static string logFilePath = Path.Combine(ApplicationConstants.BasePath, "unity_console.log");
    private static Process tailProcess;

    /// <summary>
    /// 检查是否已经附加了控制台（通过检查 tail 进程是否存在）。
    /// </summary>
    public static bool IsConsoleAttached()
    {
        return tailProcess != null && !tailProcess.HasExited;
    }

    /// <summary>
    /// 分配一个新的控制台，通过打开一个终端窗口并运行 'tail -f' 命令实时显示日志文件。
    /// </summary>
    public static bool AllocateConsole()
    {
        try
        {
            // 确保日志文件存在
            using (FileStream fs = new FileStream(logFilePath, FileMode.Append, FileAccess.Write))
            {
                // 仅确保文件被创建
            }

            // 使用 AppleScript 打开一个新的终端窗口并运行 'tail -f' 命令
            string script = $"tell application \"Terminal\" to do script \"tail -f '{EscapeForAppleScript(logFilePath)}'\"";
            Process.Start("osascript", "-e \"" + script + "\"");

            // 记录 tail 进程（可选，如果需要进一步管理）
            // 这里假设只打开一个终端窗口，因此不追踪具体进程

            UnityEngine.Debug.Log("Console allocated: Terminal window opened to display logs.");
            return true;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Failed to allocate console: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 释放/分离控制台。由于无法直接关闭终端窗口，这里留空或记录必要的信息。
    /// </summary>
    public static bool FreeConsoleWrapper()
    {
        try
        {
            // 关闭终端窗口的逻辑较为复杂，通常需要通过 AppleScript 或其他方法。
            // 这里可以选择不实现，或提醒用户手动关闭终端窗口。
            UnityEngine.Debug.LogWarning("FreeConsole is not implemented on macOS. Please close the Terminal window manually if needed.");
            return true;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Failed to free console: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// AttachToParentProcess 在 macOS 上不适用。
    /// </summary>
    public static bool AttachToParentProcess()
    {
        UnityEngine.Debug.LogWarning("AttachToParentProcess is not supported on macOS.");
        return false;
    }

    /// <summary>
    /// 将标准输出和错误流重定向到日志文件。
    /// </summary>
    public static void RedirectStandardStreams()
    {
        try
        {
            var logFileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            var encoding = System.Text.Encoding.UTF8;
            var writer = new StreamWriter(logFileStream, encoding) { AutoFlush = true };
            Console.SetOut(writer);
            Console.SetError(writer);

            UnityEngine.Debug.Log("Standard streams redirected to log file: " + logFilePath);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("An exception occurred while redirecting standard streams: " + ex.Message);
        }
    }

    /// <summary>
    /// 设置控制台代码页。macOS 下不需要设置，因为默认使用 UTF-8 编码。
    /// </summary>
    public static void SetConsoleCodePage()
    {
        // 无需在 macOS 上设置
    }

    /// <summary>
    /// 转义字符串以适应 AppleScript 的单引号。
    /// </summary>
    /// <param name="input">需要转义的字符串</param>
    /// <returns>转义后的字符串</returns>
    private static string EscapeForAppleScript(string input)
    {
        return input.Replace("'", "\\'");
    }
#else
    // Do nothing on non-Windows platforms or in the editor
    public static bool IsConsoleAttached() => false;
    public static bool AllocateConsole() => false;
    public static bool FreeConsoleWrapper() => false;
    public static bool AttachToParentProcess() => false;
    public static void RedirectStandardStreams() { }
#endif
}
