using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Api.Lxns.Models;
using Assets.Scripts.Api.Lxns.Managers; 
using Assets.Scripts.Converters;

public class ScoreItemManager : MonoBehaviour
{
    public ScoreItemController prefab;
    private List<ScoreItemController> scoreItemsStandard = new List<ScoreItemController>();
    private List<ScoreItemController> scoreItemsDX = new List<ScoreItemController>();
    void Start()
    {
        Logger.Debug("Initializing Layout.");

        for(int i = 0; i < 3; i++)
        {
            ScoreItemController newItem = Instantiate(prefab, new Vector3(-135 + (265 * i), 955, 0f), Quaternion.identity);
            newItem.transform.SetParent(transform, false);
            scoreItemsStandard.Add(newItem);
        }

        Vector2 startPos = new Vector2(-400f, 800f);

        float xOffset = 265f;
        float yOffset = 155f;

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
                scoreItemsStandard.Add(newItem);

            }
        }

        for (int i = 0; i < 3; i++)
        {
            ScoreItemController newItem = Instantiate(prefab, new Vector3(-135 + (265 * i), -555, 0f), Quaternion.identity);
            newItem.transform.SetParent(transform, false);
            scoreItemsDX.Add(newItem);
        }

        startPos = new Vector2(-400f, -710f);

        xOffset = 265f;
        yOffset = 155f;

        itemPerRow = 4;
        rowCount = 3;

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < itemPerRow; col++)
            {
                float xPos = startPos.x + col * xOffset;
                float yPos = startPos.y - row * yOffset;
                Vector3 spawnPos = new Vector3(xPos, yPos, 0f);

                ScoreItemController newItem = Instantiate(prefab, spawnPos, Quaternion.identity);
                newItem.transform.SetParent(transform, false);
                scoreItemsDX.Add(newItem);

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

        for (int i = 0; i < scoreItemsStandard.Count; i++)
        {
            if (i < best50.standard.Length)
            {
                int songId = best50.standard[i].id;

                Song lxnsSong = SongManager.Instance.GetSong(songId);
                if (lxnsSong != null)
                {
                    var score = ScoreConverter.ConvertScoreAsync(best50.standard[i], lxnsSong, i + 1);

                    scoreItemsStandard[i].Load(score);
                }
                else
                {
                    Logger.Warn($"Failed to load Song with ID {songId} for ScoreItem at index {i}.");
                    scoreItemsStandard[i].Load(null);
                }
            }
            else
            {
                Logger.Warn($"No more standard Best50 data available to load for ScoreItem at index {i}.");
                scoreItemsStandard[i].Load(null);
            }
        }

        for (int i = 0; i < scoreItemsDX.Count; i++)
        {
            if (i < best50.dx.Length)
            {
                int songId = best50.dx[i].id;

                Song lxnsSong = SongManager.Instance.GetSong(songId);
                if (lxnsSong != null)
                {
                    var score = ScoreConverter.ConvertScoreAsync(best50.dx[i], lxnsSong, i + 1);

                    scoreItemsDX[i].Load(score);
                }
                else
                {
                    Logger.Warn($"Failed to load Song with ID {songId} for ScoreItem at index {i}.");
                    scoreItemsDX[i].Load(null);
                }
            }
            else
            {
                Logger.Warn($"No more dx Best50 data available to load for ScoreItem at index {i}.");
                scoreItemsDX[i].Load(null);
            }
        }


        Logger.Info("Completed loading Best50 data into ScoreItems.");
    }
}
