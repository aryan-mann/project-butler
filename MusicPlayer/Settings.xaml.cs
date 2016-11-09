using System;
using System.Collections.Generic;
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
using MusicPlayer;

namespace MusicBrowser {

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window {

        List<string> LibraryDirectories = new List<string>();
        public static string IndexedSongsPath {
            get {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"IndexedSongs.json");
            }
        }

        public Settings() {
            InitializeComponent();

            Loaded += Settings_Loaded;
            LibraryFolderList.Drop += LibraryFolderList_Drop;
            RefreshLibrary.Click += RefreshLibrary_Click;

            Closed += Settings_Closed;
        }

        private void RefreshLibrary_Click(object sender, RoutedEventArgs e) {
            RefreshLibrary.IsEnabled = false;

            Task.Factory.StartNew(() => {
                foreach(string dir in LibraryDirectories) {
                    string[] files = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly);
                    foreach(string file in files) {
                        if(Song.ValidMusicExtensions.Contains(Path.GetExtension(file))) {
                            Song.SongList.Add(Song.GetSong(file));
                        }
                    }
                }

                Dispatcher.Invoke(() => {
                    RefreshLibrary.IsEnabled = true;
                });
            });
        }

        private void LibraryFolderList_Drop(object sender, DragEventArgs e) {
            string[] folders = (string[]) e.Data.GetData(DataFormats.FileDrop);
            if(folders == null) { return; }

            foreach(string s in folders) {
                if(Directory.Exists(s) && !LibraryDirectories.Contains(s)) {
                    LibraryDirectories.Add(s);
                }
            }

            FolderCount.Content = LibraryDirectories.Count;
            ReloadLibraryList();
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e) {
            
        }

        private void ReloadLibraryList() {
            if(LibraryDirectories == null) { LibraryDirectories = new List<string>(); }
            LibraryFolderList.ItemsSource = null;
            LibraryFolderList.ItemsSource = LibraryDirectories;
        }

        private void Settings_Closed(object sender, EventArgs e) {
            
        }

    }
}
