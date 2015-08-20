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
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace GodJungleTracker.Classes
{
    public class Packets
    {
        public Packets() { }

        public class OnAttack
        {
            public OnAttack(int header = 0, int lenght = 71)
            {
                Lenght = lenght;
                Header = header;
            }
            public int Header { get; set; }
            public int Lenght { get; set; }
        }

        public class OnMissileHit
        {
            public OnMissileHit(int header = 0, int lenght = 35)
            {
                Lenght = lenght;
                Header = header;
            }
            public int Header { get; set; }
            public int Lenght { get; set; }
        }

        public class OnDisengaged
        {
            public OnDisengaged(int header = 0, int lenght = 68)
            {
                Lenght = lenght;
                Header = header;
            }
            public int Header { get; set; }
            public int Lenght { get; set; }
        }

        public class OnMonsterSkill
        {
            public OnMonsterSkill(int header = 0, int lenght = 47, int lenght2 = 68)
            {
                Lenght = lenght;
                Lenght2 = lenght2;
                Header = header;
            }
            public int Header { get; set; }
            public int Lenght { get; set; }
            public int Lenght2 { get; set; }
        }

        public class OnCreateGromp
        {
            public OnCreateGromp(int header = 0, int lenght = 302, int lenght2 = 311)
            {
                Lenght = lenght;
                Lenght2 = lenght2;
                Header = header;
            }
            public int Header { get; set; }
            public int Lenght { get; set; }
            public int Lenght2 { get; set; }
        }

        public class OnCreateCampIcon
        {
            public OnCreateCampIcon(int header = 0, int lenght = 74, int lenght2 = 86, int lenght3 = 83, int lenght4 = 62, int lenght5 = 71)
            {
                Lenght = lenght;
                Lenght2 = lenght2;
                Lenght3 = lenght3;
                Lenght4 = lenght4;
                Lenght5 = lenght5;
                Header = header;
            }
            public int Header { get; set; }
            public int Lenght { get; set; }
            public int Lenght2 { get; set; }
            public int Lenght3 { get; set; }
            public int Lenght4 { get; set; }
            public int Lenght5 { get; set; }
        }
    }
}
