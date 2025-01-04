using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Api.Lxns.Models;
using Assets.Scripts.Converters;
using System.Linq;
using System;

public class ScoreItemManager : MonoBehaviour
{
    public ScoreItemController prefab;

    // 用来存储所有生成的 ScoreItem
    private List<ScoreItemController> scoreItems = new List<ScoreItemController>();

    void Start()
    {
        Logger.Debug("Initializing ScoreItemManager.");
        Vector2 startPos = new Vector2(-360f, 800f);

        float xOffset = 240f;
        float yOffset = 140f;

        int itemPerRow = 4;
        int rowCount = 8;

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < itemPerRow; col++)
            {
                float xPos = startPos.x + col * xOffset;
                float yPos = startPos.y - row * yOffset;
                Vector3 spawnPos = new Vector3(xPos, yPos, 0f);

                ScoreItemController newItem = Instantiate(prefab, spawnPos, Quaternion.identity);
                newItem.transform.SetParent(transform, false);
                scoreItems.Add(newItem);

            }
        }
    }

    public void Load(Best50 best50)
    {
        if (best50 == null)
        {
            Logger.Error("Best50 data is null. Aborting Load operation.");
            return;
        }

        for (int i = 0; i < scoreItems.Count; i++)
        {
            if (i < best50.standard.Length)
            {
                var score = ScoreConverter.ConvertScoreAsync(best50.standard[i], i + 1);
                scoreItems[i].Load(score);
            }
            else
            {
                Logger.Warn($"No more Best50 data available to load for ScoreItem at index {i}.");
                scoreItems[i].Load(null);
            }
        }
        Logger.Info("Completed loading Best50 data into ScoreItems.");
    }
}
