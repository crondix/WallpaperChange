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
                    .WriteTo.File($"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}/����.log")
                    .CreateLogger();

                ApplicationConfiguration.Initialize();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var serviceProvider = new ServiceCollection()
                    .AddSingleton<Form1>() // �������� Form1 � ������-���������
                    .AddSingleton<WallpaperService>()  // �������� WallpaperService � ������-���������
                    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog())
                    .BuildServiceProvider();

                using (var mainForm = serviceProvider.GetRequiredService<Form1>())
                {
                    // ������� �������� �����
                    mainForm.Hide();

                    // ����� ��������� �������� ���� ����������
                    Application.Run();

                    // ����� ��������� ����� ���������� ���������� �����
                    mainForm.Show();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "��������� ������ ��� ������� ���������� WinForms");
                Environment.Exit(1);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
    }
