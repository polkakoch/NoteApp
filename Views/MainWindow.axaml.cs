using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace NoteApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnButtonClick(object? sender, RoutedEventArgs e)
        {
            // Логика при нажатии кнопки
            Console.WriteLine("Кнопка нажата!");
        }
    }
}