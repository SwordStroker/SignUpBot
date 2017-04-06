using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowDotNetAPI;
using WowDotNetAPI.Models;

namespace PugLifeSignUpBot.Classes
{
    public class RaidMember
    {
        public string Name { get; set; }
        public string Realm { get; set; }
        public int EqItemLevel { get; set; }
        public string Spec { get; set; }
        public CharacterClass CharacterClass { get; set; }
        public ulong DiscordId { get; set; }

        public RaidMember(string name,string realm,string spec,ulong discordId)
        {
            Character character =  WowHandler.GetCharacter(name, realm, CharacterOptions.GetItems | CharacterOptions.GetTalents);
            Name = character.Name;
            Realm = character.Realm;
            EqItemLevel = character.Items.AverageItemLevelEquipped;
            Spec = spec;
            CharacterClass = character.Class;
            DiscordId = discordId;
        }

        public RaidMember()
        {

        }
    }
}
