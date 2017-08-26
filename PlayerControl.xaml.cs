using System;
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

        private int Rotation { get; set; } = 0;

        public PlayerControl()
        {
            this.InitializeComponent();
        }

        public void Init(IGameManager manager, PlayerViewModel model)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager is null");
            }
            if (model == null)
            {
                throw new ArgumentNullException("Player model is null");
            }

            this._manager = manager;
            this._manager.PlayerColorChanged += _manager_PlayerColorChanged;
            this.viewModel = model;

            if (viewModel != null)
                viewModel.PropertyChanged -= ViewModel_PropertyChanged;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;

            UpdateLifeTotal();
            UpdateName();
            UpdateEnergy();
            UpdateCommanderDmg();

            rtAngle.Angle = 0;
            rtPanelOptions.Angle = 0;

            btnCmdE1.Visibility = Visibility.Collapsed;
            btnCmdE2.Visibility = Visibility.Collapsed;
            btnCmdE3.Visibility = Visibility.Collapsed;


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
                        btnCmdE1.Visibility = Visibility.Visible;
                        break;

                    case 2:
                        btnCmdE2.Visibility = Visibility.Visible;
                        break;

                    case 3:
                        btnCmdE3.Visibility = Visibility.Visible;
                        break;

                    default:
                        break;
                }
                cmd++;
            }

            this.DataContext = viewModel;
        }

        private void _manager_PlayerColorChanged(object sender, ColorChangedEvent e)
        {
            if (e.ID == viewModel.ID)
                return;

            UpdateCommanderButtonColor(e.ID, e.Color);

        }



        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PlayerName")
            {
                UpdateName();
            }
        }

        public void Flip(int degrees = 180)
        {
            ApplyRotation(degrees);
        }

        public void SetBackGround(BackGroundColors color)
        {
            viewModel.Color = color;
            object objectStyle = null;
            SolidColorBrush pc = null;
            if (this.Resources.TryGetValue("PlayerColorBrush", out objectStyle))
            {
                pc = objectStyle as SolidColorBrush;
            }

            if (pc == null)
                return;

            switch (viewModel.Color)
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
                default:
                    break;
            }
        }

        Gametypes _lastType = Gametypes.MultiPlayer;

        public void Reset(Gametypes type = Gametypes.MultiPlayer)
        {
            _lastType = type;
            viewModel.LifeTotal = type == Gametypes.MultiPlayer ? 20 : 40;
            viewModel.Energy = 0;
            viewModel.Experience = 0;
            viewModel.Poison = 0;
            viewModel.CmdEnemy1 = 0;
            viewModel.CmdEnemy2 = 0;
            viewModel.CmdEnemy3 = 0;

            if (type == Gametypes.Commander)
                BorderLifebutton.SetValue(Grid.RowSpanProperty, 1);
            else if (type == Gametypes.MultiPlayer)
                BorderLifebutton.SetValue(Grid.RowSpanProperty, 2);

            BorderCmd.Visibility = type == Gametypes.Commander ? Visibility.Visible : Visibility.Collapsed;

            UpdateLifeTotal();
            UpdateEnergy();
            UpdateCommanderDmg();
        }

        private void UpdateName()
        {
            tbPlayerName.Text = viewModel.PlayerName;
        }

        private async void BtnDecreaseLife_Click(object sender, RoutedEventArgs e)
        {
            //viewModel.LifeTotal--;
            await UpdateLifeTotalAsync(-1);
        }

        private async void BtnIncreaseLife_Click(object sender, RoutedEventArgs e)
        {
            //viewModel.LifeTotal++;
            await UpdateLifeTotalAsync(1);
        }

        public void UpdateLifeTotal()
        {
            string lifeTotalString = viewModel.LifeTotal.ToString();
            string leftTxt = "";
            string rightTxt = "";
            if (viewModel.LifeTotal < 0)
            {
                leftTxt = "-";

                if (lifeTotalString.Length > 2)
                {
                    leftTxt += lifeTotalString[1];
                    rightTxt = lifeTotalString.Substring(2);
                }
                else
                {
                    rightTxt = lifeTotalString.Substring(1);
                }

            }
            else
            {
                if (viewModel.LifeTotal <= 9)
                {
                    leftTxt = "0";
                    rightTxt = lifeTotalString;
                }
                else
                {
                    leftTxt += lifeTotalString[0];
                    rightTxt = lifeTotalString.Substring(1);
                }
            }

            tbDecreaseLife.Text = leftTxt;
            tbIncreaseLife.Text = rightTxt;
        }

        public async Task UpdateLifeTotalAsync(int i)
        {
            await Task.Run(() =>
            {
                viewModel.LifeTotal += i;
                string lifeTotalString = viewModel.LifeTotal.ToString();
                string leftTxt = "";
                string rightTxt = "";
                if (viewModel.LifeTotal < 0)
                {
                    leftTxt = "-";

                    if (lifeTotalString.Length > 2)
                    {
                        leftTxt += lifeTotalString[1];
                        rightTxt = lifeTotalString.Substring(2);
                    }
                    else
                    {
                        rightTxt = lifeTotalString.Substring(1);
                    }

                }
                else
                {
                    if (viewModel.LifeTotal <= 9)
                    {
                        leftTxt = "0";
                        rightTxt = lifeTotalString;
                    }
                    else
                    {
                        leftTxt += lifeTotalString[0];
                        rightTxt = lifeTotalString.Substring(1);
                    }
                }

                //tbDecreaseLife.Text = leftTxt;
                //tbIncreaseLife.Text = rightTxt;
                UpdateLifeTextBox(leftTxt, rightTxt);
            });
        }

        private async void UpdateLifeTextBox(string left, string right)
        {

            if (!tbDecreaseLife.Dispatcher.HasThreadAccess)
            {
                await tbDecreaseLife.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { tbDecreaseLife.Text = left; });
            }

            if (!tbIncreaseLife.Dispatcher.HasThreadAccess)
            {
                await tbIncreaseLife.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { tbIncreaseLife.Text = right; });
            }
        }

        private void BtnIncreaseEnergy_Click(object sender, RoutedEventArgs e)
        {
            switch (viewModel.CurrentType)
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


            UpdateEnergy();
        }

        private void BtnDecreaseEnergy_Click(object sender, RoutedEventArgs e)
        {
            switch (viewModel.CurrentType)
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

            UpdateEnergy();
        }


        private void UpdateEnergy()
        {
            switch (viewModel.CurrentType)
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
            switch (viewModel.CurrentType)
            {
                case CustomCounterType.Energy:
                    viewModel.CurrentType = CustomCounterType.Experience;
                    break;
                case CustomCounterType.Experience:
                    viewModel.CurrentType = CustomCounterType.Poison;
                    break;
                case CustomCounterType.Poison:
                    viewModel.CurrentType = CustomCounterType.Energy;
                    break;
            }
            UpdateEnergy();

            imgCountertype.Source = new BitmapImage(new Uri("ms-appx:///" + CounterTypeHelper.CounterTypeImage(viewModel.CurrentType)));
        }

        private async void BtnDecreaseLife_Holding(object sender, HoldingRoutedEventArgs e)
        {
            await UpdateLifeTotalAsync(-5);
        }

        private async void BtnIncreaseLife_Holding(object sender, HoldingRoutedEventArgs e)
        {
            await UpdateLifeTotalAsync(5);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Reset(_lastType);
        }


        private async void btnCmdE1_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                this.viewModel.CmdEnemy1++;
                //viewModel.LifeTotal--;
                await UpdateLifeTotalAsync(-1);
                UpdateCommanderDmgAsync(this.viewModel.CmdEnemy1.ToString(), this.viewModel.CmdEnemy2.ToString(), this.viewModel.CmdEnemy3.ToString());
            });
        }

        private async void btnCmdE2_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                this.viewModel.CmdEnemy2++;
                //viewModel.LifeTotal--;
                await UpdateLifeTotalAsync(-1);
                UpdateCommanderDmgAsync(this.viewModel.CmdEnemy1.ToString(), this.viewModel.CmdEnemy2.ToString(), this.viewModel.CmdEnemy3.ToString());
            });
        }

        private async void btnCmdE3_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                this.viewModel.CmdEnemy3++;
                //viewModel.LifeTotal--;
                await UpdateLifeTotalAsync(-1);
                UpdateCommanderDmgAsync(this.viewModel.CmdEnemy1.ToString(), this.viewModel.CmdEnemy2.ToString(), this.viewModel.CmdEnemy3.ToString());
            });

        }

        private void UpdateCommanderDmg()
        {
            cmdE1TB.Text = this.viewModel.CmdEnemy1.ToString();
            cmdE2TB.Text = this.viewModel.CmdEnemy2.ToString();
            cmdE3TB.Text = this.viewModel.CmdEnemy3.ToString();
        }

        private async void UpdateCommanderDmgAsync(string cmd1, string cmd2, string cmd3)
        {
            //cmdE1TB.Text = this.viewModel.CmdEnemy1.ToString();
            //cmdE2TB.Text = this.viewModel.CmdEnemy2.ToString();
            //cmdE3TB.Text = this.viewModel.CmdEnemy3.ToString();

            if (!cmdE1TB.Dispatcher.HasThreadAccess)
            {
                await cmdE1TB.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { cmdE1TB.Text = cmd1; });
            }

            if (!cmdE2TB.Dispatcher.HasThreadAccess)
            {
                await cmdE2TB.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { cmdE2TB.Text = cmd2; });
            }

            if (!cmdE3TB.Dispatcher.HasThreadAccess)
            {
                await cmdE3TB.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { cmdE3TB.Text = cmd3; });
            }
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
            Rotation = (Rotation + degrees) % 360;
            rtAngle.Angle = Rotation;
            rtPanelOptions.Angle = Rotation;

            playerOption.Hide();
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

            Color clr = Colors.Red;

            switch (color)
            {
                case BackGroundColors.Red:
                    clr = Colors.Red;
                    break;
                case BackGroundColors.Blue:
                    clr = Colors.Blue;
                    break;
                case BackGroundColors.Green:
                    clr = Colors.ForestGreen;
                    break;
                case BackGroundColors.Purple:
                    clr = Colors.MediumPurple;
                    break;
                case BackGroundColors.Yellow:
                    clr = Colors.Goldenrod;
                    break;
            }

            //clr.A = 127;

            switch (btn)
            {
                case 1:
                    btnCmdE1.Background = new SolidColorBrush(clr);
                    break;
                case 2:
                    btnCmdE2.Background = new SolidColorBrush(clr);
                    break;
                case 3:
                    btnCmdE3.Background = new SolidColorBrush(clr);
                    break;
                default:
                    break;
            }
        }
    }
}
