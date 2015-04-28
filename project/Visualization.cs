using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace project
{
	using Step = KeyValuePair<Point, float>;

	#region Trying replace graphics wih canvas
	//using System.Windows.Media;
	//using System.Windows.Media.Imaging;
	//using System.IO;

	//class CanvasGraphics
	//{
	//	Canvas Canvas;
	//	Graphics Graphics;
	//	public void DrawImage(Bitmap image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
	//	{
	//		if (Canvas != null)
	//		{
	//			var Image = new System.Windows.Controls.Image();
	//			Image.Source = BitmapToImageSource(image);
	//		}
	//		else
	//			Graphics.DrawImage(image, destRect, srcRect, srcUnit);
	//	}

	//	BitmapImage BitmapToImageSource(Bitmap bitmap)
	//	{
	//		using (MemoryStream memory = new MemoryStream())
	//		{
	//			bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
	//			memory.Position = 0;
	//			BitmapImage bitmapimage = new BitmapImage();
	//			bitmapimage.BeginInit();
	//			bitmapimage.dr
	//			bitmapimage.StreamSource = memory;
	//			bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
	//			bitmapimage.EndInit();

	//			return bitmapimage;
	//		}
	//	}
	//} 
	#endregion

	public class Sprite
	{
		private Bitmap Texture;
		private Bitmap MirroredTexture;
		private RectangleF[][][] Animations;
		private bool DisableTextures;
		private Color Color;
		private bool Mirrored;

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
					new RectangleF(directionDegree < 180 ? position.X : position.X - size.Width*(spriteK-1), 
						position.Y - (spriteK * h - size.Height), spriteK * size.Width, spriteK * h),

					directionDegree < 180 ^ Mirrored ? AniRect :
					new RectangleF(Texture.Width - (AniRect.X + AniRect.Width), AniRect.Y, AniRect.Width, AniRect.Height)
				, GraphicsUnit.Pixel);
			}
		}
		private void DrawDamage(Graphics g, PointF position, SizeF size, float directionDegree, int damage, float frame)
		{
			g.DrawString(damage.ToString(), new Font("Arial", 12, FontStyle.Bold),
				new SolidBrush(Color.FromArgb(255 - (int)(frame * 255 / Visualization.SubturnFramerate), Color.Red)),
				new PointF(directionDegree >= 180 ? position.X + size.Width * 0.55F : position.X,
					position.Y + size.Height * 0.5F - (size.Height * frame / Visualization.SubturnFramerate)));
		}
		private void DrawHealth(Graphics g, PointF position, SizeF size, float directionDegree, int health)
		{
			g.DrawString(health.ToString(), new Font("Arial", 11), Brushes.Green,
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

	public class Visualization
	{
		Squad[] Squads;
		BattleData InitBD;
		BattleData BD;
		List<List<Action>> Turns = new List<List<Action>>();
		List<Action> Timeline = new List<Action>();
		public List<string> BattleLog = new List<string>();

		private int turn;

		public int Turn
		{
			get { return turn; }
			set
			{
				turn = value;
				SideA = turn % 2 == 0;
			}
		}

		bool SideA { get; set; }
		int Subturn = 0;
		int SubturnRelative = 0;
		public static float SubturnFramerate = 60;

		enum ActionType { None, Attack, Move }

		struct Action
		{
			public int Squad;
			public int Target;
			public Point[] Path;
			public int Damage;
			public ActionType Type;
		}

		public Visualization(BattleData battleData)
		{
			InitZeroState(battleData);
		}

		private int FindIndex(Squad squad)
		{
			for (int i = 0; i < Squads.Length; i++)
				if (Squads[i] == squad) return i;
			return -1;
		}
		public void RecordMove(Squad squad, Point start, Point end, Step[] path)
		{
			//	Console.WriteLine("Move");
			var pathcutted = path.Select(x => x.Key).SkipWhile(x => x != end).TakeWhile(x => x != start).Concat(new Point[] { start }).ToArray();
			if (pathcutted.Length < 2) return;
			Timeline.Add(new Action()
			{
				Type = ActionType.Move,
				Squad = FindIndex(squad),
				Target = -1,
				Path = pathcutted,
				Damage = 0
			});
			Turns.Last().Add(Timeline.Last());
		}
		public void RecordAttack(Squad squad, Squad target, int damage)
		{
			//Console.WriteLine("Attack");
			int squadIndex = FindIndex(squad);
			Timeline.Add(new Action()
			{
				Type = ActionType.Attack,
				Squad = FindIndex(squad),
				Target = FindIndex(target),
				Path = null,
				Damage = damage
			});
			Turns.Last().Add(Timeline.Last());
		}
		public void RecordTurn()
		{
			//Console.WriteLine("NextTurn");
			Turns.Add(new List<Action>());
		}

		public int BattleLength { get { return Timeline.Count; } }



		private void InitZeroState(BattleData battleData)
		{
			Squads = battleData.AllyArmy.Concat(battleData.EnemyArmy).ToArray();
			InitBD = (BattleData)battleData.Clone();
			for (int i = 0; i < InitBD.AllyArmy.Length; i++)
			{
				InitBD.Map[InitBD.AllyArmy[i].Position.X +
					(InitBD.AllyArmy[i].Position.Y << InitBD.MapHeightLog2)] = (byte)(i + 1);
			}
			for (int i = 0; i < InitBD.EnemyArmy.Length; i++)
			{
				InitBD.Map[InitBD.EnemyArmy[i].Position.X +
					(InitBD.EnemyArmy[i].Position.Y << InitBD.MapHeightLog2)] =
					(byte)(i + 1 + InitBD.AllyArmy.Length);
			}
		}


		public void SetTime(int subturn)
		{
			if (subturn < Subturn || subturn == 0)
			{
				BD = (BattleData)InitBD.Clone();
				Squads = BD.AllyArmy.Concat(BD.EnemyArmy).ToArray();
				Subturn = 0;
				SubturnRelative = 0;
				Turn = 0;
				BattleLog.Clear();
			}
			for (int t = Subturn; t < Math.Min(subturn, Timeline.Count - 1); t++)
			{
				//var Ally = SideA ? BD.AllyArmy : BD.EnemyArmy;
				//var Enemy = Ally == BD.EnemyArmy ? BD.AllyArmy : BD.EnemyArmy;

				switch (Timeline[t].Type)
				{
					case ActionType.Attack:
						Squads[Timeline[t].Target].Amount -= Timeline[t].Damage;
						BattleLog.Add(String.Format("Отряд #{0} атакует отряд {1} и наносит {2} урона",
							Timeline[t].Squad, Timeline[t].Target, Timeline[t].Damage));
						if (!Squads[Timeline[t].Target].Alive)
							BattleLog.Add(String.Format("Отряд #{0} погибает", Timeline[t].Target));
						break;
					case ActionType.Move:
						Squads[Timeline[t].Squad].Position = Timeline[t].Path.First();
						BattleLog.Add(String.Format("Отряд #{0} идет в точку {1}:{2}",
							Timeline[t].Squad, Timeline[t].Path.First().X, Timeline[t].Path.First().X));
						break;
				}

				Subturn = t + 1;
				SubturnRelative++;
				if (SubturnRelative >= Turns[Turn].Count)
				{
					SubturnRelative = 0;
					Turn++;
					BattleLog.Add(String.Format("Ход {0} завершен, теперь ходит сторона " + (SideA ? "A" : "B"), Turn - 1));
				}
			}
		}

		public static float DirectionDegree(Point source, Point destination)
		{
			var n = 270 -(Math.Atan2(source.Y - destination.Y, source.X - destination.X)) * 180 / Math.PI;
			return (float)(n % 360);

			//float xDiff = destination.X - source.X;
			//float yDiff = destination.Y - source.Y;
			//return (float)((Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI) + 90);
		}



		public void DrawFrame(Bitmap b, float frame)
		{
			using (Graphics g = Graphics.FromImage(b))
				DrawFrame(g, b.Width, b.Height, frame);
		}

		public void DrawFrame(Graphics g, int width, int height, float frame)
		{
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			g.Clear(Color.White);
			float wK = (float)width / InitBD.MapWidth;
			float hK = (float)height / InitBD.MapHeight;

			for (float x = 0; x < width; x += wK)
				g.DrawLine(Pens.LightGray, x, 0, x, height);
			for (float y = 0; y < height; y += hK)
				g.DrawLine(Pens.LightGray, 0, y, width, y);

			var YSorted = Squads.Select((x, i) => new KeyValuePair<Squad, int>(x, i)).OrderBy(x2 => x2.Key.Position.Y);

			var Size = new SizeF(wK, hK);
			var Action = Timeline[Subturn];
			foreach (var SortedSquad in YSorted)
			{
				var Squad = SortedSquad.Key;
				bool SideASquad = SortedSquad.Value < BD.AllyArmy.Length;
				var Sprite = SideASquad ? Squad.Unit.SideASprite : Squad.Unit.SideBSprite;

				if (SortedSquad.Value == Action.Squad)
				{
					switch (Action.Type)
					{
						case ActionType.Attack:

							Sprite.DrawAttack(g,
								new PointF(Squad.Position.X * wK, Squad.Position.Y * hK), Size,
								DirectionDegree(Squad.Position, Squads[Action.Target].Position),
								Squad.Amount, frame);

							break;
						case ActionType.Move:

							float framecost = (SubturnFramerate + 1) / (Action.Path.Length - 1);
							int start = Action.Path.Length - 1 - (int)(frame / framecost);
							float X = Action.Path[start].X + (Action.Path[start - 1].X - Action.Path[start].X) * ((frame % framecost) / framecost);
							float Y = Action.Path[start].Y + (Action.Path[start - 1].Y - Action.Path[start].Y) * ((frame % framecost) / framecost);

							Sprite.DrawMove(g, new PointF(X * wK, Y * hK), Size,
									DirectionDegree(Action.Path[start], Action.Path[start - 1]), Squad.Amount, frame);
							break;
					}
				}
				else if (SortedSquad.Value == Action.Target)
				{
					Sprite.DrawTakingDamage(g,
						new PointF(Squad.Position.X * wK, Squad.Position.Y * hK), Size,
						SideASquad ? 270 : 0, Squad.Amount, Action.Damage, frame);
				}
				else
				{
					Sprite.DrawStanding(g,
						new PointF(Squad.Position.X * wK, Squad.Position.Y * hK), Size,
						SideASquad ? 0 : 270, Squad.Amount, frame);
				}
			}


			#region old
			//for (int a = 0; a < 2; a++)
			//{
			//	bool sideA = (a % 2) == 0;

			//	var Army = AllyTurn() == sideA ? BD.EnemyArmy : BD.AllyArmy;
			//	for (int i = 0; i < Army.Length; i++)
			//	{
			//		var Action = sideA ? new Action() : Timeline[Turn][i, SecondAction ? 1 : 0];
			//		var Position = Army[i].Position;

			//		switch (Action.Type)
			//		{
			//			case ActionType.Attack:
			//				g.FillRectangle(AllyTurn() == sideA ? Brushes.LightBlue : Brushes.Violet, Position.X * wK, Position.Y * hK, 10, 10);
			//				g.DrawString(Action.Damage.ToString(), new Font("Arial", 15), Brushes.Red, Action.Target.X * wK, Action.Target.Y * hK - (hK * frame / SubturnFramerate));
			//				g.DrawString(Army[i].Amount.ToString(), new Font("Arial", 10), Brushes.Green, Position.X * wK - wK / 3, Position.Y * hK - hK / 3);
			//				break;
			//			default:
			//				if (Action.Path != null && Action.Path.Length >= 2)
			//				{
			//					float framecost = SubturnFramerate / (Action.Path.Length - 1);
			//					int pp = (int)(frame / framecost);
			//					int start = Action.Path.Length - 1 - pp;
			//					float X = Action.Path[start].X + (Action.Path[start - 1].X - Action.Path[start].X) * ((frame % framecost) / framecost);
			//					float Y = Action.Path[start].Y + (Action.Path[start - 1].Y - Action.Path[start].Y) * ((frame % framecost) / framecost);
			//					g.FillRectangle(AllyTurn() == sideA ? Brushes.LightBlue : Brushes.Violet, X * wK, Y * hK, 10, 10);
			//					g.DrawString(Army[i].Amount.ToString(), new Font("Arial", 10), Brushes.Green, X * wK - wK / 3, Y * hK - hK / 3);
			//				}
			//				else
			//				{
			//					if (Army[i].Alive)
			//					{
			//						g.FillRectangle(AllyTurn() == sideA ? Brushes.LightBlue : Brushes.Violet, Position.X * wK,
			//							(Position.Y * hK), 10, 10);
			//						g.DrawString(Army[i].Amount.ToString(), new Font("Arial", 10), Brushes.Green, Position.X * wK - wK / 3, Position.Y * hK - hK / 3);
			//					}
			//					else g.FillRectangle(AllyTurn() == sideA ? Brushes.LightGray : Brushes.LightGray, Position.X * wK,
			//						 (Position.Y * hK), 10, 10);
			//				}
			//				break;
			//		}
			//	}

			//}


			//var army = AllyTurn() ? BD.EnemyArmy : BD.AllyArmy;
			//for (int i = 0; i < army.Length; i++)
			//{
			//	if (army[i].Alive)
			//		g.DrawRectangle(AllyTurn() ? Pens.Violet : Pens.Blue, army[i].Position.X * wK,
			//			(army[i].Position.Y * hK) + (hK * frame / Framerate / 2), 10, 10);
			//	else g.DrawRectangle(AllyTurn() ? Pens.DarkViolet : Pens.DarkBlue, army[i].Position.X * wK,
			//		 (army[i].Position.Y * hK) + (hK * frame / Framerate / 2), 10, 10);
			//}
			//for (int i = 0; i < 2; i++)
			//{
			//	if ((Time + i) % 2 == 0)
			//		DrawArmy(g, wK, hK, BD.AllyArmy, 1, frame);
			//	else
			//		DrawArmy(g, wK, hK, BD.EnemyArmy, 1 + BD.AllyArmy.Length, frame);
			//} 



			#endregion
		}

		#region old
		//Bitmap[] ally, enemy;
		//Bitmap[] ally, enemy;

		//void DrawArmy(Graphics g, float wK, float hK, Squad[] army, int indexShift, int frame)
		//{
		//	for (int i = 0; i < army.Length; i++)
		//	{
		//		var action = actions[Time][i + indexShift, Time % 2];
		//		var Position = BD.AllyArmy[i].Position;
		//		switch (action.Type)
		//		{
		//			case ActionType.Attack:
		//				g.DrawRectangle(Pens.Blue, Position.X * wK, Position.Y * hK, 10, 10);
		//				g.DrawString(action.damage.ToString(), new Font("Arial", 15), Brushes.Red, Position.X * wK, Position.Y * hK);
		//				break;
		//			case ActionType.Move:
		//				float framecost = Framerate / (action.path.Length - 1);
		//				int pp = (int)(frame / framecost);
		//				float X = action.path[pp].X + (action.path[pp + 1].X - action.path[pp].X) * ((frame % framecost) / framecost);
		//				float Y = action.path[pp].Y + (action.path[pp + 1].Y - action.path[pp].Y) * ((frame % framecost) / framecost);
		//				g.DrawRectangle(Pens.Blue, X * wK, Y * hK, 10, 10);
		//				break;
		//			default:
		//				g.DrawRectangle(Pens.Blue, Position.X * wK, Position.Y * hK, 10, 10);
		//				break;
		//		}
		//	}
		//}
		//public void NextTurn()
		//{
		//	var army = BD.AllyArmy;
		//	int indexShift = 1;
		//	if (Turn % 2 == 0)
		//	{
		//		army = BD.EnemyArmy;
		//		indexShift = 1 + BD.AllyArmy.Length;
		//	}
		//	for (int i = 0; i < army.Length; i++)
		//	{
		//		var action = actions[Turn][i + indexShift, Turn % 2];
		//		var Position = BD.AllyArmy[i].Position;
		//		switch (action.Type)
		//		{
		//			case ActionType.Attack:
		//				g.DrawRectangle(Pens.Blue, Position.X * wK, Position.Y * hK, 10, 10);
		//				g.DrawString(action.damage.ToString(), new Font("Arial", 15), Brushes.Red, Position.X * wK, Position.Y * hK);
		//				break;
		//			case ActionType.Move:
		//				float framecost = SubturnFramerate / (action.path.Length - 1);
		//				int pp = (int)(frame / framecost);
		//				float X = action.path[pp].X + (action.path[pp + 1].X - action.path[pp].X) * ((frame % framecost) / framecost);
		//				float Y = action.path[pp].Y + (action.path[pp + 1].Y - action.path[pp].Y) * ((frame % framecost) / framecost);
		//				g.DrawRectangle(Pens.Blue, X * wK, Y * hK, 10, 10);
		//				break;
		//			default:
		//				g.DrawRectangle(Pens.Blue, Position.X * wK, Position.Y * hK, 10, 10);
		//				break;
		//		}
		//	}

		//	Turn++;
		//}
		#endregion
	}
}
