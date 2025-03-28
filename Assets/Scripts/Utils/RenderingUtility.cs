using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public static class RenderingUtility
{
    /// <summary>
    /// 捕获指定Canvas画面并返回JPEG图片的字节数组。
    /// </summary>
    /// <param name="targetCanvas">需要渲染的Canvas。</param>
    /// <param name="canvasWidth">截图的宽度。</param>
    /// <param name="canvasHeight">截图的高度。</param>
    /// <param name="jpgQuality">JPEG压缩质量（1-100）。</param>
    /// <returns>JPEG图片的字节数组。</returns>
    public static byte[] CaptureCanvasScreenshotSync(Canvas targetCanvas, int canvasWidth = 1080, int canvasHeight = 2400, int jpgQuality = 100)
    {
        if (targetCanvas.renderMode != RenderMode.WorldSpace)
        {
            Logger.Error("The target Canvas render mode must be set to World Space.");
            return null;
        }

        int canvasLayer = targetCanvas.gameObject.layer;
        string layerName = LayerMask.LayerToName(canvasLayer);
        if (string.IsNullOrEmpty(layerName))
        {
            Logger.Error("The target Canvas is not assigned to any layer. Please assign a unique layer to the Canvas.");
            return null;
        }

        Logger.Info($"Canvas is on Layer {canvasLayer} ({layerName})");

        RectTransform canvasRect = targetCanvas.GetComponent<RectTransform>();
        if (canvasRect == null)
        {
            Logger.Error("The Canvas is missing a RectTransform component.");
            return null;
        }

        float canvasWidthWorld = canvasRect.rect.width / targetCanvas.scaleFactor;
        float canvasHeightWorld = canvasRect.rect.height / targetCanvas.scaleFactor;

        Logger.Debug($"Canvas world dimensions: Width = {canvasWidthWorld}, Height = {canvasHeightWorld}");

        RenderTexture rt = new RenderTexture(canvasWidth, canvasHeight, 24, RenderTextureFormat.ARGB32);
        rt.Create();
        Logger.Debug("RenderTexture created.");

        // 创建临时相机
        GameObject cameraObj = new GameObject("CanvasCaptureCamera");
        Camera captureCamera = cameraObj.AddComponent<Camera>();
        captureCamera.orthographic = true;
        captureCamera.orthographicSize = canvasHeightWorld / 2f;
        captureCamera.transform.position = targetCanvas.transform.position + new Vector3(0, 0, -10f);
        captureCamera.transform.rotation = Quaternion.identity;
        captureCamera.cullingMask = 1 << canvasLayer;
        captureCamera.backgroundColor = Color.clear;
        captureCamera.clearFlags = CameraClearFlags.Color;
        captureCamera.aspect = (float)canvasWidth / canvasHeight;

        Logger.Debug($"Capture Camera settings: orthographicSize = {captureCamera.orthographicSize}, position = {captureCamera.transform.position}, aspect = {captureCamera.aspect}");

        captureCamera.targetTexture = rt;

        // 渲染相机
        captureCamera.Render();

        // 读取渲染结果
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D screenTexture = new Texture2D(canvasWidth, canvasHeight, TextureFormat.RGB24, false);
        screenTexture.ReadPixels(new Rect(0, 0, canvasWidth, canvasHeight), 0, 0);
        screenTexture.Apply();

        RenderTexture.active = currentRT;

        // 清理临时资源
        captureCamera.targetTexture = null;
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(cameraObj);
        Logger.Debug("Temporary camera and RenderTexture destroyed.");

        // 编码为JPEG
        byte[] jpgBytes = screenTexture.EncodeToJPG(jpgQuality);
        Object.DestroyImmediate(screenTexture);

        Logger.Info($"Image has been successfully captured and encoded in {System.Diagnostics.Stopwatch.StartNew().ElapsedMilliseconds}ms.");

        return jpgBytes;
    }


    private class RenderingRunner : MonoBehaviour
    {
        private static RenderingRunner _instance;

        public static RenderingRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject runnerObj = new GameObject("RenderingRunner");
                    _instance = runnerObj.AddComponent<RenderingRunner>();
                    Object.DontDestroyOnLoad(runnerObj);
                }
                return _instance;
            }
        }
    }

    private struct CameraParameters
    {
        public RenderTexture RenderTexture;
        public Camera WorldCamera;
        public Rect Viewport;
        public Color BackgroundColor;
        public CameraClearFlags ClearFlags;
    }
}
