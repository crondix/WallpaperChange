using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WorkerService1;

namespace WallpaperChange
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File($"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}/ЛОГИ.log")
                    .CreateLogger();

                ApplicationConfiguration.Initialize();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var serviceProvider = new ServiceCollection()
                    .AddSingleton<Form1>() // Добавьте Form1 в сервис-контейнер
                    .AddSingleton<WallpaperService>()  // Добавьте WallpaperService в сервис-контейнер
                    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog())
                    .BuildServiceProvider();

                using (var mainForm = serviceProvider.GetRequiredService<Form1>())
                {
                    // Сначала скрываем форму
                    mainForm.Hide();

                    // Затем запускаем основной цикл приложения
                    Application.Run();

                    // После окончания цикла приложения показываем форму
                    mainForm.Show();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Произошла ошибка при запуске приложения WinForms");
                Environment.Exit(1);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
    }
