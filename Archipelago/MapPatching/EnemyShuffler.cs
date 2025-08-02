using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using HammerwatchAP.Util;

namespace HammerwatchAP.Archipelago
{
    public static class EnemyShuffler
    {
        private static Random random;

        public static Dictionary<ActorType, Dictionary<string, int>[,]> actorTypeActTierCounts;
        public static Dictionary<ActorType, List<EnemyNodeData>[,]> enemyNodeData;
        public static Dictionary<string, XElement> levelActorRootNodes;

        public static Dictionary<string, string> groupSwapMapping;

        private static readonly Dictionary<string, List<string>> levelExcludedActors = new Dictionary<string, List<string>>()
        {
            {"level_bonus_1.xml" , new List<string>(){ "17 20.5" } },
        };

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

        public static int GetEnemyTier(string enemyName, bool enemyShuffleKeepTier)
        {
            if (!enemyTiers.TryGetValue(enemyName, out int enemyTier) || !enemyShuffleKeepTier)
            {
                enemyTier = 0;
            }
            return enemyTier;
        }

        public static void SetRandomSeed()
        {
            random = new Random();
        }
        public static void SetRandomSeed(int seed)
        {
            random = new Random(seed);
        }

        private static void InitDictionaries()
        {
            if (random == null)
                SetRandomSeed();

            actorTypeActTierCounts = new Dictionary<ActorType, Dictionary<string, int>[,]>();
            enemyNodeData = new Dictionary<ActorType, List<EnemyNodeData>[,]>();
            levelActorRootNodes = new Dictionary<string, XElement>();

            groupSwapMapping = new Dictionary<string, string>();

            ActorType[] actorTypesToCount = new ActorType[] { ActorType.Enemy, ActorType.Spawner, ActorType.Tower, ActorType.Miniboss };
            foreach (ActorType actorType in actorTypesToCount)
            {
                int tiersInDict = 1;
                if (actorType == ActorType.Enemy)
                    tiersInDict = ENEMY_TIERS;
                actorTypeActTierCounts[actorType] = new Dictionary<string, int>[4, tiersInDict];
                enemyNodeData[actorType] = new List<EnemyNodeData>[4, tiersInDict];
                for (int a = 0; a < actorTypeActTierCounts[actorType].GetLength(0); a++)
                {
                    for (int t = 0; t < tiersInDict; t++)
                    {
                        actorTypeActTierCounts[actorType][a, t] = new Dictionary<string, int>();
                        enemyNodeData[actorType][a, t] = new List<EnemyNodeData>();
                    }
                }
            }
        }

        public static void CountEnemies(Dictionary<string, XDocument> docs, ArchipelagoData archipelagoData)
        {
            InitDictionaries();

            int enemyShuffle = archipelagoData.GetOption(SlotDataKeys.enemyShuffleMode);
            if (enemyShuffle == 0)
                return;
            int actRange = archipelagoData.GetOption(SlotDataKeys.enemyShuffleActRange);
            bool enemyShuffleKeepTier = archipelagoData.GetOption(SlotDataKeys.enemyShuffleKeepTier) > 0;

            foreach (var pair in docs)
            {
                string levelName = Path.GetFileName(pair.Key);

                levelActorRootNodes[levelName] = pair.Value.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "actors");
                XElement scriptNodeRoot = pair.Value.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "scripting");
                XElement[] enemyGroups = levelActorRootNodes[levelName].Elements().ToArray();
                XElement[] scriptNodes = scriptNodeRoot.Element("array").Elements().ToArray();
                Dictionary<string, int> actors = new Dictionary<string, int>();
                int actIndex = APData.GetActFromLevelFileName(levelName, archipelagoData) - 1;

