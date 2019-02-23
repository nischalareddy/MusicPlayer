﻿using System;
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
        private List<Songs> MusicList = new List<Songs>();
        MediaPlaybackList mplSongsList = new MediaPlaybackList();



        public MainPage()
        {
            this.InitializeComponent();

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
                //    await GetMusicFilesAsync();
                //    base.OnNavigatedTo(e);
                //    //first call GetMusicFilesAsync in Mainpage
                //    //then call OnNavigateTo in the base class (Page)
                //}

                //public async Task GetMusicFilesAsync()
                //{

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
                MusicList.Add((new Songs { FileName = file.DisplayName, Artist = artist, Album = album }));

                lvPlayList.ItemsSource = MusicList;



                MediaPlaybackItem Item = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file));
                mplSongsList.Items.Add(Item);
                meMyPlayer.Source = mplSongsList;
            }
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();                   // FileOpenPicker helps to select the songs from fileExplorer

            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".mp3");                              //filters only the mentioned file formats
            picker.FileTypeFilter.Add(".wav");

            IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync(); //selected multiple files to add to playlist

            //To get Windows.Media.Playlists
            //https://blogs.windows.com/buildingapps/2015/09/15/dynamically-detecting-features-with-api-contracts-10-by-10/

        List<Songs> PlayList = new List<Songs>();

        MediaPlaybackList mplPlayList = new MediaPlaybackList();

            foreach (StorageFile file in files)
            {
                MediaPlaybackItem Item = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file));
                mplPlayList.Items.Add(Item);                                        //adding each song/Item to MediaPlayback List(Items)
                                                                            //mpl.Items returns a list to which we are adding the MediaPlaybackItem

                var musicProperties = await file.Properties.GetMusicPropertiesAsync();
                var artist = musicProperties.Artist;
                if (artist == "")
                    artist = "UnKnown";
                var album = musicProperties.Album;
                if (album == "")
                    album = "Unknown";
                PlayList.Add((new Songs { FileName = file.DisplayName, Artist = artist, Album = album }));

            }

            //mpl.StartingItem = mpl.Items[2];
            meMyPlayer.Source = mplPlayList;                                        // meMyPlayer.Source is a superclass(IMediaPlaybackSource) property 
                                                                            // which is holding a subclass(MediaPlaybackList Item) object mpl

            lvPlayList.ItemsSource = PlayList;                             // displays the songs on the ListView
            meMyPlayer.MediaPlayer.Play();

        }

        private void LvPlayList_ItemClick(object sender, ItemClickEventArgs e)
        {
            MediaPlaybackList mpl2 = (MediaPlaybackList)meMyPlayer.Source; //Assigning superclass object to a subclass refernce mpl2, here we can cast meMyPlayer.souce to a subclass(MediaPlaybackList) since 
                                                                           // we know that the superclass refernce previous held the same subclass object

            if (lvPlayList.SelectedIndex >= 0)
                mpl2.MoveTo((uint)lvPlayList.SelectedIndex);
        }


        private void Albums_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Artists_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Songs_Click(object sender, RoutedEventArgs e)
        {
            meMyPlayer.Source = mplSongsList;

            lvPlayList.ItemsSource = MusicList;
        }
    }

}

