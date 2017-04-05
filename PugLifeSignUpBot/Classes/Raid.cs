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
        public List<RaidMember> MemberList { get; set; }

        public string GetTankCount()
        {
            return MemberList.Where(m => m.Spec == "TANK").Count().ToString();
        }

        public string GetDpsCount()
        {
            return MemberList.Where(m => m.Spec == "DPS").Count().ToString();
        }

        public string GetHealerCount()
        {
            return MemberList.Where(m => m.Spec == "HEALING").Count().ToString();
        }
    }
}
