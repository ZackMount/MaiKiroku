using System;
using System.IO;
using UnityEngine;
using System.Collections;

public class ScreenshotOnKeyPress : MonoBehaviour
{
    // 指定输出目录路径
    private string outputsDirectory = "D:\\Unity Project\\maibot\\Build\\outputs";

    void Update()
    {
        // 检测是否按下了W键
        if (Input.GetKeyDown(KeyCode.W))
        {
            TakeScreenshot();
        }
    }

    private void TakeScreenshot()
    {
        // 创建输出目录（如果不存在）
        Directory.CreateDirectory(outputsDirectory);

        // 生成文件路径，文件名包含当前 UTC 日期
        string outputsFilePath = Path.Combine(outputsDirectory, $"outputs_{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff")}.jpg");

        // 启动协程进行截图
        StartCoroutine(ScreenshotUtility.CaptureScreenshotAsync(outputsFilePath));
    }
}
