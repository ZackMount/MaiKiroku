using System;
using System.IO;
using UnityEngine;
using System.Collections;

public class ScreenshotOnKeyPress : MonoBehaviour
{
    // ָ�����Ŀ¼·��
    private string outputsDirectory = "D:\\Unity Project\\maibot\\Build\\outputs";

    void Update()
    {
        // ����Ƿ�����W��
        if (Input.GetKeyDown(KeyCode.W))
        {
            TakeScreenshot();
        }
    }

    private void TakeScreenshot()
    {
        // �������Ŀ¼����������ڣ�
        Directory.CreateDirectory(outputsDirectory);

        // �����ļ�·�����ļ���������ǰ UTC ����
        string outputsFilePath = Path.Combine(outputsDirectory, $"outputs_{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff")}.jpg");

        // ����Э�̽��н�ͼ
        StartCoroutine(ScreenshotUtility.CaptureScreenshotAsync(outputsFilePath));
    }
}
