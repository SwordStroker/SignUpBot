﻿using PugLifeSignUpBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PugLifeSignUpBot.Classes
{
    [System.Serializable]
    public class Raid
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int RaidMaxLimit { get; set; }
        public int MinimumEqItemLevel { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public ulong CreatorDiscordId { get; set; }
        public List<RaidMember> MemberList { get; set; }

        public string GetTankCount()
        {
            return MemberList.Where(m => m.Spec == Const.SPEC_TANK).Count().ToString();
        }

        public string GetMDpsCount()
        {
            return MemberList.Where(m => m.Spec == Const.SPEC_MDPS).Count().ToString();
        }

        public string GetRDpsCount()
        {
            return MemberList.Where(m => m.Spec == Const.SPEC_RDPS).Count().ToString();
        }

        public string GetHealerCount()
        {
            return MemberList.Where(m => m.Spec == Const.SPEC_HEALER).Count().ToString();
        }
    }
}
