using UnityEngine;
using System.IO;
using System.Collections;
using System.Diagnostics;

public static class ScreenshotUtility
{
    /// <summary>
    /// 截取当前屏幕画面并保存为PNG文件到指定路径（异步操作）。
    /// 注意：建议在协程中调用此函数，确保不会阻塞主线程。
    /// </summary>
    /// <param name="savePath">截图保存路径，包含文件名和扩展名(.png)</param>
    public static IEnumerator CaptureScreenshotAsync(string savePath)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        // 创建一个纹理对象，避免每次截图都创建新的 Texture2D
        Texture2D screenTexture = new Texture2D(1080, 2400, TextureFormat.RGB24, false);

        // 等待当前帧渲染完成
        yield return new WaitForEndOfFrame();

        // 从屏幕读取像素 (0,0)点为左下角，避免不必要的内存复制
        screenTexture.ReadPixels(new Rect(0, 0, 1080, 2400), 0, 0);
        screenTexture.Apply(); // 更新纹理

        // 编码为PNG字节数组
        byte[] pngBytes = screenTexture.EncodeToPNG();

        // 释放纹理资源，避免内存泄漏
        Object.DestroyImmediate(screenTexture);

        // 将PNG字节数组写入指定文件路径
        File.WriteAllBytes(savePath, pngBytes);
        stopwatch.Stop();
        Logger.Info($"Image \"{savePath}\" saved successfully in {stopwatch.ElapsedMilliseconds}ms.");
    }

    /// <summary>
    /// 截取当前屏幕画面并保存为较低分辨率的PNG文件到指定路径（异步操作）。
    /// 通过降低分辨率来提高截图效率。
    /// </summary>
    /// <param name="savePath">截图保存路径，包含文件名和扩展名(.png)</param>
    public static IEnumerator CaptureLowerResolutionScreenshotAsync(string savePath, int targetWidth, int targetHeight)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        // 创建一个纹理对象，降低分辨率来提高效率
        Texture2D screenTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);

        // 等待当前帧渲染完成
        yield return new WaitForEndOfFrame();

        // 从屏幕读取像素并缩放到目标分辨率
        screenTexture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        screenTexture.Apply(); // 更新纹理

        // 编码为PNG字节数组
        byte[] pngBytes = screenTexture.EncodeToPNG();

        // 释放纹理资源
        Object.DestroyImmediate(screenTexture);

        // 将PNG字节数组写入指定文件路径
        File.WriteAllBytes(savePath, pngBytes);

        stopwatch.Stop();

        Logger.Info($"Lower resolution image \"{savePath}\" saved successfully in {stopwatch.ElapsedMilliseconds}ms.");
    }
    public static void demo()
    {
        //StartCoroutine(ScreenshotUtility.CaptureScreenshotAsync("path/to/your/screenshot.png"));
        //StartCoroutine(ScreenshotUtility.CaptureLowerResolutionScreenshotAsync("path/to/your/lower_res_screenshot.png", 1080/2, 2400/2));

    }
}


