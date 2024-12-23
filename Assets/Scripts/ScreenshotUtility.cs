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
        // ����һ��������󣬱���ÿ�ν�ͼ�������µ� Texture2D
        Texture2D screenTexture = new Texture2D(1080, 2400, TextureFormat.RGB24, false);

        // �ȴ���ǰ֡��Ⱦ���
        yield return new WaitForEndOfFrame();

        // ����Ļ��ȡ���� (0,0)��Ϊ���½ǣ����ⲻ��Ҫ���ڴ渴��
        screenTexture.ReadPixels(new Rect(0, 0, 1080, 2400), 0, 0);
        screenTexture.Apply(); // ��������

        // ����ΪPNG�ֽ�����
        byte[] pngBytes = screenTexture.EncodeToPNG();

        // �ͷ�������Դ�������ڴ�й©
        Object.DestroyImmediate(screenTexture);

        // ��PNG�ֽ�����д��ָ���ļ�·��
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
        // ����һ��������󣬽��ͷֱ��������Ч��
        Texture2D screenTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);

        // �ȴ���ǰ֡��Ⱦ���
        yield return new WaitForEndOfFrame();

        // ����Ļ��ȡ���ز����ŵ�Ŀ��ֱ���
        screenTexture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        screenTexture.Apply(); // ��������

        // ����ΪPNG�ֽ�����
        byte[] pngBytes = screenTexture.EncodeToPNG();

        // �ͷ�������Դ
        Object.DestroyImmediate(screenTexture);

        // ��PNG�ֽ�����д��ָ���ļ�·��
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


