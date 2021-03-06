﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WowDotNetAPI.Models;
using PugLifeSignUpBot.Handlers;
using PugLifeSignUpBot.Utility;

namespace PugLifeSignUpBot.Classes
{
    public abstract class RaidBusiness
    {
        public static List<Raid> raidList;

        public static int AddRaid(string raidName, string raidDate, string raidTime, string name, string realm, string spec, int minimumEqItemLevel, string description, ulong discordId)
        {
            if (CheckRaid(raidName)) return -1;
            if (!Const.Specs.Contains(spec)) return -2;
            Raid raid = new Raid()
            {
                Name = raidName,
                Description = description,
                Date = raidDate,
                Time = raidTime,
                MinimumEqItemLevel = minimumEqItemLevel,
                CreatorDiscordId = discordId,
                MemberList = new List<RaidMember>() { new RaidMember(name, realm, spec, discordId) }
            };

            raidList.Add(raid);
            SaveRaids();
            return 0;
        }

        public static string AddRaidMember(string raidName, string characterName, string realm, string spec, ulong discordId)
        {
            Raid raid = raidList.Find(r => r.Name == raidName);
            if (raid == null)
                return PrintMessage("No raid named " + raidName + " found!");
            RaidMember raidMember = raid.MemberList.Find(r => r.Name == CapitalizeFirstLetter(characterName));
            if (raidMember != null)
                return PrintMessage("You are already added to this raid.");
            if (!Const.Specs.Contains(spec.ToLower()))
                return PrintMessage("Please specify your role correctly.{Tank,MDps,RDps,Healer}");
            Character character = WowHandler.GetCharacter(characterName, realm, WowDotNetAPI.CharacterOptions.GetTalents | WowDotNetAPI.CharacterOptions.GetItems);
            if (character.Items.AverageItemLevelEquipped < raid.MinimumEqItemLevel)
                return PrintMessage(string.Format("Your item eq level is {0}, raid's minimum is {1}..", character.Items.AverageItemLevelEquipped, raid.MinimumEqItemLevel));
            raid.MemberList.Add(new RaidMember() { Name = character.Name, CharacterClass = character.Class, Realm = character.Realm, EqItemLevel = character.Items.AverageItemLevelEquipped, Spec = spec.ToLower(), DiscordId = discordId });
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
            return PrintMessage(characterName + " is successfully removed from " + raid.Name + " raid");
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

        public static string ShowAllRaids()
        {
            List<Tuple<string, string, string, int, string, string, string, Tuple<string>>> val = new List<Tuple<string, string, string, int, string, string, string, Tuple<string>>>();

            string[] columns = { "RaidName", "Date", "Time", "MinEqILvl", "Tank", "MDps", "RDps", "Healer" };
            foreach (var raid in raidList)
            {
                val.Add(new Tuple<string, string, string, int, string, string, string, Tuple<string>>(raid.Name, raid.Date, raid.Time, raid.MinimumEqItemLevel, raid.GetTankCount(), raid.GetMDpsCount(), raid.GetRDpsCount(), new Tuple<string>(raid.GetHealerCount())));
            }
            return val.ToStringTable(columns, a => a.Item1, a => a.Item2, a => a.Item3, a => a.Item4, a => a.Item5, a => a.Item6, a => a.Item7, a => a.Rest.Item1);
        }

        private static void SaveRaids()
        {
            DirectoryHandler.SaveRaids();
        }

        public static string PrintMessage(string message)
        {
            return "`" + message + "`";
        }

        private static string CapitalizeFirstLetter(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            if (s.Length == 1)
                return s.ToUpper();
            return s.Remove(1).ToUpper() + s.Substring(1);
        }

        public static string ChangeDateOfRaid(string raidName, string newDate, ulong discordId)
        {
            Raid raid = raidList.Find(r => r.Name == raidName);
            string oldDate = raid.Date;
            if (raid == null)
                return PrintMessage("No raid named " + raidName + " found!");

            if (raid.CreatorDiscordId != discordId)
                return PrintMessage("You are not allowed to change the date.");

            raid.Date = newDate;
            SaveRaids();
            return PrintMessage(string.Format("{0} raid date is changed to {0} from {1}", newDate, oldDate));
        }

        public static string ChangeTimeOfRaid(string raidName, string newTime, ulong discordId)
        {
            Raid raid = raidList.Find(r => r.Name == raidName);
            string oldTime = raid.Time;
            if (raid == null)
                return PrintMessage("No raid named " + raidName + " found!");

            if (raid.CreatorDiscordId != discordId)
                return PrintMessage("You are not allowed to change the date.");

            raid.Date = newTime;
            SaveRaids();
            return PrintMessage(string.Format("{0} raid date is changed to {0} from {1}", newTime, oldTime));
        }

        public static string ChangeMinimumILvlOfRaid(string raidName, int newILvl, ulong discordId)
        {
            Raid raid = raidList.Find(r => r.Name == raidName);
            int oldILvl = raid.MinimumEqItemLevel;
            if (raid == null)
                return PrintMessage("No raid named " + raidName + " found!");

            if (raid.CreatorDiscordId != discordId)
                return PrintMessage("You are not allowed to change the eq ilvl.");

            raid.MinimumEqItemLevel = newILvl;
            SaveRaids();
            return PrintMessage(string.Format("{0} raid Minimum Eq ILvl is changed to {0} from {1}", newILvl, oldILvl));
        }

        internal static bool CheckRaid(string raidName)
        {
            Raid raid = raidList.Find(r => r.Name == raidName);
            return raid != null;
        }

        internal static int CheckRaidAndPermission(string raidName, ulong discordId)
        {
            Raid raid = raidList.Find(r => r.Name == raidName);
            if (raid == null) return -1;
            if (raid.CreatorDiscordId != discordId) return -2;
            return 0;
        }

        public static string CheckGuldanAchi(string characterName, string realm)
        {
            return WowHandler.CheckGuldanCurveAchievement(characterName, realm) ? "He has it" : "He doesnt";
        }

        public static string CancelRaid(string raidName, ulong discordId, string userName)
        {
            Raid raid = raidList.Find(r => r.Name == raidName);
            if (raid == null)
                return PrintMessage("No raid named " + raidName + " found!");
            if (raid.CreatorDiscordId != discordId)
                return PrintMessage("You are not allowed to cancel the raid.");

            raidList.Remove(raid);
            SaveRaids();
            return PrintMessage(string.Format("{0} has been succesfully canceled by {1}", raidName, userName));
        }

        public static Raid GetRaid(string raidName)
        {
            return raidList.Find(r => r.Name == raidName);
        }

        public static string ShowMyRaids(string characterName,string realm)
        {
            string[] columns = { "RaidName", "Date", "Time", "Spec" };

            List<Tuple<string, string, string, string>> val = new List<Tuple<string, string, string, string>>();
            foreach (var raid in raidList)
            {
                RaidMember member = raid.MemberList.Find(m => m.Name == characterName && m.Realm == realm);
                if (member != null)
                    val.Add(new Tuple<string, string, string, string>(raid.Name, raid.Date, raid.Time, member.Spec));
            }
            return val.Count <= 0 ? "You are not signed to any raid" : val.ToStringTable(columns, a => a.Item1, a => a.Item2, a => a.Item3, a => a.Item4);
        }
    }
}
