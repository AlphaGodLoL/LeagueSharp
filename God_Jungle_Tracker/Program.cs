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
using System.Linq;
using System.Media;

namespace GodJungleTracker
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
            Game.OnUpdate += OnGameUpdate;
            Game.OnProcessPacket += OnProcessPacket;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Game.PrintChat("<font color=\"#00BFFF\">God Jungle Tracker</font> <font color=\"#FFFFFF\"> - Loaded</font>");

        }

        
        private static void PlaySound(SoundPlayer sound = null)
        {
            if (sound != null)
            {
                try
                {
                    sound.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static string TestMinionName = "";

        public static int TestMinionState = 0;

        public static int GuessNetworkID = 1;

        public static int LastPlayedDragonSound = 0;

        public static int LastPlayedBaronSound = 0;

        public static int LastPlayedBaronSound2 = 0;

        public static int BaronSoundDelay = 0;

        public static int UpdateTimer = 0;

        public static Vector3[] CampPosition = 
        { 
        new Vector3(9866f  , 4414f  , -71f),
        new Vector3(5007f  , 10471f , -71f),
        new Vector3(3872f  , 7900f  , 51f),
        new Vector3(10930f , 6992f  , 52f),
        new Vector3(7862f  , 4111f  , 54f),
        new Vector3(7017f  , 10775f , 56f),
        new Vector3(10508f , 5271f  , -62f),
        new Vector3(4418f  , 9664f  , -69f),
        new Vector3(7987f  , 9471f  , 52f),
        new Vector3(6824f  , 5458f  , 53f),
        new Vector3(2091f  , 8428f  , 52f),
        new Vector3(12703f , 6444f  , 52f),
        };

        private static readonly SoundPlayer danger = new SoundPlayer(Properties.Resources.danger);
        private static readonly SoundPlayer danger10 = new SoundPlayer(Properties.Resources.danger10);
        private static readonly SoundPlayer danger25 = new SoundPlayer(Properties.Resources.danger25);
        private static readonly SoundPlayer danger50 = new SoundPlayer(Properties.Resources.danger50);
        private static readonly SoundPlayer danger75 = new SoundPlayer(Properties.Resources.danger75);
        private static SoundPlayer sound = danger;

        public static int[] SoundFow = { 0, 0 };

        public static int[] SoundScreen = { 0, 0 };

        public static int[] NetworkID = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] LastChangeOnState = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] LastChangeOnCampState = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] HeroNetworkID = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static string[] HeroName = { "", "", "", "", "", "", "", "", "", "" };
        
        public static int[] State = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] CampState = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        //public static int[] CreateOrder = { 0, 0, 12, 11, 10, 15, 14, 13, 6, 5, 4, 9, 8, 7, 1, 2, 27, 26, 25, 24, 19, 18, 17, 16, 0, 0, 23, 22, 21, 20, 33, 32, 31, 30, 29, 28, 0 };

        public static string[] CampName = { "Dragon",
                                            "Baron",
                                            "Blue W", 
                                            "Blue E", 
                                            "Red S", 
                                            "Red N", 
                                            "Crab S", 
                                            "Crab N", 
                                            "Raptor N", 
                                            "Raptor S", 
                                            "Gromp W", 
                                            "Gromp E", 
                                            "", "", "", "", "" };

        public static string[] NameToCompare = { "Dragon", 
                                                 "Baron12.1.1",
                                                 "BlueMini21.1.3", "BlueMini1.1.2", "Blue1.1.1", 
                                                 "BlueMini27.1.3", "BlueMini7.1.2", "Blue7.1.1", 
                                                 "RedMini4.1.3", "RedMini4.1.2", "Red4.1.1",
                                                 "RedMini10.1.3", "RedMini10.1.2", "Red10.1.1", 
                                                 "Gromp13.1.1",
                                                 "Gromp14.1.1", 
                                                 "RazorbeakMini9.1.4", "RazorbeakMini9.1.3", "RazorbeakMini9.1.2", "Razorbeak9.1.1",
                                                 "RazorbeakMini3.1.4", "RazorbeakMini3.1.3", "RazorbeakMini3.1.2", "Razorbeak3.1.1", 
                                                 "Crab15.1.1",
                                                 "Crab16.1.1",
                                                 "Krug11.1.2", "KrugMini11.1.1",
                                                 "Krug5.1.2", "KrugMini5.1.1",
                                                 "MurkwolfMini8.1.3", "MurkwolfMini8.1.2", "Murkwolf8.1.1", 
                                                 "MurkwolfMini2.1.3", "MurkwolfMini2.1.2", "Murkwolf2.1.1", 
                                                 "BaronSpawn12.1.2" };


        public static void OnGameUpdate(EventArgs args)
        {

            CampState[0] = State[0];
            LastChangeOnCampState[0] = LastChangeOnState[0];
                
            CampState[1] = State[1];
            LastChangeOnCampState[1] = LastChangeOnState[1];
                
            if ((State[3] == 0 || State[3] == 4) && (State[2] == 2 || State[2] == 4))
            {
                CampState[2] = State[2];
                LastChangeOnCampState[2] = LastChangeOnState[2];
            }
            else
            {
                CampState[2] = State[3];
                LastChangeOnCampState[2] = LastChangeOnState[3];
            }
                
            if ((State[6] == 0 || State[6] == 4) && (State[5] == 2 || State[5] == 4))
            {
                CampState[3] = State[5];
                LastChangeOnCampState[3] = LastChangeOnState[5];
            }
            else
            {
                CampState[3] = State[6];
                LastChangeOnCampState[3] = LastChangeOnState[6];
            }

            if ((State[9] == 0 || State[9] == 4) && (State[8] == 2 || State[8] == 4))
            {
                CampState[4] = State[8];
                LastChangeOnCampState[4] = LastChangeOnState[8];
            }
            else
            {
                CampState[4] = State[9];
                LastChangeOnCampState[4] = LastChangeOnState[9];
            }
                
            if ((State[12] == 0 || State[12] == 4) && (State[11] == 2 || State[11] == 4))
            {
                CampState[5] = State[11];
                LastChangeOnCampState[5] = LastChangeOnState[11];
            }
            else
            {
                CampState[5] = State[12];
                LastChangeOnCampState[5] = LastChangeOnState[12];
            }
                
            CampState[6] = State[24];
            LastChangeOnCampState[6] = LastChangeOnState[24];
                
            CampState[7] = State[25];
            LastChangeOnCampState[7] = LastChangeOnState[25];
                
            CampState[8] = State[19];
            LastChangeOnCampState[8] = LastChangeOnState[19];
                
            CampState[9] = State[23];
            LastChangeOnCampState[9] = LastChangeOnState[23];
                
            CampState[10] = State[14];
            LastChangeOnCampState[10] = LastChangeOnState[14];
                
            CampState[11] = State[15];
            LastChangeOnCampState[11] = LastChangeOnState[15];


            

            if ((menu.Item("soundfow").GetValue<bool>() && SoundFow[0] == 0) || !menu.Item("soundfow").GetValue<bool>())
            {
                if ((menu.Item("soundscreen").GetValue<bool>() && SoundScreen[0] == 0) || !menu.Item("soundscreen").GetValue<bool>())
                {
                    if ((CampState[0] == 2 && menu.Item("dragonsound").GetValue<bool>()) && (Environment.TickCount - LastChangeOnCampState[0] <= 5000))
                    {
                        for (int i = 1; i <= menu.Item("dragonsoundtimes").GetValue<Slider>().Value; i++)
                        {
                            if (i == 1 && Environment.TickCount - LastChangeOnCampState[0] < 50)
                            {
                                LastChangeOnCampState[0] += 50;
                                LastPlayedDragonSound = Environment.TickCount;
                                PlaySound(sound);

                            }
                            else if (i > 1 && Environment.TickCount - LastChangeOnCampState[0] > 550 * (i - 1) && Environment.TickCount - LastChangeOnCampState[0] < 600 * (i - 1) && Environment.TickCount - LastPlayedDragonSound > 500)
                            {
                                LastChangeOnCampState[0] += (50 * (i - 1));
                                LastPlayedDragonSound = Environment.TickCount;
                                PlaySound(sound);
                            }
                        }
                    }
                }
            }

            if ((menu.Item("soundfow").GetValue<bool>() && SoundFow[1] == 0) || !menu.Item("soundfow").GetValue<bool>())
            {
                if ((menu.Item("soundscreen").GetValue<bool>() && SoundScreen[1] == 0) || !menu.Item("soundscreen").GetValue<bool>())
                {
                    if ((CampState[1] == 2 && menu.Item("baronsound").GetValue<bool>()) && (Environment.TickCount - BaronSoundDelay >= (menu.Item("sounddelay").GetValue<Slider>().Value * 1000)))
                    {
                        for (int i = 1; i <= menu.Item("baronsoundtimes").GetValue<Slider>().Value; i++)
                        {

                            if (i == 1 && (LastPlayedBaronSound == 0 || Environment.TickCount - LastPlayedBaronSound >= (menu.Item("sounddelay").GetValue<Slider>().Value * 1000)) && Environment.TickCount - LastChangeOnCampState[1] < 500)
                            {
                                LastPlayedBaronSound = Environment.TickCount;
                                LastPlayedBaronSound2 = Environment.TickCount;
                                PlaySound(sound);
                            }

                            else if (i > 1 && Environment.TickCount - LastPlayedBaronSound > 550 * (i - 1) && Environment.TickCount - LastPlayedBaronSound < 600 * (i - 1) && Environment.TickCount - LastPlayedBaronSound2 > 500)
                            {
                                LastPlayedBaronSound += (50 * (i - 1));

                                LastPlayedBaronSound2 = Environment.TickCount;


                                if (i == menu.Item("baronsoundtimes").GetValue<Slider>().Value)
                                {
                                    BaronSoundDelay = Environment.TickCount;
                                }

                                PlaySound(sound);

                            }
                        }
                    }
                }
            }

            if (Environment.TickCount - UpdateTimer < 300) return;

            if (menu.Item("soundvolume").GetValue<StringList>().SelectedIndex.Equals(0))
            {
                sound = danger10;
            }
            else if (menu.Item("soundvolume").GetValue<StringList>().SelectedIndex.Equals(1))
            {
                sound = danger25;
            }
            else if (menu.Item("soundvolume").GetValue<StringList>().SelectedIndex.Equals(2))
            {
                sound = danger50;
            }
            else if (menu.Item("soundvolume").GetValue<StringList>().SelectedIndex.Equals(3))
            {
                sound = danger75;
            }
            else if (menu.Item("soundvolume").GetValue<StringList>().SelectedIndex.Equals(4))
            {
                sound = danger;
            }

            foreach (Obj_AI_Minion Minion in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.CampNumber > 0))
            {
                for (int i = 0; i <= 25; i++)
                {

                    if (!menu.Item("dragon").GetValue<bool>() && i == 0) continue;
                    if (!menu.Item("baron").GetValue<bool>() && i == 1) continue;
                    if (!menu.Item("blue").GetValue<bool>() && i >= 2 && i <= 7) continue;
                    if (!menu.Item("red").GetValue<bool>() && i >= 8 && i <= 13) continue;
                    if (!menu.Item("gromp").GetValue<bool>() && i >= 14 && i <= 15) continue;
                    if (!menu.Item("raptor").GetValue<bool>() && i >= 16 && i <= 23) continue;
                    if (!menu.Item("crab").GetValue<bool>() && i >= 24 && i <= 25) continue;

                    if (Minion.Name.Contains(NameToCompare[i]))
                    {
                        if (Minion.IsVisible)
                        {
                            if (!Minion.IsDead && NetworkID[i] != Minion.NetworkId)
                            {

                                NetworkID[i] = Minion.NetworkId;
                                State[i] = 1;
                                LastChangeOnState[i] = Environment.TickCount;
                                //Console.WriteLine("NetworkId["+i+"]: " + Minion.NetworkId + " Name: " + Minion.Name);

                                /*  //Useless It only starts receiving attack packets after vision maybe works with buff recognition will test later
                                if (GuessNetworkID == 0) // Game.Time < 125f)
                                {
                                    //Console.WriteLine("The ID guess for: " + NetworkID[i] + " and Name: " + NameToCompare[i]);
                                    for (int c = 0; c <= 35; c++)
                                    {
                                        if (c == i || CreateOrder[c] == 0) continue;

                                        if (NetworkID[c] == 0)
                                        {
                                            if (CreateOrder[c] < CreateOrder[i])
                                            {
                                                NetworkID[c] = NetworkID[i] - ((CreateOrder[i] - CreateOrder[c]) * 2);
                                                State[c] = 1;
                                                LastChangeOnState[c] = Environment.TickCount;
                                            }
                                            else if (CreateOrder[c] > CreateOrder[i])
                                            {
                                                NetworkID[c] = NetworkID[i] + ((CreateOrder[c] - CreateOrder[i]) * 2);
                                                State[c] = 1;
                                                LastChangeOnState[c] = Environment.TickCount;
                                            }
                                            //Console.WriteLine("NetworkID["+c+"]:" + NetworkID[c] + " and Name: " + NameToCompare[c]);
                                        }
                                    }
                                    GuessNetworkID = 1;
                                }*/
                            }
                            else if (Minion.IsDead)
                            {
                                State[i] = 4;
                                LastChangeOnState[i] = Environment.TickCount;
                            }

                            if (i < 2)
                            {
                                SoundFow[i] = 1;

                                if (Game.CursorPos.Distance(CampPosition[i]) < 1800) SoundScreen[i] = 1;

                                else SoundScreen[i] = 0;
                            }
                        }
                        else if (!Minion.IsVisible && i < 2)
                        {
                            SoundFow[i] = 0;
                            SoundScreen[i] = 0;
                        }
                    }


                    int t = 3000;

                    if (i == 0) t = 60000;

                    if (State[i] == 2 && (Environment.TickCount - LastChangeOnState[i]) >= t && !(NameToCompare[i].Contains("Crab")))    //presumed dead
                    {
                        State[i] = 4;
                        LastChangeOnState[i] = Environment.TickCount;
                    }
                    else if (State[i] == 2 && (Environment.TickCount - LastChangeOnState[i]) >= 10000 && (NameToCompare[i].Contains("Crab")))
                    {
                        State[i] = 1;
                        LastChangeOnState[i] = Environment.TickCount;
                    }

                    else if (State[i] == 3 && (Environment.TickCount - LastChangeOnState[i]) >= 2000)    //after desingaged wait 2 sec's
                    {
                        State[i] = 1;
                        LastChangeOnState[i] = Environment.TickCount;
                    }

                    else if (State[i] == 4 && (Environment.TickCount - LastChangeOnState[i]) >= 5000)   //if dead stop tracking
                    {
                        State[i] = 0;
                        LastChangeOnState[i] = Environment.TickCount;
                    }
                }
            }

            /*
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                int i = -1;
                i++;
                if (hero.IsVisible)
                {
                    HeroNetworkID[i] = hero.NetworkId;
                    HeroName[i] = hero.BaseSkinName;
                }
            }*/

            UpdateTimer = Environment.TickCount;
        }

        private static void OnProcessPacket(GamePacketEventArgs args)
        {
            short header = BitConverter.ToInt16(args.PacketData, 0);
            
            /*
            if (header == 207)  //test header
            {
                Console.WriteLine("NetworkID == " + BitConverter.ToInt32(args.PacketData, 2));
                for (int d = 0; d <= 96; d += 8)
                {
                    if (d <= 8)
                    {
                        try
                        {
                            Console.WriteLine("Packet Index: 0" + d + " ---- " + (BitConverter.ToString(args.PacketData, d)).Substring(0, 23));
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine(ex);
                        }

                    }
                    else
                    {
                        try
                        {
                            Console.WriteLine("Packet Index: " + d + " ---- " + (BitConverter.ToString(args.PacketData, d)).Substring(0, 23));
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine(ex);
                        }
                    }
                }
            }*/

            for (int i = 0; i <= 25; i++)
            {

                if (NetworkID[i] == 0) continue;

                if (BitConverter.ToInt32(args.PacketData, 2) == NetworkID[i])
                {
                    /*
                    if (NameToCompare[i].Contains("Dragon")) //&& header != 52)   //Packet test
                    {
                       Game.PrintChat("Packet Header is: " + header + " For: " + NameToCompare[i] + " NetworkID == " + NetworkID[i]);
                    }*/

                    if (header == 148)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Attacking");   //"using skill" or crab dead

                        if (NameToCompare[i].Contains("Crab"))
                        {
                            State[i] = 4;
                        }
                        else
                        {
                            State[i] = 2;
                        }

                        UpdateTimer = 0;
                        LastChangeOnState[i] = Environment.TickCount;
                    }

                    else if (header == 200)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Attacking");

                        State[i] = 2;
                        UpdateTimer = 0;
                        LastChangeOnState[i] = Environment.TickCount;
                    }

                    else if (header == 88)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Disengaged");
                        if (NameToCompare[i].Contains("Crab"))
                        {
                            State[i] = 2;
                        }
                        else
                        {
                            State[i] = 3;
                        }

                        UpdateTimer = 0;
                        LastChangeOnState[i] = Environment.TickCount;
                    }
                }
            }

            /*
            //Enemy Heros header
            for (int i = 0; i <= 9; i++)
            {

                if (HeroNetworkID[i] == 0) continue;

                if (BitConverter.ToInt32(args.PacketData, 2) == HeroNetworkID[i] )//&& header == 148)
                {
                    //Console.WriteLine("Packet Header is: " + header + " For: " + HeroName[i]);
                    for (int d = 0; d <= 96; d +=8)
                    {
                        if (d <= 8)
                        {
                            try
                            {
                                //Console.WriteLine("Packet Index: 0" + d + " ---- " + (BitConverter.ToString(args.PacketData, d)).Substring(0, 23));
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine(ex);
                            }
                            
                        }
                        else
                        {
                            try
                            {
                                //Console.WriteLine("Packet Index: " + d + " ---- " + (BitConverter.ToString(args.PacketData, d)).Substring(0, 23));
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine(ex);
                            }
                        }
                    }


                    if (header == 68)
                    {
                        //Console.WriteLine(HeroName[i] + " Skill Channeling finished");
                    }
              
                    if (header == 148)
                    {
                        //Console.WriteLine(HeroName[i] + " got a new Buff");
                    }

                    if (header == 148//5.6
                    {
                        //Console.WriteLine(HeroName[i] + " is using skill");
                    }

                    if (header == 200)//5.6
                    {
                        //Console.WriteLine(HeroName[i] + " is Attacking");
                    }

                    if (header == 108)//5.6
                    {
                        //Console.WriteLine(HeroName[i] + " Lost Vision");
                    }

                    if (header == 88)//5.6
                    {
                        //Console.WriteLine(HeroName[i] + " is Disengaged");
                    }

                    if (header == 226)
                    {
                        //Console.WriteLine(HeroName[i] + " is Dead");
                    }
                }
        
            }
            
            */

        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            Color cor = Color.FromArgb(255, 0, 255, 0);

            int c = 0;

            /*
            CampState == 0 Not Tracking
            CampState == 1 Tracking/Iddle
            CampState == 2 Attacking
            CampState == 3 Disengaged
            CampState == 4 Dead
            */

            for (int i = 0; i <= 11; i++)
            {
                if (!menu.Item("drawtracklist").GetValue<bool>()) break;

                if (CampState[i] > 0)
                {
                    int x = menu.Item("posX").GetValue<Slider>().Value;
                    int y = menu.Item("posY").GetValue<Slider>().Value - c * 30;

                    c += 1;

                    if (CampName[i].Contains("Dragon"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 255, 165, 15), CampName[i]);
                    }
                    else if (CampName[i].Contains("Baron"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 153, 50, 204), CampName[i]);
                    }
                    else if (CampName[i].Contains("Blue"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 0, 255, 255), CampName[i]);
                    }
                    else if (CampName[i].Contains("Red"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 255, 0, 0), CampName[i]);
                    }
                    else if (CampName[i].Contains("Crab"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 152, 251, 152), CampName[i]);
                    }
                    else if (CampName[i].Contains("Raptor"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 255, 127, 80), CampName[i]);
                    }
                    else if (CampName[i].Contains("Gromp"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 0, 139, 45), CampName[i]);
                    }
                    if (CampState[i] == 1)
                    {
                        cor = Color.FromArgb(255, 0, 255, 0);
                    }
                    else if (CampState[i] == 2)
                    {
                        cor = Color.FromArgb(255, 255, 0, 0);
                    }
                    else if (CampState[i] == 3)
                    {
                        cor = Color.FromArgb(255, 255, 210, 0);
                    }
                    else if (CampState[i] == 4)
                    {
                        cor = Color.FromArgb(255, 200, 200, 200);
                    }


                    Drawing.DrawLine(
                        new Vector2(x - 4.5f, y - 5),
                        new Vector2(x + 70, y - 5), 3, cor);

                    Drawing.DrawLine(
                        new Vector2(x + 70, y - 4.5f),
                        new Vector2(x + 70, y + 21), 3, cor);

                    Drawing.DrawLine(
                        new Vector2(x + 70, y + 21),
                        new Vector2(x - 3, y + 21), 3, cor);

                    Drawing.DrawLine(
                        new Vector2(x - 5, y + 21),
                        new Vector2(x - 5, y - 4.5f), 3, cor);
                }
            }

            if (c == 0 && menu.Item("drawtracklist").GetValue<bool>())
            {
                cor = Color.FromArgb(255, 255, 255, 255);

                int x = menu.Item("posX").GetValue<Slider>().Value;
                int y = menu.Item("posY").GetValue<Slider>().Value - c * 30;

                Drawing.DrawText(x, y, cor, "TrackList");

                Drawing.DrawLine(
                        new Vector2(x - 4.5f, y - 5),
                        new Vector2(x + 70, y - 5), 3, cor);

                Drawing.DrawLine(
                    new Vector2(x + 70, y - 4.5f),
                    new Vector2(x + 70, y + 21), 3, cor);

                Drawing.DrawLine(
                    new Vector2(x + 70, y + 21),
                    new Vector2(x - 3, y + 21), 3, cor);

                Drawing.DrawLine(
                    new Vector2(x - 5, y + 21),
                    new Vector2(x - 5, y - 4.5f), 3, cor);

            }
        }

        public static void Drawing_OnEndScene(EventArgs args)
        {
            for (int i = 0; i <= 11; i++)
            {
                if (!menu.Item("TrackonMinimap").GetValue<bool>()) break;

                if ((i == 0 || i == 6 || i == 7) && CampState[i] == 2 && (Environment.TickCount - LastChangeOnCampState[i]) < 60000)
                {
                    Utility.DrawCircle(CampPosition[i], 500, Color.Red, 2, 15, true);
                }
                else if (i != 0 && CampState[i] == 2 && (Environment.TickCount - LastChangeOnCampState[i]) < 10000)
                {
                    Utility.DrawCircle(CampPosition[i], 500, Color.Red, 2, 15, true);
                }
                else if (CampState[i] == 1)
                {
                    Utility.DrawCircle(CampPosition[i], 400, Color.Lime, 1, 15, true);
                }
                else if (CampState[i] == 3)
                {
                    Utility.DrawCircle(CampPosition[i], 450, Color.FromArgb(255, 255, 210, 0), 2, 15, true);
                }
                else if (CampState[i] == 4)
                {
                    Utility.DrawCircle(CampPosition[i], 400, Color.FromArgb(255, 200, 200, 200), 1, 15, true);
                }
            }
        }

        static void LoadMenu()
        {
            //Start Menu
            menu = new Menu("God Jungle Tracker", "God Jungle Tracker", true);

            //Camp to Track Menu
            menu.AddSubMenu(new Menu("Camp to Track", "Camp to Track"));
            menu.SubMenu("Camp to Track").AddItem(new MenuItem("baron", "Baron").SetValue(true));
            menu.SubMenu("Camp to Track").AddItem(new MenuItem("dragon", "Dragon").SetValue(true));
            menu.SubMenu("Camp to Track").AddItem(new MenuItem("blue", "Blue").SetValue(true));
            menu.SubMenu("Camp to Track").AddItem(new MenuItem("red", "Red").SetValue(true));
            menu.SubMenu("Camp to Track").AddItem(new MenuItem("crab", "Crab").SetValue(true));
            menu.SubMenu("Camp to Track").AddItem(new MenuItem("raptor", "Raptor").SetValue(true));
            menu.SubMenu("Camp to Track").AddItem(new MenuItem("gromp", "Gromp").SetValue(true));

            //Track List Menu
            menu.AddSubMenu(new Menu("Track List", "Track List"));
            menu.SubMenu("Track List").AddItem(new MenuItem("drawtracklist", "Draw Track List").SetValue(false));
            menu.SubMenu("Track List").AddItem(new MenuItem("posX", "Track List Pos X").SetValue(new Slider(Drawing.Width / 2, 0, Drawing.Width)));
            menu.SubMenu("Track List").AddItem(new MenuItem("posY", "Track List Pos Y").SetValue(new Slider(Drawing.Height / 2, 0, Drawing.Height)));

            //Play Danger Sound
            menu.AddSubMenu(new Menu("Play Danger Sound", "Play Danger Sound"));
            menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("dragonsound", "On Dragon First Attack").SetValue(true));
            menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("dragonsoundtimes", "Play Sound X Times").SetValue(new Slider(2, 1, 4)));
            menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("baronsound", "On Baron Attack").SetValue(true));
            menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("baronsoundtimes", "Play Sound X Times").SetValue(new Slider(2, 1, 4)));
            menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("sounddelay", "Baron Sound Delay (s)").SetValue(new Slider(20, 1, 60)));
            menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("soundfow", "Only On Fog of War").SetValue(false));
            menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("soundscreen", "Only If Camp Not On Screen").SetValue(true));
            String [] volume = {"10%","25%","50%","75%","100%"};
            menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("soundvolume", "Sound Volume").SetValue(new StringList(volume, 2)));

            //Track on Minimap
            menu.AddItem(new MenuItem("TrackonMinimap", "Track on Minimap").SetValue(true));

            menu.AddToMainMenu();
        }
    }
}
