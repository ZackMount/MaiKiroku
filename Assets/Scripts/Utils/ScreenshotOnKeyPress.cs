using System;
using System.IO;
using UnityEngine;
using Assets.Scripts.Constants;
using Assets.Scripts.Api.Lxns;

public class ScreenshotOnKeyPress : MonoBehaviour
{
    private string outputsDirectory = Path.Combine(ApplicationConstants.baseMenu, "outpus");
    private bool running = false;
    public ScoreItemManager scoreItemManager;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            TakeScreenshot();
            Logger.Info("Screenshot taken successfully.");
        }
    }


    private void TakeScreenshot()
    {
        Directory.CreateDirectory(outputsDirectory);
        string outputsFilePath = Path.Combine(ApplicationConstants.baseMenu, $"{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff")}.png");
        StartCoroutine(ScreenshotUtility.CaptureScreenshotAsync(outputsFilePath));
    }
}
