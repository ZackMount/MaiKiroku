using Assets.Scripts.Constants;
using Assets.Scripts.Models;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Manager
{
    public class PlayerInfoManager : MonoBehaviour
    {
        public RatingNumberManager Rating;
        public Image Icon;
        public Image DXPanel;
        public TextMeshProUGUI Name;

        [Header("DX Rating Panel")]

        public Sprite[] DXRatingPanelSprites;


        public void Load(Player player)
        {
            Rating.Load(player.rating.ToString());
            Name.text = player.name;
            LoadIcon(player.icon_path);
            DXPanel.sprite = player.rating switch
            {
                >= 15000 => DXRatingPanelSprites[10],
                >= 14500 => DXRatingPanelSprites[9],
                >= 14000 => DXRatingPanelSprites[8],
                >= 13000 => DXRatingPanelSprites[7],
                >= 12000 => DXRatingPanelSprites[6],
                >= 10000 => DXRatingPanelSprites[5],
                >= 7000 => DXRatingPanelSprites[4],
                >= 4000 => DXRatingPanelSprites[3],
                >= 2000 => DXRatingPanelSprites[2],
                >= 1000 => DXRatingPanelSprites[1],
                _ => DXRatingPanelSprites[0],
            };
        }

        private void LoadIcon(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Cover image path is null or empty.");
                return;
            }

            if (ApplicationConstants.IconSpriteCache.ContainsKey(path))
            {
                Icon.sprite = ApplicationConstants.IconSpriteCache[path];
                Icon.preserveAspect = false;
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

                    ApplicationConstants.IconSpriteCache[path] = sprite;

                    Icon.sprite = sprite;
                    Icon.preserveAspect = false;
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
    }
}


