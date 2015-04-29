using project.Controls;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Effects;
using Teske.WPF.Effects;
namespace project
{
    public partial class MainWindow
    {

        private static Sprite SpriteSoldier = InitSoldier();
        private static Sprite SpriteHorse = InitHorse();
        private static Sprite SpriteArcher = InitArcher();
               
        private static  Unit humanKnights = new Unit(4, 17, 3, 5, 7, 25, 1.5f) { SideASprite = SpriteHorse, SideBSprite = SpriteHorse };

        private static Unit humanSoliders = new Unit(4, 16, 2, 4, 4, 30, 1.5f) { SideASprite = SpriteSoldier, SideBSprite = SpriteSoldier };
        private static Unit humanArcher = new Unit(4, 12, 5, 4, 3, 20, 10f) { SideASprite = SpriteArcher, SideBSprite = SpriteArcher };

           private static Sprite InitHorse(){ 
			#region Horse
            RectangleF[][][] Animations1 = new RectangleF[Enum.GetValues(typeof(Sprite.AnimationAction)).Length][][];
            for (int i = 0; i < Enum.GetValues(typeof(Sprite.AnimationAction)).Length; i++)
            {
                switch ((Sprite.AnimationAction)i)
                {
                    case Sprite.AnimationAction.Standing:
                        int lengthS = 23;

                        Animations1[i] = new RectangleF[1][];
                        Animations1[i][0] = new RectangleF[lengthS];
                        for (int j = 0; j < lengthS; j++)
                        {
                            Animations1[i][0][j] = new RectangleF(72 * j, 224, 72, 88);
                        }
                        break;
                    case Sprite.AnimationAction.Moving:
                        int lengthM = 2;

                        Animations1[i] = new RectangleF[1][];
                        Animations1[i][0] = new RectangleF[]{  
                            //new RectangleF(0, 320, 72, 88),
                            new RectangleF(100, 320, 72, 88),
                            new RectangleF(200, 320, 72, 88),
                            new RectangleF(310, 320, 72, 88)
                        };

                        break;
                    case Sprite.AnimationAction.Attacking:

                        int lengthA = 28;

                        Animations1[i] = new RectangleF[1][];
                        Animations1[i][0] = new RectangleF[lengthA];
                        for (int j = 0; j < lengthA; j++)
                        {
                            Animations1[i][0][j] = new RectangleF(72 * j, 96, 72, 120);
                        }

                        break;
                    case Sprite.AnimationAction.TakingDamage:
                        int lengthT = 13;

                        Animations1[i] = new RectangleF[1][];
                        Animations1[i][0] = new RectangleF[lengthT];
                        for (int j = 0; j < lengthT; j++)
                        {
                            Animations1[i][0][j] = new RectangleF(72 * j, 0, 72, 88);
                        }

                        break;
                    case Sprite.AnimationAction.Dying:
                        Animations1[i] = Animations1[(int)Sprite.AnimationAction.TakingDamage];
                        break;
                }

            }
			#endregion
			return new Sprite(global::project.Properties.Resources.MyHorseman1, Animations1, false);
           }



