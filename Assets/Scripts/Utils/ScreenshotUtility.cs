using UnityEngine;
using System.IO;
using System.Collections;
using System.Diagnostics;

public static class ScreenshotUtility
{
    /// <summary>
    /// ��ȡ��ǰ��Ļ���沢����ΪPNG�ļ���ָ��·�����첽��������
    /// ע�⣺������Э���е��ô˺�����ȷ�������������̡߳�
    /// </summary>
    /// <param name="savePath">��ͼ����·���������ļ�������չ��(.png)</param>
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
    /// ��ȡ��ǰ��Ļ���沢����Ϊ�ϵͷֱ��ʵ�PNG�ļ���ָ��·�����첽��������
    /// ͨ�����ͷֱ�������߽�ͼЧ�ʡ�
    /// </summary>
    /// <param name="savePath">��ͼ����·���������ļ�������չ��(.png)</param>
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


