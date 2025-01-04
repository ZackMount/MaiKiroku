using System;
using UnityEngine;
using Assets.Scripts.Constants;
using System.Text;

public class AppMain : MonoBehaviour
{
    // Start is called before the first frame update
    private bool consoleAllocated = false;

    void Start()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if (!ConsoleHelper.IsConsoleAttached())
        {
            // Attempt to attach to the parent process's console
            if (!ConsoleHelper.AttachToParentProcess())
            {
                // If attaching fails, attempt to allocate a new console
                if (!ConsoleHelper.AllocateConsole())
                {
                    Debug.LogError("Failed to allocate or attach to console. Standard stream redirection may fail.");
                }
            }
        }
        ConsoleHelper.SetConsoleCodePage();
        ConsoleHelper.RedirectStandardStreams();
        Console.OutputEncoding = Encoding.UTF8;

        ShowInfo();
#endif
        Logger.Info($"MaiKiroku {ApplicationConstants.version} has started.");
        Screen.SetResolution(1080, 2400, false);
        Application.targetFrameRate = 60;
        
    }
    void ShowInfo()
    {
        Console.Title = "MaiKiroku Logger";
    }
    void Update()
    {

    }
    private void OnDestroy()
    {


        if (consoleAllocated)
        {
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
            ConsoleHelper.FreeConsoleWrapper();
#endif
        }


#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        Logger.Instance?.Dispose();
#endif
    }
}
