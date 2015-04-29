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
	public partial class Battle : UserControl, IDisposable
	{
		#region Magic Shit allowing fast GDI drawing in WPF aaplication
		//or not so fast ((
		//thx to http://habrahabr.ru/post/164705/

		private System.Timers.Timer FPSTimer;
		private Thread RenderThread;
		private int FPSCounter;

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Init();
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			Dispose();
		}

		int FPS;
		int Delay = 10;
		Font font = new Font("Arial", 10);
		private void Render()
		{
			FPSCounter++;
			if (!this.IsVisible) return;
			// do lock to avoid resize/repaint race in control
			// where are BMP and GFX recreates
			// better practice is Monitor.TryEnter() pattern, but here we do it simpler
			lock (razorPainterWPFCtl1.RazorLock)
			{
				//if (visualization != null)
				//	visualization.DrawFrame(razorPainterWPFCtl1.RazorGFX, razorPainterWPFCtl1.RazorWidth, razorPainterWPFCtl1.RazorHeight, frame);

				if (RenderedFrame != RequestedFrame)
				{
					Visualization.DrawFrame(razorPainterWPFCtl1.RazorGFX,
						razorPainterWPFCtl1.RazorWidth,
						razorPainterWPFCtl1.RazorHeight, RequestedFrame);
					RenderedFrame = RequestedFrame;
					//g.Flush();
					//razorPainterWPFCtl1.RazorGFX.DrawImageUnscaled(Buffer, 0, 0);
				}
				if (Visualization == null)
				{
					razorPainterWPFCtl1.RazorGFX.Clear(System.Drawing.SystemColors.Control);
					var nya = global::project.Properties.Resources.horo_vector_4_by_straywolf;
					razorPainterWPFCtl1.RazorGFX.DrawImage(nya, 0, razorPainterWPFCtl1.RazorHeight - (nya.Height * razorPainterWPFCtl1.RazorWidth / nya.Width), razorPainterWPFCtl1.RazorWidth, nya.Height * razorPainterWPFCtl1.RazorWidth / nya.Width);
				}
				//else
				//	razorPainterWPFCtl1.RazorGFX.DrawImageUnscaled(Buffer, 0, 0);
				//if ((Buffer.Width != razorPainterWPFCtl1.RazorWidth || Buffer.Height != razorPainterWPFCtl1.RazorHeight) && (
				//	razorPainterWPFCtl1.RazorWidth > 0 && razorPainterWPFCtl1.RazorWidth < 10000 &&
				//	razorPainterWPFCtl1.RazorHeight > 0 && razorPainterWPFCtl1.RazorHeight < 10000
				//	))
				//{
				//	g.Dispose();
				//	Buffer = new Bitmap((int)razorPainterWPFCtl1.RazorWidth, (int)razorPainterWPFCtl1.RazorHeight);
				//	g = Graphics.FromImage(Buffer);
				//}
				razorPainterWPFCtl1.RazorGFX.DrawString(FPS.ToString(), font, System.Drawing.Brushes.Black, new PointF(0, 0));
				razorPainterWPFCtl1.RazorPaint();
			}
		}


		#endregion

		private float RequestedFrame;
		private float RenderedFrame;
		private object Mutex = new object();
		private bool Disposed = true;
		public float Frame
		{
			get { return RequestedFrame; }
			set
			{
				lock (razorPainterWPFCtl1.RazorLock)
				{
					if (RequestedFrame == value || visualization == null) return;
					RequestedFrame = value;
				}
			}
		}


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
				if (frame == this.RequestedFrame) return;
				this.RequestedFrame = frame;
				if (FPS < 60) Delay = 0;
			}
		}
		private int time;

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
			//if ((bool)e.NewValue)
			//	Init();
			//else
			//	Dispose();
		}


		private void Init()
		{
			lock (Mutex)
			{
				if (!Disposed) return;
				Disposed = false;

				FPSTimer = new System.Timers.Timer(1000);
				FPSTimer.Elapsed += (sender1, args) =>
				{
					FPS = FPSCounter;
					FPSCounter = 0;
					if (FPS < 60 && Delay > 0)
						Interlocked.Decrement(ref Delay);
					else if (Delay < 100)
						Interlocked.Increment(ref Delay);
				};
				FPSTimer.Start();

				RenderThread = new Thread(() =>
				{
					while (!Disposed)
					{
						Render();
						Thread.Sleep(Delay);
					}
				});
				RenderThread.Start();


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
		}


		public void Dispose()
		{
			lock (Mutex)
			{
				Disposed = true;
				Task.Run(async () =>
				{
					await Task.Delay(1000);
					if (RenderThread.IsAlive)
						RenderThread.Abort();
				});
				RenderThread.Join();

				FPSTimer.Stop();
			}

		}
	}
}
