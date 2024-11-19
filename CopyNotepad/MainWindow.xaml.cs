using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CopyNotepad
{
    public enum StatusElement
    {
        New,
        Saved,
        NotSaved
    }

    public class Document
    {
        public StatusElement Status { get; private set; }
        public string FilePath { get; private set; } = "";
        public string FileName { get; private set; } = "Notepad";
        public string Text { get; private set; } = "";

        public Document()
        {
            Status = StatusElement.New;
        }

        public void Save()
        {
            Status = StatusElement.Saved;
        }

        public void NotSave()
        {
            Status = StatusElement.NotSaved;
        }

        public void SetFilePath(string path)
        {
            FilePath = path;
            FileName = System.IO.Path.GetFileName(path);
        }

        public void SetText(string text)
        {
            Text = text;
        }
    }

    public partial class MainWindow : Window
    {
        Document Document;

        public MainWindow()
        {
            InitializeComponent();
            Document = new Document();
        }

        // Функция для "сохранить" файл
        public void SaveProjectScript()
        {
            if (Document.Text == FullText.Text) return;

            if (Document.FilePath == "")
            {
                SaveAsProjectScript();
            }
            else
            {
                string text = FullText.Text;
                File.WriteAllText(Document.FilePath, text);
                Document.Save();
                Title = Document.FileName;
            }
        }

        // Функция для "сохранить как" файл
        public void SaveAsProjectScript()
        {
            string text = FullText.Text;

            SaveFileDialog dialog = new SaveFileDialog()
            {
                FileName = "Unnamed",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt|HTML-document|*.html"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, text);

                Document.SetFilePath(dialog.FileName);
                Document.Save();
                Title = Document.FileName;
            }
        }

        public void ZoomIn()
        {
            FullText.FontSize += 5;
        }

        public void ZoomOut()
        {
            FullText.FontSize -= 5;
        }

        public void DefaultZoom()
        {
            FullText.FontSize = 11;
        }

        private void SaveProjectClick(object sender, RoutedEventArgs e)
        {
            SaveProjectScript();
        }

        private void SaveAsProjectClick(object sender, RoutedEventArgs e)
        {
            SaveAsProjectScript();
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileName.EndsWith(".txt") || dialog.FileName.EndsWith(".html"))
                {
                    string text = File.ReadAllText(dialog.FileName);
                    FullText.Text = text;
                    Document.SetFilePath(dialog.FileName);
                    Document.Save();
                    Title = Document.FileName;
                }
                else
                {
                    MessageBox.Show("Формат этого файла не поддерживается", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ZoomInClick(object sender, RoutedEventArgs e)
        {
            ZoomIn();
        }

        private void ZoomOutClick(object sender, RoutedEventArgs e)
        {
            ZoomOut();
        }

        private void DefaultZoomClick(object sender, RoutedEventArgs e)
        {
            DefaultZoom();
        }

        private void CheckUpdateChanged(object sender, TextChangedEventArgs e)
        {
            Document.NotSave();
            if (Document.FileName != "Notepad")
            {
                Title = "*" + Document.FileName;
            }
        }

        private void KeyScript(object sender, KeyEventArgs e)
        {
            // Проверяем Ctrl
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                // Проверяем Shift
                if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
                {
                    // При CTRL+SHIFT+S - Save As...
                    if (e.Key == Key.S)
                    {
                        SaveAsProjectScript();
                        e.Handled = true;
                    }
                }
                // При CTRL+S - Save
                else if (e.Key == Key.S)
                {
                    SaveProjectScript();
                    e.Handled = true;
                }
            }
        }

        private void CheckScroll(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                // CTRL + ScrollUp
                if (e.Delta > 0)
                {
                    ZoomIn();
                }
                // CTRL + ScrollDown
                else
                {
                    ZoomOut();
                }
                e.Handled = true;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (Document.Status == StatusElement.NotSaved)
            {
                e.Cancel = true;
                MessageBoxResult result = MessageBox.Show(
                    "Сохранить файл перед выходом?",
                    "Сохранение",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    if (Document.FilePath == "")
                    {
                        SaveAsProjectScript();
                    }
                    else
                    {
                        string text = FullText.Text;
                        File.WriteAllText(Document.FilePath, text);
                        Document.Save();
                        Title = Document.FileName;
                    }
                    e.Cancel = false;
                }
                else if (result == MessageBoxResult.No)
                {
                    e.Cancel = false;
                }
                else if (result == MessageBoxResult.Cancel) { }
            }
            else
            {
                e.Cancel = false;
            }
        }
    }
}
