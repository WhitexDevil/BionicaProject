using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace project
{
	using Step = KeyValuePair<Point, float>;
	class Visualization
	{
		BattleData InitBD;
		BattleData BD;
		List<Action[,]> Timeline = new List<Action[,]>();
		int Turn { get { return Subturn / 2; } }
		bool SecondAction { get { return Subturn % 2 == 0; } }
		int Subturn = 0;
		float SubturnFramerate = 60;

		enum ActionType { None, Attack, Move }

		struct Action
		{
			public Point a;
			public Point Target;
			public Point[] Path;
			public int Damage;
			public ActionType Type;
		}

		int FindIndex(Squad s)
		{
			for (int i = 0; i < InitBD.AllyArmy.Length; i++)
				if (InitBD.AllyArmy[i] == s) return i;
			for (int i = 0; i < InitBD.EnemyArmy.Length; i++)
				if (InitBD.EnemyArmy[i] == s) return i;
			return -1;
		}
		public void RecordMove(Squad s, Point a, Point b, Step[] path)
		{

			Timeline.Last()[FindIndex(s),
				Timeline.Last()[FindIndex(s), 0].Type == ActionType.None ? 0 : 1] = new Action()
			{
				Type = ActionType.Move,
				a = a,
				Target = b,
				Path = path.Select(x => x.Key).SkipWhile(x => x != a).TakeWhile(x => x != b).ToArray(),
				Damage = 0
			};
		}
		public void RecordAttack(Squad s, Point a, Point b, int damage)
		{
			Timeline.Last()[FindIndex(s),
				Timeline.Last()[FindIndex(s), 0].Type == ActionType.None ? 0 : 1] = new Action()
			{
				Type = ActionType.Attack,
				a = a,
				Target = b,
				Path = null,
				Damage = damage
			};
		}
		public void RecordEndOfTurn()
		{
			Timeline.Add(new Action[Timeline.Count % 2 == 0 ? InitBD.AllyArmy.Length : InitBD.EnemyArmy.Length, 2]);
		}

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

		public void InitZeroState(BattleData battleData)
		{
			battleData = (BattleData)battleData.Clone();
			RecordEndOfTurn();
			for (int i = 0; i < this.InitBD.AllyArmy.Length; i++)
			{
				this.InitBD.Map[this.InitBD.AllyArmy[i].Position.X +
					(this.InitBD.AllyArmy[i].Position.Y << this.InitBD.MapHeightLog2)] = (byte)(i + 1);
			}
			for (int i = 0; i < this.InitBD.EnemyArmy.Length; i++)
			{
				this.InitBD.Map[this.InitBD.EnemyArmy[i].Position.X +
					(this.InitBD.EnemyArmy[i].Position.Y << this.InitBD.MapHeightLog2)] =
					(byte)(i + 1 + this.InitBD.AllyArmy.Length);
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
			if (subturn < Subturn)
			{
				BD = (BattleData)InitBD.Clone();
				Subturn = 0;
			}
			for (int t = Subturn; t < Math.Max(subturn, Timeline.Count * 2); t++)
			{
				var Ally = ((t / 2) % 2) == 0 ? BD.AllyArmy : BD.EnemyArmy;
				var Enemy = Ally == BD.EnemyArmy ? BD.AllyArmy : BD.EnemyArmy;
				for (int i = 0; i < Ally.Length; i++)
				{
					var Action = Timeline[i / 2][i, t % 2];
					switch (Action.Type)
					{
						case ActionType.Attack:
							for (int i2 = 0; i2 < Enemy.Length; i2++)
								if (Enemy[i2].Position == Action.Target)
									Enemy[i2].TakeDamage(Action.Damage);
							break;
						case ActionType.Move:
							Ally[i].Position = Action.Path.Last();
							break;
					}
				}
				Subturn = t + 1;
			}
		}

		public void DrawFrame(Graphics g, int width, int height, int frame)
		{
			g.FillRectangle(Brushes.White, 0, 0, width, height);
			float wK = width / InitBD.MapWidth;
			float hK = height / InitBD.MapHeight;

			for (float x = 0; x < width; x += wK)
				g.DrawLine(Pens.Gray, x, 0, x, height);
			for (float y = 0; y < height; y += hK)
				g.DrawLine(Pens.Gray, 0, y, InitBD.MapWidth, y);


			for (int a = 0; a < 2; a++)
			{
				bool sideA = (a + (Turn % 2) % 2) == 0;

				var Army = sideA ? BD.AllyArmy : BD.EnemyArmy;
				for (int i = 0; i < Army.Length; i++)
				{
					var Action = AllyTurn() == sideA ? Timeline[Turn][i, SecondAction ? 1 : 0] : new Action();
					var Position = Army[i].Position;
					if (sideA)
					{
						switch (Action.Type)
						{
							case ActionType.Attack:
								g.DrawRectangle(sideA ? Pens.Blue : Pens.Violet, Position.X * wK, Position.Y * hK, 10, 10);
								g.DrawString(Action.Damage.ToString(), new Font("Arial", 15), Brushes.Red, Action.Target.X * wK, Action.Target.Y * hK);
								break;
							case ActionType.Move:
								float framecost = SubturnFramerate / (Action.Path.Length - 1);
								int pp = (int)(frame / framecost);
								float X = Action.Path[pp].X + (Action.Path[pp + 1].X - Action.Path[pp].X) * ((frame % framecost) / framecost);
								float Y = Action.Path[pp].Y + (Action.Path[pp + 1].Y - Action.Path[pp].Y) * ((frame % framecost) / framecost);
								g.DrawRectangle(sideA ? Pens.Blue : Pens.Violet, X * wK, Y * hK, 10, 10);
								break;
						}
					}
					if (Army[i].Alive)
					{
						g.DrawRectangle(sideA ? Pens.Blue : Pens.Violet, Position.X * wK,
							(Position.Y * hK) + (hK * frame / SubturnFramerate), 10, 10);
						g.DrawString(Army[i].Amount.ToString(), new Font("Arial", 15), Brushes.Green, Position.X * wK - wK / 3, Position.Y * hK - hK / 3);
					}
					else g.DrawRectangle(sideA ? Pens.DarkBlue : Pens.DarkViolet, Position.X * wK,
						 (Position.Y * hK) + (hK * frame / SubturnFramerate), 10, 10);
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
