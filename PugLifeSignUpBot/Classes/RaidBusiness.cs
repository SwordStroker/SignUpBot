using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WowDotNetAPI.Models;

namespace PugLifeSignUpBot.Classes
{
    public abstract class RaidBusiness
    {
        public static string raidFolderPathString = Directory.GetCurrentDirectory() + "\\Raids";
        public static string raidTextPathString = raidFolderPathString + "\\Raids.txt";

        public static List<Raid> raidList;

        public static void AddRaid(string raidName, string raidDate, string raidTime, string name, string realm, string spec, int minimumEqItemLevel, string description, ulong discordId)
        {
            Raid raid = new Raid()
            {
                Name = raidName,
                Description = description,
                Date = raidDate,
                Time = raidTime,
                MinimumEqItemLevel = minimumEqItemLevel,
                MemberList = new List<RaidMember>() { new RaidMember(name, realm, spec, discordId) }
            };

            raidList.Add(raid);
            SaveRaids();
        }

        public static string AddRaidMember(string raidName, string characterName, string realm, string spec, ulong discordId)
        {
            Raid raid = raidList.Find(r => r.Name == raidName);
            if (raid == null)
                return PrintMessage("No raid named " + raidName + " found!");
            RaidMember raidMember = raid.MemberList.Find(r => r.Name == CapitalizeFirstLetter(characterName));
            if (raidMember != null)
                return PrintMessage("You are already added to this raid.");
            Character character = WowHandler.GetCharacter(characterName, realm, WowDotNetAPI.CharacterOptions.GetTalents | WowDotNetAPI.CharacterOptions.GetItems);
            if (character.Items.AverageItemLevelEquipped < raid.MinimumEqItemLevel)
                return PrintMessage(string.Format("Your item eq level is {0}, raid's minimum is {1}..", character.Items.AverageItemLevelEquipped, raid.MinimumEqItemLevel));
            raid.MemberList.Add(new RaidMember() { Name = character.Name, CharacterClass = character.Class, Realm = character.Realm, EqItemLevel = character.Items.AverageItemLevelEquipped, Spec = spec, DiscordId = discordId });
            SaveRaids();
            return PrintMessage(characterName + " is successfully added to " + raid.Name);
        }

        public static string DeleteRaidMember(string raidName, string characterName, string realm, ulong discordId)
        {
            Raid raid = raidList.Find(r => r.Name == raidName);
            if (raid == null)
                return PrintMessage("No raid named " + raidName + " found!");
            RaidMember raidMember = raid.MemberList.Find(r => r.Name == CapitalizeFirstLetter(characterName) && r.Realm == CapitalizeFirstLetter(realm));
            if (raidMember == null)
                return PrintMessage(string.Format("There is no player named {0} is signed up to raid", CapitalizeFirstLetter(characterName)));
            if (raidMember.DiscordId != discordId)
                return PrintMessage(string.Format("You are not allowed to cancel other ppls signup"));

            raid.MemberList.Remove(raidMember);
            SaveRaids();
            return PrintMessage(characterName + " is successfully canceled the " + raid.Name + " raid");
        }

        public static string ShowRaid(string raidName)
        {
            Raid raid = raidList.Find(f => f.Name == raidName);
            if (raid == null) return "No raid named " + raidName + " found";

            string[] columns = { "Name", "Realm", "ILvl", "Spec", "Class" };
            List<Tuple<string, string, int, string, string>> val = new List<Tuple<string, string, int, string, string>>();

            foreach (var raidMember in raid.MemberList)
            {
                val.Add(new Tuple<string, string, int, string, string>(raidMember.Name, raidMember.Realm, raidMember.EqItemLevel, raidMember.Spec, raidMember.CharacterClass.ToString()));
            }

            return val.ToStringTable(columns, a => a.Item1, a => a.Item2, a => a.Item3, a => a.Item4, a => a.Item5);
        }

        public static void DirectoryCheck()
        {
            if (!Directory.Exists(raidFolderPathString))
            {
                Directory.CreateDirectory(raidFolderPathString);
                File.CreateText(raidTextPathString);
                raidList = new List<Raid>();
            }
            else
            {
                string[] files = Directory.GetFiles(raidFolderPathString);
                string raids = File.ReadAllText(raidTextPathString);
                raidList = JsonConvert.DeserializeObject<List<Raid>>(raids);
            }
        }

        public static string ShowAllRaids()
        {
            List<Tuple<string, string, string, int, string, string, string>> val = new List<Tuple<string, string, string, int, string, string, string>>();

            string[] columns = { "RaidName", "Date", "Time", "MinEqILvl", "DPS", "Tank", "Healer" };
            foreach (var raid in raidList)
            {
                val.Add(new Tuple<string, string, string, int, string, string, string>(raid.Name, raid.Date, raid.Time, raid.MinimumEqItemLevel, raid.GetDpsCount(), raid.GetTankCount(), raid.GetHealerCount()));
            }
            return val.ToStringTable(columns, a => a.Item1, a => a.Item2, a => a.Item3, a => a.Item4, a => a.Item5, a => a.Item6, a => a.Item7);
        }

        private static void SaveRaids()
        {
            File.WriteAllText(raidTextPathString, JsonConvert.SerializeObject(raidList));
        }

        private static string PrintMessage(string message)
        {
            return "`" + message + "`";
        }

        private static string CapitalizeFirstLetter(string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            if (s.Length == 1)
                return s.ToUpper();
            return s.Remove(1).ToUpper() + s.Substring(1);
        }

        public static string ChangeDateOfRaid(string raidName,string newDate,ulong discordId)
        {
            Raid raid = raidList.Find(r => r.Name == raidName);
            string oldDate = raid.Date;
            if (raid == null)
                return PrintMessage("No raid named " + raidName + " found!");

            if (raid.MemberList[0].DiscordId != discordId)
                return PrintMessage("You are not allowed to change the date.");

            raid.Date = newDate;
            return PrintMessage(string.Format("{0} raid date is changed to {0} from {1}", newDate, oldDate));
        }

        public static string ChangeTimeOfRaid(string raidName,string newTime,ulong discordId)
        {
            Raid raid = raidList.Find(r => r.Name == raidName);
            string oldTime = raid.Time;
            if (raid == null)
                return PrintMessage("No raid named " + raidName + " found!");

            if (raid.MemberList[0].DiscordId != discordId)
                return PrintMessage("You are not allowed to change the date.");

            raid.Date = newTime;
            return PrintMessage(string.Format("{0} raid date is changed to {0} from {1}", newTime, oldTime));
        }

        public static string CheckGuldanAchi(string characterName,string realm)
        {
            return WowHandler.CheckGuldanCurveAchievement(characterName, realm) ? "He has it" : "He doesnt";
        }
    }
}
