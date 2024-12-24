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

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            CreateNewNote();
        }

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

                // Обработчик одинарного клика
                noteButton.Click += async (s, e) =>
                {
                    // Изменяем цвет на красный
                    noteButton.Background = new SolidColorBrush(Color.Parse("Red"));

                    // Ждем 300 мс, чтобы отличить одинарный клик от двойного
                    await Task.Delay(300);
                    noteButton.Background = new SolidColorBrush(Color.Parse("#D9D9D9"));
                };

                // Обработчик двойного клика
                noteButton.DoubleTapped += async (s, e) =>
                {
                    var newTitle = await ShowRenameDialog(note.Title);
                    if (!string.IsNullOrEmpty(newTitle))
                    {
                        note.Title = newTitle;
                        UpdateNotesList();
                    }
                };

                notesList.Children.Add(noteButton);
            }
            EmptyListText.IsVisible = _notes.Count == 0;

            // Показываем кнопку CreateNote, если нет заметок
            var createNoteButton = this.FindControl<Button>("CreateNote");
            createNoteButton.IsVisible = _notes.Count == 0;
        }

        private void ShowNotePanel(int index)
        {
            if (index >= 0 && index < _notes.Count)
            {
                NoteEditor.Text = _notes[index].Text;
                _currentNoteIndex = index;
                NotePanel.IsVisible = true;

                // Скрываем кнопку CreateNote, если панель заметки видна
                var createNoteButton = this.FindControl<Button>("CreateNote");
                createNoteButton.IsVisible = false;
            }
        }

        private void SaveNotesButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_currentNoteIndex >= 0 && _currentNoteIndex < _notes.Count)
            {
                _notes[_currentNoteIndex].Text = NoteEditor.Text;
                var filePath = Path.Combine(NotesFolder, $"{_notes[_currentNoteIndex].Title}.json");
                var json = JsonSerializer.Serialize(_notes[_currentNoteIndex]);
                File.WriteAllText(filePath, json);
            }
        }

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

        private void CreateNote2_OnClick(object? sender, RoutedEventArgs e)
        {
            CreateNewNote();
        }

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

        private async Task<string> ShowRenameDialog(string currentTitle)
        {
            var dialog = new TextBox
            {
                Width = 200,
                Height = 50,
                Text = currentTitle,
                FontSize = 16,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };

            var window = new Window
            {
                Width = 300,
                Height = 150,
                Content = dialog,
                Title = "Переименовать заметку",
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var result = await window.ShowDialog<string>(this);
            return result ?? currentTitle;
        }
    }

    public class Note
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
