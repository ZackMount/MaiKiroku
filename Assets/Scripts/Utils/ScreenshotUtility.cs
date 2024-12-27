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
        Texture2D screenTexture = new Texture2D(1080, 2400, TextureFormat.RGB24, false);

        yield return new WaitForEndOfFrame();
        screenTexture.ReadPixels(new Rect(0, 0, 1080, 2400), 0, 0);
        screenTexture.Apply();

        byte[] pngBytes = screenTexture.EncodeToPNG();

        Object.DestroyImmediate(screenTexture);

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
        Texture2D screenTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);

        yield return new WaitForEndOfFrame();

        screenTexture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        screenTexture.Apply();

        byte[] pngBytes = screenTexture.EncodeToPNG();
        Object.DestroyImmediate(screenTexture);

        File.WriteAllBytes(savePath, pngBytes);

        stopwatch.Stop();

        Logger.Info($"Lower resolution image \"{savePath}\" saved successfully in {stopwatch.ElapsedMilliseconds}ms.");
    }
}


