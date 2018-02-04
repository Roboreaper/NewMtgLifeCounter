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

            LifeControl.Init(true,viewModel.LifeTotal);
            UpdateEnergy();

            cmdLife1.Init(false, viewModel.CmdEnemy1);
            cmdLife2.Init(false, viewModel.CmdEnemy2);
            cmdLife3.Init(false, viewModel.CmdEnemy3);


            ToprtAngle.Angle = 0;
            //rtPanelOptions.Angle = 0;

            cmdLife1.Visibility = Visibility.Collapsed;
            cmdLife2.Visibility = Visibility.Collapsed;
            cmdLife3.Visibility = Visibility.Collapsed;

            SettingsControl.Init(this.viewModel, this, OnClose);

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

            this.DataContext = viewModel;
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
            _lastType = type;
            viewModel.LifeTotal = type == Gametypes.MultiPlayer ? 20 : 40;
            viewModel.Energy = 0;
            viewModel.Experience = 0;
            viewModel.Poison = 0;
            viewModel.CmdEnemy1 = 0;
            viewModel.CmdEnemy2 = 0;
            viewModel.CmdEnemy3 = 0;

            if (type == Gametypes.Commander)
                LifeControl.SetValue(Grid.RowSpanProperty, 1);
            else if (type == Gametypes.MultiPlayer)
                LifeControl.SetValue(Grid.RowSpanProperty, 2);


            BorderCmd.Visibility = type == Gametypes.Commander ? Visibility.Visible : Visibility.Collapsed;

            LifeControl.SetLife(viewModel.LifeTotal);
            UpdateEnergy();
            UpdateCommanderDmg();
        }        

        //private async void BtnDecreaseLife_Click(object sender, RoutedEventArgs e)
        //{
        //    //viewModel.LifeTotal--;
        //    await UpdateLifeTotalAsync(-1);
        //}

        //private async void BtnIncreaseLife_Click(object sender, RoutedEventArgs e)
        //{
        //    //viewModel.LifeTotal++;
        //    await UpdateLifeTotalAsync(1);
        //}

        //public void UpdateLifeTotal()
        //{
        //    string lifeTotalString = viewModel.LifeTotal.ToString();
        //    string leftTxt = "";
        //    string rightTxt = "";
        //    if (viewModel.LifeTotal < 0)
        //    {
        //        leftTxt = "-";

        //        if (lifeTotalString.Length > 2)
        //        {
        //            leftTxt += lifeTotalString[1];
        //            rightTxt = lifeTotalString.Substring(2);
        //        }
        //        else
        //        {
        //            rightTxt = lifeTotalString.Substring(1);
        //        }

        //    }
        //    else
        //    {
        //        if (viewModel.LifeTotal <= 9)
        //        {
        //            leftTxt = "0";
        //            rightTxt = lifeTotalString;
        //        }
        //        else
        //        {
        //            leftTxt += lifeTotalString[0];
        //            rightTxt = lifeTotalString.Substring(1);
        //        }
        //    }

        //    tbDecreaseLife.Text = leftTxt;
        //    tbIncreaseLife.Text = rightTxt;
        //}

        //public async Task UpdateLifeTotalAsync(int i)
        //{
        //    await Task.Run(() =>
        //    {
        //        viewModel.LifeTotal += i;
        //        string lifeTotalString = viewModel.LifeTotal.ToString();
        //        string leftTxt = "";
        //        string rightTxt = "";
        //        if (viewModel.LifeTotal < 0)
        //        {
        //            leftTxt = "-";

        //            if (lifeTotalString.Length > 2)
        //            {
        //                leftTxt += lifeTotalString[1];
        //                rightTxt = lifeTotalString.Substring(2);
        //            }
        //            else
        //            {
        //                rightTxt = lifeTotalString.Substring(1);
        //            }

        //        }
        //        else
        //        {
        //            if (viewModel.LifeTotal <= 9)
        //            {
        //                leftTxt = "0";
        //                rightTxt = lifeTotalString;
        //            }
        //            else
        //            {
        //                leftTxt += lifeTotalString[0];
        //                rightTxt = lifeTotalString.Substring(1);
        //            }
        //        }

        //        //tbDecreaseLife.Text = leftTxt;
        //        //tbIncreaseLife.Text = rightTxt;
        //        UpdateLifeTextBox(leftTxt, rightTxt);
        //    });
        //}

        //private async void UpdateLifeTextBox(string left, string right)
        //{

        //    if (!tbDecreaseLife.Dispatcher.HasThreadAccess)
        //    {
        //        await tbDecreaseLife.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { tbDecreaseLife.Text = left; });
        //    }

        //    if (!tbIncreaseLife.Dispatcher.HasThreadAccess)
        //    {
        //        await tbIncreaseLife.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { tbIncreaseLife.Text = right; });
        //    }
        //}

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

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Reset(_lastType);
        }


        //private async void btnCmdE1_Click(object sender, RoutedEventArgs e)
        //{
        //    await Task.Run(async () =>
        //    {
        //        this.viewModel.CmdEnemy1++;
        //        //viewModel.LifeTotal--;
        //        await LifeControl.UpdateLifeTotalAsync(-1);
        //        UpdateCommanderDmgAsync(this.viewModel.CmdEnemy1, this.viewModel.CmdEnemy2, this.viewModel.CmdEnemy3);
        //    });
        //}

        //private async void btnCmdE2_Click(object sender, RoutedEventArgs e)
        //{
        //    await Task.Run(async () =>
        //    {
        //        this.viewModel.CmdEnemy2++;
        //        //viewModel.LifeTotal--;
        //        await LifeControl.UpdateLifeTotalAsync(-1);
        //        UpdateCommanderDmgAsync(this.viewModel.CmdEnemy1, this.viewModel.CmdEnemy2, this.viewModel.CmdEnemy3);
        //    });
        //}

        //private async void btnCmdE3_Click(object sender, RoutedEventArgs e)
        //{
        //    await Task.Run(async () =>
        //    {
        //        this.viewModel.CmdEnemy3++;
        //        //viewModel.LifeTotal--;
        //        await LifeControl.UpdateLifeTotalAsync(-1);
        //        //UpdateCommanderDmgAsync(this.viewModel.CmdEnemy1.ToString(), this.viewModel.CmdEnemy2.ToString(), this.viewModel.CmdEnemy3.ToString());
        //        UpdateCommanderDmgAsync(this.viewModel.CmdEnemy1, this.viewModel.CmdEnemy2, this.viewModel.CmdEnemy3);
        //    });

        //}

        private void UpdateCommanderDmg()
        {
            cmdLife1.SetLife(viewModel.CmdEnemy1);
            cmdLife2.SetLife(viewModel.CmdEnemy2);
            cmdLife3.SetLife(viewModel.CmdEnemy3);

            //cmdE1TB.Text = this.viewModel.CmdEnemy1.ToString();
            //cmdE2TB.Text = this.viewModel.CmdEnemy2.ToString();
            //cmdE3TB.Text = this.viewModel.CmdEnemy3.ToString();
        }

        private async void UpdateCommanderDmgAsync(int cmd1, int cmd2, int cmd3)
        {
            //cmdE1TB.Text = this.viewModel.CmdEnemy1.ToString();
            //cmdE2TB.Text = this.viewModel.CmdEnemy2.ToString();
            //cmdE3TB.Text = this.viewModel.CmdEnemy3.ToString();

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
            Rotation = (Rotation + degrees) % 360;
            //rtAngle.Angle = Rotation;
            ToprtAngle.Angle = Rotation;
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
                    cmdLife1.SetColor(color);
                    break;
                case 2:
                    //btnCmdE2.Background = new SolidColorBrush(clr);
                    cmdLife2.SetColor(color);

                    break;
                case 3:
                    //btnCmdE3.Background = new SolidColorBrush(clr);
                    cmdLife3.SetColor(color);

                    break;
                default:
                    break;
            }
        }

        private void btnRotate_Click(object sender, RoutedEventArgs e)
        {
            PlayerContainer.Visibility = Visibility.Collapsed;
            SettingsControl.Visibility = Visibility.Visible;
            SettingsControl.Update();

        }

        private void OnClose()
        {
            PlayerContainer.Visibility = Visibility.Visible;
            SettingsControl.Visibility = Visibility.Collapsed;

            LifeControl.SetLife(viewModel.LifeTotal);
            UpdateEnergy();
            UpdateCommanderDmg();

        }
    }
}