                foreach (XElement group in enemyGroups)
                {
                    string fullGroupName = group.Attribute("name").Value;
                    ActorType actorType = GetActorType(fullGroupName);
                    if (APData.excludedActors.Contains(fullGroupName) || actorType == ActorType.Boss) continue;
                    if (!actors.ContainsKey(fullGroupName))
                        actors[fullGroupName] = 0;
                    XElement[] groupElements = group.Elements().ToArray();
                    actors[fullGroupName] += groupElements.Count();
                    int groupTier = GetEnemyTier(fullGroupName, enemyShuffleKeepTier);
                    foreach (XElement enemyPosNode in groupElements)
                        enemyNodeData[actorType][actIndex, groupTier].Add(new EnemyNodeData(enemyPosNode, false, fullGroupName, levelName, actIndex, groupTier));
                    group.Remove();
                }
                //Remove that one problematic spawner if this is the bonus level and enemy shuffle is on
                if (levelName == "level_bonus_1.xml")
                {
                    List<EnemyNodeData> bonus1EnemyNodes = enemyNodeData[ActorType.Spawner][APData.GetActFromLevelFileName(levelName, archipelagoData) - 1, 0];
                    for(int e = 0; e < bonus1EnemyNodes.Count; e++)
                    {
                        if(bonus1EnemyNodes[e].enemyNode.Element("vec2").Value == "17 20.5")
                        {
                            Logging.Debug("Replaced problematic spawner in bonus 1");
                            bonus1EnemyNodes.RemoveAt(e);
                            actors["actors/spawners/bonus/skeleton_1.xml"]--;
                            break;
                        }
                    }
                }
                //Script nodes
                foreach (XElement node in scriptNodes)
                {
                    string nodeType = node.Element("string").Value;
                    if (nodeType != "SpawnObject") continue;

                    string fullGroupName = node.Elements("string").ToArray()[1].Value;
                    if (!fullGroupName.StartsWith("actors/")) continue;

                    ActorType actorType = GetActorType(fullGroupName);
                    if (APData.excludedActors.Contains(fullGroupName) || actorType == ActorType.Boss) continue;
                    if (!actors.ContainsKey(fullGroupName))
                        actors[fullGroupName] = 0;
                    actors[fullGroupName] += 1;
                    int groupTier = GetEnemyTier(fullGroupName, enemyShuffleKeepTier);
                    enemyNodeData[actorType][actIndex, groupTier].Add(new EnemyNodeData(node, true, fullGroupName, levelName, actIndex, groupTier));
                }
                //Go through actors and classify them
                foreach (var actorPairs in actors)
                {
                    string fullGroupName = actorPairs.Key;
                    ActorType groupType = GetActorType(fullGroupName);

                    int enemyTier = GetEnemyTier(fullGroupName, enemyShuffleKeepTier);
                    if (!actorTypeActTierCounts[groupType][actIndex, enemyTier].ContainsKey(fullGroupName))
                        actorTypeActTierCounts[groupType][actIndex, enemyTier][fullGroupName] = 0;
                    actorTypeActTierCounts[groupType][actIndex, enemyTier][fullGroupName] += actorPairs.Value;
                }
            }
        }
        public static void ShuffleEnemies(int actRange, int shuffleMode, ArchipelagoData archipelagoData)
        {
            if (shuffleMode == 0)
                return;
            int acts = APData.GetActCount(archipelagoData);
            bool extraFirstActWeight = actRange > 0 && actRange < acts - 1;
            for (int a = 1; a <= 4; a++)
            {
                ShuffleActTierNodes(ActorType.Enemy, a, actRange, shuffleMode, extraFirstActWeight);
                ShuffleActTierNodes(ActorType.Spawner, a, actRange, shuffleMode, extraFirstActWeight);
                ShuffleActTierNodes(ActorType.Miniboss, a, actRange, shuffleMode, extraFirstActWeight);
                ShuffleActTierNodes(ActorType.Tower, a, actRange, shuffleMode, extraFirstActWeight);
                extraFirstActWeight = false;
            }
        }
        public static void ShuffleActTierNodes(ActorType actorType, int actIndex, int actRange, int shuffleMode, bool extraFirstActWeight)
        {
            Dictionary<string, Dictionary<string, XElement>> parentNodes = new Dictionary<string, Dictionary<string, XElement>>();
            int maxAct = Math.Min(actIndex + actRange - 1, 3);
            for (int a = 0; a <= maxAct; a++)
            {
                for (int t = 0; t < actorTypeActTierCounts[actorType].GetLength(1); t++)
                {
                    bool breakLoop = false;
                    while (enemyNodeData[actorType][a, t].Count > 0)
                    {
                        if (!PopRandomEnemyNodeData(actorType, maxAct, t, out EnemyNodeData enemyNode))
                            break;
                        //Choose name based off of shuffle mode
                        string enemyToSwapTo;
                        switch (shuffleMode)
                        {
                            case 1: //Individual
                                enemyToSwapTo = GetRandomAvailableEnemyType(actorType, a, actRange, t, shuffleMode == 1, extraFirstActWeight);
                                if (enemyToSwapTo == null)
                                {
                                    breakLoop = true;
                                    break;
                                }
                                break;
                            case 2: //Type
                                if (groupSwapMapping.TryGetValue(enemyNode.enemyType, out string existingEnemyToSwapTo))
                                {
                                    enemyToSwapTo = existingEnemyToSwapTo;
                                }
                                else
                                {
                                    enemyToSwapTo = GetRandomAvailableEnemyType(actorType, a, actRange, t, shuffleMode == 1, extraFirstActWeight);
                                    if (enemyToSwapTo == null)
                                    {
                                        breakLoop = true;
                                        break;
                                    }
                                    groupSwapMapping[enemyNode.enemyType] = enemyToSwapTo;
                                    for (int a1 = 0; a1 <= maxAct; a1++)
                                        actorTypeActTierCounts[actorType][a1, t].Remove(enemyToSwapTo);
                                }
                                break;
                            case 3: //Chaos
                                List<string> availableEnemyNames = actorTypeActTierCounts[actorType][a, t].Keys.ToList();
                                enemyToSwapTo = MiscHelper.RandomFromList(random, availableEnemyNames);
                                break;
                            default:
                                throw new NotImplementedException("Shuffle mode was not a supported value!");
                        }
                        if (breakLoop)
                        {
                            //If we can't swap the node with something we need to put it back to go to the next act
                            enemyNodeData[actorType][enemyNode.act, enemyNode.tier].Add(enemyNode);
                            break;
                        }
                        if (enemyNode.isScriptNode)
                        {
                            enemyNode.enemyNode.Elements("string").ToArray()[1].Value = enemyToSwapTo;
                        }
                        else
                        {
                            if (!parentNodes.ContainsKey(enemyNode.level))
                                parentNodes[enemyNode.level] = new Dictionary<string, XElement>();
                            if (!parentNodes[enemyNode.level].ContainsKey(enemyToSwapTo))
                            {
                                XElement actorGroupNode = new XElement("array");
                                actorGroupNode.SetAttributeValue("name", enemyToSwapTo);
                                levelActorRootNodes[enemyNode.level].Add(actorGroupNode);
                                parentNodes[enemyNode.level][enemyToSwapTo] = actorGroupNode;
                            }
                            parentNodes[enemyNode.level][enemyToSwapTo].Add(enemyNode.enemyNode);
                        }
                    }
                }
            }
        }

        private static string GetRandomAvailableEnemyType(ActorType actorType, int actIndex, int actRange, int tier, bool remove, bool extraFirstActWeight)
        {
            int totalWeight = 0;
            int minAct = extraFirstActWeight ? -1 : 0;
            int maxAct = Math.Min(actIndex + actRange, actorTypeActTierCounts[actorType].GetLength(0) - 1);
            for (int a = minAct; a <= maxAct; a++)
            {
                int aa = Math.Max(a, 0);
                Dictionary<string, int> actorCounts = actorTypeActTierCounts[actorType][aa, tier];
                foreach (string type in actorCounts.Keys)
                    totalWeight += actorCounts[type];
            }
            int randomValue = random.Next(totalWeight);
            for (int a = minAct; a <= maxAct; a++)
            {
                int aa = Math.Max(a, 0);
                Dictionary<string, int> actorCounts = actorTypeActTierCounts[actorType][aa, tier];
                foreach (string type in actorCounts.Keys)
                {
                    if (randomValue < actorCounts[type])
                    {
                        if (remove)
                            MiscHelper.ReduceCountFromDict(ref actorCounts, type);
                        return type;
                    }
                    randomValue -= actorCounts[type];
                }
            }
            return null;
        }
        private static bool PopRandomEnemyNodeData(ActorType actorType, int maxAct, int tier, out EnemyNodeData enemyData)
        {
            int totalWeight = 0;
            for (int a = 0; a <= maxAct; a++)
            {
                totalWeight += enemyNodeData[actorType][a, tier].Count;
            }
            int randomValue = random.Next(totalWeight);
            for (int a = 0; a <= maxAct; a++)
            {
                if (randomValue < enemyNodeData[actorType][a, tier].Count)
                {
                    enemyData = enemyNodeData[actorType][a, tier][randomValue];
                    enemyNodeData[actorType][a, tier].RemoveAt(randomValue);
                    return true;
                }
                randomValue -= enemyNodeData[actorType][a, tier].Count;
            }
            enemyData = default;
            return false;
        }

        private static ActorType GetActorType(string actorName)
        {
            if (actorName.StartsWith("actors/tower"))
            {
                return ActorType.Tower;
            }
            else if (actorName.StartsWith("actors/spawners/"))
            {
                return ActorType.Spawner;
            }
            else if (actorName.EndsWith("mb.xml"))
            {
                return ActorType.Miniboss;
            }
            else if (actorName.StartsWith("actors/boss"))
            {
                return ActorType.Boss;
            }
            return ActorType.Enemy;
        }
        public enum ActorType
        {
            Enemy,
            Spawner,
            Miniboss,
            Tower,
            Boss,
        }

        public class ActorCounter
        {
            public Dictionary<string, int>[,] enemyActTierCounts;
            public List<EnemyNodeData>[,] enemyActTierNodes;
            public Dictionary<string, XElement> levelActorRootNodes;

            public ActorCounter(int enemyTiers, Dictionary<string, XElement> levelActorRootNodes)
            {
                enemyActTierCounts = new Dictionary<string, int>[4, enemyTiers];
                enemyActTierNodes = new List<EnemyNodeData>[4, enemyTiers];
                this.levelActorRootNodes = levelActorRootNodes;
            }


        }

        public struct EnemyNodeData
        {
            public XElement enemyNode;
            public bool isScriptNode;
            public string enemyType;
            public string level;
            public int act;
            public int tier;

            public EnemyNodeData(XElement enemyNode, bool isScriptNode, string enemyType, string level, int act, int tier)
            {
                this.enemyNode = enemyNode;
                this.isScriptNode = isScriptNode;
                this.enemyType = enemyType;
                this.level = level;
                this.act = act;
                this.tier = tier;
            }
        }
    }
}
