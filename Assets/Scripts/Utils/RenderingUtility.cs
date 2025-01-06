using System.Collections;
using System.IO;
using UnityEngine;

public static class RenderingUtility
{
    /// <summary>
    /// 捕获指定Canvas画面并保存为JPEG图片。
    /// </summary>
    /// <param name="targetCanvas">需要渲染的Canvas。</param>
    /// <param name="savePath">保存截图的路径。</param>
    /// <param name="canvasWidth">截图的宽度。</param>
    /// <param name="canvasHeight">截图的高度。</param>
    /// <param name="jpgQuality">JPEG压缩质量（1-100）。</param>
    public static void CaptureCanvasScreenshot(Canvas targetCanvas, string savePath, int canvasWidth = 1080, int canvasHeight = 2400, int jpgQuality = 80)
    {
        RenderingRunner.Instance.StartCoroutine(CaptureCanvasScreenshotCoroutine(targetCanvas, savePath, canvasWidth, canvasHeight, jpgQuality));
    }

    private static IEnumerator CaptureCanvasScreenshotCoroutine(Canvas targetCanvas, string savePath, int canvasWidth, int canvasHeight, int jpgQuality)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        if (targetCanvas.renderMode != RenderMode.WorldSpace)
        {
            Logger.Error("The target Canvas render mode must be set to World Space.");
            yield break;
        }

        int canvasLayer = targetCanvas.gameObject.layer;
        string layerName = LayerMask.LayerToName(canvasLayer);
        if (string.IsNullOrEmpty(layerName))
        {
            Logger.Error("The target Canvas is not assigned to any layer. Please assign a unique layer to the Canvas.");
            yield break;
        }

        Logger.Info($"Canvas is on Layer {canvasLayer} ({layerName})");

        RectTransform canvasRect = targetCanvas.GetComponent<RectTransform>();
        if (canvasRect == null)
        {
            Logger.Error("The Canvas is missing a RectTransform component.");
            yield break;
        }
        float canvasWidthWorld = canvasRect.rect.width / targetCanvas.scaleFactor;
        float canvasHeightWorld = canvasRect.rect.height / targetCanvas.scaleFactor;

        Logger.Debug($"Canvas world dimensions: Width = {canvasWidthWorld}, Height = {canvasHeightWorld}");

        RenderTexture rt = new RenderTexture(canvasWidth, canvasHeight, 24, RenderTextureFormat.ARGB32);
        rt.Create();
        Logger.Debug("RenderTexture created.");

        Material tempMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
        tempMaterial.SetPass(0);

        CameraParameters cameraParams = new CameraParameters
        {
            RenderTexture = rt,
            WorldCamera = null,
            Viewport = new Rect(0, 0, 1, 1),
            BackgroundColor = Color.clear,
            ClearFlags = CameraClearFlags.Color
        };

        yield return new WaitForEndOfFrame();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        GL.Clear(true, true, Color.clear);

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
        captureCamera.Render();

        Texture2D screenTexture = new Texture2D(canvasWidth, canvasHeight, TextureFormat.RGB24, false);
        screenTexture.ReadPixels(new Rect(0, 0, canvasWidth, canvasHeight), 0, 0);
        screenTexture.Apply();

        RenderTexture.active = currentRT;

        captureCamera.targetTexture = null;
        Object.Destroy(rt);
        Object.Destroy(cameraObj);
        Logger.Debug("Temporary camera and RenderTexture destroyed.");

        stopwatch.Stop();
        Logger.Info($"Image \"{savePath}\" has been successfully rendered and captured in {stopwatch.ElapsedMilliseconds}ms.");

        byte[] jpgBytes = screenTexture.EncodeToJPG(jpgQuality);

        Object.Destroy(screenTexture);

        try
        {
            File.WriteAllBytes(savePath, jpgBytes);
            Logger.Info($"Image \"{savePath}\" has been successfully saved in {stopwatch.ElapsedMilliseconds}ms.");
        }
        catch (System.Exception ex)
        {
            Logger.Error($"Failed to save image \"{savePath}\": {ex.Message}");
        }
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
