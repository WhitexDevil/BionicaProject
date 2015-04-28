using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{

	public partial class SpriteDebugForm : Form
	{
		public SpriteDebugForm()
		{
			InitializeComponent();
			textBox1.MouseWheel += textBox1_MouseWheel;

		}



		public class Sprite
		{
			public Bitmap Texture;
			public Bitmap MiroredTexture;
			public RectangleF[][][] Animations;
			public int Width;
			public int Height;

			public Sprite(Bitmap texture, RectangleF[][][] animations)
			{
				Animations = animations;
				this.Texture = texture;
			}

			public enum AnimationAction { Standing, Moving, Attacking, TakingDamage, Dying };

			private RectangleF[] GetAnimation(AnimationAction action, float directionDegree)
			{
				if (directionDegree > 180) directionDegree = 180 - (directionDegree % 180);
				RectangleF[][] ActionAnimations = Animations[(int)action];
				return ActionAnimations[(int)(directionDegree * ActionAnimations.Length / 180F)];
			}

			public void DrawSpriteFrame(Graphics g, PointF position, SizeF size, AnimationAction action, float directionDegree, float frame)
			{
				var Texture = directionDegree > 180 ? MiroredTexture : this.Texture;
				var Animation = GetAnimation(action, directionDegree);
				g.DrawImage(Texture, new RectangleF(position, size),
					Animation[(int)(frame * Animation.Length / 60)], GraphicsUnit.Pixel);
			}
			private void DrawDamage(Graphics g, PointF position, SizeF size, float directionDegree, int damage, float frame)
			{
				g.DrawString(damage.ToString(), new Font("Arial", 11),
					new SolidBrush(Color.FromArgb(255 - (int)(frame * 255 / 60), Color.Red)),
					new PointF(directionDegree > 180 ? position.X : position.X - size.Width / 2,
						position.Y + (size.Height * 2 / 3) - (size.Height * frame / 2 / 60)));
			}
			private void DrawHealth(Graphics g, PointF position, SizeF size, float directionDegree, int health)
			{
				g.DrawString(health.ToString(), new Font("Arial", 11), Brushes.Green,
					new PointF(directionDegree > 180 ? position.X : position.X - size.Width / 2, position.Y + size.Height * 2 / 3));
			}
			private void DrawDie(Graphics g, PointF position, SizeF size, float directionDegree, float frame)
			{
				DrawSpriteFrame(g, position, size, AnimationAction.Dying, directionDegree, frame);
			}
			public void DrawStanding(Graphics g, PointF position, SizeF size, float directionDegree, int health, float frame)
			{
				if (health > 0)
				{
					DrawSpriteFrame(g, position, size, AnimationAction.Standing, directionDegree, frame);
					DrawHealth(g, position, size, directionDegree, health);
				}
				else
				{
					DrawDie(g, position, size, directionDegree, 60);
				}
			}
			public void DrawTakingDamage(Graphics g, PointF position, SizeF size, float directionDegree, int health, int damage, float frame)
			{
				if (damage >= health)
				{
					DrawDie(g, position, size, directionDegree, frame);
					DrawDamage(g, position, size, directionDegree, damage, frame);
				}
				else
				{
					DrawSpriteFrame(g, position, size, AnimationAction.TakingDamage, directionDegree, frame);
					DrawHealth(g, position, size, directionDegree, health - damage);
					DrawDamage(g, position, size, directionDegree, damage, frame);
				}
			}
			public void DrawAttack(Graphics g, PointF position, SizeF size, float directionDegree, int health, float frame)
			{
				DrawSpriteFrame(g, position, size, AnimationAction.Attacking, directionDegree, frame);
				DrawHealth(g, position, size, directionDegree, health);
			}
			public void DrawMove(Graphics g, PointF position, SizeF size, float directionDegree, int health, float frame)
			{
				DrawSpriteFrame(g, position, size, AnimationAction.Moving, directionDegree, frame);
				DrawHealth(g, position, size, directionDegree, health);
			}
		}


		Bitmap Texture;
		Bitmap Freezed;
		Sprite sprite;
		RectangleF[] Frames;

		private void button1_Click(object sender, EventArgs e)
		{
			if (button1.Text == "Stop")
			{
				Animator.Enabled = false;
				button1.Text = "Animate";
			}
			else
			{
				Animator.Enabled = true;
				button1.Text = "Stop";
			}
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			try
			{
				Frames = textBox1.Lines.Where(x4 => !String.IsNullOrEmpty(x4.Trim())).Select((x) =>
				{
					float[] d = x.Trim().Split(new char[] { ' ', ';', ',', '\\' }).Select(x2 => float.Parse(x2)).ToArray();
					return new RectangleF(d[0], d[1], d[2], d[3]);
				}).ToArray();
				trackBar1.Maximum = Frames.Length - 1;
				trackBar1.Minimum = 0;
				textBox1.ForeColor = Color.Black;
				trackBar1_Scroll(sender, e);
			}
			catch (Exception Error)
			{
				textBox1.ForeColor = Color.Red;
			}
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			try
			{
				int i = trackBar1.Value;
				if (Frames == null || i < 0 || i >= Frames.Length || Texture == null) return;
				//if (i == Frames.Length) pictureBox1.Image = Texture;
				Bitmap b = new Bitmap((int)Frames[i].Width, (int)Frames[i].Height);
				using (Graphics g = Graphics.FromImage(b))
				{
					g.DrawImage(Texture, new RectangleF(0, 0, b.Width, b.Height), Frames[i], GraphicsUnit.Pixel);
					if (Freezed != null)
						g.DrawImage(Freezed, 0, 0);
				}
				pictureBox1.Image = b;
				pictureBox2.Refresh();
			}
			catch { }
		}

		private void button3_Click(object sender, EventArgs e)
		{
			try
			{
				var d = new OpenFileDialog();
				if (d.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
				Texture = new Bitmap(d.FileName);
				sprite = new Sprite(Texture, null);
				pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
				pictureBox2.SizeMode = PictureBoxSizeMode.Normal;
				pictureBox2.Image = Texture;
			}
			catch (Exception Error)
			{
				MessageBox.Show(Error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{

			try
			{
				if (button2.Text == "Unfreeze Sample")
				{
					Freezed = null;
					button2.Text = "Freeze Sample";
				}
				else if (pictureBox1.Image != null)
				{
					Freezed = pictureBox1.Image.ChangeOpacity(0.5F);
					button2.Text = "Unfreeze Sample";
				}
			}
			catch (Exception Error)
			{
				MessageBox.Show(Error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void trackBar2_Scroll(object sender, EventArgs e)
		{
			Animator.Interval = trackBar2.Value;
		}

		private void Animator_Tick(object sender, EventArgs e)
		{
			trackBar1.Value = (trackBar1.Value + 1) % (trackBar1.Maximum + 1);
			trackBar1_Scroll(sender, e);
		}


		private void button4_Click(object sender, EventArgs e)
		{
			if (Frames == null || Frames.Length == 0)
				MessageBox.Show("Nothing to code", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Error);

			StringBuilder code = new StringBuilder("var Animation = new RectangleF[]{\n");
			for (int i = 0; i < Frames.Length; i++)
				code.AppendLine(
					String.Format("new RectangleF({0},{1},{2},{3})",
					Frames[i].X, Frames[i].Y, Frames[i].Width, Frames[i].Height) + (i == Frames.Length - 1 ? "};" : ","));
			System.Windows.Forms.Clipboard.SetText(code.ToString());
			MessageBox.Show("Here! Code copied to clippoard! (^_^)", "Let me write code istead of you");

		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{

		}

		private void textBox1_KeyUp(object sender, KeyEventArgs e)
		{
			int i = textBox1.GetLineFromCharIndex(textBox1.SelectionStart);
			if (Frames != null && i >= 0 && i < Frames.Length)
			{
				trackBar1.Value = i;
				trackBar1_Scroll(sender, e);
			}
		}

		private void Form1_Scroll(object sender, ScrollEventArgs e)
		{

		}

		void textBox1_MouseWheel(object sender, MouseEventArgs e)
		{
			if (textBox1.ForeColor == Color.Black)
			{
				try
				{
					int i2 = textBox1.GetLineFromCharIndex(textBox1.SelectionStart);
					string line = textBox1.Lines[i2];
					int start = 0;
					int end = 0;
					for (int i = 0; i < textBox1.Text.Length; i++)
					{
						if (i < textBox1.SelectionStart && !char.IsNumber(textBox1.Text[i]))
							start = i;
						if (i >= textBox1.SelectionStart)
						{
							if (!char.IsNumber(textBox1.Text[i]) || textBox1.Text.Length - 1 == i)
							{
								end = i; break;
							}
						}
					}
					if (end == textBox1.Text.Length - 1) end = textBox1.Text.Length;
					if (end == 0) end = textBox1.SelectionStart;

					if (start == textBox1.SelectionStart) start = textBox1.SelectionStart - 1;
					if (start == 0) start = -1;

					string snum = "";
					for (int i = start + 1; i < end; i++)
						snum += textBox1.Text[i];
					//	Text = snum;
					int num = int.Parse(snum);
					if (e.Delta > 0)
						num++;
					else if (num > 0) num--;
					snum = "";

					for (int i = 0; i < start + 1; i++)
						snum += textBox1.Text[i];
					snum += num.ToString();
					for (int i = end; i < textBox1.Text.Length; i++)
						snum += textBox1.Text[i];

					int selstart = textBox1.SelectionStart;
					int sellen = textBox1.SelectionLength;

					textBox1.Text = snum;
					textBox1.SelectionStart = selstart;
					textBox1.SelectionLength = sellen;
				}
				catch { }
			}
		}

		private void textBox1_Click(object sender, EventArgs e)
		{
			if (textBox1.Text == "Change values with mouse scroll! Try it!")
				textBox1.Text = "";
		}

		private void textBox1_Enter(object sender, EventArgs e)
		{

		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
		}

		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{
			try
			{
				var pos = pictureBox1.PointToClient(Cursor.Position);
				if (!pictureBox1.ClientRectangle.Contains(pos)) return;
				//new Point(Cursor.Position.X - Left, Cursor.Position.Y - Top);
				e.Graphics.DrawLine(Pens.Black, pos.X, 0, pos.X, pictureBox1.Height);
				e.Graphics.DrawLine(Pens.Black, 0, pos.Y, pictureBox1.Width, pos.Y);
			}
			catch { }
		}
		private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
		{
			try
			{
				Text = String.Format("Relative {0}:{1} Absolute {2}:{3}", e.X, e.Y,
					(Frames == null || trackBar1.Value >= Frames.Length || trackBar1.Value < 0) ? 0 : Frames[trackBar1.Value].X + e.X,
					(Frames == null || trackBar1.Value >= Frames.Length || trackBar1.Value < 0) ? 0 : Frames[trackBar1.Value].Y + e.Y);
				pictureBox1.Refresh();
			}
			catch { }
		}


		private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
		{
			var pos = pictureBox2.PointToClient(Cursor.Position);
			Text = String.Format("Absolute {0}:{1}", pos.X, pos.Y);
			pictureBox2.Refresh();
		}

		private void pictureBox2_Paint(object sender, PaintEventArgs e)
		{
			try
			{
				var pos = pictureBox2.PointToClient(Cursor.Position);
				if (Frames != null && trackBar1.Value >= 0 && trackBar1.Value < Frames.Length)
				{
					e.Graphics.DrawRectangle(Pens.Black, Frames[trackBar1.Value].X, Frames[trackBar1.Value].Y,
						Frames[trackBar1.Value].Width, Frames[trackBar1.Value].Height);
					//			e.Graphics.FillRectangle(Pens.Black, Frames[trackBar1.Value].X, Frames[trackBar1.Value].Y,
					//Frames[trackBar1.Value].Width, Frames[trackBar1.Value].Height);
				}

				if (!pictureBox2.ClientRectangle.Contains(pos)) return;
				//new Point(Cursor.Position.X - Left, Cursor.Position.Y - Top);
				e.Graphics.DrawLine(Pens.Black, pos.X, 0, pos.X, pictureBox2.Height);
				e.Graphics.DrawLine(Pens.Black, 0, pos.Y, pictureBox2.Width, pos.Y);
			}
			catch { }

		}

		private void pictureBox1_MouseLeave(object sender, EventArgs e)
		{
			pictureBox1.Refresh();
		}

		private void pictureBox2_MouseLeave(object sender, EventArgs e)
		{
			pictureBox2.Refresh();
		}

		private void textBox1_MouseClick(object sender, MouseEventArgs e)
		{
			textBox1_KeyUp(sender, null);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			try
			{
				if (Frames != null && Frames.Length > 1)
				{
					var prelast = Frames[Frames.Length-2];
					var last = Frames[Frames.Length-1];
					textBox1.AppendText(String.Format("\r\n{0} {1} {2} {3}",last.X + (last.X - prelast.X), last.Y + (last.Y - prelast.Y), last.Width, last.Height));
				}
			}
			catch (Exception Error)
			{
				MessageBox.Show(Error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}

	public static class ImageTransparency
	{
		public static Bitmap ChangeOpacity(this Image img, float opacityvalue)
		{
			Bitmap bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
			Graphics graphics = Graphics.FromImage(bmp);
			ColorMatrix colormatrix = new ColorMatrix();
			colormatrix.Matrix33 = opacityvalue;
			ImageAttributes imgAttribute = new ImageAttributes();
			imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
			graphics.Dispose();   // Releasing all resource used by graphics 
			return bmp;
		}
	}
}
