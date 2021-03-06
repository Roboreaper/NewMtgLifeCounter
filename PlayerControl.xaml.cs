﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MtgLifeCounter
{
    public sealed partial class PlayerControl : UserControl
    {
        private PlayerViewModel viewModel;
        private IGameManager _manager;

        private Dictionary<PlayerID, int> CommanderButtonMapping = new Dictionary<PlayerID, int>();

        private int ControlRotation { get; set; } = 0;		

		public PlayerControl()
        {
            this.InitializeComponent();
        }

        public void Init(IGameManager manager, PlayerViewModel model)
        {
			this._manager = manager ?? throw new ArgumentNullException("manager is null");
            this._manager.PlayerColorChanged += _manager_PlayerColorChanged;

            this.viewModel = model ?? throw new ArgumentNullException("Player model is null");
            this.viewModel.PropertyChanged += ViewModel_PropertyChanged;

            LifeControl.Init(viewModel,true);
            UpdateCustomType();

            cmdLife1.Init(new PlayerViewModel() { LifeTotal = viewModel.CmdEnemy1 },false);
            cmdLife2.Init(new PlayerViewModel() { LifeTotal = viewModel.CmdEnemy2 }, false);
            cmdLife3.Init(new PlayerViewModel() { LifeTotal = viewModel.CmdEnemy3 },false);


            ToprtAngle.Angle = 0;
            //rtPanelOptions.Angle = 0;

			borderLife1.Visibility = cmdLife1.Visibility = Visibility.Collapsed;
			borderLife2.Visibility = cmdLife2.Visibility = Visibility.Collapsed;
			borderLife3.Visibility = cmdLife3.Visibility = Visibility.Collapsed;

            SettingsControl.Init(this.viewModel, this, OnCloseSettings);

            var cmd = 1;
            foreach (var id in _manager.ActivePlayers())
            {
                if (id == viewModel.ID)
                    continue;
                if (id == PlayerID.Unknown)
                    continue;

                CommanderButtonMapping[id] = cmd;

                switch (cmd)
                {
                    case 1:
                        cmdLife1.Visibility = Visibility.Visible;
                        break;

                    case 2:
                        cmdLife2.Visibility = Visibility.Visible;
                        break;

                    case 3:
                        cmdLife3.Visibility = Visibility.Visible;
                        break;

                    default:
                        break;
                }
                cmd++;
            }            

            cmdLife1.LifeChanged += CmdLife_LifeChanged;
            cmdLife2.LifeChanged += CmdLife_LifeChanged;
            cmdLife3.LifeChanged += CmdLife_LifeChanged;

            this.LifeControl.LifeChangeHistory += LifeControl_LifeChanged;

            this.DataContext = viewModel;
        }

        CustomCounterType _lastCounterType = CustomCounterType.Experience;
        int _LifeHistory = 0;
		System.Threading.CancellationTokenSource _cancelToken = null;
        private async void LifeControl_LifeChanged(object sender, LifeChangedEventArgs e)
        {

            if (!Dispatcher.HasThreadAccess)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { LifeControl_LifeChanged(sender, e); });
            }
            else
            {

				if (viewModel.CounterType != CustomCounterType.LifeHistory)
                {
					_cancelToken?.Cancel();
					btnCounterType.Visibility = Visibility.Visible;
					btnCounterType.Opacity = 1.0;
					// imgCountertype.Source = new BitmapImage();
					_lastCounterType = viewModel.CounterType;
                    viewModel.CounterType = CustomCounterType.LifeHistory;
                }

                if( e.Lifechanged == int.MinValue)
                {
					_cancelToken?.Cancel();

					btnCounterType.Visibility = Visibility.Visible;
					btnCounterType.Opacity = 1.0;

					viewModel.CounterType = _lastCounterType;
					_cancelToken = new System.Threading.CancellationTokenSource();

					FadeOut(btnCounterType, _cancelToken.Token);
					//btnCounterType.Visibility = Visibility.Collapsed;

					//imgCountertype.Source = new BitmapImage(new Uri("ms-appx:///" + CounterTypeHelper.CounterTypeImage(viewModel.CounterType)));
				}

				_LifeHistory = e.Lifechanged;

                UpdateCustomType();
            }
           
        }


		private void FadeOut(Button btn, System.Threading.CancellationToken token)
		{
			Task.Run(() =>
			{
				for (double i = 0.9; i > 0; i = i - 0.1)
				{
					if (token.IsCancellationRequested)
						return;

					if (!Dispatcher.HasThreadAccess)
					{
						Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
						{
							btn.Opacity = i;
						});
					}
					else
					{
						btn.Opacity = i;
					}

					Task.Delay(100).Wait();
				}

				Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
				{
					btn.Visibility = Visibility.Collapsed;
				});
			},token);
		}

		private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlayerViewModel.GameType))
                this.Reset(viewModel.GameType);
			if (e.PropertyName == nameof(PlayerViewModel.Color))
				ChangePlayerColor(this.viewModel.Color);
        }

		private void ChangePlayerColor(BackGroundColors color, string key = "PlayerColorBrush")
		{
			object objectStyle = null;
			SolidColorBrush pc = null;
			if (this.Resources.TryGetValue(key, out objectStyle))
			{
				pc = objectStyle as SolidColorBrush;
			}

			if (pc == null)
				return;

			switch (color)
			{
				case BackGroundColors.Red:
					pc.Color = Colors.Red;

					//BackGroundGradientStart.Color = Colors.Red;
					//BackGroundGradientEnd.Color = Colors.Maroon;
					break;
				case BackGroundColors.Blue:
					pc.Color = Colors.Blue;

					//BackGroundGradientStart.Color = Colors.Blue;
					//BackGroundGradientEnd.Color = Colors.DarkBlue;
					break;
				case BackGroundColors.Green:
					pc.Color = Colors.ForestGreen;

					//BackGroundGradientStart.Color = Colors.ForestGreen;
					//BackGroundGradientEnd.Color = Colors.DarkGreen;
					break;
				case BackGroundColors.Purple:
					pc.Color = Colors.MediumPurple;

					//BackGroundGradientStart.Color = Colors.MediumPurple;
					//BackGroundGradientEnd.Color = Colors.Purple;
					break;
				case BackGroundColors.Yellow:
					pc.Color = Colors.Goldenrod;

					//BackGroundGradientStart.Color = Colors.Goldenrod;
					//BackGroundGradientEnd.Color = Colors.DarkGoldenrod;
					break;
				case BackGroundColors.White:
					pc.Color = Colors.White;
					break;
				case BackGroundColors.Pink:
					pc.Color = Colors.Pink;
					break;
				case BackGroundColors.Cyan:
					pc.Color = Colors.Cyan;
					break;
				default:
					break;
			}
			if (key == "PlayerColorBrush")
			{
				ChangePlayerColor(color, "Player1ColorBrush");
				ChangePlayerColor(color, "Player2ColorBrush");
				ChangePlayerColor(color, "Player3ColorBrush");
			}
		}

		private async void CmdLife_LifeChanged(object sender, LifeChangedEventArgs e)
        {
            if(!Dispatcher.HasThreadAccess)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { CmdLife_LifeChanged(sender, e); });
            }
            else
            {               
                await LifeControl.UpdateLifeTotalAsync(e.Lifechanged*-1);
            }
        }

        private void _manager_PlayerColorChanged(object sender, ColorChangedEvent e)
        {
            if (e.ID == viewModel.ID)
                return;

            UpdateCommanderButtonColor(e.ID, e.Color);

        }

        public void Flip(int degrees = 180)
        {
            ApplyRotation(degrees);
        }

        public void SetBackGround(BackGroundColors color)
        {
            viewModel.Color = color;
            LifeControl.SetColor(viewModel.Color);           
        }

        Gametypes _lastType = Gametypes.MultiPlayer;

        public void Reset(Gametypes type = Gametypes.MultiPlayer)
        {
            if (type == Gametypes.Current)
                type = _lastType;

            _lastType = type;            

            viewModel.LifeTotal = _manager.GetLifeValue(type);
            viewModel.Energy = 0;
            viewModel.Experience = 0;
            viewModel.Poison = 0;
            viewModel.CmdEnemy1 = 0;
            viewModel.CmdEnemy2 = 0;
            viewModel.CmdEnemy3 = 0;

            if (type == Gametypes.Commander)
                LifeControl.SetValue(Grid.RowSpanProperty, 1);
            else 
                LifeControl.SetValue(Grid.RowSpanProperty, 2);


            cmdLife1.LifeChanged -= CmdLife_LifeChanged;
            cmdLife2.LifeChanged -= CmdLife_LifeChanged;
            cmdLife3.LifeChanged -= CmdLife_LifeChanged;

            var cmd = 1;
            foreach (var id in _manager.ActivePlayers())
            {
                if (id == viewModel.ID)
                    continue;
                if (id == PlayerID.Unknown)
                    continue;

                CommanderButtonMapping[id] = cmd;

                switch (cmd)
                {
                    case 1:
                        borderLife1.Visibility = cmdLife1.Visibility = Visibility.Visible;
                        break;

                    case 2:
						borderLife2.Visibility = cmdLife2.Visibility = Visibility.Visible;
                        break;

                    case 3:
						borderLife3.Visibility = cmdLife3.Visibility = Visibility.Visible;
                        break;

                    default:
                        break;
                }
                cmd++;
            }

            cmdLife1.LifeChanged += CmdLife_LifeChanged;
            cmdLife2.LifeChanged += CmdLife_LifeChanged;
            cmdLife3.LifeChanged += CmdLife_LifeChanged;


            BorderCmd.Visibility = type == Gametypes.Commander ? Visibility.Visible : Visibility.Collapsed;

            LifeControl.SetLife(viewModel.LifeTotal);
            UpdateCustomType();
            UpdateCommanderDmg();
        }    

        private void BtnIncreaseEnergy_Click(object sender, RoutedEventArgs e)
        {
            switch (viewModel.CounterType)
            {
                case CustomCounterType.Energy:
                    viewModel.Energy++;
                    break;
                case CustomCounterType.Experience:
                    viewModel.Experience++;
                    break;
                case CustomCounterType.Poison:
                    viewModel.Poison++;
                    break;
            }


            UpdateCustomType();
        }

        private void BtnDecreaseEnergy_Click(object sender, RoutedEventArgs e)
        {
            switch (viewModel.CounterType)
            {
                case CustomCounterType.Energy:
                    viewModel.Energy--;
                    if (viewModel.Energy <= 0)
                        viewModel.Energy = 0;
                    break;
                case CustomCounterType.Experience:
                    viewModel.Experience--;
                    if (viewModel.Experience <= 0)
                        viewModel.Experience = 0;
                    break;
                case CustomCounterType.Poison:
                    viewModel.Poison--;
                    if (viewModel.Poison <= 0)
                        viewModel.Poison = 0;
                    break;
            }

            UpdateCustomType();
        }


        private void UpdateCustomType()
        {
            switch (viewModel.CounterType)
            {
                case CustomCounterType.Energy:
                    tbCounterType.Text = viewModel.Energy.ToString();// $"Energy: {viewModel.Energy}";
                    break;
                case CustomCounterType.Experience:
                    tbCounterType.Text = viewModel.Experience.ToString();// $"Experience: {viewModel.Experience}";
                    break;
                case CustomCounterType.Poison:
                    tbCounterType.Text = viewModel.Poison.ToString();// $"Poison: {viewModel.Poison}";
                    break;
                case CustomCounterType.LifeHistory:
                    tbCounterType.Text = _LifeHistory.ToString();
                    break;
            }
        }

        private void btnCounterType_Click(object sender, RoutedEventArgs e)
        {
            SwitchCounterType();
        }

        private void tbEnergy_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            SwitchCounterType();
        }

        private void SwitchCounterType()
        {
            if (viewModel.CounterType == CustomCounterType.LifeHistory)
                return;

            switch (viewModel.CounterType)
            {
                case CustomCounterType.Energy:
                    viewModel.CounterType = CustomCounterType.Experience;
                    break;
                case CustomCounterType.Experience:
                    viewModel.CounterType = CustomCounterType.Poison;
                    break;
                case CustomCounterType.Poison:
                    viewModel.CounterType = CustomCounterType.Energy;
                    break;
            }
            UpdateCustomType();

            //imgCountertype.Source = new BitmapImage(new Uri("ms-appx:///" + CounterTypeHelper.CounterTypeImage(viewModel.CounterType)));
        }      

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Reset(_lastType);
        }
               
        private void UpdateCommanderDmg()
        {
            cmdLife1.SetLife(viewModel.CmdEnemy1);
            cmdLife2.SetLife(viewModel.CmdEnemy2);
            cmdLife3.SetLife(viewModel.CmdEnemy3);

        }

        private async void UpdateCommanderDmgAsync(int cmd1, int cmd2, int cmd3)
        {
            await cmdLife1.SetLifeAsync(viewModel.CmdEnemy1);
            await cmdLife2.SetLifeAsync(viewModel.CmdEnemy2);
            await cmdLife3.SetLifeAsync(viewModel.CmdEnemy3);
        }

        private void BtnReset_Click_1(object sender, RoutedEventArgs e)
        {
            Reset(_lastType);
        }

        private void BtnFlip_Click(object sender, RoutedEventArgs e)
        {
            ApplyRotation();
        }

        private void ApplyRotation(int degrees = 180)
        {
            ControlRotation = (ControlRotation + degrees) % 360;
            //rtAngle.Angle = Rotation;
            ToprtAngle.Angle = ControlRotation;
            //rtPanelOptions.Angle = Rotation;

            //playerOption.Hide();
        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            SetBackGround(BackGroundColors.Red);
        }

        private void btnGreen_Click(object sender, RoutedEventArgs e)
        {
            SetBackGround(BackGroundColors.Green);
        }

        private void btnBlue_Click(object sender, RoutedEventArgs e)
        {
            SetBackGround(BackGroundColors.Blue);
        }

        private void btnPurple_Click(object sender, RoutedEventArgs e)
        {
            SetBackGround(BackGroundColors.Purple);
        }

        private void btnYellow_Click(object sender, RoutedEventArgs e)
        {
            SetBackGround(BackGroundColors.Yellow);
        }

        private void UpdateCommanderButtonColor(PlayerID iD, BackGroundColors color)
        {
            int btn = CommanderButtonMapping[iD];

            //Color clr = Colors.Red;

            //switch (color)
            //{
            //    case BackGroundColors.Red:
            //        clr = Colors.Red;
            //        break;
            //    case BackGroundColors.Blue:
            //        clr = Colors.Blue;
            //        break;
            //    case BackGroundColors.Green:
            //        clr = Colors.ForestGreen;
            //        break;
            //    case BackGroundColors.Purple:
            //        clr = Colors.MediumPurple;
            //        break;
            //    case BackGroundColors.Yellow:
            //        clr = Colors.Goldenrod;
            //        break;
            //}

            //clr.A = 127;

            switch (btn)
            {
                case 1:
					//btnCmdE1.Background = new SolidColorBrush(clr);
					//ChangePlayerColor(this.viewModel.Color, "Player1ColorBrush");
					cmdLife1.SetColor(color);
                    break;
                case 2:
					//btnCmdE2.Background = new SolidColorBrush(clr);
					//ChangePlayerColor(this.viewModel.Color, "Player2ColorBrush");
					cmdLife2.SetColor(color);

                    break;
                case 3:
					//btnCmdE3.Background = new SolidColorBrush(clr);
					//ChangePlayerColor(this.viewModel.Color, "Player3ColorBrush");
					cmdLife3.SetColor(color);

                    break;
                default:
                    break;
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            PlayerContainer.Visibility = Visibility.Collapsed;
            SettingsControl.Visibility = Visibility.Visible;
            SettingsControl.Update();

        }

        private void OnCloseSettings()
        {
            PlayerContainer.Visibility = Visibility.Visible;
            SettingsControl.Visibility = Visibility.Collapsed;

            LifeControl.SetLife(viewModel.LifeTotal);
            UpdateCustomType();
            UpdateCommanderDmg();

        }
       
        private void btnSettings_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if(e.HoldingState ==  Windows.UI.Input.HoldingState.Completed)
            {
                Reset(_lastType);
            }
        }
    }
}
