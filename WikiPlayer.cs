using System;
using Terraria;
using Terraria.ModLoader;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;
using CalValEX;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using CalamityMod.Rarities;
using System.Linq;
using CalValEX.NPCs.Oracle;
using CalValEX.NPCs.JellyPriest;
using CalValEX.AprilFools.Jharim;
using System.Reflection;
using Terraria.ObjectData;
using Terraria.Map;
using Microsoft.Xna.Framework.Graphics;
using CalValEX.Items.Equips.Shirts;
using MonoMod.Core.Platforms;
using Humanizer;
using CalValEX.Items.Mounts;
using CalamityMod.Items;
using Terraria.GameContent.Bestiary;
using Steamworks;
using Terraria.Localization;
using Newtonsoft.Json.Linq;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.Astral;
using CalValEX.Items.Tiles.Statues;
using CalamityMod;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.Mime.MediaTypeNames;
using CalamityMod.NPCs.Crags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs.Yharon;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Tiles.Crags;
using CalValEX.Tiles.Blocks;
using CalamityMod.Tiles.Astral;
using CalValEX.Tiles.AstralBlocks;
using Microsoft.CodeAnalysis.CSharp;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.DraedonStructures;

namespace CalValWiki
{
    public class CalamityBestiaryPage : ModCommand
    {
        public override string Command => "CalamityBestiaryPage";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            WikiPlayer.BestiaryPage();
        }

