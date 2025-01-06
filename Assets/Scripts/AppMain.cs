using System;
using UnityEngine;
using Assets.Scripts.Constants;
using Assets.Scripts.Api.Lxns.Managers;
using Assets.Scripts;

public class AppMain : MonoBehaviour
{
    // Start is called before the first frame update
    private bool consoleAllocated = false;
    private Initializer initializer;
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
        // 初始化

        initializer = new Initializer();
        initializer.InitializeJacketSprites();

        Logger.Info($"MaiKiroku {ApplicationConstants.Version} has started.");
        Screen.SetResolution(1080, 2400, false);
        Application.targetFrameRate = 20;
        var songManager = SongManager.Instance;
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

        ApplicationConstants.jacketSpriteCache.Clear();
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
