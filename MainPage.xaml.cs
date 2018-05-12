using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MtgLifeCounter
{
    public sealed partial class MainPage : Page
    {
        GameManager _manager = new GameManager();

        public MainPage()
        {
            this.InitializeComponent();         

            Player1.Init(_manager, _manager.Player1);
            Player2.Init(_manager, _manager.Player2);
            Player3.Init(_manager, _manager.Player3);
            Player4.Init(_manager, _manager.Player4);

            Player1.Flip();
            Player2.Flip();

            Player1.SetBackGround(BackGroundColors.Yellow);
            Player2.SetBackGround(BackGroundColors.Green);
            Player3.SetBackGround(BackGroundColors.Purple);
            Player4.SetBackGround(BackGroundColors.Blue);

            SettingsBar1.Init(_manager, this);
            SettingsBar2.Init(_manager, this);


            SettingsBar2.Visibility  = SettingsBar1.Visibility = Visibility.Collapsed;

        }          
        

        //private void cmdOpen_Click(object sender, RoutedEventArgs e)
        //{
        //    CmdBar.IsOpen = true;
        //}


        private void BtnResetMP_Click(object sender, RoutedEventArgs e)
        {
            Player1.Reset( Gametypes.Current);
            Player2.Reset(Gametypes.Current);
            Player3.Reset(Gametypes.Current);
            Player4.Reset(Gametypes.Current);
        }

        private void BtnResetCommander_Click(object sender, RoutedEventArgs e)
        {
            Player1.Reset(Gametypes.Commander);
            Player2.Reset(Gametypes.Commander);
            Player3.Reset(Gametypes.Commander);
            Player4.Reset(Gametypes.Commander);
        }

        private async void BtnReset3P_Click(object sender, RoutedEventArgs e)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                          () => Frame.Navigate(typeof(ThreePlayerPage)));

        }

        private void BtnReset2P_Click(object sender, RoutedEventArgs e)
        {

            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            //             () => Frame.Navigate(typeof(TwoPlayerPage)));
        }

        private DisplayRequest KeepScreenOnRequest;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (KeepScreenOnRequest == null)
                KeepScreenOnRequest = new DisplayRequest();

            KeepScreenOnRequest.RequestActive();

            base.OnNavigatedTo(e);

        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            KeepScreenOnRequest.RequestRelease();
            base.OnNavigatingFrom(e);

        }

        private void BarButtonHideTitle_Click(object sender, RoutedEventArgs e)
        {
            ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;


            if (coreTitleBar.ExtendViewIntoTitleBar)
            {
                formattableTitleBar.ButtonBackgroundColor = Colors.White;
                coreTitleBar.ExtendViewIntoTitleBar = false;
            }
            else
            {
                formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
                coreTitleBar.ExtendViewIntoTitleBar = true;
            }           
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsBar2.Visibility = SettingsBar1.Visibility = SettingsBar1.Visibility  == Visibility.Collapsed ?  Visibility.Visible : Visibility.Collapsed;

        }
    }
}