        public override CommandType Type => CommandType.World;
    }

    public class WikiPlayer : ModPlayer
    {
        internal static string exportPath = Path.Combine(Main.SavePath + "/Data Dumps");
        internal static string importPath = Path.Combine(Main.SavePath, "CalValWikiImport.txt");

        public static bool InUI = false;

        public static string ModName => "Calamity Community Remix";
        public static Mod CurMod => ModLoader.GetMod("CalRemix");
        public override bool HoverSlot(Item[] inventory, int context, int slot)
        {
            string path;
            List<string> list = new List<string>();
            if (Player.controlHook)
            {
                path = $@"{exportPath}\CalValWikiMasterExport.txt";
                list.Add(path);
                File.WriteAllText(path, GenerateAllItemPages(), Encoding.UTF8);
                Main.NewText("Exported!");
            }
                //MakeCalamityProjectilePage();
                //MakeCalamityNPCPage();
            //MakeCalamityItemPage();
            //CalValInteractions(inventory, slot);
            return false;
        }

        public static void ExportPage(Func<string> function)
        {
            string path;
            List<string> list = new List<string>();
            path = $@"{exportPath}\CalValWikiMasterExport.txt";
            list.Add(path);
            File.WriteAllText(path, function.Invoke(), Encoding.UTF8);
            Main.NewText("Exported!");
        }

        public static string GenerateAllItemPages()
        {
            string ret = "";
            foreach (var item in ContentSamples.ItemsByType)
            {
                if (item.Value.ModItem != null)
                {
                    if (item.Value.ModItem.Mod == CurMod && !item.Value.Name.Contains(" Stone"))
                    {
                        ret += ExportItemInfo(item.Value) + "\n\n\n\n";
                    }
                }
            }
            return ret;

        }

        /// <summary>
        /// Dumps out the Calamity Bestiary page. Several entries are hardcoded and require maintenance 
        /// </summary>
        public static void BestiaryPage()
        {

            string path;
            List<string> list = new List<string>();
            List<NPC> npcs = new List<NPC>();
            List<string> Names = new List<string>();
            path = $@"{exportPath}\Bestiary.txt";
            list.Add(path);
            string ret = "{| class=\"terraria lined sortable\" style=\"margin:auto;\"\n! Entity\r\n! Stars\n! Filters\n! Description\n|-\n";
            int key = -1;
            for (int i = 0; i < ContentSamples.NpcsByNetId.Count; i++)
            {
                if (!ContentSamples.NpcsByNetId.ContainsKey(i))
                {
                    Names.Add("");
                    continue;
                }
                NPC n = ContentSamples.NpcsByNetId[i];
                bool hasThe = n.FullName.Contains("The ");
                string noThe = hasThe ? n.FullName.Remove(0, 4) : n.FullName;
                if (n.FullName == "Ebonian Paladin")
                {
                    noThe = "Slime God2";
                }
                Names.Add(noThe);
                if (n == null)
                    continue;
                if (n.ModNPC == null)
                    continue;
                if (n.ModNPC.Mod != ModLoader.GetMod("CalamityMod"))
                    continue;
                if (n.type == ModContent.NPCType<HiveBlob>() || n.type == ModContent.NPCType<DankCreeper>() || n.type == ModContent.NPCType<DarkHeart>() || n.type == ModContent.NPCType<PhantomSpiritL>() 
                    || n.type == ModContent.NPCType<PhantomSpiritS>()
                    || n.type == ModContent.NPCType<PhantomSpiritM>()
                    || n.type == ModContent.NPCType<PlagueChargerLarge>()
                    || n.type == ModContent.NPCType<CrimulanPaladin>()
                    || n.type == ModContent.NPCType<SplitCrimulanPaladin>()
                    || n.type == ModContent.NPCType<PerforatorHeadSmall>()
                    || n.type == ModContent.NPCType<PerforatorHeadLarge>())
                    continue;
                npcs.Add(n);
            }

            npcs.Sort((x, y) => Names[x.type].CompareTo(Names[y.type]));
            List<BestiaryEntry> entries = Main.BestiaryDB.GetBestiaryEntriesByMod(ModLoader.GetMod("CalamityMod"));
            for (int i = 0; i < npcs.Count; i++)
            {
                NPC n = npcs[i];
                NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(n.type, out var value);
                if (value.Hide)
                    continue;

                BestiaryEntry bE = Main.BestiaryDB.FindEntryByNPCID(n.type);
                string name = "[[" + n.FullName + "]]";
                if (n.type == ModContent.NPCType<SplitEbonianPaladin>())
                {
                    name = "[[The Slime God]] (Paladins)";
                }
                if (n.type == ModContent.NPCType<SlimeGodCore>())
                {
                    name = "[[The Slime God]] (core)";
                }
                if (n.type == ModContent.NPCType<CrimsonSlimeSpawn2>())
                {
                    name = "[[Crimson Slime Spawn (spiked)]]";
                }
                if (n.type == ModContent.NPCType<SoulSeekerSupreme>())
                {
                    name = "[[Soul Seeker]] (supreme)";
                }
                if (n.type == ModContent.NPCType<PerforatorHeadMedium>())
                {
                    name = "[[The Perforators]]";
                }
                if (n.type == ModContent.NPCType<HiveEnemy>())
                {
                    name = "[[Hive (enemy)]]";
                }
                ret += "| " + name + "\n";

                string rarity = ContentSamples.NpcBestiaryRarityStars[n.type].ToString();
                if (n.type == ModContent.NPCType<PhantomSpirit>())
                {
                    rarity = "2 (Normal and Sad)<br/>3 (Angry and Happy)";
                }
                if (n.type == ModContent.NPCType<PerforatorHeadMedium>())
                {
                    rarity = "2 (Small and Medium)<br/>3 (Large)";
                }

                ret += "| " + rarity + "\n| ";

                bool comma = false;
                for (int j = 0; j < bE.Info.Count; j++)
                {
                    IBestiaryInfoElement element = bE.Info[j];
                    string commaText = comma ? ", " : "";
                    if (element is Terraria.GameContent.Bestiary.SpawnConditionBestiaryInfoElement biome)
                    {
                        ret += commaText;
                        FilterProviderInfoElement filterer = element as FilterProviderInfoElement;
                        ret += Language.GetText(filterer.GetDisplayNameKey()).Value;
                        comma = true;
                    }
                    if (element is Terraria.GameContent.Bestiary.SpawnConditionBestiaryOverlayInfoElement evente)
                    {
                        ret += commaText;
                        FilterProviderInfoElement filterer = element as FilterProviderInfoElement;
                        ret += Language.GetText(filterer.GetDisplayNameKey()).Value;
                        comma = true;
                    }
                    if (element is ModBiomeBestiaryInfoElement modbiome)
                    {
                        ret += commaText;
                        ret += Language.GetText(modbiome.GetDisplayNameKey()).Value;
                        comma = true;
                    }
                    if (element is BossBestiaryInfoElement)
                    {
                        ret += commaText;
                        ret += "Boss Enemy";
                        comma = true;
                    }
                    if (element is RareSpawnBestiaryInfoElement)
                    {
                        ret += commaText;
                        ret += "Rare Creature";
                        comma = true;
                    }
                }
                if (!comma)
                {
                    ret += "{{na}}";
                }

                string codeName = n.FullName;
                if (n.type == ModContent.NPCType<PerforatorHeadMedium>())
                {
                    codeName = "The Perforators";
                }
                if (n.type == ModContent.NPCType<CrimsonSlimeSpawn2>())
                {
                    codeName = "Crimson Slime Spawn (spiked)";
                }

                ret += "\n| {{#dplvar:_bestiary:" + codeName + "}}\n|-\n";
            }
            ret += "|}";
            File.WriteAllText(path, ret, Encoding.UTF8);
            Main.NewText("Exported");
        }


        /// <summary>
        /// Dumps out a list of wings not in the Any Wings recipe group
        /// </summary>
        public static void WingItems()
        {
            string path;
            List<string> list = new List<string>();
            List<Item> items = new List<Item>();
            List<Item> existingItems = new List<Item>();
            path = $@"{exportPath}\Wings.txt";
            list.Add(path);
            string ret = "Missing Wings:";
            int key = -1;
            foreach (var r in RecipeGroup.recipeGroups)
            {
                if (r.Value.ContainsItem(ItemID.AngelWings))
                {
                    key = r.Key;
                    break;
                }
            }
            foreach (int i in RecipeGroup.recipeGroups[key].ValidItems)
            {
                existingItems.Add(ContentSamples.ItemsByType[i]);
            }
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if ((item.wingSlot <= 0) || existingItems.Contains(item) || (item.ModItem != null && item.ModItem.Mod != ModLoader.GetMod("CalamityMod")) || item.shoeSlot > 0)
                    continue;
                items.Add(item);
            }

            for (int i = 0; i < items.Count; i++)
            {
                ret += "\n" + items[i].Name;
            }
            File.WriteAllText(path, ret, Encoding.UTF8);
            Main.NewText("Exported");
        }

        /// <summary>
        /// Dumps out a list of food items not in the Any Food recipe group
        /// </summary>
        public static void FoodItems()
        {
            string path;
            List<string> list = new List<string>();
            List<Item> items = new List<Item>();
            List<Item> existingItems = new List<Item>();
            path = $@"{exportPath}\FoodItems.txt";
            list.Add(path);
            string ret = "Missing Food items:";
            int key = -1;
            foreach (var r in RecipeGroup.recipeGroups)
            {
                if (r.Value.ContainsItem(ItemID.BananaSplit))
                {
                    key = r.Key;
                    break;
                }
            }
            foreach (int i in RecipeGroup.recipeGroups[key].ValidItems)
            {
                existingItems.Add(ContentSamples.ItemsByType[i]);
            }
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if ((item.buffType != BuffID.WellFed && item.buffType != BuffID.WellFed2 && item.buffType != BuffID.WellFed3) || existingItems.Contains(item) || (item.ModItem != null && item.ModItem.Mod != ModLoader.GetMod("CalamityMod")))
                    continue;
                items.Add(item);
            }

            for (int i = 0; i < items.Count; i++)
            {
                ret += "\n" + items[i].Name;
            }
            File.WriteAllText(path, ret, Encoding.UTF8);
            Main.NewText("Exported");
        }

        /// <summary>
        /// Dumps out a list of Calamity items with the cyan rarity
        /// </summary>
        public static void CyanRarity()
        {
            string path;
            List<string> list = new List<string>();
            List<Item> items = new List<Item>();
            path = $@"{exportPath}\CyanCalamityItems.txt";
            list.Add(path);
            string ret = "Cyan rarity Calamity items:";
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (item.rare != ItemRarityID.Cyan)
                    continue;
                if (item.ModItem != null)
                    if (item.ModItem.Mod == ModLoader.GetMod("CalamityMod"))
                        items.Add(item);
            }

            for (int i = 0; i < items.Count; i++)
            {
                ret += "\n" + items[i].Name;
            }
            File.WriteAllText(path, ret, Encoding.UTF8);
            Main.NewText("Exported");
        }

        /// <summary>
        /// Dumps out Calamity items with sell prices not matching their rarity
        /// </summary>
        public static void InconsistentPrices()
        {
            string path;
            List<string> list = new List<string>();
            List<Item> items = new List<Item>();
            path = $@"{exportPath}\CalamityRarities.txt";
            list.Add(path);

            string ret = "";
            ret += "Current Turquoise rarity: " + ModContent.RarityType<Turquoise>() + "\n";
            ret += "Current Pure Green rarity: " + ModContent.RarityType<PureGreen>() + "\n";
            ret += "Current Dark Blue rarity: " + ModContent.RarityType<DarkBlue>() + "\n";
            ret += "Current Violet rarity: " + ModContent.RarityType<Violet>() + "\n";
            ret += "Current Hot Pink rarity: " + ModContent.RarityType<HotPink>() + "\n";
            ret += "Current Calamity Red rarity: " + ModContent.RarityType<CalamityRed>() + "\n";
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (item.rare == ItemRarityID.Master || item.rare == ItemRarityID.Expert)
                    continue;
                if (item.maxStack == 1)
                    continue;
                if (item.value == 0)
                    continue;
                if (item.damage < 1)
                    continue;
                if (item.ModItem != null)
                    if (item.ModItem.Mod == ModLoader.GetMod("CalamityMod"))
                        items.Add(item);
            }

            items.Sort((x, y) => ContentSamples.ItemsByType[x.type].rare.CompareTo(ContentSamples.ItemsByType[y.type].rare));
            for (int i = 0; i < items.Count; i++)
            {

                if (items[i].value == GetPrice(items[i].rare))
                    continue;
                ret += "\nItem name: " + items[i].Name + ", Rarity: " + items[i].rare + ", Price: " + items[i].value + ", Expected price: " + GetPrice(items[i].rare);
            }
            File.WriteAllText(path, ret, Encoding.UTF8);
            Main.NewText("Exported");
        }

        /// <summary>
        /// Grabs the intended price of an item based on its rarity
        /// </summary>
        /// <param name="rare">The rarity</param>
        /// <returns></returns>
        public static int GetPrice(int rare)
        {
            switch (rare)
            {
                case 0:
                    return 0;
                case 1:
                    return CalamityGlobalItem.RarityBlueBuyPrice;
                case 2:
                    return CalamityGlobalItem.RarityGreenBuyPrice;
                case 3:
                    return CalamityGlobalItem.RarityOrangeBuyPrice;
                case 4:
                    return CalamityGlobalItem.RarityLightRedBuyPrice;
                case 5:
                    return CalamityGlobalItem.RarityPinkBuyPrice;
                case 6:
                    return CalamityGlobalItem.RarityLightPurpleBuyPrice;
                case 7:
                    return CalamityGlobalItem.RarityLimeBuyPrice;
                case 8:
                    return CalamityGlobalItem.RarityYellowBuyPrice;
                case 9:
                    return CalamityGlobalItem.RarityCyanBuyPrice;
                case 10:
                    return CalamityGlobalItem.RarityRedBuyPrice;
                case 11:
                    return CalamityGlobalItem.RarityPurpleBuyPrice;
            }

            if (rare == ModContent.RarityType<Turquoise>())
            {
                return CalamityGlobalItem.RarityTurquoiseBuyPrice;
            }
            if (rare == ModContent.RarityType<PureGreen>())
            {
                return CalamityGlobalItem.RarityPureGreenBuyPrice;
            }
            if (rare == ModContent.RarityType<DarkBlue>())
            {
                return CalamityGlobalItem.RarityDarkBlueBuyPrice;
            }
            if (rare == ModContent.RarityType<Violet>())
            {
                return CalamityGlobalItem.RarityVioletBuyPrice;
            }
            if (rare == ModContent.RarityType<HotPink>())
            {
                return CalamityGlobalItem.RarityHotPinkBuyPrice;
            }
            if (rare == ModContent.RarityType<CalamityRed>())
            {
                return CalamityGlobalItem.RarityCalamityRedBuyPrice;
            }
            return 0;
        }

        /// <summary>
        /// Grabs a list of Calamity blocks with no journey mode support
        /// </summary>
        public static void PlaceableStacks()
        {
            string path;
            List<string> list = new List<string>();
            List<Item> items = new List<Item>();
            path = $@"{exportPath}\CalamityPlaceResearch.txt";
            list.Add(path);

            string ret = "";
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (item.ModItem != null)
                    if (item.ModItem.Mod == ModLoader.GetMod("CalamityMod"))
                if (item.ResearchUnlockCount == 1 && (item.createTile > 0 || item.createWall > 0))
                {
                    items.Add(item);
                }
            }
            for (int i = 0; i < items.Count; i++)
            {
                
                if (Main.tileFrameImportant[items[i].createTile])
                    continue;
                ret += "\n" + items[i].Name;
            }
            File.WriteAllText(path, ret, Encoding.UTF8);
            Main.NewText("Exported");
        }

        /// <summary>
        /// Creates a master list of Calamity projectiles, Wiki formatted
        /// </summary>
        public void MakeCalamityProjectilePage()
        {
            string path;
            List<string> list = new List<string>();
            List<Projectile> calProj = new List<Projectile>();
            path = $@"{exportPath}\CalamityProjectileDump.txt";
            list.Add(path);
            string ret = "{| class=\"terraria sortable lined\" style=\"text-align:center\"\r\n" +
                "! width=10% | Image\r\n" +
                "! width=20% | Display Name\r\n" +
                "! width=20% | Internal Name\r\n" +
                "! width=50% | Source\r\n" +
                "|-";
            for (int i = 0; i < ContentSamples.ProjectilesByType.Count; i++)
            {
                Projectile p = ContentSamples.ProjectilesByType[i];
                if (p.ModProjectile != null)
                {
                    if (p.ModProjectile.Mod == ModLoader.GetMod("CalamityMod"))
                    {
                        calProj.Add(p);
                    }
                }
            }
            Texture2D nothing = ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj").Value;
            calProj.Sort((x, y) => ProjectileLoader.GetProjectile(x.type).Name.CompareTo(ProjectileLoader.GetProjectile(y.type).Name));
            for (int i = 0; i < calProj.Count; i++)
            {
                Projectile p = calProj[i];
                bool invis = TextureAssets.Projectile[p.type].Value == nothing;
                int frameCount = Main.projFrames[p.type];
                string ext = frameCount > 1 ? ".gif" : ".png";
                ret += "\r\n";
                string invistext = invis ? "| {{na}}" : "| ";
                ret += invistext + " || " + p.Name + " || <code>" + ProjectileLoader.GetProjectile(p.type).Name + "</code>" + " || ";
                ret += "\r\n|-";
            }
            File.WriteAllText(path, ret, Encoding.UTF8);
        }

        /// <summary>
        /// Creates a list of every Calamity item, wiki formatted
        /// </summary>
        public static void MakeCalamityItemPage()
        {
            string path;
            List<string> list = new List<string>();
            List<Item> calProj = new List<Item>();
            path = $@"{exportPath}\CalamityItemDump.txt";
            list.Add(path);
            string ret = "{| class=\"terraria sortable lined\" style=\"text-align:center\"\r\n" +
                "! width=10% | Item\r\n" +
                "! width=20% | Internal Name\r\n" +
                "|-";
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item p = ContentSamples.ItemsByType[i];
                if (p.ModItem != null)
                {
                    if (p.ModItem.Mod == ModLoader.GetMod("CalamityMod"))
                    {
                        calProj.Add(p);
                    }
                }
            }
            calProj.Sort((x, y) => ItemLoader.GetItem(x.type).Name.CompareTo(ItemLoader.GetItem(y.type).Name));
            for (int i = 0; i < calProj.Count; i++)
            {
                Item p = calProj[i];
                string lore = "";
                string internalName = ItemLoader.GetItem(p.type).Name;
                if (internalName[0] == 'L' && internalName[1] == 'o' && internalName[2] == 'r' && internalName[3] == 'e')
                {
                    lore = " (Lore)";
                }
                switch (p.Name)
                {
                    case "Butcher":
                    case "Thunderstorm":
                    case "Sandstorm":
                        lore = " (weapon)";
                        break;
                    case "Elderberry":
                    case "Blood Orange":
                    case "Pineapple":
                        lore = " (calamity)";
                        break;
                    case "Trash Can":
                        lore = " (pet)";
                        break;
                    case "Purity":
                        lore = " (accessory)";
                        break;
                }
                if (internalName == "SlimeGodMask")
                {
                    lore = " (Corruption)";
                }
                if (internalName == "SlimeGodMask2")
                {
                    lore = " (Crimson)";
                }
                ret += "\r\n";
                ret += "| {{item|" + p.Name + lore + "}} || <code>" + ItemLoader.GetItem(p.type).Name + "</code>";
                ret += "\r\n|-";
            }
            File.WriteAllText(path, ret, Encoding.UTF8);
        }

        /// <summary>
        /// Creates a list of every Calamity NPC, wiki formatted
        /// </summary>
        public static void MakeCalamityNPCPage()
        {

            string path;
            List<string> list = new List<string>();
            List<NPC> calProj = new List<NPC>();
            path = $@"{exportPath}\CalamityNPCDump.txt";
            list.Add(path);
            string ret = "{| class=\"terraria sortable lined\" style=\"text-align:center\"\r\n" +
                "! width=10% | Item\r\n" +
                "! width=20% | Internal Name\r\n" +
                "|-";
            for (int i = 0; i < ContentSamples.NpcsByNetId.Count; i++)
            {
                if (ContentSamples.NpcsByNetId.ContainsKey(i))
                {
                    NPC p = ContentSamples.NpcsByNetId[i];
                    if (p.ModNPC != null)
                    {
                        if (p.ModNPC.Mod == ModLoader.GetMod("CalamityMod"))
                        {
                            calProj.Add(p);
                        }
                    }
                }
            }
            calProj.Sort((x, y) => NPCLoader.GetNPC(x.type).Name.CompareTo(NPCLoader.GetNPC(y.type).Name));
            for (int i = 0; i < calProj.Count; i++)
            {
                NPC p = calProj[i];
                string lore = "";
                string internalName = NPCLoader.GetNPC(p.type).Name;
                switch (p.FullName)
                {
                    case "Hive":
                    case "Crown Jewel":
                        lore = " (enemy)";
                        break;
                    case "Demon":
                        lore = " (Indignant)";
                        break;
                    case "Tooth Ball":
                        lore = " (Old Duke)";
                        break;
                }
                if (internalName.Contains("Head"))
                {
                    lore = " Head";
                }
                if (internalName.Contains("Body"))
                {
                    if (internalName != "AresBody")
                        lore = " Body";
                }
                if (internalName.Contains("Tail"))
                {
                    lore = " Tail";
                }
                ret += "\r\n";
                ret += "| {{item|" + p.FullName + lore + "}} || <code>" + NPCLoader.GetNPC(p.type).Name + "</code>";
                ret += "\r\n|-";
            }
            File.WriteAllText(path, ret, Encoding.UTF8);
        }

        /// <summary>
        /// Various Calamity's Vanities page dumps
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="slot"></param>
        public void CalValInteractions(Item[] inventory, int slot)
        {
            if (inventory[slot]?.ModItem?.Mod != CurMod)
                return;
            string path;
            List<string> list = new List<string>();
            if (Player.controlLeft)
            {
                path = $@"{exportPath}\CalValWikiExport.txt";
                list.Add(path);
                File.WriteAllText(path, ExportItemInfo(inventory[slot]), Encoding.UTF8);
                Main.NewText("Exported!");
            }
            if (Player.controlHook)
            {
                path = $@"{exportPath}\CalValWikiMasterExport.txt";
                list.Add(path);
                File.WriteAllText(path, GenerateMasterTemplate(), Encoding.UTF8);
                Main.NewText("Exported!");
            }
            if (Player.controlUp)
            {
                //ContentSamples.Initialize();
                path = $@"{exportPath}\CalValWikiMasterExport.txt";
                list.Add(path);
                File.WriteAllText(path, GenerateBlocks(), Encoding.UTF8);
                Main.NewText("Exported!");
            }
            if (Player.controlMount)
            {
                path = $@"{exportPath}\CalValImage.png";
                list.Add(path);
                Main.NewText(TextureAssets.ArmorBody.Length + " " + ContentSamples.ItemsByType[ModContent.ItemType<FallenPaladinsPlateMail>()].bodySlot);
                return;
                Texture2D nya = TextureAssets.ArmorBody[365]?.Value;
                using (Stream stream = File.OpenWrite(path))
                {
                    nya.SaveAsPng(stream, nya.Width, nya.Height);
                }
                Main.NewText("Exported!");
            }
        }
        /// <summary>
        /// Creates an item page for the Calamity's Vanities wiki
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>

        public static string ExportItemInfo(Item item)
        {
            Projectile shootSample = new Projectile();
            shootSample.type = -1;
            if (item.shoot > 0)
            {
                shootSample = ContentSamples.ProjectilesByType[item.shoot];
            }

            string itemtype = "";
            string itemtype2 = "";
            string projName = "";
            string templateType = "";
            string extension = "png";
            string mountName = "";
            if (shootSample.type > 0)
            {
                if (Main.vanityPet[item.buffType])
                {
                    itemtype = "Pet Summon";
                    itemtype2 = "pet";
                    projName = shootSample.Name;
                    templateType = "Pets";
                    if (Main.projFrames[shootSample.type] > 2)
                    {
                        extension = "png";
                    }
                }
                if (Main.lightPet[item.buffType])
                {
                    itemtype = "Light Pet";
                    itemtype2 = "light pet";
                    projName = shootSample.Name;
                    templateType = "Pets";
                    if (Main.projFrames[shootSample.type] > 2)
                    {
                        extension = "png";
                    }
                }
            }
            if (item.createTile > 0)
            {
                itemtype = "Block";
                itemtype2 = "block";
                templateType = "Blocks";
            }
            if (item.createWall > 0)
            {
                itemtype = "Block";
                itemtype2 = "block";
                templateType = "Blocks";
            }
            if (item.accessory)
            {
                itemtype = "Accessory";
                itemtype2 = "accessory";
                templateType = "Equipables";
            }
            if (item.mountType > 0)
            {
                itemtype = "Mount summon";
                itemtype2 = "mount";
                templateType = "Mounts";
                mountName = MountLoader.GetMount(item.mountType).Name;
            }
            if (item.damage > 0)
            {
                itemtype = "Weapon";
                itemtype2 = "weapon";
                templateType = "Weapons";
            }

            string dropstuff = Drops(item);
            string craftingstuff = Crafting(item);
            string craftable = IsCraftable(item.type) ? " craftable" : "";
            string summoninfo = SummonInfo(item, projName, extension, itemtype2);
            string mountInfo = MountInfo(item, mountName);
            string addWrapper = dropstuff != "" ? "\n{{infobox wrapper" +"\n|" : "";
            string n = dropstuff != "" ? "" : "\n";
            string consumable = item.consumable ? "\n| consumable = yes" : "";
            string placeable = item.createTile > 0 || item.createWall > 0 ? "\n| placeable = yes" : "";
            string auto = item.autoReuse ? "\n| auto = yes" : "";
            string summontxt = templateType == "Pets" ? " summoning item.It summons a " + projName + ", to follow the player." : "";
            return 
                "{{mod sub-page}}<!--DO NOT REMOVE THIS LINE! It is required for Mod sub-pages to work properly.-->"+
                addWrapper + n + "{{item infobox"
                +"\n| type = "+ itemtype
                +"\n| stack = " + item.maxStack
                +"\n| research = " + item.ResearchUnlockCount
                +"\n| tooltip = " + GetTooltip(item)
                +"\n| rare = " + GetRarity(item)
                + (item.damage > 0 ? ("\n| damage = " + item.damage) : "")
                + (item.damage > 0 ? ("\n| knockback = " + item.knockBack) : "")
                + (item.damage > 0 ? ("\n| damagetype = " + item.DamageType.Name.Replace("DamageClass", "")) : "")
                + GetTileDimensions(item)
                +GetUse(item)
                +GetBuff(item)
                +GetBuyPrice(item.type)
                +consumable
                +placeable
                +auto
                +"\n| sell = " + GetValue(item.value)
                +"\n}}"
                + dropstuff
                + summoninfo
                + mountInfo
                +"\nThe '''" + item.Name + "''' is a" + craftable + " {{+|" + itemtype2 + "}}" + summontxt +
                "\n"+
                craftingstuff +
                "\n{{-}}"+
                "\n{{" + ModName + "/Master Template " + templateType +
                "\n| show-main = yes"+
                GetClass(item)+
                "\n}}"+
                "\n";
        }

        public static string GetClass(Item item)
        {
            if (item.damage <= 0)
                return "";
            if (item.ammo <= 0)
            {
                if (item.DamageType.CountsAsClass(DamageClass.Melee))
                    return "\n| show-melee =  yes";
                else if (item.DamageType.CountsAsClass(DamageClass.Ranged))
                    return "\n| show-ranged =  yes";
                else if(item.DamageType.CountsAsClass(DamageClass.Magic))
                    return "\n| show-magic =  yes";
                else if(item.DamageType.CountsAsClass(DamageClass.Summon))
                    return "\n| show-summon =  yes";
                else if(item.DamageType.CountsAsClass(DamageClass.Throwing))
                    return "\n| show-rogue =  yes";
                else
                    return "\n| show-typeless =  yes";
            }
            return "";
        }

        /// <summary>
        /// Grabs NPC drop rates for said item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string Drops(Item item)
        {
            List<(NPC, int, int, float)> npcDropIds = new List<(NPC, int, int, float)>();
            for (int i = 0; i < ContentSamples.NpcsByNetId.Count; i++)
            {
                List<IItemDropRule> loot = Terraria.Main.ItemDropsDB.GetRulesForNPCID(i);
                for (int l = 0; l < loot.Count; l++)
                {
                    if (loot[l] is CommonDrop fuck)
                    {
                        if (fuck.itemId == item.type)
                        {
                            float percent = (float)Math.Round(1 / (float)fuck.chanceDenominator * 100, 2);
                            int min = fuck.amountDroppedMinimum;
                            int max = fuck.amountDroppedMaximum;
                            npcDropIds.Add((ContentSamples.NpcsByNetId[i], min, max, percent));
                        }
                    }
                }
            }

            string dropstuff = npcDropIds.Count > 0 ? "\n|{{drop infobox" : "";

            if (dropstuff.Length > 2)
            {
                foreach ((NPC, int, int, float) drop in npcDropIds)
                {
                    string name = drop.Item1.ModNPC != null ? "#" + drop.Item1.FullName : drop.Item1.FullName;
                    string dropAmt = drop.Item2 == drop.Item3 ? drop.Item2.ToString() : drop.Item2 + "-" + drop.Item3;
                    dropstuff += "\n| " + name + "|" + dropAmt + "|" + drop.Item4 + "%";
                }
                dropstuff += "\n}}" + "\n}}";
            }
            return dropstuff;
        }

        /// <summary>
        /// Grabs if an item is craftable and/or is a material
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string Crafting(Item item)
        {
            bool craftable = IsCraftable(item.type);
            bool crafty = item.material || craftable;
            string recipestuff = crafty ? "\n== Crafting ==" : "";
            if (craftable)
            {
                recipestuff +=
                "\n=== Recipes ===" +
                "\n{{recipes|result=#" + item.Name + "}}" +
                "\n";
            }
            if (item.material)
            {
                recipestuff +=
                "\n=== Used in ===" +
                "\n{{recipes|ingredient=#" + item.Name + "}}" +
                "\n";
            }
            return recipestuff;
        }

        /// <summary>
        /// Grabs if an item is craftable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCraftable(int type)
        {
            foreach (Recipe r in Main.recipe)
            {
                if (r.HasResult(type))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// A wiki formatted function to write the type of minion/pet/light pet that is summoned
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="extension"></param>
        /// <param name="summonType"></param>
        /// <returns></returns>
        public static string SummonInfo(Item item, string name, string extension, string summonType)
        {
            return item.shoot > 0 ? "\n{{summoned|" + name + "|image=[[File:" + name + " (" + ModName + ")." + extension + "]]|type=" + summonType + "}}"
                + "\n" : "";
        }

        /// <summary>
        /// Writes information for a mount
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string MountInfo(Item item, string name)
        {
            return item.mountType > 0 ? "\n{{summoned|" + name + "|image=[[File:" + name + " (" + ModName + ").png]]|type=Mount}}"
                + "\n" : "";
        }

        /// <summary>
        /// A wiki formatted sell price for an item
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetValue(int num)
        {
            string finalVal = "0";

            if (num > 0)
            {
                finalVal = "{{value";
                // platinum
                if (num > 999999 * 5)
                {
                    finalVal += "|" + (int)num / 5000000;
                    num -= (int)(num / 1000000) * 1000000;
                }
                else
                {
                    finalVal += "|0";
                }
                // gold
                if (num > 9999 * 5)
                {
                    finalVal += "|" + (int)(num / 50000);
                    num -= (int)(num / 10000) * 10000;
                }
                else
                {
                    finalVal += "|0";
                }
                // silver
                if (num > 99 * 5)
                {
                    finalVal += "|" + (int)(num / 500);
                    num -= (int)(num / 100) * 100;
                }
                else
                {
                    finalVal += "|0";
                }
                // copper
                finalVal += "|" + num / 5 + "}}";
            }

            return finalVal;
        }

        /// <summary>
        /// Gets use time for an item, accounting for true melee's funny - 1
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetUse(Item item)
        {
            if (item.useStyle < ItemUseStyleID.Swing)
                return "";

            int trueUse = item.useStyle == ItemUseStyleID.Swing ? item.useTime - 1 : item.useTime;
            return "\n| use = " + trueUse;
        }

        /// <summary>
        /// Grabs a buff's information, wiki formatted
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetBuff(Item item)
        {
            string ret = "";
            if (item.buffType > 0)
            {
                string buffName = Lang.GetBuffName(item.buffType);
                if (item.shoot > 0)
                {
                    //if (ContentSamples.ProjectilesByType[item.shoot].Name == buffName)
                    {
                        buffName += " (buff)";
                    }
                }
                ret = "\n| buff = #"+ buffName + " | bufflink = no"
                + "\n| bufftip = " + Lang.GetBuffDescription(item.buffType);
            }
            if (item.mountType > 0)
            {
                Mount.MountData data = MountLoader.GetMount(item.mountType).MountData;
                string buffName = Lang.GetBuffName(data.buff);
                ret = "\n| buff = #" + buffName + " | bufflink = no"
                + "\n| bufftip = " + Lang.GetBuffDescription(data.buff);
            }
            return ret;
        }

        /// <summary>
        /// Grabs a string with the item's rarity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetRarity(Item item)
        {
            if (item.master)
            {
                return "fire red";
            }
            if (item.expert)
            {
                return "rainbow";
            }
            if (item.rare < 12)
            {
                return "{{rare|" + item.rare.ToString() + "}}";
            }
            else
            {
                string rarityName = "";
                if (item.rare == ModContent.RarityType<CalValEX.Rarities.Aqua>())
                {
                    rarityName = "aqua";
                }
                if (item.rare == ModContent.RarityType<Turquoise>())
                {
                    rarityName = "turquoise";
                }
                if (item.rare == ModContent.RarityType<PureGreen>())
                {
                    rarityName = "pure green";
                }
                if (item.rare == ModContent.RarityType<DarkBlue>())
                {
                    rarityName = "dark blue";
                }
                if (item.rare == ModContent.RarityType<Violet>())
                {
                    rarityName = "violet";
                }
                if (item.rare == ModContent.RarityType<HotPink>())
                {
                    rarityName = "hot pink";
                }
                if (item.rare == ModContent.RarityType<CalamityRed>())
                {
                    rarityName = "calamity red";
                }
                if (item.rare == ModContent.RarityType<DarkOrange>())
                {
                    rarityName = "dark orange";
                }
                return "[[File:Rarity color " + rarityName + " (Calamity's Vanities).png]]";
            }
        }
        /// <summary>
        /// Grabs an item's tooltip, Wiki formatted
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>

        public static string GetTooltip(Item item)
        {
            string ret = "";
            for (int i = 0; i < item.ToolTip.Lines; i++)
            {
                ret += item.ToolTip.GetLine(i);
                if (i != item.ToolTip.Lines - 1)
                {
                    ret += "<br/>";
                }
            }
            return ret;
        }
        /// <summary>
        /// Generates various master templates for the Calamity's Vanities wiki
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>

        public static string GenerateMasterTemplate()
        {
            string ret = "";
            List<string> melee = new List<string> { };
            List<string> ranged = new List<string> { };
            List<string> magic = new List<string> { };
            List<string> summon = new List<string> { };
            List<string> rogue = new List<string> { };
            List<string> typeless = new List<string> { };

            Mod CalVal = CurMod;
            foreach (var item in ContentSamples.ItemsByType)
            {
                Item i = item.Value;
                if (item.Value.ModItem != null)
                {
                    if (item.Value.ModItem.Mod == CurMod)
                    {
                        if (i.damage > 0 && !i.accessory)
                        {
                            if (i.DamageType.CountsAsClass(DamageClass.Melee) && i.pick <= 0 && i.axe <= 0 && i.hammer <= 0)
                            {
                                melee.Add(i.Name);
                            }
                            else if (i.DamageType.CountsAsClass(DamageClass.Magic))
                            {
                                magic.Add(i.Name);
                            }
                            else if (i.DamageType.CountsAsClass(DamageClass.Ranged) && i.ammo <= 0)
                            {
                                ranged.Add(i.Name);
                            }
                            else if (i.DamageType.CountsAsClass(DamageClass.Summon))
                            {
                                summon.Add(i.Name);
                            }
                            else if (i.DamageType.CountsAsClass(DamageClass.Throwing))
                            {
                                rogue.Add(i.Name);
                            }
                            else
                            {
                                typeless.Add(i.Name);
                            }
                        }
                    }
                }
            }
            melee.Sort();
            ranged.Sort();
            magic.Sort();
            summon.Sort();
            rogue.Sort();
            typeless.Sort();

            ret += "<!--- MELEE --->\n\n";
            foreach (string m in melee)
            {
                ret += "            -->|{{+|" + m + "}}<!--\n";
            }

            ret += "<!--- RANGED --->\n\n";
            foreach (string m in ranged)
            {
                ret += "            -->|{{+|" + m + "}}<!--\n";
            }

            ret += "<!--- MAGIC --->\n\n";
            foreach (string m in magic)
            {
                ret += "            -->|{{+|" + m + "}}<!--\n";
            }

            ret += "<!--- SUMMON --->\n\n";
            foreach (string m in summon)
            {
                ret += "            -->|{{+|" + m + "}}<!--\n";
            }

            ret += "<!--- ROGUE --->\n\n";
            foreach (string m in rogue)
            {
                ret += "            -->|{{+|" + m + "}}<!--\n";
            }

            ret += "<!--- TYPELESS --->\n\n";
            foreach (string m in typeless)
            {
                ret += "            -->|{{+|" + m + "}}<!--\n";
            }

            return ret;
            /*if (type == "Pet summon")
            {
                for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
                {
                    if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                    if (Main.vanityPet[ContentSamples.ItemsByType[i].buffType] && !Main.lightPet[ContentSamples.ItemsByType[i].buffType])
                    {
                        itemsToAdd.Add(ContentSamples.ItemsByType[i].Name);
                    }
                }
                for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
                {
                    if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                        if (Main.lightPet[ContentSamples.ItemsByType[i].buffType])
                        {
                            litemsToAdd.Add(ContentSamples.ItemsByType[i].Name);
                        }
                }
            }*/
            // consumables
            /*
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                {
                    Projectile p = new Projectile();
                    if (item.shoot > 0)
                    {
                        if (ContentSamples.ProjectilesByType[item.shoot].aiStyle == ProjAIStyleID.Hook)
                        {
                            continue;
                        }
                    }
                    if (item.Name != "Compensation" && !item.Name.Contains("Plush") && !Main.vanityPet[item.buffType] && !Main.lightPet[item.buffType] && item.createTile <= 0 && item.createWall <= 0 && !item.accessory && item.mountType <= 0 && item.makeNPC <= 0 && !item.vanity)
                    {
                        itemsToAdd.Add(ContentSamples.ItemsByType[i].Name);
                    }
                }
            }*/
            List<string> itemsToAdd = GenerateTilesTemplate();
            string finalTemp = "";

            foreach (string item in itemsToAdd)
            {
                finalTemp += "{{+|" + item + "}}{{•}}";
            }

            return finalTemp;
        }

        public static string GetBuyPrice(int type)
        {
            return "";
            string oracleShopName = NPCShopDatabase.GetShopName(ModContent.NPCType<OracleNPC>());
            string jellyShopName = NPCShopDatabase.GetShopName(ModContent.NPCType<JellyPriestNPC>());
            string jharimShopName = NPCShopDatabase.GetShopName(ModContent.NPCType<Jharim>());
            NPCShopDatabase.TryGetNPCShop(oracleShopName, out AbstractNPCShop oracleShop);
            NPCShopDatabase.TryGetNPCShop(jellyShopName, out AbstractNPCShop jellyShop);
            NPCShopDatabase.TryGetNPCShop(jharimShopName, out AbstractNPCShop jharShop);
            NPCShop oShop = (NPCShop)oracleShop;
            List<NPCShop.Entry> entry1 = (List<NPCShop.Entry>)oShop.GetType().GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(oShop);
            foreach (NPCShop.Entry i in entry1)
            {
                if (i.Item.type == type)
                {
                    return "\n buy = " + GetValue(i.Item.value * 5);
                }
            }
            return "";
        }

        public static string GetTileDimensions(Item item)
        {
            if (item.createTile <= 0)
            {
                return "";
            }
            string ret = "";
            MapTile m = new MapTile();
            m.Type = (ushort)item.createTile;
            Color c = MapHelper.GetMapTileXnaColor(ref m);
            ret += "\n| color = " + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
            TileObjectData data = TileObjectData.GetTileData(item.createTile, item.placeStyle);
            if (data == null)
                return ret;
            if (data.Height > 0)
            {
                ret += "\n| height = " + data.Height;
            }
            if (data.Width > 0)
            {
                ret += "\n| width = " + data.Width;
            }
            return ret;
        }

        public static List<string> GenerateEquipsTemplate()
        {
            Mod CalVal = CurMod;
            List<string> itemsToAdd = new List<string>();
            List<string> boditemsToAdd = new List<string>();
            List<string> legitemsToAdd = new List<string>();
            List<string> balitemsToAdd = new List<string>();
            List<string> scarfitemsToAdd = new List<string>();
            List<string> capeitemsToAdd = new List<string>();
            List<string> transitemsToAdd = new List<string>();
            List<string> shielditemsToAdd = new List<string>();
            List<string> miscitemstoadd = new List<string>();
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                {
                    if (item.headSlot > 0)
                        itemsToAdd.Add(ContentSamples.ItemsByType[i].Name);

                }
            }
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                {
                    if (item.bodySlot > 0)
                        boditemsToAdd.Add(ContentSamples.ItemsByType[i].Name);

                }
            }
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                {
                    if (item.legSlot > 0)
                        legitemsToAdd.Add(ContentSamples.ItemsByType[i].Name);

                }
            }
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                {
                    if (item.balloonSlot > 0)
                        balitemsToAdd.Add(ContentSamples.ItemsByType[i].Name);

                }
            }
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                {
                    if (item.neckSlot > 0)
                        scarfitemsToAdd.Add(ContentSamples.ItemsByType[i].Name);

                }
            }
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                {
                    if (item.shieldSlot > 0)
                        shielditemsToAdd.Add(ContentSamples.ItemsByType[i].Name);

                }
            }
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                {
                    if (item.accessory)
                    {
                        if (item.backSlot > 0)
                            capeitemsToAdd.Add(ContentSamples.ItemsByType[i].Name);
                    }
                }
            }
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                {
                    if (item.accessory && item.headSlot + item.bodySlot + item.legSlot + item.backSlot + item.shieldSlot + item.neckSlot + item.wingSlot + item.balloonSlot + item.wingSlot <= 0)
                    {
                        miscitemstoadd.Add(ContentSamples.ItemsByType[i].Name);
                    }
                }
            }

            itemsToAdd.Sort();
            boditemsToAdd.Sort();
            legitemsToAdd.Sort();
            balitemsToAdd.Sort();
            scarfitemsToAdd.Sort();
            capeitemsToAdd.Sort();
            shielditemsToAdd.Sort();
            miscitemstoadd.Sort();
            itemsToAdd.AddRange(boditemsToAdd);
            itemsToAdd.AddRange(legitemsToAdd);
            itemsToAdd.AddRange(balitemsToAdd);
            itemsToAdd.AddRange(scarfitemsToAdd);
            itemsToAdd.AddRange(capeitemsToAdd);
            itemsToAdd.AddRange(shielditemsToAdd);
            itemsToAdd.AddRange(miscitemstoadd);
            return itemsToAdd;
        }

        public static string GenerateWingStats()
        {
            Mod Calval = CurMod;
            List<Item> items = new List<Item>();
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (item?.ModItem?.Mod != Calval)
                {
                    continue;
                }
                if (item.wingSlot > 0)
                {
                    items.Add(item);
                }
            }
            string ret = "";
            for (int i = 0; i < items.Count; i++)
            {
                List<(NPC, int, int, float)> npcDropIds = new List<(NPC, int, int, float)>();
                for (int n = 0; n < ContentSamples.NpcsByNetId.Count; n++)
                {
                    List<IItemDropRule> loot = Terraria.Main.ItemDropsDB.GetRulesForNPCID(n);
                    for (int l = 0; l < loot.Count; l++)
                    {
                        if (loot[l] is CommonDrop fuck)
                        {
                            if (fuck.itemId == items[i].type)
                            {
                                float percent = (float)Math.Round(1 / (float)fuck.chanceDenominator * 100, 2);
                                int min = fuck.amountDroppedMinimum;
                                int max = fuck.amountDroppedMaximum;
                                npcDropIds.Add((ContentSamples.NpcsByNetId[n], min, max, percent));
                                break;
                            }
                        }
                    }
                }

                string craftable = IsCraftable(items[i].type) ? "\n| {{recipes/extract|result=#" + items[i].Name + "}}" : npcDropIds.Count > 0 ? "\n| {{+|" + npcDropIds[0].Item1.FullName + "}} " + npcDropIds[0].Item4 + "%" : "\n| ";
                ret += "| {{item|class=boldname|#" + items[i].Name + "}}" +
                   "\n| [[File:" + items[i].Name + " (equipped) (" + ModName + ").png|link=]]" +
                   craftable +
                   "\n| " + GetTooltip(items[i]) + 
                   "\n| " + ArmorIDs.Wing.Sets.Stats[items[i].wingSlot].FlyTime +
                   "\n| " + Math.Round((float)ArmorIDs.Wing.Sets.Stats[items[i].wingSlot].FlyTime / 60f, 2) +
                   "\n| " + ArmorIDs.Wing.Sets.Stats[items[i].wingSlot].AccRunSpeedOverride +
                   "\n| " + ArmorIDs.Wing.Sets.Stats[items[i].wingSlot].AccRunAccelerationMult +
                   "\n| " + GetValue(items[i].value) +
                   "\n| " + GetRarity(items[i]) +
                   "\n|-\n";
            }
            return ret;
        }

        public static string GenerateVanityAccessories()
        {
            string ret = "";
            Mod Calval = CurMod;
            List<Item> items = new List<Item>();
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (item?.ModItem?.Mod != Calval)
                {
                    continue;
                }
                if (item.accessory && item.headSlot <= 0 && item.wingSlot <= 0 && item.bodySlot <= 0 && item.legSlot <= 0)
                {
                    items.Add(item);
                }
            }
            for (int i = 0; i < items.Count; i++)
            {
                List<(NPC, int, int, float)> npcDropIds = new List<(NPC, int, int, float)>();
                for (int n = 0; n < ContentSamples.NpcsByNetId.Count; n++)
                {
                    List<IItemDropRule> loot = Terraria.Main.ItemDropsDB.GetRulesForNPCID(n);
                    for (int l = 0; l < loot.Count; l++)
                    {
                        if (loot[l] is CommonDrop fuck)
                        {
                            if (fuck.itemId == items[i].type)
                            {
                                float percent = (float)Math.Round(1 / (float)fuck.chanceDenominator * 100, 2);
                                int min = fuck.amountDroppedMinimum;
                                int max = fuck.amountDroppedMaximum;
                                npcDropIds.Add((ContentSamples.NpcsByNetId[n], min, max, percent));
                                break;
                            }
                        }
                    }
                }

                string craftable = IsCraftable(items[i].type) ? "\n| {{recipes/extract|result=#" + items[i].Name + "}}" : npcDropIds.Count > 0 ? "\n| {{+|" + npcDropIds[0].Item1.FullName + "}} " + npcDropIds[0].Item4 + "%" : "\n| ";
                ret += "| {{item|class=boldname|#" + items[i].Name + "}}" +
                   "\n| [[File:" + items[i].Name + " (equipped) (" + ModName + ").png|link=]]" +
                   craftable +
                   "\n| " + GetTooltip(items[i]) +
                   "\n| " + GetSlot(items[i]) +
                   "\n| " + GetValue(items[i].value) +
                   "\n| " + GetRarity(ContentSamples.ItemsByType[items[i].type]) +
                   "\n|-\n";
            }
            return ret;
        }

        public static string GetSlot(Item item)
        {
            if (item.backSlot > 0)
                return "Back";
            if (item.frontSlot > 0)
                return "Front";
            if (item.balloonSlot > 0)
                return "Balloon";
            if (item.shieldSlot > 0)
                return "Shield";
            if (item.neckSlot > 0)
                return "Neck";
            return "";
        }

        public static Color[,] PopulateColor(Color[,] exportColors, Color[,] baseColors, int curIDX, int curIDY, int expOffX, int expOffY, int impOffX, int impOffY)
        {
            exportColors[curIDX + expOffX, curIDY + expOffY] = baseColors[impOffX + curIDX, impOffY + curIDY];
            return exportColors;
        }

        public static string GenerateConsumablesPage()
        {
            string ret = "";
            Mod Calval = CurMod;
            List<Item> items = new List<Item>();
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (item?.ModItem?.Mod != Calval)
                {
                    continue;
                }
                if ((item.material && item.maxStack > 1 && item.shoot <= 0 && item.makeNPC <= 0 && item.createTile <= 0 && item.createWall <= 0) || item.type == ModContent.ItemType<ProfanedBattery>() || item.type == ModContent.ItemType<ProfanedWheels>() || item.type == ModContent.ItemType<ProfanedFrame>())
                {
                    items.Add(item);
                }
            }
            for (int i = 0; i < items.Count; i++)
            {
                List<(NPC, int, int, float)> npcDropIds = new List<(NPC, int, int, float)>();
                for (int n = 0; n < ContentSamples.NpcsByNetId.Count; n++)
                {
                    List<IItemDropRule> loot = Terraria.Main.ItemDropsDB.GetRulesForNPCID(n);
                    for (int l = 0; l < loot.Count; l++)
                    {
                        if (loot[l] is CommonDrop fuck)
                        {
                            if (fuck.itemId == items[i].type)
                            {
                                float percent = (float)Math.Round(1 / (float)fuck.chanceDenominator * 100, 2);
                                int min = fuck.amountDroppedMinimum;
                                int max = fuck.amountDroppedMaximum;
                                npcDropIds.Add((ContentSamples.NpcsByNetId[n], min, max, percent));
                                break;
                            }
                        }
                    }
                }

                string craftable = IsCraftable(items[i].type) ? "\n| {{recipes/extract|result=#" + items[i].Name + "}}" : npcDropIds.Count > 0 ? "\n| {{+|" + npcDropIds[0].Item1.FullName + "}} " + npcDropIds[0].Item4 + "%" : "\n| ";
                ret += "| {{item|class=boldname|#" + items[i].Name + "}}" +
                   craftable +
                   "\n| " + GetTooltip(items[i]) +
                   "\n| " + GetValue(items[i].value) +
                   "\n| " + GetRarity(ContentSamples.ItemsByType[items[i].type]) +
                   "\n|-\n";
            }
            return ret;
        }

        public static string GenerateVanityStandalone()
        {
            string ret = "";
            Mod Calval = CurMod;
            List<Item> items = new List<Item>();
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (item?.ModItem?.Mod != Calval)
                {
                    continue;
                }
                if (item.headSlot >= 0 || item.bodySlot >= 0 || item.legSlot >= 0)
                {
                    if (!item.Name.Contains("Profaned Cultist") && !item.Name.Contains("Cultist Assassin") && !item.Name.Contains("Odd Polterghast") && !item.Name.Contains("Brimstone") && !item.Name.Contains("Fallen Paladin's") && !item.Name.Contains("Draedon") && !item.Name.Contains("Arsenal Soldier") && !item.Name.Contains("Cryo") && !item.Name.Contains("Demonshade") && !item.Name.Contains("Belladonna") && !item.Name.Contains("Astrachnid") && !item.Name.Contains("Bloody Mary") && !item.Name.Contains("Earthen"))
                    items.Add(item);
                }
            }
            for (int i = 0; i < items.Count; i++)
            {
                List<(NPC, int, int, float)> npcDropIds = new List<(NPC, int, int, float)>();
                for (int n = 0; n < ContentSamples.NpcsByNetId.Count; n++)
                {
                    List<IItemDropRule> loot = Terraria.Main.ItemDropsDB.GetRulesForNPCID(n);
                    for (int l = 0; l < loot.Count; l++)
                    {
                        if (loot[l] is CommonDrop fuck)
                        {
                            if (fuck.itemId == items[i].type)
                            {
                                float percent = (float)Math.Round(1 / (float)fuck.chanceDenominator * 100, 2);
                                int min = fuck.amountDroppedMinimum;
                                int max = fuck.amountDroppedMaximum;
                                npcDropIds.Add((ContentSamples.NpcsByNetId[n], min, max, percent));
                                break;
                            }
                        }
                    }
                }

                string craftable = IsCraftable(items[i].type) ? "\n| {{recipes/extract|result=#" + items[i].Name + "}}" : npcDropIds.Count > 0 ? "\n| {{+|" + npcDropIds[0].Item1.FullName + "}} " + npcDropIds[0].Item4 + "%" : "\n| ";
                ret += "| {{item|class=boldname|#" + items[i].Name + "}}" +
                   "\n| [[File:" + items[i].Name + " (equipped) (" + ModName + ").png|link=]]" +
                   craftable +
                   "\n| " + GetTooltip(items[i]) +
                   "\n| " + GetArmorSlot(items[i]) +
                   "\n| " + GetValue(items[i].value) +
                   "\n| " + GetRarity(ContentSamples.ItemsByType[items[i].type]) +
                   "\n|-\n";
            }
            return ret;
        }

        public static string GetArmorSlot(Item i)
        {
            if (i.headSlot > 0)
                return "Head";
            if (i.bodySlot > 0 && i.legSlot > 0)
                return "Body and Legs";
            if (i.bodySlot > 0)
                return "Body";
            if (i.legSlot > 0)
                return "Legs";
            return "";
        }

        public static string GenerateVanitySets()
        {
            string ret = "";
            Mod Calval = CurMod;
            List<Item> items = new List<Item>();
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (item?.ModItem?.Mod != Calval)
                {
                    continue;
                }
                if (item.headSlot >= 0 || item.bodySlot >= 0 || item.legSlot >= 0)
                {
                    if (item.Name.Contains("Profaned Cultist") || item.Name.Contains("Cultist Assassin") || item.Name.Contains("Odd Polterghast") || item.Name.Contains("Brimstone") || item.Name.Contains("Fallen Paladin's") || item.Name.Contains("Draedon") || item.Name.Contains("Arsenal Soldier") || item.Name.Contains("Cryo") || item.Name.Contains("Demonshade") || item.Name.Contains("Belladonna") || item.Name.Contains("Astrachnid") || item.Name.Contains("Bloody Mary") || item.Name.Contains("Earthen") || item.Name.Contains("Perennial"))
                        items.Add(item);
                }
            }
            for (int i = 0; i < items.Count; i++)
            {
                List<(NPC, int, int, float)> npcDropIds = new List<(NPC, int, int, float)>();
                for (int n = 0; n < ContentSamples.NpcsByNetId.Count; n++)
                {
                    List<IItemDropRule> loot = Terraria.Main.ItemDropsDB.GetRulesForNPCID(n);
                    for (int l = 0; l < loot.Count; l++)
                    {
                        if (loot[l] is CommonDrop fuck)
                        {
                            if (fuck.itemId == items[i].type)
                            {
                                float percent = (float)Math.Round(1 / (float)fuck.chanceDenominator * 100, 2);
                                int min = fuck.amountDroppedMinimum;
                                int max = fuck.amountDroppedMaximum;
                                npcDropIds.Add((ContentSamples.NpcsByNetId[n], min, max, percent));
                                break;
                            }
                        }
                    }
                }

                string craftable = IsCraftable(items[i].type) ? "\n| {{recipes/extract|result=#" + items[i].Name + "}}" : npcDropIds.Count > 0 ? "\n| {{+|" + npcDropIds[0].Item1.FullName + "}} " + npcDropIds[0].Item4 + "%" : "\n| ";
                ret += "| {{item|class=boldname|#" + items[i].Name + "}}" +
                   "\n| [[File:" + items[i].Name + " (equipped) (" + ModName + ").png|link=]]" +
                   craftable +
                   "\n| " + GetTooltip(items[i]) +
                   "\n| " + GetArmorSlot(items[i]) +
                   "\n| " + GetValue(items[i].value) +
                   "\n| " + GetRarity(ContentSamples.ItemsByType[items[i].type]) +
                   "\n|-\n";
            }
            return ret;
        }

        public static string GenerateTransformations()
        {
            string ret = "";
            Mod Calval = CurMod;
            List<Item> items = new List<Item>();
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (item?.ModItem?.Mod != Calval)
                {
                    continue;
                }
                if (item.accessory && !item.vanity && item.wingSlot <= 0 && item.createTile <= 0)
                {
                        items.Add(item);
                }
            }
            for (int i = 0; i < items.Count; i++)
            {
                List<(NPC, int, int, float)> npcDropIds = new List<(NPC, int, int, float)>();
                for (int n = 0; n < ContentSamples.NpcsByNetId.Count; n++)
                {
                    List<IItemDropRule> loot = Terraria.Main.ItemDropsDB.GetRulesForNPCID(n);
                    for (int l = 0; l < loot.Count; l++)
                    {
                        if (loot[l] is CommonDrop fuck)
                        {
                            if (fuck.itemId == items[i].type)
                            {
                                float percent = (float)Math.Round(1 / (float)fuck.chanceDenominator * 100, 2);
                                int min = fuck.amountDroppedMinimum;
                                int max = fuck.amountDroppedMaximum;
                                npcDropIds.Add((ContentSamples.NpcsByNetId[n], min, max, percent));
                                break;
                            }
                        }
                    }
                }

                string craftable = IsCraftable(items[i].type) ? "\n| {{recipes/extract|result=#" + items[i].Name + "}}" : npcDropIds.Count > 0 ? "\n| {{+|" + npcDropIds[0].Item1.FullName + "}} " + npcDropIds[0].Item4 + "%" : "\n| ";
                ret += "| {{item|class=boldname|#" + items[i].Name + "}}" +
                   "\n| [[File:" + items[i].Name + " (equipped) (" + ModName + ").png|link=]]" +
                   craftable +
                   "\n| " + GetTooltip(items[i]) +
                   "\n| " + GetValue(items[i].value) +
                   "\n| " + GetRarity(ContentSamples.ItemsByType[items[i].type]) +
                   "\n|-\n";
            }
            return ret;
        }

        public static string GenerateItemsPageLikeTheBIGOne()
        {
            string ret = "";
            Mod Calval = CurMod;
            List<string> items = new List<string>();
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (item?.ModItem?.Mod != Calval)
                {
                    continue;
                }
                items.Add(item.Name);
            }
            items.Sort();
            for (int i = 0; i < items.Count; i++)
            {
                ret += "\n| {{item|#" + items[i] + "}}";
            }
            return ret;
        }

        public static string GenerateBlocks()
        {
            string ret = "";
            Mod Calval = CurMod;
            List<Item> items = new List<Item>();
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (item?.ModItem?.Mod != Calval)
                {
                    continue;
                }
                if (item.createTile >= 0)
                {
                    if (Main.tileFrameImportant[item.createTile])
                        continue;
                    items.Add(item);
                }
            }
            items.Sort((x, y) => x.Name.CompareTo(y.Name));
            for (int i = 0; i < items.Count; i++)
            {
                List<(NPC, int, int, float)> npcDropIds = new List<(NPC, int, int, float)>();
                for (int n = 0; n < ContentSamples.NpcsByNetId.Count; n++)
                {
                    List<IItemDropRule> loot = Terraria.Main.ItemDropsDB.GetRulesForNPCID(n);
                    for (int l = 0; l < loot.Count; l++)
                    {
                        if (loot[l] is CommonDrop fuck)
                        {
                            if (fuck.itemId == items[i].type)
                            {
                                float percent = (float)Math.Round(1 / (float)fuck.chanceDenominator * 100, 2);
                                int min = fuck.amountDroppedMinimum;
                                int max = fuck.amountDroppedMaximum;
                                npcDropIds.Add((ContentSamples.NpcsByNetId[n], min, max, percent));
                                break;
                            }
                        }
                    }
                }

                string craftable = IsCraftable(items[i].type) ? "\n| {{recipes/extract|result=#" + items[i].Name + "}}" : npcDropIds.Count > 0 ? "\n| {{+|" + npcDropIds[0].Item1.FullName + "}} " + npcDropIds[0].Item4 + "%" : "\n| ";
                ret += "| {{item|class=boldname|#" + items[i].Name + "}}" +
                   "\n| [[File:" + items[i].Name + " (placed) (" + ModName + ").png|link=]]" +
                   craftable +
                   "\n| " + GetRarity(ContentSamples.ItemsByType[items[i].type]) +
                   "\n|-\n";
            }
            return ret;
        }

        public static List<string> GenerateTilesTemplate()
        {
            Mod CalVal = CurMod;
            List<string> blocksToAdd = new List<string>();
            List<string> wallsToAdd = new List<string>();
            List<string> furnitureToAdd = new List<string>();
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                {
                    if (item.createTile > 0 && !Main.tileFrameImportant[item.createTile])
                        blocksToAdd.Add(ContentSamples.ItemsByType[i].Name);

                }
            }
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                {
                    if (item.createWall > 0)
                        wallsToAdd.Add(ContentSamples.ItemsByType[i].Name);

                }
            }
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (ContentSamples.ItemsByType[i]?.ModItem?.Mod == CalVal)
                {
                    if (item.createTile > 0 && Main.tileFrameImportant[item.createTile])
                        furnitureToAdd.Add(ContentSamples.ItemsByType[i].Name);

                }
            }

           blocksToAdd.Sort();
           wallsToAdd.Sort();
            //furnitureToAdd.Sort();
            blocksToAdd.AddRange(wallsToAdd);
            blocksToAdd.AddRange(furnitureToAdd);
            return blocksToAdd;
        }

        public static void GenerateBlockSprites()
        {
            foreach (var item in ContentSamples.ItemsByType)
            {
                if (item.Value.ModItem == null) continue;
                if (item.Value.ModItem.Mod != CurMod) continue;
                if (item.Value.createTile <= 0)
                    continue;
                if (Main.tileFrameImportant[item.Value.createTile]) continue;

                Texture2D baseTexture = TextureAssets.Tile[item.Value.createTile].Value;
                int width = 48;
                int height = 48;

                Texture2D exporter = new Texture2D(Main.instance.GraphicsDevice, width, height);

                Color[] baseColors = new Color[baseTexture.Width * baseTexture.Height];
                baseTexture.GetData(baseColors);

                Color[] exportColors = new Color[exporter.Width * exporter.Height];
                exporter.GetData(exportColors);
                Color[] result = new Color[exportColors.Length];

                Color[,] baseArrayColors = baseTexture.GetColorsFromTexture();
                Color[,] finalArray = new Color[48, 48];

                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        finalArray = PopulateColor(finalArray, baseArrayColors, i, j, 0, 0, 0, 54); // var 1
                        finalArray = PopulateColor(finalArray, baseArrayColors, i, j, 16, 0, 36, 0); // var 2
                        finalArray = PopulateColor(finalArray, baseArrayColors, i, j, 32, 0, 90, 54); // var 3

                        finalArray = PopulateColor(finalArray, baseArrayColors, i, j, 0, 16, 0, 18); // var 2
                        finalArray = PopulateColor(finalArray, baseArrayColors, i, j, 16, 16, 54, 18); // var 3
                        finalArray = PopulateColor(finalArray, baseArrayColors, i, j, 32, 16, 72, 0); // var 3

                        finalArray = PopulateColor(finalArray, baseArrayColors, i, j, 0, 32, 0, 72); // var 1
                        finalArray = PopulateColor(finalArray, baseArrayColors, i, j, 16, 32, 36, 36); // var 2
                        finalArray = PopulateColor(finalArray, baseArrayColors, i, j, 32, 32, 90, 72); // var 3
                    }
                }

                for (int i = 0; i < 48; i++)
                {
                    for (int j = 0; j < 48; j++)
                    {
                        result[i + j * 48] = finalArray[i, j];
                    }
                }
                exporter.SetData(result);

                string path = $@"{exportPath}\" + item.Value.Name + " (placed) (" + CurMod.DisplayName + ").png";
                using (Stream stream = File.OpenWrite(path))
                {
                    exporter.SaveAsPng(stream, exporter.Width, exporter.Height);
                }
            }
        }

        public static string GenerateAllRecipes(int tileType)
        {
            string ret = "";

            foreach (Recipe r in Main.recipe)
            {
                if (r.Mod == null)
                    continue;
                if (r.Mod != CurMod)
                    continue;
                if (tileType <= -1 && r.requiredTile.Count > 0)
                    continue;
                if (r.requiredTile.Count > 0 && ((tileType > -1 && r.requiredTile[0] != tileType) || (tileType == TileID.Ebonstone && r.requiredTile[0] != TileID.DemonAltar)))
                    continue;

                string itemName = r.createItem.ModItem == null ? r.createItem.Name : "#" + r.createItem.Name;

                string station = r.requiredTile.Count > 0 ? GetTileItemName(r.requiredTile[0]) : "By Hand";

                ret +=
                    "\n{{recipes/register\n|result=" + GetTaggedItemName(r.createItem) + "|amount=" + r.createItem.stack + "\r\n|station=" + station;
                    
                foreach (var v in r.requiredItem)
                {
                    ret += "\n|" + GetTaggedItemName(v) + "|" + v.stack;
                }
                ret += "\n}}\n";
            }

            return ret;

        }

        public static string GetTileItemName(int type)
        {
            if (type == TileID.DemonAltar)
                return "Demon Altar";
            foreach (var v in ContentSamples.ItemsByType)
            {
                if (v.Value.createTile < 0)
                    continue;
                if (v.Value.createTile == type)
                {
                    return GetTaggedItemName(v.Value);
                }
            }
            return "";
        }

        public static string GetTaggedItemName(Item i)
        {
            return i.ModItem == null || i.ModItem.Mod == ModLoader.GetMod("CalamityMod") ? i.Name : "#" + i.Name;
        }

        public static string GenerateNPCMasterTemplate()
        {
            string ret = "";

            ret += "-->{{navbox/start<!--\n" +
                "-->|header = NPCs<!--\n" +
                "-->|class = $show-common$<!--\n" +
                "-->}}<!--\n" +
                "--><div class=\"table\"><!--\n";

            List<(string, string)> entriesByBiome = new List<(string, string)>();

            ret += "-->}}<!--\n--></div><!--\n--><div><!--\n-->{{navbox/h1|[[Common]]}}<!--\n-->{{dotlist<!--\n";
            foreach (var n in ContentSamples.NpcsByNetId)
            {
                if (n.Value.ModNPC == null)
                    continue;
                if (n.Value.ModNPC.Mod != CurMod)
                    continue;
                ret += "-->|{{item|#" + n.Value.FullName + "}}<!--\n";

            }
            ret += "\n-->{{navbox/end}}<!--";

            return ret;
        }
    }
}
