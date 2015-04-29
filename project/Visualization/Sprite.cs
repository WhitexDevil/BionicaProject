using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace project
{
	using Step = KeyValuePair<Point, float>;
	using System.Drawing.Imaging;
	public class Sprite
	{
		private Bitmap Texture;
		private Bitmap MirroredTexture;
		private RectangleF[][][] Animations;
		private bool DisableTextures;
		private Color Color;
		private bool Mirrored;
		public static Bitmap Background = global::project.Properties.Resources.Fon;
		private Font Font = new Font("Arial", 12, FontStyle.Bold);


		public Sprite(Color color)
		{
			DisableTextures = true;
			this.Color = color;
		}
		public Sprite(Bitmap texture, RectangleF[][][] animations, bool mirrored)
		{
			this.Animations = animations;
			for (int i = 0; i < animations[(int)AnimationAction.Moving].Length; i++)
			{
				var a = animations[(int)AnimationAction.Moving][i];
				while (animations[(int)AnimationAction.Moving][i].Length < 30)
					animations[(int)AnimationAction.Moving][i] = animations[(int)AnimationAction.Moving][i].Concat(a).ToArray();
				animations[(int)AnimationAction.Moving][i] = animations[(int)AnimationAction.Moving][i].Take(30).ToArray();
			}
			this.Texture = new Bitmap(texture);
			this.MirroredTexture = new Bitmap(Texture);
			(mirrored ? this.Texture : this.MirroredTexture).RotateFlip(RotateFlipType.RotateNoneFlipX);
			this.Mirrored = mirrored;
		}

		public enum AnimationAction { Standing, Moving, Attacking, TakingDamage, Dying };

		private RectangleF[] GetAnimation(AnimationAction action, float directionDegree)
		{
			if (directionDegree >= 180) directionDegree = 180 - (directionDegree % 180);
			RectangleF[][] ActionAnimations = Animations[(int)action];
			return ActionAnimations[(int)(directionDegree * ActionAnimations.Length / 181F)];
		}
		const float spriteK = 1.5F;
		private void DrawSpriteFrame(Graphics g, PointF position, SizeF size, AnimationAction action, float directionDegree, float frame)
		{
			if (DisableTextures)
			{
				g.FillEllipse(new SolidBrush(Color), new RectangleF(position, size));
			}
			else
			{
				var Texture = directionDegree >= 180 ? MirroredTexture : this.Texture;
				var Animation = GetAnimation(action, directionDegree);
				var AniRect = Animation[(int)(frame * Animation.Length / (Visualization.SubturnFramerate + 1))];
				var h = AniRect.Height * size.Width / AniRect.Width;
				g.DrawImage(Texture,
					new RectangleF(directionDegree < 180 ? position.X : position.X - size.Width * (spriteK - 1),
						position.Y - (spriteK * h - size.Height), spriteK * size.Width, spriteK * h),

					directionDegree < 180 ^ Mirrored ? AniRect :
					new RectangleF(Texture.Width - (AniRect.X + AniRect.Width), AniRect.Y, AniRect.Width, AniRect.Height)
				, GraphicsUnit.Pixel);
			}
		}

		private void DrawDamage(Graphics g, PointF position, SizeF size, float directionDegree, int damage, float frame)
		{
			g.DrawString(damage.ToString(), Font,
				new SolidBrush(Color.FromArgb(255 - (int)(frame * 255 / Visualization.SubturnFramerate), Color.Red)),
				new PointF(directionDegree >= 180 ? position.X + size.Width * 0.55F : position.X,
					position.Y + size.Height * 0.5F - (size.Height * frame / Visualization.SubturnFramerate)));
		}
		private void DrawHealth(Graphics g, PointF position, SizeF size, float directionDegree, int health)
		{
			g.DrawString(health.ToString(), Font, Brushes.White,
				new PointF(directionDegree >= 180 ? position.X + size.Width * 0.55F : position.X, position.Y + size.Height * 0.5F));
		}
		private void DrawDie(Graphics g, PointF position, SizeF size, float directionDegree, float frame)
		{
			if (DisableTextures)
			{
				g.FillEllipse(new SolidBrush(Color), new RectangleF(position, size));
				g.DrawLine(Pens.Red, position, new PointF(position.X + size.Width, position.Y + size.Height));
				g.DrawLine(Pens.Red,
					new PointF(position.X + size.Width, position.Y),
					new PointF(position.X, position.Y + size.Height));
			}
			else
				DrawSpriteFrame(g, position, size, AnimationAction.Dying, directionDegree, frame);
		}
		public void DrawStanding(Graphics g, PointF position, SizeF size, float directionDegree, int health, float frame)
		{
			position = new PointF(position.X, position.Y + size.Height);
			if (health > 0)
			{
				DrawSpriteFrame(g, position, size, AnimationAction.Standing, directionDegree, frame);
				DrawHealth(g, position, size, directionDegree, health);
			}
			else
			{
				//DrawDie(g, position, size, directionDegree, Visualization.SubturnFramerate);
			}
		}
		public void DrawTakingDamage(Graphics g, PointF position, SizeF size, float directionDegree, int health, int damage, float frame)
		{
			position = new PointF(position.X, position.Y + size.Height);
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
			position = new PointF(position.X, position.Y + size.Height);
			DrawSpriteFrame(g, position, size, AnimationAction.Attacking, directionDegree, frame);
			DrawHealth(g, position, size, directionDegree, health);
		}
		public void DrawMove(Graphics g, PointF position, SizeF size, float directionDegree, int health, float frame)
		{
			position = new PointF(position.X, position.Y + size.Height);
			DrawSpriteFrame(g, position, size, AnimationAction.Moving, directionDegree, frame);
			DrawHealth(g, position, size, directionDegree, health);
		}
	}

}
