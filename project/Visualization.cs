using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace project
{
	using Step = KeyValuePair<Point, float>;

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

		private void DrawSpriteFrame(Graphics g, PointF position, SizeF size, AnimationAction action, float directionDegree, float frame)
		{
			var Texture = directionDegree > 180 ? MiroredTexture : this.Texture;
			var Animation = GetAnimation(action, directionDegree);
			g.DrawImage(Texture, new RectangleF(position, size),
				Animation[(int)(frame * Animation.Length / Visualization.SubturnFramerate)], GraphicsUnit.Pixel);
		}
		private void DrawDamage(Graphics g, PointF position, SizeF size, float directionDegree, int damage, float frame)
		{
			g.DrawString(damage.ToString(), new Font("Arial", 11),
				new SolidBrush(Color.FromArgb(255 - (int)(frame * 255 / Visualization.SubturnFramerate), Color.Red)),
				new PointF(directionDegree > 180 ? position.X : position.X - size.Width / 2,
					position.Y + (size.Height * 2 / 3) - (size.Height * frame / 2 / Visualization.SubturnFramerate)));
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
				DrawDie(g, position, size, directionDegree, Visualization.SubturnFramerate);
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
		Squad[] AllyIndex;
		Squad[] EnemyIndex;
		BattleData InitBD;
		BattleData BD;
		List<List<Action>> Timeline = new List<List<Action>>();
		int Turn { get { return Subturn / 2; } }
		bool SecondAction { get { return Subturn % 2 == 1; } }
		int Subturn = 0;
		public static float SubturnFramerate = 60;

		enum ActionType { None, Attack, Move }

		struct Action
		{
			public Point a;
			public Point Target;
			public Point[] Path;
			public int Damage;
			public ActionType Type;
		}

		public Visualization(BattleData battleData)
		{
			InitZeroState(battleData);
		}

		private int FindIndex(Squad s)
		{
			for (int i = 0; i < AllyIndex.Length; i++)
				if (AllyIndex[i] == s) return i;
			for (int i = 0; i < EnemyIndex.Length; i++)
				if (EnemyIndex[i] == s) return i;
			return -1;
		}
		public void RecordMove(Squad s, Point start, Point end, Step[] path)
		{
			//	Console.WriteLine("Move");
			int squadIndex = FindIndex(s);
			Timeline.Last()[squadIndex,
			Timeline.Last()[squadIndex, 0].Type == ActionType.None ? 0 : 1] = new Action()
			{
				Type = ActionType.Move,
				a = start,
				Target = end,
				Path = path.Select(x => x.Key).SkipWhile(x => x != end).TakeWhile(x => x != start).ToArray(),
				Damage = 0
			};
		}
		public void RecordAttack(Squad s, Point target, int damage)
		{
			//Console.WriteLine("Attack");
			int squadIndex = FindIndex(s);
			Timeline.Last()[squadIndex,
				Timeline.Last()[squadIndex, 0].Type == ActionType.None ? 0 : 1] = new Action()
			{
				Type = ActionType.Attack,
				Target = target,
				Path = null,
				Damage = damage
			};
		}
		public void RecordTurn()
		{
			//Console.WriteLine("NextTurn");
			Timeline.Add(new Action[Timeline.Count % 2 == 0 ? InitBD.AllyArmy.Length : InitBD.EnemyArmy.Length, 2]);
		}

		public int BattleLength { get { return Timeline.Count * 2; } }

		bool AllyTurn() { return Turn % 2 == 0; }

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

		private void InitZeroState(BattleData battleData)
		{
			AllyIndex = battleData.AllyArmy;
			EnemyIndex = battleData.EnemyArmy;
			InitBD = (BattleData)battleData.Clone();
			SetTime(0);
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

		public void SetTime(int subturn)
		{
			if (subturn < Subturn || subturn == 0)
			{
				BD = (BattleData)InitBD.Clone();
				Subturn = 0;
			}
			for (int t = Subturn; t < Math.Min(subturn, (Timeline.Count * 2) - 1); t++)
			{
				var Ally = ((t / 2) % 2) == 0 ? BD.AllyArmy : BD.EnemyArmy;
				var Enemy = Ally == BD.EnemyArmy ? BD.AllyArmy : BD.EnemyArmy;
				for (int i = 0; i < Ally.Length; i++)
				{
					var Action = Timeline[t / 2][i, t % 2];
					switch (Action.Type)
					{
						case ActionType.Attack:
							for (int i2 = 0; i2 < Enemy.Length; i2++)
								if (Enemy[i2].Position == Action.Target)
									Enemy[i2].Amount -= Action.Damage;
							break;
						case ActionType.Move:
							Ally[i].Position = Action.Path.First();
							break;
					}
				}
				Subturn = t + 1;
			}
		}

		public void DrawFrame(Bitmap b, int frame)
		{
			using (Graphics g = Graphics.FromImage(b))
				DrawFrame(g, b.Width, b.Height, frame);
		}

		public void DrawFrame(Graphics g, int width, int height, int frame)
		{
			g.FillRectangle(Brushes.White, 0, 0, width, height);
			float wK = (float)width / InitBD.MapWidth;
			float hK = (float)height / InitBD.MapHeight;

			for (float x = 0; x < width; x += wK)
				g.DrawLine(Pens.LightGray, x, 0, x, height);
			for (float y = 0; y < height; y += hK)
				g.DrawLine(Pens.LightGray, 0, y, width, y);


			for (int a = 0; a < 2; a++)
			{
				bool sideA = (a % 2) == 0;

				var Army = AllyTurn() == sideA ? BD.EnemyArmy : BD.AllyArmy;
				for (int i = 0; i < Army.Length; i++)
				{
					var Action = sideA ? new Action() : Timeline[Turn][i, SecondAction ? 1 : 0];
					var Position = Army[i].Position;

					switch (Action.Type)
					{
						case ActionType.Attack:
							g.FillRectangle(AllyTurn() == sideA ? Brushes.LightBlue : Brushes.Violet, Position.X * wK, Position.Y * hK, 10, 10);
							g.DrawString(Action.Damage.ToString(), new Font("Arial", 15), Brushes.Red, Action.Target.X * wK, Action.Target.Y * hK - (hK * frame / SubturnFramerate));
							g.DrawString(Army[i].Amount.ToString(), new Font("Arial", 10), Brushes.Green, Position.X * wK - wK / 3, Position.Y * hK - hK / 3);
							break;
						default:
							if (Action.Path != null && Action.Path.Length >= 2)
							{
								float framecost = SubturnFramerate / (Action.Path.Length - 1);
								int pp = (int)(frame / framecost);
								int start = Action.Path.Length - 1 - pp;
								float X = Action.Path[start].X + (Action.Path[start - 1].X - Action.Path[start].X) * ((frame % framecost) / framecost);
								float Y = Action.Path[start].Y + (Action.Path[start - 1].Y - Action.Path[start].Y) * ((frame % framecost) / framecost);
								g.FillRectangle(AllyTurn() == sideA ? Brushes.LightBlue : Brushes.Violet, X * wK, Y * hK, 10, 10);
								g.DrawString(Army[i].Amount.ToString(), new Font("Arial", 10), Brushes.Green, X * wK - wK / 3, Y * hK - hK / 3);
							}
							else
							{
								if (Army[i].Alive)
								{
									g.FillRectangle(AllyTurn() == sideA ? Brushes.LightBlue : Brushes.Violet, Position.X * wK,
										(Position.Y * hK), 10, 10);
									g.DrawString(Army[i].Amount.ToString(), new Font("Arial", 10), Brushes.Green, Position.X * wK - wK / 3, Position.Y * hK - hK / 3);
								}
								else g.FillRectangle(AllyTurn() == sideA ? Brushes.LightGray : Brushes.LightGray, Position.X * wK,
									 (Position.Y * hK), 10, 10);
							}
							break;
					}
				}

			}


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
		}
	}
}
