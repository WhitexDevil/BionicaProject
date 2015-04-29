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

	public class Visualization
	{
		Squad[] Squads;
		BattleData InitBD;
		BattleData BD;
		List<List<Action>> Turns = new List<List<Action>>();
		List<Action> Timeline = new List<Action>();

		public List<string> BattleLog { get; set; }

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
			BattleLog = new List<string>();
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


		private int ToArmyIndex(int index)
		{
			return index >= BD.AllyArmy.Length ? index - BD.AllyArmy.Length : index;
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
				lock (BattleLog) BattleLog.Clear();
			}
			for (int t = Subturn; t < Math.Min(subturn, Timeline.Count - 1); t++)
			{
				//var Ally = SideA ? BD.AllyArmy : BD.EnemyArmy;
				//var Enemy = Ally == BD.EnemyArmy ? BD.AllyArmy : BD.EnemyArmy;

				switch (Timeline[t].Type)
				{
					case ActionType.Attack:
						Squads[Timeline[t].Target].Amount -= Timeline[t].Damage;
						lock (BattleLog) BattleLog.Add(String.Format("Squad #{0} attacks squad {1} and kill {2} units\r\n",
							ToArmyIndex(Timeline[t].Squad), ToArmyIndex(Timeline[t].Target), Timeline[t].Damage));
						if (!Squads[Timeline[t].Target].Alive)
							lock (BattleLog) BattleLog.Add(String.Format("Squad #{0} DEFEATED\r\n", ToArmyIndex(Timeline[t].Target)));
						break;
					case ActionType.Move:
						Squads[Timeline[t].Squad].Position = Timeline[t].Path.First();
						lock (BattleLog) BattleLog.Add(String.Format("Squad #{0} moving to {1}:{2}\r\n",
							ToArmyIndex(Timeline[t].Squad), Timeline[t].Path.First().X, Timeline[t].Path.First().X));
						break;
				}

				Subturn = t + 1;
				SubturnRelative++;
				if (SubturnRelative >= Turns[Turn].Count)
				{
					SubturnRelative = 0;
					Turn++;
					lock (BattleLog) BattleLog.Add(String.Format("Turn {0} finished, now turn of Side" + (SideA ? "A" : "B") + "\r\n", Turn - 1));
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
			//g.Clear(Color.White);
			for (int x = 0; x < Math.Ceiling((float)width / Sprite.Background.Width); x++)
				for (int y = 0; y < Math.Ceiling((float)height / Sprite.Background.Height); y++)
					g.DrawImageUnscaled(Sprite.Background, new Point(x * Sprite.Background.Width, y * Sprite.Background.Height));

			float wK = (float)width / InitBD.MapWidth;
			float hK = (float)height / (1+InitBD.MapHeight);

			for (float x = 0; x < width; x += wK)
				g.DrawLine(Pens.DarkGreen, x, 0, x, height);
			for (float y = 0; y < height; y += hK)
				g.DrawLine(Pens.DarkGreen, 0, y, width, y);

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
						DirectionDegree(Squad.Position, Squads[Action.Squad].Position), Squad.Amount, Action.Damage, frame);
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
