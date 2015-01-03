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
            Game.PrintChat("<font color=\"#00BFFF\">Shared Experience</font> <font color=\"#FFFFFF\"> - Loaded! v1.0.0.3</font>");
        }

        public static float[] Exp = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] SharingCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] VisibleCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] InvisibleCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] TimeExperienceIncrease = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] TimeUpdateVisible = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] TimeMissing = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] LastMeleeMinionCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] LastRangedMinionCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] LastSiegeMinionCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static float RangedExp = 29.44f;
        public static float MeleeExp = 58.88f;
        public static float SiegeExp = 92.00f;
        public static Color[] Cor = { Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White};
        public static int Zilean = 0;
        public static string MinionName;

        public static void OnGameUpdate(EventArgs args)
        {
            int i = -1;
            float expReceived = 0;

            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                i += 1;

                if (!hero.IsVisible || hero.IsDead || hero.IsMe || (hero.Level == 18) || hero.IsAlly || hero.IsInvulnerable) continue;

                if (hero.IsEnemy && !menu.Item("showEnemies").GetValue<bool>()) continue;

                if (Environment.TickCount - TimeMissing[i] >= 5000)
                {
                    Exp[i] = (hero.Experience);
                    SharingCount[i] = 0;
                }

                TimeMissing[i] = Environment.TickCount;

                int meleeMinionCount = 0;
                int rangedMinionCount = 0;
                int siegeMinionCount = 0;

                foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (hero.IsEnemy && !minion.IsDead && !minion.IsEnemy && Vector3.Distance(hero.Position, minion.Position) <= 1400)
                    {
                        MinionName = minion.BaseSkinName;
                        //Console.WriteLine(minionName);
                        if (MinionName.Contains("Melee"))
                        {
                            meleeMinionCount++;
                        }
                        else if (MinionName.Contains("Ranged"))
                        {
                            rangedMinionCount++;
                        }
                        else if (MinionName.Contains("Siege") || MinionName.Contains("Super"))
                        {
                            siegeMinionCount++;
                        }
                    }
                }
                //Console.WriteLine("Minions next to " + (hero.ChampionName) + " -- " + aliveMeleeMinionCount + " Melee -- " + aliveRangedMinionCount + " Ranged -- " + aliveSiegeMinionCount + " Siege");

                if (Exp[i] != hero.Experience)
                {
                    int zRangedMinionCount = (LastRangedMinionCount[i] - rangedMinionCount);
                    int zMeleeMinionCount = (LastMeleeMinionCount[i] - meleeMinionCount);
                    int zSiegeMinionCount = (LastSiegeMinionCount[i] - siegeMinionCount);

                    LastRangedMinionCount[i] = rangedMinionCount;
                    LastMeleeMinionCount[i] = meleeMinionCount;
                    LastSiegeMinionCount[i] = siegeMinionCount;

                    expReceived = (float)Math.Round(hero.Experience - Exp[i], 2);

                    //Console.WriteLine((i) + " " + (hero.ChampionName) + "  Last exp received: " + (expReceived) + " exp ");
                    //Console.WriteLine("Got Experience from -- " + zMeleeMinionCount + " Melee -- " + zRangedMinionCount + " Ranged -- " + zSiegeMinionCount + " Siege");

                    for (int c = 1; c <= 5; c++)
                    {
                        if (c == 1)
                        {
                            for (float z = 1.00f; z <= 1.14f; z += 0.02f)
                            {
                                if (expReceived == ((RangedExp * zRangedMinionCount * z) + (MeleeExp * zMeleeMinionCount * z) + (SiegeExp * zSiegeMinionCount * z)))
                                {
                                    VisibleCount[i] = 1;
                                    SharingCount[i] = 1;
                                    TimeExperienceIncrease[i] = Environment.TickCount;
                                    TimeUpdateVisible[i] = Environment.TickCount;
                                }
                            }
                        }
                        else
                        {
                            for (float z = 1.00f; z <= 1.14f; z += 0.02f)
                            {
                                if (expReceived == (Math.Round(((RangedExp * zRangedMinionCount * 1.30435f / c) * z), 2) + Math.Round(((MeleeExp * zMeleeMinionCount * 1.30435f / c) * z), 2) + Math.Round(((SiegeExp * zSiegeMinionCount * 1.30435f / c) * z), 2)))
                                {
                                    SharingCount[i] = c;
                                    TimeExperienceIncrease[i] = Environment.TickCount;
                                    TimeUpdateVisible[i] = 0;
                                }
                            }
                        }
                    }
                    Exp[i] = (hero.Experience);
                }

                if (meleeMinionCount >= LastMeleeMinionCount[i]) LastMeleeMinionCount[i] = meleeMinionCount;

                if (rangedMinionCount >= LastRangedMinionCount[i]) LastRangedMinionCount[i] = rangedMinionCount;

                if (siegeMinionCount >= LastSiegeMinionCount[i]) LastSiegeMinionCount[i] = siegeMinionCount;

                if (hero.IsEnemy && Environment.TickCount - TimeUpdateVisible[i] >= 1000)
                {
                    VisibleCount[i] = 0;
                    foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>())
                    {
                        if (enemy.IsEnemy && !enemy.IsInvulnerable && Vector3.Distance(hero.Position, enemy.Position) <= (hero.AttackRange + 1400))
                        {
                            if (enemy.IsVisible) VisibleCount[i] += 1;
                            if (enemy.IsDead) SharingCount[i] -= 1;
                        }
                    }
                    
                    if (SharingCount[i] < VisibleCount[i]) SharingCount[i] = VisibleCount[i];

                    TimeUpdateVisible[i] = Environment.TickCount;
                    InvisibleCount[i] = (SharingCount[i] - VisibleCount[i]);
                }
                
                if ((Environment.TickCount - TimeExperienceIncrease[i]) >= 0)
                {
                    Cor[i] = Color.White;
                }

                if (hero.IsEnemy && (Environment.TickCount - TimeExperienceIncrease[i]) >= 20000)
                {
                    Cor[i] = Color.Yellow;
                    SharingCount[i] = VisibleCount[i];
                }

                if (hero.IsEnemy && InvisibleCount[i] > 0)
                {
                    Cor[i] = menu.Item("invColor").GetValue<Color>();
                    Console.WriteLine(Cor[i]);
                }
            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            int i = -1;

            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                i += 1;

                if (!hero.IsVisible || hero.IsDead || hero.IsMe || (hero.Level == 18)) continue;

                if (hero.IsEnemy && !menu.Item("showEnemies").GetValue<bool>()) continue;

                int textXOffset = menu.Item("positionX").GetValue<Slider>().Value;
                int textYOffset = menu.Item("positionY").GetValue<Slider>().Value;

                if (hero.IsAlly)
                {
                    textYOffset -= 2;
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
            menu.AddItem(new MenuItem("invColor", "Text Color When Not Visible Enemies").SetValue(Color.FromArgb(255, 245,25,25)));
            menu.AddItem(new MenuItem("positionX", "Text Position X").SetValue(new Slider(142, -100, 200)));
            menu.AddItem(new MenuItem("positionY", "Text Position Y").SetValue(new Slider(21, -100, 100)));
            menu.AddToMainMenu();
        }
    }
}
