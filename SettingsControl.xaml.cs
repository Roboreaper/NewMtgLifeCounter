using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class SettingsControl : UserControl
    {
        private PlayerControl playerControl;
        private PlayerViewModel viewModel;
        private Action onClose;

        public SettingsControl()
        {
            this.InitializeComponent();
        }

        public void Init(PlayerViewModel vm, PlayerControl parent, Action actionClose)
        {
            this.playerControl = parent;
            this.viewModel = vm;
            this.onClose = actionClose;
        }

        public void Update()
        {
            btnRed.IsChecked = false;
            btnGreen.IsChecked = false;
            btnPurple.IsChecked = false;
            BtnBlue.IsChecked = false;
            btnYellow.IsChecked = false;
            btnWhite.IsChecked = false;
            btnPink.IsChecked = false;
            btnCyan.IsChecked = false;

            switch (viewModel.Color)
            {
                case BackGroundColors.Red:
                    btnRed.IsChecked = true;
                    break;
                case BackGroundColors.Blue:
                    BtnBlue.IsChecked = true;
                    break;
                case BackGroundColors.Green:
                    btnGreen.IsChecked = true;
                    break;
                case BackGroundColors.Purple:
                    btnPurple.IsChecked = true;
                    break;
                case BackGroundColors.Yellow:
                    btnYellow.IsChecked = true;
                    break;
                case BackGroundColors.White:
                    btnWhite.IsChecked = true;
                    break;
                case BackGroundColors.Pink:
                    btnPink.IsChecked = true;
                    break;
                case BackGroundColors.Cyan:
                    btnCyan.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            onClose();
        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            playerControl.SetBackGround(BackGroundColors.Red);
            Update();
        }

        private void btnGreen_Click(object sender, RoutedEventArgs e)
        {
            playerControl.SetBackGround(BackGroundColors.Green);
            Update();

        }

        private void btnYellow_Click(object sender, RoutedEventArgs e)
        {
            playerControl.SetBackGround(BackGroundColors.Yellow);
            Update();

        }

        private void BtnBlue_Click(object sender, RoutedEventArgs e)
        {
            playerControl.SetBackGround(BackGroundColors.Blue);
            Update();
        }

        private void btnPurple_Click(object sender, RoutedEventArgs e)
        {
            playerControl.SetBackGround(BackGroundColors.Purple);

            Update();
        }

        private void btnWhite_Click(object sender, RoutedEventArgs e)
        {
            playerControl.SetBackGround(BackGroundColors.White);
            Update();
        }


        private void btnOrange_Checked(object sender, RoutedEventArgs e)
        {
            playerControl.SetBackGround(BackGroundColors.Pink);
            Update();

        }

        private void btnCyan_Checked(object sender, RoutedEventArgs e)
        {
            playerControl.SetBackGround(BackGroundColors.Cyan);
            Update();
        }
        private void btnSet20_Click(object sender, RoutedEventArgs e)
        {
            viewModel.LifeTotal = 20;
        }

        private void btnSet40_Click(object sender, RoutedEventArgs e)
        {
            viewModel.LifeTotal = 40;
        }

        private void btnRotate_Click(object sender, RoutedEventArgs e)
        {
            playerControl.Flip();
        }

    }
}
