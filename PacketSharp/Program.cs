/*
██████╗ ██╗   ██╗     █████╗ ██╗     ██████╗ ██╗  ██╗ █████╗  ██████╗  ██████╗ ██████╗ 
██╔══██╗╚██╗ ██╔╝    ██╔══██╗██║     ██╔══██╗██║  ██║██╔══██╗██╔════╝ ██╔═══██╗██╔══██╗
██████╔╝ ╚████╔╝     ███████║██║     ██████╔╝███████║███████║██║  ███╗██║   ██║██║  ██║
██╔══██╗  ╚██╔╝      ██╔══██║██║     ██╔═══╝ ██╔══██║██╔══██║██║   ██║██║   ██║██║  ██║
██████╔╝   ██║       ██║  ██║███████╗██║     ██║  ██║██║  ██║╚██████╔╝╚██████╔╝██████╔╝
╚═════╝    ╚═╝       ╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═════╝ 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;


namespace PacketSharp
{
    class Program
    {
        static Menu _menu;

        public static int NetworkIdRange;
        public static int TestNetworkId;
        public static int HeaderLenght;
        public static int HeaderLenght2;
        public static int LastLenght;
        public static bool WritePacket;

        public static List<GameObject> GameObjects = new List<GameObject>();
        public static List<GameObject> ObjCloseToMouse = new List<GameObject>();
        public static int UpdateTick = 0;

        public static List<short> HeaderBlockedList { get; set; }

        public static List<int[]> Packets = new List<int[]>();

        static void Main(string[] args)
        {
            LoadMenu();

            Game.OnProcessPacket += OnProcessPacket;

            HeaderBlockedList = new List<short>();

            CustomEvents.Game.OnGameLoad += OnGameLoad;
            Drawing.OnDraw += OnDraw;
        }

        public static void OnGameLoad(EventArgs args)
        {
            Game.OnUpdate += OnUpdate;

            Utility.DelayAction.Add(300, () => Console.WriteLine("-------------------------"));
            Utility.DelayAction.Add(400, () => Console.WriteLine("Region is: " + Game.Region));
            Utility.DelayAction.Add(500, () => Console.WriteLine("-------------------------"));
            Utility.DelayAction.Add(600, () => Console.WriteLine("Game Version is: " + Game.Version));
            Utility.DelayAction.Add(700, () => Console.WriteLine("-------------------------"));
            
            

            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                NetworkIdRange = hero.NetworkId;
                return;
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Environment.TickCount - UpdateTick > 150)
            {
                GameObjects = ObjectManager.Get<GameObject>().ToList();
                ObjCloseToMouse = GameObjects.Where(o => o.Position.Distance(Game.CursorPos) < _menu.Item("cursorrange").GetValue<Slider>().Value).ToList();//&& !(o is Obj_Turret) && o.Name != "missile" && !(o is Obj_LampBulb) && !(o is Obj_SpellMissile) && !(o is GrassObject) && !(o is DrawFX) && !(o is LevelPropSpawnerPoint) && !(o is Obj_GeneralParticleEmitter) && !o.Name.Contains("MoveTo")).ToList();
                UpdateTick = Environment.TickCount;
                Packets.RemoveAll(item => item[1] > _menu.Item("packetamount").GetValue<Slider>().Value);
            }
        }

        private static void OnProcessPacket(GamePacketEventArgs args)
        {
            short header = BitConverter.ToInt16(args.PacketData, 0);

            if (header == 0) Console.WriteLine("Test");

            bool searchAll = false;

            if (_menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(0))
            {
                HeaderLenght = 302;
                HeaderLenght2 = 311;
            }
            else if (_menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(1))
            {
                HeaderLenght = 35;
                HeaderLenght2 = 35;
            }
            else if (_menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(2))
            {
                HeaderLenght = 71;
                HeaderLenght2 = 71;
            }
            else if (_menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(3))
            {
                HeaderLenght = 68;
                HeaderLenght2 = 68;
            }
            else if (_menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(4))
            {
                HeaderLenght = 47;
                HeaderLenght2 = 47;
            }
            else if (_menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(5))
            {
                HeaderLenght = _menu.Item("customLenght1").GetValue<Slider>().Value;
                HeaderLenght2 = _menu.Item("customLenght2").GetValue<Slider>().Value;
                searchAll = true;
            }

            if (_menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(5) && header >= _menu.Item("customHeader1").GetValue<Slider>().Value && header <= _menu.Item("customHeader2").GetValue<Slider>().Value)
            {
                if (_menu.Item("headersblock").GetValue<bool>() && !HeaderBlockedList.Contains(header))
                {
                    HeaderBlockedList.Add(header);
                    Console.WriteLine("Header: " + header + " added to the block list");
                    return;
                }
            }

            if (_menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(0) ||
                _menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(1) ||
                _menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(2) ||
                _menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(3) ||
                _menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(4))
            {
                if (_menu.Item("headersblock").GetValue<bool>() && !HeaderBlockedList.Contains(header))
                {
                    HeaderBlockedList.Add(header);
                    Console.WriteLine("Header: " + header + " added to the block list");
                    return;
                }
            }

            if (_menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(5) && 
                    (header < _menu.Item("customHeader1").GetValue<Slider>().Value ||
                    header > _menu.Item("customHeader2").GetValue<Slider>().Value))
            {
                return;
            }

            if (!HeaderBlockedList.Contains(header) &&//BitConverter.ToInt32(args.PacketData, 2) >= NetworkIdRange &&
                 ((BitConverter.ToString(args.PacketData, 0).Length == HeaderLenght ||
                BitConverter.ToString(args.PacketData, 0).Length == HeaderLenght2) ||
                (searchAll && (BitConverter.ToString(args.PacketData, 0).Length >= HeaderLenght &&
                BitConverter.ToString(args.PacketData, 0).Length <= HeaderLenght2))))
            {
                WritePacket = false;

                TestNetworkId = BitConverter.ToInt32(args.PacketData, 2);

                LastLenght = BitConverter.ToString(args.PacketData, 0).Length;

                if (TestNetworkId > 0)
                {
                    try
                    {
                        foreach (var packet in Packets.Where(packet => packet[0] == TestNetworkId))
                        {
                            try
                            {
                                packet[1]++;
                            }
                            catch (Exception)
                            {

                                //ignored
                            }
                        }
                        try
                        {
                            Packets.Add(new int[] { TestNetworkId, 1, (int)header, LastLenght });
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                }
                

                #region Get Object

                if (_menu.Item("getobject").GetValue<bool>())
                {
                    foreach (var obj in ObjectManager.Get<GameObject>().ToList())
                    {
                        if (obj.IsValid && obj.NetworkId == TestNetworkId && TestNetworkId != 0)
                        {
                            if (obj is Obj_AI_Hero && !_menu.Item("obj1").GetValue<bool>())
                            {
                                return;
                            }
                            if (obj is Obj_AI_Minion && !_menu.Item("obj2").GetValue<bool>())
                            {
                                return;
                            }
                            if (obj is Obj_AI_Turret && !_menu.Item("obj3").GetValue<bool>())
                            {
                                return;
                            }
                            if (!_menu.Item("packetobjmouse").GetValue<bool>() || (_menu.Item("packetobjmouse").GetValue<bool>() && obj.Position.Distance(Game.CursorPos) < _menu.Item("cursorrange").GetValue<Slider>().Value))
                            {
                                Console.WriteLine(" ");
                                Console.WriteLine("-------------------------------------------------------------------------------");
                                Console.WriteLine(" ");
                                Console.WriteLine("Header: " + header + " - Lenght: " + LastLenght + " - NetworkId: " + obj.NetworkId);
                                Console.WriteLine(" ");
                                Console.WriteLine("Name: " + obj.Name + " - Team: " + obj.Team.ToString());
                                Console.WriteLine(" ");
                                Console.WriteLine("Type: " + obj.Type.ToString() + " - IsVisible: " + obj.IsVisible);
                                WritePacket = true;
                            }
                        }
                    }
                    //GetObject(TestNetworkId, header);
                }

                if (!WritePacket && 
                    (_menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(0)
                    || (_menu.Item("finding").GetValue<StringList>().SelectedIndex.Equals(5) && !_menu.Item("packetobjknown").GetValue<bool>())))
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("-------------------------------------------------------------------------------");
                    Console.WriteLine(" ");
                    Console.WriteLine("Header: " + header + " - Lenght: " + LastLenght + " - NetworkId: " + BitConverter.ToInt32(args.PacketData, 2));
                    WritePacket = true;
                }

                #endregion

                #region Write PacketData

                if (_menu.Item("packetdata").GetValue<bool>() && WritePacket)
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("-----------");
                    for (int d = 0; d <= 128; d += 4)
                    {
                        try
                        {
                            Console.WriteLine((BitConverter.ToString(args.PacketData, d)).Substring(0, 11));
                        }
                        catch (Exception)
                        {
                            try
                            {
                                if (BitConverter.ToString(args.PacketData, d).Length > 0)
                                {
                                    Console.WriteLine((BitConverter.ToString(args.PacketData, d)).Substring(0, BitConverter.ToString(args.PacketData, d).Length));
                                }
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                            break;
                        }
                    }
                    Console.WriteLine("-----------");
                }
                # endregion
            }
        }

        private static void OnDraw(EventArgs args)
        {
            foreach (var obj in ObjCloseToMouse)
            {
                if (!obj.IsValid)
                {
                    continue;
                }
                if (obj is Obj_AI_Hero && !_menu.Item("obj1").GetValue<bool>())
                {
                    continue;
                }
                if (obj is Obj_AI_Minion && !_menu.Item("obj2").GetValue<bool>())
                {
                    continue;
                }
                if (obj is Obj_AI_Turret && !_menu.Item("obj3").GetValue<bool>())
                {
                    continue;
                }

                var X = Drawing.WorldToScreen(obj.Position).X;
                var Y = Drawing.WorldToScreen(obj.Position).Y;

                int i = 1;
                foreach (var packet in Packets.Where(packet => packet[0] == obj.NetworkId))
                {
                    if (packet[1] <= _menu.Item("packetamount").GetValue<Slider>().Value)
                    {
                        try
                        {
                            Drawing.DrawText(X, Y - (10 * i), Color.Red, "Packet" + packet[1] + " -  Header: " + packet[2] + " - Lenght: " + packet[3]);
                        }
                        catch (Exception)
                        {
                            
                            //ignored
                        }
                        
                        i++;
                    }
                    
                }
            }
        }

        static void LoadMenu()
        {
            //Start Menu
            _menu = new Menu("Header Finder", "Header Finder", true);

            String[] find = { "OnCreateGromp", "OnMissileHit", "OnAttack", "OnDisengaged", "OnMonsterSkill", "Custom" };
            _menu.AddItem(new MenuItem("finding", "Finding Header").SetValue(new StringList(find,5)));

            //Custom Header
            _menu.AddSubMenu(new Menu("Custom Header", "Custom Header"));
            _menu.SubMenu("Custom Header").AddItem(new MenuItem("customHeader1", "From Header").SetValue(new Slider(1,1,450)));
            _menu.SubMenu("Custom Header").AddItem(new MenuItem("customHeader2", "To Header").SetValue(new Slider(450, 1, 450)));
            _menu.SubMenu("Custom Header").AddItem(new MenuItem("customLenght1", "From Lenght").SetValue(new Slider(1, 1, 450)));
            _menu.SubMenu("Custom Header").AddItem(new MenuItem("customLenght2", "To Lenght").SetValue(new Slider(450, 1, 450)));
            
            //Objects
            _menu.AddSubMenu(new Menu("Objects", "Objects"));
            _menu.SubMenu("Objects").AddItem(new MenuItem("getobject", "Get Object").SetValue(true));
            _menu.SubMenu("Objects").AddItem(new MenuItem("obj1", "Obj_AI_Hero").SetValue(false));
            _menu.SubMenu("Objects").AddItem(new MenuItem("obj2", "Obj_AI_Minion").SetValue(true));
            _menu.SubMenu("Objects").AddItem(new MenuItem("obj3", "Obj_AI_Turret").SetValue(false));

            //Misc
            _menu.AddItem(new MenuItem("packetobjknown", "Only get packets with known Objects").SetValue(false));
            _menu.AddItem(new MenuItem("packetobjmouse", "Only print Objects close to mouse (Console)").SetValue(false));
            _menu.AddItem(new MenuItem("packetdata", "Write Packet Data(Console)").SetValue(false));
            _menu.AddItem(new MenuItem("headersblock", "Add headers to block list").SetValue(false));
            _menu.AddItem(new MenuItem("packetamount", "Amount of packets to draw").SetValue(new Slider(3, 0, 10)));
            _menu.AddItem(new MenuItem("cursorrange", "Max object dist from cursor").SetValue(new Slider(300, 100, 1000)));

            _menu.AddToMainMenu();
        }
    }
}
