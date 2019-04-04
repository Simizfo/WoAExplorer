using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace WoAExplorer.CustomControls
{
    public sealed partial class StorageItemControl : UserControl
    {
        public StorageItemControl()
        {
            InitializeComponent();
        }

        public IStorageItem Item
        {
            get { return (IStorageItem)GetValue(ItemProperty); }
            set {
                SetValue(ItemProperty, value);
            }
        }

        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register("Item", typeof(IStorageItem), typeof(StorageItemControl), new PropertyMetadata(default(int), new PropertyChangedCallback(async (s, e) =>
            {
                if (e.OldValue is IStorageItem oldValue) { }
                if (e.NewValue is IStorageItem newValue)
                {
                    ((StorageItemControl)(s)).ItemNameLabel.Text = newValue.Name;
                    ((StorageItemControl)(s)).ItemFlyoutName.Text = newValue.Name;
                    if (newValue.IsOfType(StorageItemTypes.File))
                    {
                        StorageItemThumbnail icon = await GetFileIcon((StorageFile)newValue);
                        BitmapImage bmp = new BitmapImage();
                        bmp.SetSource(icon);
                        ((StorageItemControl)(s)).ItemIcon.Source = bmp;
                        ((StorageItemControl)(s)).ItemFlyoutIcon.Source = bmp;
                        BasicProperties bp = await ((StorageFile)newValue).GetBasicPropertiesAsync();
                        ((StorageItemControl)(s)).ItemFlyoutType.Text = ((StorageFile)newValue).DisplayType;
                        ((StorageItemControl)(s)).ItemFlyoutSize.Text = bp.Size.ToString() + " bytes";
                    }
                    else if (newValue.IsOfType(StorageItemTypes.Folder))
                    {
                        StorageItemThumbnail icon = await GetFolderIcon((StorageFolder)newValue);
                        BitmapImage bmp = new BitmapImage();
                        bmp.SetSource(icon);
                        ((StorageItemControl)(s)).ItemIcon.Source = bmp;
                        ((StorageItemControl)(s)).ItemFlyoutIcon.Source = bmp;
                    }
                }
            })));

        private void MainGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (((IStorageItem)GetValue(ItemProperty)).IsOfType(StorageItemTypes.Folder)){
                MainPage.loadFolderContentFromOutside(((IStorageItem)GetValue(ItemProperty)).Path);
            }
        }

        public async static Task<StorageItemThumbnail> GetFileIcon(StorageFile file, uint size = 32)
        {
            StorageItemThumbnail iconTmb;
            var imgExt = new[] { "bmp", "gif", "jpeg", "jpg", "png" }.FirstOrDefault(ext => file.Path.ToLower().EndsWith(ext));
            if (imgExt != null)
            {
                var dummy = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("dummy." + imgExt, CreationCollisionOption.ReplaceExisting); //may overwrite existing
                iconTmb = await dummy.GetThumbnailAsync(ThumbnailMode.SingleItem, size);
            }
            else
            {
                iconTmb = await file.GetThumbnailAsync(ThumbnailMode.SingleItem, size);
            }
            return iconTmb;
        }

        public async static Task<StorageItemThumbnail> GetFolderIcon(StorageFolder folder, uint size = 32)
        {
            StorageItemThumbnail iconTmb;
            iconTmb = await folder.GetThumbnailAsync(ThumbnailMode.SingleItem, size);
            return iconTmb;
        }
    }
}