           private static Sprite InitSoldier(){ 
			#region Soldier
            RectangleF[][][] Animations2 = new RectangleF[Enum.GetValues(typeof(Sprite.AnimationAction)).Length][][];
            for (int i = 0; i < Enum.GetValues(typeof(Sprite.AnimationAction)).Length; i++)
            {
                switch ((Sprite.AnimationAction)i)
                {
                    case Sprite.AnimationAction.Standing:
                        int lengthS = 23;

                        Animations2[i] = new RectangleF[1][];
                        Animations2[i][0] = new RectangleF[lengthS];
                        for (int j = 0; j < lengthS; j++)
                        {
                            Animations2[i][0][j] = new RectangleF(72 * j, 88, 72, 80);
                        }
                        break;
                    case Sprite.AnimationAction.Moving:
                        int lengthM = 10;

                        Animations2[i] = new RectangleF[1][];
                        Animations2[i][0] = new RectangleF[lengthM];
                        for (int j = 0; j < lengthM; j++)
                        {
                            Animations2[i][0][j] = new RectangleF(96 * (j + 3), 176, 72, 80);

                        }
                        break;
                    case Sprite.AnimationAction.Attacking:

                        int lengthA = 21;

                        Animations2[i] = new RectangleF[1][];
                        Animations2[i][0] = new RectangleF[lengthA];
                        for (int j = 0; j < lengthA; j++)
                        {
                            Animations2[i][0][j] = new RectangleF(96 * j, 264, 72, 80);
                        }

                        break;
                    case Sprite.AnimationAction.TakingDamage:
                        int lengthT = 13;

                        Animations2[i] = new RectangleF[1][];
                        Animations2[i][0] = new RectangleF[lengthT];
                        for (int j = 0; j < lengthT; j++)
                        {
                            Animations2[i][0][j] = new RectangleF(72 * j, 0, 72, 80);
                        }

                        break;
                    case Sprite.AnimationAction.Dying:
                        Animations2[i] = Animations2[(int)Sprite.AnimationAction.TakingDamage];
                        break;
                }

            }
			#endregion
            return new Sprite(global::project.Properties.Resources.fighter_transparent_2, Animations2, true);
           }

           private static Sprite InitArcher(){ 
			#region Archer
            RectangleF[][][] Animations3 = new RectangleF[Enum.GetValues(typeof(Sprite.AnimationAction)).Length][][];
            for (int i = 0; i < Enum.GetValues(typeof(Sprite.AnimationAction)).Length; i++)
            {
                switch ((Sprite.AnimationAction)i)
                {
                    case Sprite.AnimationAction.Standing:
                        int lengthS = 23;

                        Animations3[i] = new RectangleF[1][];
                        Animations3[i][0] = new RectangleF[lengthS];
                        for (int j = 0; j < lengthS; j++)
                        {
                            Animations3[i][0][j] = new RectangleF(56 * j, 104, 56, 80);
                        }
                        break;
                    case Sprite.AnimationAction.Moving:
                        int lengthM = 12;
                        Animations3[i] = new RectangleF[1][];
						Animations3[i][0] = new RectangleF[lengthM - 1];
                        for (int j = 1; j < lengthM; j++)
                        {
							Animations3[i][0][j - 1] = new RectangleF(8 + 80 * j, 184, 72, 80);
                        }
                        break;
                    case Sprite.AnimationAction.Attacking:

                        int lengthA = 38;

                        Animations3[i] = new RectangleF[1][];
                        Animations3[i][0] = new RectangleF[lengthA];
                        for (int j = 0; j < lengthA; j++)
                        {
                            Animations3[i][0][j] = new RectangleF(232 + (j % 7) * 288, 280 + (j / 7) * 88, 56, 80);
                        }

                        break;
                    case Sprite.AnimationAction.TakingDamage:
                        int lengthT = 13;

                        Animations3[i] = new RectangleF[1][];
                        Animations3[i][0] = new RectangleF[lengthT];
                        for (int j = 0; j < lengthT; j++)
                        {
                            Animations3[i][0][j] = new RectangleF(64 * j, 24, 64, 80);
                        }

                        break;
                    case Sprite.AnimationAction.Dying:
                        Animations3[i] = Animations3[(int)Sprite.AnimationAction.TakingDamage];
                        break;
                }

            }
			#endregion
			return new Sprite(global::project.Properties.Resources.Archer, Animations3, true);
           }
            //#region Phoenix
            //RectangleF[][][] Animations4 = new RectangleF[Enum.GetValues(typeof(Sprite.AnimationAction)).Length][][];
            //for (int i = 0; i < Enum.GetValues(typeof(Sprite.AnimationAction)).Length; i++)
            //{
            //    switch ((Sprite.AnimationAction)i)
            //    {
            //        case Sprite.AnimationAction.Standing:

            //            Animations4[i] = new RectangleF[1][];
            //            Animations4[i][0] = new RectangleF[]{
            //                new RectangleF(0,0,83,108),
            //                new RectangleF(220,0,83,108),
            //                new RectangleF(421,0,83,108),
            //                new RectangleF(622,0,83,108),
            //                new RectangleF(823,0,83,108),
            //                new RectangleF(1024,0,83,108)};

