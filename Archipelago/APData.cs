using System;
using System.Collections.Generic;
using OpenTK;
using ARPGGame;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using TiltedEngine.Drawing;
using HammerwatchAP.Util;

namespace HammerwatchAP.Archipelago
{
    public static class APData
    {
        public const int castleStartID = 0x111000;
        public const int templeStartID = 0x110000;
        public const int castleButtonItemStartID = templeStartID + 768;
        public const int templeButtonItemStartID = templeStartID + 1024;
        public const int shopLocationIdOffset = templeStartID + 65536; //0x10000

        public const int PLAYER_KEYS = 11;
        public static int GetAdjustedLocationId(int locId, ArchipelagoData archipelagoData)
        {
            switch (archipelagoData.mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    locId += castleStartID;
                    break;
                case ArchipelagoData.MapType.Temple:
                    locId += templeStartID;
                    break;
            }
            return locId;
        }

        public const int shopItemOffset = 1280;
        public const int shopItemBaseId = templeStartID + shopItemOffset;

        public const string archipelagoItemXmlName = "items/archipelago_item.xml";
        public const string archipelagoFillerItemXmlName = "items/archipelago_filler_item.xml";
        public const string archipelagoUsefulItemXmlName = "items/archipelago_useful_item.xml";
        public const string buttonEffectItemXmlName = "items/button_effect.xml";
        public const string bossRune1ItemXmlName = "items/boss_rune_1.xml";
        public const string bossRune2ItemXmlName = "items/boss_rune_2.xml";
        public const string bossRune3ItemXmlName = "items/boss_rune_3.xml";
        public const string checkItemXmlName = "items/ap_check.xml";
        public static bool IsItemXmlNameCorporeal(string xmlName)
        {
            switch (xmlName)
            {
                case buttonEffectItemXmlName:
                case bossRune1ItemXmlName:
                case bossRune2ItemXmlName:
                case bossRune3ItemXmlName:
                    return false;
                default:
                    return true;
            }
        }

        public const string apHoloPrefix = "items/archipelago_holo";
        public const string apHoloItemXmlName = "items/archipelago_holo.xml";
        public const string apHoloFillerItemXmlName = "items/archipelago_holo_filler.xml";
        public const string apHoloProgItemXmlName = "items/archipelago_holo_prog.xml";

        public const string bombTrapXmlName = "items/chest_trap_bomb.xml";

        public static readonly List<string> deathlinkMessages = new List<string>()
        {
            "_ decided they had too many ankhs.",
            "_ is teleporting back to a checkpoint.",
            "_ needed to heal, but couldn't in time.",
            "_ has purchased a deadly rejuvenation potion.",
            "_ got gibbed.",
            "_ has made a tactical retreat by dying.",
            "_ probably stepped on spikes.",
            "_ couldn't reach the steak on the other side of the map.",
            "Hope _ has plenty more anhks.",
            "_ didn't have enough health upgrades.",
            "_ should have invested in more defense upgrades.",
            "_ died playing the easiest class.",
            "_ almost tried.",
            "_ really should visit the shop.",
        };
        public static readonly Dictionary<PlayerClass, List<string>> classDeathlinkMessages = new Dictionary<PlayerClass, List<string>>()
        {
            {
                PlayerClass.KNIGHT, new List<string>()
                {
                    "_ couldn't tank hard enough.",
                    "_'s shield was embarrasingly small.",
                }
            },
            {
                PlayerClass.WIZARD, new List<string>()
                {
                    "_ couldn't take their own heat.",
                    "_'s fire has gone out.",
                }
            },
            {
                PlayerClass.RANGER, new List<string>()
                {
                    "_ failed their dodge chance.",
                    "Nature has let down _.",
                }
            },
            {
                PlayerClass.WARLOCK, new List<string>()
                {
                    "_'s shiny glowstick did not save them.",
                    "_ was sacrificed before they could sacrifice the enemies.",
                }
            },
            {
                PlayerClass.THIEF, new List<string>()
                {
                    "Capitalism has failed _.",
                    "_ got stabbed first.",
                }
            },
            {
                PlayerClass.PRIEST, new List<string>()
                {
                    "_'s mana shield ran out.",
                    "_'s regen hasn't kicked in yet.",
                }
            },
            {
                PlayerClass.SORCERER, new List<string>()
                {
                    "_ couldn't freeze the enemies fast enough.",
                    "_ couldn't outrun the enemies they should've frozen.",
                }
            },
        };

        public static readonly Dictionary<string, string> itemNameToXML = new Dictionary<string, string>()
        {
            { "Bonus Chest", "items/bonus_chest.xml" },
            { "Bonus Key", "items/bonus_key.xml" },
            { "Blue Chest", "items/chest_blue.xml" },
            { "Green Chest", "items/chest_green.xml" },
            { "Purple Chest", "items/chest_purple.xml" },
            { "Red Chest", "items/chest_red.xml" },
            { "Wood Chest", "items/chest_wood.xml" },
            { "Vendor Coin", "items/collectable_1.xml" },
            { "Strange Plank", "items/collectable_2.xml" },
            { "Bronze Key", "items/key_bronze.xml" },
            { "Silver Key", "items/key_silver.xml" },
            { "Gold Key", "items/key_gold.xml" },
            { "Mirror", "items/key_mirror.xml" },
            { "Ore", "items/key_ore.xml" },
            { "Rune Key", "items/key_teleport.xml" },
            { "1-Up Ankh", "items/powerup_1up.xml" },
            { "5-Up Ankh", "items/powerup_5up.xml" },
            { "7-Up Ankh", "items/powerup_7up.xml" },
            { "Damage Potion", "items/powerup_potion3.xml" },
            { "Rejuvenation Potion", "items/powerup_potion2.xml" },
            { "Invulnerability Potion", "items/powerup_potion1.xml" },
            { "Diamond", "items/valuable_diamond.xml" },
            { "Red Diamond", "items/valuable_diamond_red.xml" },
            { "Small Diamond", "items/valuable_diamond_small.xml" },
            { "Small Red Diamond", "items/valuable_diamond_small_red.xml" },
            { "Random Stat Upgrade",    "prefabs/upgrade_random.xml" },
            { "Damage Upgrade", "items/upgrade_damage.xml" },
            { "Defense Upgrade", "items/upgrade_defense.xml" },
            { "Health Upgrade", "items/upgrade_health.xml" },
            { "Mana Upgrade", "items/upgrade_mana.xml" },
            { "Copper Coin", "items/valuable_1.xml" },
            { "Copper Coins", "items/valuable_2.xml" },
            { "Copper Coin Pile", "items/valuable_3.xml" },
            { "Silver Coin", "items/valuable_4.xml" },
            { "Silver Coins", "items/valuable_5.xml" },
            { "Silver Coin Pile", "items/valuable_6.xml" },
            { "Gold Coin", "items/valuable_7.xml" },
            { "Gold Coins", "items/valuable_8.xml" },
            { "Gold Coin Pile", "items/valuable_9.xml" },
            { "Apple", "items/health_1.xml" },
            { "Orange", "items/health_2.xml" },
            { "Steak", "items/health_3.xml" },
            { "Fish", "items/health_4.xml" },
            { "Mana Shard", "items/mana_1.xml" },
            { "Mana Orb", "items/mana_2.xml" },
            { "Frying Pan", "items/tool_pan.xml" },
            { "Pumps Lever",    "items/tool_lever.xml" },
            { "Pickaxe", "items/tool_pickaxe.xml" },
            { "Hammer", "items/tool_hammer.xml" },
            { "Frying Pan Fragment", "items/tool_pan_fragment.xml" },
            { "Pumps Lever Fragment",   "items/tool_lever_fragment.xml" },
            { "Pickaxe Fragment", "items/tool_pickaxe_fragment.xml" },
            { "Hammer Fragment", "items/tool_hammer_fragment.xml" },
            { "Gold Ring", "items/special_ring.xml" },
            { "Serious Health Upgrade", "items/special_serious_health.xml" },
            { "Bomb Trap", bombTrapXmlName }, //If changing this update the generation code
            { "Mana Drain Trap", "items/chest_trap_drain.xml" },
            { "Poison Trap", "items/chest_trap_poison.xml" },
            { "Frost Trap", "items/chest_trap_frost.xml" },
            { "Fire Trap", "items/chest_trap_fire.xml" },
            { "Confuse Trap", "items/chest_trap_confuse.xml" },
            { "Banner Trap", "items/chest_trap_banner.xml" },
            { "Fly Trap", "items/chest_trap_flies.xml" },
            { "Big Bronze Key", "items/key_bronze_big.xml" },
            { "Big Prison Bronze Key", "items/key_bronze_big_prison.xml" },
            { "Big Armory Bronze Key", "items/key_bronze_big_armory.xml" },
            { "Big Archives Bronze Key", "items/key_bronze_big_archives.xml" },
            { "Big Chambers Bronze Key", "items/key_bronze_big_chambers.xml" },
            { "Prison Bronze Key", "items/key_bronze_prison.xml" },
            { "Prison Silver Key", "items/key_silver_prison.xml" },
            { "Prison Gold Key", "items/key_gold_prison.xml" },
            { "Armory Bronze Key", "items/key_bronze_armory.xml" },
            { "Armory Silver Key", "items/key_silver_armory.xml" },
            { "Armory Gold Key", "items/key_gold_armory.xml" },
            { "Archives Bronze Key", "items/key_bronze_archives.xml" },
            { "Archives Silver Key", "items/key_silver_archives.xml" },
            { "Archives Gold Key", "items/key_gold_archives.xml" },
            { "Chambers Bronze Key", "items/key_bronze_chambers.xml" },
            { "Chambers Silver Key", "items/key_silver_chambers.xml" },
            { "Chambers Gold Key", "items/key_gold_chambers.xml" },
            { "Prison Bonus Key", "items/bonus_key_prison.xml" },
            { "Armory Bonus Key", "items/bonus_key_armory.xml" },
            { "Archives Bonus Key", "items/bonus_key_archives.xml" },
            { "Chambers Bonus Key", "items/bonus_key_chambers.xml" },
            { "Master Bronze Key", "items/floor_master_key_bronze.xml" },
            { "Master Silver Key", "items/floor_master_key_silver.xml" },
            { "Master Gold Key", "items/floor_master_key_gold.xml" },
            { "Master Bonus Key", "items/floor_master_key_bonus.xml" },
            { "Dune Sharks Arena Gold Key", "items/floor_master_key_gold.xml" },
            { "Dune Sharks Arena Silver Key", "items/floor_master_key_silver.xml" },
            { "Offense Shop Upgrade", "items/shop_item_off.xml" },
            { "Defense Shop Upgrade", "items/shop_item_def.xml" },
            { "Vitality Shop Upgrade", "items/shop_item_misc.xml" },
            { "Combo Shop Upgrade", "items/shop_item_combo.xml" },
            { "Miscellaneous Shop Upgrade", "items/shop_item_power.xml" },
            { "Gamble Shop Upgrade", "items/shop_item_gamble.xml" },
        };
        public static readonly Dictionary<string, string> trapNameToXML = new Dictionary<string, string>()
        {
            { "Bomb Trap", "prefabs/bomb_roof_trap.xml" },
            { "Mana Drain Trap", "items/bomb_drain.xml" },
            { "Poison Trap", "items/aptrap_poison.xml" },
            { "Frost Trap", "items/aptrap_frost.xml" },
            { "Fire Trap", "items/aptrap_fire.xml" },
            { "Confuse Trap", "items/aptrap_confuse.xml" },
            { "Fly Trap", "projectiles/enemy_mummy_1_mb.xml" },
        };
        public static readonly List<string> consumableItemXmlName = new List<string>()
        {
            "items/health_1.xml",
            "items/health_2.xml",
            "items/health_3.xml",
            "items/health_4.xml",
            "items/mana_1.xml",
            "items/mana_2.xml"
        };
        public static readonly string[] randomUpgradeXmlName = new string[]
        {
            "items/upgrade_damage.xml",
            "items/upgrade_defense.xml",
            "items/upgrade_health.xml",
            "items/upgrade_mana.xml"
        };
        public static readonly string[] foodItemNames = new[]
        {
            "Apple",
            "Orange",
            "Steak",
            "Fish",
        };

        public static readonly List<(string, string)> fuzzyItemNameToXML = new List<(string, string)>()
        {
        };
        public static readonly Dictionary<string, List<string[]>> gameFuzzyItemNameToXML = new Dictionary<string, List<string[]>>()
        {
            { "", new List<string[]>()
            {
                new string[]{ "Recipe", "doodads/generic/deco_bookstand_ground_paper_v3.xml" }, //We want recipes to be first before seeds and stuff

                new string[]{ "Sapling", "doodads/theme_e/e_deco_vgt_ivy_v4.xml" },
                new string[]{ "Branch", "doodads/theme_e/e_deco_vgt_ivy_v4.xml" },
                new string[]{ "Twig", "doodads/theme_e/e_deco_vgt_ivy_v4.xml" },
                new string[]{ "Stick", "doodads/theme_e/e_deco_vgt_ivy_v4.xml" },
                new string[]{ "Root", "doodads/theme_e/e_deco_vgt_ivy_v4.xml" },
                //new string[]{ "Seed", "doodads/theme_e/e_deco_vgt_v2.xml" },
                new string[]{ "Seed", "items/vgt_plant.xml:default" },
                new string[]{ "Plant", "items/vgt_plant.xml:default" },
                new string[]{ "Herb", "items/vgt_plant.xml:default" },
                new string[]{ "Flower", "items/vgt_plant.xml:default" },
                new string[]{ "Petal", "items/vgt_plant.xml:default" },

                new string[]{ "Bomb", "items/disp_bomb.xml" },
                new string[]{ "Missile", "items/disp_bomb.xml" },
                new string[]{ "Rocket", "items/disp_bomb.xml" },
                new string[]{ "Blast", "items/disp_bomb.xml" },
                new string[]{ "Bombchu", "items/disp_bomb.xml" },
                new string[]{ "Firecracker", "items/disp_bomb.xml" },
                new string[]{ "TNT", "items/disp_bomb.xml" },

                new string[]{ "Coins", "items/valuable_9.xml" },

                //new string[]{ "Bonus Chest", "items/bonus_chest.xml" },
                //{ "Bonus Key", "items/bonus_key.xml" },
                //{ "Blue Chest", "items/chest_blue.xml" },
                //{ "Green Chest", "items/chest_green.xml" },
                //{ "Purple Chest", "items/chest_purple.xml" },
                //{ "Red Chest", "items/chest_red.xml" },
                new string[]{ "Relic", "items/chest_green.xml" },
                new string[]{ "Chest", "items/chest_wood.xml" },
                new string[]{ "Coin", "items/collectable_1.xml" },

                new string[]{ "Boss Key", "items/key_gold.xml" },
                new string[]{ "Big Key", "items/key_gold.xml" },
                new string[]{ "Simple Key", "items/key_silver.xml" },
                new string[]{ "Key", "items/key_bronze.xml" },

                new string[]{ "Shortcut", "items/key_bronze.xml" },

                new string[]{ "Mirror", "items/key_mirror.xml" },
                new string[]{ "Glass", "items/key_mirror.xml" },
                new string[]{ "Lens", "items/key_mirror.xml" },
                new string[]{ "Ore", "items/key_ore.xml" },
                //{ "Rune Key", "items/key_teleport.xml" },
                //{ "1-Up Ankh", "items/powerup_1up.xml" },
                //{ "5-Up Ankh", "items/powerup_5up.xml" },
                //{ "7-Up Ankh", "items/powerup_7up.xml" },
                //{ "Damage Potion", "items/powerup_potion3.xml" },
                new string[]{ "Potion", "items/powerup_potion2.xml" }, //Yellow
                new string[]{ "Ambrosia", "items/powerup_potion3.xml" }, //Red
                new string[]{ "Juice", "items/powerup_potion1.xml" }, //Purple
                new string[]{ "Bottle", "items/powerup_potion2.xml" },
                new string[]{ "Flask", "items/powerup_potion2.xml" },
                new string[]{ "Lemonade", "items/powerup_potion2.xml" },
                new string[]{ "Tea", "items/powerup_potion2.xml" },
                new string[]{ "Tank", "items/powerup_potion2.xml" },
                new string[]{ "Jar", "items/powerup_potion2.xml" },
                new string[]{ "Brew", "items/powerup_potion3.xml" },
                new string[]{ "Vessel", "items/powerup_potion3.xml" },

                new string[]{ "Diamond", "items/valuable_diamond.xml" },
                new string[]{ "Ruby", "items/valuable_diamond_red.xml" },

                new string[]{ "Sword", "items/upgrade_damage.xml" },
                new string[]{ "sword", "items/upgrade_damage.xml" }, //To catch items such as Longsword or Greatsword
                new string[]{ "Nail", "items/upgrade_damage.xml" },
                new string[]{ "Blade", "items/upgrade_damage.xml" },
                new string[]{ "Dagger", "items/upgrade_damage.xml" },
                new string[]{ "Dirk", "items/upgrade_damage.xml" },
                new string[]{ "Edge", "items/upgrade_damage.xml" },
                new string[]{ "Excalibur", "items/upgrade_damage.xml" },
                new string[]{ "Falchion", "items/upgrade_damage.xml" },
                new string[]{ "Katana", "items/upgrade_damage.xml" },
                new string[]{ "Knife", "items/upgrade_damage.xml" },
                new string[]{ "Machete", "items/upgrade_damage.xml" },
                new string[]{ "Saber", "items/upgrade_damage.xml" },
                new string[]{ "Slash", "items/upgrade_damage.xml" },
                new string[]{ "Weapon", "items/upgrade_damage.xml" },
                new string[]{ "Spear", "items/upgrade_damage.xml" },
                new string[]{ "Halberd", "items/upgrade_damage.xml" },
                new string[]{ "Trident", "items/upgrade_damage.xml" },

                new string[]{ "Combat", "items/upgrade_damage.xml" },

                new string[]{ "Shield", "items/upgrade_defense.xml" },
                new string[]{ "Plate", "items/upgrade_defense.xml" },
                new string[]{ "Aegis", "items/upgrade_defense.xml" },
                new string[]{ "Buckler", "items/upgrade_defense.xml" },
                new string[]{ "Armor", "items/upgrade_defense.xml" },
                new string[]{ "Mail", "items/upgrade_defense.xml" },
                new string[]{ "Suit", "items/upgrade_defense.xml" },
                new string[]{ "Defend", "items/upgrade_defense.xml" },
                new string[]{ "Defense", "items/upgrade_defense.xml" },

                new string[]{ "Bow", "doodads/generic/deco_corpse_weapon_bow.xml" },
                new string[]{ "bow", "doodads/generic/deco_corpse_weapon_bow.xml" }, //To catch items such as Shortbow or Longbow
                new string[]{ "Ballista", "doodads/generic/deco_corpse_weapon_bow.xml" },

                //ALttP/OoT
                new string[]{ "Rupees (300)", "items/valuable_diamond_red.xml" },
                new string[]{ "Rupees (200)", "items/valuable_diamond_red.xml" },
                new string[]{ "Rupees (100)", "items/valuable_diamond.xml" },
                new string[]{ "Rupees (20)", "items/valuable_diamond_small_red.xml" },

                //LA
                new string[]{ "500 Rupees", "items/valuable_diamond_red.xml" },
                new string[]{ "100 Rupees", "items/valuable_diamond.xml" },
                new string[]{ "20 Rupees", "items/valuable_diamond_small_red.xml" },

                new string[]{ "Five Rupees", "items/valuable_diamond_small_red.xml" }, //LoZ

                new string[]{ "Rupee", "items/valuable_diamond_small.xml" },

                //
                new string[]{ "Soul", "actors/wisp_1.xml:south" },
                new string[]{ "Essence", "actors/wisp_2.xml:south" },

                new string[]{ "Heal", "items/upgrade_health.xml" },
                new string[]{ "Life", "items/upgrade_health.xml" },
                new string[]{ "Cure", "items/upgrade_health.xml" },
                new string[]{ "HP", "items/upgrade_health.xml" },

                new string[]{ "MP", "items/upgrade_mana.xml" },

                new string[]{ "Parry", "items/upgrade_damage.xml" },

                new string[]{ "Diamond", "items/valuable_diamond.xml" },
                new string[]{ "Ruby", "items/valuable_diamond_red.xml" },
                new string[]{ "Emerald", "items/valuable_diamond.xml" },
                new string[]{ "Gem", "items/valuable_diamond.xml" },
                new string[]{ "Jewel", "items/valuable_diamond.xml" },

                //{ "Mana Upgrade", "items/upgrade_mana.xml" },
                //{ "Copper Coin", "items/valuable_1.xml" },
                //{ "Copper Coins", "items/valuable_2.xml" },
                //{ "Copper Coin Pile", "items/valuable_3.xml" },
                //{ "Silver Coin", "items/valuable_4.xml" },
                //{ "Silver Coins", "items/valuable_5.xml" },
                //{ "Gold Coin", "items/valuable_7.xml" },
                //{ "Gold Coins", "items/valuable_8.xml" },
                new string[]{ "Currency", "items/valuable_9.xml" },
                new string[]{ "Money", "items/valuable_9.xml" },
                new string[]{ "Zenny", "items/valuable_9.xml" },

                new string[]{ "Apple", "items/health_1.xml" },
                new string[]{ "Fruit", "items/health_1.xml" },
                new string[]{ "Orange", "items/health_2.xml" },
                new string[]{ "Steak", "items/health_3.xml" },
                new string[]{ "Meat", "items/health_3.xml" },
                new string[]{ "Fish", "items/health_4.xml" },
                new string[]{ "Trout", "items/health_4.xml" },

                new string[]{ "Pan", "items/tool_pan.xml" },
                new string[]{ "Lever", "items/tool_lever.xml" },
                new string[]{ "Pumps", "items/tool_lever.xml" },
                new string[]{ "Shovel", "items/tool_shovel.xml" },
                new string[]{ "Dig", "items/tool_shovel.xml" },
                new string[]{ "Hookshot", "items/tool_shovel.xml" },
                new string[]{ "Grapple", "items/tool_shovel.xml" },
                new string[]{ "Grappling", "items/tool_shovel.xml" },
                new string[]{ "Club", "items/tool_shovel.xml" },
                new string[]{ "Hammer", "items/tool_hammer.xml" },

                new string[]{ "Pickaxe", "items/tool_pickaxe.xml" },
                new string[]{ "Axe", "items/tool_pickaxe.xml" },
                new string[]{ "Claw", "items/tool_pickaxe.xml" },
                new string[]{ "Hook", "items/tool_pickaxe.xml" },
                new string[]{ "Scythe", "items/tool_pickaxe.xml" },
                new string[]{ "Mining", "items/tool_pickaxe.xml" },

                new string[]{ "Ring", "items/special_ring.xml" },
                new string[]{ "Heart", "items/special_serious_health.xml" },
                new string[]{ "Friend", "items/special_serious_health.xml" },
                new string[]{ "Badge", "items/crystal_green.xml" },
                new string[]{ "Rosary", "items/crystal_red.xml" },
                new string[]{ "Crystal", "items/crystal_purple.xml" },
                //{ "Offense Shop Upgrade", "items/shop_item_off.xml" },
                //{ "Defense Shop Upgrade", "items/shop_item_def.xml" },
                //{ "Vitality Shop Upgrade", "items/shop_item_misc.xml" },
                //{ "Combo Shop Upgrade", "items/shop_item_combo.xml" },
                //{ "Miscellaneous Shop Upgrade", "items/shop_item_power.xml" },
                //{ "Gamble Shop Upgrade", "items/shop_item_gamble.xml" },

                new string[]{ "Arrow", "items/disp_gold_arrows.xml" },
                new string[]{ "Quiver", "items/disp_gold_arrows.xml" },
                new string[]{ "Note", "items/disp_note.xml" },
                new string[]{ "Music", "items/disp_note.xml" },
                new string[]{ "Song", "items/disp_note.xml" },
                new string[]{ "Minuet", "items/disp_note.xml" },
                new string[]{ "Bolero", "items/disp_note.xml" },
                new string[]{ "Serenade", "items/disp_note.xml" },
                new string[]{ "Requiem", "items/disp_note.xml" },
                new string[]{ "Nocturne", "items/disp_note.xml" },
                new string[]{ "Prelude", "items/disp_note.xml" },
                new string[]{ "Lullaby", "items/disp_note.xml" },

                new string[]{ "Card", "doodads/generic/deco_bookstand_ground_paper_v1.xml" },
                new string[]{ "Map", "doodads/generic/deco_bookstand_ground_paper_v3.xml" },
                new string[]{ "Letter", "doodads/generic/deco_bookstand_ground_paper_v3.xml" },
                new string[]{ "Message", "doodads/generic/deco_bookstand_ground_paper_v3.xml" },
                new string[]{ "Tablet", "doodads/generic/deco_bookstand_ground_paper_v3.xml" },
                new string[]{ "Scroll", "doodads/generic/deco_bookstand_ground_paper_v3.xml" },

                new string[]{ "Contract", "doodads/generic/deco_bookstand_ground_paper_v3.xml" }, //Cuphead

                new string[]{ "Book", "doodads/generic/deco_bookstand_ground_book.xml" },
                new string[]{ "Journal", "doodads/generic/deco_bookstand_ground_book.xml" },
                new string[]{ "Lore", "doodads/generic/deco_bookstand_ground_book.xml" },
                new string[]{ "Guide", "doodads/generic/deco_bookstand_ground_book.xml" },
                new string[]{ "Tome", "doodads/generic/deco_bookstand_ground_book.xml" },

                new string[]{ "Bin", "doodads/generic/deco_vendor_cart.xml" }, //SDV
                new string[]{ "Cart", "doodads/generic/deco_vendor_cart.xml" },
                new string[]{ "Wagon", "doodads/generic/deco_vendor_cart.xml" },
                new string[]{ "Caravan", "doodads/generic/deco_vendor_cart.xml" },
                new string[]{ "Minecart", "doodads/generic/deco_vendor_cart.xml" },

                new string[]{ "Mask", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Skull", "doodads/generic/deco_corpse_skull_v4.xml" },

                new string[]{ "Orb", "items/crystal_purple.xml" },

                new string[]{ "Grub", "actors/maggot_1_small.xml:northeast" },
                new string[]{ "Slime", "actors/slime_1_spawn.xml:north" },

                new string[]{ "Banner", "actors/tower_banner_1.xml:north" },
                new string[]{ "Scarecrow", "actors/tower_banner_3.xml:north" },
                new string[]{ "Obelisk", "actors/tower_banner_2.xml:north" },

                new string[]{ "Pot", "items/breakable_vase_v4.xml" },
                new string[]{ "Vase", "items/breakable_vase_v4.xml" },
                new string[]{ "Urn", "items/breakable_vase_v4.xml" },
                new string[]{ "Basin", "items/breakable_vase_v4.xml" },
                new string[]{ "Bowl", "items/breakable_vase_v4.xml" },

                new string[]{ "Bone", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "bone", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Vertebra", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Jaw", "doodads/generic/deco_corpse_skull_v4.xml" },

                new string[]{ "Bird", "doodads/generic/seylyns_seagull2.xml" },
                new string[]{ "gull", "doodads/generic/seylyns_seagull2.xml" },
                new string[]{ "Cuckoo", "doodads/generic/seylyns_seagull2.xml" },
                new string[]{ "Rooster", "doodads/generic/seylyns_seagull2.xml" },
                new string[]{ "Chicken", "doodads/generic/seylyns_seagull2.xml" },
                new string[]{ "Duck", "doodads/generic/seylyns_seagull2.xml" },

                new string[]{ "Day", "doodads/generic/exit_teleport_boss_desert_activated.xml" },
                new string[]{ "Sun", "doodads/generic/exit_teleport_boss_desert_activated.xml" },

                new string[]{ "Merchant", "doodads/special/vendor_power.xml" },
                new string[]{ "Shop", "doodads/special/vendor_power.xml" },
                new string[]{ "Vendor", "doodads/special/vendor_power.xml" },

                new string[]{ "Statue", "effects/warlock_gargoyle.xml:gargoyle" },

                new string[]{ "Flame", "projectiles/player_fireball_6.xml:6" },
                new string[]{ "Fire", "projectiles/player_fireball_6.xml:6" },
                new string[]{ "Ember", "projectiles/player_fireball_4.xml:6" },
                new string[]{ "Cinder", "projectiles/player_fireball_4.xml:6" },

                new string[]{ "Shard", "doodads/generic/deco_crystal_small.xml" },

                //Adjectives
                new string[]{ "Repair", "items/tool_hammer.xml" },
                new string[]{ "Solar", "doodads/generic/exit_teleport_boss_desert_activated.xml" },
                new string[]{ "Bronze", "items/valuable_3.xml" },
                new string[]{ "Silver", "items/valuable_6.xml" },
                new string[]{ "Gold", "items/valuable_9.xml" },
                new string[]{ "Wood", "items/collectable_2.xml" },
                new string[]{ "Stone", "doodads/theme_g/g_deco_ground_stone_v2.xml" },
                new string[]{ "Rock", "doodads/theme_g/g_deco_ground_stone_v2.xml" },
            }},
            { "Cuphead", new List<string[]>()
            {
                new string[]{ "Coin", "items/collectable_1.xml" }, //So all multiples of coins appear as vendor coins

                new string[]{"Peashooter", "items/powerup_potion2.xml"}, //Yellow
                new string[]{"Chaser", "items/powerup_potion2.xml"}, //Yellow
                new string[]{"Converge", "items/powerup_potion2.xml"}, //Yellow
                new string[]{"Spread", "items/powerup_potion3.xml"}, //Red
                new string[]{"Charge", "items/powerup_potion3.xml"}, //Red
                new string[]{"Crackshot", "items/powerup_potion3.xml"}, //Red
                new string[]{"Roundabout", "items/powerup_potion1.xml"}, //Purple
                new string[]{"Lobber", "items/powerup_potion1.xml"}, //Purple
                new string[]{"Twist-Up", "items/powerup_potion1.xml"}, //Purple

                new string[]{ "Super", "doodads/generic/deco_bookstand_ground_paper_v1.xml" },
            }},
            { "Cave Story", new List<string[]>()
            {
                new string[]{"Life Pot", "items/powerup_potion3.xml"}, //Red
            }},
            { "Paper Mario", new List<string[]>()
            {
                new string[]{ "Ultra Stone", "items/crystal_red.xml" },
            }},
            { "The Messenger", new List<string[]>()
            {
                new string[]{ "Shard", "items/valuable_diamond_small.xml" },
                new string[]{ "Key", "items/disp_note.xml" },
                new string[]{ "Thistle", "items/vgt_plant.xml:default" },
                new string[]{ "Power Seal", "items/crystal_green.xml" },

                new string[]{ "Windmill Shuriken", "items/upgrade_damage_2.xml" },

                new string[]{ "Mind", "items/shop_item_misc.xml" },
                new string[]{ "Bodies", "items/shop_item_misc.xml" },
                new string[]{ "Meditation", "items/shop_item_misc.xml" },
                new string[]{ "Second Wind", "items/shop_item_misc.xml" },
                new string[]{ "Currents Master", "items/shop_item_misc.xml" },
                new string[]{ "Devil's Due", "items/shop_item_power.xml" },

                new string[]{ "Rejuvenative Spirit", "items/shop_item_def.xml" },
                new string[]{ "Jacket", "items/shop_item_def.xml" },
                new string[]{ "Plates", "items/shop_item_def.xml" },
                new string[]{ "Path of Resilience", "items/shop_item_def.xml" },

                new string[]{ "Shuriken", "items/shop_item_off.xml" },
                new string[]{ "Strike of the Ninja", "items/shop_item_off.xml" },
                new string[]{ "Aerobatics Warrior", "items/shop_item_off.xml" },
                new string[]{ "Demon's Bane", "items/shop_item_off.xml" },

                new string[]{ "Sense", "items/shop_item_combo.xml" },

                new string[]{ "Money Wrench", "items/bonus_key.xml" },

                new string[]{ "Figurine", "items/breakable_vase_v4.xml" },

                new string[]{ "Rope Dart", "items/tool_shovel.xml" },
            }},
            { "Hollow Knight", new List<string[]>()
            {
                new string[]{ "Geo", "items/valuable_6.xml" },
                new string[]{ "slash", "items/upgrade_damage.xml" }, //For leftslash, rightslash, etc
                new string[]{ "Lifeblood", "items/mana_2.xml" },
                new string[]{ "Soul_Totem", "items/mana_1.xml" },
                new string[]{ "Mask_Shard", "items/upgrade_health.xml" },
                new string[]{ "Vessel", "items/upgrade_mana.xml" },
                new string[]{ "Lurien", "doodads/generic/deco_corpse_skull_v4.xml" }, //Mask
                new string[]{ "Monomon", "doodads/generic/deco_corpse_skull_v4.xml" }, //Mask
                new string[]{ "Herrah", "doodads/generic/deco_corpse_skull_v4.xml" }, //Mask
            }},
            { "Tunic", new List<string[]>()
            {
                new string[]{ "Aura's Gem", "items/upgrade_defense.xml" },
                new string[]{ "Lucky Cup", "items/upgrade_health.xml" },
                new string[]{ "Inverted Ash", "items/powerup_potion2.xml" }, //Yellow
                new string[]{ "Spring Falls", "items/vgt_plant.xml:default" },
            }},
            { "Blasphemous", new List<string[]>()
            {
                new string[]{ "Calcaneum", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Capitate", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Clavicle", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Coccyx", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Coxal", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Femur", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Fibula", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Fibula", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Frontal", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Hamate", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Humerus", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Hyoid", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Jaw", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Kneecap", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Lunate", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Maxilla", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Metacarpus", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Metatarsus", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Navicular", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Occipital", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Phalanx", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Pisiform", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Radius", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Rib", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Sacrum of the Dark Warlock", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Scaphoid", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Scapula", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Sternum", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Temporal", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Tibia", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Trapezium", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Trapezoid", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Triquetral", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Ulna", "doodads/generic/deco_corpse_skull_v4.xml" },
                new string[]{ "Vertebra", "doodads/generic/deco_corpse_skull_v4.xml" },
            }},
            { "Stardew Valley", new List<string[]>()
            {
                new string[]{ "Rarecrow", "actors/tower_banner_3.xml:north" },
            }},
        };
        public static string GetItemNameFromXml(string xmlName)
        {
            string itemName = "";
            foreach (KeyValuePair<string, string> item in itemNameToXML)
            {
                if (item.Value != xmlName) continue;
                itemName = item.Key;
                break;
            }
            return itemName;
        }
        public static string GetAPXmlItemFromItemFlags(ItemFlags flags)
        {
            if ((flags & ItemFlags.Advancement) == ItemFlags.Advancement)
            {
                return archipelagoItemXmlName;
            }
            if ((flags & ItemFlags.NeverExclude) == ItemFlags.NeverExclude)
            {
                return archipelagoUsefulItemXmlName;
            }
            if ((flags & ItemFlags.Trap) == ItemFlags.Trap) //If the item is a trap we choose a random item to use
            {
                Random random = new Random();
                string[] apItems = { archipelagoItemXmlName, archipelagoUsefulItemXmlName, archipelagoFillerItemXmlName };
                return apItems[random.Next(apItems.Length)];
            }
            return archipelagoFillerItemXmlName;
        }
        public static string GetAPHoloXmlItemFromItemFlags(ItemFlags flags, string trueItemXmlName)
        {
            //If this is an offworld item we don't have a sprite for then we want the holo effect to always be generic
            switch (trueItemXmlName)
            {
                case archipelagoFillerItemXmlName:
                    return apHoloFillerItemXmlName;
                case archipelagoItemXmlName:
                    return apHoloProgItemXmlName;
                case archipelagoUsefulItemXmlName:
                case "items/chest_trap_banner.xml": //Purple chests are classified as useful, if the trap was filler
                    return apHoloItemXmlName;
            }
            //Force the other chests to be always filler to match the chest items
            if (trueItemXmlName != null && trueItemXmlName.StartsWith("items/chest_trap_"))
                return apHoloFillerItemXmlName;
            if ((flags & ItemFlags.Advancement) == ItemFlags.Advancement)
            {
                return apHoloProgItemXmlName;
            }
            if ((flags & ItemFlags.NeverExclude) == ItemFlags.NeverExclude)
            {
                return apHoloItemXmlName;
            }
            if ((flags & ItemFlags.Trap) == ItemFlags.Trap) //If the item is a trap we choose a random item to use
            {
                Random random = new Random();
                string[] apItems = { apHoloProgItemXmlName, apHoloItemXmlName, apHoloFillerItemXmlName };
                return apItems[random.Next(apItems.Length)];
            }
            return apHoloFillerItemXmlName;
        }

        public static readonly Dictionary<string, Dictionary<int, int>> castleGateIDLookup = new Dictionary<string, Dictionary<int, int>>
        {
            {"level_1.xml", new Dictionary<int, int>{
            { 2175, 0 },
            { 2174, 0 },
            { 2176, 0 },
            { 2510, 1 },
            { 2511, 1 },
            { 2999, 2 },
            { 3000, 2 },
            { 3001, 2 },
            { 3002, 2 },
            { 4136, 3 },
            { 4137, 3 },
            }},
            {"level_2.xml", new Dictionary<int, int>{
            { 2868, 0 },
            { 2869, 0 },
            { 2867, 0 },
            { 5637, 1 },
            { 5638, 1 },
            { 5636, 1 },
            { 6338, 2 },
            { 6339, 2 },
            { 6337, 2 },
            { 4013, 3 },
            { 4015, 3 },
            { 4016, 3 },
            { 4014, 3 },
            { 5408, 4 },
            { 5410, 4 },
            { 5411, 4 },
            { 5412, 4 },
            { 5409, 4 },
            { 2493, 5 },
            { 2494, 5 },
            { 2495, 5 },
            { 2496, 5 },
            { 5830, 6 },
            { 5829, 6 },
            { 6055, 7 },
            { 6054, 7 },
            }},
            {"level_3.xml", new Dictionary<int, int>{
            { 1792, 0 },
            { 1788, 0 },
            { 1787, 0 },
            { 1786, 0 },
            { 1794, 0 },
            { 1793, 1 },
            { 1791, 1 },
            { 1790, 1 },
            { 1789, 1 },
            { 1795, 1 },
            { 7292, 2 },
            { 7291, 2 },
            { 7290, 2 },
            { 7289, 2 },
            { 7422, 2 },
            { 2052, 3 },
            { 3278, 3 },
            { 3280, 3 },
            { 3277, 3 },
            { 3279, 3 },
            { 3276, 4 },
            { 3274, 4 },
            { 3275, 4 },
            { 3452, 4 },
            { 6276, 5 },
            { 6275, 5 },
            { 6277, 5 },
            }},
            {"level_bonus_1.xml", new Dictionary<int, int>{
            }},
            {"level_boss_1.xml", new Dictionary<int, int>{
            }},
            {"level_4.xml", new Dictionary<int, int>{
            { 300, 0 },
            { 299, 0 },
            { 298, 0 },
            { 2752, 1 },
            { 2751, 1 },
            { 2750, 1 },
            { 4277, 2 },
            { 4280, 2 },
            { 4276, 2 },
            { 4275, 2 },
            { 6209, 3 },
            { 6208, 3 },
            { 6207, 3 },
            { 7234, 4 },
            { 7242, 4 },
            { 7243, 4 },
            { 7244, 4 },
            { 7233, 4 },
            { 7318, 5 },
            { 7344, 5 },
            { 7317, 5 },
            { 7316, 5 },
            { 4713, 6 },
            { 4712, 6 },
            { 4714, 6 },
            { 6329, 7 },
            { 6333, 7 },
            { 6331, 7 },
            { 6330, 8 },
            { 6334, 8 },
            { 6332, 8 },
            { 659, 9 },
            { 658, 9 },
            { 1731, 10 },
            { 1730, 10 },
            { 3172, 11 },
            { 3171, 11 },
            { 4711, 12 },
            { 4710, 12 },
            { 5377, 13 },
            { 5376, 13 },
            { 5844, 14 },
            { 5843, 14 },
            { 1226, 15 },
            { 1227, 15 },
            { 1225, 15 },
            }},
            {"level_5.xml", new Dictionary<int, int>{
            { 1805, 0 },
            { 1803, 0 },
            { 1801, 0 },
            { 1806, 1 },
            { 1804, 1 },
            { 1802, 1 },
            { 5301, 2 },
            { 5300, 2 },
            { 5302, 2 },
            { 5299, 2 },
            { 6206, 3 },
            { 6207, 3 },
            { 6205, 3 },
            { 6204, 3 },
            { 6903, 4 },
            { 6904, 4 },
            { 6905, 4 },
            { 6902, 4 },
            { 6901, 4 },
            { 7028, 5 },
            { 7029, 5 },
            { 7027, 5 },
            { 7026, 5 },
            { 4154, 6 },
            { 4256, 6 },
            { 4257, 6 },
            { 1338, 7 },
            { 3435, 7 },
            { 2104, 8 },
            { 2107, 8 },
            { 2108, 8 },
            { 3547, 9 },
            { 3548, 9 },
            { 6640, 10 },
            { 6641, 10 },
            }},
            {"level_bonus_2.xml", new Dictionary<int, int>{
            }},
            {"level_6.xml", new Dictionary<int, int>{
            { 1065, 0 },
            { 1061, 0 },
            { 1063, 0 },
            { 1066, 1 },
            { 1062, 1 },
            { 1064, 1 },
            { 2682, 2 },
            { 2680, 2 },
            { 2685, 2 },
            { 2681, 2 },
            { 6114, 3 },
            { 6112, 3 },
            { 6115, 3 },
            { 6113, 3 },
            { 3439, 4 },
            { 3441, 4 },
            { 3440, 4 },
            { 3899, 5 },
            { 4359, 5 },
            { 5106, 6 },
            { 5108, 6 },
            { 5107, 6 },
            { 5109, 6 },
            { 5309, 6 },
            }},
            {"level_boss_2.xml", new Dictionary<int, int>{
            }},
            {"level_7.xml", new Dictionary<int, int>{
            { 242, 0 },
            { 241, 0 },
            { 243, 0 },
            { 1501, 1 },
            { 1499, 1 },
            { 1500, 1 },
            { 1512, 1 },
            { 1502, 1 },
            { 5437, 2 },
            { 5436, 2 },
            { 5438, 2 },
            { 1513, 3 },
            { 1514, 3 },
            { 1515, 3 },
            { 1870, 3 },
            { 1869, 3 },
            { 1871, 3 },
            { 4723, 4 },
            { 4727, 4 },
            { 4726, 4 },
            { 4725, 4 },
            { 4724, 4 },
            { 4738, 4 },
            { 5004, 5 },
            { 5011, 5 },
            { 5007, 5 },
            { 5006, 5 },
            { 5009, 5 },
            { 5005, 6 },
            { 5012, 6 },
            { 5008, 6 },
            { 5010, 6 },
            { 5468, 7 },
            { 5466, 7 },
            { 5467, 7 },
            { 5470, 7 },
            { 5469, 7 },
            { 2974, 8 },
            { 2972, 8 },
            { 2973, 8 },
            { 6082, 9 },
            { 6081, 9 },
            { 4155, 10 },
            { 4156, 10 },
            { 4157, 10 },
            { 4353, 11 },
            { 4354, 11 },
            { 6269, 11 },
            }},
            {"level_8.xml", new Dictionary<int, int>{
            { 3273, 0 },
            { 3274, 0 },
            { 3275, 0 },
            { 4024, 1 },
            { 4025, 1 },
            { 4027, 1 },
            { 4028, 1 },
            { 4029, 1 },
            { 4030, 1 },
            { 4031, 1 },
            { 4032, 1 },
            { 4026, 1 },
            { 5299, 2 },
            { 5300, 2 },
            { 5302, 2 },
            { 5301, 2 },
            { 8386, 3 },
            { 8387, 3 },
            { 8389, 3 },
            { 8388, 3 },
            { 3464, 4 },
            { 3462, 4 },
            { 3463, 4 },
            { 7085, 5 },
            { 7083, 5 },
            { 7086, 5 },
            { 7082, 5 },
            { 7084, 5 },
            { 1472, 6 },
            { 1473, 6 },
            { 1474, 6 },
            { 3059, 7 },
            { 3061, 7 },
            { 3060, 7 },
            { 3062, 7 },
            { 3063, 7 },
            { 3064, 7 },
            { 3065, 7 },
            { 3634, 8 },
            { 3636, 8 },
            { 3635, 8 },
            }},
            {"level_9.xml", new Dictionary<int, int>{
            { 1660, 0 },
            { 1656, 0 },
            { 1657, 0 },
            { 1658, 0 },
            { 1659, 0 },
            { 1860, 0 },
            { 1861, 0 },
            { 9340, 1 },
            { 9339, 1 },
            { 9337, 1 },
            { 9338, 1 },
            { 9336, 1 },
            { 9335, 1 },
            { 9334, 1 },
            { 9333, 1 },
            { 9636, 1 },
            { 9635, 1 },
            { 9634, 1 },
            { 9637, 1 },
            { 11218, 2 },
            { 11217, 2 },
            { 11216, 2 },
            { 11219, 2 },
            { 11220, 2 },
            { 6006, 3 },
            { 6005, 3 },
            { 6004, 3 },
            { 6003, 3 },
            { 6002, 3 },
            { 9973, 4 },
            { 9971, 4 },
            { 10733, 4 },
            { 10732, 4 },
            { 9974, 5 },
            { 9972, 5 },
            { 9975, 5 },
            { 2466, 6 },
            { 2465, 6 },
            { 6485, 6 },
            { 3402, 7 },
            { 3398, 7 },
            { 3399, 7 },
            { 3400, 7 },
            { 3401, 7 },
            { 6001, 8 },
            { 6000, 8 },
            { 6489, 9 },
            { 6486, 9 },
            { 6487, 9 },
            { 6488, 9 },
            { 6978, 9 },
            { 6979, 9 },
            { 6980, 9 },
            { 11389, 10 },
            { 11388, 10 },
            }},
            {"level_bonus_3.xml", new Dictionary<int, int>{
            }},
            {"level_boss_3.xml", new Dictionary<int, int>{
            }},
            {"level_10.xml", new Dictionary<int, int>{
            { 84, 0 },
            { 85, 0 },
            { 86, 0 },
            { 209, 1 },
            { 210, 1 },
            { 211, 1 },
            { 3168, 2 },
            { 3246, 2 },
            { 3169, 2 },
            { 3535, 3 },
            { 3536, 3 },
            { 5017, 4 },
            { 5064, 4 },
            { 5175, 4 },
            { 5140, 4 },
            { 5311, 5 },
            { 5312, 5 },
            { 5829, 6 },
            { 5841, 6 },
            { 5830, 6 },
            { 5831, 6 },
            { 1051, 7 },
            { 1052, 7 },
            { 1053, 7 },
            { 2686, 8 },
            { 2687, 8 },
            { 2692, 8 },
            { 2691, 8 },
            { 2690, 8 },
            { 2689, 8 },
            { 2688, 8 },
            { 3245, 9 },
            { 3714, 9 },
            { 3575, 10 },
            { 3576, 10 },
            { 3577, 10 },
            { 4470, 11 },
            { 4471, 11 },
            { 4472, 11 },
            { 4473, 11 },
            { 4474, 11 },
            { 1315, 12 },
            { 1312, 12 },
            { 1313, 12 },
            { 1314, 12 },
            { 4897, 13 },
            { 4898, 13 },
            { 4899, 13 },
            }},
            {"level_10_special.xml", new Dictionary<int, int>{
            }},
            {"level_11.xml", new Dictionary<int, int>{
            { 1472, 0 },
            { 1475, 0 },
            { 1473, 0 },
            { 1474, 0 },
            { 1592, 0 },
            { 1606, 0 },
            { 5112, 1 },
            { 5113, 1 },
            { 5115, 1 },
            { 5114, 1 },
            { 5147, 1 },
            { 7618, 2 },
            { 7619, 2 },
            { 7620, 2 },
            { 7816, 3 },
            { 7818, 3 },
            { 7817, 3 },
            { 8504, 4 },
            { 8551, 4 },
            { 8507, 4 },
            { 8506, 4 },
            { 8505, 4 },
            { 8549, 4 },
            { 9178, 5 },
            { 9209, 5 },
            { 9179, 5 },
            { 9180, 5 },
            { 9269, 5 },
            { 9268, 5 },
            { 9292, 5 },
            { 9883, 6 },
            { 9934, 6 },
            { 9886, 6 },
            { 9885, 6 },
            { 9884, 6 },
            { 9933, 6 },
            { 10450, 7 },
            { 10451, 7 },
            { 10480, 7 },
            { 10479, 7 },
            { 13040, 8 },
            { 13074, 8 },
            { 13043, 8 },
            { 13042, 8 },
            { 13041, 8 },
            { 13073, 8 },
            { 4915, 9 },
            { 4913, 9 },
            { 4914, 9 },
            { 4916, 9 },
            { 6782, 10 },
            { 7526, 10 },
            { 7527, 10 },
            { 7141, 11 },
            { 7142, 11 },
            { 7143, 11 },
            { 1350, 12 },
            { 5309, 12 },
            { 2071, 13 },
            { 2107, 13 },
            { 2108, 13 },
            { 2109, 13 },
            { 2110, 13 },
            { 6335, 14 },
            { 6353, 14 },
            { 6354, 14 },
            { 7488, 15 },
            { 9774, 15 },
            { 9775, 15 },
            { 7700, 16 },
            { 7703, 16 },
            { 8214, 17 },
            { 8227, 17 },
            { 8228, 17 },
            { 11465, 18 },
            { 11483, 18 },
            { 11941, 18 },
            { 11942, 18 },
            }},
            {"level_bonus_4.xml", new Dictionary<int, int>{
            }},
            {"level_12.xml", new Dictionary<int, int>{
            { 354, 0 },
            { 355, 0 },
            { 500, 0 },
            { 526, 0 },
            { 2034, 1 },
            { 2044, 1 },
            { 2642, 2 },
            { 2645, 2 },
            { 2644, 2 },
            { 2643, 2 },
            { 2703, 2 },
            { 2685, 2 },
            { 3276, 3 },
            { 3278, 3 },
            { 3277, 3 },
            { 3312, 3 },
            { 3614, 4 },
            { 3615, 4 },
            { 3616, 4 },
            { 3669, 4 },
            { 3655, 4 },
            { 5231, 5 },
            { 5243, 5 },
            { 5235, 5 },
            { 6547, 6 },
            { 6597, 6 },
            { 6612, 6 },
            { 6875, 7 },
            { 6876, 7 },
            { 6914, 7 },
            { 8798, 8 },
            { 8800, 8 },
            { 8859, 8 },
            { 8799, 8 },
            { 8831, 8 },
            { 4018, 9 },
            { 4019, 9 },
            { 4020, 9 },
            { 4154, 9 },
            { 4155, 9 },
            { 1315, 10 },
            { 1316, 10 },
            { 1317, 10 },
            { 4521, 11 },
            { 4522, 11 },
            { 4523, 11 },
            { 4768, 12 },
            { 4769, 12 },
            { 4770, 12 },
            { 6944, 13 },
            { 6945, 13 },
            { 6946, 13 },
            { 8489, 14 },
            { 8490, 14 },
            { 8491, 14 },
            { 8492, 14 },
            }},
            {"level_boss_4.xml", new Dictionary<int, int>{
            }},
            {"level_esc_1.xml", new Dictionary<int, int>{
            }},
            {"level_esc_2.xml", new Dictionary<int, int>{
            }},
            {"level_esc_3.xml", new Dictionary<int, int>{
            { 299, 0 },
            { 298, 0 },
            { 297, 0 },
            { 531, 1 },
            { 530, 1 },
            { 1532, 2 },
            { 1531, 2 },
            { 928, 3 },
            { 929, 3 },
            { 927, 3 },
            }},
            {"level_esc_4.xml", new Dictionary<int, int>{
            }},
        };
        public static readonly Dictionary<string, Dictionary<int, int>> templeGateIDLookup = new Dictionary<string, Dictionary<int, int>>
        {
            {"level_hub.xml", new Dictionary<int, int>{
            }},
            {"level_library.xml", new Dictionary<int, int>{
            }},
            {"level_cave_1.xml", new Dictionary<int, int>{
            }},
            {"level_cave_2.xml", new Dictionary<int, int>{
            }},
            {"level_cave_3.xml", new Dictionary<int, int>{
            }},
            {"level_boss_1.xml", new Dictionary<int, int>{
            { 114749, 0 },
            { 114748, 0 },
            { 114747, 0 },
            { 114750, 0 },
            }},
            {"level_passage.xml", new Dictionary<int, int>{
            }},
            {"level_temple_entrance.xml", new Dictionary<int, int>{
            }},
            {"level_temple_1.xml", new Dictionary<int, int>{
            { 129796, 0 },
            { 129799, 0 },
            { 129797, 0 },
            { 126991, 1 },
            { 127015, 1 },
            { 126990, 1 },
            { 129923, 2 },
            { 129929, 2 },
            { 129927, 2 },
            { 129925, 2 },
            { 129924, 2 },
            { 104547, 3 },
            { 104548, 3 },
            { 104546, 3 },
            { 126576, 4 },
            { 130469, 4 },
            { 126577, 4 },
            { 130466, 5 },
            { 130468, 5 },
            { 130471, 5 },
            { 130467, 5 },
            }},
            {"level_boss_2.xml", new Dictionary<int, int>{
            }},
            {"level_boss_2_special.xml", new Dictionary<int, int>{
            }},
            {"level_temple_2.xml", new Dictionary<int, int>{
            { 1387, 0 },
            { 1386, 0 },
            { 1385, 0 },
            { 1384, 0 },
            { 1388, 0 },
            { 1799, 1 },
            { 1798, 1 },
            { 1796, 1 },
            { 1797, 1 },
            { 1800, 1 },
            { 1580, 2 },
            { 1581, 2 },
            { 1578, 2 },
            { 1577, 2 },
            { 1579, 2 },
            }},
            {"level_temple_3.xml", new Dictionary<int, int>{
            }},
            {"level_bonus_5.xml", new Dictionary<int, int>{
            }},
            {"level_boss_3.xml", new Dictionary<int, int>{
            }},
        };
        public static Dictionary<int, string> gateIdToCode = new Dictionary<int, string>()
        {
            {0, "1|0"},
            {1, "1|1"},
            {2, "1|2"},
            {3, "1|3"},
            {4, "2|0"},
            {5, "2|1"},
            {6, "2|2"},
            {7, "2|3"},
            {8, "2|4"},
            {9, "2|5"},
            {10, "2|6"},
            {11, "2|7"},
            {12, "3|0"},
            {13, "3|1"},
            {14, "3|2"},
            {15, "3|3"},
            {16, "3|4"},
            {17, "3|5"},
            {18, "4|0"},
            {19, "4|1"},
            {20, "4|2"},
            {21, "4|3"},
            {22, "4|4"},
            {23, "4|5"},
            {24, "4|6"},
            {25, "4|7"},
            {26, "4|8"},
            {27, "4|9"},
            {28, "4|10"},
            {29, "4|11"},
            {30, "4|12"},
            {31, "4|13"},
            {32, "4|14"},
            {33, "4|15"},
            {34, "5|0"},
            {35, "5|1"},
            {36, "5|2"},
            {37, "5|3"},
            {38, "5|4"},
            {39, "5|5"},
            {40, "5|6"},
            {41, "5|7"},
            {42, "5|8"},
            {43, "5|9"},
            {44, "5|10"},
            {45, "6|0"},
            {46, "6|1"},
            {47, "6|2"},
            {48, "6|3"},
            {49, "6|4"},
            {50, "6|5"},
            {51, "6|6"},
            {52, "7|0"},
            {53, "7|1"},
            {54, "7|2"},
            {55, "7|3"},
            {56, "7|4"},
            {57, "7|5"},
            {58, "7|6"},
            {59, "7|7"},
            {60, "7|8"},
            {61, "7|9"},
            {62, "7|10"},
            {63, "7|11"},
            {64, "8|0"},
            {65, "8|1"},
            {66, "8|2"},
            {67, "8|3"},
            {68, "8|4"},
            {69, "8|5"},
            {70, "8|6"},
            {71, "8|7"},
            {72, "8|8"},
            {73, "9|0"},
            {74, "9|1"},
            {75, "9|2"},
            {76, "9|3"},
            {77, "9|4"},
            {78, "9|5"},
            {79, "9|6"},
            {80, "9|7"},
            {81, "9|8"},
            {82, "9|9"},
            {83, "9|10"},
            {84, "10|0"},
            {85, "10|1"},
            {86, "10|2"},
            {87, "10|3"},
            {88, "10|4"},
            {89, "10|5"},
            {90, "10|6"},
            {91, "10|7"},
            {92, "10|8"},
            {93, "10|9"},
            {94, "10|10"},
            {95, "10|11"},
            {96, "10|12"},
            {97, "10|13"},
            {98, "11|0"},
            {99, "11|1"},
            {100, "11|2"},
            {101, "11|3"},
            {102, "11|4"},
            {103, "11|5"},
            {104, "11|6"},
            {105, "11|7"},
            {106, "11|8"},
            {107, "11|9"},
            {108, "11|10"},
            {109, "11|11"},
            {110, "11|12"},
            {111, "11|13"},
            {112, "11|14"},
            {113, "11|15"},
            {114, "11|16"},
            {115, "11|17"},
            {116, "11|18"},
            {117, "12|0"},
            {118, "12|1"},
            {119, "12|2"},
            {120, "12|3"},
            {121, "12|4"},
            {122, "12|5"},
            {123, "12|6"},
            {124, "12|7"},
            {125, "12|8"},
            {126, "12|9"},
            {127, "12|10"},
            {128, "12|11"},
            {129, "12|12"},
            {130, "12|13"},
            {131, "12|14"},
            {500, "boss_1|0"},
            {501, "t1|0"},
            {502, "t1|1"},
            {503, "t1|2"},
            {504, "t1|3"},
            {505, "t1|4"},
            {506, "t1|5"},
            {507, "t2|0"},
            {508, "t2|1"},
            {509, "t2|2"},
        };
        public static Dictionary<int, string> exitIdToCode = new Dictionary<int, string>()
        {
            {0, "1|0"},
            {1, "1|1"},
            {2, "1|2"},
            {3, "1|3"},
            {4, "1|4"},
            {5, "1|10"},
            {6, "1|20"},
            {7, "2|0"},
            {8, "2|1"},
            {9, "2|2"},
            {10, "2|3"},
            {11, "3|0"},
            {12, "3|1"},
            {13, "3|10"},
            {14, "3|20"},
            {15, "3|81"},
            {16, "3|88"},
            {17, "3|100"},
            {18, "bonus_1|0"},
            {19, "boss_1|0"},
            {20, "boss_1|1"},
            {21, "4|0"},
            {22, "4|1"},
            {23, "4|5"},
            {24, "4|6"},
            {25, "4|100"},
            {26, "5|0"},
            {27, "5|1"},
            {28, "5|2"},
            {29, "5|3"},
            {30, "5|10"},
            {31, "5|88"},
            {32, "6|0"},
            {33, "6|1"},
            {34, "6|2"},
            {35, "bonus_2|0"},
            {36, "boss_2|0"},
            {37, "boss_2|1"},
            {38, "7|0"},
            {39, "7|1"},
            {40, "7|2"},
            {41, "8|0"},
            {42, "8|1"},
            {43, "8|2"},
            {44, "8|200"},
            {45, "9|0"},
            {46, "9|99"},
            {47, "9|100"},
            {48, "9|200"},
            {49, "9|250"},
            {50, "bonus_3|0"},
            {51, "bonus_3|12"},
            {52, "bonus_3|80"},
            {53, "boss_3|0"},
            {54, "boss_3|1"},
            {55, "10|0"},
            {56, "10|75"},
            {57, "10|99"},
            {58, "10|100"},
            {59, "10|169"},
            {60, "11|0"},
            {61, "11|1"},
            {62, "11|45"},
            {63, "11|50"},
            {64, "11|77"},
            {65, "11|88"},
            {66, "11|105"},
            {67, "11|125"},
            {68, "12|0"},
            {69, "12|54"},
            {70, "12|67"},
            {71, "12|156"},
            {72, "bonus_4|0"},
            {73, "boss_4|0"},
            {74, "10b|0"},
            {500, "hub|0"},
            {501, "hub|1"},
            {502, "hub|50"},
            {503, "hub|56"},
            {504, "hub|111"},
            {505, "hub|111*"},
            {506, "hub|128"},
            {507, "library|0"},
            {508, "library|1"},
            {509, "library|3"},
            {510, "library|5"},
            {511, "c1|0"},
            {512, "c1|1"},
            {513, "c1|111"},
            {514, "c1|123"},
            {515, "c1|197"},
            {516, "c2|0"},
            {517, "c2|1"},
            {518, "c3|0"},
            {519, "c3|49"},
            {520, "c3|97"},
            {521, "c3|123"},
            {522, "boss_1|0"},
            {523, "boss_1|2"},
            {524, "passage|0"},
            {525, "passage|2"},
            {526, "passage|1"},
            {527, "passage|10"},
            {528, "passage|20"},
            {529, "passage|30"},
            {530, "passage|40"},
            {531, "passage|50"},
            {532, "passage|11"},
            {533, "passage|21"},
            {534, "passage|31"},
            {535, "passage|41"},
            {536, "passage|51"},
            {537, "passage|110"},
            {538, "passage|120"},
            {539, "passage|130"},
            {540, "passage|128"},
            {541, "boss_2|0"},
            {542, "t1|0"},
            {543, "t1|1"},
            {544, "t2|0"},
            {545, "t2|1"},
            {546, "t2|78"},
            {547, "t2|97"},
            {548, "t2|123"},
            {549, "t3|0"},
            {550, "t3|1"},
            {551, "t3|2"},
            {552, "t3|123"},
            {553, "t3|201"},
            {554, "t3|202"},
            {555, "t3|203"},
            {556, "t_entrance|0"},
            {557, "t_entrance|1"},
            {558, "t_entrance|11"},
            {559, "boss_3|0"},
            {560, "bonus_5|0"},
            {561, "bonus_5|12"},
            {562, "bonus_5|15"},
            {563, "bonus_5|18"},
            {564, "bonus_5|20"},
            {565, "bonus_5|25"},
            {566, "bonus_5|35"},
            {567, "bonus_5|75"},
            {568, "bonus_5|80"},
            {569, "bonus_5|100"},
            {570, "bonus_5|160"},
        };

        public static int GetGateId(string levelFile, int gateId, ArchipelagoData archipelagoData)
        {
            switch (archipelagoData.mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    return castleGateIDLookup[levelFile][gateId];
                case ArchipelagoData.MapType.Temple:
                    return templeGateIDLookup[levelFile][gateId];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static readonly Dictionary<string, string> castleLevelIdToLevelFile = new Dictionary<string, string>()
        {
            {"1", "level_1.xml"},
            {"2", "level_2.xml"},
            {"3", "level_3.xml"},
            {"boss_1", "level_boss_1.xml"},
            {"bonus_1", "level_bonus_1.xml"},
            {"4", "level_4.xml"},
            {"5", "level_5.xml"},
            {"6", "level_6.xml"},
            {"boss_2", "level_boss_2.xml"},
            {"bonus_2", "level_bonus_2.xml"},
            {"7", "level_7.xml"},
            {"8", "level_8.xml"},
            {"9", "level_9.xml"},
            {"boss_3", "level_boss_3.xml"},
            {"bonus_3", "level_bonus_3.xml"},
            {"10", "level_10.xml"},
            {"10b", "level_10_special.xml"},
            {"11", "level_11.xml"},
            {"12", "level_12.xml"},
            {"boss_4", "level_boss_4.xml"},
            {"bonus_4", "level_bonus_4.xml"},
            {"esc_1", "level_esc_1.xml"},
            {"esc_2", "level_esc_2.xml"},
            {"esc_3", "level_esc_3.xml"},
            {"esc_4", "level_esc_4.xml"},
            {"ap_hub", "ap_hub.xml"},
        };
        public static readonly Dictionary<string, int> castleLevelIdToAct = new Dictionary<string, int>()
        {
            {"1", 1},
            {"2", 1},
            {"3", 1},
            {"boss_1", 1},
            {"bonus_1", 1},
            {"4", 2},
            {"5", 2},
            {"6", 2},
            {"boss_2", 2},
            {"bonus_2", 2},
            {"7", 3},
            {"8", 3},
            {"9", 3},
            {"boss_3", 3},
            {"bonus_3", 3},
            {"10", 4},
            {"10b", 1},
            {"11", 4},
            {"12", 4},
            {"boss_4", 4},
            {"bonus_4", 4},
            {"esc_1", 4},
            {"esc_2", 3},
            {"esc_3", 2},
            {"esc_4", 1},
            {"ap_hub", 1},
        };
        public static readonly Dictionary<string, int> castleLevelFileToLevelIndex = new Dictionary<string, int>()
        {
            {"level_1.xml", 0},
            {"level_2.xml", 1},
            {"level_3.xml", 2},
            {"level_bonus_1.xml", 0},
            {"level_4.xml", 3},
            {"level_5.xml", 4},
            {"level_6.xml", 5},
            {"level_bonus_2.xml", 1},
            {"level_7.xml", 6},
            {"level_8.xml", 7},
            {"level_9.xml", 8},
            {"level_bonus_3.xml", 2},
            {"level_10.xml", 9},
            {"level_10_special.xml", 0},
            {"level_11.xml", 10},
            {"level_12.xml", 11},
            {"level_bonus_4.xml", 3},
        };
        public static readonly Dictionary<string, string> templeLevelIdToLevelFile = new Dictionary<string, string>()
        {
            { "hub", "level_hub.xml" },
            { "library", "level_library.xml" },
            { "passage", "level_passage.xml" },
            { "boss_1", "level_boss_1.xml" },
            { "boss_2", "level_boss_2.xml" },
            { "boss_2_special", "level_boss_2_special.xml" },
            { "boss_3", "level_boss_3.xml" },
            { "t_entrance", "level_temple_entrance.xml" },
            { "t1", "level_temple_1.xml" },
            { "t2", "level_temple_2.xml" },
            { "t3", "level_temple_3.xml" },
            { "c1", "level_cave_1.xml" },
            { "c2", "level_cave_2.xml" },
            { "c3", "level_cave_3.xml" },
            { "bonus_5", "level_bonus_5.xml" },
            {"ap_hub", "ap_hub.xml"},
        };
        public static readonly Dictionary<string, int> templeLevelIdToAct = new Dictionary<string, int>()
        {
            { "hub", 3 },
            { "library", 1 },
            { "passage", 2 },
            { "boss_1", 1 },
            { "boss_2", 2 },
            { "boss_2_special", 2 },
            { "boss_3", 3 },
            { "t_entrance", 3 },
            { "t1", 3 },
            { "t2", 3 },
            { "t3", 3},
            { "c1", 1 },
            { "c2", 1 },
            { "c3", 1 },
            { "bonus_5", 4 },
            {"ap_hub", 1},
        };
        public static readonly Dictionary<string, int> templeLevelFileToLevelIndex = new Dictionary<string, int>()
        {
            { "level_boss_1.xml", 2 },
            { "level_temple_1.xml", 0 },
            { "level_temple_2.xml", 1 },
            { "level_bonus_5.xml", 3 },
        };

        public static string GetLevelFileNameFromId(string levelId, ArchipelagoData archipelagoData)
        {
            switch (archipelagoData.mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    return castleLevelIdToLevelFile[levelId];
                case ArchipelagoData.MapType.Temple:
                    return templeLevelIdToLevelFile[levelId];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static string GetLevelIdFromFileName(string levelFileName, ArchipelagoData archipelagoData)
        {
            switch (archipelagoData.mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    foreach (var pair in castleLevelIdToLevelFile)
                    {
                        if (pair.Value != levelFileName) continue;
                        return pair.Key;
                    }
                    return null;
                case ArchipelagoData.MapType.Temple:
                    foreach (var pair in templeLevelIdToLevelFile)
                    {
                        if (pair.Value != levelFileName) continue;
                        return pair.Key;
                    }
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static int GetActFromLevelId(string levelId, ArchipelagoData archipelagoData)
        {
            if (archipelagoData.mapType == ArchipelagoData.MapType.Castle)
            {
                return castleLevelIdToAct[levelId];
            }
            else
            {
                return templeLevelIdToAct[levelId];
            }
        }
        public static int GetActFromLevelFileName(string fileName, ArchipelagoData archipelagoData)
        {
            Dictionary<string, string> dict;
            switch (archipelagoData.mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    dict = castleLevelIdToLevelFile;
                    break;
                case ArchipelagoData.MapType.Temple:
                    dict = templeLevelIdToLevelFile;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            string levelId = "";
            foreach (var keyValue in dict)
            {
                if (keyValue.Value != fileName) continue;
                levelId = keyValue.Key;
                break;
            }
            return GetActFromLevelId(levelId, archipelagoData);
        }
        public static int GetActIndexFromActName(string actName, ArchipelagoData archipelagoData)
        {
            if (archipelagoData.mapType == ArchipelagoData.MapType.Castle)
            {
                switch (actName)
                {
                    case "Prison":
                        return 0;
                    case "Armory":
                        return 1;
                    case "Archives":
                        return 2;
                    case "Chambers":
                        return 3;
                }
            }
            return -1;
        }
        public static int GetFloorIndex(string levelName, ArchipelagoData archipelagoData)
        {
            int levelIndex = -1;
            switch (archipelagoData.mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    if (castleLevelFileToLevelIndex.TryGetValue(levelName, out int cIndex))
                        levelIndex = cIndex;
                    break;
                case ArchipelagoData.MapType.Temple:
                    if (templeLevelFileToLevelIndex.TryGetValue(levelName, out int tIndex))
                        levelIndex = tIndex;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return levelIndex;
        }
        public static string GetLevelPrefix(string levelFile, ArchipelagoData archipelagoData)
        {
            switch (archipelagoData.mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    switch (levelFile)
                    {
                        case "level_1.xml":
                            return "Prison 1";
                        case "level_2.xml":
                            return "Prison 2";
                        case "level_3.xml":
                            return "Prison 3";
                        case "level_4.xml":
                            return "Armory 4";
                        case "level_5.xml":
                            return "Armory 5";
                        case "level_6.xml":
                            return "Armory 6";
                        case "level_7.xml":
                            return "Archives 7";
                        case "level_8.xml":
                            return "Archives 8";
                        case "level_9.xml":
                            return "Archives 9";
                        case "level_10.xml":
                            return "Chambers 10";
                        case "level_10_special.xml":
                            return "Prison Return";
                        case "level_11.xml":
                            return "Chambers 11";
                        case "level_12.xml":
                            return "Chambers 12";
                        //case "level_bonus_1.xml":
                        //    return "N1";
                        //case "level_bonus_2.xml":
                        //    return "N2";
                        //case "level_bonus_3.xml":
                        //    return "N3";
                        //case "level_bonus_4.xml":
                        //    return "N4";
                        case "level_boss_1.xml":
                            return "Boss 1";
                        case "level_boss_2.xml":
                            return "Boss 2";
                        case "level_boss_3.xml":
                            return "Boss 3";
                        case "level_boss_4.xml":
                            return "Boss 4";
                            //case "level_esc_1.xml":
                            //    return "E1";
                            //case "level_esc_2.xml":
                            //    return "E2";
                            //case "level_esc_3.xml":
                            //    return "E3";
                            //case "level_esc_4.xml":
                            //    return "E4";
                    }
                    break;
                case ArchipelagoData.MapType.Temple:
                    switch (levelFile)
                    {
                        case "level_hub.xml":
                            return "";
                        case "level_cave_1.xml":
                            return "Cave 3";
                        case "level_cave_2.xml":
                            return "Cave 2";
                        case "level_cave_3.xml":
                            return "Cave 1";
                        case "level_boss_1.xml":
                            return "Temple Boss 1";
                        case "level_passage.xml":
                            return "Passage";
                        case "level_temple_1.xml":
                            return "Temple 1";
                        case "level_temple_2.xml":
                            return "Temple 2";
                        case "level_temple_3.xml":
                            return "Temple 3";
                        case "level_bonus_5.xml":
                            return "PoF";
                    }
                    break;
            }
            return null;
        }

        public static int GetActCount(ArchipelagoData archipelagoData)
        {
            switch (archipelagoData.mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    return 4;
                case ArchipelagoData.MapType.Temple:
                    return 3;
                default:
                    return 1;
            }
        }

        public static Difficulty GetDifficulty(ArchipelagoData archipelagoData)
        {
            int difficultyIndex = archipelagoData.GetOption("difficulty");
            return (Difficulty)difficultyIndex;
        }

        //Don't need to put towers in here as they can't move
        public static readonly Dictionary<string, Dictionary<int, List<int>>> castleMinibossIdToLocations = new Dictionary<string, Dictionary<int, List<int>>>
        {
            {"level_2.xml", new Dictionary<int, List<int>>{
                { 3346, new List<int>{ 1201, 1202 } },
            }},
            {"level_3.xml", new Dictionary<int, List<int>>{
                { 3754, new List<int>{ 1205, 1206 } },
            }},
            {"level_4.xml", new Dictionary<int, List<int>>{
                { 3898, new List<int>{ 1215, 1216 } },
            }},
            {"level_5.xml", new Dictionary<int, List<int>>{
                { 3410, new List<int>{ 1221, 1222 } },
            }},
            {"level_6.xml", new Dictionary<int, List<int>>{
                { 5007, new List<int>{ 1229, 1230 } },
            }},
            {"level_8.xml", new Dictionary<int, List<int>>{
                { 2760, new List<int>{ 1244, 1245 } },
                { 4005, new List<int>{ 1246, 1247 } },
            }},
            {"level_9.xml", new Dictionary<int, List<int>>{
                { 796, new List<int>{ 1253, 1254 } },
                { 8112, new List<int>{ 1255, 1256 } },
                { 10678, new List<int>{ 1257, 1258 } },
            }},
            {"level_10.xml", new Dictionary<int, List<int>>{
                { 2934, new List<int>{ 1269, 1270 } },
            }},
            {"level_11.xml", new Dictionary<int, List<int>>{
                { 1437, new List<int>{ 1276, 1277 } },
                { 4888, new List<int>{ 1278, 1279 } },
                { 7882, new List<int>{ 1280, 1281 } },
            }},
            {"level_12.xml", new Dictionary<int, List<int>>{
                { 5581, new List<int>{ 1301, 1302 } },
                { 3269, new List<int>{ 1303, 1304 } },
            }},
            {"level_boss_2.xml", new Dictionary<int, List<int>>{
                { 312, new List<int>{ 1321 } },
            }},
            {"level_boss_3.xml", new Dictionary<int, List<int>>{
                { 281, new List<int>{ 1322 } },
            }},
            {"level_boss_4.xml", new Dictionary<int, List<int>>{
                { 99999, new List<int>{ 1323, 1324 } },
            }},
        };
        public static readonly Dictionary<string, Dictionary<int, List<int>>> templeMinibossIdToLocations = new Dictionary<string, Dictionary<int, List<int>>>
        {
            {"level_cave_1.xml", new Dictionary<int, List<int>>{
                { 104090, new List<int>{ 515, 516 } },
            }},
            {"level_cave_2.xml", new Dictionary<int, List<int>>{
                { 111561, new List<int>{ 526, 527 } },
                { 113226, new List<int>{ 528, 529 } },
                { 108181, new List<int>{ 530, 531 } },
            }},
            {"level_cave_3.xml", new Dictionary<int, List<int>>{
                { 116348, new List<int>{ 558, 559 } },
                { 122837, new List<int>{ 560, 561 } },
                { 115550, new List<int>{ 562, 563 } },
            }},
            //{"level_boss_1.xml", new Dictionary<int, List<int>>{
            //  { 114725, new List<int>{ 582, 583 } }, //Also includes through 589 with 4 total dune sharks
            //  { 114746, new List<int>{ 590 } }, //Technicallyyyy has two items, but we forgot about that when assigning the ids. Nobody should notice though...
            //}},
            {"level_temple_1.xml", new Dictionary<int, List<int>>{
                { 130639, new List<int>{ 597, 598 } },
            }},
            {"level_temple_2.xml", new Dictionary<int, List<int>>{
                { 143227, new List<int>{ 601, 602 } },
                { 139384, new List<int>{ 603, 604 } },
            }},
        };

        public static Dictionary<string, int> castleButtonProgressCounts = new Dictionary<string, int>()
        {
            //{ 0, 4 }, //Activate Prison Boss Rune
            //{ 2, 4 }, //Activate Armory Boss Rune
            //{ 4, 4 }, //Activate Archives Boss Rune
            //{ 6, 4 }, //Activate Chambers Boss Rune
            { "Enable PrF2 Spike Puzzle East Buttons", 3 }, //Enable PrF2 Spike Puzzle East Buttons //16
            { "Enable PrF2 Spike Puzzle South Buttons", 3 }, //Enable PrF2 Spike Puzzle South Buttons //17
            { "Enable PrF2 Spike Puzzle North Buttons", 3 }, //Enable PrF2 Spike Puzzle North Buttons //18
            { "Teleport PrF2 West Rune Puzzle Item", 4 }, //Teleport PrF2 West Rune Puzzle Item //26
            { "Activate PrF2 SE Rune Puzzle Reward", 4 }, //Activate PrF2 SE Rune Puzzle Reward //28
            { "Open PrF3 Bonus Portal", 5 }, //Open PrF3 Bonus Portal //39
            { "Open PrF3 Bonus Room", 9 }, //Open PrF3 Bonus Room //41
            { "Open AmF4 SE Cache", 4 }, //Open AmF4 SE Cache //63
            { "Teleport AmF5 NE Gates Item", 4 }, //Teleport AmF5 NE Gates Item //78
            { "Open AmF6 Spike Turret Reward Rooms", 5 }, //Open AmF6 Spike Turret Reward Rooms //88
            { "Open AmF6 Spike Turret 2nd Reward Rooms", 2 }, //Open AmF6 Spike Turret 2nd Reward Rooms //92
            { "Open ArF8 Puzzle Room", 4 }, //Open ArF8 Puzzle Room //115
            { "Activate ArF9 Bonus Return Light Bridge", 4 }, //Activate ArF9 Bonus Return Light Bridge //134
            { "Open ArF9 Bonus Portal Passage", 6 }, //Open ArF9 Bonus Portal Passage //136
            { "Open ArF9 Simon Says Room", 5 }, //Open ArF9 Simon Says Room //142
            { "Activate PrF1 SW Light Bridge", 4 }, //Activate PrF1 SW Light Bridge //152
            { "Open ChF11 Bonus Portal", 8 }, //Open ChF11 Bonus Portal //157
            { "Teleport ChF11 West Spike Items", 4 }, //Teleport ChF11 West Spike Items //159
            { "Open ChF11 Bonus Room", 5 }, //Open ChF11 Bonus Room //161
            { "Open ChF12 Hidden Hall", 6 }, //Open ChF12 Hidden Hall //171
        };
        public static Dictionary<string, int> templeButtonProgressCounts = new Dictionary<string, int>()
        {
            { "Activate TF2 Light Bridges", 5 }, //Activate TF2 Light Bridges //33
            { "Open TF2 Portal Room Shortcut", 2 }, //Open TF2 Portal Room Shortcut //43
            { "Open TF3 Puzzle Room", 4 }, //Open TF3 Puzzle Room //51
        };

        public static readonly Dictionary<string, Dictionary<Vector2, long>> castlePosToLocationId = new Dictionary<string, Dictionary<Vector2, long>>
        {
            {"level_1.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(23.0f, -48.0f), 0 },
            { new Vector2(10.0f, -2.5f), 1 },
            //{ new Vector2(-74.375f, -36.5f), 2 },
            { new Vector2(-28.5f, -42.25f), 3 },
            { new Vector2(-29.5f, -42.25f), 4 },
            { new Vector2(51.75f, -35.75f), 5 },
            { new Vector2(52.75f, -35.75f), 6 },
            { new Vector2(-27.375f, -23.875f), 7 },
            { new Vector2(12.0f, -16.5f), 8 },
            { new Vector2(21.5f, -22.0f), 9 },
            { new Vector2(20.5f, -23.5f), 10 },
            { new Vector2(22.5f, -23.5f), 11 },
            { new Vector2(-18.25f, 27.0f), 12 },
            { new Vector2(-18.25f, 28.0f), 13 },
            { new Vector2(4.75f, 23.75f), 14 },
            { new Vector2(5.75f, 23.75f), 15 },
            { new Vector2(6.75f, 23.75f), 16 },
            { new Vector2(13.875f, 10.125f), 17 },
            { new Vector2(25.5f, 15.5f), 18 },
            { new Vector2(-42.0f, -40.5f), 19 },
            { new Vector2(-35.5f, -33.75f), 20 },
            { new Vector2(51.125f, -37.0f), 21 },
            { new Vector2(25.5f, -13.5f), 22 },
            { new Vector2(0.0f, -1.375f), 23 },
            { new Vector2(-27.5f, 17.75f), 24 },
            { new Vector2(5.25f, 10.75f), 25 },
            { new Vector2(-3.625f, 24.125f), 26 },
            { new Vector2(21.375f, 27.625f), 27 },
            { new Vector2(33.0f, -29.125f), 28 },
            { new Vector2(51.0f, -20.25f), 29 },
            { new Vector2(35.0f, 1.25f), 30 },
            { new Vector2(32.5f, 14.0f), 31 },
            { new Vector2(-2.0f, 37.875f), 32 },
            { new Vector2(-31.5f, 3.375f), 33 },
            { new Vector2(9.0f, -6.125f), 34 },
            { new Vector2(-18.5f, -44.75f), 35 },
            { new Vector2(-17.75f, -44.75f), 36 },
            { new Vector2(-18.5f, -44.0f), 37 },
            { new Vector2(-17.75f, -44.0f), 38 },
            { new Vector2(-27.25f, -12.25f), 39 },
            { new Vector2(28.5f, -20.0f), 40 },
            { new Vector2(28.0f, -19.25f), 41 },
            { new Vector2(27.25f, -19.25f), 42 },
            { new Vector2(20.5f, -29.5f), 43 },
            { new Vector2(22.5f, -29.5f), 44 },
            { new Vector2(21.5f, -28.0f), 45 },
            { new Vector2(-14.625f, -8.625f), 46 },
            { new Vector2(-15.375f, -9.375f), 47 },
            { new Vector2(-15.25f, -8.0f), 48 },
            { new Vector2(-19.25f, 0.875f), 49 },
            { new Vector2(-19.25f, 1.875f), 50 },
            { new Vector2(-19.25f, 2.875f), 51 },
            { new Vector2(-19.25f, 3.875f), 52 },
            { new Vector2(-0.5f, 5.5f), 53 },
            { new Vector2(-2.5f, 5.5f), 54 },
            { new Vector2(-1.5f, 6.75f), 55 },
            { new Vector2(15.5f, 21.75f), 56 },
            { new Vector2(15.5f, 20.5f), 57 },
            { new Vector2(15.5f, 19.25f), 58 },
            { new Vector2(15.5f, 23.0f), 59 },
            { new Vector2(14.5f, 24.0f), 60 },
            { new Vector2(13.25f, 24.0f), 61 },
            { new Vector2(15.5f, 24.0f), 62 },
            { new Vector2(6.5f, -19.75f), 63 },
            { new Vector2(-20.5f, -52.0f), 64 },
            { new Vector2(41.5f, 2.125f), 65 },
            { new Vector2(19.0f, -39.5f), 66 },
            { new Vector2(-30.0f, 12.0f), 67 },
            { new Vector2(41.0f, -24.0f), 68 },
            { new Vector2(35.75f, 28.0f), 69 },
            { new Vector2(-22.0f, 32.0f), 70 },
            { new Vector2(-30.0f, 10.25f), 71 },

            { new Vector2(33f, -49f), 1325 },
            { new Vector2(34.5f, -50.5f), 1326 },
            { new Vector2(31.5f, -50.5f), 1327 },
            { new Vector2(31.5f, -47.5f), 1328 },
            { new Vector2(34.5f, -47.5f), 1329 },
            { new Vector2(-19.5f, -24.5f), 1331 },
            }},
            {"level_2.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-7.0f, -76.75f), 72 },
            //{ new Vector2(-29.0f, -23.75f), 73 },
            { new Vector2(-29.0f, 19.5f), 74 },
            { new Vector2(-34.0f, -23.5f), 75 },
            { new Vector2(9.0f, -74.5f), 76 },
            { new Vector2(10.0f, -74.5f), 77 },
            { new Vector2(27.0f, -68.5f), 78 },
            { new Vector2(26.0f, -68.5f), 79 },
            { new Vector2(18.0f, -32.5f), 80 },
            { new Vector2(17.0f, -32.5f), 81 },
            { new Vector2(56.5f, -82.0f), 82 },
            { new Vector2(56.5f, -81.0f), 83 },
            { new Vector2(56.5f, -80.0f), 84 },
            { new Vector2(51.5f, -43.0f), 85 },
            { new Vector2(-32.5f, 11.5f), 86 },
            { new Vector2(-31.5f, 11.5f), 87 },
            { new Vector2(-15.5f, -21.25f), 88 },
            { new Vector2(-16.5f, -21.25f), 89 },
            { new Vector2(52.5f, -21.75f), 90 },
            { new Vector2(70.5f, -23.5f), 91 },
            { new Vector2(70.5f, -24.5f), 92 },
            { new Vector2(-22.5f, 39.375f), 93 },
            { new Vector2(-21.5f, 39.375f), 94 },
            { new Vector2(28.5f, -52.375f), 95 },
            { new Vector2(56.25f, -55.625f), 96 },
            { new Vector2(51.0f, -31.75f), 97 },
            { new Vector2(52.625f, -42.875f), 98 },
            { new Vector2(20.75f, 8.25f), 99 },
            { new Vector2(36.0f, -16.25f), 100 },
            { new Vector2(47.0f, 8.0f), 101 },
            { new Vector2(38.25f, -9.0f), 102 },
            { new Vector2(55.0f, 7.5f), 103 },
            { new Vector2(52.75f, -2.75f), 104 },
            { new Vector2(66.25f, 3.0f), 105 },
            { new Vector2(35.5f, 28.25f), 106 },
            { new Vector2(58.75f, 25.75f), 107 },
            { new Vector2(26.0f, 30.5f), 108 },
            { new Vector2(67.0f, -92.5f), 109 },
            { new Vector2(63.0f, -79.5f), 110 },
            { new Vector2(50.75f, -71.0f), 111 },
            { new Vector2(76.0f, -56.25f), 112 },
            { new Vector2(-48.0f, -29.5f), 113 },
            { new Vector2(-50.0f, -18.5f), 114 },
            { new Vector2(-50.0f, -2.5f), 115 },
            { new Vector2(15.0f, -92.5f), 116 },
            { new Vector2(16.0f, -92.5f), 117 },
            { new Vector2(17.0f, -92.5f), 118 },
            { new Vector2(13.0f, -64.5f), 119 },
            { new Vector2(14.0f, -64.5f), 120 },
            { new Vector2(70.5f, -72.5f), 121 },
            { new Vector2(71.5f, -72.5f), 122 },
            { new Vector2(-47.5f, 24.5f), 123 },
            { new Vector2(-47.5f, 23.5f), 124 },
            { new Vector2(-47.5f, 22.5f), 125 },
            { new Vector2(-23.5f, -7.5f), 126 },
            { new Vector2(-23.5f, -8.5f), 127 },
            { new Vector2(-23.5f, -9.5f), 128 },
            { new Vector2(57.5f, -26.5f), 129 },
            { new Vector2(57.5f, -27.5f), 130 },
            { new Vector2(57.5f, -28.5f), 131 },
            { new Vector2(30.0f, 26.5f), 132 },
            { new Vector2(31.0f, 26.5f), 133 },
            { new Vector2(32.0f, 26.5f), 134 },
            { new Vector2(25.25f, -80.625f), 135 },
            { new Vector2(-60.75f, -10.25f), 136 },
            { new Vector2(45.0f, 28.5f), 137 },
            { new Vector2(-2.0f, -15.0f), 138 },
            { new Vector2(-8.25f, -53.75f), 139 },
            { new Vector2(12.0f, -57.25f), 140 },
            { new Vector2(7.0f, -49.75f), 141 },
            { new Vector2(-38.0f, 1.5f), 142 },
            { new Vector2(12.0f, 3.75f), 143 },
            { new Vector2(74.0f, -28.25f), 144 },
            { new Vector2(-22.0f, 37.75f), 145 },
            { new Vector2(2.5f, -114.5f), 146 },
            { new Vector2(45.5f, -1.5f), 147 },
            { new Vector2(2.375f, -116.5f), 148 },
            { new Vector2(-6.75f, -63.25f), 149 },
            { new Vector2(-38.0f, -0.25f), 150 },
            { new Vector2(12.5f, 5.0f), 151 },
            { new Vector2(11.5f, 5.0f), 152 },
            { new Vector2(43.5f, -1.5f), 153 },
            { new Vector2(68.5f, -101.5f), 154 },
            { new Vector2(64.0f, -106.0f), 155 },
            { new Vector2(73.0f, -106.0f), 156 },
            { new Vector2(68.5f, -110.5f), 157 },
            { new Vector2(-48.125f, -22.625f), 1201 },
            { new Vector2(-47.625f, -22.125f), 1202 },
            { new Vector2(-42.0f, 20.0f), 1203 },
            { new Vector2(-42.0f, 21.0f), 1203 },
            { new Vector2(65.0f, -26.0f), 1204 },
            { new Vector2(65.0f, -25.0f), 1204 },

            { new Vector2(-6f, -15f), 1337 },
            { new Vector2(-4.5f, -16.5f), 1338 },
            { new Vector2(-4.5f, -13.5f), 1339 },
            { new Vector2(-7.5f, -16.5f), 1340 },
            { new Vector2(-7.5f, -13.5f), 1341 },
            { new Vector2(6f, -79.5f), 1342 },
            { new Vector2(2f, -83f), 1343 },
            { new Vector2(0f, -83f), 1344 },
            { new Vector2(6f, -77.5f), 1345 },
            { new Vector2(-2f, -83f), 1346 },
            { new Vector2(6f, -75.5f), 1347 },
            { new Vector2(2f, -71f), 1348 },
            { new Vector2(0f, -71f), 1349 },
            { new Vector2(-2f, -71f), 1350 },
            { new Vector2(21f, -69.5f), 1351 },
            { new Vector2(-30.5f, -4f), 1352 },
            { new Vector2(-44.5f, 1f), 1353 },
            { new Vector2(57.5f, -21.5f), 1354 },
            { new Vector2(68.5f, -106f), 1355 },
            { new Vector2(-40.5f, -36.5f), 1356 },
            { new Vector2(-39.5f, -37.5f), 1357 },
            //{ new Vector2(29f, -7f), 1358 },
            { new Vector2(28f, -8f), 1359 },
            { new Vector2(-41.5f, -37.5f), 1360 },
            { new Vector2(63f, -8f), 1361 },
            { new Vector2(-39.5f, -35.5f), 1362 },
            { new Vector2(55f, 16.5f), 1363 },
            { new Vector2(-41.5f, -35.5f), 1364 },
            { new Vector2(33.5f, 12.5f), 1365 },
            { new Vector2(12.5f, -22.5f), 1366 },
            { new Vector2(2.75f, -4.5f), 1367 },
            { new Vector2(2.5f, 11.5f), 1368 },
            }},
            {"level_3.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(52.0f, -33.5f), 158 },
            { new Vector2(-33.5f, 8.0f), 159 },
            { new Vector2(10.0f, -23.25f), 160 },
            { new Vector2(22.375f, 32.0f), 161 },
            { new Vector2(-59.0f, -44.5f), 162 },
            { new Vector2(-46.0f, -46.75f), 163 },
            { new Vector2(-51.5f, -11.5f), 164 },
            { new Vector2(-50.5f, -11.5f), 165 },
            { new Vector2(-51.0f, -10.75f), 166 },
            { new Vector2(2.5f, -13.5f), 167 },
            { new Vector2(3.5f, -13.5f), 168 },
            { new Vector2(63.0f, -0.5f), 169 },
            { new Vector2(62.0f, -0.5f), 170 },
            { new Vector2(-27.75f, 30.125f), 171 },
            { new Vector2(-26.75f, 30.125f), 172 },
            { new Vector2(24.5f, 30.75f), 173 },
            { new Vector2(24.5f, 31.5f), 174 },
            { new Vector2(-22.125f, 76.375f), 175 },
            { new Vector2(61.5f, 40.5f), 176 },
            { new Vector2(62.5f, 40.5f), 177 },
            { new Vector2(-38.0f, -37.5f), 178 },
            { new Vector2(-29.0f, -51.75f), 179 },
            { new Vector2(3.0f, -37.25f), 180 },
            { new Vector2(-9.0f, -47.0f), 181 },
            { new Vector2(-35.0f, -13.0f), 182 },
            { new Vector2(-24.0f, -26.5f), 183 },
            { new Vector2(-11.5f, -11.75f), 184 },
            { new Vector2(3.0f, -12.25f), 185 },
            { new Vector2(-26.5f, 2.5f), 186 },
            { new Vector2(18.0f, 64.25f), 187 },
            { new Vector2(18.0f, 54.75f), 188 },
            { new Vector2(33.0f, 43.75f), 189 },
            { new Vector2(60.75f, 56.5f), 190 },
            { new Vector2(-15.625f, -1.5f), 191 },
            { new Vector2(-59.0f, 19.0f), 192 },
            { new Vector2(-69.5f, 27.25f), 193 },
            { new Vector2(-45.0f, 14.0f), 194 },
            { new Vector2(-47.0f, 28.0f), 195 },
            { new Vector2(-66.5f, 45.0f), 196 },
            { new Vector2(-41.25f, -11.0f), 197 },
            { new Vector2(-46.0f, 14.25f), 198 },
            { new Vector2(-65.0f, -40.5f), 199 },
            { new Vector2(-64.0f, -40.5f), 200 },
            { new Vector2(-27.5f, -38.5f), 201 },
            { new Vector2(-27.5f, -37.5f), 202 },
            { new Vector2(-27.5f, -36.5f), 203 },
            { new Vector2(58.5f, -31.5f), 204 },
            { new Vector2(59.5f, -31.5f), 205 },
            { new Vector2(57.5f, -31.5f), 206 },
            { new Vector2(-40.5f, -23.25f), 207 },
            { new Vector2(-40.5f, -22.5f), 208 },
            { new Vector2(-41.0f, -22.0f), 209 },
            { new Vector2(-10.0f, -12.25f), 210 },
            { new Vector2(-10.0f, -11.5f), 211 },
            { new Vector2(-15.5f, 55.5f), 212 },
            { new Vector2(-15.5f, 54.5f), 213 },
            { new Vector2(-15.5f, 53.5f), 214 },
            { new Vector2(57.5f, 47.0f), 215 },
            { new Vector2(57.5f, 48.0f), 216 },
            { new Vector2(57.5f, 49.0f), 217 },
            { new Vector2(57.5f, 50.0f), 218 },
            { new Vector2(-46.25f, -23.25f), 219 },
            { new Vector2(27.0f, 50.0f), 220 },
            { new Vector2(0.0f, 70.0f), 221 },
            { new Vector2(-28.75f, 70.375f), 222 },
            { new Vector2(35.0f, 11.25f), 223 },
            { new Vector2(-52.0f, -37.75f), 224 },
            { new Vector2(-62.0f, -39.75f), 225 },
            { new Vector2(-65.75f, -19.5f), 226 },
            { new Vector2(-60.0f, -10.5f), 227 },
            { new Vector2(-45.0f, -27.5f), 228 },
            { new Vector2(-17.875f, 3.0f), 229 },
            { new Vector2(-18.75f, 1.375f), 230 },
            { new Vector2(-18.0f, -1.5f), 231 },
            { new Vector2(-19.75f, 1.0f), 232 },
            { new Vector2(-16.25f, 0.75f), 233 },
            { new Vector2(-44.0f, 14.25f), 234 },
            { new Vector2(-26.25f, 70.5f), 235 },
            { new Vector2(71.0f, -31.0f), 236 },
            { new Vector2(-41.5f, -29.0f), 237 },
            { new Vector2(-50.5f, 11.25f), 238 },
            { new Vector2(-28.125f, 19.625f), 239 },
            { new Vector2(36.75f, 22.25f), 240 },
            { new Vector2(-52.375f, -23.5f), 241 },
            { new Vector2(-52.25f, -10.25f), 242 },
            { new Vector2(-51.75f, 11.25f), 243 },
            { new Vector2(-37.5f, 12.75f), 244 },
            { new Vector2(22.5f, 24.75f), 245 },
            { new Vector2(23.5f, 24.75f), 246 },
            { new Vector2(-22.125f, 13.375f), 1205 },
            { new Vector2(-21.625f, 13.875f), 1206 },
            { new Vector2(-47.5f, -30.5f), 1207 },
            { new Vector2(-47.5f, -29.5f), 1207 },
            { new Vector2(23.5f, -47.0f), 1208 },
            { new Vector2(23.5f, -46.0f), 1208 },
            { new Vector2(-53.5f, 16.5f), 1209 },
            { new Vector2(-53.5f, 17.5f), 1209 },
            { new Vector2(3.0f, -23.0f), 1210 },
            { new Vector2(3.0f, -22.0f), 1210 },
            { new Vector2(-26.5f, 18.0f), 1211 },
            { new Vector2(-26.5f, 19.0f), 1211 },
            { new Vector2(72.5f, -25.5f), 1212 },
            { new Vector2(72.5f, -24.5f), 1212 },
            { new Vector2(35.0f, -0.5f), 1213 },
            { new Vector2(35.0f, 0.5f), 1213 },
            { new Vector2(-58.0f, 56.0f), 1214 },
            { new Vector2(-58.0f, 57.0f), 1214 },

            { new Vector2(6f, 59f), 1369 },
            { new Vector2(-3.5f, 52f), 1371 },
            { new Vector2(-5f, 53.5f), 1376 },
            { new Vector2(-2f, 53.5f), 1377 },
            { new Vector2(-5f, 50.5f), 1378 },
            { new Vector2(-2f, 50.5f), 1379 },
            { new Vector2(-3f, -46.5f), 1380 },
            { new Vector2(10f, -33.5f), 1381 },
            { new Vector2(37f, -50f), 1382 },
            { new Vector2(-69f, 55f), 1383 },
            { new Vector2(23f, 38f), 1384 },
            { new Vector2(39.5f, -33.5f), 1385 },
            { new Vector2(39.5f, -31.5f), 1386 },
            { new Vector2(41.5f, -33.5f), 1387 },
            { new Vector2(43.5f, -31.5f), 1388 },
            { new Vector2(43.5f, -33.5f), 1389 },
            { new Vector2(41.5f, -31.5f), 1390 },
            { new Vector2(39.5f, -29.5f), 1391 },
            { new Vector2(41.5f, -29.5f), 1392 },
            { new Vector2(43.5f, -29.5f), 1393 },
            { new Vector2(24.5f, -50.5f), 1394 },
            { new Vector2(-18f, -38.5f), 1395 },
            { new Vector2(-15f, -48.5f), 1396 },
            { new Vector2(36.5f, -32.5f), 1397 },
            { new Vector2(63f, -43.5f), 1398 },
            { new Vector2(-43.5f, 13.5f), 1399 },
            { new Vector2(3f, -24.5f), 1400 },
            { new Vector2(10f, -24.5f), 1401 },
            { new Vector2(37.5f, -9.5f), 1402 },
            { new Vector2(67f, -5.5f), 1403 },
            { new Vector2(27.5f, 42.5f), 1404 },
            { new Vector2(4.5f, 68.5f), 1405 },
            { new Vector2(11f, 58.5f), 1406 },
            { new Vector2(-35.5f, -52.5f), 1635 },
            { new Vector2(41.5f, -34.75f), 1636 },
            }},
            {"level_bonus_1.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-13.0f, 25.5f), 247 },
            { new Vector2(-12.0f, 25.5f), 248 },
            { new Vector2(-11.0f, 25.5f), 249 },
            { new Vector2(-17.5f, 36.0f), 250 },
            { new Vector2(-17.5f, 35.0f), 251 },
            { new Vector2(-12.0f, 4.5f), 252 },
            { new Vector2(-11.0f, 4.5f), 253 },
            { new Vector2(-11.0f, 3.5f), 254 },
            { new Vector2(-12.0f, 3.5f), 255 },
            { new Vector2(-3.0f, 4.5f), 256 },
            { new Vector2(-3.0f, 3.5f), 257 },
            { new Vector2(-4.0f, 4.5f), 258 },
            { new Vector2(-4.0f, 3.5f), 259 },
            { new Vector2(-5.0f, 4.5f), 260 },
            { new Vector2(-5.0f, 3.5f), 261 },
            { new Vector2(-10.0f, 4.5f), 262 },
            { new Vector2(-10.0f, 3.5f), 263 },
            { new Vector2(-13.5f, 27.75f), 264 },
            { new Vector2(-13.5f, 28.75f), 265 },
            { new Vector2(-13.5f, 29.75f), 266 },
            { new Vector2(-12.5f, 29.75f), 267 },
            { new Vector2(-12.5f, 28.75f), 268 },
            { new Vector2(-12.5f, 27.75f), 269 },
            { new Vector2(-11.5f, 27.75f), 270 },
            { new Vector2(-10.5f, 27.75f), 271 },
            { new Vector2(-10.5f, 28.75f), 272 },
            { new Vector2(-11.5f, 28.75f), 273 },
            { new Vector2(-9.5f, 27.75f), 274 },
            { new Vector2(-9.5f, 28.75f), 275 },
            { new Vector2(-8.5f, 27.75f), 276 },
            { new Vector2(-7.5f, 27.75f), 277 },
            { new Vector2(-7.5f, 28.75f), 278 },
            { new Vector2(-8.5f, 28.75f), 279 },
            { new Vector2(9.0f, 22.5f), 280 },
            { new Vector2(11.0f, 22.5f), 281 },
            { new Vector2(10.0f, 22.5f), 282 },
            { new Vector2(-13.5f, 30.75f), 283 },
            { new Vector2(-13.5f, 31.75f), 284 },
            { new Vector2(-12.5f, 31.75f), 285 },
            { new Vector2(-12.5f, 30.75f), 286 },
            { new Vector2(-12.5f, 49.5f), 287 },
            { new Vector2(-12.5f, 50.5f), 288 },
            { new Vector2(-11.5f, 50.5f), 289 },
            { new Vector2(-17.0f, 5.0f), 290 },
            { new Vector2(7.625f, 22.25f), 291 },
            { new Vector2(-4.0f, 14.75f), 292 },
            { new Vector2(-29.0f, 35.0f), 293 },
            { new Vector2(-7.75f, 39.625f), 294 },
            { new Vector2(9.0f, 41.0f), 295 },
            { new Vector2(2.125f, 3.125f), 296 },

            { new Vector2(-6.5f, 16f), 1628 },
            { new Vector2(14f, 12.5f), 1629 },
            { new Vector2(17f, 12.5f), 1630 },
            { new Vector2(5.5f, 33.5f), 1631 },
            }},
            {"level_boss_1.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-3.0f, -38.0f), 297 },
            { new Vector2(-6.0f, -18.0f), 298 },
            { new Vector2(6.0f, -14.0f), 299 },
            { new Vector2(-3.0f, 5.5f), 300 },

            { new Vector2(-7.5f, -9.5f), 1407 },
            { new Vector2(1.5f, -9.5f), 1408 },
            }},
            {"level_4.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(20.5f, -20.0f), 301 },
            { new Vector2(42.75f, 54.0f), 302 },
            { new Vector2(-90.5f, -39.5f), 303 },
            { new Vector2(-90.5f, -38.5f), 304 },
            { new Vector2(-78.5f, -70.5f), 305 },
            { new Vector2(-48.75f, -54.5f), 306 },
            { new Vector2(-14.5f, -51.25f), 307 },
            { new Vector2(37.5f, -59.5f), 308 },
            { new Vector2(37.5f, -58.75f), 309 },
            { new Vector2(-58.0f, -2.0f), 310 },
            { new Vector2(-57.0f, -2.0f), 311 },
            { new Vector2(-59.0f, -2.0f), 312 },
            { new Vector2(-11.0f, -29.75f), 313 },
            { new Vector2(-13.0f, -25.875f), 314 },
            { new Vector2(27.0f, -20.625f), 315 },
            { new Vector2(26.0f, 15.5f), 316 },
            { new Vector2(27.5f, 15.5f), 317 },
            { new Vector2(29.0f, 15.5f), 318 },
            { new Vector2(31.0f, -20.625f), 319 },
            { new Vector2(-53.75f, -35.5f), 320 },
            { new Vector2(-51.25f, -37.25f), 321 },
            { new Vector2(10.0f, -54.0f), 322 },
            { new Vector2(2.75f, -21.375f), 323 },
            { new Vector2(2.5f, 0.0f), 324 },
            { new Vector2(46.25f, -13.0f), 325 },
            { new Vector2(68.5f, -16.5f), 326 },
            { new Vector2(53.0f, -27.0f), 327 },
            { new Vector2(60.5f, -0.375f), 328 },
            { new Vector2(49.375f, 18.375f), 329 },
            { new Vector2(58.375f, 26.125f), 330 },
            { new Vector2(4.0f, 45.0f), 331 },
            { new Vector2(10.0f, -50.0f), 332 },
            { new Vector2(-77.0f, -74.0f), 333 },
            { new Vector2(-16.0f, -34.0f), 334 },
            { new Vector2(-59.5f, 22.5f), 335 },
            { new Vector2(-56.5f, 22.5f), 336 },
            { new Vector2(29.0f, -20.375f), 337 },
            { new Vector2(44.625f, 53.5f), 338 },
            { new Vector2(43.0f, 52.375f), 339 },
            { new Vector2(-12.75f, -31.5f), 340 },
            { new Vector2(-11.0f, -28.5f), 341 },
            { new Vector2(-82.5f, -57.5f), 342 },
            { new Vector2(-82.5f, -53.5f), 343 },
            { new Vector2(-82.5f, -61.5f), 344 },
            { new Vector2(-36.75f, -45.25f), 345 },
            { new Vector2(-36.25f, -45.25f), 346 },
            { new Vector2(-35.75f, -45.25f), 347 },
            { new Vector2(33.5f, -36.25f), 348 },
            { new Vector2(34.5f, -36.25f), 349 },
            { new Vector2(-83.5f, 7.5f), 350 },
            { new Vector2(-83.5f, 6.5f), 351 },
            { new Vector2(15.5f, -1.5f), 352 },
            { new Vector2(15.5f, -2.5f), 353 },
            { new Vector2(15.5f, -3.5f), 354 },
            { new Vector2(-49.875f, -54.375f), 355 },
            { new Vector2(-11.5f, -30.5f), 356 },
            { new Vector2(46.5f, -6.5f), 357 },
            { new Vector2(-20.25f, -25.75f), 358 },
            { new Vector2(38.0f, -54.5f), 359 },
            { new Vector2(35.5f, 53.625f), 360 },
            { new Vector2(-14.25f, -31.5f), 361 },
            { new Vector2(-58.0f, 22.25f), 362 },
            { new Vector2(-73.5f, -59.5f), 363 },
            { new Vector2(-16.75f, -15.125f), 364 },
            { new Vector2(16.0f, -21.75f), 365 },
            { new Vector2(-72.0f, 32.125f), 366 },
            { new Vector2(42.0f, 52.5f), 367 },
            { new Vector2(-10.75f, -32.875f), 368 },
            { new Vector2(-11.875f, -32.875f), 369 },
            { new Vector2(-39.5f, -50.5f), 370 },
            { new Vector2(-44.0f, -55.0f), 371 },
            { new Vector2(-35.0f, -55.0f), 372 },
            { new Vector2(-39.5f, -59.5f), 373 },
            { new Vector2(2.25f, -22.5675f), 1215 },
            { new Vector2(2.75f, -22.0675f), 1216 },
            { new Vector2(-77.0f, -59.5f), 1217 },
            { new Vector2(-77.0f, -58.5f), 1217 },
            { new Vector2(-53.5f, -37.5f), 1218 },
            { new Vector2(-53.5f, -36.5f), 1218 },
            { new Vector2(19.0f, -30.0f), 1219 },
            { new Vector2(19.0f, -29.0f), 1219 },
            { new Vector2(5.0f, 0.0f), 1220 },
            { new Vector2(5.0f, 1.0f), 1220 },

            { new Vector2(-40f, 37.5f), 1409 },
            { new Vector2(-67f, -51f), 1410 },
            { new Vector2(-65.5f, -52.5f), 1411 },
            { new Vector2(-68.5f, -52.5f), 1412 },
            { new Vector2(-68.5f, -49.5f), 1413 },
            { new Vector2(-65.5f, -49.5f), 1414 },
            { new Vector2(14f, -52.5f), 1415 },
            { new Vector2(-21f, -3f), 1416 },
            { new Vector2(61.5f, -20f), 1417 },
            { new Vector2(-76.5f, 32f), 1418 },
            { new Vector2(-39.5f, -55f), 1419 },
            { new Vector2(67.25f, 27f), 1420 },
            { new Vector2(65f, 27f), 1421 },
            { new Vector2(66.5f, 27f), 1422 },
            { new Vector2(68f, 27f), 1423 },
            { new Vector2(69.5f, 27f), 1424 },
            { new Vector2(-19.5f, -26.5f), 1425 },
            { new Vector2(13.75f, -14.5f), 1426 },
            { new Vector2(-27.25f, 36.5f), 1427 },
            { new Vector2(61.25f, 38.5f), 1428 },
            }},
            {"level_5.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-40.0f, -58.5f), 374 },
            { new Vector2(-28.0f, -58.75f), 375 },
            { new Vector2(53.0f, -60.75f), 376 },
            { new Vector2(-29.0f, -5.75f), 377 },
            { new Vector2(58.0f, -60.5f), 378 },
            { new Vector2(-32.5f, 4.0f), 379 },
            { new Vector2(-11.75f, -49.75f), 380 },
            { new Vector2(-11.0f, -49.75f), 381 },
            { new Vector2(-11.5f, -29.0f), 382 },
            { new Vector2(20.5f, 20.5f), 383 },
            { new Vector2(21.5f, 20.5f), 384 },
            { new Vector2(22.5f, 20.5f), 385 },
            { new Vector2(35.5f, 0.5f), 386 },
            { new Vector2(36.25f, 0.5f), 387 },
            { new Vector2(75.0f, -7.5f), 388 },
            { new Vector2(-29.5f, 41.0f), 389 },
            { new Vector2(-28.5f, 41.0f), 390 },
            { new Vector2(58.5f, 42.5f), 391 },
            { new Vector2(-1.5f, -57.125f), 392 },
            { new Vector2(16.25f, -41.25f), 393 },
            { new Vector2(48.0f, -55.5f), 394 },
            { new Vector2(27.125f, -22.625f), 395 },
            { new Vector2(23.875f, 21.375f), 396 },
            { new Vector2(76.75f, -16.625f), 397 },
            { new Vector2(64.125f, 24.625f), 398 },
            { new Vector2(-76.0f, 60.625f), 399 },
            { new Vector2(6.125f, 43.375f), 400 },
            { new Vector2(11.125f, 63.75f), 401 },
            { new Vector2(-58.0f, 0.25f), 402 },
            { new Vector2(-56.0f, -4.0f), 403 },
            { new Vector2(-19.0f, -49.875f), 404 },
            { new Vector2(-24.5f, -50.0f), 405 },
            { new Vector2(36.125f, 28.375f), 406 },
            { new Vector2(-6.5f, 70.5f), 407 },
            { new Vector2(59.5f, 43.0f), 408 },
            { new Vector2(73.0f, 39.75f), 409 },
            { new Vector2(-7.5f, -32.5f), 410 },
            { new Vector2(-7.5f, -33.5f), 411 },
            { new Vector2(-7.5f, -34.5f), 412 },
            { new Vector2(4.5f, -40.5f), 413 },
            { new Vector2(4.5f, -41.25f), 414 },
            { new Vector2(48.0f, -53.5f), 415 },
            { new Vector2(48.0f, -54.25f), 416 },
            { new Vector2(68.25f, -49.25f), 417 },
            { new Vector2(69.25f, -49.25f), 418 },
            { new Vector2(49.75f, -15.25f), 419 },
            { new Vector2(50.5f, -14.5f), 420 },
            { new Vector2(50.5f, -15.25f), 421 },
            { new Vector2(52.5f, 8.75f), 422 },
            { new Vector2(52.5f, 7.75f), 423 },
            { new Vector2(52.5f, 6.75f), 424 },
            { new Vector2(3.25f, 45.75f), 425 },
            { new Vector2(4.0f, 45.75f), 426 },
            { new Vector2(2.5f, 45.75f), 427 },
            { new Vector2(50.25f, 36.5f), 428 },
            { new Vector2(51.25f, 36.5f), 429 },
            { new Vector2(50.75f, 37.0f), 430 },
            { new Vector2(50.75f, 36.0f), 431 },
            { new Vector2(42.75f, -60.25f), 432 },
            { new Vector2(63.5f, -60.5f), 433 },
            { new Vector2(64.5f, -60.5f), 434 },
            { new Vector2(-54.0f, 17.625f), 435 },
            { new Vector2(60.25f, 38.25f), 436 },
            { new Vector2(51.0f, 65.5f), 437 },
            { new Vector2(47.0f, 65.5f), 438 },
            { new Vector2(-39.0f, -35.875f), 439 },
            { new Vector2(-38.0f, -40.0f), 440 },
            { new Vector2(28.5f, -70.75f), 441 },
            { new Vector2(71.5f, -28.5f), 445 },
            { new Vector2(-14.75f, -39.0f), 446 },
            { new Vector2(30.0f, -70.75f), 447 },
            { new Vector2(41.125f, 15.625f), 448 },
            { new Vector2(-58.0f, 62.0f), 449 },
            { new Vector2(69.125f, 43.75f), 450 },
            { new Vector2(-54.0f, 60.0f), 451 },
            { new Vector2(24.5f, 13.5f), 452 },
            { new Vector2(20.0f, 9.0f), 453 },
            { new Vector2(29.0f, 9.0f), 454 },
            { new Vector2(24.5f, 4.5f), 455 },
            { new Vector2(-3.5f, -22.875f), 1221 },
            { new Vector2(-3.0f, -22.375f), 1222 },
            { new Vector2(-17.0f, -34.5f), 1223 },
            { new Vector2(-17.0f, -33.5f), 1223 },
            { new Vector2(33.0f, -55.0f), 1224 },
            { new Vector2(33.0f, -54.0f), 1224 },
            { new Vector2(-3.0f, 23.0f), 1225 },
            { new Vector2(-3.0f, 24.0f), 1225 },
            { new Vector2(43.0f, -10.5f), 1226 },
            { new Vector2(43.0f, -9.5f), 1226 },
            { new Vector2(-68.0f, 56.0f), 1227 },
            { new Vector2(-68.0f, 57.0f), 1227 },
            { new Vector2(65.0f, 33.5f), 1228 },
            { new Vector2(65.0f, 34.5f), 1228 },

            { new Vector2(20f, -55f), 1429 },
            { new Vector2(21.5f, -56.5f), 1430 },
            { new Vector2(18.5f, -56.5f), 1431 },
            { new Vector2(18.5f, -53.5f), 1432 },
            { new Vector2(21.5f, -53.5f), 1433 },
            { new Vector2(-43f, -40f), 1434 },
            { new Vector2(-13.5f, -50f), 1435 },
            { new Vector2(-19f, -34.5f), 1436 },
            { new Vector2(-51f, -1.5f), 1437 },
            { new Vector2(-36f, 0f), 1438 },
            { new Vector2(-28f, -21f), 1439 },
            { new Vector2(64.5f, -24f), 1440 },
            { new Vector2(-62f, 60f), 1441 },
            { new Vector2(50f, 58f), 1442 },
            { new Vector2(24.5f, 9f), 1443 },
            { new Vector2(64f, -68f), 1444 },
            { new Vector2(65f, -69f), 1445 },
            { new Vector2(63f, -69f), 1446 },
            { new Vector2(65f, -67f), 1447 },
            { new Vector2(63f, -67f), 1448 },
            { new Vector2(-32.5f, 14.5f), 1449 },
            { new Vector2(-17.5f, 26.5f), 1450 },
            { new Vector2(36f, 27.5f), 1451 },
            { new Vector2(50.25f, 10.5f), 1452 },
            }},
            {"level_bonus_2.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-2.5f, 23.0f), 456 },
            { new Vector2(-4.0f, 23.0f), 457 },
            { new Vector2(-28.0f, 5.0f), 458 },
            { new Vector2(-27.0f, 5.0f), 459 },
            { new Vector2(-26.0f, 5.0f), 460 },
            { new Vector2(-28.0f, 6.0f), 461 },
            { new Vector2(-27.0f, 6.0f), 462 },
            { new Vector2(-26.0f, 6.0f), 463 },
            { new Vector2(-28.0f, 7.0f), 464 },
            { new Vector2(-27.0f, 7.0f), 465 },
            { new Vector2(-26.0f, 7.0f), 466 },
            { new Vector2(-17.0f, 25.0f), 467 },
            { new Vector2(-16.0f, 25.0f), 468 },
            { new Vector2(-17.0f, 24.0f), 469 },
            { new Vector2(-16.0f, 24.0f), 470 },
            { new Vector2(-17.0f, 23.0f), 471 },
            { new Vector2(-16.0f, 23.0f), 472 },
            { new Vector2(-15.0f, 25.0f), 473 },
            { new Vector2(-15.0f, 24.0f), 474 },
            { new Vector2(-15.0f, 23.0f), 475 },
            { new Vector2(-14.0f, 23.0f), 476 },
            { new Vector2(-14.0f, 24.0f), 477 },
            { new Vector2(-14.0f, 25.0f), 478 },
            { new Vector2(-18.0f, 25.0f), 479 },
            { new Vector2(-18.0f, 24.0f), 480 },
            { new Vector2(-25.25f, 16.25f), 481 },
            { new Vector2(-25.25f, 15.25f), 482 },
            { new Vector2(-29.75f, 16.25f), 483 },
            { new Vector2(-29.75f, 15.25f), 484 },
            { new Vector2(6.5f, 23.0f), 485 },
            { new Vector2(7.5f, 23.0f), 486 },
            { new Vector2(8.5f, 23.0f), 487 },
            { new Vector2(6.5f, 24.0f), 488 },
            { new Vector2(7.5f, 24.0f), 489 },
            { new Vector2(8.5f, 24.0f), 490 },
            { new Vector2(6.5f, 25.0f), 491 },
            { new Vector2(7.5f, 25.0f), 492 },
            { new Vector2(8.5f, 25.0f), 493 },
            { new Vector2(-4.5f, 16.5f), 494 },
            { new Vector2(-3.5f, 16.5f), 495 },
            { new Vector2(-2.5f, 16.5f), 496 },
            { new Vector2(-4.5f, 17.5f), 497 },
            { new Vector2(-3.5f, 17.5f), 498 },
            { new Vector2(-2.5f, 17.5f), 499 },
            { new Vector2(-4.5f, 18.5f), 500 },
            { new Vector2(-3.5f, 18.5f), 501 },
            { new Vector2(-2.5f, 18.5f), 502 },
            { new Vector2(-1.5f, 16.5f), 503 },
            { new Vector2(-1.5f, 17.5f), 504 },
            { new Vector2(-1.5f, 18.5f), 505 },
            { new Vector2(-5.5f, 16.5f), 506 },
            { new Vector2(-5.5f, 17.5f), 507 },
            { new Vector2(-5.5f, 18.5f), 508 },
            { new Vector2(5.5f, 23.0f), 509 },
            { new Vector2(5.5f, 24.0f), 510 },
            { new Vector2(5.5f, 25.0f), 511 },
            { new Vector2(9.5f, 23.0f), 512 },
            { new Vector2(9.5f, 24.0f), 513 },
            { new Vector2(9.5f, 25.0f), 514 },
            { new Vector2(14.5f, 13.0f), 515 },
            { new Vector2(15.5f, 13.0f), 516 },
            { new Vector2(16.5f, 13.0f), 517 },
            { new Vector2(-20.0f, 45.5f), 518 },
            { new Vector2(-20.0f, 44.5f), 519 },
            { new Vector2(-21.0f, 45.5f), 520 },
            { new Vector2(2.5f, 40.75f), 521 },
            { new Vector2(3.5f, 40.75f), 522 },
            { new Vector2(4.5f, 40.75f), 523 },
            { new Vector2(4.5f, 41.75f), 524 },
            { new Vector2(4.5f, 42.75f), 525 },
            { new Vector2(4.5f, 43.75f), 526 },
            { new Vector2(4.5f, 44.75f), 527 },
            { new Vector2(4.5f, 45.75f), 528 },
            { new Vector2(4.5f, 46.75f), 529 },
            { new Vector2(12.0f, 49.5f), 530 },
            { new Vector2(13.0f, 49.5f), 531 },
            { new Vector2(14.0f, 49.5f), 532 },
            { new Vector2(-14.75f, 3.5f), 533 },
            { new Vector2(3.5f, 28.5f), 534 },
            { new Vector2(14.5f, 25.25f), 535 },
            { new Vector2(21.25f, 29.25f), 536 },
            { new Vector2(-21.0f, 43.5f), 537 },
            { new Vector2(20.125f, 49.0f), 538 },
            { new Vector2(17.5f, 42.5f), 539 },
            { new Vector2(-18.0f, 23.0f), 540 },

            { new Vector2(-27.5f, 15.5f), 1453 },
            { new Vector2(-8.5f, 2.5f), 1632 },
            { new Vector2(22f, 32.5f), 1633 },
            { new Vector2(-6.5f, 4.5f), 1634 },
            }},
            {"level_6.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-29.0f, -6.0f), 541 },
            { new Vector2(-26.5f, 24.0f), 542 },
            { new Vector2(-66.125f, -53.5f), 543 },
            { new Vector2(-65.25f, -53.5f), 544 },
            { new Vector2(-22.25f, -53.25f), 545 },
            { new Vector2(-19.0f, -53.25f), 546 },
            { new Vector2(49.25f, -46.25f), 547 },
            { new Vector2(49.25f, -47.5f), 548 },
            { new Vector2(49.25f, -48.75f), 549 },
            { new Vector2(-42.0f, -14.5f), 550 },
            { new Vector2(-41.0f, -14.5f), 551 },
            { new Vector2(-40.0f, -14.5f), 552 },
            { new Vector2(-0.25f, -15.5f), 553 },
            { new Vector2(0.75f, -15.5f), 554 },
            { new Vector2(-29.5f, 46.5f), 555 },
            { new Vector2(3.0f, 30.0f), 556 },
            { new Vector2(7.0f, 38.0f), 557 },
            { new Vector2(13.0f, 32.125f), 558 },
            { new Vector2(13.0f, 34.0f), 559 },
            { new Vector2(-58.0f, -63.625f), 560 },
            { new Vector2(-8.625f, -40.875f), 561 },
            { new Vector2(-65.125f, -20.375f), 562 },
            { new Vector2(-71.625f, 6.5f), 563 },
            { new Vector2(-70.875f, -2.125f), 564 },
            { new Vector2(22.75f, -17.25f), 565 },
            { new Vector2(16.875f, 24.375f), 566 },
            { new Vector2(-58.0f, 0.25f), 567 },
            { new Vector2(-16.0f, -55.5f), 568 },
            { new Vector2(3.0f, -57.5f), 569 },
            { new Vector2(20.0f, -49.375f), 570 },
            { new Vector2(5.0f, 28.25f), 571 },
            { new Vector2(9.0f, 26.25f), 572 },
            { new Vector2(13.0f, 29.75f), 573 },
            { new Vector2(11.0f, 34.25f), 574 },
            { new Vector2(18.25f, -37.5f), 575 },
            { new Vector2(-27.75f, -43.0f), 576 },
            { new Vector2(-27.75f, -44.0f), 577 },
            { new Vector2(-27.75f, -45.0f), 578 },
            { new Vector2(44.5f, -34.75f), 579 },
            { new Vector2(44.5f, -35.75f), 580 },
            { new Vector2(44.5f, -36.75f), 581 },
            { new Vector2(-40.75f, -24.75f), 582 },
            { new Vector2(-40.75f, -25.75f), 583 },
            { new Vector2(-41.25f, 2.5f), 584 },
            { new Vector2(-40.25f, 2.5f), 585 },
            { new Vector2(-31.5f, 24.625f), 586 },
            { new Vector2(-31.5f, 25.5f), 587 },
            { new Vector2(28.75f, 7.75f), 588 },
            { new Vector2(29.5f, 7.75f), 589 },
            { new Vector2(15.0f, 26.125f), 590 },
            { new Vector2(13.0f, 26.125f), 591 },
            { new Vector2(30.25f, 7.75f), 592 },
            { new Vector2(18.25f, -38.25f), 593 },
            { new Vector2(-7.0f, -23.5f), 594 },
            { new Vector2(7.0f, 26.0f), 595 },
            { new Vector2(7.0f, 34.25f), 596 },
            { new Vector2(5.0f, 26.25f), 597 },
            { new Vector2(-39.0f, -35.875f), 598 },
            { new Vector2(-21.0f, -28.0f), 599 },
            { new Vector2(-58.0f, -28.0f), 600 },
            { new Vector2(-2.0f, -26.5f), 601 },
            { new Vector2(-22.0f, 13.75f), 602 },
            { new Vector2(38.0f, 23.0f), 603 },
            { new Vector2(7.0f, 32.125f), 604 },
            { new Vector2(2.0f, -26.5f), 605 },
            { new Vector2(-26.0f, 12.5f), 606 },
            { new Vector2(-21.25f, -53.25f), 607 },
            { new Vector2(-20.0f, -53.25f), 608 },
            { new Vector2(46.5f, 8.0f), 1229 },
            { new Vector2(47.0f, 8.5f), 1230 },
            { new Vector2(-38.5f, -44.0f), 1231 },
            { new Vector2(-38.5f, -43.0f), 1231 },
            { new Vector2(7.0f, -37.5f), 1232 },
            { new Vector2(7.0f, -36.5f), 1232 },
            { new Vector2(34.0f, -56.5f), 1233 },
            { new Vector2(34.0f, -55.5f), 1233 },
            { new Vector2(43.0f, -47.0f), 1234 },
            { new Vector2(43.0f, -46.0f), 1234 },
            { new Vector2(-58.0f, 8.5f), 1235 },
            { new Vector2(-58.0f, 9.5f), 1235 },
            { new Vector2(-33.0f, 0.5f), 1236 },
            { new Vector2(-33.0f, 1.5f), 1236 },
            { new Vector2(-15.5f, -28.0f), 1237 },
            { new Vector2(-15.5f, -27.0f), 1237 },
            { new Vector2(20.5f, -17.5f), 1238 },
            { new Vector2(20.5f, -16.5f), 1238 },
            { new Vector2(0.5f, 35.0f), 1239 },
            { new Vector2(0.5f, 36.0f), 1239 },

            { new Vector2(50.5f, -57f), 1454 },
            { new Vector2(49f, -58.5f), 1455 },
            { new Vector2(49f, -55.5f), 1456 },
            { new Vector2(52f, -58.5f), 1457 },
            { new Vector2(52f, -55.5f), 1458 },
            { new Vector2(-6.5f, -61.5f), 1459 },
            { new Vector2(-6.5f, -58.5f), 1460 },
            { new Vector2(-3.5f, -58.5f), 1461 },
            { new Vector2(-6.5f, -55.5f), 1462 },
            { new Vector2(-9.5f, -58.5f), 1463 },
            { new Vector2(-6f, -26.5f), 1464 },
            { new Vector2(-20f, 8.5f), 1465 },
            { new Vector2(-16f, -56.5f), 1466 },
            { new Vector2(3f, -58.5f), 1467 },
            { new Vector2(-18.5f, 9.5f), 1468 },
            { new Vector2(-24.75f, 36.5f), 1469 },
            { new Vector2(-22.25f, 36.5f), 1470 },
            }},
            {"level_boss_2.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(1.0f, -42.5f), 609 },
            { new Vector2(1.0f, -26.25f), 1321 },
            }},
            {"level_7.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-39.5f, -72.5f), 610 },
            { new Vector2(-76.5f, -57.0f), 611 },
            { new Vector2(23.5f, -43.25f), 612 },
            { new Vector2(24.25f, -43.25f), 613 },
            { new Vector2(-57.75f, -4.5f), 614 },
            { new Vector2(-57.75f, -3.75f), 615 },
            { new Vector2(-57.75f, -3.0f), 616 },
            { new Vector2(-10.0f, 3.0f), 617 },
            { new Vector2(-10.0f, 6.0f), 618 },
            { new Vector2(-10.0f, 5.0f), 619 },
            { new Vector2(-10.0f, 4.0f), 620 },
            { new Vector2(28.5f, -8.0f), 621 },
            { new Vector2(28.5f, -9.0f), 622 },
            { new Vector2(28.5f, -10.0f), 623 },
            { new Vector2(49.0f, 23.5f), 624 },
            { new Vector2(50.0f, 23.5f), 625 },
            { new Vector2(51.0f, 23.5f), 626 },
            { new Vector2(53.0f, 20.0f), 627 },
            { new Vector2(-2.0f, -13.0f), 628 },
            { new Vector2(2.5f, -29.0f), 629 },
            { new Vector2(18.625f, 14.375f), 630 },
            { new Vector2(62.5f, -20.0f), 631 },
            { new Vector2(-49.0f, 43.25f), 632 },
            { new Vector2(-30.0f, 30.25f), 633 },
            { new Vector2(-63.0f, -34.75f), 634 },
            { new Vector2(-47.875f, 7.375f), 635 },
            { new Vector2(-3.0f, -20.5f), 636 },
            { new Vector2(58.5f, 49.0f), 637 },
            { new Vector2(7.25f, -53.75f), 638 },
            { new Vector2(-50.75f, -35.75f), 639 },
            { new Vector2(18.0f, -34.75f), 640 },
            { new Vector2(19.0f, -35.75f), 641 },
            { new Vector2(20.0f, -34.75f), 642 },
            { new Vector2(19.0f, -34.0f), 643 },
            { new Vector2(-34.5f, 0.25f), 644 },
            { new Vector2(-34.5f, -0.75f), 645 },
            { new Vector2(-34.5f, -1.75f), 646 },
            { new Vector2(18.5f, -8.25f), 647 },
            { new Vector2(19.25f, -8.25f), 648 },
            { new Vector2(17.75f, -8.25f), 649 },
            { new Vector2(-45.75f, 40.75f), 650 },
            { new Vector2(-45.0f, 40.75f), 651 },
            { new Vector2(-44.25f, 40.75f), 652 },
            { new Vector2(-13.5f, 31.0f), 653 },
            { new Vector2(-13.5f, 30.25f), 654 },
            { new Vector2(47.5f, 49.5f), 655 },
            { new Vector2(48.5f, 49.5f), 656 },
            { new Vector2(-54.0f, -45.5f), 657 },
            { new Vector2(7.25f, -52.75f), 658 },
            { new Vector2(29.0f, 0.0f), 659 },
            { new Vector2(-16.0f, 50.0f), 660 },
            { new Vector2(-8.25f, 43.25f), 661 },
            { new Vector2(8.125f, 13.625f), 662 },
            { new Vector2(-60.375f, -45.625f), 663 },
            { new Vector2(24.25f, -52.375f), 664 },
            { new Vector2(42.5f, -43.125f), 665 },
            { new Vector2(-33.0f, -7.25f), 666 },
            { new Vector2(60.625f, 9.625f), 667 },
            { new Vector2(-11.5f, -68.5f), 668 },
            { new Vector2(-16.0f, -73.0f), 669 },
            { new Vector2(-7.0f, -73.0f), 670 },
            { new Vector2(-11.5f, -77.5f), 671 },
            { new Vector2(-61.0f, -35.0f), 1240 },
            { new Vector2(-61.0f, -34.0f), 1240 },
            { new Vector2(39.125f, -42.125f), 1241 },
            { new Vector2(39.125f, -41.125f), 1241 },
            { new Vector2(-46.0f, 7.0f), 1242 },
            { new Vector2(-46.0f, 8.0f), 1242 },
            { new Vector2(-16.0f, 42.0f), 1243 },
            { new Vector2(-16.0f, 43.0f), 1243 },

            { new Vector2(-54.5f, -52.5f), 1471 },
            { new Vector2(19f, -35f), 1472 },
            { new Vector2(-39.5f, -21.5f), 1473 },
            { new Vector2(12.5f, -6.5f), 1474 },
            { new Vector2(-8f, 29f), 1475 },
            { new Vector2(-8f, 20.5f), 1476 },
            { new Vector2(61.5f, 26.5f), 1477 },
            { new Vector2(-2f, 37.5f), 1478 },
            { new Vector2(-11.5f, -73f), 1479 },
            { new Vector2(14.5f, -43.5f), 1480 },
            { new Vector2(-34f, -8.5f), 1481 },
            { new Vector2(63.5f, 5.5f), 1482 },
            }},
            {"level_8.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(41.5f, -55.75f), 672 },
            { new Vector2(-15.25f, -41.5f), 673 },
            { new Vector2(-14.25f, -41.5f), 674 },
            { new Vector2(23.25f, -34.5f), 675 },
            { new Vector2(22.25f, -34.5f), 676 },
            { new Vector2(-16.5f, 7.5f), 677 },
            { new Vector2(-15.75f, 7.5f), 678 },
            { new Vector2(37.5f, 3.25f), 679 },
            { new Vector2(37.5f, 2.25f), 680 },
            { new Vector2(-14.5f, 34.5f), 681 },
            { new Vector2(-14.5f, 33.5f), 682 },
            { new Vector2(-14.5f, 32.5f), 683 },
            { new Vector2(40.5f, -55.75f), 684 },
            { new Vector2(21.625f, 32.5f), 685 },
            { new Vector2(-9.5f, 70.25f), 686 },
            { new Vector2(-7.5f, 70.25f), 687 },
            { new Vector2(26.875f, -36.0f), 688 },
            { new Vector2(-75.0f, -19.875f), 689 },
            { new Vector2(-3.25f, -28.5f), 690 },
            { new Vector2(5.5f, 1.625f), 691 },
            { new Vector2(45.5f, 24.375f), 692 },
            { new Vector2(-31.25f, 31.75f), 693 },
            { new Vector2(26.625f, 39.0f), 694 },
            { new Vector2(-15.0f, 67.5f), 695 },
            { new Vector2(-59.75f, 4.125f), 696 },
            { new Vector2(-42.5f, -39.0f), 697 },
            { new Vector2(-42.5f, -40.0f), 698 },
            { new Vector2(-5.5f, -70.5f), 699 },
            { new Vector2(47.5f, -35.375f), 700 },
            { new Vector2(-64.5f, -19.0f), 701 },
            { new Vector2(-11.625f, 28.75f), 702 },
            { new Vector2(42.5f, 3.5f), 703 },
            { new Vector2(42.5f, 2.5f), 704 },
            { new Vector2(25.25f, 39.25f), 705 },
            { new Vector2(-10.5f, -44.0f), 706 },
            { new Vector2(-9.5f, -44.0f), 707 },
            { new Vector2(-8.5f, -44.0f), 708 },
            { new Vector2(67.5f, -33.5f), 709 },
            { new Vector2(67.5f, -32.5f), 710 },
            { new Vector2(67.5f, -31.5f), 711 },
            { new Vector2(-64.5f, -16.875f), 712 },
            { new Vector2(-64.5f, -17.875f), 713 },
            { new Vector2(-64.5f, -19.875f), 714 },
            { new Vector2(-64.5f, -20.875f), 715 },
            { new Vector2(45.5f, 29.5f), 716 },
            { new Vector2(40.5f, 24.0f), 717 },
            { new Vector2(41.5f, 24.0f), 718 },
            { new Vector2(-48.0f, 64.0f), 719 },
            { new Vector2(-48.0f, 63.0f), 720 },
            { new Vector2(-48.0f, 62.0f), 721 },
            { new Vector2(-48.0f, 65.0f), 722 },
            { new Vector2(-48.0f, 66.0f), 723 },
            { new Vector2(-48.0f, 61.0f), 724 },
            { new Vector2(-48.0f, 60.0f), 725 },
            { new Vector2(-48.0f, 59.0f), 726 },
            { new Vector2(-48.0f, 58.0f), 727 },
            { new Vector2(-48.0f, 67.0f), 728 },
            { new Vector2(-48.0f, 68.0f), 729 },
            { new Vector2(-13.875f, 30.875f), 730 },
            { new Vector2(-12.875f, 30.75f), 731 },
            { new Vector2(-13.375f, 31.375f), 732 },
            { new Vector2(24.5f, 38.5f), 733 },
            { new Vector2(25.375f, 38.125f), 734 },
            { new Vector2(26.25f, 39.75f), 735 },
            { new Vector2(27.125f, 38.125f), 736 },
            { new Vector2(-4.5f, -51.25f), 737 },
            { new Vector2(45.5f, 54.5f), 738 },
            { new Vector2(39.5f, -55.75f), 739 },
            { new Vector2(19.875f, 38.375f), 740 },
            { new Vector2(-72.25f, -34.0f), 741 },
            { new Vector2(20.75f, 35.5f), 742 },
            { new Vector2(-3.5f, 66.5f), 743 },
            { new Vector2(91.0f, -32.0f), 744 },
            { new Vector2(-19.0f, -51.0f), 745 },
            { new Vector2(-75.625f, -25.25f), 746 },
            { new Vector2(75.5f, -26.375f), 747 },
            { new Vector2(-65.125f, 60.875f), 748 },
            { new Vector2(20.5f, 32.5f), 749 },
            { new Vector2(-8.5f, 62.25f), 750 },
            { new Vector2(-62.5f, 49.0f), 751 },
            { new Vector2(-67.0f, 44.5f), 752 },
            { new Vector2(-58.0f, 44.5f), 753 },
            { new Vector2(-62.5f, 40.0f), 754 },
            { new Vector2(-60.25f, -29.25f), 1244 },
            { new Vector2(-59.75f, -28.75f), 1245 },
            { new Vector2(21.0f, -21.0f), 1246 },
            { new Vector2(21.5f, -20.5f), 1247 },
            { new Vector2(-38.5f, -36.0f), 1248 },
            { new Vector2(-38.5f, -35.0f), 1248 },
            { new Vector2(25.0f, -36.0f), 1249 },
            { new Vector2(25.0f, -35.0f), 1249 },
            { new Vector2(-13.5f, 29.25f), 1250 },
            { new Vector2(-13.5f, 30.25f), 1250 },
            { new Vector2(34.0f, 19.0f), 1251 },
            { new Vector2(34.0f, 20.0f), 1251 },
            { new Vector2(-31.0f, 66.0f), 1252 },
            { new Vector2(-31.0f, 67.0f), 1252 },

            { new Vector2(-22f, -58f), 1483 },
            { new Vector2(21f, 3f), 1484 },
            { new Vector2(-20.5f, -59.5f), 1485 },
            { new Vector2(-23.5f, -59.5f), 1486 },
            { new Vector2(-20.5f, -56.5f), 1487 },
            { new Vector2(-23.5f, -56.5f), 1488 },
            { new Vector2(22.5f, 1.5f), 1489 },
            { new Vector2(19.5f, 1.5f), 1490 },
            { new Vector2(19.5f, 4.5f), 1491 },
            { new Vector2(22.5f, 4.5f), 1492 },
            { new Vector2(-24f, -50f), 1493 },
            { new Vector2(-1.5f, -63.5f), 1494 },
            { new Vector2(13f, -35.5f), 1495 },
            { new Vector2(-35.5f, 4.5f), 1496 },
            { new Vector2(-19f, 30f), 1497 },
            { new Vector2(69.5f, -16.5f), 1498 },
            { new Vector2(-54.5f, 37.5f), 1499 },
            { new Vector2(-62.5f, 44.5f), 1500 },
            { new Vector2(-69.5f, 53.5f), 1501 },
            { new Vector2(-72.5f, 54.5f), 1502 },
            { new Vector2(-70.5f, 54.5f), 1503 },
            { new Vector2(-68.5f, 54.5f), 1504 },
            { new Vector2(-66.5f, 54.5f), 1505 },
            { new Vector2(-15.5f, -30.5f), 1506 },
            { new Vector2(-4.5f, 0.5f), 1507 },
            { new Vector2(-9.5f, 61.5f), 1508 },
            { new Vector2(-8.5f, 69.5f), 1509 },
            { new Vector2(92f, -54.5f), 1510 },
            { new Vector2(-36.5f, 48.5f), 1637 },
            }},
            {"level_9.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(62.0f, -31.0f), 755 },
            { new Vector2(49.0f, -26.0f), 756 },
            { new Vector2(79.5f, -49.0f), 757 },
            { new Vector2(-10.5f, -31.5f), 758 },
            { new Vector2(-10.5f, -32.5f), 759 },
            { new Vector2(59.5f, -58.5f), 760 },
            { new Vector2(59.5f, -57.5f), 761 },
            { new Vector2(36.0f, -11.0f), 762 },
            { new Vector2(37.0f, -11.0f), 763 },
            { new Vector2(-62.5f, 54.5f), 764 },
            { new Vector2(-61.5f, 54.5f), 765 },
            { new Vector2(-60.5f, 54.5f), 766 },
            { new Vector2(-24.0f, 35.0f), 767 },
            { new Vector2(-23.0f, 35.0f), 768 },
            { new Vector2(49.0f, 62.5f), 769 },
            { new Vector2(77.5f, -64.25f), 770 },
            { new Vector2(78.5f, -64.25f), 771 },
            { new Vector2(-81.0f, -26.0f), 772 },
            { new Vector2(-71.0f, 3.0f), 773 },
            { new Vector2(7.625f, -61.125f), 774 },
            { new Vector2(-56.875f, -29.875f), 775 },
            { new Vector2(-25.125f, 21.25f), 776 },
            { new Vector2(73.25f, -22.875f), 777 },
            { new Vector2(17.25f, 41.375f), 778 },
            { new Vector2(40.125f, 54.0f), 779 },
            { new Vector2(-74.625f, -62.5f), 780 },
            { new Vector2(57.0f, -21.0f), 781 },
            { new Vector2(47.0f, -21.0f), 782 },
            { new Vector2(33.5f, -71.5f), 783 },
            { new Vector2(34.5f, -71.5f), 784 },
            { new Vector2(-24.5f, -4.5f), 785 },
            { new Vector2(-82.5f, 63.5f), 786 },
            { new Vector2(-82.5f, 62.5f), 787 },
            { new Vector2(72.5f, 44.5f), 788 },
            { new Vector2(-43.0f, -63.0f), 789 },
            { new Vector2(-43.0f, -64.0f), 790 },
            { new Vector2(-43.0f, -65.0f), 791 },
            { new Vector2(68.5f, -37.625f), 792 },
            { new Vector2(68.5f, -36.625f), 793 },
            { new Vector2(-27.5f, -4.5f), 794 },
            { new Vector2(-28.5f, -4.5f), 795 },
            { new Vector2(40.5f, 0.5f), 796 },
            { new Vector2(40.5f, -0.5f), 797 },
            { new Vector2(40.5f, -1.5f), 798 },
            { new Vector2(-6.5f, 55.0f), 799 },
            { new Vector2(-6.5f, 54.0f), 800 },
            { new Vector2(-6.5f, 53.0f), 801 },
            { new Vector2(-17.625f, -47.25f), 802 },
            { new Vector2(-35.0f, -11.5f), 803 },
            { new Vector2(-61.75f, -8.5f), 804 },
            { new Vector2(70.5f, 31.0f), 805 },
            { new Vector2(-67.875f, 9.125f), 806 },
            { new Vector2(-25.25f, 70.875f), 807 },
            { new Vector2(-73.125f, -63.875f), 808 },
            { new Vector2(-40.875f, -69.0f), 809 },
            { new Vector2(-79.5f, -42.25f), 810 },
            { new Vector2(65.5f, -40.0f), 811 },
            { new Vector2(-43.375f, -11.375f), 812 },
            { new Vector2(49.5f, 24.0f), 813 },
            { new Vector2(-41.5f, 32.0f), 814 },
            { new Vector2(-72.5f, -42.5f), 815 },
            { new Vector2(-67.5f, 38.5f), 816 },
            { new Vector2(-66.625f, 38.5f), 817 },
            { new Vector2(-37.375f, 33.125f), 818 },
            { new Vector2(-34.75f, -68.0f), 1253 },
            { new Vector2(-34.25f, -67.5f), 1254 },
            { new Vector2(67.25f, -21.5f), 1255 },
            { new Vector2(67.75f, -21.0f), 1256 },
            { new Vector2(-25.375f, 37.375f), 1257 },
            { new Vector2(-24.875f, 37.875f), 1258 },
            { new Vector2(-39.0f, -68.0f), 1259 },
            { new Vector2(-39.0f, -67.0f), 1259 },
            { new Vector2(-61.5f, -44.0f), 1260 },
            { new Vector2(-61.5f, -43.0f), 1260 },
            { new Vector2(-6.0f, -30.5f), 1261 },
            { new Vector2(-6.0f, -29.5f), 1261 },
            { new Vector2(57.0f, -56.5f), 1262 },
            { new Vector2(57.0f, -55.5f), 1262 },
            { new Vector2(-40.0f, 5.0f), 1263 },
            { new Vector2(-40.0f, 6.0f), 1263 },
            { new Vector2(25.0f, -27.0f), 1264 },
            { new Vector2(25.0f, -26.0f), 1264 },
            { new Vector2(16.75f, -7.375f), 1265 },
            { new Vector2(16.75f, -6.375f), 1265 },
            { new Vector2(70.125f, 14.625f), 1266 },
            { new Vector2(70.125f, 15.625f), 1266 },
            { new Vector2(-53.0f, 41.5f), 1267 },
            { new Vector2(-53.0f, 42.5f), 1267 },
            { new Vector2(17.0f, 63.5f), 1268 },
            { new Vector2(17.0f, 64.5f), 1268 },

            { new Vector2(-38f, -25.5f), 1511 },
            { new Vector2(65f, 72f), 1512 },
            { new Vector2(63.5f, 73.5f), 1513 },
            { new Vector2(66.5f, 73.5f), 1514 },
            { new Vector2(63.5f, 70.5f), 1515 },
            { new Vector2(66.5f, 70.5f), 1516 },
            { new Vector2(-88.5f, -30f), 1517 },
            { new Vector2(-79f, -37f), 1518 },
            { new Vector2(-25.5f, -31.5f), 1519 },
            { new Vector2(50.5f, -62.5f), 1520 },
            { new Vector2(-78.5f, -28.5f), 1521 },
            { new Vector2(11.5f, -17.5f), 1522 },
            { new Vector2(-39.5f, 36.5f), 1523 },
            { new Vector2(3f, 56f), 1524 },
            { new Vector2(54f, 37f), 1525 },
            { new Vector2(78.5f, 54f), 1526 },
            { new Vector2(83.5f, -29.5f), 1527 },
            { new Vector2(84.5f, -28.5f), 1528 },
            { new Vector2(78.5f, 50.5f), 1529 },
            { new Vector2(81f, 49.5f), 1530 },
            { new Vector2(82.5f, -28.5f), 1531 },
            { new Vector2(77f, 47.5f), 1532 },
            { new Vector2(84.5f, -30.5f), 1533 },
            { new Vector2(77f, 51.5f), 1534 },
            { new Vector2(82.5f, -30.5f), 1535 },
            { new Vector2(80f, 47.5f), 1536 },
            { new Vector2(76f, 49.5f), 1537 },
            { new Vector2(80f, 51.5f), 1538 },
            { new Vector2(-76f, -26.5f), 1539 },
            { new Vector2(-70f, 2.5f), 1540 },
            { new Vector2(54f, -26.5f), 1541 },
            { new Vector2(53f, -26.5f), 1542 },
            { new Vector2(39.75f, 13.5f), 1543 },
            { new Vector2(56.5f, 25.5f), 1544 },
            { new Vector2(-64.5f, 46.5f), 1545 },
            { new Vector2(19.25f, 39.5f), 1546 },
            { new Vector2(35f, 30.5f), 1547 },
            { new Vector2(45.625f, 52.5f), 1548 },
            { new Vector2(-84.5f, 44.5f), 1638 },
            }},
            {"level_bonus_3.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(0.25f, 21.0f), 819 },
            { new Vector2(5.0f, 4.5f), 820 },
            { new Vector2(6.0f, 4.5f), 821 },
            { new Vector2(0.0f, -20.0f), 822 },
            { new Vector2(1.0f, -20.0f), 823 },
            { new Vector2(2.0f, -20.0f), 824 },
            { new Vector2(2.0f, -19.0f), 825 },
            { new Vector2(0.0f, -19.0f), 826 },
            { new Vector2(1.0f, -19.0f), 827 },
            { new Vector2(0.0f, -18.0f), 828 },
            { new Vector2(1.0f, -18.0f), 829 },
            { new Vector2(2.0f, -18.0f), 830 },
            { new Vector2(-20.0f, -10.0f), 831 },
            { new Vector2(-19.0f, -10.0f), 832 },
            { new Vector2(-18.0f, -10.0f), 833 },
            { new Vector2(-18.0f, -9.0f), 834 },
            { new Vector2(-19.0f, -9.0f), 835 },
            { new Vector2(-20.0f, -9.0f), 836 },
            { new Vector2(-20.0f, -8.0f), 837 },
            { new Vector2(-19.0f, -8.0f), 838 },
            { new Vector2(-18.0f, -8.0f), 839 },
            { new Vector2(-5.0f, -2.5f), 840 },
            { new Vector2(-3.0f, -2.5f), 841 },
            { new Vector2(-4.0f, -2.5f), 842 },
            { new Vector2(-5.0f, -3.5f), 843 },
            { new Vector2(-3.0f, -3.5f), 844 },
            { new Vector2(-4.0f, -3.5f), 845 },
            { new Vector2(-5.0f, -4.5f), 846 },
            { new Vector2(-3.0f, -4.5f), 847 },
            { new Vector2(-4.0f, -4.5f), 848 },
            { new Vector2(0.0f, -2.5f), 849 },
            { new Vector2(2.0f, -2.5f), 850 },
            { new Vector2(1.0f, -2.5f), 851 },
            { new Vector2(0.0f, -3.5f), 852 },
            { new Vector2(2.0f, -3.5f), 853 },
            { new Vector2(1.0f, -3.5f), 854 },
            { new Vector2(0.0f, -4.5f), 855 },
            { new Vector2(2.0f, -4.5f), 856 },
            { new Vector2(1.0f, -4.5f), 857 },
            { new Vector2(7.5f, 22.5f), 858 },
            { new Vector2(7.5f, 21.5f), 859 },
            { new Vector2(7.5f, 20.5f), 860 },
            { new Vector2(7.5f, 19.5f), 861 },
            { new Vector2(-5.0f, 12.5f), 862 },
            { new Vector2(-4.0f, 12.5f), 863 },
            { new Vector2(-3.0f, 12.5f), 864 },
            { new Vector2(-3.0f, 11.5f), 865 },
            { new Vector2(-5.0f, 11.5f), 866 },
            { new Vector2(-5.0f, 10.5f), 867 },
            { new Vector2(-3.0f, 10.5f), 868 },
            { new Vector2(-4.0f, 10.5f), 869 },
            { new Vector2(20.0f, 15.0f), 870 },
            { new Vector2(21.0f, 15.0f), 871 },
            { new Vector2(22.0f, 15.0f), 872 },
            { new Vector2(22.0f, 16.0f), 873 },
            { new Vector2(20.0f, 16.0f), 874 },
            { new Vector2(20.0f, 17.0f), 875 },
            { new Vector2(21.0f, 17.0f), 876 },
            { new Vector2(22.0f, 17.0f), 877 },
            { new Vector2(-8.875f, -9.875f), 878 },
            { new Vector2(-4.0f, 11.25f), 879 },
            { new Vector2(21.0f, 15.75f), 880 },
            { new Vector2(-3.0f, -15.0f), 881 },
            { new Vector2(1.0f, -13.0f), 882 },
            }},
            {"level_boss_3.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(1.0f, -23.0f), 883 },
            { new Vector2(1.0f, 4f), 1322 },
            }},
            {"level_10.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-8.0f, -56.0f), 884 },
            { new Vector2(-22.0f, -14.5f), 885 },
            { new Vector2(-47.0f, 34.25f), 886 },
            { new Vector2(-1.125f, -46.625f), 887 },
            { new Vector2(0.0f, -47.375f), 888 },
            { new Vector2(1.5f, 42.0f), 889 },
            { new Vector2(3.5f, 42.0f), 890 },
            { new Vector2(-18.5f, -61.0f), 891 },
            { new Vector2(-21.5f, -61.0f), 892 },
            { new Vector2(2.5f, -48.5f), 893 },
            { new Vector2(52.5f, -51.0f), 894 },
            { new Vector2(-50.5f, -18.25f), 895 },
            { new Vector2(-72.0f, -4.75f), 896 },
            { new Vector2(-30.625f, 24.5f), 897 },
            { new Vector2(-3.625f, 7.75f), 898 },
            { new Vector2(2.0f, 20.875f), 899 },
            { new Vector2(58.125f, -7.625f), 900 },
            { new Vector2(-21.0f, 30.0f), 901 },
            { new Vector2(2.5f, 42.0f), 902 },
            { new Vector2(42.0f, 37.125f), 903 },
            { new Vector2(-20.0f, -61.0f), 904 },
            { new Vector2(7.5f, -42.5f), 905 },
            { new Vector2(2.25f, -44.5f), 906 },
            { new Vector2(17.0f, -50.5f), 907 },
            { new Vector2(-29.5f, -5.875f), 908 },
            { new Vector2(-12.0f, -51.75f), 909 },
            { new Vector2(18.0f, -50.5f), 910 },
            { new Vector2(-33.5f, -6.0f), 911 },
            { new Vector2(-20.0f, -14.5f), 912 },
            { new Vector2(-47.5f, 43.0f), 913 },
            { new Vector2(-34.625f, 22.5f), 1269 },
            { new Vector2(-34.125f, 23.0f), 1270 },
            { new Vector2(0.5f, -46.0f), 1271 },
            { new Vector2(0.5f, -45.0f), 1271 },
            { new Vector2(-60.0f, 55.5f), 1272 },
            { new Vector2(-60.0f, 56.5f), 1272 },
            { new Vector2(-10.0f, -54.0f), 1273 },
            { new Vector2(-10.0f, -53.0f), 1273 },
            { new Vector2(-5.5f, 7.0f), 1274 },
            { new Vector2(-5.5f, 8.0f), 1274 },
            { new Vector2(6.5f, 23.0f), 1275 },
            { new Vector2(6.5f, 24.0f), 1275 },

            { new Vector2(17.5f, -60.5f), 1549 },
            { new Vector2(13.5f, -3.5f), 1550 },
            { new Vector2(13.5f, -38.5f), 1551 },
            { new Vector2(-26.5f, -11.5f), 1552 },
            { new Vector2(-19.625f, 41.5f), 1553 },
            { new Vector2(-47.5f, 40.25f), 1639 },
            }},
            {"level_10_special.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-74.5f, 13.5f), 914 },
            { new Vector2(-79.0f, 9.0f), 915 },
            { new Vector2(-70.0f, 9.0f), 916 },
            { new Vector2(-74.5f, 4.5f), 917 },

            { new Vector2(-74.5f, 9f), 1554 },
            { new Vector2(-79f, -9.25f), 1555 },
            { new Vector2(-79f, -7f), 1556 },
            { new Vector2(-79f, -8.5f), 1557 },
            { new Vector2(-79f, -10f), 1558 },
            { new Vector2(-79f, -11.5f), 1559 },
            }},
            {"level_11.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(48.0f, -109.5f), 918 },
            { new Vector2(-79.5f, -56.75f), 919 },
            { new Vector2(39.875f, -44.5f), 920 },
            { new Vector2(-30.0f, -13.0f), 921 },
            { new Vector2(98.0f, 2.0f), 922 },
            { new Vector2(-86.0f, -50.0f), 923 },
            { new Vector2(-103.125f, -40.5f), 924 },
            { new Vector2(-102.0f, -41.625f), 925 },
            { new Vector2(-103.875f, 35.375f), 926 },
            { new Vector2(-103.125f, 34.625f), 927 },
            { new Vector2(-48.5f, -44.0f), 928 },
            { new Vector2(-49.5f, -44.0f), 929 },
            { new Vector2(-64.0f, -14.375f), 930 },
            { new Vector2(-67.5f, -7.5f), 931 },
            { new Vector2(-67.5f, -8.5f), 932 },
            { new Vector2(-32.25f, -9.375f), 933 },
            { new Vector2(-31.25f, -9.375f), 934 },
            { new Vector2(25.0f, -25.5f), 935 },
            { new Vector2(24.0f, -25.5f), 936 },
            { new Vector2(49.125f, -2.75f), 937 },
            { new Vector2(48.25f, -3.375f), 938 },
            { new Vector2(-50.0f, 39.5f), 939 },
            { new Vector2(-50.0f, 38.5f), 940 },
            { new Vector2(-9.25f, 44.75f), 941 },
            { new Vector2(-8.625f, 44.0f), 942 },
            { new Vector2(46.0f, 45.0f), 943 },
            { new Vector2(45.0f, 44.5f), 944 },
            { new Vector2(46.5f, 45.875f), 945 },
            { new Vector2(102.0f, -70.5f), 946 },
            { new Vector2(103.5f, -69.0f), 947 },
            { new Vector2(103.0f, -70.0f), 948 },
            { new Vector2(109.5f, -30.5f), 949 },
            { new Vector2(-65.75f, -3.75f), 950 },
            { new Vector2(-67.0f, -3.75f), 951 },
            { new Vector2(-103.625f, -39.375f), 952 },
            { new Vector2(-46.5f, -43.25f), 953 },
            { new Vector2(47.125f, -78.75f), 954 },
            { new Vector2(67.125f, -50.5f), 955 },
            { new Vector2(38.25f, -44.625f), 956 },
            { new Vector2(72.5f, -46.5f), 957 },
            { new Vector2(-88.5f, -28.0f), 958 },
            { new Vector2(-54.0f, -24.0f), 959 },
            { new Vector2(-68.0f, 29.0f), 960 },
            { new Vector2(8.875f, -24.0f), 961 },
            { new Vector2(-53.5f, 31.5f), 962 },
            { new Vector2(-35.5f, 45.5f), 963 },
            { new Vector2(-33.0f, 76.125f), 964 },
            { new Vector2(30.0f, 48.0f), 965 },
            { new Vector2(62.5f, 51.75f), 966 },
            { new Vector2(50.75f, 84.25f), 967 },
            { new Vector2(-111.5f, -50.0f), 968 },
            { new Vector2(-51.0f, -49.875f), 969 },
            { new Vector2(65.5f, -105.5f), 970 },
            { new Vector2(49.0f, -80.5f), 971 },
            { new Vector2(42.125f, -44.75f), 972 },
            { new Vector2(60.875f, -33.5f), 973 },
            { new Vector2(-36.5f, 24.5f), 974 },
            { new Vector2(-32.5f, 87.0f), 975 },
            { new Vector2(44.0f, 77.5f), 976 },
            { new Vector2(-96.0f, 27.0f), 977 },
            { new Vector2(-95.0f, 27.0f), 978 },
            { new Vector2(-94.0f, 27.0f), 979 },
            { new Vector2(-75.0f, -39.5f), 980 },
            { new Vector2(-74.0f, -39.5f), 981 },
            { new Vector2(-73.0f, -39.5f), 982 },
            { new Vector2(48.0f, -80.5f), 983 },
            { new Vector2(50.0f, -80.5f), 984 },
            { new Vector2(54.625f, 10.125f), 985 },
            { new Vector2(55.0f, 11.125f), 986 },
            { new Vector2(56.0f, 11.625f), 987 },
            { new Vector2(34.0f, 47.5f), 988 },
            { new Vector2(35.0f, 47.5f), 989 },
            { new Vector2(36.0f, 47.5f), 990 },
            { new Vector2(73.0f, -84.5f), 991 },
            { new Vector2(59.75f, -33.5f), 992 },
            { new Vector2(-37.75f, -9.625f), 993 },
            { new Vector2(10.5f, -9.875f), 994 },
            { new Vector2(-68.875f, 30.0f), 995 },
            { new Vector2(-32.875f, 72.125f), 996 },
            { new Vector2(44.0f, 80.5f), 997 },
            { new Vector2(67.0f, 85.375f), 998 },
            { new Vector2(92.0f, 8.0f), 999 },
            { new Vector2(-97.5f, -54.5f), 1000 },
            { new Vector2(-75.5f, -45.5f), 1001 },
            { new Vector2(-92.0f, -50.0f), 1002 },
            { new Vector2(-82.0f, -46.0f), 1003 },
            { new Vector2(38.25f, -46.5f), 1004 },
            { new Vector2(-47.0f, -28.5f), 1005 },
            { new Vector2(-52.25f, -52.25f), 1006 },
            { new Vector2(-52.25f, -51.0f), 1007 },
            { new Vector2(-49.75f, -52.25f), 1008 },
            { new Vector2(-49.75f, -51.0f), 1009 },
            { new Vector2(-52.25f, -49.75f), 1010 },
            { new Vector2(-49.75f, -49.75f), 1011 },
            { new Vector2(42.0f, -46.125f), 1012 },
            { new Vector2(53.0f, -91.25f), 1013 },
            { new Vector2(-101.5f, -46.5f), 1014 },
            { new Vector2(-108.5f, 50.0f), 1015 },
            { new Vector2(39.875f, -46.5f), 1016 },
            { new Vector2(-29.5f, 52.0f), 1017 },
            { new Vector2(-96.0f, -46.0f), 1018 },
            { new Vector2(-66.0f, -103.5f), 1019 },
            { new Vector2(-65.0f, -103.5f), 1020 },
            { new Vector2(-66.0f, -102.5f), 1021 },
            { new Vector2(-65.0f, -102.5f), 1022 },
            { new Vector2(-64.0f, -103.5f), 1023 },
            { new Vector2(-19.0f, 35.0f), 1024 },
            { new Vector2(-23.5f, 30.5f), 1025 },
            { new Vector2(-14.5f, 30.5f), 1026 },
            { new Vector2(-19.0f, 26.0f), 1027 },
            { new Vector2(86.125f, -103.0f), 1276 },
            { new Vector2(86.625f, -102.5f), 1277 },
            { new Vector2(-0.625f, -45.125f), 1278 },
            { new Vector2(-0.125f, -44.625f), 1279 },
            { new Vector2(3.016258f, -5.243076f), 1280 },
            { new Vector2(3.516258f, -4.743076f), 1281 },
            { new Vector2(-43.0f, -46.0f), 1282 },
            { new Vector2(-43.0f, -45.0f), 1282 },
            { new Vector2(41.0f, -47.5f), 1283 },
            { new Vector2(41.0f, -46.5f), 1283 },
            { new Vector2(41.0f, -43.5f), 1284 },
            { new Vector2(41.0f, -42.5f), 1284 },
            { new Vector2(-72.0f, -10.5f), 1285 },
            { new Vector2(-72.0f, -9.5f), 1285 },
            { new Vector2(-90.0f, 15.0f), 1286 },
            { new Vector2(-90.0f, 16.0f), 1286 },
            { new Vector2(55.0f, 0.5f), 1287 },
            { new Vector2(55.0f, 1.5f), 1287 },
            { new Vector2(69.0f, 34.5f), 1288 },
            { new Vector2(69.0f, 35.5f), 1288 },
            { new Vector2(50.0f, 73.0f), 1289 },
            { new Vector2(50.0f, 74.0f), 1289 },
            { new Vector2(-102.0f, 36.5f), 1290 },
            { new Vector2(-102.0f, 37.5f), 1290 },
            { new Vector2(33.0f, -53.0f), 1291 },
            { new Vector2(33.0f, -52.0f), 1291 },
            { new Vector2(39.0f, -45.5f), 1292 },
            { new Vector2(39.0f, -44.5f), 1292 },
            { new Vector2(83.0f, -35.5f), 1293 },
            { new Vector2(83.0f, -34.5f), 1293 },
            { new Vector2(-28.0f, 52.0f), 1294 },
            { new Vector2(-28.0f, 53.0f), 1294 },
            { new Vector2(-41.0f, -85.5f), 1295 },
            { new Vector2(-41.0f, -84.5f), 1295 },
            { new Vector2(-51.0f, -51.0f), 1296 },
            { new Vector2(-51.0f, -50.0f), 1296 },
            { new Vector2(-19.0f, -52.5f), 1297 },
            { new Vector2(-19.0f, -51.5f), 1297 },
            { new Vector2(-88.5f, -30.0f), 1298 },
            { new Vector2(-88.5f, -29.0f), 1298 },
            { new Vector2(-80.0f, -20.5f), 1299 },
            { new Vector2(-80.0f, -19.5f), 1299 },
            { new Vector2(-55.0f, 31.0f), 1300 },
            { new Vector2(-55.0f, 32.0f), 1300 },

            { new Vector2(-51f, -39f), 1560 },
            { new Vector2(74f, -35f), 1561 },
            { new Vector2(-52.5f, -40.5f), 1562 },
            { new Vector2(-52.5f, -37.5f), 1563 },
            { new Vector2(-49.5f, -40.5f), 1564 },
            { new Vector2(-49.5f, -37.5f), 1565 },
            { new Vector2(75.5f, -36.5f), 1566 },
            { new Vector2(72.5f, -36.5f), 1567 },
            { new Vector2(72.5f, -33.5f), 1568 },
            { new Vector2(75.5f, -33.5f), 1569 },
            { new Vector2(-97f, 54f), 1570 },
            { new Vector2(-57f, -49f), 1571 },
            { new Vector2(-44f, -39f), 1572 },
            { new Vector2(-19f, 30.5f), 1573 },
            { new Vector2(-38.5f, -71.75f), 1574 },
            { new Vector2(-40f, -70.5f), 1575 },
            { new Vector2(-88f, -59f), 1576 },
            { new Vector2(-89f, -60f), 1577 },
            { new Vector2(-38.5f, -70.5f), 1578 },
            { new Vector2(-87f, -60f), 1579 },
            { new Vector2(-37f, -70.5f), 1580 },
            { new Vector2(-89f, -58f), 1581 },
            { new Vector2(-40f, -64.5f), 1582 },
            { new Vector2(-87f, -58f), 1583 },
            { new Vector2(-41.5f, -69f), 1584 },
            { new Vector2(-41.5f, -67.5f), 1585 },
            { new Vector2(-41.5f, -66f), 1586 },
            { new Vector2(-35.5f, -69f), 1587 },
            { new Vector2(-61.5f, -103.5f), 1597 },
            { new Vector2(26f, -105.5f), 1598 },
            { new Vector2(55.5f, -90.5f), 1599 },
            { new Vector2(-59.25f, -69.5f), 1600 },
            { new Vector2(-39.5f, -38.5f), 1601 },
            { new Vector2(4.5f, -68.5f), 1602 },
            { new Vector2(20.5f, -57.5f), 1603 },
            { new Vector2(27.5f, -48.5f), 1604 },
            { new Vector2(8.5f, -14.5f), 1605 },
            { new Vector2(13.75f, -25.5f), 1606 },
            { new Vector2(-30f, 44.5f), 1607 },
            { new Vector2(5f, 44.5f), 1608 },
            }},
            {"level_bonus_4.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(4.5f, 8.75f), 1028 },
            { new Vector2(-15.5f, 10.75f), 1029 },
            { new Vector2(-17.5f, -13.25f), 1030 },
            { new Vector2(-16.5f, -13.25f), 1031 },
            { new Vector2(-15.5f, -13.25f), 1032 },
            { new Vector2(-15.5f, -12.25f), 1033 },
            { new Vector2(-17.5f, -12.25f), 1034 },
            { new Vector2(-17.5f, -11.25f), 1035 },
            { new Vector2(-16.5f, -11.25f), 1036 },
            { new Vector2(-15.5f, -11.25f), 1037 },
            { new Vector2(-17.5f, -10.25f), 1038 },
            { new Vector2(-16.5f, -10.25f), 1039 },
            { new Vector2(-15.5f, -10.25f), 1040 },
            { new Vector2(-17.5f, -8.25f), 1041 },
            { new Vector2(-16.5f, -8.25f), 1042 },
            { new Vector2(-15.5f, -8.25f), 1043 },
            { new Vector2(-15.5f, -9.25f), 1044 },
            { new Vector2(-17.5f, -9.25f), 1045 },
            { new Vector2(-14.5f, 8.75f), 1046 },
            { new Vector2(-15.5f, 8.75f), 1047 },
            { new Vector2(-16.5f, 8.75f), 1048 },
            { new Vector2(-17.5f, 8.75f), 1049 },
            { new Vector2(-14.5f, 9.75f), 1050 },
            { new Vector2(-15.5f, 9.75f), 1051 },
            { new Vector2(9.5f, 9.75f), 1052 },
            { new Vector2(9.5f, 8.75f), 1053 },
            { new Vector2(9.5f, 7.75f), 1054 },
            { new Vector2(8.5f, 7.75f), 1055 },
            { new Vector2(7.5f, 7.75f), 1056 },
            { new Vector2(7.5f, 8.75f), 1057 },
            { new Vector2(8.5f, 8.75f), 1058 },
            { new Vector2(7.5f, 9.75f), 1059 },
            { new Vector2(5.5f, 9.75f), 1060 },
            { new Vector2(4.5f, 9.75f), 1061 },
            { new Vector2(3.5f, 9.75f), 1062 },
            { new Vector2(2.5f, 9.75f), 1063 },
            { new Vector2(2.5f, 8.75f), 1064 },
            { new Vector2(3.5f, 8.75f), 1065 },
            { new Vector2(5.5f, 8.75f), 1066 },
            { new Vector2(6.5f, 8.75f), 1067 },
            { new Vector2(6.5f, 7.75f), 1068 },
            { new Vector2(5.5f, 7.75f), 1069 },
            { new Vector2(4.5f, 7.75f), 1070 },
            { new Vector2(3.5f, 7.75f), 1071 },
            { new Vector2(2.5f, 7.75f), 1072 },
            { new Vector2(-14.5f, 10.75f), 1073 },
            { new Vector2(-14.5f, 11.75f), 1074 },
            { new Vector2(-17.5f, 12.75f), 1075 },
            { new Vector2(-16.5f, 12.75f), 1076 },
            { new Vector2(-15.5f, 12.75f), 1077 },
            { new Vector2(-14.5f, 12.75f), 1078 },
            { new Vector2(-15.5f, 11.75f), 1079 },
            { new Vector2(2.5f, 10.75f), 1080 },
            { new Vector2(3.5f, 10.75f), 1081 },
            { new Vector2(4.5f, 10.75f), 1082 },
            { new Vector2(5.5f, 10.75f), 1083 },
            { new Vector2(6.5f, 10.75f), 1084 },
            { new Vector2(7.5f, 10.75f), 1085 },
            { new Vector2(8.5f, 10.75f), 1086 },
            { new Vector2(9.5f, 10.75f), 1087 },
            { new Vector2(21.5f, -19.5f), 1088 },
            { new Vector2(-21.75f, 5.0f), 1089 },
            { new Vector2(8.0f, 4.5f), 1090 },
            { new Vector2(-10.25f, 16.75f), 1091 },
            { new Vector2(8.5f, 9.625f), 1092 },
            { new Vector2(6.5f, 9.75f), 1093 },
            }},
            {"level_12.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-24.5f, -55.0f), 1094 },
            { new Vector2(32.0f, -32.5f), 1095 },
            { new Vector2(-66.5f, -13.25f), 1096 },
            { new Vector2(52.0f, 51.5f), 1097 },
            { new Vector2(-36.5f, -65.0f), 1098 },
            { new Vector2(48.5f, -83.5f), 1099 },
            { new Vector2(-62.5f, 15.0f), 1100 },
            { new Vector2(-20.5f, -2.0f), 1101 },
            { new Vector2(25.0f, 0.5f), 1102 },
            { new Vector2(20.5f, 9.5f), 1103 },
            { new Vector2(14.0f, 24.625f), 1104 },
            { new Vector2(34.5f, -12.5f), 1105 },
            { new Vector2(54.0f, -25.5f), 1106 },
            { new Vector2(73.25f, 11.0f), 1107 },
            { new Vector2(-32.25f, 40.625f), 1108 },
            { new Vector2(-44.5f, 65.5f), 1109 },
            { new Vector2(-64.0f, 85.0f), 1110 },
            { new Vector2(9.0f, 51.0f), 1111 },
            { new Vector2(-18.0f, 83.0f), 1112 },
            { new Vector2(67.5f, -74.0f), 1113 },
            { new Vector2(66.5f, -74.0f), 1114 },
            { new Vector2(56.5f, -59.75f), 1115 },
            { new Vector2(22.0f, 58.0f), 1116 },
            { new Vector2(-61.5f, -24.0f), 1117 },
            { new Vector2(26.0f, 62.0f), 1118 },
            { new Vector2(32.25f, -72.0f), 1119 },
            { new Vector2(49.0f, -48.0f), 1120 },
            { new Vector2(-66.5f, -15.5f), 1121 },
            { new Vector2(-66.5f, -11.0f), 1122 },
            { new Vector2(24.75f, 18.75f), 1123 },
            { new Vector2(-72.0f, 70.5f), 1124 },
            { new Vector2(38.375f, 25.625f), 1125 },
            { new Vector2(-14.0f, -64.0f), 1126 },
            { new Vector2(-18.0f, -64.0f), 1127 },
            { new Vector2(30.0f, -69.125f), 1128 },
            { new Vector2(-14.0f, -7.5f), 1129 },
            { new Vector2(25.0f, 9.5f), 1130 },
            { new Vector2(11.0f, 38.5f), 1131 },
            { new Vector2(-7.5f, -7.5f), 1132 },
            { new Vector2(24.0f, 36.0f), 1133 },
            { new Vector2(45.125f, -27.75f), 1301 },
            { new Vector2(45.625f, -27.25f), 1302 },
            { new Vector2(-59.5f, 67.25f), 1303 },
            { new Vector2(-59.0f, 67.75f), 1304 },
            { new Vector2(-19.0f, -72.0f), 1305 },
            { new Vector2(-19.0f, -71.0f), 1305 },
            { new Vector2(31.0f, -71.0f), 1306 },
            { new Vector2(31.0f, -70.0f), 1306 },
            { new Vector2(-43.0f, -20.5f), 1307 },
            { new Vector2(-43.0f, -19.5f), 1307 },
            { new Vector2(-23.0f, 21.5f), 1308 },
            { new Vector2(-23.0f, 22.5f), 1308 },
            { new Vector2(68.0f, 10.0f), 1309 },
            { new Vector2(68.0f, 11.0f), 1309 },
            { new Vector2(-21.0f, 67.0f), 1310 },
            { new Vector2(-21.0f, 68.0f), 1310 },
            { new Vector2(-12.0f, -64.0f), 1311 },
            { new Vector2(-12.0f, -63.0f), 1311 },
            { new Vector2(-16.0f, -64.0f), 1312 },
            { new Vector2(-16.0f, -63.0f), 1312 },
            { new Vector2(-20.0f, -64.0f), 1313 },
            { new Vector2(-20.0f, -63.0f), 1313 },
            { new Vector2(-8.0f, -64.0f), 1314 },
            { new Vector2(-8.0f, -63.0f), 1314 },
            { new Vector2(-32.0f, 24.0f), 1315 },
            { new Vector2(-32.0f, 25.0f), 1315 },
            { new Vector2(2.5f, -20.5f), 1316 },
            { new Vector2(2.5f, -19.5f), 1316 },
            { new Vector2(23.0f, 9.5f), 1317 },
            { new Vector2(23.0f, 10.5f), 1317 },
            { new Vector2(23.0f, 18.5f), 1318 },
            { new Vector2(23.0f, 19.5f), 1318 },
            { new Vector2(52.0f, -11.0f), 1319 },
            { new Vector2(52.0f, -10.0f), 1319 },
            { new Vector2(-15.5f, 69.5f), 1320 },
            { new Vector2(-15.5f, 70.5f), 1320 },

            { new Vector2(49f, -53f), 1609 },
            { new Vector2(47.5f, -54.5f), 1610 },
            { new Vector2(47.5f, -51.5f), 1611 },
            { new Vector2(50.5f, -54.5f), 1612 },
            { new Vector2(50.5f, -51.5f), 1613 },
            { new Vector2(24f, 28.5f), 1614 },
            { new Vector2(-62f, 85f), 1615 },
            { new Vector2(-66f, 85f), 1616 },
            { new Vector2(20.5f, 18.5f), 1617 },
            { new Vector2(-35f, -20.5f), 1618 },
            { new Vector2(55f, -14f), 1619 },
            { new Vector2(-29.75f, -65.25f), 1620 },
            { new Vector2(54f, -28f), 1621 },
            { new Vector2(72.5f, -84.5f), 1622 },
            { new Vector2(71f, -74.5f), 1623 },
            { new Vector2(-19.375f, -2.5f), 1624 },
            { new Vector2(-7.5f, 24.5f), 1625 },
            { new Vector2(18f, 33.5f), 1626 },
            { new Vector2(-2.5f, 65.5f), 1627 },
            { new Vector2(-65.5f, 65.5f), 1640 },
            }},
            {"level_boss_4.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(1.375f, -17.5f), 1134 },
            { new Vector2(-8.5f, -9.25f), 1135 },
            { new Vector2(-14.0f, -17.25f), 1136 },
            { new Vector2(-1.625f, -25.5f), 1137 },
            { new Vector2(-7.875f, -25.5f), 1138 },
            { new Vector2(-7.5f, -9.5f), 1139 },
            { new Vector2(-14.0f, -20.5f), 1140 },
            { new Vector2(-12.5f, -16.5f), 1141 },
            { new Vector2(2.5f, -16.25f), 1142 },
            { new Vector2(3.5f, -16.75f), 1143 },
            { new Vector2(4.0f, -20.5f), 1144 },
            { new Vector2(-5.375f, -24.0f), 1145 },
            { new Vector2(15.625f, -14.5f), 1146 },
            { new Vector2(18.125f, -14.5f), 1147 },
            { new Vector2(20.625f, -14.5f), 1148 },
            { new Vector2(23.125f, -14.5f), 1149 },
            { new Vector2(23.0f, -17.0f), 1150 },
            { new Vector2(22.875f, -19.25f), 1151 },
            { new Vector2(25.5f, -19.25f), 1152 },
            { new Vector2(28.125f, -19.25f), 1153 },
            { new Vector2(30.625f, -19.25f), 1154 },
            { new Vector2(30.5f, -21.875f), 1155 },
            { new Vector2(30.5f, -24.25f), 1156 },
            { new Vector2(-10.75f, -10.25f), 1157 },
            { new Vector2(-10.375f, -14.375f), 1158 },
            { new Vector2(-11.5f, -15.5f), 1159 },
            { new Vector2(-7.375f, -24.25f), 1160 },
            { new Vector2(-2.75f, -25.125f), 1161 },
            { new Vector2(-2.5f, -22.75f), 1162 },
            { new Vector2(-6.5f, -19.0f), 1163 },
            { new Vector2(1.25f, -7.875f), 1164 },
            { new Vector2(-7.875f, -7.625f), 1165 },
            { new Vector2(-2.25f, -9.75f), 1166 },
            { new Vector2(-11.375f, -14.875f), 1167 },
            { new Vector2(-14.5f, -12.375f), 1168 },
            { new Vector2(-10.625f, -11.125f), 1169 },
            { new Vector2(0.75f, -10.125f), 1170 },
            { new Vector2(1.625f, -11.75f), 1171 },
            { new Vector2(-6.0f, -23.125f), 1172 },
            { new Vector2(-1.25f, -26.25f), 1173 },
            { new Vector2(-9.0f, -24.5f), 1174 },
            { new Vector2(-4.0f, -24.25f), 1175 },
            { new Vector2(-7.5f, -26.25f), 1176 },
            { new Vector2(-5.25f, -15.25f), 1323 },
            { new Vector2(-4.75f, -14.75f), 1324 },
            }},
            {"level_esc_1.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(1.5f, 42.0f), 1177 },
            { new Vector2(3.5f, 42.0f), 1178 },
            { new Vector2(2.5f, 42.0f), 1179 },
            }},
            {"level_esc_2.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-39.5f, -72.5f), 1180 },
            { new Vector2(-76.5f, -57.0f), 1181 },
            { new Vector2(-63.0f, -34.75f), 1182 },
            { new Vector2(-50.75f, -35.75f), 1183 },
            }},
            {"level_esc_3.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-90.5f, -39.5f), 1184 },
            { new Vector2(-90.5f, -38.5f), 1185 },
            { new Vector2(-78.5f, -70.5f), 1186 },
            { new Vector2(-48.75f, -54.5f), 1187 },
            { new Vector2(-58.0f, -2.0f), 1188 },
            { new Vector2(-57.0f, -2.0f), 1189 },
            { new Vector2(-59.0f, -2.0f), 1190 },
            { new Vector2(-53.75f, -35.5f), 1191 },
            { new Vector2(-51.25f, -37.25f), 1192 },
            { new Vector2(-77.0f, -74.0f), 1193 },
            { new Vector2(-36.75f, -45.25f), 1194 },
            { new Vector2(-83.5f, 7.5f), 1195 },
            { new Vector2(-83.5f, 6.5f), 1196 },
            { new Vector2(-49.875f, -54.375f), 1197 },
            { new Vector2(-73.5f, -59.5f), 1198 },
            { new Vector2(-72.0f, 32.125f), 1199 },
            }},
            {"level_esc_4.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-74.375f, -36.5f), 1200 },
            }},
        };
        public static readonly Dictionary<string, Dictionary<Vector2, long>> templePosToLocationId = new Dictionary<string, Dictionary<Vector2, long>>
        {
            {"level_hub.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(41.75f, -26.125f), 0 },
            { new Vector2(-34.25f, 0.25f), 1 },
            { new Vector2(24.0f, -1.5f), 2 }, //Changed from 24, -2
            { new Vector2(-29.0f, 4.0f), 3 },
            { new Vector2(2.75f, 46.0f), 4 },
            { new Vector2(93.25f, -34.875f), 5 },
            { new Vector2(3.0f, -31.5f), 6 },
            { new Vector2(-30.875f, -12.375f), 7 },
            { new Vector2(6.25f, 11.125f), 8 },
            { new Vector2(60.0f, 21.0f), 9 },
            { new Vector2(-40.0f, -5.5f), 622 }, //Bumped down by one, pof switch
            { new Vector2(-41.5f, -8.0f), 623 },
            { new Vector2(-38.5f, -8.0f), 624 },
            { new Vector2(-41.5f, -5.0f), 625 },
            { new Vector2(-38.5f, -5.0f), 626 },
            }},
            {"level_cave_1.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-102.0f, -19.875f), 10 },
            { new Vector2(-22.5f, -27.5f), 11 },
            { new Vector2(20.0f, 14.25f), 12 },
            { new Vector2(-103.5f, -19.875f), 13 },
            { new Vector2(-66.875f, -26.875f), 14 },
            { new Vector2(-16.875f, -12.125f), 15 },
            { new Vector2(32.25f, 5.625f), 16 },
            { new Vector2(-90.75f, 25.625f), 17 },
            { new Vector2(25.25f, -22.5f), 18 },
            { new Vector2(-134.0f, -9.75f), 19 },
            { new Vector2(-93.5f, 25.5f), 20 },
            { new Vector2(-73.5f, 57.375f), 21 },
            { new Vector2(-134.5f, -11.875f), 22 },
            { new Vector2(-90.75f, 22.5f), 23 },
            { new Vector2(30.5f, -2.75f), 24 },
            { new Vector2(23.0f, 9.25f), 25 },
            { new Vector2(-36.5f, -55.0f), 26 },
            { new Vector2(-65.5f, -35.0f), 27 },
            { new Vector2(-23.5f, 21.0f), 28 },
            { new Vector2(-3.0f, 38.0f), 29 },
            { new Vector2(-2.0f, 38.0f), 30 },
            { new Vector2(-1.0f, 38.0f), 31 },
            { new Vector2(-4.0f, 38.0f), 32 },
            { new Vector2(-3.0f, 37.0f), 33 },
            { new Vector2(-2.0f, 37.0f), 34 },
            { new Vector2(-1.0f, 37.0f), 35 },
            { new Vector2(-4.0f, 37.0f), 36 },
            { new Vector2(-4.0f, 39.0f), 37 },
            { new Vector2(-3.0f, 39.0f), 38 },
            { new Vector2(-2.0f, 39.0f), 39 },
            { new Vector2(-1.0f, 39.0f), 40 },
            { new Vector2(-93.25f, 22.75f), 41 },
            { new Vector2(-24.0f, -16.5f), 42 },
            { new Vector2(-74.375f, 27.375f), 43 },
            { new Vector2(-36.5f, -38.5f), 44 },
            { new Vector2(-41.0f, -43.0f), 45 },
            { new Vector2(-32.0f, -43.0f), 46 },
            { new Vector2(-36.5f, -47.5f), 47 },
            { new Vector2(-65.36057f, -24.98942f), 515 },
            { new Vector2(-64.86057f, -24.48942f), 516 },
            { new Vector2(-2.0f, 29.5f), 517 },
            { new Vector2(-2.0f, 30.5f), 517 },
            { new Vector2(-37.0f, -28.0f), 518 },
            { new Vector2(-37.0f, -27.0f), 518 },
            { new Vector2(-45.5f, -12.5f), 519 },
            { new Vector2(-45.5f, -11.5f), 519 },
            { new Vector2(1.375f, -29.25f), 520 },
            { new Vector2(1.375f, -28.25f), 520 },
            { new Vector2(1.0f, -21.875f), 521 },
            { new Vector2(1.0f, -20.875f), 521 },
            { new Vector2(7.375f, -27.0f), 522 },
            { new Vector2(7.375f, -26.0f), 522 },
            { new Vector2(24.375f, -18.25f), 523 },
            { new Vector2(24.375f, -17.25f), 523 },
            { new Vector2(7.875f, 17.625f), 524 },
            { new Vector2(7.875f, 18.625f), 524 },
            { new Vector2(31.0f, 9.0f), 525 },
            { new Vector2(31.0f, 10.0f), 525 },
            { new Vector2(-88.5f, 23.75f), 627 },
            { new Vector2(1.0f, 4.5f), 628 },
            { new Vector2(-36.5f, -43f), 629 },
            { new Vector2(-99.5f, -9.5f), 630 }, //Bumped down by one, pof switch
            { new Vector2(-101.0f, -12.0f), 631 },
            { new Vector2(-98.0f, -12.0f), 632 },
            { new Vector2(-101.0f, -9.0f), 633 },
            { new Vector2(-98.0f, -9.0f), 634 },
            }},
            {"level_cave_2.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-33.125f, 44.875f), 48 },
            { new Vector2(2.0f, -42.0f), 49 },
            { new Vector2(-16.0f, -15.25f), 50 },
            { new Vector2(-15.25f, -15.25f), 51 },
            { new Vector2(3.125f, 16.375f), 52 },
            { new Vector2(3.875f, 16.375f), 53 },
            { new Vector2(47.5f, 1.5f), 54 },
            { new Vector2(46.75f, 1.5f), 55 },
            { new Vector2(-39.25f, -34.5f), 56 },
            { new Vector2(-38.5f, -34.5f), 57 },
            { new Vector2(-9.625f, -40.5f), 58 },
            { new Vector2(10.375f, -55.75f), 59 },
            { new Vector2(25.125f, 13.5f), 60 },
            { new Vector2(-39.625f, -27.375f), 61 },
            { new Vector2(10.25f, -16.375f), 62 },
            { new Vector2(29.125f, 15.75f), 63 },
            { new Vector2(-38.5f, -13.25f), 64 },
            { new Vector2(-38.5f, -14.25f), 65 },
            { new Vector2(-38.5f, -15.25f), 66 },
            { new Vector2(25.75f, -2.5f), 67 },
            { new Vector2(26.5f, -2.5f), 68 },
            { new Vector2(46.5f, 12.75f), 69 },
            { new Vector2(46.5f, 13.5f), 70 },
            { new Vector2(-14.0f, -56.0f), 71 },
            { new Vector2(13.0f, -14.0f), 72 },
            { new Vector2(22.0f, 20.875f), 73 },
            { new Vector2(8.0f, -52.5f), 74 },
            { new Vector2(48.5f, -31.5f), 75 },
            { new Vector2(-34.5f, 43.5f), 76 },
            { new Vector2(-26.0f, -37.0f), 77 },
            { new Vector2(-51.0f, -29.0f), 78 },
            { new Vector2(-36.0f, -11.0f), 79 },
            { new Vector2(8.0f, -14.0f), 80 },
            { new Vector2(-17.5f, -9.5f), 81 },
            { new Vector2(22.5f, -45.0f), 82 },
            { new Vector2(-27.5f, 0.0f), 83 },
            { new Vector2(1.5f, -11.0f), 84 },
            { new Vector2(-3.25f, -56.125f), 85 },
            { new Vector2(-51.25f, -27.25f), 86 },
            { new Vector2(-34.0f, 21.5f), 87 },
            { new Vector2(9.5f, 15.75f), 88 },
            { new Vector2(-32.625f, 43.625f), 89 },
            { new Vector2(-1.5f, -52.0f), 90 },
            { new Vector2(-17.5f, -47.5f), 91 },
            { new Vector2(13.5f, -16.0f), 92 },
            { new Vector2(-31.875f, 44.875f), 93 },
            { new Vector2(-48.0f, 24.5f), 94 },
            { new Vector2(-52.5f, 20.0f), 95 },
            { new Vector2(-43.5f, 20.0f), 96 },
            { new Vector2(-48.0f, 15.5f), 97 },
            { new Vector2(-45.67786f, -13.36271f), 526 },
            { new Vector2(-45.17786f, -12.86271f), 527 },
            { new Vector2(8.288351f, 32.72399f), 528 },
            { new Vector2(8.788351f, 33.22399f), 529 },
            { new Vector2(14.75f, -0.5f), 530 },
            { new Vector2(15.25f, 0.0f), 531 },
            { new Vector2(12.875f, -35.0f), 532 },
            { new Vector2(12.875f, -34.0f), 532 },
            { new Vector2(-13.0f, 14.5f), 533 },
            { new Vector2(-13.0f, 15.5f), 533 },
            { new Vector2(40.5f, -2.5f), 534 },
            { new Vector2(40.5f, -1.5f), 534 },
            { new Vector2(-42.375f, -31.375f), 535 },
            { new Vector2(-42.375f, -30.375f), 535 },
            { new Vector2(-24.125f, -33.625f), 536 },
            { new Vector2(-24.125f, -32.625f), 536 },
            { new Vector2(9.125f, -37.25f), 537 },
            { new Vector2(9.125f, -36.25f), 537 },
            { new Vector2(-7.5f, -48.5f), 538 },
            { new Vector2(-7.5f, -47.5f), 538 },
            { new Vector2(18.25f, -32.125f), 539 },
            { new Vector2(18.25f, -31.125f), 539 },
            { new Vector2(15.75f, -38.25f), 540 },
            { new Vector2(15.75f, -37.25f), 540 },
            { new Vector2(-35.625f, -29.375f), 541 },
            { new Vector2(-35.625f, -28.375f), 541 },
            { new Vector2(-36.25f, 5.0f), 542 },
            { new Vector2(-36.25f, 6.0f), 542 },
            { new Vector2(-43.75f, 2.25f), 543 },
            { new Vector2(-43.75f, 3.25f), 543 },
            { new Vector2(-38.0f, 24.0f), 544 },
            { new Vector2(-38.0f, 25.0f), 544 },
            { new Vector2(-10.75f, -29.875f), 545 },
            { new Vector2(-10.75f, -28.875f), 545 },
            { new Vector2(-18.5f, -13.625f), 546 },
            { new Vector2(-18.5f, -12.625f), 546 },
            { new Vector2(21.375f, -26.5f), 547 },
            { new Vector2(21.375f, -25.5f), 547 },
            { new Vector2(15.75f, -21.75f), 548 },
            { new Vector2(15.75f, -20.75f), 548 },
            { new Vector2(10.25f, -24.625f), 549 },
            { new Vector2(10.25f, -23.625f), 549 },
            { new Vector2(-23.25f, 2.75f), 550 },
            { new Vector2(-23.25f, 3.75f), 550 },
            { new Vector2(4.375f, 5.375f), 551 },
            { new Vector2(4.375f, 6.375f), 551 },
            { new Vector2(21.5f, -2.375f), 552 },
            { new Vector2(21.5f, -1.375f), 552 },
            { new Vector2(3.5f, 29.5f), 553 },
            { new Vector2(3.5f, 30.5f), 553 },
            { new Vector2(45.75f, 23.75f), 554 },
            { new Vector2(45.75f, 24.75f), 554 },
            { new Vector2(52.5f, 23.75f), 555 },
            { new Vector2(52.5f, 24.75f), 555 },
            { new Vector2(21.75f, 37.0f), 556 },
            { new Vector2(21.75f, 38.0f), 556 },
            { new Vector2(41.125f, 34.125f), 557 },
            { new Vector2(41.125f, 35.125f), 557 },
            { new Vector2(48.0f, -44.0f), 635 },
            { new Vector2(-34.5f, 5.5f), 636 },
            { new Vector2(9.5f, 14.0f), 637 },
            { new Vector2(20.5f, 36.5f), 638 },
            { new Vector2(-2.0f, -41.0f), 639 },
            { new Vector2(-48f, 20f), 640 },
            { new Vector2(22.5f, 14.5f), 641 }, //Bumped down by one, pof switch
            { new Vector2(21.0f, 12.0f), 642 },
            { new Vector2(24.0f, 12.0f), 643 },
            { new Vector2(21.0f, 15.0f), 644 },
            { new Vector2(24.0f, 15.0f), 645 },
            }},
            {"level_cave_3.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(58.5f, -54.5f), 98 },
            { new Vector2(90.0f, -9.0f), 99 },
            { new Vector2(-18.5f, -30.5f), 100 },
            { new Vector2(-19.25f, -30.5f), 101 },
            { new Vector2(-72.0f, 2.5f), 102 },
            { new Vector2(-71.0f, 2.5f), 103 },
            { new Vector2(-5.0f, 47.5f), 104 },
            { new Vector2(-6.0f, 47.5f), 105 },
            { new Vector2(101.5f, -44.5f), 106 },
            { new Vector2(101.5f, -45.25f), 107 },
            { new Vector2(92.25f, -1.25f), 108 },
            { new Vector2(21.5f, 29.5f), 109 },
            { new Vector2(-12.625f, 31.125f), 110 },
            { new Vector2(0.0f, 37.5f), 111 },
            { new Vector2(24.5f, 35.5f), 112 },
            { new Vector2(36.5f, 31.5f), 113 },
            { new Vector2(1.0f, -30.125f), 114 },
            { new Vector2(-49.125f, 22.0f), 115 },
            { new Vector2(79.0f, -4.5f), 116 },
            { new Vector2(14.5f, 12.5f), 117 },
            { new Vector2(11.5f, -29.125f), 118 },
            { new Vector2(58.5f, -51.5f), 119 },
            { new Vector2(-32.5f, -22.5f), 120 },
            { new Vector2(-32.5f, -23.5f), 121 },
            { new Vector2(-32.5f, -24.5f), 122 },
            { new Vector2(-32.5f, -25.5f), 123 },
            { new Vector2(45.25f, 19.75f), 124 },
            { new Vector2(45.25f, 20.5f), 125 },
            { new Vector2(-25.5f, 42.5f), 126 },
            { new Vector2(-24.5f, 42.5f), 127 },
            { new Vector2(104.75f, -36.25f), 128 },
            { new Vector2(105.5f, -36.25f), 129 },
            { new Vector2(106.25f, -36.25f), 130 },
            { new Vector2(-64.5f, 21.5f), 131 },
            { new Vector2(75.5f, -4.5f), 132 },
            { new Vector2(-4.5f, 25.625f), 133 },
            { new Vector2(-1.0f, 10.0f), 134 },
            { new Vector2(40.5f, -9.5f), 135 },
            { new Vector2(53.0f, -86.0f), 136 },
            { new Vector2(27.0f, 24.0f), 137 },
            { new Vector2(89.0f, -9.0f), 138 },
            { new Vector2(-0.25f, 33.5f), 139 },
            { new Vector2(91.0f, -9.0f), 140 },
            { new Vector2(51.0f, -39.0f), 141 },
            { new Vector2(-67.5f, 21.5f), 142 },
            { new Vector2(-3.0f, -20.0f), 143 },
            { new Vector2(-27.5f, -56.0f), 144 },
            { new Vector2(53.5f, -95.0f), 145 },
            { new Vector2(85.5f, -50.0f), 146 },
            { new Vector2(-83.5f, 0.0f), 147 },
            { new Vector2(-3.5f, -27.0f), 148 },
            { new Vector2(49.5f, -11.0f), 149 },
            { new Vector2(90.25f, -1.125f), 150 },
            { new Vector2(91.375f, 0.875f), 151 },
            { new Vector2(12.5f, -29.25f), 152 },
            { new Vector2(90.0f, -10.25f), 153 },
            { new Vector2(58.5f, -53.0f), 154 },
            { new Vector2(-9.75f, -31.25f), 155 },
            { new Vector2(55.0f, -85.5f), 156 },
            { new Vector2(4.375f, -26.375f), 157 },
            { new Vector2(5.75f, 28.0f), 158 },
            { new Vector2(14.5f, 11.0f), 159 },
            { new Vector2(77.5f, -3.25f), 160 },
            { new Vector2(-16.625f, 38.125f), 161 },
            { new Vector2(57.5f, -86.5f), 162 },
            { new Vector2(52.0f, -88.5f), 163 },
            { new Vector2(53.5f, -88.5f), 164 },
            { new Vector2(5.0f, -30.0f), 165 },
            { new Vector2(-32.5f, -41.5f), 166 },
            { new Vector2(-37.0f, -46.0f), 167 },
            { new Vector2(-28.0f, -46.0f), 168 },
            { new Vector2(-32.5f, -50.5f), 169 },
            { new Vector2(84.0f, 15.5f), 170 },
            { new Vector2(79.5f, 11.0f), 171 },
            { new Vector2(88.5f, 11.0f), 172 },
            { new Vector2(84.0f, 6.5f), 173 },
            { new Vector2(52.0f, -36.75f), 558 },
            { new Vector2(52.5f, -36.25f), 559 },
            { new Vector2(-22.0f, 43.875f), 560 },
            { new Vector2(-21.5f, 44.375f), 561 },
            { new Vector2(-32.5f, -32.375f), 562 },
            { new Vector2(-32.0f, -31.875f), 563 },
            { new Vector2(-8.125f, 3.875f), 564 },
            { new Vector2(-8.125f, 4.875f), 564 },
            { new Vector2(48.0f, -29.5f), 565 },
            { new Vector2(48.0f, -28.5f), 565 },
            { new Vector2(-47.0f, 46.0f), 566 },
            { new Vector2(-47.0f, 47.0f), 566 },
            { new Vector2(93.0f, -71.0f), 567 },
            { new Vector2(93.0f, -70.0f), 567 },
            { new Vector2(85.5f, -75.5f), 568 },
            { new Vector2(85.5f, -74.5f), 568 },
            { new Vector2(74.5f, -71.5f), 569 },
            { new Vector2(74.5f, -70.5f), 569 },
            { new Vector2(63.5f, -41.0f), 570 },
            { new Vector2(63.5f, -40.0f), 570 },
            { new Vector2(81.5f, -47.5f), 571 },
            { new Vector2(81.5f, -46.5f), 571 },
            { new Vector2(-64.0f, -15.75f), 572 },
            { new Vector2(-64.0f, -14.75f), 572 },
            { new Vector2(-61.625f, 3.625f), 573 },
            { new Vector2(-61.625f, 4.625f), 573 },
            { new Vector2(-63.75f, 8.375f), 574 },
            { new Vector2(-63.75f, 9.375f), 574 },
            { new Vector2(-68.75f, 2.75f), 575 },
            { new Vector2(-68.75f, 3.75f), 575 },
            { new Vector2(65.5f, -28.5f), 576 },
            { new Vector2(65.5f, -27.5f), 576 },
            { new Vector2(86.0f, -22.0f), 577 },
            { new Vector2(86.0f, -21.0f), 577 },
            { new Vector2(-33.5f, 42.375f), 578 },
            { new Vector2(-33.5f, 43.375f), 578 },
            { new Vector2(-27.875f, 47.625f), 579 },
            { new Vector2(-27.875f, 48.625f), 579 },
            { new Vector2(97.5f, -70.5f), 580 },
            { new Vector2(97.5f, -69.5f), 580 },
            { new Vector2(105.5f, -63.5f), 581 },
            { new Vector2(105.5f, -62.5f), 581 },
            { new Vector2(-12.0f, -38.5f), 646 },
            { new Vector2(-82.0f, 11.5f), 646 }, //The West blue switch, it has the same location id
            { new Vector2(14.5f, -28.5f), 648 },
            { new Vector2(38.5f, 28.5f), 649 },
            { new Vector2(92.5f, -22.5f), 648 },
            { new Vector2(-32.5f, -46f), 651 },
            { new Vector2(84f, 11f), 652 },
            { new Vector2(14.5f, 3.0f), 653 }, //Bumped down by one, pof switch
            { new Vector2(13.0f, 0.5f), 654 },
            { new Vector2(16.0f, 0.5f), 655 },
            { new Vector2(13.0f, 3.5f), 656 },
            { new Vector2(16.0f, 3.5f), 657 },
            { new Vector2(56.25f, -86.5f), 658 },
            }},
            {"level_boss_1.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-29.0f, 0.0f), 174 },
            { new Vector2(-17.25f, 26.25f), 175 },
            { new Vector2(-16.5f, 26.25f), 176 },
            //{ new Vector2(-17.5f, 5.5f), 177 },
            //{ new Vector2(7.0f, -6.125f), 178 },
            { new Vector2(-19.0f, 26.25f), 179 },
            { new Vector2(-35.25f, -4.0f), 180 },
            { new Vector2(-26.5f, -35.0f), 181 },
            { new Vector2(-10.25f, -20.25f), 582 },
            { new Vector2(-9.75f, -19.75f), 583 },
            { new Vector2(-8.25f, -20.25f), 584 },
            { new Vector2(-7.75f, -19.75f), 585 },
            { new Vector2(-6.25f, -20.25f), 586 },
            { new Vector2(-5.75f, -19.75f), 587 },
            { new Vector2(-4.25f, -20.25f), 588 },
            { new Vector2(-3.75f, -19.75f), 589 },
            { new Vector2(7, -6.125f), 590 },
            { new Vector2(-35.0f, -1.5f), 659 },
            }},
            {"level_passage.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(9.75f, 96.125f), 182 },
            { new Vector2(154.75f, 232.875f), 183 },
            { new Vector2(1.5f, 105.0f), 184 },
            { new Vector2(1.5f, 105.75f), 185 },
            { new Vector2(-10.0f, -93.5f), 186 },
            { new Vector2(12.5f, -112.0f), 187 },
            { new Vector2(12.5f, -110.5f), 188 },
            { new Vector2(12.5f, -109.0f), 189 },
            { new Vector2(3.5f, 74.5f), 190 },
            { new Vector2(7.0f, 201.25f), 191 },
            { new Vector2(157.5f, 232.0f), 192 },
            { new Vector2(-116.5f, 50.0f), 193 },
            { new Vector2(-12.5f, -12.0f), 194 },
            { new Vector2(0.5f, -1.0f), 195 },
            { new Vector2(5.5f, -10.0f), 196 },
            { new Vector2(10.5f, -11.0f), 197 },
            { new Vector2(110.5f, -15.0f), 198 },
            { new Vector2(10.5f, 193.0f), 199 },
            { new Vector2(20.5f, -208.5f), 200 },
            { new Vector2(19.5f, -208.5f), 201 },
            { new Vector2(0.25f, 187.0f), 202 },
            { new Vector2(-3.0f, -11.5f), 203 },
            { new Vector2(-7.5f, -16.0f), 204 },
            { new Vector2(1.5f, -16.0f), 205 },
            { new Vector2(-3.0f, -20.5f), 206 },
            { new Vector2(3.0f, 76.0f), 591 },
            { new Vector2(3.0f, 77.0f), 591 },
            { new Vector2(7.0f, 79.0f), 592 },
            { new Vector2(7.0f, 80.0f), 592 },
            { new Vector2(1.0f, 90.0f), 593 },
            { new Vector2(1.0f, 91.0f), 593 },
            { new Vector2(-1.0f, 101.0f), 594 },
            { new Vector2(-1.0f, 102.0f), 594 },
            { new Vector2(-0.5f, 107.5f), 595 },
            { new Vector2(-0.5f, 108.5f), 595 },
            { new Vector2(10.5f, 91.5f), 596 },
            { new Vector2(10.5f, 92.5f), 596 },
            { new Vector2(-3f, -16f), 660 },
            }},
            {"level_temple_entrance.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-0.25f, -9.25f), 207 },
            { new Vector2(0.25f, -9.75f), 208 },
            }},
            {"level_temple_1.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-12.0f, 11.25f), 209 },
            { new Vector2(-55.75f, 71.5f), 210 },
            { new Vector2(37.75f, -30.0f), 211 },
            { new Vector2(-37.0f, -44.5f), 212 },
            { new Vector2(29.5f, -34.25f), 213 },
            { new Vector2(49.125f, -49.125f), 214 },
            { new Vector2(48.375f, -49.375f), 215 },
            { new Vector2(49.5f, -48.5f), 216 },
            { new Vector2(30.25f, -34.25f), 217 },
            { new Vector2(78.75f, -32.75f), 218 },
            { new Vector2(77.25f, -32.75f), 219 },
            { new Vector2(-67.0f, -22.0f), 220 },
            { new Vector2(-66.0f, -22.0f), 221 },
            { new Vector2(32.5f, 24.5f), 222 },
            { new Vector2(31.5f, 24.5f), 223 },
            { new Vector2(-64.0f, 34.5f), 224 },
            { new Vector2(-64.0f, 35.5f), 225 },
            { new Vector2(81.5f, -0.5f), 226 },
            { new Vector2(-33.0f, -42.0f), 227 },
            { new Vector2(4.0f, -33.0f), 228 },
            { new Vector2(43.5f, -3.0f), 229 },
            { new Vector2(-53.0f, 47.25f), 230 },
            { new Vector2(-54.5f, 71.5f), 231 },
            { new Vector2(-33.0f, -47.5f), 232 },
            { new Vector2(1.5f, 53.25f), 233 },
            { new Vector2(3.0f, 53.25f), 234 },
            { new Vector2(4.5f, 53.25f), 235 },
            { new Vector2(-3.125f, -33.0f), 236 },
            { new Vector2(39.0f, -54.0f), 237 },
            { new Vector2(42.625f, -1.625f), 238 },
            { new Vector2(-55.75f, 46.0f), 239 },
            { new Vector2(-31.375f, 46.25f), 240 },
            { new Vector2(-29.0f, 43.125f), 241 },
            { new Vector2(63.5f, -10.0f), 242 },
            { new Vector2(87.0f, 8.5f), 243 },
            { new Vector2(90.0f, -13.0f), 244 },
            { new Vector2(-42.0f, 10.0f), 245 },
            { new Vector2(77.625f, -12.375f), 246 },
            { new Vector2(62.0f, -30.25f), 247 },
            { new Vector2(-68.5f, -12.5f), 248 },
            { new Vector2(-26.0f, 26.75f), 249 },
            { new Vector2(20.5f, 22.5f), 250 },
            { new Vector2(109.0f, -14.0f), 251 },
            { new Vector2(56.0f, -35.0f), 252 },
            { new Vector2(-57.25f, 71.25f), 253 },
            { new Vector2(24.75f, 33.0f), 254 },
            { new Vector2(23.0f, 32.75f), 255 },
            { new Vector2(63.75f, -30.25f), 256 },
            { new Vector2(-68.5f, -22.0f), 257 },
            { new Vector2(63.5f, -12.0f), 258 },
            { new Vector2(-41.5f, 40.5f), 259 },
            { new Vector2(23.0f, -54.0f), 260 },
            { new Vector2(45.0f, -39.0f), 261 },
            { new Vector2(-1.0f, -11.0f), 262 },
            { new Vector2(26.5f, -18.0f), 263 },
            { new Vector2(15.75f, 28.25f), 264 },
            { new Vector2(29.0f, 24.0f), 265 },
            { new Vector2(-52.5f, 52.0f), 266 },
            { new Vector2(-9.0f, 53.0f), 267 },
            { new Vector2(56.0f, -31.25f), 268 },
            { new Vector2(17.75f, 28.0f), 269 },
            { new Vector2(19.375f, -50.25f), 270 },
            { new Vector2(4.5f, -30.5f), 271 },
            { new Vector2(78.0f, -32.625f), 272 },
            { new Vector2(-28.0f, 9.25f), 273 },
            { new Vector2(45.0f, -8.5f), 274 },
            { new Vector2(35.0f, 15.5f), 275 },
            { new Vector2(-55.75f, 70.25f), 276 },
            { new Vector2(0.0f, -31.5f), 277 },
            { new Vector2(1.0f, -31.5f), 278 },
            { new Vector2(35.375f, -54.0f), 279 },
            { new Vector2(37.25f, -36.25f), 280 },
            { new Vector2(30.0f, -35.0f), 281 },
            { new Vector2(69.5f, -34.25f), 282 },
            { new Vector2(70.5f, -34.25f), 283 },
            { new Vector2(-31.75f, -15.125f), 284 },
            { new Vector2(-26.0f, 15.0f), 285 },
            { new Vector2(-24.5f, 15.0f), 286 },
            { new Vector2(35.0f, -2.5f), 287 },
            { new Vector2(-6.5f, 52.5f), 288 },
            { new Vector2(-5.0f, 52.5f), 289 },
            { new Vector2(36.0f, -35.25f), 290 },
            { new Vector2(-67.0f, 21.5f), 291 },
            { new Vector2(-71.5f, 17.0f), 292 },
            { new Vector2(-62.5f, 17.0f), 293 },
            { new Vector2(-67.0f, 12.5f), 294 },
            { new Vector2(101.0f, -29.5f), 295 },
            { new Vector2(96.5f, -34.0f), 296 },
            { new Vector2(105.5f, -34.0f), 297 },
            { new Vector2(101.0f, -38.5f), 298 },
            { new Vector2(87.375f, -0.625f), 597 },
            { new Vector2(87.875f, -0.125f), 598 },
            { new Vector2(-33.0f, -34.5f), 599 },
            { new Vector2(-33.0f, -33.5f), 599 },
            { new Vector2(24.0f, -18.0f), 600 },
            { new Vector2(24.0f, -17.0f), 600 },
            { new Vector2(-67f, 17f), 661 },
            { new Vector2(101, -34), 662 },
            { new Vector2(65.5f, -23.5f), 663 }, //Bumped down by one, pof switch
            { new Vector2(64.0f, -26.0f), 664 },
            { new Vector2(67.0f, -26.0f), 665 },
            { new Vector2(64.0f, -23.0f), 666 },
            { new Vector2(67.0f, -23.0f), 667 },
            { new Vector2(-3.5f, -33.5f), 668 },
            { new Vector2(36.5f, -36.5f), 669 },
            { new Vector2(83.5f, -38.5f), 670 },
            { new Vector2(-10.375f, 25.5f), 671 },
            { new Vector2(25.0f, 32.5f), 672 },
            { new Vector2(106.5f, -15.5f), 673 },
            }},
            {"level_boss_2.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-10.0f, -14.5f), 299 },
            { new Vector2(15.0f, 7.5f), 300 },
            }},
            {"level_boss_2_special.xml", new Dictionary<Vector2, long>
            {
                { new Vector2(-10.0f, -14f), 299 },
                { new Vector2(15.0f, 7.5f), 300 },
            }},
            {"level_temple_2.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-80.0f, -55.0f), 301 },
            { new Vector2(14.0f, -37.0f), 302 },
            { new Vector2(-7.0f, 16.0f), 303 },
            { new Vector2(-29.0f, 34.125f), 304 },
            { new Vector2(57.5f, 54.0f), 305 },
            { new Vector2(-48.0f, -16.75f), 306 },
            { new Vector2(-36.5f, -47.5f), 307 },
            { new Vector2(-35.5f, -47.5f), 308 },
            { new Vector2(-34.5f, -47.5f), 309 },
            { new Vector2(-41.5f, -1.5f), 310 },
            { new Vector2(-40.75f, -1.5f), 311 },
            { new Vector2(-40.0f, -1.5f), 312 },
            { new Vector2(54.5f, 29.5f), 313 },
            { new Vector2(55.25f, 29.5f), 314 },
            { new Vector2(-6.375f, 43.375f), 315 },
            { new Vector2(-5.5f, 42.0f), 316 },
            { new Vector2(-5.625f, 42.875f), 317 },
            { new Vector2(38.0f, 53.0f), 318 },
            { new Vector2(36.75f, 52.125f), 319 },
            { new Vector2(-81.5f, -56.0f), 320 },
            { new Vector2(-80.5f, -56.0f), 321 },
            { new Vector2(-39.5f, -33.25f), 322 },
            { new Vector2(9.5f, -37.0f), 323 },
            { new Vector2(10.5f, -37.0f), 324 },
            { new Vector2(-28.0f, -76.5f), 325 },
            { new Vector2(-42.0f, -22.5f), 326 },
            { new Vector2(-55.75f, 71.25f), 327 },
            { new Vector2(-3.0f, 53.0f), 328 },
            { new Vector2(-49.125f, 43.75f), 329 },
            { new Vector2(-37.5f, -33.375f), 330 },
            { new Vector2(-48.0f, -45.25f), 331 },
            { new Vector2(14.0f, -31.0f), 332 },
            { new Vector2(-71.0f, 4.75f), 333 },
            { new Vector2(-23.75f, -0.625f), 334 },
            { new Vector2(-0.5f, 9.875f), 335 },
            { new Vector2(44.0f, -3.25f), 336 },
            { new Vector2(38.5f, 52.0f), 337 },
            { new Vector2(-73.0f, 3.0f), 338 },
            { new Vector2(26.5f, -3.0f), 339 },
            { new Vector2(-7.0f, 13.75f), 340 },
            { new Vector2(-79.375f, 60.625f), 341 },
            { new Vector2(-40.5f, -33.25f), 342 },
            { new Vector2(-50.375f, 45.25f), 343 },
            { new Vector2(-65.0f, -17.0f), 344 },
            { new Vector2(3.0f, 34.0f), 345 },
            { new Vector2(-81.0f, -55.0f), 346 },
            { new Vector2(-38.75f, -62.0f), 347 },
            { new Vector2(10.875f, -35.75f), 348 },
            { new Vector2(-76.0f, 10.0f), 349 },
            { new Vector2(-0.5f, 18.5f), 350 },
            { new Vector2(-48.0f, 37.0f), 351 },
            { new Vector2(48.5f, 54.0f), 352 },
            { new Vector2(-19.25f, 6.75f), 353 },
            { new Vector2(-40.25f, -62.5f), 354 },
            { new Vector2(17.0f, -31.5f), 355 },
            { new Vector2(-50.125f, 43.5f), 356 },
            { new Vector2(2.0f, 76.0f), 357 },
            { new Vector2(-36.5f, -51.25f), 358 },
            { new Vector2(-79.25f, 18.5f), 359 },
            { new Vector2(-24.5f, 22.5f), 360 },
            { new Vector2(47.0f, -1.0f), 361 },
            { new Vector2(-28.5f, 52.75f), 362 },
            { new Vector2(11.0f, 61.75f), 363 },
            { new Vector2(61.75f, 46.5f), 364 },
            { new Vector2(-79.5f, 3.5f), 365 },
            { new Vector2(-53.0f, -2.25f), 366 },
            { new Vector2(-40.75f, -0.75f), 367 },
            { new Vector2(-9.0f, -3.0f), 368 },
            { new Vector2(-15.0f, 17.0f), 369 },
            { new Vector2(30.5f, 20.25f), 370 },
            { new Vector2(3.0f, 57.75f), 371 },
            { new Vector2(31.25f, 39.25f), 372 },
            { new Vector2(84.25f, 61.75f), 373 },
            { new Vector2(-52.0f, -24.25f), 374 },
            { new Vector2(-50.375f, 44.25f), 375 },
            { new Vector2(-82.25f, -54.875f), 376 },
            { new Vector2(10.5f, -34.0f), 377 },
            { new Vector2(-24.5f, 21.0f), 378 },
            { new Vector2(-30.0f, 53.0f), 379 },
            { new Vector2(3.0f, 76.0f), 380 },
            { new Vector2(-79.5f, 57.75f), 381 },
            { new Vector2(-53.75f, 70.125f), 382 },
            { new Vector2(-7.5f, -44.0f), 383 },
            { new Vector2(-12.0f, -48.5f), 384 },
            { new Vector2(-3.0f, -48.5f), 385 },
            { new Vector2(-7.5f, -53.0f), 386 },
            { new Vector2(-80.5f, -24.5f), 387 },
            { new Vector2(-85.0f, -29.0f), 388 },
            { new Vector2(-76.0f, -29.0f), 389 },
            { new Vector2(-80.5f, -33.5f), 390 },
            { new Vector2(34.5f, -10.5f), 391 },
            { new Vector2(30.0f, -15.0f), 392 },
            { new Vector2(39.0f, -15.0f), 393 },
            { new Vector2(34.5f, -19.5f), 394 },
            { new Vector2(-79.5f, 52.0f), 395 },
            { new Vector2(-84.0f, 47.5f), 396 },
            { new Vector2(-75.0f, 47.5f), 397 },
            { new Vector2(-79.5f, 43.0f), 398 },
            { new Vector2(-53.14848f, -13.22477f), 601 },
            { new Vector2(-52.64848f, -12.72477f), 602 },
            { new Vector2(33.99756f, 10.8798f), 603 },
            { new Vector2(34.49756f, 11.3798f), 604 },
            { new Vector2(61.0f, -6.0f), 605 },
            { new Vector2(61.0f, -5.0f), 605 },
            { new Vector2(-48.0f, -43.0f), 606 },
            { new Vector2(-48.0f, -42.0f), 606 },
            { new Vector2(-28.0f, -32.0f), 607 },
            { new Vector2(-28.0f, -31.0f), 607 },
            { new Vector2(-44.5f, 15.0f), 608 },
            { new Vector2(-44.5f, 16.0f), 608 },
            { new Vector2(-71.0f, -11.0f), 609 },
            { new Vector2(-71.0f, -10.0f), 609 },
            { new Vector2(7.0f, 15.0f), 610 },
            { new Vector2(7.0f, 16.0f), 610 },
            { new Vector2(-37.0f, 53.0f), 611 },
            { new Vector2(-37.0f, 54.0f), 611 },
            { new Vector2(-41.5f, 40.5f), 674 },
            { new Vector2(-7.5f, -48.5f), 675 },
            { new Vector2(-80.5f, -29), 676 },
            { new Vector2(34.5f, -15), 677 },
            { new Vector2(-79.5f, 47.5f), 678 },
            { new Vector2(-17.5f, 55.5f), 679 }, //Bumped down by one, pof switch
            { new Vector2(-19.0f, 53.0f), 680 },
            { new Vector2(-16.0f, 53.0f), 681 },
            { new Vector2(-19.0f, 56.0f), 682 },
            { new Vector2(-16.0f, 56.0f), 683 },
            { new Vector2(2.5f, -22.5f), 684 },
            { new Vector2(43.0f, 42.0f), 685 },
            { new Vector2(-70.5f, 65.5f), 686 },
            { new Vector2(-48.0f, -2.5f), 687 },
            { new Vector2(70.0f, 0.0f), 688 },
            { new Vector2(-47.5f, -73.5f), 689 },
            { new Vector2(-42.5f, -33.5f), 690 },
            { new Vector2(-26.0f, -51.5f), 691 },
            { new Vector2(11.5f, -31.5f), 692 },
            { new Vector2(21.5f, -31.5f), 693 },
            { new Vector2(-58.0f, -24.5f), 694 },
            { new Vector2(-66.5f, -14.5f), 695 },
            { new Vector2(-5.5f, -29.5f), 696 },
            { new Vector2(-21.5f, 5.5f), 697 },
            { new Vector2(-8.5f, 14.5f), 698 },
            { new Vector2(64.0f, 29.5f), 699 },
            { new Vector2(-46.0f, 43.5f), 700 },
            { new Vector2(-31.0f, 36.5f), 701 },
            { new Vector2(-39.5f, 41.5f), 702 },
            { new Vector2(1.5f, 36.5f), 703 },
            { new Vector2(-17.5f, 52.5f), 704 },
            { new Vector2(-12.5f, 58.5f), 705 },
            { new Vector2(-79.5f, 8.5f), 722 },
            { new Vector2(8f, 19f), 723 },
            { new Vector2(-17, 7), 724 },
            }},
            {"level_temple_3.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(-19.75f, 4.25f), 399 },
            { new Vector2(31.5f, -18.5f), 400 },
            { new Vector2(32.5f, -18.5f), 401 },
            { new Vector2(62.0f, -35.5f), 402 },
            { new Vector2(9.5f, -70.5f), 403 },
            { new Vector2(19.0f, -31.0f), 404 },
            { new Vector2(75.25f, -41.0f), 405 },
            { new Vector2(34.0f, -18.0f), 406 },
            { new Vector2(6.25f, 32.0f), 407 },
            { new Vector2(58.0f, -34.5f), 408 },
            { new Vector2(13.25f, -11.0f), 409 },
            { new Vector2(-19.75f, 5.75f), 410 },
            { new Vector2(13.0f, 7.0f), 411 },
            { new Vector2(-15.5f, 1.5f), 412 },
            { new Vector2(19.0f, -71.0f), 413 },
            { new Vector2(37.0f, -34.0f), 414 },
            { new Vector2(62.0f, -33.5f), 415 },
            { new Vector2(-14.5f, 3.5f), 416 },
            { new Vector2(8.5f, -9.25f), 417 },
            { new Vector2(11.5f, 3.5f), 418 },
            { new Vector2(-18.0f, 22.0f), 419 },
            { new Vector2(-10.25f, -2.5f), 420 },
            { new Vector2(-7.25f, -2.5f), 421 },
            { new Vector2(-4.25f, -2.5f), 422 },
            { new Vector2(-1.25f, -2.5f), 423 },
            { new Vector2(1.75f, -2.5f), 424 },
            { new Vector2(8.5f, -11.5f), 425 },
            { new Vector2(8.5f, 3.5f), 426 },
            { new Vector2(7.0f, 29.5f), 427 },
            { new Vector2(-12.5f, -27.5f), 428 },
            { new Vector2(-17.0f, -32.0f), 429 },
            { new Vector2(-8.0f, -32.0f), 430 },
            { new Vector2(-12.5f, -36.5f), 431 },
            { new Vector2(75.0f, -42.0f), 612 },
            { new Vector2(75.0f, -41.0f), 612 },
            { new Vector2(-34.0f, -7.0f), 613 },
            { new Vector2(-34.0f, -6.0f), 613 },
            { new Vector2(9.5f, -67.5f), 614 },
            { new Vector2(9.5f, -66.5f), 614 },
            { new Vector2(-3.5f, -11.0f), 615 },
            { new Vector2(-3.5f, -10.0f), 615 },
            { new Vector2(-3.5f, 6.0f), 616 },
            { new Vector2(-3.5f, 7.0f), 616 },
            { new Vector2(47.5f, -43.5f), 617 },
            { new Vector2(47.5f, -42.5f), 617 },
            { new Vector2(4.0f, 32.0f), 618 },
            { new Vector2(4.0f, 33.0f), 618 },
            { new Vector2(1.0f, -10.0f), 706 },
            { new Vector2(-8.0f, -10.0f), 707 },
            { new Vector2(-8.0f, 7.0f), 708 },
            { new Vector2(1.0f, 7.0f), 709 },
            { new Vector2(-12.5f, -32), 710 },
            { new Vector2(13.25f, -11.5f), 711 },
            { new Vector2(-20.375f, 1.5f), 712 },
            { new Vector2(-3.5f, -3.5f), 713 },
            { new Vector2(13.25f, 3.5f), 714 },
            { new Vector2(-14.5f, 16.5f), 715 },
            { new Vector2(-3.5f, -16), 725 },
            }},
            {"level_bonus_5.xml", new Dictionary<Vector2, long>
            {
            { new Vector2(7.0f, -86.0f), 432 },
            { new Vector2(6.0f, -86.0f), 433 },
            { new Vector2(6.0f, -87.0f), 434 },
            { new Vector2(7.0f, -87.0f), 435 },
            { new Vector2(8.0f, -82.0f), 436 },
            { new Vector2(7.0f, -82.0f), 437 },
            { new Vector2(8.0f, -81.0f), 438 },
            { new Vector2(7.0f, -81.0f), 439 },
            { new Vector2(6.0f, -82.0f), 440 },
            { new Vector2(6.0f, -81.0f), 441 },
            { new Vector2(12.5f, -89.0f), 442 },
            { new Vector2(12.5f, -90.0f), 443 },
            { new Vector2(11.5f, -89.0f), 444 },
            { new Vector2(11.5f, -90.0f), 445 },
            { new Vector2(-10.5f, -70.0f), 446 },
            { new Vector2(-10.5f, -69.0f), 447 },
            { new Vector2(-9.5f, -69.0f), 448 },
            { new Vector2(-9.5f, -70.0f), 449 },
            { new Vector2(-2.5f, -16.0f), 450 },
            { new Vector2(-1.5f, -16.0f), 451 },
            { new Vector2(-0.5f, -16.0f), 452 },
            { new Vector2(-2.5f, -15.0f), 453 },
            { new Vector2(-0.5f, -15.0f), 454 },
            { new Vector2(-2.5f, -14.0f), 455 },
            { new Vector2(-1.5f, -14.0f), 456 },
            { new Vector2(-0.5f, -14.0f), 457 },
            { new Vector2(-6.5f, -12.25f), 458 },
            { new Vector2(-7.5f, -12.25f), 459 },
            { new Vector2(-7.5f, -11.25f), 460 },
            { new Vector2(-6.5f, -11.25f), 461 },
            { new Vector2(17.5f, -14.25f), 462 },
            { new Vector2(16.5f, -14.25f), 463 },
            { new Vector2(17.5f, -13.25f), 464 },
            { new Vector2(16.5f, -13.25f), 465 },
            { new Vector2(9.5f, 6.75f), 466 },
            { new Vector2(5.5f, 6.75f), 467 },
            { new Vector2(8.5f, 6.75f), 468 },
            { new Vector2(6.5f, 6.75f), 469 },
            { new Vector2(7.5f, 6.75f), 470 },
            { new Vector2(1.5f, 6.75f), 471 },
            { new Vector2(4.5f, 6.75f), 472 },
            { new Vector2(2.5f, 6.75f), 473 },
            { new Vector2(3.5f, 6.75f), 474 },
            { new Vector2(0.5f, 6.75f), 475 },
            { new Vector2(12.5f, 6.75f), 476 },
            { new Vector2(10.5f, 6.75f), 477 },
            { new Vector2(11.5f, 6.75f), 478 },
            { new Vector2(14.5f, -9.0f), 479 },
            { new Vector2(13.5f, -9.0f), 480 },
            { new Vector2(13.5f, -8.0f), 481 },
            { new Vector2(14.5f, -8.0f), 482 },
            { new Vector2(22.0f, 7.75f), 483 },
            { new Vector2(21.0f, 7.75f), 484 },
            { new Vector2(21.0f, 8.75f), 485 },
            { new Vector2(22.0f, 8.75f), 486 },
            { new Vector2(12.5f, -9.0f), 487 },
            { new Vector2(12.5f, -8.0f), 488 },
            { new Vector2(-15.0f, 14.0f), 489 },
            { new Vector2(-16.0f, 13.0f), 490 },
            { new Vector2(-16.0f, 14.0f), 491 },
            { new Vector2(-17.0f, 14.0f), 492 },
            { new Vector2(-16.0f, 15.0f), 493 },
            { new Vector2(-17.0f, 15.0f), 494 },
            { new Vector2(-16.0f, 16.0f), 495 },
            { new Vector2(-17.0f, 16.0f), 496 },
            { new Vector2(-15.0f, 16.0f), 497 },
            { new Vector2(-16.0f, 17.0f), 498 },
            { new Vector2(-15.0f, 15.0f), 499 },
            { new Vector2(-3.0f, 87.0f), 500 },
            { new Vector2(-11.0f, 94.0f), 501 },
            { new Vector2(-1.5f, 93.75f), 502 },
            { new Vector2(-1.5f, 92.75f), 503 },
            { new Vector2(-3.0f, 100.0f), 504 },
            { new Vector2(4.0f, 94.0f), 505 },
            { new Vector2(-1.5f, 94.75f), 506 },
            { new Vector2(-1.5f, -15.25f), 507 },
            { new Vector2(21.5f, 9.5f), 508 },
            { new Vector2(13.25f, -13.75f), 509 },
            { new Vector2(-8.75f, 93.5f), 510 },
            { new Vector2(-3.0f, -86.5f), 511 },
            { new Vector2(-7.5f, -91.0f), 512 },
            { new Vector2(1.5f, -91.0f), 513 },
            { new Vector2(-3.0f, -95.5f), 514 },
            { new Vector2(-3f, -91f), 716 },
            { new Vector2(6.5f, 11.5f), 717 },
            { new Vector2(-1.5f, -10.5f), 718 },
            { new Vector2(4.5f, -91f), 719 },
            { new Vector2(-8.5f, -91f), 720 },
            { new Vector2(-0.5f, 93.5f), 721 },
            { new Vector2(-79.5f, 8.5f), 722 },
            }},
            {"level_boss_3.xml", new Dictionary<Vector2, long>{
            { new Vector2(-0.5f, -10.75f), 619 },
            { new Vector2(-0.5f, -9.75f), 619 },
            { new Vector2(8.0f, 1.0f), 620 },
            { new Vector2(8.0f, 2.0f), 620 },
            { new Vector2(-9.0f, 1.0f), 621 },
            { new Vector2(-9.0f, 2.0f), 621 },
            }},
        };

        public static readonly Dictionary<string, Dictionary<Vector2, (string, Vector2)>> castleLinkedPositions = new Dictionary<string, Dictionary<Vector2, (string, Vector2)>>
        {
            { "level_2.xml", new Dictionary<Vector2, (string, Vector2)>(){{new Vector2(-29.0f, -23.75f), (null, new Vector2(-34.0f, -23.5f))}}},
            { "level_4.xml", new Dictionary<Vector2, (string, Vector2)>(){{new Vector2(10f, -54f), (null, new Vector2(10f, -50f))}}},
            { "level_5.xml", new Dictionary<Vector2, (string, Vector2)>()
            {
                {new Vector2(-58f, 62f), (null, new Vector2(-54f, 60f))},
                {new Vector2(51f, 65.5f), (null, new Vector2(47f, 65.5f))},
                {new Vector2(-19f, -49.875f), (null, new Vector2(-24.5f, -50f))},
                {new Vector2(64f, -28.875f), (null, new Vector2(71.5f, -28.5f))},
                {new Vector2(53f, -60.75f), (null, new Vector2(58f, -60.5f))},
                {new Vector2(-39f, -35.875f), (null, new Vector2(-38f, -40f))},
                {new Vector2(-58f, 0.25f), (null, new Vector2(-56f, -4f))},
                {new Vector2(-29f, -5.75f), (null, new Vector2(-32.5f, 4f))},

                {new Vector2(-21f, -17.875f), ("level_6.xml", new Vector2(-21f, -28f))},
                {new Vector2(-21f, -28f), ("level_6.xml", new Vector2(-21f, -28f))},
            }},
            { "level_6.xml", new Dictionary<Vector2, (string, Vector2)>()
            {
                {new Vector2(-22f, 13.75f), (null, new Vector2(-26f, 12.5f))},
                {new Vector2(-2f, -26.5f), (null, new Vector2(2f, -26.5f))},

                {new Vector2(-39f, -35.875f), ("level_5.xml", new Vector2(-38f, -40f))},
                {new Vector2(-29f, -6f), ("level_5.xml", new Vector2(-32.5f, 4f))},
                {new Vector2(-58f, 0.25f), ("level_5.xml", new Vector2(-56f, -4f))},
            }},
            { "level_9.xml", new Dictionary<Vector2, (string, Vector2)>()
            {
                {new Vector2(62f, -31f), (null, new Vector2(49f, -26f))},
                {new Vector2(57f, -21f), (null, new Vector2(47f, -21f))},
                {new Vector2(-79.5f, -42.25f), (null, new Vector2(-72.5f, -42.5f))},
            }},
            { "level_11.xml", new Dictionary<Vector2, (string, Vector2)>()
            {
                {new Vector2(-101.5f, -46.5f), (null, new Vector2(-96f, -46f))},
                {new Vector2(-97.5f, -54.5f), (null, new Vector2(-92f, -50f))},
                {new Vector2(-75.5f, -45.5f), (null, new Vector2(-82f, -46f))},
                {new Vector2(-79.5f, -56.75f), (null, new Vector2(-86f, -50f))},
            }},
            { "level_12.xml", new Dictionary<Vector2, (string, Vector2)>()
            {
                {new Vector2(-14f, -7.5f), (null, new Vector2(-7.5f, -7.5f))},
                {new Vector2(11f, 38.5f), (null, new Vector2(24f, 36f))},
            }},
        };
        public static readonly Dictionary<string, Dictionary<Vector2, (string, Vector2)>> templeLinkedPositions = new Dictionary<string, Dictionary<Vector2, (string, Vector2)>>
        {
            { "level_boss_2_special.xml", new Dictionary<Vector2, (string, Vector2)>()
            {
                {new Vector2(-10f, -14f), ("level_boss_2.xml", new Vector2(-10f, -14.5f))},
                {new Vector2(15f, 7.5f), ("level_boss_2.xml", new Vector2(15f, 7.5f))},
            }},
        };

        public static readonly Dictionary<string, Dictionary<string, int>> castleButtonEventToLocationId = new Dictionary<string, Dictionary<string, int>>()
        {
            { "level_2.xml", new Dictionary<string, int>{
            { "p2_rune_sequence", 1358 },
            }},
            { "level_3.xml", new Dictionary<string, int>{
            { "p3_rune_bonus", 1636 },
            { "p3_seq_bonus", 1641 },
            }},
            //{ "level_bonus_1.xml", new Dictionary<string, int>{
            //{ "n1_panel_n", 1628 },
            //{ "n1_panel_e_1", 1629 },
            //{ "n1_panel_e_2", 1630 },
            //{ "n1_panel_s", 1631 },
            //}},
            //{ "level_bonus_2.xml", new Dictionary<string, int>{
            //{ "n2_panel_n", 1632 },
            //{ "n2_panel_se", 1633 },
            //{ "n2_panel_ne", 1634 },
            //}},
            { "level_6.xml", new Dictionary<string, int>{
            { "a3_seq_knife", 1642 },
            { "a3_seq_knife_2", 1643 },
            }},
            { "level_9.xml", new Dictionary<string, int>{
            { "r3_seq_simon_room", 1644 },
            }},
            { "level_11.xml", new Dictionary<string, int>{
            { "c2_rune_bonus", 1574 },
            { "c2_seq_bonus", 1645 },
            }},
            { "level_12.xml", new Dictionary<string, int>{
            { "c3_rune", 1615 },
            }},
        };
        public static readonly Dictionary<string, Dictionary<string, int>> templeButtonEventToLocationId = new Dictionary<string, Dictionary<string, int>>()
        {
            { "level_temple_2.xml", new Dictionary<string, int>{
            { "t2_runes", 723 },
            { "t2_portal", 724 },
            }},
            { "level_temple_3.xml", new Dictionary<string, int>{
            { "t3_levers", 725 },
            }},
        };
        private static Dictionary<string, int> puzzleCodeToItemId = new Dictionary<string, int>()
        {
            {"Prison Return Puzzle 1", 8}, //"PrF1 Puzzle"
            {"Prison 2 Puzzle 1", 9}, //"PrF2 Puzzle"
            {"Armory 4 Puzzle 1", 10}, //"AmF4 Puzzle"
            {"Armory 5 Puzzle 1", 11}, //"AmF5 Puzzle"
            {"Archives 7 Puzzle 1", 12}, //"AvF7 Puzzle"
            {"Archives 8 Puzzle 1", 13}, //"AvF8 Puzzle"
            {"Chambers 11 Puzzle 1", 144}, //"ChF11 Puzzle"

            {"Cave 3 Puzzle 1", 1}, //"CL3 Puzzle"
            {"Cave 2 Puzzle 1", 2}, //"CL2 Puzzle"
            {"Cave 1 Puzzle 1", 4}, //"CL1 West Puzzle"
            {"Cave 1 Puzzle 2", 3}, //"CL1 East Puzzle"
            {"Passage Puzzle 1", 5}, //"SP Puzzle"
            {"Temple 1 Puzzle 1", 6}, //"TF1 West Puzzle"
            {"Temple 1 Puzzle 2", 7}, //"TF1 East Puzzle"
            {"Temple 2 Puzzle 1", 10}, //"TF2 North Puzzle"
            {"Temple 2 Puzzle 2", 8}, //"TF2 West Puzzle"
            {"Temple 2 Puzzle 3", 9}, //"TF2 East Puzzle"
            {"Temple 2 Puzzle 4", 11}, //"TF2 South Puzzle"
            {"Temple 3 Puzzle 1", 12}, //"TF3 Puzzle"
            {"PoF Puzzle 1", 13}, //"PoF Puzzle"
        };
        public static string GetPuzzleItemNameFromPuzzleCode(string code, ArchipelagoData archipelagoData)
        {
            if (!puzzleCodeToItemId.TryGetValue(code, out int relItemId))
            {
                ResourceContext.Log("Could not find puzzle item for code: " + code);
                return null;
            }
            int baseId = 0;
            switch (archipelagoData.mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    baseId = castleButtonItemStartID;
                    break;
                case ArchipelagoData.MapType.Temple:
                    baseId = templeButtonItemStartID;
                    break;
                default:
                    ResourceContext.Log("Unknown campaign: " + archipelagoData.mapType);
                    return null;
            }
            return ArchipelagoManager.GetItemName(baseId + relItemId);
        }

        public static string GetButtonEventName(string buttonItemName)
        {
            int spaceIndex = buttonItemName.IndexOf(' ');
            string firstWord = buttonItemName.Substring(0, spaceIndex);
            string buttonItemEventName;
            switch (firstWord)
            {
                case "Elevate":
                case "Activate":
                case "Enable":
                case "Disable":
                    buttonItemEventName = $"{firstWord}d";
                    break;
                case "Fix":
                case "Open":
                case "Teleport":
                    buttonItemEventName = $"{firstWord}ed";
                    break;
                default:
                    if (buttonItemName.Contains("Boss Rune"))
                    {
                        return $"Activated {buttonItemName}!";
                    }
                    throw new ArgumentOutOfRangeException();
            }
            buttonItemEventName += $"{buttonItemName.Substring(spaceIndex)}!";
            return buttonItemEventName;
        }

        //Generation data
        public const int ENEMY_TIERS = 4;
        public static readonly Dictionary<string, int> enemyTiers = new Dictionary<string, int>()
        {
            { "actors/bonus/archer_1.xml", 2 },
            { "actors/bonus/skeleton_1.xml", 1 },

            { "actors/boss_knight/archer_1.xml", 1 },
            { "actors/boss_knight/knight_guard.xml", 3 },
            { "actors/boss_knight/knight_guard_lich_1.xml", 3 },
            { "actors/boss_knight/knight_guard_lich_2.xml", 3 },
            { "actors/boss_knight/knight_guard_lich_3.xml", 3 },
            { "actors/boss_knight/skeleton_1_small.xml", 1 },
            { "actors/boss_knight/skeleton_1.xml", 2 },

            { "actors/archer_1.xml", 2 },
            { "actors/archer_2.xml", 2 },
            { "actors/archer_3.xml", 2 },
            { "actors/archer_1_elite.xml", 3 },
            { "actors/bat_1.xml", 1 },
            { "actors/bat_2.xml", 2 },
            { "actors/bat_3.xml", 3 },
            { "actors/eye_1_small.xml", 1 },
            { "actors/eye_1.xml", 2 },
            { "actors/guard_desert_1.xml", 2 },
            { "actors/lich_1.xml", 3 },
            { "actors/lich_1_elite.xml", 3 },
            { "actors/lich_2.xml", 3 },
            { "actors/lich_3.xml", 3 },
            { "actors/lich_desert_1.xml", 3 },
            { "actors/lich_desert_2.xml", 3 },
            { "actors/lich_desert_3.xml", 3 },
            { "actors/maggot_1_small.xml", 1 },
            { "actors/maggot_1.xml", 2 },
            { "actors/maggot_1_elite.xml", 3 },
            { "actors/mummy_1_small.xml", 1 },
            { "actors/mummy_1.xml", 2 },
            { "actors/mummy_1_elite.xml", 3 },
            { "actors/mummy_ranged_1.xml", 2 },
            { "actors/mummy_ranged_2.xml", 3 },
            { "actors/skeleton_1_small.xml", 1 },
            { "actors/skeleton_1.xml", 2 },
            { "actors/skeleton_1_elite.xml", 3 },
            { "actors/skeleton_2_small.xml", 1 },
            { "actors/skeleton_2.xml", 2 },
            { "actors/skeleton_2_elite.xml", 3 },
            { "actors/skeleton_3.xml", 2 },
            { "actors/slime_1_host.xml", 1 },
            { "actors/special_beheaded_kamikaze.xml", 3 },
            { "actors/spider.xml", 3 },
            { "actors/tick_1_small.xml", 1 },
            { "actors/tick_1.xml", 2 },
            { "actors/tick_1_elite.xml", 3 },
            { "actors/tick_2_small.xml", 1 },
            { "actors/tick_2.xml", 2 },
            { "actors/wisp_1_small.xml", 1 },
            { "actors/wisp_1.xml", 2 },
            { "actors/wisp_2.xml", 3 },

        };

        public static string[] excludedActors =
        {
            "actors/npc_guard_desert_1.xml",
            "actors/tower_battlement_archer_1.xml",
            "actors/tower_battlement_archer_2.xml",
            "actors/tower_battlement_archer_3.xml",
            "actors/tower_battlement_empty.xml",
            "actors/tower_static_frost.xml",
            "actors/tower_static_frost_grounded.xml",
            "actors/spawners/doomspawn_1_razed.xml",
        };

        public static int GetEnemyTier(string enemyName, bool enemyShuffleKeepTier)
        {
            if (!enemyTiers.TryGetValue(enemyName, out int enemyTier) || !enemyShuffleKeepTier)
            {
                enemyTier = 0;
            }
            return enemyTier;
        }

        public static HashSet<string> treasureNames = new HashSet<string>()
        {
            "items/valuable_1.xml",
            "items/valuable_2.xml",
            "items/valuable_3.xml",
            "items/valuable_4.xml",
            "items/valuable_5.xml",
            "items/valuable_6.xml",
            "items/valuable_7.xml",
            "items/valuable_8.xml",
            "items/valuable_9.xml",
            "items/breakable_barrel.xml",
            "items/breakable_barrel_v2.xml",
            "items/breakable_barrel_b.xml",
            "items/breakable_barrel_b_v2.xml",
            "items/breakable_crate.xml",
            "items/breakable_crate_v2.xml",
            "items/breakable_crate_b.xml",
            "items/breakable_vase.xml",
            "items/breakable_vase_v2.xml",
            "items/breakable_vase_v3.xml",
            "items/breakable_vase_v4.xml",
            "items/vgt_plant.xml",
        };

        public static Dictionary<string, string> spawnerDoodads = new Dictionary<string, string>
        {
            { "actors/spawners/archer_1.xml", "doodads/special/marker_grave.xml" },
            { "actors/spawners/archer_2.xml", "doodads/special/marker_grave.xml" },

            { "actors/spawners/skeleton_1.xml", "doodads/special/marker_grave.xml" },
            { "actors/spawners/skeleton_2.xml", "doodads/special/marker_grave.xml" },

            { "actors/spawners/maggot_1.xml", "doodads/special/marker_maggot_1.xml" },
        };
        public static Dictionary<string, Dictionary<string, string>> spawnerDoodadPositionCorrections = new Dictionary<string, Dictionary<string, string>>()
        {
            { "level_12.xml", new Dictionary<string, string>{
                { "25 -82.375", "25 -82.5" },
            }
            }
        };
        public static Dictionary<string, Dictionary<string, string>> floorSignExits = new Dictionary<string, Dictionary<string, string>>()
        {
            {"level_1.xml", new Dictionary<string, string>{
            { "-47.5 -41.5", "esc_3|1"}, //Not an entrance yet
            { "34.5 -52.5", "3|10"},
            { "49.5 -45.5", "2|0"},
            { "-37.5 -2", "3|1"},
            { "11.5 -29.5", "2|2"},
            { "16.5 -7.5", "2|1"},
            }},
            {"level_2.xml", new Dictionary<string, string>{
            { "-16.5 -33.5", "3|0"},
            { "49.5 -44.5", "1|1"},
            { "8.5 -27.5", "1|3"},
            { "16.5 -6.5", "1|2"},
            }},
            {"level_3.xml", new Dictionary<string, string>{
            { "-12.5 -33.5", "2|3"},
            { "-38.5 -1.5", "1|4"},
            { "7.5 39.5", "boss_1|0"},
            { "14.5 58.5", "1|10"},
            }},
            {"level_bonus_1.xml", new Dictionary<string, string>{
            }},
            {"level_boss_1.xml", new Dictionary<string, string>{
            { "-1.5 -56.5", "4|0"},
            { "-22.5 19.5", "3|100"},
            }},
            {"level_4.xml", new Dictionary<string, string>{
            { "-94.5 -47.5", "esc_2|1"}, //Not an entrance
            { "-81.75 -9.5", "esc_4|0"}, //Not an entrance
            { "-62.5 24.5", "6|0"},
            { "-50.5 17.5", "5|1"},
            { "-30.5 29.5", "5|0"},
            { "-38.5 17.5", "boss_2|0"},
            { "-44.5 50.5", "boss_1|1"},
            }},
            {"level_5.xml", new Dictionary<string, string>{
            { "-44.5 -67.5", "6|2"},
            { "-53.5 17.5", "4|1"},
            { "-30.5 29.5", "4|5"},
            { "-12.5 8.5", "6|1"},
            }},
            {"level_bonus_2.xml", new Dictionary<string, string>{
            }},
            {"level_6.xml", new Dictionary<string, string>{
            { "-47.5 -67.5", "5|3"},
            { "-62.5 24.5", "4|6"},
            { "-15.5 9.5", "5|2"},
            }},
            {"level_boss_2.xml", new Dictionary<string, string>{
            { "2.5 -58.5", "7|0"},
            { "-14.25 6.5", "4|100"},
            }},
            {"level_7.xml", new Dictionary<string, string>{
            { "-42.25 -74.5", "esc_1|1"}, //Not an entrance
            { "-79.25 -49.5", "esc_2|0"}, //Not an entrance
            { "0.75 11.5", "boss_2|1"},
            { "-0.5 36.5", "8|0"},
            { "20.5 34.5", "8|1"},
            }},
            {"level_8.xml", new Dictionary<string, string>{
            { "-17.5 0.5", "9|0"},
            { "2.25 0.5", "7|1"},
            { "22.5 -1.5", "7|2"},
            { "90.5 -57.5", "9|250"},
            }},
            {"level_9.xml", new Dictionary<string, string>{
            { "-36 -45.5", "boss_3|0"},
            { "-10.5 9.5", "8|2"},
            { "90.75 -57.5", "8|200"},
            }},
            {"level_bonus_3.xml", new Dictionary<string, string>{
            }},
            {"level_boss_3.xml", new Dictionary<string, string>{
            { "2.5 -39.5", "10|0"},
            { "-3.25 20.5", "9|100"},
            }},
            {"level_10.xml", new Dictionary<string, string>{
            { "74 -13.5", "boss_3|1"},
            { "-45.625 33.5", "10b|0"},
            { "5.75 35.5", "11|0"},
            { "27.75 42.5", "boss_4|1"},
            { "53.75 50.5", "esc_3|0"}, //The doodad name is a 4 >:|
            }},
            {"level_10_special.xml", new Dictionary<string, string>{
            }},
            {"level_11.xml", new Dictionary<string, string>{
            { "65 -105.5", "12|0"},
            { "52.5 -105.5", "12|54"},
            { "17.75 34.5", "10|100"},
            }},
            {"level_bonus_4.xml", new Dictionary<string, string>{
            }},
            {"level_12.xml", new Dictionary<string, string>{
            { "43 -84.5", "11|105"},
            { "55 -84.5", "11|45"},
            }},
            {"level_boss_4.xml", new Dictionary<string, string>{
            { "31.5 -27.5", "esc_1|0"},
            }},
            {"level_esc_1.xml", new Dictionary<string, string>{
            { "5.75 35.5", "11|0"},
            { "27.75 42.5", "boss_4|1"},
            { "53.75 50.5", "esc_3|0"}, //The doodad name is a 4 >:|
            }},
            {"level_esc_2.xml", new Dictionary<string, string>{
            { "-42.25 -74.5", "esc_1|1"}, //Not an entrance
            { "-79.25 -49.5", "esc_2|0"}, //Not an entrance
            }},
            {"level_esc_3.xml", new Dictionary<string, string>{
            { "-94.5 -47.5", "esc_2|1"}, //Not an entrance
            { "-81.75 -9.5", "esc_4|0"}, //Not an entrance
            { "-62.5 24.5", "6|0"},
            { "-50.5 17.5", "5|1"},
            { "-38.5 17.5", "boss_2|0"},
            }},
            {"level_esc_4.xml", new Dictionary<string, string>{
            { "-47.5 -41.5", "esc_3|1"}, //Not an entrance yet
            { "-37.5 -2", "3|1"},
            }},
        };

        //Shop data
        public static string GetShopItemDescription(NetworkItem item)
        {
            long itemId = item.Item;
            string playerString = "";
            string descriptionString = playerString;
            if (ArchipelagoManager.connectionInfo.IsPlayerPlayingSameGame(item.Player))
            {
                descriptionString += GetHammerwatchItemDescription((int)itemId);
            }
            else
            {
                descriptionString += "Does something ";
                if ((item.Flags & ItemFlags.Advancement) == ItemFlags.Advancement)
                {
                    descriptionString += "that will help the receiving player progress in their world";
                    if ((item.Flags & ItemFlags.Trap) == ItemFlags.Trap)
                    {
                        descriptionString += ", but at a cost";
                    }
                }
                else if ((item.Flags & ItemFlags.NeverExclude) == ItemFlags.NeverExclude)
                {
                    descriptionString += "that may be useful for the receiving player";
                }
                else if ((item.Flags & ItemFlags.Trap) == ItemFlags.Trap)
                {
                    descriptionString += "that will harm the receiving player";
                }
                else
                {
                    descriptionString += "for the receiving player that isn't particularly notable";
                }
            }
            return descriptionString;
        }
        public static string GetHammerwatchItemDescription(int itemId)
        {
            int relId = itemId - templeStartID;
            if (itemDescriptions.TryGetValue(relId, out string desc))
                return desc;
            if (relId >= 545 && relId < 592) //Floor master keys
            {
                string[] splits = ArchipelagoManager.GetItemName(itemId).Split(' ');
                string location = splits[0];
                if (location == "Dune")
                    location = "Dune Sharks arena";
                else if (location == "Pyramid")
                    location = "Pyramid of Fear";
                else
                    location = $"{splits[0]} {splits[1]} {splits[2]} floors";
                string keyType = splits[splits.Length - 2].ToLower();
                if (keyType == "bonus")
                    keyType = "bonus level";
                desc = $"Opens all {keyType} gates in the {location}";
                return desc;
            }
            if (relId >= 768 && relId < 1090) //Button effects
            {
                return "Activates a specific button effect";
            }
            if (IsItemShopUpgrade(itemId)) //Shop upgrades
            {
                int shopRelId = itemId - shopItemBaseId;
                return ArchipelagoMessageManager.GetLanguageString(shopItemDescriptions[shopRelId]);
            }
            return $"Unknown item description: {itemId}";
        }

        private static readonly Dictionary<int, string> itemDescriptions = new Dictionary<int, string>()
        {
            { 0, "Worth 20 gold" }, //Bonus chest
            { 1, "Opens a bonus level gate" }, //Bonus key
            { 4, "Contains a vendor coin, an extra life, and a random stronger upgrade" }, //Purple chest
            { 2, "Contains some treasure, an extra life, or a random upgrade" }, //Blue chest
            { 3, "Contains some treasure, an extra life, or a random upgrade" }, //Green chest
            { 5, "Contains some treasure, an extra life, or a random upgrade" }, //Red chest
            { 6, "Contains some treasure, an extra life, or a random upgrade" }, //Wood chest
            { 7, "Provides a 0.5% discount at shops" }, //Vendor coin
            { 8, "A strange plank?" }, //Strange plank
            { 9, "Opens a bronze gate" }, //Bronze key
            { 10, "Opens a silver gate" }, //Silver key
            { 11, "Opens a gold gate" }, //Gold key
            { 12, "Used to direct sunlight to the solar nodes by placing on the mirror stands" }, //Mirror
            { 13, "Used to upgrade a shop by one level" }, //Ore
            { 14, "Activates a teleporter to outside the temple by placing in a keystone" }, //Rune Key
            { 15, "Lets you respawn after death" }, //1-Up Ankh
            { 16, "Lets you respawn after death five times" }, //5-Up Ankh
            { 17, "Lets you respawn after death seven times" }, //7-Up Ankh
            { 18, "Temporarily increases damage when used" }, //Damage Potion
            { 19, "Fills up health and mana when used" }, //Rejuvenation Potion
            { 20, "Temporarily protects from damage" }, //Invulnerability Potion
            { 21, "Worth 250 gold" }, //Diamond
            { 22, "Worth 500 gold" }, //Red Diamond
            { 23, "Worth 50 gold" }, //Small Diamond
            { 24, "Worth 100 gold" }, //Small Red Diamond
            { 25, "" }, //Random stat upgrade
            { 26, "Increases damage by 5%" }, //Damage upgrade
            { 27, "Increases armor by 1" }, //Defense upgrade
            { 28, "Increases max health by 5" }, //Health upgrade
            { 29, "Increases max mana by 10" }, //Mana upgrade
            { 30, "Worth 1 gold" }, //Copper coin
            { 31, "Worth 5 gold" }, //Copper coins
            { 32, "Worth 10 gold" }, //Copper coin pile
            { 33, "Worth 3 gold" }, //Silver coin
            { 34, "Worth 13 gold" }, //Silver coins
            { 35, "Worth 25 gold" }, //Silver coin pile
            { 36, "Worth 5 gold" }, //Gold coin
            { 37, "Worth 27 gold" }, //Gold coins
            { 38, "Worth 42 gold" }, //Gold coin pile
            { 39, "Heals 10 health" }, //Apple
            { 40, "Heals 25 health" }, //Orange
            { 41, "Heals 75 health" }, //Steak
            { 42, "Heals 50 health" }, //Fish
            { 43, "Restores 15 mana" }, //Mana shard
            { 44, "Restores 50 mana" }, //Mana orb
            { 45, "Return to Danias to open the miscellaneous shop" }, //Frying pan
            { 46, "Bring to the second level of the caves to activate the water pumps" }, //Pumps lever
            { 47, "Give to Lyron so he can clear the rocks outside of the castle" }, //Pickaxe
            { 48, "A fragment of the Frying Pan. Collect all and return to Danias to open the miscellaneous shop" }, //Frying pan fragment
            { 49, "A fragment of the Pumps Lever. Collect all and bring to the second level of the caves to activate the water pumps" }, //Pumps lever fragment
            { 50, "A fragment of the Pickaxe. Collect all and give to Lyron so he can clear the rocks outside of the castle" }, //Pickaxe fragment
            { 51, "Enables the destruction of fragile walls" }, //Hammer
            { 52, "Enables the destruction of fragile walls" }, //Hammer fragment
            { 53, "Worth 1 gold. Gotta go fast!" }, //Gold Ring
            { 54, "Increases max health by 1. This is a serious upgrade!" }, //Serious health
            { 256, "Causes bombs to drop from above" }, //Bomb trap
            { 257, "Creates a small bomb that drains mana" }, //Mana drain trap
            { 258, "Poisons all creatures in a large radius" }, //Poison trap
            { 259, "Slows all creatures in a large radius" }, //Frost trap
            { 260, "Ignites all creatures in a large radius" }, //Fire trap
            { 261, "Confuses all creatures in a large radius" }, //Confuse trap
            { 262, "Drops a random enemy banner" }, //Banner trap
            { 263, "Summons a swarm of flies" }, //Fly trap
            { 512, "Counts as 3 Bronze Keys" }, //Big bronze key
            { 513, "Counts as 3 Silver Keys" }, //Silver keyring
            { 514, "Counts as 3 Gold Keys" }, //Gold keyring
            { 515, "Counts as 3 Bonus Keys" }, //Bonus keyring
            { 516, "Opens a bronze gate in the Prison floors" }, //Prison bronze key
            { 517, "Opens a silver gate in the Prison floors" }, //Prison silver key
            { 518, "Opens a gold gate in the Prison floors" }, //Prison gold key
            { 519, "Opens a bonus level gate in the Prison floors" }, //Prison bonus key
            { 523, "Opens a bronze gate in the Armory floors" }, //Armory bronze key
            { 524, "Opens a silver gate in the Armory floors" }, //Armory silver key
            { 525, "Opens a gold gate in the Armory floors" }, //Armory gold key
            { 526, "Opens a bonus level gate in the Armory floors" }, //Armory bonus key
            { 530, "Opens a bronze gate in the Archives floors" }, //Archives bronze key
            { 531, "Opens a silver gate in the Archives floors" }, //Archives silver key
            { 532, "Opens a gold gate in the Archives floors" }, //Archives gold key
            { 533, "Opens a bonus level gate in the Archives floors" }, //Archives bonus key
            { 537, "Opens a bronze gate in the Chambers floors" }, //Chambers bronze key
            { 538, "Opens a silver gate in the Chambers floors" }, //Chambers silver key
            { 539, "Opens a gold gate in the Chambers floors" }, //Chambers gold key
            { 540, "Opens a bonus level gate in the Chambers floors" }, //Chambers bonus key
            { 541, "Counts as 3 Prison Bronze Keys" }, //Prison big bronze key
            { 542, "Counts as 3 Armory Bronze Keys" }, //Armory big bronze key
            { 543, "Counts as 3 Archives Bronze Keys" }, //Archives big bronze key
            { 544, "Counts as 3 Chambers Bronze Keys" }, //Chambers big bronze key
        };
        private static readonly string shopDescMaxHealth = "Increases max health";
        private static readonly string shopDescMaxMana = "Increases max mana and mana regen";
        private static readonly string shopDescArmor = "Increases damage reduction";
        private static readonly string shopDescMoveSpeed = "Increases move speed";
        private static readonly string shopDescCombo = "combo-udesc";
        private static readonly string shopDescComboTimer = "Makes it easier to chain combos by increasing the combo timer";
        private static readonly string shopDescComboNova = "Continuously shoots a multi-part nova that deals damage while a combo is active";
        private static readonly string shopDescComboHealing = "Heals over time while a combo is active";
        private static readonly string shopDescComboMana = "Regenerates mana over time while a combo is active";
        private static readonly Dictionary<int, string> shopItemDescriptions = new Dictionary<int, string>()
        {
            { 0, shopDescMaxHealth }, // from 75 to 120 to 165 to 210 to 255 to 300
            { 1, shopDescMaxMana },
            { 2, shopDescArmor },
            { 3, shopDescMoveSpeed },
            { 4, shopDescCombo },
            { 5, shopDescComboTimer },
            { 6, shopDescComboNova },
            { 7, shopDescComboHealing },
            { 8, shopDescComboMana },
            { 9, "Increases sword damage" },
            { 10, "Increases charge damage multiplier by 0.25x the sword damage" },
            { 11, "Increases charge range and speed by 1 unit" },
            { 12, "Heals the paladin and all team members at a rate of 5 health per 10 mana spen" },
            { 13, "Increases the amount of healing per mana spent" },
            { 14, "The paladin starts to spin for 4 seconds and strikes nearby enemies for 1.5x the sword damage" },
            { 15, "Increases holy storm damage by 0.5x the sword damage" },
            { 16, "Increases holy storm duration by 2 seconds" },
            { 17, "Gives the paladin a chance to stun any target he hits for 2.5 seconds" },
            { 18, "Increases sword arc by 30 degrees" },
            { 19, "Blocks most incoming projectiles within an arc" },
            { 20, shopDescMaxHealth },
            { 21, shopDescMaxMana },
            { 22, shopDescArmor },
            { 23, shopDescMoveSpeed },
            { 24, shopDescCombo },
            { 25, shopDescComboTimer },
            { 26, shopDescComboNova },
            { 27, shopDescComboHealing },
            { 28, shopDescComboMana },
            { 29, "Increases bow damage" },
            { 30, "Increases bow penetration by 1" },
            { 31, "Increases bomb damage" },
            { 32, "Enemies within 5 units are snared for 3 seconds" },
            { 33, "Increases snare duration of overgrowth by 1 second" },
            { 34, "Increases overgrowth range" },
            { 35, "The ranger spins around 2 times and shoots 12 arrows per revolution" },
            { 36, "Increases the number of flurry revolutions by 1" },
            { 37, "Increases the number of arrows shot per revolution by 4" },
            { 38, "Increases dodge chance by 10%" },
            { 39, "Increases double damage chance" },
            { 40, shopDescMaxHealth },
            { 41, shopDescMaxMana },
            { 42, shopDescArmor },
            { 43, shopDescMoveSpeed },
            { 44, shopDescCombo },
            { 45, shopDescComboTimer },
            { 46, shopDescComboNova },
            { 47, shopDescComboHealing },
            { 48, shopDescComboMana },
            { 49, "Increases fireball damage and splash range" },
            { 50, "Increases fireball range" },
            { 51, "Increases fire breath damage by 4" },
            { 52, "Shoots 10 flames in a circle around the wizard, each flame slows enemies for 30% and applies the current level of combustion" },
            { 53, "Increases the number of flames while also increasing their range" },
            { 54, "Increases fire nova slow by 20%" },
            { 55, "Calls upon 3 meteors to strike the ground and do 60 damage each in a large area" },
            { 56, "Increases the damage from each meteor by 40" },
            { 57, "Increases the amount of meteors summoned" },
            { 58, "Whenever the wizard takes damage his damager gets applied the current level of combustion and a combo is triggered" },
            { 59, "All the wizards fire based attacks cause their victims to burn for 8 damage per second for 3 seconds" },
            { 60, "Increases the burn damage by 4 per second" },
            { 61, "Increases the burn time by 1 second" },
            { 62, shopDescMaxHealth },
            { 63, shopDescMaxMana },
            { 64, shopDescArmor },
            { 65, shopDescMoveSpeed },
            { 66, shopDescCombo },
            { 67, shopDescComboTimer },
            { 68, shopDescComboNova },
            { 69, shopDescComboHealing },
            { 70, shopDescComboMana },
            { 71, "Increases dagger damage" },
            { 72, "Increases poison damage by 4 per second" },
            { 73, "Increases lightning strike damage" }, //4-5
            { 74, "Increases the number of targets the lightning strike hits by 1" },
            { 75, "Summons an immobile gargoyle that stands and shoots your enemies for 4 seconds" },
            { 76, "Increases the gargoyle damage by 5" },
            { 77, "Increases the duration of the gargoyle by 2 seconds" },
            { 78, "Summons an electrical storm that follows the warlock for 7 seconds and strikes enemies with lightning that does 16 damage" },
            { 79, "Increases electrical storm damage" },
            { 80, "Increases electrical storm duration by 2 seconds" },
            { 81, "Increases the chance to replenish 1 health every time he kills an enemy by 20%" },
            { 82, "Increases mana gain every time he kills an enemy by 1" },
            { 83, shopDescMaxHealth },
            { 84, shopDescMaxMana },
            { 85, shopDescArmor },
            { 86, shopDescMoveSpeed },
            { 87, shopDescCombo },
            { 88, shopDescComboTimer },
            { 89, shopDescComboNova },
            { 90, shopDescComboHealing },
            { 91, shopDescComboMana },
            { 92, "Increases damage of your knives" },
            { 93, "Increases damage of knife fan" },
            { 94, "Increases the number of knives thrown by 1" },
            { 95, "chain-udesc" },
            { 96, "Increases the length of the chain by 2 units" },
            { 97, "Increases stun duration by 0.5 seconds" },
            { 98, "Stuns all enemies within 4 units for 2 seconds" },
            { 99, "Increases smoke bomb range" },
            { 100, "Increases the max 5% attack speed stacks every time the thief kills an enemy with a melee attack within quick succession" },
            { 101, "Decreases the move speed penalty when attacking by 10%" },
            { 102, "Increases the dodge chance by 10%" },
            { 103, shopDescMaxHealth },
            { 104, shopDescMaxMana },
            { 105, shopDescArmor },
            { 106, shopDescMoveSpeed },
            { 107, shopDescCombo },
            { 108, shopDescComboTimer },
            { 109, shopDescComboNova },
            { 110, shopDescComboHealing },
            { 111, shopDescComboMana },
            { 112, "Increases damage of your smite spell" },
            { 113, "Decreases move speed penalty when attacking" },
            { 114, "Increases damage and healing of the beam" },
            { 115, "Increases the range of the beam by 1 unit" },
            { 116, "Places a field on the ground that drains 16 health from enemies to players" },
            { 117, "Increases the damage of the field" },
            { 118, "Increases the number of simultaneous fields that can be placed by 1" },
            { 119, "The priest radiates an aura that halves the damage of enemies and slows them for 30%" },
            { 120, "Increases the slow by 20%" },
            { 121, "auradrain-udesc" },
            { 122, "Increases health regen by 0.2 health per second" },
            { 123, "Increases mana shield efficiency to decrease incoming damage by 0.25 per mana point" },
            { 124, shopDescMaxHealth },
            { 125, shopDescMaxMana },
            { 126, shopDescArmor },
            { 127, shopDescMoveSpeed },
            { 128, shopDescCombo },
            { 129, shopDescComboTimer },
            { 130, shopDescComboNova },
            { 131, shopDescComboHealing },
            { 132, shopDescComboMana },
            { 133, "Increases damage of your ice shards" },
            { 134, "Icreases the range of your ice shards by 0.5 units and the number of bounces by 1" },
            { 135, "Increases damage of the comet" },
            { 136, "Shoots a nova of 9 ice shards in every direction" },
            { 137, "Increases the number of ice shards shot by the nova by 4" },
            { 138, "Decreases the mana cost of the ice shard nova by 10" },
            { 139, "orb-udesc" },
            { 140, "Increases the damage of the ice shards shot from the ice orb by 6" },
            { 141, "Increases the duration of the ice orb by 1.5 seconds" },
            { 142, "Causes the sorcerers attacks to slow enemies by 20% for 2 seconds" },
            { 143, "Increases the chill slow by 15%" },
            { 144, "Increases the chill duration by 1 second" },
            { 145, "Increases frost shield chance by 20%" },
        };
        public static Dictionary<string, Dictionary<string, string>> shopSignVendors = new Dictionary<string, Dictionary<string, string>>()
        {
            {"level_1.xml", new Dictionary<string, string>{
            { "11.5 -6", "Prison 1 Vitality Shop"},
            { "31.5 -20", "Prison 1 Combo Shop"},
            { "45.75 -21", "Prison 1 Combo Shop"},
            }},
            {"level_2.xml", new Dictionary<string, string>{
            { "13.25 -7", "Prison 1 Vitality Shop"},
            { "7.25 21", "Prison 2 Combo Shop"},
            { "41.25 -25", "Prison 2 Offense Shop"},
            { "56.5 -17", "Prison 2 Offense Shop"},
            }},
            {"level_3.xml", new Dictionary<string, string>{
            { "1 -29", "Prison 3 Defense Shop"},
            { "2.25 -29", "Prison 3 Vitality Shop"},
            { "26.375 -15", "Prison 3 Vitality Shop"},
            { "-15.75 7", "Prison 3 Offense Shop"},
            { "53.875 28", "Prison 3 Vitality Shop"},
            }},
            {"level_4.xml", new Dictionary<string, string>{
            { "-57.75 36", "Armory 4 Vitality Shop"},
            { "-60.25 36", "Armory 4 Powerup Shop"},
            { "-43.75 36", "Armory 4 Defense Shop"},
            { "-46.25 36", "Armory 4 Offense Shop"},
            }},
            {"level_5.xml", new Dictionary<string, string>{
            { "-38 29", "Armory 4 Powerup Shop"},
            { "-39.25 29", "Armory 4 Vitality Shop"},
            { "-35.375 29", "Armory 4 Combo Shop"},
            { "-26.5 31", "Armory 4 Offense Shop"},
            { "-25.375 31", "Armory 4 Defense Shop"},
            }},
            {"level_6.xml", new Dictionary<string, string>{
            { "-60.5 33", "Armory 4 Defense Shop"},
            { "-59 33", "Armory 4 Vitality Shop"},
            { "-57.5 33", "Armory 4 Offense Shop"},
            { "-35 36", "Armory 4 Combo Shop"},
            { "-33 36", "Armory 4 Powerup Shop"},
            }},
            {"level_7.xml", new Dictionary<string, string>{
            { "-17 -46", "Archives 7 Vitality Shop"},
            { "-19 -46", "Archives 7 Powerup Shop"},
            { "-53.75 14", "Archives 7 Vitality Shop"},
            { "-19 49", "Archives 8 Combo Shop"},
            { "-17.5 49", "Archives 9 Defense Shop"},
            { "-16 49", "Archives 8 Offense Shop"},
            }},
            {"level_8.xml", new Dictionary<string, string>{
            { "-36.5 9", "Archives 8 Combo Shop"},
            { "8 -7", "Archives 8 Offense Shop"},
            { "6.125 0", "Archives 8 Offense Shop"},
            { "-6 0", "Archives 8 Combo Shop"},
            { "24.75 9", "Archives 8 Offense Shop"},
            { "45 -30", "Archives 8 Offense Shop"},
            }},
            {"level_9.xml", new Dictionary<string, string>{
            { "-9 25", "Archives 9 Defense Shop"},
            { "7.5 39", "Archives 9 Defense Shop"},
            { "-10 66", "Archives 9 Defense Shop"},
            }},
            {"level_10.xml", new Dictionary<string, string>{
            { "10 0", "Chambers 10 Powerup Shop"},
            }},
            {"level_11.xml", new Dictionary<string, string>{
            { "92 -92", "Chambers 11 Secret Offense Shop"},
            { "90.75 -92", "Chambers 11 Secret Defense Shop"},
            { "16.125 -4", "Chambers 11 Combo Shop"},
            { "18.625 -4", "Chambers 11 Defense Shop"},
            { "17.375 -4", "Chambers 11 Vitality Shop"},
            { "19.875 -4", "Chambers 11 Offense Shop"},
            { "22.25 -4", "Chambers 11 Secret Offense Shop"},
            { "23.5 -4", "Chambers 11 Secret Defense Shop"},
            { "20 34", "Chambers 11 Combo Shop"},
            { "21.25 34", "Chambers 11 Defense Shop"},
            { "22.5 34", "Chambers 11 Vitality Shop"},
            { "23.75 34", "Chambers 11 Offense Shop"},
            }},
            {"level_12.xml", new Dictionary<string, string>{
            { "39 -85", "Chambers 11 Secret Offense Shop"},
            { "37.75 -85", "Chambers 11 Secret Defense Shop"},
            }},
            {"level_esc_2.xml", new Dictionary<string, string>{
            { "-17 -46", "Archives 7 Vitality Shop"},
            { "-19 -46", "Archives 7 Powerup Shop"},
            }},
        };
        public static string GetShopTypeName(string internalName)
        {
            string typeName = null;
            switch (internalName)
            {
                case "power":
                    typeName = "Powerup";
                    break;
                case "combo":
                    typeName = "Combo";
                    break;
                case "off":
                    typeName = "Offense";
                    break;
                case "offense":
                    typeName = "Offense";
                    break;
                case "misc":
                    typeName = "Vitality";
                    break;
                case "def":
                    typeName = "Defense";
                    break;
                case "defense":
                    typeName = "Defense";
                    break;
            }
            return typeName;
        }
        public static string GetInternalShopTypeFromName(string name, bool scriptNode)
        {
            string typeName = null;
            switch (name)
            {
                case "Powerup":
                    typeName = "power";
                    break;
                case "Combo":
                    typeName = "combo";
                    break;
                case "Offense":
                    typeName = scriptNode ? "off" : "offense";
                    break;
                case "Vitality":
                    typeName = "misc";
                    break;
                case "Defense":
                    typeName = scriptNode ? "def" : "defense";
                    break;
            }
            return typeName;
        }

        public static int GetLocIdFromUpgradeId(PlayerClass playerClass, string upgradeId)
        {
            //int relativeId = shopLocationId - ArchipelagoManager.templeStartID - ArchipelagoManager.shopLocationIdOffset;
            if (!shopLocationVanillaUpgradeMapping[playerClass].TryGetValue(upgradeId, out int locId))
                return -1;
            return locId + shopLocationIdOffset;
        }
        public static string[] GetShopIdsFromItemId(int itemId)
        {
            int relId = itemId - shopItemBaseId;
            return shopItemData[relId].shopItemIds;
        }
        public static PlayerClass GetShopItemClass(int itemId)
        {
            int relId = itemId - templeStartID;
            if (relId < 1280)
                return PlayerClass.KNIGHT; //This isn't a shop item!
            if (relId < 1300)
                return PlayerClass.KNIGHT;
            if (relId < 1320)
                return PlayerClass.RANGER;
            if (relId < 1342)
                return PlayerClass.WIZARD;
            if (relId < 1363)
                return PlayerClass.WARLOCK;
            if (relId < 1383)
                return PlayerClass.THIEF;
            if (relId < 1404)
                return PlayerClass.PRIEST;
            if (relId < 1426)
                return PlayerClass.SORCERER;
            return PlayerClass.KNIGHT; //This isn't a shop item!
        }
        public static string GetShopItemXml(int itemId)
        {
            int relId = itemId - shopItemBaseId;
            return $"items/shop_item_{shopItemData[relId].shopType}.xml";
        }

        private static readonly Dictionary<PlayerClass, Dictionary<string, int>> shopLocationVanillaUpgradeMapping = new Dictionary<PlayerClass, Dictionary<string, int>>()
        {
            { PlayerClass.KNIGHT, new Dictionary<string, int>(){
                { "health-1", 0 },
                { "mana-1", 1 },
                { "health-2", 2 },
                { "mana-2", 3 },
                { "health-3", 4 },
                { "mana-3", 5 },
                { "speed-1", 6 },
                { "health-4", 7 },
                { "mana-4", 8 },
                { "speed-2", 9 },
                { "health-5", 10 },
                { "mana-5", 11 },
                { "speed-3", 12 },
                { "combo", 13 },
                { "combo-time-1", 14 },
                { "combo-nova-1", 15 },
                { "combo-heal-1", 16 },
                { "combo-mana-1", 17 },
                { "combo-time-2", 18 },
                { "combo-nova-2", 19 },
                { "combo-heal-2", 20 },
                { "combo-mana-2", 21 },
                { "combo-time-3", 22 },
                { "combo-nova-3", 23 },
                { "combo-heal-3", 24 },
                { "combo-mana-3", 25 },
                { "combo-time-4", 26 },
                { "combo-nova-4", 27 },
                { "combo-heal-4", 28 },
                { "combo-mana-4", 29 },
                { "combo-time-5", 30 },
                { "combo-nova-5", 31 },
                { "combo-heal-5", 32 },
                { "combo-mana-5", 33 },
                { "dmg1", 34 },
                { "arc1", 35 },
                { "dmg2", 36 },
                { "chrgdmg1", 37 },
                { "chrgrng1", 38 },
                { "arc2", 39 },
                { "dmg3", 40 },
                { "chrgdmg2", 41 },
                { "chrgrng2", 42 },
                { "whirl", 43 },
                { "arc3", 44 },
                { "dmg4", 45 },
                { "chrgdmg3", 46 },
                { "chrgrng3", 47 },
                { "whirldmg1", 48 },
                { "whirldur1", 49 },
                { "arc4", 50 },
                { "dmg5", 51 },
                { "whirldmg2", 52 },
                { "whirldur", 53 },
                { "arc5", 54 },
                { "armor-1", 55 },
                { "bash1", 56 },
                { "shield1", 57 },
                { "armor-2", 58 },
                { "heal", 59 },
                { "bash2", 60 },
                { "shield2", 61 },
                { "armor-3", 62 },
                { "healeff1", 63 },
                { "bash3", 64 },
                { "shield3", 65 },
                { "armor-4", 66 },
                { "healeff2", 67 },
                { "armor-5", 68 },
                { "healeff3", 69 },
                }},
            { PlayerClass.RANGER, new Dictionary<string, int>(){
                { "health-1", 100 },
                { "mana-1", 101 },
                { "health-2", 102 },
                { "mana-2", 103 },
                { "health-3", 104 },
                { "mana-3", 105 },
                { "speed-1", 106 },
                { "health-4", 107 },
                { "mana-4", 108 },
                { "speed-2", 109 },
                { "health-5", 110 },
                { "mana-5", 111 },
                { "speed-3", 112 },
                { "combo", 113 },
                { "combo-time-1", 114 },
                { "combo-nova-1", 115 },
                { "combo-heal-1", 116 },
                { "combo-mana-1", 117 },
                { "combo-time-2", 118 },
                { "combo-nova-2", 119 },
                { "combo-heal-2", 120 },
                { "combo-mana-2", 121 },
                { "combo-time-3", 122 },
                { "combo-nova-3", 123 },
                { "combo-heal-3", 124 },
                { "combo-mana-3", 125 },
                { "combo-time-4", 126 },
                { "combo-nova-4", 127 },
                { "combo-heal-4", 128 },
                { "combo-mana-4", 129 },
                { "combo-time-5", 130 },
                { "combo-nova-5", 131 },
                { "combo-heal-5", 132 },
                { "combo-mana-5", 133 },
                { "dmg1", 134 },
                { "pen1", 135 },
                { "dmg2", 136 },
                { "pen2", 137 },
                { "bombdmg-1", 138 },
                { "crit1", 139 },
                { "dmg3", 140 },
                { "pen3", 141 },
                { "bombdmg-2", 142 },
                { "spread", 143 },
                { "crit2", 144 },
                { "dmg4", 145 },
                { "pen4", 146 },
                { "bombdmg-3", 147 },
                { "spreadwvs-1", 148 },
                { "spreadshts-1", 149 },
                { "crit3", 150 },
                { "dmg5", 151 },
                { "pen5", 152 },
                { "spreadwvs-2", 153 },
                { "spreadshts-2", 154 },
                { "crit4", 155 },
                { "armor-1", 156 },
                { "dodge1", 157 },
                { "armor-2", 158 },
                { "growth", 159 },
                { "dodge2", 160 },
                { "armor-3", 161 },
                { "growthdur-1", 162 },
                { "dodge3", 163 },
                { "armor-4", 164 },
                { "growthdur-2", 165 },
                { "growthrng-1", 166 },
                { "dodge4", 167 },
                { "armor-5", 168 },
                { "growthrng-2", 169 },
                { "dodge5", 170 },
                }},
            { PlayerClass.WIZARD, new Dictionary<string, int>(){
                { "health-1", 200 },
                { "mana-1", 201 },
                { "health-2", 202 },
                { "mana-2", 203 },
                { "health-3", 204 },
                { "mana-3", 205 },
                { "speed-1", 206 },
                { "health-4", 207 },
                { "mana-4", 208 },
                { "speed-2", 209 },
                { "health-5", 210 },
                { "mana-5", 211 },
                { "speed-3", 212 },
                { "combo", 213 },
                { "combo-time-1", 214 },
                { "combo-nova-1", 215 },
                { "combo-heal-1", 216 },
                { "combo-mana-1", 217 },
                { "combo-time-2", 218 },
                { "combo-nova-2", 219 },
                { "combo-heal-2", 220 },
                { "combo-mana-2", 221 },
                { "combo-time-3", 222 },
                { "combo-nova-3", 223 },
                { "combo-heal-3", 224 },
                { "combo-mana-3", 225 },
                { "combo-time-4", 226 },
                { "combo-nova-4", 227 },
                { "combo-heal-4", 228 },
                { "combo-mana-4", 229 },
                { "combo-time-5", 230 },
                { "combo-nova-5", 231 },
                { "combo-heal-5", 232 },
                { "combo-mana-5", 233 },
                { "dmg1", 234 },
                { "rng1", 235 },
                { "dmg2", 236 },
                { "rng2", 237 },
                { "spraydmg1", 238 },
                { "combust", 239 },
                { "dmg3", 240 },
                { "rng3", 241 },
                { "spraydmg2", 242 },
                { "meteor", 243 },
                { "meteornum-1", 244 },
                { "combustdmg1", 245 },
                { "combustdur1", 246 },
                { "dmg4", 247 },
                { "rng4", 248 },
                { "spraydmg3", 249 },
                { "meteordmg-1", 250 },
                { "meteornum-2", 251 },
                { "combustdmg2", 252 },
                { "combustdur2", 253 },
                { "dmg5", 254 },
                { "rng5", 255 },
                { "spraydmg4", 256 },
                { "meteordmg-2", 257 },
                { "meteornum-3", 258 },
                { "combustdmg3", 259 },
                { "combustdur3", 260 },
                { "armor-1", 261 },
                { "fire-shield", 262 },
                { "armor-2", 263 },
                { "fnova", 264 },
                { "armor-3", 265 },
                { "fnovanum-1", 266 },
                { "fnovaslow-1", 267 },
                { "armor-4", 268 },
                { "fnovanum-2", 269 },
                { "fnovaslow-2", 270 },
                { "fnovanum-3", 271 },
                { "fnovaslow-3", 272 },
                }},
            { PlayerClass.WARLOCK, new Dictionary<string, int>(){
                { "health-1", 300 },
                { "mana-1", 301 },
                { "health-2", 302 },
                { "mana-2", 303 },
                { "health-3", 304 },
                { "mana-3", 305 },
                { "speed-1", 306 },
                { "health-4", 307 },
                { "mana-4", 308 },
                { "speed-2", 309 },
                { "health-5", 310 },
                { "mana-5", 311 },
                { "speed-3", 312 },
                { "combo", 313 },
                { "combo-time-1", 314 },
                { "combo-nova-1", 315 },
                { "combo-heal-1", 316 },
                { "combo-mana-1", 317 },
                { "combo-time-2", 318 },
                { "combo-nova-2", 319 },
                { "combo-heal-2", 320 },
                { "combo-mana-2", 321 },
                { "combo-time-3", 322 },
                { "combo-nova-3", 323 },
                { "combo-heal-3", 324 },
                { "combo-mana-3", 325 },
                { "combo-time-4", 326 },
                { "combo-nova-4", 327 },
                { "combo-heal-4", 328 },
                { "combo-mana-4", 329 },
                { "combo-time-5", 330 },
                { "combo-nova-5", 331 },
                { "combo-heal-5", 332 },
                { "combo-mana-5", 333 },
                { "dmg1", 334 },
                { "poison1", 335 },
                { "dmg2", 336 },
                { "poison2", 337 },
                { "lightningdmg1", 338 },
                { "lightningtrg1", 339 },
                { "kmana1", 340 },
                { "dmg3", 341 },
                { "poison3", 342 },
                { "lightningdmg2", 343 },
                { "lightningtrg2", 344 },
                { "storm", 345 },
                { "kmana2", 346 },
                { "dmg4", 347 },
                { "poison4", 348 },
                { "lightningdmg3", 349 },
                { "lightningtrg3", 350 },
                { "stormdmg-1", 351 },
                { "stormdur-1", 352 },
                { "kmana3", 353 },
                { "dmg5", 354 },
                { "poison5", 355 },
                { "lightningdmg4", 356 },
                { "lightningtrg4", 357 },
                { "stormdmg-2", 358 },
                { "stormdur-2", 359 },
                { "armor-1", 360 },
                { "kheal1", 361 },
                { "armor-2", 362 },
                { "garg", 363 },
                { "kheal2", 364 },
                { "armor-3", 365 },
                { "gargdmg1", 366 },
                { "gargdur1", 367 },
                { "kheal3", 368 },
                { "armor-4", 369 },
                { "gargdmg2", 370 },
                { "gargdur2", 371 },
                { "kheal4", 372 },
                { "armor-5", 373 },
                { "kheal5", 374 },
                }},
            { PlayerClass.THIEF, new Dictionary<string, int>(){
                { "health-1", 400 },
                { "mana-1", 401 },
                { "health-2", 402 },
                { "mana-2", 403 },
                { "health-3", 404 },
                { "mana-3", 405 },
                { "speed-1", 406 },
                { "health-4", 407 },
                { "mana-4", 408 },
                { "speed-2", 409 },
                { "health-5", 410 },
                { "mana-5", 411 },
                { "speed-3", 412 },
                { "combo", 413 },
                { "combo-time-1", 414 },
                { "combo-nova-1", 415 },
                { "combo-heal-1", 416 },
                { "combo-mana-1", 417 },
                { "combo-time-2", 418 },
                { "combo-nova-2", 419 },
                { "combo-heal-2", 420 },
                { "combo-mana-2", 421 },
                { "combo-time-3", 422 },
                { "combo-nova-3", 423 },
                { "combo-heal-3", 424 },
                { "combo-mana-3", 425 },
                { "combo-time-4", 426 },
                { "combo-nova-4", 427 },
                { "combo-heal-4", 428 },
                { "combo-mana-4", 429 },
                { "combo-time-5", 430 },
                { "combo-nova-5", 431 },
                { "combo-heal-5", 432 },
                { "combo-mana-5", 433 },
                { "dmg1", 434 },
                { "aspeed1", 435 },
                { "dmg2", 436 },
                { "kfandmg1", 437 },
                { "kfanprojs1", 438 },
                { "fervor1", 439 },
                { "aspeed2", 440 },
                { "dmg3", 441 },
                { "kfandmg2", 442 },
                { "kfanprojs2", 443 },
                { "fervor2", 444 },
                { "aspeed3", 445 },
                { "dmg4", 446 },
                { "kfandmg3", 447 },
                { "kfanprojs3", 448 },
                { "fervor3", 449 },
                { "aspeed4", 450 },
                { "dmg5", 451 },
                { "armor-1", 452 },
                { "dodge1", 453 },
                { "armor-2", 454 },
                { "chain", 455 },
                { "chainrng1", 456 },
                { "dodge2", 457 },
                { "armor-3", 458 },
                { "chainrng2", 459 },
                { "chainstn1", 460 },
                { "smoke", 461 },
                { "dodge3", 462 },
                { "armor-4", 463 },
                { "chainstn2", 464 },
                { "smokerng1", 465 },
                { "dodge4", 466 },
                { "armor-5", 467 },
                { "smokerng2", 468 },
                { "dodge5", 469 },
                }},
            { PlayerClass.PRIEST, new Dictionary<string, int>(){
                { "health-1", 500 },
                { "mana-1", 501 },
                { "health-2", 502 },
                { "mana-2", 503 },
                { "health-3", 504 },
                { "mana-3", 505 },
                { "speed-1", 506 },
                { "health-4", 507 },
                { "mana-4", 508 },
                { "speed-2", 509 },
                { "health-5", 510 },
                { "mana-5", 511 },
                { "speed-3", 512 },
                { "combo", 513 },
                { "combo-time-1", 514 },
                { "combo-nova-1", 515 },
                { "combo-heal-1", 516 },
                { "combo-mana-1", 517 },
                { "combo-time-2", 518 },
                { "combo-nova-2", 519 },
                { "combo-heal-2", 520 },
                { "combo-mana-2", 521 },
                { "combo-time-3", 522 },
                { "combo-nova-3", 523 },
                { "combo-heal-3", 524 },
                { "combo-mana-3", 525 },
                { "combo-time-4", 526 },
                { "combo-nova-4", 527 },
                { "combo-heal-4", 528 },
                { "combo-mana-4", 529 },
                { "combo-time-5", 530 },
                { "combo-nova-5", 531 },
                { "combo-heal-5", 532 },
                { "combo-mana-5", 533 },
                { "dmg1", 534 },
                { "sspeed1", 535 },
                { "dmg2", 536 },
                { "sspeed2", 537 },
                { "beamdmg1", 538 },
                { "beamrng1", 539 },
                { "area", 540 },
                { "dmg3", 541 },
                { "sspeed3", 542 },
                { "beamdmg2", 543 },
                { "beamrng2", 544 },
                { "areadmg-1", 545 },
                { "areanum-1", 546 },
                { "dmg4", 547 },
                { "sspeed4", 548 },
                { "beamdmg3", 549 },
                { "beamrng3", 550 },
                { "areadmg-2", 551 },
                { "areanum-2", 552 },
                { "dmg5", 553 },
                { "sspeed5", 554 },
                { "beamdmg4", 555 },
                { "beamrng4", 556 },
                { "areadmg-3", 557 },
                { "armor-1", 558 },
                { "hpregen1", 559 },
                { "mshield1", 560 },
                { "armor-2", 561 },
                { "hpregen2", 562 },
                { "mshield2", 563 },
                { "armor-3", 564 },
                { "aura", 565 },
                { "auradrain", 566 },
                { "hpregen3", 567 },
                { "mshield3", 568 },
                { "armor-4", 569 },
                { "auraslow-1", 570 },
                { "hpregen4", 571 },
                { "mshield4", 572 },
                { "armor-5", 573 },
                { "auraslow-2", 574 },
                { "hpregen5", 575 },
                { "mshield5", 576 },
                }},
            { PlayerClass.SORCERER, new Dictionary<string, int>(){
                { "health-1", 600 },
                { "mana-1", 601 },
                { "health-2", 602 },
                { "mana-2", 603 },
                { "health-3", 604 },
                { "mana-3", 605 },
                { "speed-1", 606 },
                { "health-4", 607 },
                { "mana-4", 608 },
                { "speed-2", 609 },
                { "health-5", 610 },
                { "mana-5", 611 },
                { "speed-3", 612 },
                { "combo", 613 },
                { "combo-time-1", 614 },
                { "combo-nova-1", 615 },
                { "combo-heal-1", 616 },
                { "combo-mana-1", 617 },
                { "combo-time-2", 618 },
                { "combo-nova-2", 619 },
                { "combo-heal-2", 620 },
                { "combo-mana-2", 621 },
                { "combo-time-3", 622 },
                { "combo-nova-3", 623 },
                { "combo-heal-3", 624 },
                { "combo-mana-3", 625 },
                { "combo-time-4", 626 },
                { "combo-nova-4", 627 },
                { "combo-heal-4", 628 },
                { "combo-mana-4", 629 },
                { "combo-time-5", 630 },
                { "combo-nova-5", 631 },
                { "combo-heal-5", 632 },
                { "combo-mana-5", 633 },
                { "dmg1", 634 },
                { "rng1", 635 },
                { "dmg2", 636 },
                { "rng2", 637 },
                { "cometdmg1", 638 },
                { "chill", 639 },
                { "dmg3", 640 },
                { "rng3", 641 },
                { "cometdmg2", 642 },
                { "orb", 643 },
                { "orbtime-1", 644 },
                { "chillslow1", 645 },
                { "chilldur1", 646 },
                { "dmg4", 647 },
                { "rng4", 648 },
                { "cometdmg3", 649 },
                { "orbdmg-1", 650 },
                { "orbtime-2", 651 },
                { "chillslow2", 652 },
                { "chilldur2", 653 },
                { "dmg5", 654 },
                { "rng5", 655 },
                { "cometdmg4", 656 },
                { "orbdmg-2", 657 },
                { "orbtime-3", 658 },
                { "chillslow3", 659 },
                { "chilldur3", 660 },
                { "armor-1", 661 },
                { "fshield1", 662 },
                { "armor-2", 663 },
                { "nova", 664 },
                { "fshield2", 665 },
                { "armor-3", 666 },
                { "novanum-1", 667 },
                { "novamana-1", 668 },
                { "fshield3", 669 },
                { "armor-4", 670 },
                { "novanum-2", 671 },
                { "fshield4", 672 },
                { "novamana-2", 673 },
                { "fshield5", 674 },
                }},
        };
        public static readonly Dictionary<string, PlayerClass> classFromString = new Dictionary<string, PlayerClass>()
        {
            { "knight.xml", PlayerClass.KNIGHT },
            { "priest.xml", PlayerClass.PRIEST },
            { "ranger.xml", PlayerClass.RANGER },
            { "sorcerer.xml", PlayerClass.SORCERER },
            { "thief.xml", PlayerClass.THIEF },
            { "warlock.xml", PlayerClass.WARLOCK },
            { "wizard.xml", PlayerClass.WIZARD },
        };
        private static readonly Dictionary<int, ShopItemData> shopItemData = new Dictionary<int, ShopItemData>()
        {
            { 0, new ShopItemData("misc", new string[]{ "health-1", "health-2", "health-3", "health-4", "health-5", }) },
            { 1, new ShopItemData("misc", new string[]{ "mana-1", "mana-2", "mana-3", "mana-4", "mana-5", }) },
            { 2, new ShopItemData("def", new string[]{ "armor-1", "armor-2", "armor-3", "armor-4", "armor-5", }) },
            { 3, new ShopItemData("misc", new string[]{ "speed-1", "speed-2", "speed-3", }) },
            { 4, new ShopItemData("combo", new string[]{ "combo", }) },
            { 5, new ShopItemData("combo", new string[]{ "combo-time-1", "combo-time-2", "combo-time-3", "combo-time-4", "combo-time-5", }) },
            { 6, new ShopItemData("combo", new string[]{ "combo-nova-1", "combo-nova-2", "combo-nova-3", "combo-nova-4", "combo-nova-5", }) },
            { 7, new ShopItemData("combo", new string[]{ "combo-heal-1", "combo-heal-2", "combo-heal-3", "combo-heal-4", "combo-heal-5", }) },
            { 8, new ShopItemData("combo", new string[]{ "combo-mana-1", "combo-mana-2", "combo-mana-3", "combo-mana-4", "combo-mana-5", }) },
            { 9, new ShopItemData("off", new string[]{ "dmg1", "dmg2", "dmg3", "dmg4", "dmg5", }) },
            { 10, new ShopItemData("off", new string[]{ "chrgdmg1", "chrgdmg2", "chrgdmg3", }) },
            { 11, new ShopItemData("off", new string[]{ "chrgrng1", "chrgrng2", "chrgrng3", }) },
            { 12, new ShopItemData("def", new string[]{ "heal", }) },
            { 13, new ShopItemData("def", new string[]{ "healeff1", "healeff2", "healeff3", }) },
            { 14, new ShopItemData("off", new string[]{ "whirl", }) },
            { 15, new ShopItemData("off", new string[]{ "whirldmg1", "whirldmg2", }) },
            { 16, new ShopItemData("off", new string[]{ "whirldur1", "whirldur", }) },
            { 17, new ShopItemData("def", new string[]{ "bash1", "bash2", "bash3", }) },
            { 18, new ShopItemData("off", new string[]{ "arc1", "arc2", "arc3", "arc4", "arc5", }) },
            { 19, new ShopItemData("def", new string[]{ "shield1", "shield2", "shield3", }) },
            { 20, new ShopItemData("misc", new string[]{ "health-1", "health-2", "health-3", "health-4", "health-5", }) },
            { 21, new ShopItemData("misc", new string[]{ "mana-1", "mana-2", "mana-3", "mana-4", "mana-5", }) },
            { 22, new ShopItemData("def", new string[]{ "armor-1", "armor-2", "armor-3", "armor-4", "armor-5", }) },
            { 23, new ShopItemData("misc", new string[]{ "speed-1", "speed-2", "speed-3", }) },
            { 24, new ShopItemData("combo", new string[]{ "combo", }) },
            { 25, new ShopItemData("combo", new string[]{ "combo-time-1", "combo-time-2", "combo-time-3", "combo-time-4", "combo-time-5", }) },
            { 26, new ShopItemData("combo", new string[]{ "combo-nova-1", "combo-nova-2", "combo-nova-3", "combo-nova-4", "combo-nova-5", }) },
            { 27, new ShopItemData("combo", new string[]{ "combo-heal-1", "combo-heal-2", "combo-heal-3", "combo-heal-4", "combo-heal-5", }) },
            { 28, new ShopItemData("combo", new string[]{ "combo-mana-1", "combo-mana-2", "combo-mana-3", "combo-mana-4", "combo-mana-5", }) },
            { 29, new ShopItemData("off", new string[]{ "dmg1", "dmg2", "dmg3", "dmg4", "dmg5", }) },
            { 30, new ShopItemData("off", new string[]{ "pen1", "pen2", "pen3", "pen4", "pen5", }) },
            { 31, new ShopItemData("off", new string[]{ "bombdmg-1", "bombdmg-2", "bombdmg-3", }) },
            { 32, new ShopItemData("def", new string[]{ "growth", }) },
            { 33, new ShopItemData("def", new string[]{ "growthdur-1", "growthdur-2", }) },
            { 34, new ShopItemData("def", new string[]{ "growthrng-1", "growthrng-2", }) },
            { 35, new ShopItemData("off", new string[]{ "spread", }) },
            { 36, new ShopItemData("off", new string[]{ "spreadwvs-1", "spreadwvs-2", }) },
            { 37, new ShopItemData("off", new string[]{ "spreadshts-1", "spreadshts-2", }) },
            { 38, new ShopItemData("def", new string[]{ "dodge1", "dodge2", "dodge3", "dodge4", "dodge5", }) },
            { 39, new ShopItemData("off", new string[]{ "crit1", "crit2", "crit3", "crit4", }) },
            { 40, new ShopItemData("misc", new string[]{ "health-1", "health-2", "health-3", "health-4", "health-5", }) },
            { 41, new ShopItemData("misc", new string[]{ "mana-1", "mana-2", "mana-3", "mana-4", "mana-5", }) },
            { 42, new ShopItemData("def", new string[]{ "armor-1", "armor-2", "armor-3", "armor-4", }) },
            { 43, new ShopItemData("misc", new string[]{ "speed-1", "speed-2", "speed-3", }) },
            { 44, new ShopItemData("combo", new string[]{ "combo", }) },
            { 45, new ShopItemData("combo", new string[]{ "combo-time-1", "combo-time-2", "combo-time-3", "combo-time-4", "combo-time-5", }) },
            { 46, new ShopItemData("combo", new string[]{ "combo-nova-1", "combo-nova-2", "combo-nova-3", "combo-nova-4", "combo-nova-5", }) },
            { 47, new ShopItemData("combo", new string[]{ "combo-heal-1", "combo-heal-2", "combo-heal-3", "combo-heal-4", "combo-heal-5", }) },
            { 48, new ShopItemData("combo", new string[]{ "combo-mana-1", "combo-mana-2", "combo-mana-3", "combo-mana-4", "combo-mana-5", }) },
            { 49, new ShopItemData("off", new string[]{ "dmg1", "dmg2", "dmg3", "dmg4", "dmg5", }) },
            { 50, new ShopItemData("off", new string[]{ "rng1", "rng2", "rng3", "rng4", "rng5", }) },
            { 51, new ShopItemData("off", new string[]{ "spraydmg1", "spraydmg2", "spraydmg3", "spraydmg4", }) },
            { 52, new ShopItemData("def", new string[]{ "fnova", }) },
            { 53, new ShopItemData("def", new string[]{ "fnovanum-1", "fnovanum-2", "fnovanum-3", }) },
            { 54, new ShopItemData("def", new string[]{ "fnovaslow-1", "fnovaslow-2", "fnovaslow-3", }) },
            { 55, new ShopItemData("off", new string[]{ "meteor", }) },
            { 56, new ShopItemData("off", new string[]{ "meteordmg-1", "meteordmg-2", }) },
            { 57, new ShopItemData("off", new string[]{ "meteornum-1", "meteornum-2", "meteornum-3", }) },
            { 58, new ShopItemData("def", new string[]{ "fire-shield", }) },
            { 59, new ShopItemData("off", new string[]{ "combust", }) },
            { 60, new ShopItemData("off", new string[]{ "combustdmg1", "combustdmg2", "combustdmg3", }) },
            { 61, new ShopItemData("off", new string[]{ "combustdur1", "combustdur2", "combustdur3", }) },
            { 62, new ShopItemData("misc", new string[]{ "health-1", "health-2", "health-3", "health-4", "health-5", }) },
            { 63, new ShopItemData("misc", new string[]{ "mana-1", "mana-2", "mana-3", "mana-4", "mana-5", }) },
            { 64, new ShopItemData("def", new string[]{ "armor-1", "armor-2", "armor-3", "armor-4", "armor-5", }) },
            { 65, new ShopItemData("misc", new string[]{ "speed-1", "speed-2", "speed-3", }) },
            { 66, new ShopItemData("combo", new string[]{ "combo", }) },
            { 67, new ShopItemData("combo", new string[]{ "combo-time-1", "combo-time-2", "combo-time-3", "combo-time-4", "combo-time-5", }) },
            { 68, new ShopItemData("combo", new string[]{ "combo-nova-1", "combo-nova-2", "combo-nova-3", "combo-nova-4", "combo-nova-5", }) },
            { 69, new ShopItemData("combo", new string[]{ "combo-heal-1", "combo-heal-2", "combo-heal-3", "combo-heal-4", "combo-heal-5", }) },
            { 70, new ShopItemData("combo", new string[]{ "combo-mana-1", "combo-mana-2", "combo-mana-3", "combo-mana-4", "combo-mana-5", }) },
            { 71, new ShopItemData("off", new string[]{ "dmg1", "dmg2", "dmg3", "dmg4", "dmg5", }) },
            { 72, new ShopItemData("off", new string[]{ "poison1", "poison2", "poison3", "poison4", "poison5", }) },
            { 73, new ShopItemData("off", new string[]{ "lightningdmg1", "lightningdmg2", "lightningdmg3", "lightningdmg4", }) },
            { 74, new ShopItemData("off", new string[]{ "lightningtrg1", "lightningtrg2", "lightningtrg3", "lightningtrg4", }) },
            { 75, new ShopItemData("def", new string[]{ "garg", }) },
            { 76, new ShopItemData("def", new string[]{ "gargdmg1", "gargdmg2", }) },
            { 77, new ShopItemData("def", new string[]{ "gargdur1", "gargdur2", }) },
            { 78, new ShopItemData("off", new string[]{ "storm", }) },
            { 79, new ShopItemData("off", new string[]{ "stormdmg-1", "stormdmg-2", }) },
            { 80, new ShopItemData("off", new string[]{ "stormdur-1", "stormdur-2", }) },
            { 81, new ShopItemData("def", new string[]{ "kheal1", "kheal2", "kheal3", "kheal4", "kheal5", }) },
            { 82, new ShopItemData("off", new string[]{ "kmana1", "kmana2", "kmana3", }) },
            { 83, new ShopItemData("misc", new string[]{ "health-1", "health-2", "health-3", "health-4", "health-5", }) },
            { 84, new ShopItemData("misc", new string[]{ "mana-1", "mana-2", "mana-3", "mana-4", "mana-5", }) },
            { 85, new ShopItemData("def", new string[]{ "armor-1", "armor-2", "armor-3", "armor-4", "armor-5", }) },
            { 86, new ShopItemData("misc", new string[]{ "speed-1", "speed-2", "speed-3", }) },
            { 87, new ShopItemData("combo", new string[]{ "combo", }) },
            { 88, new ShopItemData("combo", new string[]{ "combo-time-1", "combo-time-2", "combo-time-3", "combo-time-4", "combo-time-5", }) },
            { 89, new ShopItemData("combo", new string[]{ "combo-nova-1", "combo-nova-2", "combo-nova-3", "combo-nova-4", "combo-nova-5", }) },
            { 90, new ShopItemData("combo", new string[]{ "combo-heal-1", "combo-heal-2", "combo-heal-3", "combo-heal-4", "combo-heal-5", }) },
            { 91, new ShopItemData("combo", new string[]{ "combo-mana-1", "combo-mana-2", "combo-mana-3", "combo-mana-4", "combo-mana-5", }) },
            { 92, new ShopItemData("off", new string[]{ "dmg1", "dmg2", "dmg3", "dmg4", "dmg5", }) },
            { 93, new ShopItemData("off", new string[]{ "kfandmg1", "kfandmg2", "kfandmg3", }) },
            { 94, new ShopItemData("off", new string[]{ "kfanprojs1", "kfanprojs2", "kfanprojs3", }) },
            { 95, new ShopItemData("def", new string[]{ "chain", }) },
            { 96, new ShopItemData("def", new string[]{ "chainrng1", "chainrng2", }) },
            { 97, new ShopItemData("def", new string[]{ "chainstn1", "chainstn2", }) },
            { 98, new ShopItemData("def", new string[]{ "smoke", }) },
            { 99, new ShopItemData("def", new string[]{ "smokerng1", "smokerng2", }) },
            { 100, new ShopItemData("off", new string[]{ "fervor1", "fervor2", "fervor3", }) },
            { 101, new ShopItemData("off", new string[]{ "aspeed1", "aspeed2", "aspeed3", "aspeed4", }) },
            { 102, new ShopItemData("def", new string[]{ "dodge1", "dodge2", "dodge3", "dodge4", "dodge5", }) },
            { 103, new ShopItemData("misc", new string[]{ "health-1", "health-2", "health-3", "health-4", "health-5", }) },
            { 104, new ShopItemData("misc", new string[]{ "mana-1", "mana-2", "mana-3", "mana-4", "mana-5", }) },
            { 105, new ShopItemData("def", new string[]{ "armor-1", "armor-2", "armor-3", "armor-4", "armor-5", }) },
            { 106, new ShopItemData("misc", new string[]{ "speed-1", "speed-2", "speed-3", }) },
            { 107, new ShopItemData("combo", new string[]{ "combo", }) },
            { 108, new ShopItemData("combo", new string[]{ "combo-time-1", "combo-time-2", "combo-time-3", "combo-time-4", "combo-time-5", }) },
            { 109, new ShopItemData("combo", new string[]{ "combo-nova-1", "combo-nova-2", "combo-nova-3", "combo-nova-4", "combo-nova-5", }) },
            { 110, new ShopItemData("combo", new string[]{ "combo-heal-1", "combo-heal-2", "combo-heal-3", "combo-heal-4", "combo-heal-5", }) },
            { 111, new ShopItemData("combo", new string[]{ "combo-mana-1", "combo-mana-2", "combo-mana-3", "combo-mana-4", "combo-mana-5", }) },
            { 112, new ShopItemData("off", new string[]{ "dmg1", "dmg2", "dmg3", "dmg4", "dmg5", }) },
            { 113, new ShopItemData("off", new string[]{ "sspeed1", "sspeed2", "sspeed3", "sspeed4", "sspeed5", }) },
            { 114, new ShopItemData("off", new string[]{ "beamdmg1", "beamdmg2", "beamdmg3", "beamdmg4", }) },
            { 115, new ShopItemData("off", new string[]{ "beamrng1", "beamrng2", "beamrng3", "beamrng4", }) },
            { 116, new ShopItemData("off", new string[]{ "area", }) },
            { 117, new ShopItemData("off", new string[]{ "areadmg-1", "areadmg-2", "areadmg-3", }) },
            { 118, new ShopItemData("off", new string[]{ "areanum-1", "areanum-2", }) },
            { 119, new ShopItemData("def", new string[]{ "aura", }) },
            { 120, new ShopItemData("def", new string[]{ "auraslow-1", "auraslow-2", }) },
            { 121, new ShopItemData("def", new string[]{ "auradrain", }) },
            { 122, new ShopItemData("def", new string[]{ "hpregen1", "hpregen2", "hpregen3", "hpregen4", "hpregen5", }) },
            { 123, new ShopItemData("def", new string[]{ "mshield1", "mshield2", "mshield3", "mshield4", "mshield5", }) },
            { 124, new ShopItemData("misc", new string[]{ "health-1", "health-2", "health-3", "health-4", "health-5", }) },
            { 125, new ShopItemData("misc", new string[]{ "mana-1", "mana-2", "mana-3", "mana-4", "mana-5", }) },
            { 126, new ShopItemData("def", new string[]{ "armor-1", "armor-2", "armor-3", "armor-4", }) },
            { 127, new ShopItemData("misc", new string[]{ "speed-1", "speed-2", "speed-3", }) },
            { 128, new ShopItemData("combo", new string[]{ "combo", }) },
            { 129, new ShopItemData("combo", new string[]{ "combo-time-1", "combo-time-2", "combo-time-3", "combo-time-4", "combo-time-5", }) },
            { 130, new ShopItemData("combo", new string[]{ "combo-nova-1", "combo-nova-2", "combo-nova-3", "combo-nova-4", "combo-nova-5", }) },
            { 131, new ShopItemData("combo", new string[]{ "combo-heal-1", "combo-heal-2", "combo-heal-3", "combo-heal-4", "combo-heal-5", }) },
            { 132, new ShopItemData("combo", new string[]{ "combo-mana-1", "combo-mana-2", "combo-mana-3", "combo-mana-4", "combo-mana-5", }) },
            { 133, new ShopItemData("off", new string[]{ "dmg1", "dmg2", "dmg3", "dmg4", "dmg5", }) },
            { 134, new ShopItemData("off", new string[]{ "rng1", "rng2", "rng3", "rng4", "rng5", }) },
            { 135, new ShopItemData("off", new string[]{ "cometdmg1", "cometdmg2", "cometdmg3", "cometdmg4", }) },
            { 136, new ShopItemData("def", new string[]{ "nova", }) },
            { 137, new ShopItemData("def", new string[]{ "novanum-1", "novanum-2", }) },
            { 138, new ShopItemData("def", new string[]{ "novamana-1", "novamana-2", }) },
            { 139, new ShopItemData("off", new string[]{ "orb", }) },
            { 140, new ShopItemData("off", new string[]{ "orbdmg-1", "orbdmg-2", }) },
            { 141, new ShopItemData("off", new string[]{ "orbtime-1", "orbtime-2", "orbtime-3", }) },
            { 142, new ShopItemData("off", new string[]{ "chill", }) },
            { 143, new ShopItemData("off", new string[]{ "chillslow1", "chillslow2", "chillslow3", }) },
            { 144, new ShopItemData("off", new string[]{ "chilldur1", "chilldur2", "chilldur3", }) },
            { 145, new ShopItemData("def", new string[]{ "fshield1", "fshield2", "fshield3", "fshield4", "fshield5", }) },
        };

        //Button data
        //<level_name, <AreaTrigger id, <item effect id, List<button effect script nodes (to unhook)>, ButtonType, unhook node, position override>>>
        public static Dictionary<string, Dictionary<string, ButtonData>> castleButtonNodes = new Dictionary<string, Dictionary<string, ButtonData>>()
        {
            {"level_1.xml", new Dictionary<string, ButtonData>{
            { "2181", new ButtonData(15, new List<string>(){"2183"}, ButtonType.Floor, "2178")},
            { "971", new ButtonData(192, new List<string>(){ "964", "961", "962", "963" }, ButtonType.Floor)},
            { "969", new ButtonData(192, new List<string>(){ "964", "961", "962", "963" }, ButtonType.Floor)},
            { "918", new ButtonData(192, new List<string>(){ "964", "961", "962", "963" }, ButtonType.Floor)},
            { "1137", new ButtonData(192, new List<string>(){ "964", "961", "962", "963" }, ButtonType.Floor)},
            }},
            {"level_2.xml", new Dictionary<string, ButtonData>{
            { "4191", new ButtonData(193, new List<string>(){ "4196", "4189", "4182", "4183" }, ButtonType.Floor)}, //Boss buttons
            { "4181", new ButtonData(193, new List<string>(){ "4196", "4189", "4182", "4183" }, ButtonType.Floor)},
            { "4194", new ButtonData(193, new List<string>(){ "4196", "4189", "4182", "4183" }, ButtonType.Floor)},
            { "4178", new ButtonData(193, new List<string>(){ "4196", "4189", "4182", "4183" }, ButtonType.Floor)},
            { "882", new ButtonData(19, new List<string>(), ButtonType.Floor, null, new Vector2(6, -79.5f))}, //East spike buttons
            { "927", new ButtonData(19, new List<string>(), ButtonType.Floor, null, new Vector2(6, -77.5f))}, //East spike buttons
            { "1017", new ButtonData(19, new List<string>(), ButtonType.Floor, null, new Vector2(6, -75.5f))}, //East spike buttons
            { "1074", new ButtonData(20, new List<string>(), ButtonType.Floor, null, new Vector2(-2, -71))}, //South spike buttons
            { "1490", new ButtonData(20, new List<string>(), ButtonType.Floor, null, new Vector2(0, -71))}, //South spike buttons
            { "1061", new ButtonData(20, new List<string>(), ButtonType.Floor, null, new Vector2(2, -71))}, //South spike buttons
            { "961", new ButtonData(21, new List<string>(), ButtonType.Floor, null, new Vector2(-2, -83))}, //North spike buttons
            { "933", new ButtonData(21, new List<string>(), ButtonType.Floor, null, new Vector2(0, -83))}, //North spike buttons
            { "897", new ButtonData(21, new List<string>(), ButtonType.Floor, null, new Vector2(2, -83))}, //North spike buttons
            { "1247", new ButtonData(22, new List<string>(){ "1248" }, ButtonType.Floor, "1660")}, //Red spike button
            { "3643", new ButtonData(23, new List<string>(){ "3649" }, ButtonType.Floor, "3641")}, //Tp button left
            { "4465", new ButtonData(24, new List<string>(){ "3639" }, ButtonType.Floor, "4466")}, //Tp button right
            { "5640", new ButtonData(25, new List<string>(){ "5639" }, ButtonType.Floor, "5642")}, //East save button
            //{ "6673", new ButtonData(27, new List<string>(), ButtonType.Floor )}, //West puzzle button
            //{ "4", new ButtonData(27, new List<string>(), ButtonType.Floor )}, //West puzzle button
            //{ "26", new ButtonData(27, new List<string>(), ButtonType.Floor )}, //West puzzle button
            //{ "32", new ButtonData(27, new List<string>(), ButtonType.Floor )}, //West puzzle button
            { "4846", new ButtonData(29, new List<string>(){ "5833", "5834", "5843", "5838" }, ButtonType.Floor )}, //SE Sequence rune
            { "6057", new ButtonData(29, new List<string>(){ "5833", "5834", "5843", "5838" }, ButtonType.Floor )}, //SE Sequence rune
            { "6207", new ButtonData(29, new List<string>(){ "5833", "5834", "5843", "5838" }, ButtonType.Floor )}, //SE Sequence rune
            { "6341", new ButtonData(29, new List<string>(){ "5833", "5834", "5843", "5838" }, ButtonType.Floor )}, //SE Sequence rune
            { "4370", new ButtonData(30, new List<string>(){ "4172" }, ButtonType.Wall, "4171", new Vector2(12.5f, -22.5f))}, //Middle room open
            { "4619", new ButtonData(31, new List<string>(){ "4620" }, ButtonType.Wall, "4617")}, //Middle shortcut top
            { "4626", new ButtonData(32, new List<string>(){ "5099" }, ButtonType.Wall, "5098", new Vector2(2.5f, 11.5f))}, //Middle shortcut bottom
            }},
            {"level_3.xml", new Dictionary<string, ButtonData>{
            { "6301", new ButtonData(33, new List<string>(){ "6517" }, ButtonType.Floor, "6304")}, //Boss door
            { "5790", new ButtonData(194, new List<string>(){ "5785", "5786", "5787", "5788" }, ButtonType.Floor)}, //Boss buttons
            { "5791", new ButtonData(194, new List<string>(){ "5785", "5786", "5787", "5788" }, ButtonType.Floor)}, //Boss buttons
            { "6284", new ButtonData(194, new List<string>(){ "5785", "5786", "5787", "5788" }, ButtonType.Floor)}, //Boss buttons
            { "6289", new ButtonData(194, new List<string>(){ "5785", "5786", "5787", "5788" }, ButtonType.Floor)}, //Boss buttons
            { "776", new ButtonData(34, new List<string>(){ "3285" }, ButtonType.Floor, "773")}, //Entrance silver gate spikes
            { "779", new ButtonData(35, new List<string>(){ "992" }, ButtonType.Floor, "985")}, //Red spikes
            { "1206", new ButtonData(36, new List<string>(){ "1018" }, ButtonType.Floor, "1016")}, //Blue spikes
            { "5455", new ButtonData(37, new List<string>(){ "5459" }, ButtonType.Floor, "5458")}, //East passage
            { "3867", new ButtonData(38, new List<string>(){ "4042" }, ButtonType.Floor, "6019")}, //South spikes
            //{ "1197", new ButtonData(40, new List<string>(), ButtonType.Floor, null, new Vector2(39.5f, -33.5f))}, //Open Bonus Portal
            //{ "1181", new ButtonData(40, new List<string>(), ButtonType.Floor, null, new Vector2(41.5f, -33.5f))}, //Open Bonus Portal
            //{ "1394", new ButtonData(40, new List<string>(), ButtonType.Floor, null, new Vector2(43.5f, -33.5f))}, //Open Bonus Portal
            //{ "1188", new ButtonData(40, new List<string>(), ButtonType.Floor, null, new Vector2(39.5f, -31.5f))}, //Open Bonus Portal
            //{ "1393", new ButtonData(40, new List<string>(), ButtonType.Floor, null, new Vector2(41.5f, -31.5f))}, //Open Bonus Portal
            //{ "1388", new ButtonData(40, new List<string>(), ButtonType.Floor, null, new Vector2(43.5f, -31.5f))}, //Open Bonus Portal
            //{ "4233", new ButtonData(40, new List<string>(), ButtonType.Floor, null, new Vector2(39.5f, -29.5f))}, //Open Bonus Portal
            //{ "4228", new ButtonData(40, new List<string>(), ButtonType.Floor, null, new Vector2(41.5f, -29.5f))}, //Open Bonus Portal
            //{ "4429", new ButtonData(40, new List<string>(), ButtonType.Floor, null, new Vector2(43.5f, -29.5f))}, //Open Bonus Portal
            { "983", new ButtonData(42, new List<string>(){ "4262", "4263", "4264", "4265", "4266" }, ButtonType.Wall, "500")}, //Open Bonus Room
            { "1395", new ButtonData(42, new List<string>(){ "4262", "4263", "4264", "4265", "4266" }, ButtonType.Wall, "1398", new Vector2(63, -43.5f))}, //Open Bonus Room
            { "2884", new ButtonData(42, new List<string>(){ "4262", "4263", "4264", "4265", "4266" }, ButtonType.Wall, "778")}, //Open Bonus Room
            { "4634", new ButtonData(42, new List<string>(){ "4262", "4263", "4264", "4265", "4266" }, ButtonType.Wall, "4250")}, //Open Bonus Room
            { "4735", new ButtonData(42, new List<string>(){ "4262", "4263", "4264", "4265", "4266" }, ButtonType.Wall, "4737")}, //Open Bonus Room
            { "2604", new ButtonData(43, new List<string>(){ "2602" }, ButtonType.Wall, "2601")}, //Entrance room
            { "423", new ButtonData(44, new List<string>(){ "784" }, ButtonType.Wall, "425")}, //North secret shop
            { "4248", new ButtonData(45, new List<string>(){ "4249" }, ButtonType.Wall, "4247")}, //Open bonus room side
            { "2443", new ButtonData(46, new List<string>(){ "2445" }, ButtonType.Wall, "2444")}, //Open sw shortcut
            { "3078", new ButtonData(47, new List<string>(){ "2885" }, ButtonType.Wall, "3077")}, //North tower room shortcut
            { "6020", new ButtonData(48, new List<string>(){ "6780" }, ButtonType.Wall, "6021")}, //South arrow hall passage
            { "6307", new ButtonData(49, new List<string>(){ "6518" }, ButtonType.Wall, "6305")}, //South exit room left side
            { "6524", new ButtonData(50, new List<string>(){ "6523" }, ButtonType.Wall, "6521")}, //South exit room right side
            { "31", new ButtonData(51, new List<string>(){ "379" }, ButtonType.Wall, "32")}, //NW treasure room
            }},
            {"level_bonus_1.xml", new Dictionary<string, ButtonData>{
            { "587", new ButtonData(52, new List<string>(){ "589" }, ButtonType.Panel)}, //NW treasure room
            { "207", new ButtonData(53, new List<string>(){ "694" }, ButtonType.Panel)}, //NE treasure room
            { "692", new ButtonData(54, new List<string>(){ "693" }, ButtonType.Panel)}, //East hall top
            { "959", new ButtonData(55, new List<string>(){ "960" }, ButtonType.Panel)}, //East hall bottom
            }},
            {"level_boss_1.xml", new Dictionary<string, ButtonData>{
                //Buttons handled in EditLevel
            }},
            {"level_4.xml", new Dictionary<string, ButtonData>{
            { "666", new ButtonData(195, new List<string>(){ "675", "671", "672", "673" }, ButtonType.Floor)}, //Boss buttons
            { "663", new ButtonData(195, new List<string>(){ "675", "671", "672", "673" }, ButtonType.Floor)}, //Boss buttons
            { "1002", new ButtonData(195, new List<string>(){ "675", "671", "672", "673" }, ButtonType.Floor)}, //Boss buttons
            { "1102", new ButtonData(195, new List<string>(){ "675", "671", "672", "673" }, ButtonType.Floor)}, //Boss buttons
            { "6340", new ButtonData(58, new List<string>(){ "6338" }, ButtonType.Floor, "6339")}, //Boss door
            { "1413", new ButtonData(59, new List<string>(){ "1415" }, ButtonType.Floor, "1412")}, //North tp item
            { "4284", new ButtonData(60, new List<string>(){ "4286" }, ButtonType.Floor, "4282")}, //Open middle shortcut
            { "5224", new ButtonData(61, new List<string>(){ "5044" }, ButtonType.Floor, "5222")}, //Disable red spikes
            { "6075", new ButtonData(62, new List<string>(){ "6078" }, ButtonType.Floor, "6074")}, //Disable sw spikes
            { "3801", new ButtonData(65, new List<string>(){ "3800" }, ButtonType.Wall, "3804")}, //Open middle cache
            { "4129", new ButtonData(66, new List<string>(){ "4131" }, ButtonType.Wall, "4126", new Vector2(13.75f, -14.5f))}, //Open ne cache
            { "6585", new ButtonData(67, new List<string>(){ "6590" }, ButtonType.Wall, "6589")}, //Open south wall
            { "7345", new ButtonData(68, new List<string>(){ "5917" }, ButtonType.Wall, "7346")}, //Open se rune room
            }},
            {"level_5.xml", new Dictionary<string, ButtonData>{
            { "995", new ButtonData(196, new List<string>(){ "1004", "1000", "1001", "1002" }, ButtonType.Floor)}, //Boss buttons
            { "992", new ButtonData(196, new List<string>(){ "1004", "1000", "1001", "1002" }, ButtonType.Floor)}, //Boss buttons
            { "1006", new ButtonData(196, new List<string>(){ "1004", "1000", "1001", "1002" }, ButtonType.Floor)}, //Boss buttons
            { "1011", new ButtonData(196, new List<string>(){ "1004", "1000", "1001", "1002" }, ButtonType.Floor)}, //Boss buttons
            { "502", new ButtonData(69, new List<string>(){ "503", "494" }, ButtonType.Floor, "495")}, //Tp nw pyramid item
            { "898", new ButtonData(70, new List<string>(){ "785" }, ButtonType.Floor, "1167")}, //Tp blue spikes item
            { "1170", new ButtonData(71, new List<string>(){ "300" }, ButtonType.Floor, "1169")}, //Open bonus portal
            { "2458", new ButtonData(72, new List<string>(){ "2459", "3272" }, ButtonType.Floor, "3271")}, //Tp ne pyramid item
            { "2721", new ButtonData(73, new List<string>(){ "2731", "2604" }, ButtonType.Floor, "2607")}, //Tp sw pyramid item
            { "2730", new ButtonData(74, new List<string>(){ "2732", "2728" }, ButtonType.Floor, "2727")}, //Tp se pyramid item
            { "4717", new ButtonData(75, new List<string>(){ "4711" }, ButtonType.Floor, "4714")}, //Tp ne
            { "6022", new ButtonData(76, new List<string>(){ "6015" }, ButtonType.Floor, "6019")}, //Tp sw
            { "7724", new ButtonData(77, new List<string>(){ "7664" }, ButtonType.Floor, "7723")}, //Tp se
            { "3115", new ButtonData(80, new List<string>(){ "4160" }, ButtonType.Wall, "3117")}, //Open west exits hall
            { "4155", new ButtonData(81, new List<string>(){ "6382" }, ButtonType.Wall, "4159")}, //Open start shortcut
            { "5306", new ButtonData(82, new List<string>(){ "5309" }, ButtonType.Wall, "5308")}, //Open se room left
            { "5311", new ButtonData(83, new List<string>(){ "5312" }, ButtonType.Wall, "5100")}, //Open se room top
            }},
            {"level_bonus_2.xml", new Dictionary<string, ButtonData>{
            { "20", new ButtonData(84, new List<string>(){ "22" }, ButtonType.Floor, "312")}, //Disable armory floor 5 blue spikes
            { "1026", new ButtonData(85, new List<string>(){ "1025" }, ButtonType.Panel)}, //Open se room
            { "1024", new ButtonData(86, new List<string>(){ "599" }, ButtonType.Panel)}, //Open ne room
            { "311", new ButtonData(87, new List<string>(){ "127" }, ButtonType.Panel)}, //Open exit
            }},
            {"level_6.xml", new Dictionary<string, ButtonData>{
            { "2084", new ButtonData(197, new List<string>(){ "2086", "2135", "2136", "2137" }, ButtonType.Floor)}, //Boss buttons
            { "2130", new ButtonData(197, new List<string>(){ "2086", "2135", "2136", "2137" }, ButtonType.Floor)}, //Boss buttons
            { "2087", new ButtonData(197, new List<string>(){ "2086", "2135", "2136", "2137" }, ButtonType.Floor)}, //Boss buttons
            { "2141", new ButtonData(197, new List<string>(){ "2086", "2135", "2136", "2137" }, ButtonType.Floor)}, //Boss buttons
            { "1073", new ButtonData(89, new List<string>(){ "1305", "1077", "1301", "1302", "1304" }, ButtonType.Floor)}, //Open knife reward
            { "1078", new ButtonData(89, new List<string>(){ "1305", "1077", "1301", "1302", "1304" }, ButtonType.Floor)}, //Open knife reward
            { "1318", new ButtonData(89, new List<string>(){ "1305", "1077", "1301", "1302", "1304" }, ButtonType.Floor)}, //Open knife reward
            { "1324", new ButtonData(89, new List<string>(){ "1305", "1077", "1301", "1302", "1304" }, ButtonType.Floor)}, //Open knife reward
            { "1330", new ButtonData(89, new List<string>(){ "1305", "1077", "1301", "1302", "1304" }, ButtonType.Floor)}, //Open knife reward
            { "3627", new ButtonData(90, new List<string>(){ "3623" }, ButtonType.Floor, "3628")}, //Tp middle
            { "3902", new ButtonData(91, new List<string>(){ "4364" }, ButtonType.Floor, "3901")}, //Tp middle bronze gate
            { "1075", new ButtonData(93, new List<string>(){ "1335", "1336" }, ButtonType.Wall, "1076")}, //Open knife reward 2
            { "1308", new ButtonData(93, new List<string>(){ "1335", "1336" }, ButtonType.Wall, "1300")}, //Open knife reward 2
            { "4367", new ButtonData(94, new List<string>(){ "3907" }, ButtonType.Wall, "3906", new Vector2(-18.5f, 9.5f))}, //Open middle stairs
            { "5757", new ButtonData(95, new List<string>(){ "3313" }, ButtonType.Wall, "5759", new Vector2(-24.75f, 36.5f))}, //Open start north
            { "5764", new ButtonData(96, new List<string>(){ "5766" }, ButtonType.Wall, "5762")}, //Open start east
            }},
            {"level_boss_2.xml", new Dictionary<string, ButtonData>{
            }},
            {"level_7.xml", new Dictionary<string, ButtonData>{
            { "247", new ButtonData(97, new List<string>(){ "2181" }, ButtonType.Floor, "245")}, //Open west wall
            { "1536", new ButtonData(98, new List<string>(){ "1334" }, ButtonType.Floor, "1534")}, //Open north wall
            { "2178", new ButtonData(99, new List<string>(){ "696" }, ButtonType.Floor, "2179")}, //Open nw room
            { "3796", new ButtonData(100, new List<string>(){ "3797" }, ButtonType.Floor, "3944")}, //Open m ledge
            { "4166", new ButtonData(101, new List<string>(){ "6272" }, ButtonType.Floor, "4360")}, //Open left exit bottom
            { "4361", new ButtonData(102, new List<string>(){ "4357" }, ButtonType.Floor, "4364")}, //Open start closed room
            { "5475", new ButtonData(103, new List<string>(){ "5316" }, ButtonType.Floor, "5473")}, //Open east wall
            { "6278", new ButtonData(104, new List<string>(){ "6274" }, ButtonType.Floor, "6276")}, //Open exits
            { "1539", new ButtonData(105, new List<string>(){ "1538" }, ButtonType.Wall, "1542", new Vector2(14.5f, -43.5f))}, //Open ne shortcut
            { "2576", new ButtonData(106, new List<string>(){ "2579" }, ButtonType.Wall, "2184", new Vector2(-34f, -8.5f))}, //Open west shortcut
            { "5174", new ButtonData(107, new List<string>(){ "5482" }, ButtonType.Wall, "5176", new Vector2(63.5f, 5.5f))}, //Open se room
            }},
            {"level_8.xml", new Dictionary<string, ButtonData>{
            { "847", new ButtonData(199, new List<string>(){ "853", "852", "850", "851" }, ButtonType.Floor)}, //Boss buttons
            { "845", new ButtonData(199, new List<string>(){ "853", "852", "850", "851" }, ButtonType.Floor)}, //Boss buttons
            { "856", new ButtonData(199, new List<string>(){ "853", "852", "850", "851" }, ButtonType.Floor)}, //Boss buttons
            { "861", new ButtonData(199, new List<string>(){ "853", "852", "850", "851" }, ButtonType.Floor)}, //Boss buttons
            { "4566", new ButtonData(198, new List<string>(){ "4038", "4039", "4040", "4041" }, ButtonType.Floor)}, //Boss buttons
            { "4572", new ButtonData(198, new List<string>(){ "4038", "4039", "4040", "4041" }, ButtonType.Floor)}, //Boss buttons
            { "4574", new ButtonData(198, new List<string>(){ "4038", "4039", "4040", "4041" }, ButtonType.Floor)}, //Boss buttons
            { "4578", new ButtonData(198, new List<string>(){ "4038", "4039", "4040", "4041" }, ButtonType.Floor)}, //Boss buttons
            { "1308", new ButtonData(108, new List<string>(){ "172" }, ButtonType.Floor, "841")}, //Open boss switch room left
            { "1015", new ButtonData(109, new List<string>(){ "1016" }, ButtonType.Floor, "1014")}, //Open boss switch room right
            { "1763", new ButtonData(110, new List<string>(){ "2361" }, ButtonType.Floor, "1761")}, //Open fire trap room top
            { "3467", new ButtonData(111, new List<string>(){ "4183" }, ButtonType.Floor, "3466")}, //Open left exit
            { "7087", new ButtonData(112, new List<string>(){ "7790" }, ButtonType.Floor, "4777")}, //Open south area right side
            { "5546", new ButtonData(113, new List<string>(){ "5393" }, ButtonType.Floor)}, //Open fire trap room bottom
            { "6108", new ButtonData(114, new List<string>(){ "6112" }, ButtonType.Floor, "6110")}, //Open south area left side
            { "1312", new ButtonData(117, new List<string>(){ "3781" }, ButtonType.Wall, "1311", new Vector2(-15.5f, -30.5f))}, //Open nw shortcut
            { "4349", new ButtonData(118, new List<string>(){ "4347" }, ButtonType.Wall, "4351", new Vector2(-4.5f, 0.5f))}, //Open start
            { "7814", new ButtonData(119, new List<string>(){ "7809" }, ButtonType.Wall, "7812", new Vector2(-9.5f, 61.5f))}, //Open spike turret island top shortcut
            { "7824", new ButtonData(120, new List<string>(){ "6547" }, ButtonType.Wall)}, //Activate light bridge
            { "8570", new ButtonData(121, new List<string>(){ "2156" }, ButtonType.Wall, "8567", new Vector2(92, -54.5f))}, //Open ne cache
            { "6246", new ButtonData(122, new List<string>(){ "7589" }, ButtonType.Wall, "6243", new Vector2(-36.5f, 48.5f))}, //Open spike turret island left shortcut
            }},
            {"level_9.xml", new Dictionary<string, ButtonData>{
            { "12605", new ButtonData(200, new List<string>(){ "12608", "12606", "12607", "12611" }, ButtonType.Floor)}, //Boss buttons
            { "12603", new ButtonData(200, new List<string>(){ "12608", "12606", "12607", "12611" }, ButtonType.Floor)}, //Boss buttons
            { "12944", new ButtonData(200, new List<string>(){ "12608", "12606", "12607", "12611" }, ButtonType.Floor)}, //Boss buttons
            { "12942", new ButtonData(200, new List<string>(){ "12608", "12606", "12607", "12611" }, ButtonType.Floor)}, //Boss buttons
            { "4854", new ButtonData(123, new List<string>(){ "4861" }, ButtonType.Floor, "4859")}, //Open boss gate, 200 delay
            { "4530", new ButtonData(124, new List<string>(){ "4532" }, ButtonType.Floor, "1022", new Vector2(-88.5f, -30f))}, //Open secret passage north room
            { "1039", new ButtonData(125, new List<string>(){ "1042" }, ButtonType.Floor, "1036")}, //Tp nw item
            { "2227", new ButtonData(126, new List<string>(){ "4873" }, ButtonType.Floor, "2225")}, //Open exit
            { "3042", new ButtonData(127, new List<string>(){ "3775" }, ButtonType.Floor, "3407")}, //Open ne room top side
            { "1027", new ButtonData(128, new List<string>(){ "1024" }, ButtonType.Floor, "4537")}, //Disable secret passage north room spikes
            { "6490", new ButtonData(129, new List<string>(){ "2653" }, ButtonType.Floor, "6693")}, //Open ne room left side
            { "9986", new ButtonData(130, new List<string>(){ "9982" }, ButtonType.Floor, "9985")}, //Open south room top side
            { "11392", new ButtonData(131, new List<string>(){ "11395" }, ButtonType.Floor, "11393")}, //Open south room right side
            { "12199", new ButtonData(132, new List<string>(){ "12197" }, ButtonType.Floor, "12201")}, //Open boss switch room
            { "1033", new ButtonData(138, new List<string>(){ "1032" }, ButtonType.Wall, "4540", new Vector2(-76f, -26.5f))}, //Open secret passage end
            { "4977", new ButtonData(139, new List<string>(){ "5208" }, ButtonType.Wall, "5207", new Vector2(-70f, 2.5f))}, //Open secret passage south room
            { "8142", new ButtonData(140, new List<string>(){ "7950", "7949" }, ButtonType.Wall, "8141", new Vector2(54f, -26.5f))}, //Tp ne top item
            { "8149", new ButtonData(141, new List<string>(){ "7956" }, ButtonType.Wall, "8147", new Vector2(53f, -26.5f))}, //Tp ne bottom item
            { "9643", new ButtonData(144, new List<string>(){ "9640" }, ButtonType.Wall, "9644")}, //Open sw secret room
            { "9345", new ButtonData(145, new List<string>(){ "9342" }, ButtonType.Wall, "9344", new Vector2(-84.5f, 44.5f))}, //Open sw secret passage
            { "11029", new ButtonData(143, new List<string>(), ButtonType.Wall, null, new Vector2(19.25f, 39.5f))}, //Open simon says room
            { "8871", new ButtonData(143, new List<string>(), ButtonType.Wall)}, //Open simon says room
            { "8876", new ButtonData(143, new List<string>(), ButtonType.Wall, null, new Vector2(39.75f, 13.5f))}, //Open simon says room
            { "9096", new ButtonData(143, new List<string>(), ButtonType.Wall, null, new Vector2(56.5f, 25.5f))}, //Open simon says room
            { "12473", new ButtonData(143, new List<string>(), ButtonType.Wall, null, new Vector2(45.625f, 52.5f))}, //Open simon says room
            }},
            {"level_bonus_3.xml", new Dictionary<string, ButtonData>{
            }},
            {"level_boss_3.xml", new Dictionary<string, ButtonData>{
            }},
            {"level_10.xml", new Dictionary<string, ButtonData>{
            { "1184", new ButtonData(146, new List<string>(){ "1495" }, ButtonType.Floor)}, //Disable north spikes
            { "3822", new ButtonData(147, new List<string>(){ "6124" }, ButtonType.Floor)}, //Disable red spikes
            { "1641", new ButtonData(148, new List<string>(){ "1640" }, ButtonType.Wall, "1643")}, //Disable red spike turrets
            { "3087", new ButtonData(149, new List<string>(){ "3083" }, ButtonType.Wall, "3085")}, //Disable red flame turrets
            { "5442", new ButtonData(150, new List<string>(){ "5445" }, ButtonType.Wall, "5444", new Vector2(-19.625f, 41.5f))}, //Open south shortcut
            { "4905", new ButtonData(151, new List<string>(){ "4908" }, ButtonType.Wall, "4907", new Vector2(-47.5f, 40.25f))}, //Open sw exit
            }},
            {"level_10_special.xml", new Dictionary<string, ButtonData>{
                //Puzzle created in EditLevels
            }},
            {"level_11.xml", new Dictionary<string, ButtonData>{
            { "3640", new ButtonData(202, new List<string>(){ "3644", "3643", "3940", "3934" }, ButtonType.Floor)}, //Boss buttons
            { "3938", new ButtonData(202, new List<string>(){ "3644", "3643", "3940", "3934" }, ButtonType.Floor)}, //Boss buttons
            { "3645", new ButtonData(202, new List<string>(){ "3644", "3643", "3940", "3934" }, ButtonType.Floor)}, //Boss buttons
            { "3942", new ButtonData(202, new List<string>(){ "3644", "3643", "3940", "3934" }, ButtonType.Floor)}, //Boss buttons
            { "6361", new ButtonData(201, new List<string>(){ "6368", "6364", "6365", "6366" }, ButtonType.Floor)}, //Boss buttons
            { "6358", new ButtonData(201, new List<string>(){ "6368", "6364", "6365", "6366" }, ButtonType.Floor)}, //Boss buttons
            { "6377", new ButtonData(201, new List<string>(){ "6368", "6364", "6365", "6366" }, ButtonType.Floor)}, //Boss buttons
            { "6375", new ButtonData(201, new List<string>(){ "6368", "6364", "6365", "6366" }, ButtonType.Floor)}, //Boss buttons
            { "2485", new ButtonData(154, new List<string>(){ "2351" }, ButtonType.Floor, "2486")}, //Open sw secret room
            { "3650", new ButtonData(155, new List<string>(){ "1355" }, ButtonType.Floor, "3648")}, //Open north shops room
            { "3943", new ButtonData(156, new List<string>(){ "6697" }, ButtonType.Floor, "3946")}, //Open west shortcut
            { "1369", new ButtonData(163, new List<string>(){ "1367" }, ButtonType.Wall, "1365")}, //Open north exit closed room
            { "4723", new ButtonData(164, new List<string>(){ "4727" }, ButtonType.Wall, null, new Vector2(20.5f, -57.5f))}, //Activate north red flame turret
            { "4729", new ButtonData(165, new List<string>(){ "5623" }, ButtonType.Wall)}, //Disable blue spike turret
            { "7710", new ButtonData(166, new List<string>(){ "10306" }, ButtonType.Wall, null, new Vector2(8.5f, -14.5f))}, //Open chance puzzle room
            { "7708", new ButtonData(167, new List<string>(){ "7623" }, ButtonType.Wall, "7707", new Vector2(13.75f, -25.5f))}, //Open east shop shortcut
            { "10197", new ButtonData(168, new List<string>(){ "10201" }, ButtonType.Wall, "10199")}, //Disable south red flame turrets
            { "10304", new ButtonData(169, new List<string>(){ "10300" }, ButtonType.Wall, "10303", new Vector2(5f, 44.5f))}, //Open south entrance shortcut
            { "2597", new ButtonData(162, new List<string>(), ButtonType.Wall)}, //Open bonus room sequence
            { "375", new ButtonData(162, new List<string>(), ButtonType.Wall)}, //Open bonus room sequence
            { "993", new ButtonData(162, new List<string>(), ButtonType.Wall)}, //Open bonus room sequence
            { "4231", new ButtonData(162, new List<string>(), ButtonType.Wall)}, //Open bonus room sequence
            { "3950", new ButtonData(162, new List<string>(), ButtonType.Wall)}, //Open bonus room sequence
            }},
            {"level_bonus_4.xml", new Dictionary<string, ButtonData>{
            }},
            {"level_12.xml", new Dictionary<string, ButtonData>{
            { "2316", new ButtonData(203, new List<string>(){ "2317", "2321", "2534", "2536" }, ButtonType.Floor)}, //Boss buttons
            { "2533", new ButtonData(203, new List<string>(){ "2317", "2321", "2534", "2536" }, ButtonType.Floor)}, //Boss buttons
            { "2315", new ButtonData(203, new List<string>(){ "2317", "2321", "2534", "2536" }, ButtonType.Floor)}, //Boss buttons
            { "2874", new ButtonData(203, new List<string>(){ "2317", "2321", "2534", "2536" }, ButtonType.Floor)}, //Boss buttons
            { "5398", new ButtonData(170, new List<string>(){ "4771" }, ButtonType.Floor, "5401")}, //Open east shortcut
            { "2088", new ButtonData(173, new List<string>(){ "31" }, ButtonType.Wall, "2091", new Vector2(72.5f, -84.5f))}, //Disable red spikes
            { "2093", new ButtonData(174, new List<string>(){ "2062" }, ButtonType.Wall, "2060", new Vector2(71f, -74.5f))}, //Activate blue flame turrets
            { "4347", new ButtonData(175, new List<string>(){ "4566" }, ButtonType.Wall, "4346", new Vector2(-19.375f, -2.5f))}, //Tp middle button
            { "5257", new ButtonData(176, new List<string>(){ "5261" }, ButtonType.Wall, "5259", new Vector2(-7.5f, 24.5f))}, //Disable red spike turrets
            { "5393", new ButtonData(177, new List<string>(){ "8057" }, ButtonType.Wall, "8050")}, //Tp fire floor item
            { "8497", new ButtonData(178, new List<string>(){ "8495" }, ButtonType.Wall)}, //Open west secret room
            { "7120", new ButtonData(179, new List<string>(){ "7123" }, ButtonType.Wall, "7122", new Vector2(-65.5f, 65.5f))}, //Open s hall
            { "7406", new ButtonData(172, new List<string>(), ButtonType.Floor)}, //Open sw hall sequence
            { "917", new ButtonData(172, new List<string>(), ButtonType.Floor)},
            { "2873", new ButtonData(172, new List<string>(), ButtonType.Floor)},
            { "5828", new ButtonData(172, new List<string>(), ButtonType.Floor)},
            { "5410", new ButtonData(172, new List<string>(), ButtonType.Floor)},
            { "3327", new ButtonData(172, new List<string>(), ButtonType.Floor)},
            }},
            {"level_boss_4.xml", new Dictionary<string, ButtonData>{
            }},
            //{"level_esc_1.xml", new Dictionary<string, ButtonData>{
            //}},
            //{"level_esc_2.xml", new Dictionary<string, ButtonData>{
            //}},
            //{"level_esc_3.xml", new Dictionary<string, ButtonData>{
            //}},
            //{"level_esc_4.xml", new Dictionary<string, ButtonData>{
            //}},
        };
        public static Dictionary<string, Dictionary<string, ButtonData>> templeButtonNodes = new Dictionary<string, Dictionary<string, ButtonData>>()
        {
            {"level_cave_1.xml", new Dictionary<string, ButtonData>{
            { "138828", new ButtonData(14, new List<string>(){"138823"}, ButtonType.Floor)},
            { "105264", new ButtonData(15, new List<string>(){"105163"}, ButtonType.Floor)},
            }},
            {"level_cave_2.xml", new Dictionary<string, ButtonData>{
            { "105264", new ButtonData(16, new List<string>(){"105163"}, ButtonType.Floor)},
            { "112532", new ButtonData(17, new List<string>(){"112534"}, ButtonType.Floor)},
            { "110673", new ButtonData(18, new List<string>(){"110674"}, ButtonType.Floor)},
            { "107538", new ButtonData(19, new List<string>(){"107535"}, ButtonType.Floor)},
            //{ "110673", new ButtonData(20, new List<string>(){"110674"}, ButtonType.Lever)},
            }},
            {"level_cave_3.xml", new Dictionary<string, ButtonData>{
            { "114017", new ButtonData(21, new List<string>(){"114019"}, ButtonType.Floor)},
            { "118859", new ButtonData(21, new List<string>(){"118857"}, ButtonType.Floor)},
            { "122690", new ButtonData(22, new List<string>(){"122688"}, ButtonType.Floor)},
            { "122694", new ButtonData(22, new List<string>(){"122696"}, ButtonType.Floor)},
            { "156092", new ButtonData(23, new List<string>(){"156090"}, ButtonType.Floor)},
            { "123747", new ButtonData(24, new List<string>(){"122747"}, ButtonType.Wall, null, new Vector2(56.25f, -86.5f))},
            }},
            {"level_boss_1.xml", new Dictionary<string, ButtonData>{
            { "141584", new ButtonData(25, new List<string>(){"141586"}, ButtonType.Floor)},
            }},
            {"level_temple_1.xml", new Dictionary<string, ButtonData>{
            { "135037", new ButtonData(26, new List<string>(){"135036"}, ButtonType.Wall, "135035")}, //Need to unhook 135035
            { "134974", new ButtonData(27, new List<string>(){"134973"}, ButtonType.Wall, "134972")}, //134972
            //{ "134974", new ButtonData(28, new List<string>(){"134973"}, ButtonType.Wall)}, //Telarian gate
            //{ "134974", new ButtonData(29, new List<string>(){"134973"}, ButtonType.Wall)}, //Guard gate
            { "131835", new ButtonData(30, new List<string>(){"131845"}, ButtonType.Wall, null, new Vector2(83.5f, -38.5f))},
            { "127727", new ButtonData(31, new List<string>(){"127609"}, ButtonType.Wall)},
            }},
            {"level_temple_2.xml", new Dictionary<string, ButtonData>{
            { "144966", new ButtonData(32, new List<string>(){"144969"}, ButtonType.Floor, "144968")}, //144968
            { "141335", new ButtonData(63, new List<string>(){"141392", "141394", "141396", "141395", "150271"}, ButtonType.Floor)}, //Do not create item effect nodes
            { "141265", new ButtonData(63, new List<string>(){"141392", "141394", "141396", "141395", "150271"}, ButtonType.Floor)},
            { "141320", new ButtonData(63, new List<string>(){"141392", "141394", "141396", "141395", "150271"}, ButtonType.Floor)},
            { "141322", new ButtonData(63, new List<string>(){"141392", "141394", "141396", "141395", "150271"}, ButtonType.Floor)},
            { "150275", new ButtonData(63, new List<string>(){"141392", "141394", "141396", "141395", "150271"}, ButtonType.Floor)},
            { "135651", new ButtonData(34, new List<string>(){"135647"}, ButtonType.Wall, null, new Vector2(-47.5f, -73.5f))},
            { "135891", new ButtonData(35, new List<string>(){"135886"}, ButtonType.Wall, null, new Vector2(-26, -51.5f))},
            { "144295", new ButtonData(36, new List<string>(){"144297"}, ButtonType.Wall, "144300")}, //144300
            { "149062", new ButtonData(37, new List<string>(){"149064"}, ButtonType.Wall, "149067")}, //149067
            { "150013", new ButtonData(38, new List<string>(){"150009"}, ButtonType.Wall, "150008")}, //150008
            { "135631", new ButtonData(39, new List<string>(){"135624"}, ButtonType.Wall, null, new Vector2(-58, -24.5f))},
            { "150361", new ButtonData(40, new List<string>(){"150356"}, ButtonType.Wall, "150355")}, //150355
            { "150369", new ButtonData(41, new List<string>(){"150364"}, ButtonType.Wall, "150367")}, //150367
            { "135945", new ButtonData(42, new List<string>(){"135604"}, ButtonType.Wall, null, new Vector2(-5.5f, -29.5f))},
            { "143574", new ButtonData(64, new List<string>(){"143586", "143590"}, ButtonType.Wall)},
            { "143578", new ButtonData(64, new List<string>(){"143586", "143590"}, ButtonType.Wall, null, new Vector2(-8.5f, 14.5f))},
            { "140971", new ButtonData(44, new List<string>(){"140973"}, ButtonType.Wall, "140976")}, //140976
            { "138308", new ButtonData(45, new List<string>(){"138310"}, ButtonType.Wall, "138313")}, //138313
            { "3064", new ButtonData(46, new List<string>(){"3065"}, ButtonType.Wall, "3067")}, //3067
            { "146321", new ButtonData(47, new List<string>(){"146317"}, ButtonType.Wall, "146316")}, //146316
            { "147757", new ButtonData(48, new List<string>(){"147755"}, ButtonType.Wall)},
            { "3071", new ButtonData(49, new List<string>(){"3123"}, ButtonType.Wall)},
            { "149216", new ButtonData(50, new List<string>(){"149212"}, ButtonType.Floor, "149211")}, //149211
            }},
            {"level_temple_3.xml", new Dictionary<string, ButtonData>{
            { "153147", new ButtonData(52, new List<string>(){"153148"}, ButtonType.Wall, null, new Vector2(13.25f, 3.5f))},
            { "153760", new ButtonData(53, new List<string>(){"153753"}, ButtonType.Wall)},
            { "153121", new ButtonData(54, new List<string>(){"153125"}, ButtonType.Wall, null, new Vector2(-20.375f, 1.5f))},
            { "138308", new ButtonData(55, new List<string>(){"138310"}, ButtonType.Wall, "138313")},
            { "138456", new ButtonData(56, new List<string>(){"138453"}, ButtonType.Wall, "138454")},
            //{ "149241", new ButtonData(65, new List<string>(), ButtonType.Lever)},
            //{ "149242", new ButtonData(65, new List<string>(), ButtonType.Lever)},
            //{ "149243", new ButtonData(65, new List<string>(), ButtonType.Lever)},
            //{ "148142", new ButtonData(65, new List<string>(), ButtonType.Lever)},
            }},
            {"level_bonus_5.xml", new Dictionary<string, ButtonData>{
            { "86381", new ButtonData(57, new List<string>(){"86367"}, ButtonType.Panel)}, //Technically have no delay
            { "86441", new ButtonData(58, new List<string>(){"86448"}, ButtonType.Panel)},
            { "87670", new ButtonData(59, new List<string>(){"87672"}, ButtonType.Panel)},
            { "87632", new ButtonData(60, new List<string>(){"87634"}, ButtonType.Panel)},
            { "87991", new ButtonData(61, new List<string>(){"87990"}, ButtonType.Panel)},
            }},
        };
        public struct ButtonData
        {
            public int itemId;
            public List<string> buttonEffectNodes;
            public ButtonType buttonType;
            public string unhookNode;
            public Vector2 posOverride;

            public ButtonData(int itemId, List<string> effectNodes, ButtonType buttonType)
            {
                this.itemId = itemId;
                buttonEffectNodes = effectNodes;
                this.buttonType = buttonType;
                unhookNode = null;
                posOverride = Vector2.Zero;
            }
            public ButtonData(int itemId, List<string> effectNodes, ButtonType buttonType, string unhookNode)
            {
                this.itemId = itemId;
                buttonEffectNodes = effectNodes;
                this.buttonType = buttonType;
                this.unhookNode = unhookNode;
                posOverride = Vector2.Zero;
            }
            public ButtonData(int itemId, List<string> effectNodes, ButtonType buttonType, string unhookNode, Vector2 positionOverride)
            {
                this.itemId = itemId;
                buttonEffectNodes = effectNodes;
                this.buttonType = buttonType;
                this.unhookNode = unhookNode;
                posOverride = positionOverride;
            }
        }
        public enum ButtonType
        {
            Floor,
            Wall,
            Lever,
            Panel,
        }

        //Breakable wall data
        public static Dictionary<ArchipelagoData.MapType, Dictionary<string, List<string>>> breakableWallNodes = new Dictionary<ArchipelagoData.MapType, Dictionary<string, List<string>>>()
        {
            {ArchipelagoData.MapType.Castle, new Dictionary<string, List<string>>
            {
                {"level_1.xml", new List<string>(){ "426", "813", "2031" } },
                {"level_2.xml", new List<string>(){ "496", "3647", "3664", "543" } },
                {"level_3.xml", new List<string>(){ "413", "7453", "4056", "6684" } },
                {"level_4.xml", new List<string>(){ "681", "3442" } },
                {"level_5.xml", new List<string>(){ "4419", "4710" } },
                {"level_6.xml", new List<string>(){ "2252" } },
                {"level_7.xml", new List<string>(){ "3438" } },
                {"level_8.xml", new List<string>(){ "2364", "6360", "2149" } },
                {"level_9.xml", new List<string>(){ "8861", "9356", "9372", "4983", "3403", "1876", "6016" } },
                {"level_12.xml", new List<string>(){ "2920" } },
                {"level_esc_4.xml", new List<string>(){ "868" } },
                {"level_bonus_1.xml", new List<string>(){ "186", "201" } },
            }},
            {ArchipelagoData.MapType.Temple, new Dictionary<string, List<string>>
            {
                {"level_cave_1.xml", new List<string>(){ "135758", "104284", "106167", "156254" } },
                {"level_cave_2.xml", new List<string>(){ "105512", "140826", "109951", "107692" } },
                {"level_cave_3.xml", new List<string>(){ "136111", "116965", "124450" } },
                {"level_passage.xml", new List<string>(){ "126996" } },
                {"level_temple_1.xml", new List<string>(){ "134191", "134160" } },
                {"level_temple_2.xml", new List<string>(){ "136437", "136460", "136476", "136421" } },
                {"level_bonus_5.xml", new List<string>(){ "243", "86227" } },
            }}
        };

        public static bool IsItemUnique(long itemId)
        {
            long relId = itemId - templeStartID;
            //Tool items
            switch (relId)
            {
                case 45:
                case 46:
                case 47:
                case 51:
                    return true;
            }
            if (IsItemButton(itemId) && itemId - templeButtonItemStartID != 0)
            {
                return true;
            }
            return false;
        }
        public static bool IsItemButton(long itemId)
        {
            return (itemId >= castleButtonItemStartID && itemId <= castleButtonItemStartID + 203)
                || (itemId >= templeButtonItemStartID && itemId <= templeButtonItemStartID + 65);
        }
        public static bool IsItemShopUpgrade(long itemId)
        {
            long relId = itemId - templeStartID - shopItemOffset;
            return relId >= 0 && relId < 146;
        }
        public static bool IsItemFloorMasterKey(long itemId)
        {
            return itemId >= templeStartID + 545 && itemId <= templeStartID + 591;
        }

        public class ShopItemData
        {
            public string shopType;
            public string[] shopItemIds;

            public ShopItemData(string shopType, string[] shopItemIds)
            {
                this.shopType = shopType;
                this.shopItemIds = shopItemIds;
            }
        }
        public static string GetShortShopTypeFromAPId(int shopId)
        {
            string longType = shopTypeCodeToString[shopId / 10];
            switch (longType)
            {
                case "offense":
                case "defense":
                    longType = longType.Substring(0, 3);
                    break;
            }
            return longType;
        }
        public static string GetShopPrefabSuffixFromAPId(int shopId)
        {
            return shopTypeCodeToString[shopId / 10];
        }
        public static int GetShopLevelFromAPId(int shopId)
        {
            return shopId % 10;
        }
        private static Dictionary<int, string> shopTypeCodeToString = new Dictionary<int, string>()
        {
            { 0, "misc" },
            { 1, "combo" },
            { 2, "offense" },
            { 3, "defense" },
            { 4, "power" },
            { 5, "gambler" },
        };
    }
}
