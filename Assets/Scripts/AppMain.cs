using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
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
        Console.Title = "MaiKiroku Logger";
        Console.WriteLine();
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("------------------------------------------------------------------------");
        Console.WriteLine("------------------------------------------------------------------------");
        Console.WriteLine();
        Console.ResetColor();

#endif
        Logger.Debug("MaiKiroku has started.");
        Screen.SetResolution(1080, 2400, false);
        Application.targetFrameRate = 60;
        Logger.Info("Current Resolution: " + Screen.currentResolution.width + "x" + Screen.currentResolution.height);

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
