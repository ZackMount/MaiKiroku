using Assets.Scripts.Api.Lxns.Models;
using Assets.Scripts.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Initializer
    {
        public void InitializeSprites(string directoryPath, Dictionary<string, Sprite> spriteCache, string spriteType)
        {
            if (!Directory.Exists(directoryPath))
            {
                Logger.Warn($"{spriteType} directory does not exist: {directoryPath}");
                return;
            }

            string[] supportedExtensions = new[] { ".png" };

            string[] imageFiles = Directory.GetFiles(directoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                           .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                                           .ToArray();

            if (imageFiles.Length == 0)
            {
                Logger.Warn($"No PNG image files found in directory: {directoryPath}");
                return;
            }

            foreach (string filePath in imageFiles)
            {
                if (spriteCache.ContainsKey(filePath))
                {
                    Logger.Info($"Image already cached, skipping load: {filePath}");
                    continue;
                }

                if (File.Exists(filePath))
                {
                    try
                    {
                        byte[] imageData = File.ReadAllBytes(filePath);
                        Texture2D texture = new Texture2D(2, 2);

                        if (texture.LoadImage(imageData))
                        {
                            Sprite sprite = Sprite.Create(
                                texture,
                                new Rect(0, 0, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f)
                            );

                            spriteCache[filePath] = sprite;

                            Logger.Debug($"Successfully loaded and cached image: {filePath}");
                        }
                        else
                        {
                            Logger.Error($"Failed to load texture from image data: {filePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal($"Exception occurred while loading image: {filePath}\n{ex.Message}");
                    }
                }
                else
                {
                    Logger.Error($"Image not found: {filePath}");
                }
            }

            Logger.Info($"{spriteType} images initialization completed.");
        }

        public void InitializeJacketSprites()
        {
            InitializeSprites(ApplicationConstants.JacketPath, ApplicationConstants.JacketSpriteCache, "Jacket");
        }

        public void InitializeIconSprites()
        {
            InitializeSprites(ApplicationConstants.IconPath, ApplicationConstants.IconSpriteCache, "Icon");
        }

        public void InitializePlateSprites()
        {
            InitializeSprites(ApplicationConstants.PlatePath, ApplicationConstants.PlateSpriteCache, "Plate");
        }

    }
}
