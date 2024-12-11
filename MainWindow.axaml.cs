using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.IO;
using System.Text.Json;
using Avalonia;
namespace NoteApp.Views
{
    public partial class MainWindow : Window
    {
        private List<Note> _notes = new List<Note>(); // Список заметок
        private int _currentNoteIndex = -1;
        private const string NotesFolder = "Notes"; // Папка для сохранения заметок

        public MainWindow()
        {
            InitializeComponent();

            // Создаем папку для заметок, если её нет
            if (!Directory.Exists(NotesFolder))
            {
                Directory.CreateDirectory(NotesFolder);
            }

            // Загружаем заметки из файлов
            LoadNotes();

            // Если есть заметки, показываем первую заметку
            if (_notes.Count > 0)
            {
                _currentNoteIndex = 0;
                ShowNotePanel(_currentNoteIndex);
            }
            else
            {
                // Если заметок нет, показываем текст "Ваш список пуст..."
                EmptyListText.IsVisible = true;
                NotePanel.IsVisible = false;
            }
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            CreateNewNote();
        }

        private void CreateNote2_OnClick(object? sender, RoutedEventArgs e)
        {
            CreateNewNote();
        }

        private void CreateNewNote()
        {
            // Создаем новую заметку
            var newNote = new Note
            {
                Text = "",
                Title = $"Заметка {_notes.Count + 1}"
            };

            // Добавляем заметку в список
            _notes.Add(newNote);

            // Создаем кнопку для новой заметки
            var newNoteButton = new Button
            {
                Width = 130,
                Height = 40,
                Content = newNote.Title,
                Background = new SolidColorBrush(Color.Parse("#D9D9D9")),
                Foreground = new SolidColorBrush(Color.Parse("#53422E")),
                Margin = new Thickness(10)
            };
            newNoteButton.Click += (s, e) => ShowNotePanel(_notes.Count - 1);

            // Добавляем кнопку в NotesPanel
            NotesPanel.Children.Add(newNoteButton);

            // Скрываем текст "Ваш список пуст..."
            EmptyListText.IsVisible = false;

            // Показываем панель редактирования для новой заметки
            _currentNoteIndex = _notes.Count - 1;
            ShowNotePanel(_currentNoteIndex);
        }

        private void ShowNotePanel(int index)
        {
            if (index >= 0 && index < _notes.Count)
            {
                // Показываем TextBox для редактирования заметки
                NoteEditor.Text = _notes[index].Text;
                NotePanel.IsVisible = true;
                EmptyListText.IsVisible = false;
            }
            else
            {
                // Если индекс некорректен, скрываем редактор
                NotePanel.IsVisible = false;
                EmptyListText.IsVisible = true;
            }
        }

        private void DeleteNote_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_notes.Count > 0)
            {
                // Удаляем последнюю заметку
                NotesPanel.Children.RemoveAt(NotesPanel.Children.Count - 1);
                var noteToDelete = _notes[_notes.Count - 1];
                _notes.RemoveAt(_notes.Count - 1);

                // Удаляем файл заметки
                var filePath = Path.Combine(NotesFolder, $"{noteToDelete.Title}.json");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                if (_notes.Count == 0)
                {
                    // Если заметок больше нет, показываем текст "Ваш список пуст..."
                    EmptyListText.IsVisible = true;
                    NotePanel.IsVisible = false;
                }
                else
                {
                    // Показываем предыдущую заметку
                    _currentNoteIndex = _notes.Count - 1;
                    ShowNotePanel(_currentNoteIndex);
                }
            }
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            // Сохраняем текст заметок перед закрытием
            SaveNotes();
            base.OnClosing(e);
        }

        private void SaveNotes()
        {
            foreach (var note in _notes)
            {
                // Сохраняем каждую заметку в отдельный файл JSON
                var filePath = Path.Combine(NotesFolder, $"{note.Title}.json");
                var json = JsonSerializer.Serialize(note);
                File.WriteAllText(filePath, json);
            }
        }

        private void LoadNotes()
        {
            // Загружаем заметки из файлов JSON
            foreach (var filePath in Directory.GetFiles(NotesFolder, "*.json"))
            {
                var json = File.ReadAllText(filePath);
                var note = JsonSerializer.Deserialize<Note>(json);
                if (note != null)
                {
                    _notes.Add(note);

                    
                    var noteButton = new Button
                    {
                        Width = 130,
                        Height = 40,
                        Content = note.Title,
                        Background = new SolidColorBrush(Color.Parse("#D9D9D9")),
                        Foreground = new SolidColorBrush(Color.Parse("#53422E")),
                        Margin = new Thickness(10)
                    };
                    noteButton.Click += (s, e) => ShowNotePanel(_notes.IndexOf(note));
                    NotesPanel.Children.Add(noteButton);
                }
            }
        }
    }

    // Класс для хранения данных заметки
    public class Note
    {
        public string Title { get; set; } // Заголовок заметки
        public string Text { get; set; } // Текст заметки
    }
}