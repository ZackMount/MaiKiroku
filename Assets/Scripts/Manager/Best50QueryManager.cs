using System;
using System.IO;
using UnityEngine;
using Assets.Scripts.Constants;
using Assets.Scripts.Api.Lxns;
using Assets.Scripts.Manager;
using Assets.Scripts.Converters;
using Assets.Scripts.Api.Lxns.Models;

public class Best50QueryManager : MonoBehaviour
{
    private string _outputsDirectory =
        Path.Combine(ApplicationConstants.BasePath, "outputs");

    [Header("Managers / Components")]
    public ScoreItemManager scoreItemManager;
    public PlayerInfoManager playerInfoManager;
    public Canvas targetCanvas;

    private void Start()
    {
        // 创建输出目录（如果不存在）
        Directory.CreateDirectory(_outputsDirectory);
    }

    /// <summary>
    /// 同步方法：查询指定QQ的Best50数据，加载到各管理器，然后捕获Canvas截图并返回图片的字节数组。
    /// 注意：此方法会阻塞主线程，直到网络请求和截图完成！
    /// </summary>
    /// <param name="qqId">要查询的QQ号</param>
    /// <returns>渲染完成的截图Byte[]。如果出现异常或无数据则可能返回null或抛出异常。</returns>
    public byte[] QueryBest50Async(string qqId)
    {
        Logger.Info($"[Fetch] Starting query for QQ ID: {qqId}");

        // 构造网络请求
        var requests = new Requests(ApplicationConstants.Token);

        try
        {
            // 同步等待网络请求完成
            var best50Task = requests.GetBest50ByQQAsync(qqId);
            best50Task.Wait(); // 等同于 .Result，但更能明确阻塞调用
            var result = best50Task.Result; // (Best50, Player)

            var best50 = result.Item1;
            var player = result.Item2;

            // 判空
            if (best50 == null || (best50.standard == null && best50.dx == null))
            {
                Logger.Warn($"[Fetch] No data found for QQ ID={qqId}. Returning null.");
                return null;
            }

            Logger.Info($"[Fetch] Data for QQ ID={qqId} retrieved successfully.");

            // 将Best50和Player数据加载到对应的Manager
            Logger.Info("[Render] Starting to render Best50 data...");
            scoreItemManager.Load(best50);

            var playerInfo = PlayerConverter.ConvertPlayerAsync(player);
            playerInfoManager.Load(playerInfo);
            Logger.Info("[Render] Data loaded into managers successfully.");

            // 同步截图
            var imageBytes = RenderingUtility.CaptureCanvasScreenshotSync(targetCanvas, 1080, 2400, 100);
            Logger.Info("[Render] Screenshot captured successfully.");

            return imageBytes;
        }
        catch (Exception ex)
        {
            Logger.Error($"[Fetch] Failed to retrieve Best50 for QQ ID={qqId}: {ex.Message}", ex);
            // 这里可选择返回null或往外继续抛异常
            throw;
        }
    }

    /// <summary>
    /// 示例：单纯捕获并保存当前Canvas截图到本地输出目录。
    /// </summary>
    private void TakeScreenshot()
    {
        Directory.CreateDirectory(_outputsDirectory);

        string fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}.jpg";
        string outputsFilePath = Path.Combine(_outputsDirectory, fileName);

        // 使用同步截图方法
        var imageBytes = RenderingUtility.CaptureCanvasScreenshotSync(targetCanvas);

        if (imageBytes != null)
        {
            File.WriteAllBytes(outputsFilePath, imageBytes);
            Logger.Info($"[Render] Screenshot saved to: {outputsFilePath}");
        }
        else
        {
            Logger.Warn("[Render] Screenshot failed or returned null byte array.");
        }
    }
}
