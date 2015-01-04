/*
██████╗ ██╗   ██╗     █████╗ ██╗     ██████╗ ██╗  ██╗ █████╗  ██████╗  ██████╗ ██████╗ 
██╔══██╗╚██╗ ██╔╝    ██╔══██╗██║     ██╔══██╗██║  ██║██╔══██╗██╔════╝ ██╔═══██╗██╔══██╗
██████╔╝ ╚████╔╝     ███████║██║     ██████╔╝███████║███████║██║  ███╗██║   ██║██║  ██║
██╔══██╗  ╚██╔╝      ██╔══██║██║     ██╔═══╝ ██╔══██║██╔══██║██║   ██║██║   ██║██║  ██║
██████╔╝   ██║       ██║  ██║███████╗██║     ██║  ██║██║  ██║╚██████╔╝╚██████╔╝██████╔╝
╚═════╝    ╚═╝       ╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═════╝ 
*/                                                                                       
using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace SharedExperience
{
    class Program
    {
        static Menu menu;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        public static void OnGameLoad(EventArgs args)
        {
            LoadMenu();
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.PrintChat("<font color=\"#00BFFF\">Shared Experience</font> <font color=\"#FFFFFF\"> - Loaded! v1.0.0.4</font>");
        }

        public static float[] Exp = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] SharingCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] VisibleCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] InvisibleCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] TimeSharingChange = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] TimeNewShareFound = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] TimeUpdateVisible = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] TimeMissing = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static Vector3[] LastMinionPosition = { Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero };
        public static float RangedMinonExp = 29.44f;
        public static float MeleeMiniobExp = 58.88f;
        public static float SiegeMinionExp = 92.00f;
        public static Color[] Cor = { Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White};
        public static int Zilean = 0;

        public static void OnGameUpdate(EventArgs args)
        {
            int i = -1;
            float expReceived = 0;

            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                i += 1;

                if (!hero.IsVisible)
                {
                    Exp[i] = (hero.Experience);
                    SharingCount[i] = 0;
                    InvisibleCount[i] = 0;
                    VisibleCount[i] = 0;
                    TimeNewShareFound[i] = 0;
                    continue;
                }

                if (hero.IsDead || hero.IsMe || (hero.Level == 18) || hero.IsAlly || hero.IsInvulnerable) continue;

                if (hero.IsEnemy && !menu.Item("showEnemies").GetValue<bool>()) continue;

                TimeMissing[i] = Environment.TickCount;



                foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (hero.IsEnemy && !minion.IsEnemy && Vector3.Distance(hero.Position, minion.Position) <= 1400)
                    {
                        if (minion.IsDead)
                        {
                            LastMinionPosition[i] = minion.Position;
                            continue;
                        }
                        //string MinionName = minion.BaseSkinName;
                        //Console.WriteLine(MinionName);
                    }
                }
                //Console.WriteLine("Minions next to " + (hero.ChampionName) + " -- " + aliveMeleeMinionCount + " Melee -- " + aliveRangedMinionCount + " Ranged -- " + aliveSiegeMinionCount + " Siege");

                if (Exp[i] != hero.Experience)
                {
                    int rangedMinion = 0;   // melee == 2*ranged
                    int siegeMinion = 0;

                    expReceived = (float)Math.Round(hero.Experience - Exp[i], 2);

                    //Console.WriteLine((i) + " " + (hero.ChampionName) + "  Last exp received: " + (expReceived) + " exp ");
                    //Console.WriteLine("Got Experience from -- " + killedRangedMinionCount + " Ranged -- " + killedMeleeMinionCount + " Melee -- " + killedSiegeMinionCount + " Siege");

                    int found = 0;

                    for (int expSharingCount = 1; expSharingCount <= 5; expSharingCount++)
                    {
                        if (expSharingCount == 1)
                        {
                            for (float increasedExp = 1.00f; increasedExp <= 1.14f; increasedExp += 0.02f)
                            {
                                for (rangedMinion = 0; rangedMinion <= 27; rangedMinion += 1)
                                {
                                    if (expReceived == (Math.Round((RangedMinonExp * rangedMinion * increasedExp), 2)))
                                    {
                                        if (SharingCount[i] <= expSharingCount) TimeNewShareFound[i] = Environment.TickCount; 
                                        SharingCount[i] = expSharingCount;
                                        TimeSharingChange[i] = Environment.TickCount;
                                        TimeUpdateVisible[i] = 0;
                                        found = 1;
                                        break;
                                    }
                                    for (siegeMinion = 1; siegeMinion <= 3; siegeMinion += 1)
                                    {
                                        if (expReceived == (Math.Round((RangedMinonExp * rangedMinion * increasedExp), 2) + Math.Round((SiegeMinionExp * siegeMinion * increasedExp), 2)))
                                        {
                                            if (SharingCount[i] < expSharingCount) TimeNewShareFound[i] = Environment.TickCount; 
                                            SharingCount[i] = expSharingCount;
                                            TimeSharingChange[i] = Environment.TickCount;
                                            TimeUpdateVisible[i] = 0;
                                            found = 1;
                                            break;
                                        }
                                    }
                                    if (found == 1) break;
                                }
                                if (found == 1) break;
                            }
                        }
                        else
                        {
                            for (float increasedExp = 1.00f; increasedExp <= 1.14f; increasedExp += 0.02f)
                            {
                                for (rangedMinion = 0; rangedMinion <= 27; rangedMinion += 1)
                                {
                                    if (expReceived == (Math.Round(((RangedMinonExp * rangedMinion * 1.30435f / expSharingCount) * increasedExp), 2)))
                                    {
                                        if (SharingCount[i] < expSharingCount) TimeNewShareFound[i] = Environment.TickCount; 
                                        SharingCount[i] = expSharingCount;
                                        TimeSharingChange[i] = Environment.TickCount;
                                        TimeUpdateVisible[i] = 0;
                                        found = 1;
                                        break;
                                    }
                                    for (siegeMinion = 1; siegeMinion <= 3; siegeMinion += 1)
                                    {
                                        if (expReceived == (Math.Round(((RangedMinonExp * rangedMinion * 1.30435f / expSharingCount) * increasedExp), 2) + Math.Round(((SiegeMinionExp * siegeMinion * 1.30435f / expSharingCount) * increasedExp), 2)))
                                        {
                                            if (SharingCount[i] < expSharingCount) TimeNewShareFound[i] = Environment.TickCount; 
                                            SharingCount[i] = expSharingCount;
                                            TimeSharingChange[i] = Environment.TickCount;
                                            TimeUpdateVisible[i] = 0;
                                            found = 1;
                                            break;
                                        }
                                    }
                                    if (found == 1) break;
                                }
                                if (found == 1) break;
                            }
                        }
                        if (found == 1)
                        {
                            //Console.WriteLine("Shared Experience between " + expSharingCount + " champions ");
                            break;
                        }
                    }
                    Exp[i] = (hero.Experience);
                }

                if (hero.IsEnemy && Environment.TickCount - TimeUpdateVisible[i] >= 100)
                {
                    int deadCount = 0;
                    VisibleCount[i] = 0;
                    foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>())
                    {
                        if (enemy.IsEnemy && Vector3.Distance(hero.Position, enemy.Position) <= (3000))
                        {
                            if (enemy.IsVisible) VisibleCount[i] += 1;
                            if (enemy.IsDead)
                            {
                                VisibleCount[i] -= 1;
                                deadCount += 1;
                            }
                        }
                    }

                    //Console.WriteLine((i) + " " + (hero.ChampionName) + "  deadCount: " + deadCount + " VisibleCount[i] -- " + VisibleCount[i] + " SharingCount[i] -- " + SharingCount[i] + " InvisibleCount[i] -- " + InvisibleCount[i]);

                    if (SharingCount[i] < VisibleCount[i])
                    {
                        SharingCount[i] = VisibleCount[i];
                        InvisibleCount[i] = 0;
                        TimeSharingChange[i] = Environment.TickCount;
                    }
                    else
                    {
                        InvisibleCount[i] = (SharingCount[i] - VisibleCount[i] - deadCount);

                        if (deadCount > 0 && InvisibleCount[i] == 0)
                        {
                            SharingCount[i] = VisibleCount[i];
                            TimeSharingChange[i] = Environment.TickCount;
                        }

                        if (InvisibleCount[i] < 0) InvisibleCount[i] = 0;

                    }

                    TimeUpdateVisible[i] = Environment.TickCount;

                    if (InvisibleCount[i] > SharingCount[i] - 1 && SharingCount[i] > 0) InvisibleCount[i] = SharingCount[i] - 1;

                    if ((Environment.TickCount - TimeSharingChange[i]) >= 0)
                    {
                        Cor[i] = Color.White;
                    }

                    if ((Environment.TickCount - TimeSharingChange[i]) >= 20000)
                    {
                        SharingCount[i] = VisibleCount[i];
                        TimeSharingChange[i] = Environment.TickCount;
                    }
                }


                if (InvisibleCount[i] > 0)
                {
                    Cor[i] = menu.Item("invColor").GetValue<Color>();
                    //Console.WriteLine(Cor[i]);
                }
            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            int i = -1;

            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                i += 1;

                if (!hero.IsVisible || hero.IsDead || hero.IsMe || (hero.Level == 18) || hero.IsAlly || hero.IsInvulnerable) continue;

                if (hero.IsEnemy && !menu.Item("showEnemies").GetValue<bool>()) continue;

                int textXOffset = menu.Item("positionX").GetValue<Slider>().Value;
                int textYOffset = menu.Item("positionY").GetValue<Slider>().Value;

                if (hero.IsAlly)
                {
                    textYOffset -= 2;
                }

                if ((Environment.TickCount - TimeNewShareFound[i]) < 15000 && InvisibleCount[i] > 0 && menu.Item("drawPredictionCircle").GetValue<bool>())
                {
                    Drawing.DrawCircle(LastMinionPosition[i], 1500, Color.Red);
                }

                if (SharingCount[i] > 0)
                {
                    if (InvisibleCount[i] > 0)
                    {
                        Drawing.DrawText(hero.HPBarPosition.X + textXOffset, hero.HPBarPosition.Y + textYOffset, Cor[i], "+" + (SharingCount[i] - 1) + " (" + InvisibleCount[i] + " Inv)");
                    }
                    if (!menu.Item("onlyShowInv").GetValue<bool>() && InvisibleCount[i] == 0)
                    {
                        Drawing.DrawText(hero.HPBarPosition.X + textXOffset, hero.HPBarPosition.Y + textYOffset, Cor[i], "+" + (SharingCount[i] - 1));
                    }
                }
            }
        }

        static void LoadMenu()
        {
            menu = new Menu("Shared Experience", "Shared Experience", true);
            menu.AddItem(new MenuItem("showEnemies", "Show Enemies Exp Share Count").SetValue(true));
            menu.AddItem(new MenuItem("onlyShowInv", "Only Show Text When Not Visible Enemies").SetValue(false));
            menu.AddItem(new MenuItem("drawPredictionCircle", "Draw Prediction Circle For Not Visible Enemies").SetValue(true));
            menu.AddItem(new MenuItem("invColor", "Text Color When Not Visible Enemies").SetValue(Color.FromArgb(255, 245,25,25)));
            menu.AddItem(new MenuItem("positionX", "Text Position X").SetValue(new Slider(142, -100, 200)));
            menu.AddItem(new MenuItem("positionY", "Text Position Y").SetValue(new Slider(21, -100, 100)));
            menu.AddToMainMenu();
        }
    }
}
