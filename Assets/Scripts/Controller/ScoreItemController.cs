using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Models;
using System.IO;
using Assets.Scripts.Constants;
using System.Collections.Generic;

public class ScoreItemController : MonoBehaviour
{
    [Header("Raw")]
    public Image Panel;
    public Image FC;
    public Image FS;
    public Image Rank;
    public Image Cover;
    public Image InfoiconMode;

    [Header("Text")]
    public TextMeshProUGUI SongName;
    public TextMeshProUGUI RankInB50;
    public TextMeshProUGUI SongID;
    public TextMeshProUGUI DXScore;
    public TextMeshProUGUI Achievements;
    public TextMeshProUGUI Level;
    public TextMeshProUGUI Rating;

    [Header("DXScore Star")]
    public GameObject Star5;
    public GameObject Star4;
    public GameObject Star3;
    public GameObject Star2;
    public GameObject Star1;

    [Header("Panel")]
    /// <summary>
    /// 0 = BSC, 1 = ADV, 2 = EXP, 3 = MST, 4 = RE
    /// </summary>
    public Sprite[] PanelBackgroundSprites;
    /// <summary>
    /// 0 = none, 1 = FC, 2 = FCP, 3 = AP, 4 = APP
    /// </summary>
    public Sprite[] PanelFCSprites;
    /// <summary>
    /// 0 = none, 1 = SP, 2 = FS, 3 = FSP, 4 = FSD, 5 = FSDP
    /// </summary>
    public Sprite[] PanelFSSprites;
    /// <summary>
    /// 0 = D, 1 = C, 2 = B, 3 = BB, 4 = BBB, 5 = A, 6 = AA, 7 = AAA, 8 = S, 9 = SP, 10 = SS, 11 = SSP, 12 = SSS, 13 = SSSP
    /// </summary>
    public Sprite[] PanelRankSprites;
    /// <summary>
    /// 0 = Standard, 1 = Deluxe
    /// </summary>
    public Sprite[] PanelInfoiconModeSprites;

    // ¾²Ì¬»º´æ×Öµä£¬ÓÃÓÚ´æ´¢ÒÑ¼ÓÔØµÄSprite
    private static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

    public void Load(Score score)
    {
        if (score == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        SongName.text = score.song_name;
        RankInB50.text = $"#{score.rank}";
        SongID.text = $"ID:{score.song_id}";
        DXScore.text = $"DX:{score.dx_score}";
        Achievements.text = $"{score.achievements}%";
        Level.text = $"{score.level_details}¡ú";
        Rating.text = $"{score.dx_rating}";
        Panel.sprite = PanelBackgroundSprites[(int)score.level_index];
        InfoiconMode.sprite = PanelInfoiconModeSprites[(int)score.type];
        Rank.sprite = PanelRankSprites[(int)score.rate];
        FC.sprite = PanelFCSprites[(int)score.fc];
        FS.sprite = PanelFSSprites[(int)score.fs];


        string coverPath = Path.Combine(ApplicationConstants.baseMenu, score.cover_path);
        LoadCoverImage(coverPath);
    }

    private void LoadCoverImage(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Cover image path is null or empty.");
            return;
        }

        if (spriteCache.ContainsKey(path))
        {
            Cover.sprite = spriteCache[path];
            Cover.preserveAspect = false;
            return;
        }

        if (File.Exists(path))
        {
            byte[] imageData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                spriteCache[path] = sprite;

                Cover.sprite = sprite;
                Cover.preserveAspect = false;
            }
            else
            {
                Debug.LogError("Failed to load texture from image data.");
            }
        }
        else
        {
            Debug.LogError($"Image not found at path: {path}");
        }
    }


    private void OnDestroy()
    {
        spriteCache.Clear();
    }
}
