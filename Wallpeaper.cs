using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Serilog;
using Microsoft.Extensions.Logging;
namespace WorkerService1
{
    public static class Wallpaper
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        const int SPI_SETDESKWALLPAPER = 0x0014;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDCHANGE = 0x02;

        public static void Set(string path, ILogger<WallpaperService> logger)
        {
            
            
            //Console.WriteLine($"Попытка установить обои с путем: {path}");
            if (File.Exists(path))
            {
                //Console.WriteLine("Файл изображения существует.");
                System.Drawing.Image img = System.Drawing.Image.FromFile(path);

                using (var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height))
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(img, 0, 0, bmp.Width, bmp.Height);
                    }

                    string tempPath = Path.Combine(Path.GetTempPath(), "tempWallpaper.bmp");
                    bmp.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);
                    SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, tempPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
                    //SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
                    Thread.Sleep(1000);
                
                }
               
            }
            else
            {
                logger.LogInformation($"Ошибка: Файл изображения не найден по пути {path}");
            }
        }
    }
}
