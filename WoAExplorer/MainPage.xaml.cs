using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WoAExplorer
{

    public sealed partial class MainPage : Page
    {

        public StorageFolder currentFolder;
        public ObservableCollection<IStorageItem> currentItems = new ObservableCollection<IStorageItem>();
        public ObservableCollection<string> currentItemsStrings = new ObservableCollection<string>();
        public ObservableCollection<StorageFolder> currentFolders = new ObservableCollection<StorageFolder>();
        public ObservableCollection<StorageFile> currentFiles = new ObservableCollection<StorageFile>();

        public ObservableCollection<StorageFolder> availableDrives = new ObservableCollection<StorageFolder>();

        public static MainPage thisPage = null;

        public MainPage()
        {
            InitializeComponent();
            thisPage = this;
            DataContext = this;
            updateDrivesList();
            try
            {
                loadFolderContent(availableDrives[0]);
            } catch
            {
                try
                {
                    loadFolderContent("C:\\");
                } catch
                {
                    Debug.WriteLine("Can't load any folder.");
                }
            }
            
        }

        public async void loadFolderContent(StorageFolder sf)
        {
            currentFolder = sf;
            currentFolders.Clear();
            currentFiles.Clear();
            foreach(StorageFolder s in await sf.GetFoldersAsync())
            {
                currentFolders.Add(s);
            }
            foreach(StorageFile f in await sf.GetFilesAsync())
            {
                currentFiles.Add(f);
            }
            updatePage();
        }

        public static async void loadFolderContentFromOutside(string path)
        {
            thisPage.loadFolderContent(await StorageFolder.GetFolderFromPathAsync(path));
        }

        public async void loadFolderContent(string path)
        {
            loadFolderContent(await StorageFolder.GetFolderFromPathAsync(path));
        }
        
        public async void updateDrivesList()
        {
            availableDrives.Clear();
            char driveToScan = 'A';
            for(; driveToScan <= 'Z'; driveToScan++)
            {
                try
                {
                    if (await StorageFolder.GetFolderFromPathAsync(driveToScan + ":\\") != null)
                    {
                        availableDrives.Add(await StorageFolder.GetFolderFromPathAsync(driveToScan + ":\\"));
                        Debug.WriteLine(driveToScan);
                    }
                }catch(Exception e)
                {
                    Debug.WriteLine(driveToScan + " is not an available drive");
                }
            }
        }

        public void updatePage()
        {
            currentItemsStrings.Clear();
            currentItems.Clear();
            currentFolderPathTextBlock.Text = currentFolder.Path;
            currentFolderPathTextBlock.Text = currentFolder.Path.Replace("\\", " > ");
            foreach (StorageFolder s in currentFolders)
            {
                currentItemsStrings.Add(s.DisplayName);
                currentItems.Add(s);
            }
            foreach (StorageFile f in currentFiles)
            {
                currentItemsStrings.Add(f.Name);
                currentItems.Add(f);
            }
        }

        private async void FolderUpButton_Click(object sender, RoutedEventArgs e)
        {
            if(await currentFolder.GetParentAsync() == null)
            {
                return;
            }
            loadFolderContent(await currentFolder.GetParentAsync());
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width >= 800)
            {
                driveDropdownButton.Visibility = Visibility.Collapsed;
                shortcutsScrollViewer.Visibility = Visibility.Visible;
                contentPane.Margin = new Thickness(300, 60, 0, 0);
            }
            else if (e.NewSize.Width < 800)
            {
                driveDropdownButton.Visibility = Visibility.Visible;
                shortcutsScrollViewer.Visibility = Visibility.Collapsed;
                contentPane.Margin = new Thickness(0, 60, 0, 0);
            }
        }
    }
}
