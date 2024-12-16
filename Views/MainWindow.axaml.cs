using System.Collections.ObjectModel;
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

            // P.S тут баг при повторном заходе не прячется кнопка
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
                noteButton.Click += (s, e) => ShowNotePanel(_notes.IndexOf(note));
                notesList.Children.Add(noteButton);
            }
            EmptyListText.IsVisible = _notes.Count == 0;
        }

        private void ShowNotePanel(int index)
        {
            if (index >= 0 && index < _notes.Count)
            {
                NoteEditor.Text = _notes[index].Text;
                _currentNoteIndex = index;
                NotePanel.IsVisible = true;
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
                _notes.RemoveAt(_currentNoteIndex);
                NoteEditor.Text = string.Empty;
                NotePanel.IsVisible = false;
                UpdateNotesList();
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
    }

    public class Note
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
