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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MtgLifeCounter
{
    public sealed partial class LifeControl : UserControl
    {
        private int LifeTotal = 20;
        private bool CanGoNegative = true;

        public event LifeChangedEventHandler LifeChanged;

        public LifeControl()
        {
            this.InitializeComponent();

            //BtnDecreaseLife.AddHandler(PointerPressedEvent, new PointerEventHandler(BtnDecreaseLife_PointerPressed), true);
            BtnDecreaseLife.AddHandler(PointerReleasedEvent, new PointerEventHandler(BtnDecreaseLife_PointerReleased), true);

           // BtnIncreaseLife.AddHandler(PointerPressedEvent, new PointerEventHandler(BtnIncreaseLife_PointerPressed), true);
            BtnIncreaseLife.AddHandler(PointerReleasedEvent, new PointerEventHandler(BtnIncreaseLife_PointerReleased), true);

        }

        public void Init(bool negativeAllowed, int life)
        {
            CanGoNegative = negativeAllowed;

            if (!CanGoNegative && life < 0)
                life = 0;
            SetLife(life);
        }

        public  void SetLife(int life)
        {
            if (!CanGoNegative && life < 0)
                life = 0;

            LifeTotal = life;
            UpdateLifeTotal();
        }

        public async Task SetLifeAsync(int life)
        {
            if (!CanGoNegative && life < 0)
                life = 0;

            LifeTotal = life;
            await UpdateLifeTotalAsync(0);
        }

        public void SetColor(BackGroundColors color)
        {
            object objectStyle = null;
            SolidColorBrush pc = null;
            if (this.Resources.TryGetValue("PlayerColorBrush", out objectStyle))
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
        }

        public async Task UpdateLifeTotalAsync(int i)
        {          
            await Task.Run(() =>
            {
                lock (this)
                {
                    if (!CanGoNegative && (LifeTotal + i < 0))
                        return;

                     LifeTotal += i;

                    string leftTxt, rightTxt;
                    LifeTotalToText(out leftTxt, out rightTxt);

                    UpdateLifeTextBox(leftTxt, rightTxt);
                    LifeChanged?.Invoke(this, new LifeChangedEventArgs(i));
                }
            });
        }

        private void LifeTotalToText(out string leftTxt, out string rightTxt)
        {
            string lifeTotalString = LifeTotal.ToString();
            leftTxt = "";
            rightTxt = "";
            if (LifeTotal < 0)
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
            //else if(LifeTotal ==0)
            //{
            //    leftTxt = "";
            //    rightTxt = "0";
            //}
            else
            {
                if (LifeTotal <= 9)
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
        }

        public void UpdateLifeTotal()
        {         
            string leftTxt, rightTxt;
            LifeTotalToText(out leftTxt, out rightTxt);

            BtnDecreaseLife.Content = leftTxt;
            BtnIncreaseLife.Content = rightTxt;
        }

        private async void UpdateLifeTextBox(string leftTxt, string rightTxt)
        {

            if (!BtnDecreaseLife.Dispatcher.HasThreadAccess)
            {
                await BtnDecreaseLife.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { BtnDecreaseLife.Content = leftTxt; });
            }

            if (!BtnIncreaseLife.Dispatcher.HasThreadAccess)
            {
                await BtnIncreaseLife.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { BtnIncreaseLife.Content = rightTxt; });
            }
        }

        int _incrementDecreaseLife = 0;
        private void BtnDecreaseLife_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _incrementDecreaseLife = 0;
        }

        private async void BtnDecreaseLife_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_incrementDecreaseLife > 5)
            {
                await UpdateLifeTotalAsync(-5);
            }
            else
            {
                await UpdateLifeTotalAsync(-1);
                _incrementDecreaseLife++;
            }
        }
        int _incrementIncreaseLife = 0;
        private void BtnIncreaseLife_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _incrementIncreaseLife = 0;
        }

        private async void BtnIncreaseLife_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_incrementDecreaseLife > 5)
            {
                await UpdateLifeTotalAsync(5);
            }
            else
            {
                await UpdateLifeTotalAsync(1);
                _incrementIncreaseLife++;
            }
        }

        private async void BtnDecreaseLife_Click_1(object sender, RoutedEventArgs e)
        {
            await UpdateLifeTotalAsync(-1);
        }

        private async void BtnIncreaseLife_Click(object sender, RoutedEventArgs e)
        {
            await UpdateLifeTotalAsync(1);


        }
    }
}
