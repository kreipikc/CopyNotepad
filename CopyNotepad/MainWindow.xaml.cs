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
    public partial class MainWindow : Window
    {
        ProjectManager projectManager;

        public MainWindow()
        {
            InitializeComponent();
            projectManager = new ProjectManager();
        }

        private void SaveProjectClick(object sender, RoutedEventArgs e)
        {
            projectManager.SaveProjectScript(this);
        }

        private void SaveAsProjectClick(object sender, RoutedEventArgs e)
        {
            projectManager.SaveAsProjectScript(this);
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            projectManager.OpenFileScript(this);
        }

        private void ZoomInClick(object sender, RoutedEventArgs e)
        {
            projectManager.ZoomIn(this);
        }

        private void ZoomOutClick(object sender, RoutedEventArgs e)
        {
            projectManager.ZoomOut(this);
        }

        private void DefaultZoomClick(object sender, RoutedEventArgs e)
        {
            projectManager.DefaultZoom(this);
        }

        private void CheckUpdateChanged(object sender, TextChangedEventArgs e)
        {
            projectManager.ChangeTitle(this);
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
                        projectManager.SaveAsProjectScript(this);
                        e.Handled = true;
                    }
                }
                // При CTRL+S - Save
                else if (e.Key == Key.S)
                {
                    projectManager.SaveProjectScript(this);
                    e.Handled = true;
                }
                // При CTRL+O - Open...
                else if (e.Key == Key.O)
                {
                    projectManager.OpenFileScript(this);
                    e.Handled = true;
                }
                // При CTRL+Q - Exit
                else if (e.Key == Key.Q)
                {
                    Close();
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
                    projectManager.ZoomIn(this);
                }
                // CTRL + ScrollDown
                else
                {
                    projectManager.ZoomOut(this);
                }
                e.Handled = true;
            }
        }

        // При перетаскивании файла на блокнот (держать)
        private void FullText_DragOver(object sender, DragEventArgs e)
        {
            projectManager.DragOver(e);
        }

        // При перетаскивании файла на блокнот (отпустить)
        private void FullText_Drop(object sender, DragEventArgs e)
        {
            projectManager.Drop(this, e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            projectManager.OnCloseScript(this, e);
        }
    }
}
