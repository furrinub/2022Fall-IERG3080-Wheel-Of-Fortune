using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Printing;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace _2022Fall_IERG3080_Wheel_Of_Fortune
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private static Random rng = new Random();
        private static List<int> rewardList = new List<int>() { 1000, 100, 10, -500, 500, -100, -999, -10 };
        private static Dictionary<int, SolidColorBrush> colorTable = new ()
            {
                { 1000, Brushes.Red },
                { 100, Brushes.MediumBlue },
                { 10, Brushes.Lime },
                { -500, Brushes.Gray },
                { 500, Brushes.Purple },
                { -100, Brushes.LightPink },
                { -999, Brushes.LightBlue },
                { -10, Brushes.Yellow },
            };
        private static System.Windows.Threading.DispatcherTimer dispatcherTimer = new ();

        private static int money = 1000;
        private static int status = 0;
        private static bool gameOver = false;

        private static readonly double maxRotateSpeed = 12.0;
        private static double rotateSpeed = 0.0;
        private static double angle = 22.5;


        public MainWindow()
        {
            InitializeComponent();
            GameInit();
            dispatcherTimer.Tick += TimerTick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);
            dispatcherTimer.Start();
        }

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        private void GameInit()
        {
            // Init wheel
            Shuffle(rewardList);
            for (int i=0; i<8; i++)
            {
                (pies.Children[i] as Path)!.Fill = colorTable[rewardList[i]];
                (pies.Children[8 + i] as TextBlock)!.Text = rewardList[i].ToString();
            }

            // Init component
            money_text.Text = money.ToString();
            profit_indicator.Text = "";
            title_text.Text = "Wheel of Fortune";
            gameOver = false;
            start_button.Content = "Start";
            EnableStartButton(true);
            EnableStopButton(false);
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            if (status == 0)
            {
                return;
            }
            else if (status > 0 && status <= 100)
            {
                rotateSpeed = status / 100.0 * maxRotateSpeed;
                status++;
            }
            else if (status > 100 && status <= 500)
            {
                // Enable stop button
                if (status == 101)
                {
                    EnableStopButton(true);
                }
                status++;
            }
            else if (status > 500 && status <= 600)
            {
                // Disable stop button
                if (status == 501)
                {
                    EnableStopButton(false);
                }
                double t = (status - 500) / 100.0;
                rotateSpeed = (1.0 - t*t) * maxRotateSpeed;
                status++;
            }
            else if (status > 600)
            {
                EnableStartButton(true);
                status = 0;
                rotateSpeed = 0;

                UpdateMoney(rewardList[(int)(angle % 360.0 / 45.0)]);
                if (money <= 0)
                {
                    GameOver();
                }
            }
            
            angle = (angle + rotateSpeed) % 360;
            var transformGroup = new TransformGroup();
            TranslateTransform tr = pies.RenderTransform as TranslateTransform ?? new();
            transformGroup.Children.Add(tr);
            transformGroup.Children.Add(new RotateTransform(angle));
            pies.RenderTransform = transformGroup;
        }

        private void UpdateMoney(int n)
        {
            money += n;
            money_text.Text = "$" + money.ToString();
            if (n >= 0)
            {
                profit_indicator.Text = "(+" + n.ToString() + ")";
                profit_indicator.Foreground = Brushes.Green;
            }
            else
            {
                profit_indicator.Text = "(-" + (-n).ToString() + ")";
                profit_indicator.Foreground = Brushes.Red;
            }
        }

        private void EnableStartButton(bool b)
        {
            if (b)
            {
                // Enable start button
                start_button.IsEnabled = true;
                start_button.Foreground = Brushes.Black;
            }
            else
            {
                // Disable start button
                start_button.IsEnabled = false;
                start_button.Foreground = Brushes.Gray;
            }
        }

        private void GameOver()
        {
            title_text.Text = "Game Over!";
            start_button.Content = "Restart";

        }

        private void EnableStopButton(bool b)
        {
            if (b)
            {
                // Enable stop button
                stop_button.IsEnabled = true;
                stop_button.Foreground = Brushes.Red;
            }
            else
            {
                // Disable stop button
                stop_button.IsEnabled = false;
                stop_button.Foreground = Brushes.Gray;
            }
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            if (gameOver)
            {
                GameInit();
            }
            else
            {
                status = 1;
                EnableStartButton(false);
            }
        }

        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            status = 500;
        }
    }
}