            //            break;
            //        case Sprite.AnimationAction.Moving:
            //            Animations4[i] = new RectangleF[1][];
            //            Animations4[i][0] = new RectangleF[]{
            //                new RectangleF(6,122,124,155),
            //                new RectangleF(200,122,124,155),
            //                new RectangleF(395,122,124,155),
            //                new RectangleF(602,122,124,155),
            //                new RectangleF(805,122,124,155),
            //                new RectangleF(1003,122,124,155),
            //                new RectangleF(1201,122,124,155),
            //                new RectangleF(1402,122,124,155),
            //                new RectangleF(395,122,124,155),
            //                new RectangleF(602,122,124,155),
            //                new RectangleF(805,122,124,155),
            //                new RectangleF(1003,122,124,155),
            //                new RectangleF(1201,122,124,155),
            //                new RectangleF(1402,122,124,155),
            //                new RectangleF(395,122,124,155),
            //                new RectangleF(602,122,124,155),
            //                new RectangleF(805,122,124,155),
            //                new RectangleF(1003,122,124,155),
            //                new RectangleF(1201,122,124,155),
            //                new RectangleF(1402,122,124,155),
            //                new RectangleF(1611,122,124,155)};

            //            break;
            //        case Sprite.AnimationAction.Attacking:

            //            Animations4[i] = new RectangleF[3][];
            //            Animations4[i][2] = new RectangleF[]{
            //                new RectangleF(235,626,120,150),
            //                new RectangleF(22,627,168,150),
            //                new RectangleF(11,312,150,150),
            //                new RectangleF(213,312,150,150),
            //                new RectangleF(415,312,150,150),
            //                new RectangleF(213,312,150,150),
            //                new RectangleF(11,312,150,150),
            //                new RectangleF(22,627,168,150),
            //                new RectangleF(235,626,120,150)};

            //            Animations4[i][1] = new RectangleF[]{
            //                new RectangleF(235,626,120,150),
            //                new RectangleF(16,458,168,150),
            //                new RectangleF(211,458,168,150),
            //                new RectangleF(406,458,168,150),
            //                new RectangleF(22,627,168,150),
            //                new RectangleF(235,626,120,150)};

            //            Animations4[i][0] = new RectangleF[]{
            //                new RectangleF(235,626,120,150),
            //                new RectangleF(420,625,120,150),
            //                new RectangleF(222,757,168,184),
            //                new RectangleF(15,757,168,184),
            //                new RectangleF(222,757,168,184),
            //                new RectangleF(427,757,168,184)};

            //            break;
            //        case Sprite.AnimationAction.TakingDamage:

            //            Animations4[i] = new RectangleF[1][];
            //            Animations4[i][0] = new RectangleF[]{
            //                new RectangleF(15,990,85,123),
            //                new RectangleF(216,990,85,123),
            //                new RectangleF(413,990,85,123),
            //                new RectangleF(216,990,85,123),
            //                new RectangleF(15,990,85,123)};

            //            break;
            //        case Sprite.AnimationAction.Dying:
            //            Animations4[i] = new RectangleF[1][];
            //            Animations4[i][0] = new RectangleF[]{
            //                new RectangleF(15,990,85,123),
            //                new RectangleF(216,990,85,123),
            //                new RectangleF(413,990,85,123),
            //                new RectangleF(618,990,85,123),
            //                new RectangleF(819,990,85,123),
            //                new RectangleF(1020,990,85,123),
            //                new RectangleF(1221,990,85,123),
            //                new RectangleF(1422,990,85,123),
            //                new RectangleF(15,1190,85,123),
            //                new RectangleF(216,1190,85,123),
            //                new RectangleF(417,1190,85,123),
            //                new RectangleF(618,1190,85,123),
            //                new RectangleF(819,1190,85,123),
            //                new RectangleF(1020,1190,85,123)};

            //            break;
            //    }

            //}
            //#endregion
            //var gSpritePhoenix = new Sprite(global::project.Properties.Resources._41843, Animations4, false);

    }
}
