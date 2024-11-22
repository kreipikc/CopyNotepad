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

    internal class ProjectManager
    {
        Document Document;
        List<string> BanExtensionList = new List<string>() { ".mp3", ".mp4", ".mp5", ".wav", ".aac", ".flac", ".avi", ".mkv", ".mov" };

        public ProjectManager() 
        {
            Document = new Document();
        }

        // Вопрос о сохранении перед открытием нового файла
        void QuestionForSave(MainWindow mainWindow)
        {
            if (Document.Status == StatusElement.NotSaved)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Save the file before exiting?",
                    "Save",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    if (Document.FilePath == "")
                    {
                        SaveAsProjectScript(mainWindow);
                    }
                    else
                    {
                        string text = mainWindow.FullText.Text;
                        File.WriteAllText(Document.FilePath, text);
                        Document.Save();
                        mainWindow.Title = Document.FileName;
                    }
                }
                else if (result == MessageBoxResult.No) { }
            }
        }

        public void SaveProjectScript(MainWindow mainWindow)
        {
            if (Document.Text == mainWindow.FullText.Text) return;

            if (Document.FilePath == "")
            {
                SaveAsProjectScript(mainWindow);
            }
            else
            {
                string text = mainWindow.FullText.Text;
                File.WriteAllText(Document.FilePath, text);
                Document.Save();
                mainWindow.Title = Document.FileName;
            }
        }

        public void SaveAsProjectScript(MainWindow mainWindow)
        {
            string text = mainWindow.FullText.Text;

            SaveFileDialog dialog = new SaveFileDialog()
            {
                FileName = "Unnamed",
                DefaultExt = ".txt",
                Filter = "TXT-documents|*.txt|All files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, text);

                Document.SetFilePath(dialog.FileName);
                Document.Save();
                mainWindow.Title = Document.FileName;
            }
        }

        public void OpenFileScript(MainWindow mainWindow)
        {
            if (Document.Status == StatusElement.NotSaved) 
            {
                QuestionForSave(mainWindow);
            }

            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                if (!BanExtensionList.Contains(Path.GetExtension(dialog.FileName)))
                {
                    string text = File.ReadAllText(dialog.FileName);
                    mainWindow.FullText.Text = text;
                    Document.SetFilePath(dialog.FileName);
                    Document.Save();
                    mainWindow.Title = Document.FileName;
                }
                else
                {
                    MessageBox.Show(
                        "The file in this format is not supported!",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        public void ZoomIn(MainWindow mainWindow)
        {
            if (mainWindow.FullText.FontSize < 72)
            {
                mainWindow.FullText.FontSize += 5;
            }
        }

        public void ZoomOut(MainWindow mainWindow)
        {
            if (mainWindow.FullText.FontSize > 6)
            {
                mainWindow.FullText.FontSize -= 5;
            }
        }

        public void DefaultZoom(MainWindow mainWindow)
        {
            mainWindow.FullText.FontSize = 11;
        }

        public void ChangeTitle(MainWindow mainWindow)
        {
            Document.NotSave();
            if (Document.FileName != "Notepad")
            {
                mainWindow.Title = "*" + Document.FileName;
            }
        }

        public void DragOver(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        public void Drop(MainWindow mainWindow, DragEventArgs e)
        {
            if (Document.Status == StatusElement.NotSaved)
            {
                QuestionForSave(mainWindow);
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string FirstFile = files[0];
                if (files.Length != 1)
                {
                    MessageBox.Show(
                        $"Only one {System.IO.Path.GetFileName(files[0])} file will be scanned",
                        "Message",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }

                if (!BanExtensionList.Contains(System.IO.Path.GetExtension(FirstFile)))
                {
                    try
                    {
                        string fileContent = File.ReadAllText(FirstFile);
                        Document.SetFilePath(FirstFile);
                        Document.SetText(fileContent);
                        mainWindow.FullText.Text = fileContent;
                        mainWindow.Title = Document.FileName;
                        Document.Save();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error reading the file {files[0]}: {ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
                else
                {
                    MessageBox.Show(
                        "The file in this format is not supported!",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
            e.Handled = true;
        }

        public void OnCloseScript(MainWindow mainWindow, CancelEventArgs e)
        {
            if (Document.Status == StatusElement.NotSaved)
            {
                e.Cancel = true;
                MessageBoxResult result = MessageBox.Show(
                    "Save the file before exiting?",
                    "Save",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    if (Document.FilePath == "")
                    {
                        SaveAsProjectScript(mainWindow);
                    }
                    else
                    {
                        string text = mainWindow.FullText.Text;
                        File.WriteAllText(Document.FilePath, text);
                        Document.Save();
                        mainWindow.Title = Document.FileName;
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
