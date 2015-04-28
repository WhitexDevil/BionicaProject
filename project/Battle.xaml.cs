using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using RazorGDIControlWPF;

namespace project
{
	/// <summary>
	/// Interaction logic for Battle.xaml
	/// </summary>
	public partial class Battle : UserControl
	{
		#region Magic Shit allowing fast GDI drawing in WPF aaplication
		//thx to http://habrahabr.ru/post/164705/

		private System.Timers.Timer fpstimer;
		private DispatcherTimer rendertimer;
		private Thread renderthread;
		private int fps;

		private delegate void fpsdelegate();
		private void showfps()
		{
			//this.Title = "FPS: " + fps; 
			fps = 0;
		}
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			fpstimer = new System.Timers.Timer(1000);
			fpstimer.Elapsed += (sender1, args) =>
			{
				Dispatcher.BeginInvoke(DispatcherPriority.Render, new fpsdelegate(showfps));
				fps2 = fps;
				if (fps2 < 60 && delay > 0)
					Interlocked.Decrement(ref delay);
				else if (delay < 1000)
					Interlocked.Increment(ref delay);
			};
			fpstimer.Start();

			// !! uncomment for regular FPS renderloop !!
			//Task.Run(async () =>
			//{
			//	while (true)
			//	{
			//		Render();
			//		await Task.Delay(delay);
			//	}
			//});
			//rendertimer = new DispatcherTimer();
			//rendertimer.Interval = TimeSpan.FromMilliseconds(5); /* ~60Hz LCD on my PC */
			//rendertimer.Tick += (o, args) => Render();
			//rendertimer.Start();

			// !! comment for maximum FPS renderloop !!

		}
		int fps2;
		int delay = 10;
		bool stop = false;
		Font font = new Font("Arial", 10);
		private void Render()
		{
			if (!this.IsVisible) return;
			// do lock to avoid resize/repaint race in control
			// where are BMP and GFX recreates
			// better practice is Monitor.TryEnter() pattern, but here we do it simpler
			lock (razorPainterWPFCtl1.RazorLock)
			{
				//if (visualization != null)
				//	visualization.DrawFrame(razorPainterWPFCtl1.RazorGFX, razorPainterWPFCtl1.RazorWidth, razorPainterWPFCtl1.RazorHeight, frame);

				if (renderedframe != frame)
				{
					Visualization.DrawFrame(g, Buffer.Width, Buffer.Height, frame);
					g.Flush();
					renderedframe = frame;
				}
				if (Visualization == null)
				{
					razorPainterWPFCtl1.RazorGFX.Clear(System.Drawing.SystemColors.Control);
					var nya = global::project.Properties.Resources.horo_vector_4_by_straywolf;
					razorPainterWPFCtl1.RazorGFX.DrawImage(nya, 0, razorPainterWPFCtl1.RazorHeight - (nya.Height * razorPainterWPFCtl1.RazorWidth / nya.Width), razorPainterWPFCtl1.RazorWidth, nya.Height * razorPainterWPFCtl1.RazorWidth / nya.Width);
				}
				else
					razorPainterWPFCtl1.RazorGFX.DrawImageUnscaled(Buffer, 0, 0);
				if ((Buffer.Width != razorPainterWPFCtl1.RazorWidth || Buffer.Height != razorPainterWPFCtl1.RazorHeight) && (
					razorPainterWPFCtl1.RazorWidth > 0 && razorPainterWPFCtl1.RazorWidth < 10000 &&
					razorPainterWPFCtl1.RazorHeight > 0 && razorPainterWPFCtl1.RazorHeight < 10000
					))
				{
					g.Dispose();
					Buffer = new Bitmap((int)razorPainterWPFCtl1.RazorWidth, (int)razorPainterWPFCtl1.RazorHeight);
					g = Graphics.FromImage(Buffer);
				}
				razorPainterWPFCtl1.RazorGFX.DrawString(fps2.ToString(), font, System.Drawing.Brushes.Black, new PointF(0, 0));
				razorPainterWPFCtl1.RazorPaint();
			}
			fps++;
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			g.Dispose();
			stop = true;
			Task.Run(async () =>
			{
				await Task.Delay(1000);
				if (renderthread.IsAlive)
					renderthread.Abort();
			});
			renderthread.Join();

			//rendertimer.Stop();
			fpstimer.Stop();
		}
		#endregion
		Bitmap Buffer = new Bitmap(10, 10);
		Graphics g;

		private float frame;
		private float renderedframe;

		public float Frame
		{
			get { return frame; }
			set
			{
				lock (razorPainterWPFCtl1.RazorLock)
				{
					if (frame == value || visualization == null) return;
					frame = value;
				}
				//lock (razorPainterWPFCtl1.RazorLock)
				//	using (Graphics g = Graphics.FromImage(Buffer))
				//		Visualization.DrawFrame(g, Buffer.Width, Buffer.Height, Frame);
			}
		}


		private int time;

		public void SetTimeAndFrame(int time, float frame)
		{
			lock (razorPainterWPFCtl1.RazorLock)
			{
				if (visualization == null) return;
				if (time != this.time)
				{
					this.time = Math.Min(Visualization.BattleLength, time);
					Visualization.SetTime(time);
				}
				if (frame == this.frame) return;
				this.frame = frame;
			}
		}

		public int Time
		{
			get { return time; }
			set
			{
				lock (razorPainterWPFCtl1.RazorLock)
				{
					if (value == time || visualization == null) return;
					time = Math.Min(Visualization.BattleLength, value);
					Visualization.SetTime(time);
					Frame = 0;
				}
			}
		}

		public Battle()
		{
			InitializeComponent();
		}

		private Visualization visualization;

		public Visualization Visualization
		{
			get { return visualization; }
			set
			{
				if (value == null || value == visualization) return;
				visualization = value;
				Time = 0;
			}
		}

		private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue && (renderthread == null || !renderthread.IsAlive))
			{
				if (g != null) g.Dispose();
				g = Graphics.FromImage(Buffer);
				renderthread = new Thread(() =>
				{
					while (!stop)
					{
						Render();
						Thread.Sleep(delay);
					}
				});
				renderthread.Start();
			}
			else
			{
				stop = true;
				Task.Run(async () =>
				{
					await Task.Delay(1000);
					if (renderthread.IsAlive)
						renderthread.Abort();
				});
				renderthread.Join();
			}
		}




	}
}
