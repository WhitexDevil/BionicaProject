using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GA ga;
        Player enemy;
        Squad[] army;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            enemy = new Player(new double[4]{0.1,0.9,0.9,0.9});
            Unit humanKnights =  new Unit( 4, 17, 3, 5, 7, 25, 1.5f);
            Unit humanSoliders = new Unit( 4, 16, 2, 4,  4, 30, 1.5f);
            Unit humanArcher =   new Unit( 4, 12, 5, 4,  3, 20, 10f);
            int n = 4;
            army = new Squad[3*n];
            for (int i = 0; i < army.Length; i+=3)
            {
                army[i] = new Squad(humanKnights);
                army[i+1] = new Squad(humanSoliders);
                army[i+2] = new Squad(humanArcher);
            }
            //for (int i = 0; i < ; i++)
            //{
            //    if (i<army.Length/2)
            //    army[i] = new Squad(humanKnights);
            //    else
            //    army[i] = new Squad(humanSoliders);
            //}

			Stopwatch sw = Stopwatch.StartNew();

            ga = new GA(enemy, army, battleCount: 10,generationSize:3,populationSize:50);
            ga.Go();

            sw.Stop();
            System.Windows.MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
		
			SandBox sb = new SandBox(enemy, ga.GetBest(), army, army, 64) { Visualization = true };
			v = sb.BattleData.Visualization;
			sb.Fight(1);
			if (b.Width != PictureBox.ActualWidth || b.Height != PictureBox.ActualHeight)
				b = new Bitmap((int)Math.Max(PictureBox.ActualWidth, 100), (int)Math.Max(PictureBox.ActualHeight, 100));

				v.DrawFrame(b, 0);

			PictureBox.Source = BitmapToImageSource(b);
                        
        }


		void HowToCreateSprite()
		{

			RectangleF[][][] Animations = new RectangleF[Enum.GetValues(typeof(Sprite.AnimationAction)).Length][][];
			for (int i = 0; i < Enum.GetValues(typeof(Sprite.AnimationAction)).Length; i++)
			{
				switch ((Sprite.AnimationAction)i)
				{
					case Sprite.AnimationAction.Standing:
						Animations[i] = new RectangleF[directions][];
						Animations[i][direction] = new RectangleF[frames];
						
						Animations[i][direction][frame1] = new RectangleF(0, 0, 50, 50);
						Animations[i][direction][frame2] = new RectangleF(50, 0, 50, 50);
						Animations[i][direction][frame3] = new RectangleF(100, 0, 50, 50);

						break;
					case Sprite.AnimationAction.Moving:
						break;
					case Sprite.AnimationAction.Attacking:
						break;
					case Sprite.AnimationAction.TakingDamage:
						break;
					case Sprite.AnimationAction.Dying:
						break;
				}
			

			var Sprite = new Sprite(texture, Animations);

		}

		Visualization v;
		BitmapImage BitmapToImageSource(Bitmap bitmap)
		{
			using (MemoryStream memory = new MemoryStream())
			{
				bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
				memory.Position = 0;
				BitmapImage bitmapimage = new BitmapImage();
				bitmapimage.BeginInit();
				bitmapimage.StreamSource = memory;
				bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapimage.EndInit();

				return bitmapimage;
			}
		}
		Bitmap b;
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			b = new Bitmap((int)Math.Max(PictureBox.ActualWidth, 100), (int)Math.Max(PictureBox.ActualHeight,100));
		}

		private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			SubTurnLabel.Content = "Subturn: " + ((int)(e.NewValue * v.BattleLength / TimeSlider.Maximum)).ToString();
			v.SetTime((int)(e.NewValue * v.BattleLength / TimeSlider.Maximum));
			if (b.Width != (int)PictureBox.ActualWidth || b.Height != (int)PictureBox.ActualHeight)
				b = new Bitmap((int)Math.Max(PictureBox.ActualWidth, 100), (int)Math.Max(PictureBox.ActualHeight, 100));

				v.DrawFrame(b, (int)(e.NewValue * v.BattleLength * 60 / TimeSlider.Maximum) % 60);

			PictureBox.Source = BitmapToImageSource(b);
		}

		private async void Button_Click_1(object sender, RoutedEventArgs e)
		{
			while (TimeSlider.Value < TimeSlider.Maximum)
			{
				TimeSlider.Value += TimeSlider.Maximum * 0.001;
				await Task.Delay(1);
			}
		}
    }
}
