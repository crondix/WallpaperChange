﻿using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService1
{
    public class WallpaperService
    {
        private readonly ILogger<WallpaperService> _logger;
        private string lastWallpaperPath;
        private string folderPath;

        public WallpaperService(ILogger<WallpaperService> logger)
        {
            _logger = logger;
            folderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public void Start()
        {
            _logger.LogInformation("Service is starting...");
            InitializeWallpaper();
            _logger.LogInformation("Service started successfully.");
        }

        public void Stop()
        {
            _logger.LogInformation("Service is stopping...");
        }

        public void CheckWallpaperChange()
        {
            string currentWallpaperPath = GetCurrentWallpaperPath();

            if (!string.Equals(currentWallpaperPath, lastWallpaperPath, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Обнаружили изменение бекграунда");
                string[] imageFiles = GetImageFiles();

                if (imageFiles.Length > 0)
                {
                    Wallpaper.Set(imageFiles[0], _logger);
                    _logger.LogInformation($"Wallpaper Set task is started for: {imageFiles[0]}");
                }
                else
                {
                    _logger.LogError("Ошибка: Нет файлов изображений для установки обоев.");
                }
                lastWallpaperPath = GetCurrentWallpaperPath();
                _logger.LogInformation($"Wallpaper changed: {lastWallpaperPath}");
            }
        }

        private void InitializeWallpaper()
        {
            string[] imageFiles = GetImageFiles();
          

            if (imageFiles.Length > 0)
            {
                lastWallpaperPath = imageFiles[0];
                Wallpaper.Set(lastWallpaperPath, _logger);
               
            }
            else
            {
                _logger.LogError("Ошибка: Файл изображения не найден.");
            }
        }

        private string GetCurrentWallpaperPath()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", false);
            return key?.GetValue("Wallpaper").ToString();
        }

        private string[] GetImageFiles()
        {
            return Directory.GetFiles(folderPath, "*.jpg", SearchOption.TopDirectoryOnly)
                .Concat(Directory.GetFiles(folderPath, "*.png", SearchOption.TopDirectoryOnly))
                .ToArray();
        }
    }
}