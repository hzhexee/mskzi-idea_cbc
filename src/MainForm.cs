using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace IdeaCbcApp
{
    public partial class MainForm : Form
    {
        private string inputFilePath = string.Empty;
        private string outputFilePath = string.Empty;
        
        public MainForm()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.Text = "IDEA CBC Шифрование";
            this.Width = 600;
            this.Height = 400;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Создание элементов управления
            Label lblKey = new Label
            {
                Text = "Ключ шифрования (16 символов):",
                Location = new Point(20, 20),
                Width = 250
            };
            
            TextBox txtKey = new TextBox
            {
                Text = "1234567890abcdef",
                Location = new Point(20, 45),
                Width = 300
            };
            
            Label lblInputFile = new Label
            {
                Text = "Входной файл:",
                Location = new Point(20, 80),
                Width = 150
            };
            
            TextBox txtInputFile = new TextBox
            {
                ReadOnly = true,
                Location = new Point(20, 105),
                Width = 450
            };
            
            Button btnSelectInputFile = new Button
            {
                Text = "Обзор...",
                Location = new Point(480, 104),
                Width = 80
            };
            
            Label lblOutputFile = new Label
            {
                Text = "Выходной файл:",
                Location = new Point(20, 140),
                Width = 150
            };
            
            TextBox txtOutputFile = new TextBox
            {
                ReadOnly = true,
                Location = new Point(20, 165),
                Width = 450
            };
            
            Button btnSelectOutputFile = new Button
            {
                Text = "Обзор...",
                Location = new Point(480, 164),
                Width = 80
            };
            
            Button btnEncrypt = new Button
            {
                Text = "Зашифровать",
                Location = new Point(150, 220),
                Width = 120,
                Height = 40
            };
            
            Button btnDecrypt = new Button
            {
                Text = "Расшифровать",
                Location = new Point(320, 220),
                Width = 120,
                Height = 40
            };
            
            Label lblStatus = new Label
            {
                Text = "Готово к работе",
                Location = new Point(20, 300),
                Width = 560,
                Height = 40,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Добавление обработчиков событий
            btnSelectInputFile.Click += (sender, e) =>
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        inputFilePath = openFileDialog.FileName;
                        txtInputFile.Text = inputFilePath;
                    }
                }
            };
            
            btnSelectOutputFile.Click += (sender, e) =>
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        outputFilePath = saveFileDialog.FileName;
                        txtOutputFile.Text = outputFilePath;
                    }
                }
            };
            
            btnEncrypt.Click += (sender, e) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(inputFilePath) || string.IsNullOrEmpty(outputFilePath))
                    {
                        lblStatus.Text = "Ошибка: Выберите входной и выходной файлы";
                        return;
                    }
                    
                    string keyText = txtKey.Text;
                    if (keyText.Length != 16)
                    {
                        lblStatus.Text = "Ошибка: Ключ должен содержать ровно 16 символов";
                        return;
                    }
                    
                    byte[] key = Encoding.UTF8.GetBytes(keyText);
                    IdeaCbcExample.EncryptFile(inputFilePath, outputFilePath, key);
                    lblStatus.Text = "Файл успешно зашифрован";
                }
                catch (Exception ex)
                {
                    lblStatus.Text = $"Ошибка шифрования: {ex.Message}";
                }
            };
            
            btnDecrypt.Click += (sender, e) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(inputFilePath) || string.IsNullOrEmpty(outputFilePath))
                    {
                        lblStatus.Text = "Ошибка: Выберите входной и выходной файлы";
                        return;
                    }
                    
                    string keyText = txtKey.Text;
                    if (keyText.Length != 16)
                    {
                        lblStatus.Text = "Ошибка: Ключ должен содержать ровно 16 символов";
                        return;
                    }
                    
                    byte[] key = Encoding.UTF8.GetBytes(keyText);
                    IdeaCbcExample.DecryptFile(inputFilePath, outputFilePath, key);
                    lblStatus.Text = "Файл успешно расшифрован";
                }
                catch (Exception ex)
                {
                    lblStatus.Text = $"Ошибка расшифрования: {ex.Message}";
                }
            };
            
            // Добавление элементов на форму
            this.Controls.AddRange(new Control[] {
                lblKey, txtKey, 
                lblInputFile, txtInputFile, btnSelectInputFile,
                lblOutputFile, txtOutputFile, btnSelectOutputFile,
                btnEncrypt, btnDecrypt, lblStatus
            });
        }
    }
}
