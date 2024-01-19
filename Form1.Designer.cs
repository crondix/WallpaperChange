using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WorkerService1;


namespace WallpaperChange
{
    partial class Form1 : Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem closeMenuItem;
        private RichTextBox consoleTextBox;
        private WallpaperService _wallpaperService;
        private System.Windows.Forms.Timer wallpaperCheckTimer;
        public Form1(WallpaperService wallpaperService)
        {
       
            InitializeComponent();
            InitializeNotifyIcon();
            InitializeContextMenu();
            InitializeConsole();
            _wallpaperService = wallpaperService;
            _wallpaperService.Start();
            string[] imageFiles = WallpaperService.GetImageFiles(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            if (imageFiles.Length > 0)
            {
                // Инициализация таймера
                wallpaperCheckTimer = new System.Windows.Forms.Timer();
                wallpaperCheckTimer.Interval = 2000; // Период проверки в миллисекундах (здесь 2 секунды)
                wallpaperCheckTimer.Tick += WallpaperCheckTimer_Tick;
                wallpaperCheckTimer.Start();
            }
            else
            {
                Log.Error("Ошибка: Файл изображения не найден. Добавте изображение в кореневую папку прогррамы. ");             
            }
            this.Text = "WalpeaperChange";
            this.Icon = new Icon(GetType(), "Icon.ico");
        }
        private void WallpaperCheckTimer_Tick(object sender, EventArgs e)
        {
            // Проверка изменения обоев
            _wallpaperService.CheckWallpaperChange();
        }
        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(GetType(), "Icon.ico"), // Используйте "Icon.ico" вместо "trayIcon.ico"
                Visible = true
            };

            notifyIcon.MouseDown += NotifyIcon_MouseDown;
            notifyIcon.ContextMenuStrip = contextMenuStrip;
        }

        private void InitializeContextMenu()
        {
            contextMenuStrip = new ContextMenuStrip();

            // Добавление пункта меню "Закрыть"
            closeMenuItem = new ToolStripMenuItem("Закрыть");
            closeMenuItem.Click += CloseMenuItem_Click;

            contextMenuStrip.Items.Add(closeMenuItem);
        }

        private void InitializeConsole()
        {
            consoleTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                WordWrap = true,
                BackColor = Color.Black,
                ForeColor = Color.White
            };

            // Добавление окна вывода в форму
            Controls.Add(consoleTextBox);

            // Перенаправление вывода в окно вывода
            Console.SetOut(new ControlWriter(consoleTextBox));
        }

        private void NotifyIcon_MouseDown(object sender, MouseEventArgs e)
        {
            // Показать консоль по двойному щелчку
            if (e.Button == MouseButtons.Left && e.Clicks == 2)
            {
               
                if (consoleTextBox.Visible)
                {
                    consoleTextBox.Visible = false;
                }
                else
                {
                    consoleTextBox.Visible = true;
                    this.Visible = !this.Visible; // Показать форму, если она была скрыта
                }
            }

            // Отображение контекстного меню при нажатии правой кнопки мыши
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(Cursor.Position);
            }
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            // Обработка нажатия кнопки "Закрыть"
            Application.Exit();
            
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Убедимся, что мы скрываем форму, а не закрываем её
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;  // Отменить действие по умолчанию
                this.Hide();      // Скрыть форму вместо её закрытия
            }
        }
        private class ControlWriter : System.IO.TextWriter
        {
            
            private readonly RichTextBox _textBox;

            public ControlWriter(RichTextBox textBox)
            {
                _textBox = textBox;
            }

            public override void Write(char value)
            {
                try
                {
                    _textBox.AppendText(value.ToString());
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Произошла ошибка в _textBox.AppendText(value.ToString())");
                   
                }
            }

            public override void Write(string value)
            {
                try
                {
                    _textBox.AppendText(value);
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Произошла ошибка в   _textBox.AppendText(value);");
                    
                }
            }

            public override System.Text.Encoding Encoding
            {
                get { return System.Text.Encoding.UTF8; }
            }
        
      
        }
       

    //protected override void Dispose(bool disposing)
    //{
    //    if (disposing && (components != null))
    //    {
    //        components.Dispose();
    //    }
    //    base.Dispose(disposing);
    //}

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}