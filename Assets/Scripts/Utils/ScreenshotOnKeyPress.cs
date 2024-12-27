using System;
using System.IO;
using UnityEngine;
using System.Collections;

public class ScreenshotOnKeyPress : MonoBehaviour
{
    private string outputsDirectory = Path.Combine(Environment.CurrentDirectory,"outpus");

    void Update()
    {

        // 测试用快捷创建截图
        if (Input.GetKeyDown(KeyCode.W))
        {
            TakeScreenshot();
        }
    }

    private void TakeScreenshot()
    {
        Directory.CreateDirectory(outputsDirectory);
        string outputsFilePath = Path.Combine(outputsDirectory, $"{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff")}.png");
        StartCoroutine(ScreenshotUtility.CaptureScreenshotAsync(outputsFilePath));
    }
}
