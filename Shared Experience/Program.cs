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
            Game.PrintChat("Shared Experience Loaded! v.1.0.0.2");
        }

        public static float[] Exp = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] SharingCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] VisibleCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] InvisibleCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] TimeExperienceIncrease = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] TimeUpdateVisible = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] TimeMissing = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static Color[] Cor = { Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White};
        public static int Zilean = 0;

        public static void OnGameUpdate(EventArgs args)
        {
            int i = -1;
            int expReceived = 0;


            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                i += 1;

                if (hero.IsEnemy && hero.ChampionName == "Zilean" && Zilean == 0)
                {
                    Game.PrintChat("Shared Experience: No support for Zilean passive yet, could give incorrect values");
                    Zilean = 1;
                }

                if (!hero.IsVisible || hero.IsDead || hero.IsMe || (hero.Level == 18)) continue;

                if (hero.IsAlly && !menu.Item("showAllies").GetValue<bool>()) continue;

                if (hero.IsEnemy && !menu.Item("showEnemies").GetValue<bool>()) continue;

                if (Environment.TickCount - TimeMissing[i] >= 5000)
                {
                    Exp[i] = (hero.Experience);
                    SharingCount[i] = 0;
                }

                TimeMissing[i] = Environment.TickCount;

                if (Exp[i] != hero.Experience)
                {
                    expReceived = (int)Math.Floor(hero.Experience - Exp[i]);

                    if (expReceived == 7 || expReceived == 15 || expReceived == 24)
                    {
                        SharingCount[i] = 5;
                        TimeExperienceIncrease[i] = Environment.TickCount;
                        TimeUpdateVisible[i] = 0;
                    }
                    else if (expReceived == 9  || expReceived == 30)
                    {
                        SharingCount[i] = 4;
                        TimeExperienceIncrease[i] = Environment.TickCount;
                        TimeUpdateVisible[i] = 0;
                    }
                    else if (expReceived == 12 || expReceived == 25 || expReceived == 40)
                    {
                        SharingCount[i] = 3;
                        TimeExperienceIncrease[i] = Environment.TickCount;
                        TimeUpdateVisible[i] = 0;
                    }
                    else if (expReceived == 38 || expReceived == 60)
                    {
                        SharingCount[i] = 2;
                        TimeExperienceIncrease[i] = Environment.TickCount;
                        TimeUpdateVisible[i] = 0;
                    }
                    else if (expReceived == 29 || expReceived == 58|| expReceived == 92)
                    {
                        VisibleCount[i] = 1;
                        SharingCount[i] = 1;
                        TimeExperienceIncrease[i] = Environment.TickCount;
                        TimeUpdateVisible[i] = Environment.TickCount;
                    }
                    Exp[i] = (hero.Experience);
                    //Console.WriteLine((i) + " " + (hero.ChampionName) + "  Last exp received: " + (expReceived) + " exp " + (SharingCount[i]) + " champions sharing that exp");
                }

                if (hero.IsEnemy && Environment.TickCount - TimeUpdateVisible[i] >= 1000)
                {
                    VisibleCount[i] = 0;
                    foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>())
                    {
                        if (enemy.IsEnemy && Vector3.Distance(hero.Position, enemy.Position) <= (hero.AttackRange + 1400))
                        {
                            if (enemy.IsVisible) VisibleCount[i] += 1;
                            if (enemy.IsDead) SharingCount[i] -= 1;
                        }
                    }
                    TimeUpdateVisible[i] = Environment.TickCount;
                    if (SharingCount[i] < VisibleCount[i])
                    {
                        SharingCount[i] = VisibleCount[i];
                    }
                    InvisibleCount[i] = (SharingCount[i] - VisibleCount[i]);
                }
                
                if ((Environment.TickCount - TimeExperienceIncrease[i]) >= 0)
                {
                    Cor[i] = Color.White;
                }

                if (hero.IsEnemy && (Environment.TickCount - TimeExperienceIncrease[i]) >= 30000)
                {
                    Cor[i] = Color.Yellow;
                    SharingCount[i] = VisibleCount[i];
                }

                if (hero.IsEnemy && InvisibleCount[i] > 0)
                {
                    Cor[i] = menu.Item("invColor").GetValue<Color>();
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

                if (hero.IsAlly && !menu.Item("showAllies").GetValue<bool>()) continue;

                if (hero.IsEnemy && !menu.Item("showEnemies").GetValue<bool>()) continue;

                int textXOffset = menu.Item("positionX").GetValue<Slider>().Value;
                int textYOffset = menu.Item("positionY").GetValue<Slider>().Value;

                if (hero.IsAlly)
                {
                    textYOffset -= 2;
                }

                if (SharingCount[i] > 1)
                {
                    if (InvisibleCount[i] > 0)
                    {
                        Drawing.DrawText(hero.HPBarPosition.X + textXOffset, hero.HPBarPosition.Y + textYOffset, Cor[i], "+" + (SharingCount[i] - 1) + " (" + InvisibleCount[i] + " Inv)");
                    }
                    else
                    {
                        Drawing.DrawText(hero.HPBarPosition.X + textXOffset, hero.HPBarPosition.Y + textYOffset, Cor[i], "+" + (SharingCount[i] - 1));
                    }
                }
            }
        }

        static void LoadMenu()
        {
            menu = new Menu("Shared Experience", "Shared Experience", true);
            menu.AddItem(new MenuItem("showAllies", "Show Allies Exp Share Count").SetValue(false));
            menu.AddItem(new MenuItem("showEnemies", "Show Enemies Exp Share Count").SetValue(true));
            menu.AddItem(new MenuItem("invColor", "Text Color When Not Visible Enemies").SetValue(Color.FromArgb(150, Color.Red)));
            menu.AddItem(new MenuItem("positionX", "Text Position X").SetValue(new Slider(142, -100, 200)));
            menu.AddItem(new MenuItem("positionY", "Text Position Y").SetValue(new Slider(21, -100, 100)));
            menu.AddToMainMenu();
        }
    }
}
