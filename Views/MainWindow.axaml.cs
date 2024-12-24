using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.IO;
using System.Text.Json;
using Avalonia;
using System.Threading.Tasks;

namespace NoteApp.Views
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Note> _notes = new ObservableCollection<Note>();
        private const string NotesFolder = "Notes";
        private int _currentNoteIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists(NotesFolder))
            {
                Directory.CreateDirectory(NotesFolder);
            }
            LoadNotes();
            UpdateNotesList();
        }

        // Создание новой заметки
        private void CreateNote_OnClick(object? sender, RoutedEventArgs e)
        {
            CreateNewNote();
        }

        // Создание новой заметки
        private void CreateNewNote()
        {
            var newNote = new Note { Title = $"Заметка {_notes.Count + 1}", Text = $"Это заметка {_notes.Count + 1}" };
            _notes.Add(newNote);
            UpdateNotesList();
            ShowNotePanel(_notes.Count - 1);

            // Скрываем кнопку CreateNote
            var createNoteButton = this.FindControl<Button>("CreateNote");
            createNoteButton.IsVisible = false;
        }

        // Обновление списка заметок
        private void UpdateNotesList()
        {
            var notesList = this.FindControl<StackPanel>("NotesPanel");
            notesList.Children.Clear();
            foreach (var note in _notes)
            {
                var noteButton = new Button
                {
                    Width = 130,
                    Height = 40,
                    Content = note.Title,
                    Background = new SolidColorBrush(Color.Parse("#D9D9D9")),
                    Foreground = new SolidColorBrush(Color.Parse("#53422E")),
                    Margin = new Thickness(10)
                };

                // Обработчик двойного клика
                noteButton.DoubleTapped += (s, e) =>
                {
                    // Переключение цвета
                    if (noteButton.Background.Equals(new SolidColorBrush(Color.Parse("#FF0000"))))
                    {
                        noteButton.Background = new SolidColorBrush(Color.Parse("#D9D9D9")); // Меняем на стандартный
                    }
                    else
                    {
                        noteButton.Background = new SolidColorBrush(Color.Parse("#FF0000")); // Меняем на красный
                    }

                    // Переход к заметке
                    ShowNotePanel(_notes.IndexOf(note));
                };

                notesList.Children.Add(noteButton);
            }
            EmptyListText.IsVisible = _notes.Count == 0;

            // Показываем кнопку "Создать" если нет заметок
            var createNoteButton = this.FindControl<Button>("CreateNote");
            createNoteButton.IsVisible = _notes.Count == 0;
        }

        // Отображение панели для редактирования заметки
        private void ShowNotePanel(int index)
        {
            if (index >= 0 && index < _notes.Count)
            {
                NoteEditor.Text = _notes[index].Text;
                _currentNoteIndex = index;
                NotePanel.IsVisible = true;

                // Обновляем название заметки на первую строку текста
                _notes[_currentNoteIndex].Title = GetFirstLine(NoteEditor.Text);

                // Скрываем кнопку "Создать", когда панель заметки отображается
                var createNoteButton = this.FindControl<Button>("CreateNote");
                createNoteButton.IsVisible = false;
            }
        }

        // Получаем первую строку текста
        private string GetFirstLine(string text)
        {
            var lines = text.Split('\n');
            return lines.Length > 0 ? lines[0] : "";
        }

        // Сохранение заметки
        private void SaveNotesButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_currentNoteIndex >= 0 && _currentNoteIndex < _notes.Count)
            {
                _notes[_currentNoteIndex].Text = NoteEditor.Text;
                _notes[_currentNoteIndex].Title = GetFirstLine(NoteEditor.Text); // Обновляем название при сохранении

                var filePath = Path.Combine(NotesFolder, $"{_notes[_currentNoteIndex].Title}.json");
                var json = JsonSerializer.Serialize(_notes[_currentNoteIndex]);
                File.WriteAllText(filePath, json);
            }
        }

        // Удаление заметки
        private void DeleteNote_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_currentNoteIndex >= 0 && _currentNoteIndex < _notes.Count)
            {
                var noteToDelete = _notes[_currentNoteIndex];

                _notes.RemoveAt(_currentNoteIndex);
                
                var filePath = Path.Combine(NotesFolder, $"{noteToDelete.Title}.json");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                
                NoteEditor.Text = string.Empty;
                NotePanel.IsVisible = false;
                
                UpdateNotesList();

                // Показываем кнопку CreateNote, если нет заметок
                var createNoteButton = this.FindControl<Button>("CreateNote");
                createNoteButton.IsVisible = _notes.Count == 0;
            }
        }

        // Слушатель для кнопки "Создать"
        private void CreateNote2_OnClick(object? sender, RoutedEventArgs e)
        {
            CreateNewNote();
        }

        // Загрузка заметок из файлов
        private void LoadNotes()
        {
            if (Directory.Exists(NotesFolder))
            {
                var files = Directory.GetFiles(NotesFolder, "*.json");
                foreach (var file in files)
                {
                    var json = File.ReadAllText(file);
                    var note = JsonSerializer.Deserialize<Note>(json);
                    if (note != null)
                    {
                        _notes.Add(note);
                    }
                }
            }
        }
    }

    public class Note
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
