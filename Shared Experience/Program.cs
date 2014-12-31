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
            Game.PrintChat("Shared Experience Loaded!");
        }

        public static float[] Exp = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] SharingCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static int[] VisibleCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] InvisibleCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] Time = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static Color[] Cor = { Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White };

        public static void OnGameUpdate(EventArgs args)
        {
            int i = -1;
            int expReceived = 0;


            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                i += 1;

                if (!hero.IsVisible || hero.IsDead || hero.IsMe || (hero.Level == 18))
                {
                    continue;
                }

                if (hero.IsAlly && !menu.Item("showAllies").GetValue<bool>()) continue;

                if (hero.IsEnemy && !menu.Item("showEnemies").GetValue<bool>()) continue;

                if (Exp[i] != hero.Experience)
                {
                    expReceived = (int)Math.Floor(hero.Experience - Exp[i]);

                    if ((expReceived >= 7 && expReceived < 9) || (expReceived >= 15 && expReceived < 18) || (expReceived >= 24 && expReceived < 25))
                    {
                        SharingCount[i] = 5;
                        Time[i] = Environment.TickCount;

                        if (hero.IsEnemy)
                        {
                            VisibleCount[i] = 0;
                            foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>())
                            {
                                if (enemy.IsEnemy && enemy.IsVisible && Vector3.Distance(hero.Position, enemy.Position) <= (hero.AttackRange + 1400))
                                {
                                    VisibleCount[i] += 1;
                                }
                            }
                            //Console.WriteLine((i) + " " + (hero.ChampionName) + "  " + (expReceived) + "  5 champions sharing that exp and " + (SharingCount[i] - VisibleCount[i]) + " Invisible champs");
                        }
                    }
                    else if ((expReceived >= 9 && expReceived < 12) || (expReceived >= 30 && expReceived < 33))
                    {
                        SharingCount[i] = 4;
                        Time[i] = Environment.TickCount;

                        if (hero.IsEnemy)
                        {
                            VisibleCount[i] = 0;
                            foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>())
                            {
                                if (enemy.IsEnemy && enemy.IsVisible && Vector3.Distance(hero.Position, enemy.Position) <= (hero.AttackRange + 1400))
                                {
                                    VisibleCount[i] += 1;
                                }
                            }
                            //Console.WriteLine((i) + " " + (hero.ChampionName) + "  " + (expReceived) + "  4 champions sharing that exp and " + (SharingCount[i] - VisibleCount[i]) + " Invisible champs");
                        }
                    }
                    else if ((expReceived >= 12 && expReceived < 15) || (expReceived >= 25 && expReceived < 28) || (expReceived >= 40 && expReceived < 44))
                    {
                        SharingCount[i] = 3;
                        Time[i] = Environment.TickCount;

                        if (hero.IsEnemy)
                        {
                            VisibleCount[i] = 0;
                            foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>())
                            {
                                if (enemy.IsEnemy && enemy.IsVisible && Vector3.Distance(hero.Position, enemy.Position) <= (hero.AttackRange + 1400))
                                {
                                    VisibleCount[i] += 1;
                                }
                            }
                            //Console.WriteLine((i) + " " + (hero.ChampionName) + "  " + (expReceived) + "  3 champions sharing that exp and " + (SharingCount[i] - VisibleCount[i]) + " Invisible champs");
                        }
                    }
                    else if ((expReceived >= 38 && expReceived < 40) || (expReceived >= 60 && expReceived < 66))
                    {
                        SharingCount[i] = 2;
                        Time[i] = Environment.TickCount;

                        if (hero.IsEnemy)
                        {
                            VisibleCount[i] = 0;
                            foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>())
                            {
                                if (enemy.IsEnemy && enemy.IsVisible && Vector3.Distance(hero.Position, enemy.Position) <= (hero.AttackRange + 1400))
                                {
                                    VisibleCount[i] += 1;
                                }
                            }
                            //Console.WriteLine((i) + " " + (hero.ChampionName) + "  " + (expReceived) + "  2 champions sharing that exp and " + (SharingCount[i] - VisibleCount[i]) + " Invisible champs");
                        }
                    }
                    else if ((expReceived >= 29 && expReceived < 30) || (expReceived >= 58 && expReceived < 60) || (expReceived >= 92 && expReceived < 100))
                    {
                        VisibleCount[i] = 1;
                        SharingCount[i] = 1;
                        Time[i] = Environment.TickCount;
                    }
                    Exp[i] = (hero.Experience);
                }
                if ((Environment.TickCount - Time[i]) > 20000)
                {
                    SharingCount[i] = 1;
                }
                else if ((Environment.TickCount - Time[i]) > 15000)
                {
                    Cor[i] = Color.Red;
                }
                else if ((Environment.TickCount - Time[i]) > 10000)
                {
                    Cor[i] = Color.Yellow;
                }
                else if ((Environment.TickCount - Time[i]) > 0)
                {
                    Cor[i] = Color.White;
                }

                if (hero.IsAlly)
                {
                    VisibleCount[i] = 6;
                }

                if (SharingCount[i] > 1 && VisibleCount[i] < SharingCount[i])
                {
                    InvisibleCount[i] = (SharingCount[i] - VisibleCount[i]);
                    Cor[i] = menu.Item("invColor").GetValue<Color>();

                    Console.WriteLine((i) + " " + (hero.ChampionName) + " Invis Color:" + (Cor[i]));
                }
            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            
            int i = -1;

            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                i += 1;

                if (!hero.IsVisible || hero.IsDead || hero.IsMe || (hero.Level == 18))
                {
                    continue;
                }

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
                    if (VisibleCount[i] < SharingCount[i])
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
            menu.AddItem(new MenuItem("invColor", "Text Color When Not Visible Enemies").SetValue(Color.FromArgb(255, Color.White)));
            menu.AddItem(new MenuItem("positionX", "Text Position X").SetValue(new Slider(142, -100, 200)));
            menu.AddItem(new MenuItem("positionY", "Text Position Y").SetValue(new Slider(21, -100, 100)));
            menu.AddToMainMenu();
        }
    }
}
