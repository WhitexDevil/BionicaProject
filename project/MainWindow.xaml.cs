using project.Controls;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Effects;
using Teske.WPF.Effects;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Linq;

namespace project
{
    class GeneralPrototype
    {

        public double Agr { get; set; }
        public double Wair { get; set; }
        public double Perc { get; set; }
        public double Prd { get; set; }
    }

    public partial class MainWindow
    {

        GA ga;
        Player enemy;
        GeneralPrototype general = new GeneralPrototype();

        Squad[] army;

        public MainWindow()
        {
            InitializeComponent();
            Animator.Tick += Animator_Tick;
        }

        void Animator_Tick(object sender, EventArgs e)
        {
            TimeSlider.Value += TimeSlider.Maximum * 0.0001;
            if (TimeSlider.Value == TimeSlider.Maximum)
            {
                Animator.IsEnabled = false;
            }
        }

        void Animator_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //TimeSlider.Value += TimeSlider.Maximum * 0.0001;
            //if (TimeSlider.Value == TimeSlider.Maximum) { 
            //	Animator.Enabled = false; 
            //}
        }



     
        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            rightControl.IsOpen = false;
            general.Agr = GeneralOption.Agr.Slider1.Value;
            general.Wair = GeneralOption.Wair.Slider1.Value;
            general.Perc = GeneralOption.Per.Slider1.Value;
            general.Prd = GeneralOption.Prd.Slider1.Value;
            enemy = new Player(new double[4] { general.Agr, general.Wair, general.Perc, general.Prd });


            int Generation = (int)GaOption.Generation.Slider1.Value;
            int Population = (int)GaOption.Population.Slider1.Value;
            double WintRate = GaOption.WintRate.Slider1.Value / 100d;
            int BattleCount = (int)GaOption.Battles.Slider1.Value;

            Simulate.IsEnabled = false;
            Reset.IsEnabled = false;
            Start.IsEnabled = false;
            loading.IsActive = false;

            int n = 4;
            army = new Squad[3 * n];
            for (int i = 0; i < army.Length; i += 3)
            {
                army[i] = new Squad(humanKnights);
                army[i + 1] = new Squad(humanSoliders);
                army[i + 2] = new Squad(humanArcher);
            }


            ga = new GA(enemy, army, mapSize: 32,
                generationSize: Generation,
                populationSize: Population,
                goalFitness: WintRate,
                battleCount: BattleCount);
            await Task.Run(() => ga.Go());
        
            loading.IsActive = false;
            Start.IsEnabled = true;
            TimeSlider.IsEnabled = true;
            GeneralList.ItemsSource = ga.GetLastGeneration().ToArray().Select((x) => { return "Power: "+((Player)x).WinRate.ToString("0.##") + " Gens: " + String.Join("/ ",((Player)x).Genes().Select(x2=>x2.ToString("0.##"))); });
          

        }

        Visualization Visualization;



        private async void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double d = TimeSlider.Maximum;
            SubTurnLabel.Content = "Subturn: " + ((int)(e.NewValue * Visualization.BattleLength / TimeSlider.Maximum)).ToString();
            await Task.Run(() =>
            {
                Battle.SetTimeAndFrame(
                    ((int)(e.NewValue * Visualization.BattleLength / d)),
                    (float)(e.NewValue * Visualization.BattleLength * 60 / d) % 60);
                Dispatcher.Invoke((Action)(() =>
                {
                    lock (Visualization.BattleLog)
                        BattleLog.Text = String.Join(String.Empty, Visualization.BattleLog);
                    BattleLog.ScrollToEnd();
                }));
            });
        }

        DispatcherTimer Animator = new DispatcherTimer();
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {



             fly.IsEnabled = false;
            rightControl.IsOpen = false;
            if ((string)Animate.Content == "Animate")
            {
              
                Animate.Content = "Stop";
                Animator.IsEnabled = true;
            }
            else
            {
                //fly.IsEnabled = true;
                Animate.Content = "Animate";
                Animator.IsEnabled = false;
            }



        }


        private void SetUpWindow(object sender, RoutedEventArgs e)
        {
            Animate.IsEnabled = false;
            TimeSlider.IsEnabled = false;
            Battle.Width = Battle.ActualHeight;
        }

        private void SetUpWindow(object sender, SizeChangedEventArgs e)
        {
            if (main.Width > main.Height)
            {
                Battle.Width = main.Height * 0.85;
                Battle.Height = main.Height * 0.85;
            }
            else
            {
                Battle.Width = main.Width * 0.85;
                Battle.Height = main.Width * 0.85;
            }
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            rightControl.IsOpen = true;

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Animator.IsEnabled = false;
            fly.IsEnabled = true;
            TimeSlider.Value = 0;
            Battle.Frame = -1;
            BattleLog.Clear();
            GeneralList.ItemsSource = null;
            Reset.IsEnabled = false;
            Animate.IsEnabled = false;
            Simulate.IsEnabled = false;
            Animate.Content = "Animate";

        }

        private void Simulate_Click(object sender, RoutedEventArgs e)
        {
            int i = GeneralList.SelectedIndex;
            if (i == -1)
                return;
            SandBox sb = new SandBox(enemy, ga.GetNthGenome(i), army, army, 32) { Visualization = true };
            Visualization = sb.BattleData.Visualization;
            sb.Fight(1);
            Visualization.SetTime(0);
            Battle.Visualization = Visualization;
            Battle.Frame = 0;
            Animate.IsEnabled = true;
            Reset.IsEnabled = true;
            
        }

        private void GeneralList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Simulate.IsEnabled = true;
        }



        //private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //	int X1 = (int)(Width / 2);
        //	int Y1 = (int)(Height / 2);
        //	int X2 =  (int)(e.GetPosition(this).X);
        //	int Y2 = (int)(e.GetPosition(this).Y);
        //	Title = String.Format("Angle: {0} Start: {1}:{2} Mouse: {3}:{4}",
        //		Visualization.DirectionDegree(new System.Drawing.Point(X1,Y1),
        //		new System.Drawing.Point(X2,Y2)),X1,Y1,X2,Y2 

        //		);
        //	l.X1 = X1;
        //	l.X2 = X2;
        //	l.Y1 = Y1;
        //	l.Y2 = Y2;
        //}

    }
}
