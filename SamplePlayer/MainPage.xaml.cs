using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Playlists;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SamplePlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<Song> SongList = new List<Song>();
        List<string> AlbumList = new List<string>();
        List<string> ArtistList = new List<string>();
        MediaPlaybackList mplSongsList = new MediaPlaybackList();
        //Dictionary<String, List<Song>> Albums = new Dictionary<String, List<Song>>();

        public MainPage()
        {
            this.InitializeComponent();

        }

        public void HideAllListViews()
        {
            lvArtistsList.Visibility = Visibility.Collapsed;
            lvSongsList.Visibility = Visibility.Collapsed;
            lvAlbumsList.Visibility = Visibility.Collapsed;
        }




        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            //https://stackoverflow.com/questions/53934858/c-sharp-uwp-system-unauthorizedaccessexception-access-is-denied

            StorageFolder musicLib = KnownFolders.MusicLibrary;

            var files = await musicLib.GetFilesAsync();
            foreach (var file in files)
            {
                var musicProperties = await file.Properties.GetMusicPropertiesAsync();
                var artist = musicProperties.Artist;
                if (artist == "")
                    artist = "UnKnown";
                var album = musicProperties.Album;
                if (album == "")
                    album = "Unknown";

                SongList.Add((new Song { SongName = file.DisplayName, Artist = artist, Album = album }));
                MediaPlaybackItem Item = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file));
                mplSongsList.Items.Add(Item);
            }

            PopulateAlbumAndArtistLists();

            lvSongsList.ItemsSource = SongList;
            lvAlbumsList.ItemsSource = AlbumList;
            lvArtistsList.ItemsSource = ArtistList;

            meMyPlayer.Source = mplSongsList;
        }

        private void PopulateAlbumAndArtistLists()
        {
            foreach (Song song in SongList)
            {
                if (!AlbumList.Contains(song.Album))
                    AlbumList.Add(song.Album);
            }

            foreach (Song song in SongList)
            {
                if (!ArtistList.Contains(song.Artist))
                    ArtistList.Add(song.Artist);
            }
        }

        private async void PlayList_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();                   // FileOpenPicker helps to select the songs from fileExplorer

            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".mp3");                              //filters only the mentioned file formats
            picker.FileTypeFilter.Add(".wav");

            IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync(); //selected multiple files to add to playlist

            //To get Windows.Media.Playlists
            //https://blogs.windows.com/buildingapps/2015/09/15/dynamically-detecting-features-with-api-contracts-10-by-10/

            List<Song> PlayList = new List<Song>();

            MediaPlaybackList mplPlayList = new MediaPlaybackList();

            foreach (StorageFile file in files)
            {
                MediaPlaybackItem Item = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file));
                mplPlayList.Items.Add(Item);                                        //adding each song/Item to MediaPlayback List(Items)
                                                                                    //mplPlayList.Items returns a list to which we will be adding the MediaPlaybackItem

                var musicProperties = await file.Properties.GetMusicPropertiesAsync();
                var artist = musicProperties.Artist;
                if (artist == "")
                    artist = "UnKnown";
                var album = musicProperties.Album;
                if (album == "")
                    album = "Unknown";
                PlayList.Add((new Song { SongName = file.DisplayName, Artist = artist, Album = album }));

            }

            //mpl.StartingItem = mpl.Items[2];
            meMyPlayer.Source = mplPlayList;                                        // meMyPlayer.Source is a superclass(IMediaPlaybackSource) property 
                                                                                    // which is holding a subclass(MediaPlaybackList Item) object mpl

            lvSongsList.ItemsSource = PlayList;                             // displays the songs on the ListView
            meMyPlayer.MediaPlayer.Play();

        }

        private void Albums_Click(object sender, RoutedEventArgs e)
        {
            HideAllListViews();
            lvAlbumsList.Visibility = Visibility.Visible;
        }

        private void Artists_Click(object sender, RoutedEventArgs e)
        {
            HideAllListViews();
            lvArtistsList.Visibility = Visibility.Visible;
        }

        private void Songs_Click(object sender, RoutedEventArgs e)
        {
            meMyPlayer.Source = mplSongsList;
            lvSongsList.ItemsSource = SongList;

            HideAllListViews();
            lvSongsList.Visibility = Visibility.Visible;
        }

        private void LvAlbumsList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void LvSongsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            MediaPlaybackList mpl2 = (MediaPlaybackList)meMyPlayer.Source; //Assigning superclass object to a subclass refernce mpl2, here we can cast meMyPlayer.souce to a subclass(MediaPlaybackList) since 
                                                                           // we know that the superclass refernce previous held the same subclass object

            if (lvSongsList.SelectedIndex >= 0)
                mpl2.MoveTo((uint)lvSongsList.SelectedIndex);
        }

        private void LvArtistsList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }

}

