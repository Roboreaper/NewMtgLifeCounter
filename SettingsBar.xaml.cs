﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MtgLifeCounter
{
    public sealed partial class SettingsBar : UserControl
    {
        private GameManager Manager;
        private Page parentPage;

        public delegate void RoutedEvent(object sender, RoutedEventArgs e);
        public event RoutedEvent ButtonClicked;

        public SettingsBar()
        {
            this.InitializeComponent();
        }

        public void Init(GameManager manager, Page parent)
        {
            this.Manager = manager;
            this.parentPage = parent;

            if (parentPage.GetType() == typeof(MainPage))
            {
                btn4Player.Visibility = Visibility.Collapsed;
                col4P.Width = new GridLength(1.0);

            }

            if (parentPage.GetType() == typeof(ThreePlayerPage))
            {
                btn3Player.Visibility = Visibility.Collapsed;
                col3P.Width = new GridLength(1.0);
            }

            if (Windows.UI.ViewManagement.UIViewSettings.GetForCurrentView().UserInteractionMode == Windows.UI.ViewManagement.UserInteractionMode.Mouse)
            {
                btnQuit.Visibility = Visibility.Collapsed;
                //col3P.Width = new GridLength(1.0);
                //colSep6.Width = new GridLength(1.0);
            }
        }

        private async void btn3Player_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                       () => parentPage.Frame.Navigate(typeof(ThreePlayerPage)));
        }

        private void btnBrawl_Click(object sender, RoutedEventArgs e)
        {
            Manager.ResetPlayers(Gametypes.Brawl);
            ButtonClicked?.Invoke(this, e);
        }

        private void btnCommander_Click(object sender, RoutedEventArgs e)
        {
            Manager.ResetPlayers(Gametypes.Commander );
            ButtonClicked?.Invoke(this, e);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Manager.ResetPlayers(Gametypes.Current);
            ButtonClicked?.Invoke(this, e);
        }

        private void btnMultiplayer_Click(object sender, RoutedEventArgs e)
        {
            Manager.ResetPlayers(Gametypes.MultiPlayer);
            ButtonClicked?.Invoke(this, e);
        }

        private async void btn4Player_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                      () => parentPage.Frame.Navigate(typeof(MainPage)));
        }

        private async void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog notify = new ContentDialog() { Title = "ShutDown Request", Content = "Are you sure?", PrimaryButtonText = "No", SecondaryButtonText = "Yes" };

            ContentDialogResult res = await notify.ShowAsync();
            if( res ==  ContentDialogResult.Secondary)
                Application.Current.Exit();
        }
    }
}
