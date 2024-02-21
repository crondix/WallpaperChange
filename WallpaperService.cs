using Microsoft.Extensions.Logging;
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
            string[] imageFiles = GetImageFiles(folderPath);
            if (imageFiles.Length > 0)
            {
                object currentValue = Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Colors", "Background", null);
                if (lastWallpaperPath != imageFiles[0] || (currentValue != null && !string.IsNullOrWhiteSpace(currentValue.ToString())))
                {
                    if (!string.Equals(currentWallpaperPath, lastWallpaperPath, StringComparison.OrdinalIgnoreCase)|| (currentValue != null && !string.IsNullOrWhiteSpace(currentValue.ToString())))
                    {
                        _logger.LogInformation("Обнаружили изменение бекграунда");
                        
                        if (currentValue != null && !string.IsNullOrWhiteSpace(currentValue.ToString()))
                        {
                            _logger.LogInformation("Фон установлен в виде сплошного цвета");
                            // Установка значения "" только если текущее значение не пустое
                            Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Colors", "Background", "");
                          
                        }
                        else
                        {
                            _logger.LogInformation($"currentWallpaperPath={currentWallpaperPath}");
                            _logger.LogInformation($"lastWallpaperPath={lastWallpaperPath}");
                        }
                        
                         
                        Wallpaper.Set(imageFiles[0], _logger);
                        
                        _logger.LogInformation($"Wallpaper Set task is started for: {imageFiles[0]}");
                       
                 
                        _logger.LogInformation($"Wallpaper changed: {lastWallpaperPath}");
                    }
                }
            }
            else
            {
                _logger.LogError("Ошибка: Нет файлов изображений для установки обоев.");
                
             
            }
        } 
    

        private void InitializeWallpaper()
        {
            string[] imageFiles = GetImageFiles(folderPath);
          

            if (imageFiles.Length > 0)
            {
                lastWallpaperPath = imageFiles[0];
                Wallpaper.Set(lastWallpaperPath, _logger);
                object currentValue = Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Colors", "Background", null);
                if (currentValue != null && !string.IsNullOrWhiteSpace(currentValue.ToString()))
                {
                    _logger.LogInformation("Изначально фон был установлен в виде сплошного цвета");
                    Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Colors", "Background", "");
                   
                }
                lastWallpaperPath = GetCurrentWallpaperPath();
                _logger.LogInformation($"Инициализация:Фон изменен на предоставленный файл изображения");
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

        public static string[] GetImageFiles(string folderPath)
        {
            return Directory.GetFiles(folderPath, "*.jpg", SearchOption.TopDirectoryOnly)
                .Concat(Directory.GetFiles(folderPath, "*.png", SearchOption.TopDirectoryOnly))
                .ToArray();
        }
    }
}
