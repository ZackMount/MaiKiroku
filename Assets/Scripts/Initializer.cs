using Assets.Scripts.Api.Lxns.Models;
using Assets.Scripts.Constants;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Initializer
    {
        public void InitializeJacketSprites()
        {
            string jacketDirectory = ApplicationConstants.JacketPath;

            if (!Directory.Exists(jacketDirectory))
            {
                Logger.Warn($"Jacket directory does not exist: {jacketDirectory}");
                return;
            }

            string[] supportedExtensions = new[] { ".png" };

            string[] imageFiles = Directory.GetFiles(jacketDirectory, "*.png", SearchOption.TopDirectoryOnly)
                                           .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                                           .ToArray();

            if (imageFiles.Length == 0)
            {
                Logger.Warn($"No PNG image files found in directory: {jacketDirectory}");
                return;
            }

            foreach (string filePath in imageFiles)
            {
                if (ApplicationConstants.jacketSpriteCache.ContainsKey(filePath))
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

                            ApplicationConstants.jacketSpriteCache[filePath] = sprite;

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

            Logger.Info("Jacket images initialization completed.");
        }
    }
}
