using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OpenTK;

namespace HammerwatchAP.Util
{
    public static class NodeHelper
    {
        public static Vector2 PosFromString(string posStr)
        {
            string[] posStrs = posStr.Split(' ');
            Vector2 pos = new Vector2(float.Parse(posStrs[0], System.Globalization.CultureInfo.InvariantCulture), float.Parse(posStrs[1], System.Globalization.CultureInfo.InvariantCulture));
            return pos;
        }

        public static XElement[] CreateDestroySpawnNodes(int id, string spawnObject, Vector2 pos, int checkDestroyedId, bool isDynamic)
        {
            XElement[] nodes = new XElement[2];
            nodes[0] = CreateSpawnObjectNode(id, spawnObject, pos);
            nodes[1] = CreateObjectEventTriggerNode(id + 1, pos + new Vector2(0, 5), 1, "Destroyed", new[] { checkDestroyedId }, isDynamic, new[] { id });
            return nodes;
        }

        public static XElement CreateSpawnObjectNode(int id, string spawnObject, Vector2 pos)
        {
            XElement spawnObjectNode = CreateScriptNodeBase(id, "SpawnObject", true, 1, pos);
            spawnObjectNode.Add(CreateXNode("parameters", spawnObject));
            return spawnObjectNode;
        }
        public static XElement CreateDestroyObjectNode(int id, Vector2 pos, int triggerTimes, int[] objects)
        {
            XElement levelStartNode = CreateScriptNodeBase(id, "DestroyObject", true, triggerTimes, pos);
            levelStartNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("static", objects),
            }));
            return levelStartNode;
        }
        public static XElement CreateLevelStartNode(int id, Vector2 pos, int startId, int dir = 2)
        {
            XElement levelExitNode = CreateScriptNodeBase(id, "LevelStart", true, -1, pos);
            levelExitNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("id", startId),
                CreateXNode("dir", dir),
            }));
            return levelExitNode;
        }
        public static XElement CreateLevelExitNode(int id, bool enabled, Vector2 pos, string level, int startId, int[] shapeIds)
        {
            XElement levelStartNode = CreateScriptNodeBase(id, "LevelExitArea", enabled, -1, pos);
            levelStartNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("level", level),
                CreateXNode("start id", startId),
                CreateDictionaryNode("shape", new[]
                {
                    CreateXNode("static", shapeIds)
                })
            }));
            return levelStartNode;
        }
        public static XElement CreateToggleElementNode(int id, Vector2 pos, int triggerTimes, int state, int[] connections)
        {
            XElement levelStartNode = CreateScriptNodeBase(id, "ToggleElement", true, triggerTimes, pos);
            levelStartNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("state", state),
                CreateDictionaryNode("element", new []
                {
                    CreateXNode("static", connections)
                })
            }));
            return levelStartNode;
        }
        public static XElement CreateChangeDoodadStateNode(int id, Vector2 pos, int triggerTimes, string stateName, int[] objects)
        {
            XElement baseNode = CreateScriptNodeBase(id, "ChangeDoodadState", true, triggerTimes, pos);
            baseNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("state", stateName),
                CreateDictionaryNode("object", new []
                {
                    CreateXNode("static", objects)
                })
            }));
            return baseNode;
        }
        public static XElement CreateGlobalEventTriggerNode(int id, int triggerTimes, Vector2 pos, string eventName, int[] connections = null, int[] connectionDelays = null)
        {
            XElement node = CreateScriptNodeBase(id, "GlobalEventTrigger", true, triggerTimes, pos);
            node.Add(CreateXNode("parameters", eventName));
            if (connections == null) return node;
            AddConnectionNodes(node, connections, connectionDelays);
            return node;
        }
        public static XElement CreateObjectEventTriggerNode(int id, Vector2 pos, int triggerTimes, string eventName, int[] objects, bool objectsAreDynamic, int[] connections)
        {
            int[] objects2 = new int[objectsAreDynamic ? objects.Length + 1 : objects.Length];
            objects.CopyTo(objects2, 0);
            XElement onDestroyedNodeBase = CreateScriptNodeBase(id, "ObjectEventTrigger", true, triggerTimes, pos);
            onDestroyedNodeBase.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("event", eventName),
                CreateDictionaryNode("object", new []
                {
                    CreateXNode(objectsAreDynamic ? "dynamic" : "static", objects2)
                })
            }));
            AddConnectionNodes(onDestroyedNodeBase, connections);
            return onDestroyedNodeBase;
        }
        public static XElement CreateCheckGlobalFlagNode(int id, Vector2 pos, string flag, int[] trueNodes, int[] falseNodes, bool enabled = true, int triggerTimes = -1)
        {
            XElement flagNode = CreateScriptNodeBase(id, "CheckGlobalFlag", enabled, triggerTimes, pos);
            flagNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("flag", flag),
                CreateDictionaryNode("on-true", trueNodes != null ? new []
                {
                    CreateXNode("static", trueNodes)
                } : null),
                CreateDictionaryNode("on-false", falseNodes != null ? new []
                {
                    CreateXNode("static", falseNodes)
                } : null),
            }));
            return flagNode;
        }
        public static XElement CreateCheckGlobalFlagNode(int id, Vector2 pos, string flag, bool trueDynamic, int[] trueNodes, bool falseDynamic, int[] falseNodes)
        {
            XElement flagNode = CreateScriptNodeBase(id, "CheckGlobalFlag", true, -1, pos);
            int[] trueDynamicNodes = trueNodes;
            if (trueDynamic)
            {
                trueDynamicNodes = new int[trueNodes.Length + 1];
                trueNodes.CopyTo(trueDynamicNodes, 0);
            }
            flagNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("flag", flag),
                CreateDictionaryNode("on-true", trueDynamicNodes != null ? new XElement[]
                {
                    CreateXNode(trueDynamic ? "dynamic" : "static", trueDynamicNodes)
                } : null),
                CreateDictionaryNode("on-false", falseNodes != null ? new XElement[]
                {
                    CreateXNode(falseDynamic ? "dynamic" : "static", falseNodes)
                } : null),
            }));
            return flagNode;
        }
        public static XElement CreateSetGlobalFlagNode(int id, Vector2 pos, int triggerTimes, string flagName, bool enabled)
        {
            XElement baseNode = CreateScriptNodeBase(id, "SetGlobalFlag", true, triggerTimes, pos);
            baseNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("flag", flagName),
                CreateXNode("state", enabled ? 0 : 1)
            }));
            return baseNode;
        }
        public static XElement CreatePlaySoundNode(int id, Vector2 pos, string sound, bool loop, bool play3D, float range3d)
        {
            XElement baseNode = CreateScriptNodeBase(id, "PlaySound", true, -1, pos);
            baseNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("sound", sound),
                CreateXNode("loop", loop),
                CreateXNode("play3d", play3D),
                CreateXNode("range3d", range3d),
            }));
            return baseNode;
        }
        public static XElement CreatePlayMusicNode(int id, Vector2 pos, int triggerTimes, string music, bool loop)
        {
            XElement baseNode = CreateScriptNodeBase(id, "PlayMusic", true, triggerTimes, pos);
            baseNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("sound", music),
                CreateXNode("loop", loop)
            }));
            return baseNode;
        }
        public static XElement CreatePlayEffectNode(int id, Vector2 pos, string effect, int layer = 50)
        {
            XElement baseNode = CreateScriptNodeBase(id, "PlayEffect", true, -1, pos);
            baseNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("effect", effect),
                CreateXNode("layer", layer)
            }));
            return baseNode;
        }
        public static XElement CreatePlayAttachedEffectNode(int id, Vector2 pos, string effect, bool looping, int[] attachObjects, bool objectsAreDynamic, int layer = 20, int yOffset = 0)
        {
            if (objectsAreDynamic)
            {
                List<int> attachObjectIds = attachObjects.ToList();
                attachObjectIds.Add(0);
                attachObjects = attachObjectIds.ToArray();
            }
            XElement baseNode = CreateScriptNodeBase(id, "PlayAttachedEffect", true, 1, pos);
            if (!effect.Contains(":"))
                effect += ":";
            baseNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("effect", $"{effect}"),
                CreateXNode("layer", layer),
                CreateXNode("looping", looping),
                CreateDictionaryNode("objects", new []{
                    CreateXNode(objectsAreDynamic ? "dynamic" : "static", attachObjects),
                }),
                CreateXNode("y-offset", yOffset),
            }));
            return baseNode;
        }
        public static XElement CreateSpeechBubbleNode(int id, Vector2 pos, int triggerTimes, string style, string text, int[] objects, Vector2 offset, int width, int layer, int time)
        {
            XElement baseNode = CreateScriptNodeBase(id, "ShowSpeechBubble", true, triggerTimes, pos);
            baseNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("style", style),
                CreateXNode("text", text),
                CreateDictionaryNode("objects", new []
                {
                    CreateXNode("static", objects)
                }),
                CreateXNode("x-offset", offset.X),
                CreateXNode("y-offset", offset.Y),
                CreateXNode("width", width),
                CreateXNode("layer", layer),
                CreateXNode("time", time),
                CreateXNode("dictionary", "execute", " ")
            }));
            return baseNode;
        }
        public static XElement CreateAnnounceTextNode(int id, Vector2 pos, string text, int time, int type, bool enabled, int triggerTimes)
        {
            XElement baseNode = CreateScriptNodeBase(id, "AnnounceText", enabled, triggerTimes, pos);
            baseNode.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("text", text),
                CreateXNode("time", time),
                CreateXNode("type", type),
            }));
            return baseNode;
        }
        public static XElement CreateScriptLinkNode(int id, bool enabled, int triggerTimes, Vector2 pos, int[] connections = null, int[] connectionDelays = null)
        {
            XElement node = CreateScriptNodeBase(id, "ScriptLink", enabled, triggerTimes, pos);
            if (connections != null)
                AddConnectionNodes(node, connections, connectionDelays);
            return node;
        }
        //Type flags: player, enemy, allied, neutral
        public static XElement CreateRectangleShapeNode(int id, Vector2 pos, float w, float h, int types)
        {
            XElement node = CreateScriptNodeBase(id, "RectangleShape", true, -1, pos);
            node.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("w", w),
                CreateXNode("h", h),
                CreateXNode("types", types)
            }));
            return node;
        }
        //Event types: 0 = OnEnter, 1 = OnLeave, 2 = OnEnterOrLeave
        public static XElement CreateAreaTriggerNode(int id, int triggerTimes, Vector2 pos, int onEvent, int types, int[] shapes, int[] connections, int[] connectionDelays = null)
        {
            XElement node = CreateScriptNodeBase(id, "AreaTrigger", true, triggerTimes, pos);
            node.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("event", onEvent),
                CreateXNode("types", types),
                CreateDictionaryNode("shape", new[]
                {
                    CreateXNode("static", shapes)
                })
            }));
            AddConnectionNodes(node, connections, connectionDelays);
            return node;
        }
        public static XElement[] CreateAreaTriggerNodes(ref int id, Vector2 pos, List<int> triggerNodeIds, bool delay = true, int size = 1)
        {
            XElement[] nodes = new XElement[2];
            int rectangleShapeNodeId = id++;

            int[] connectionDelays = new int[triggerNodeIds.Count];
            for (int c = 0; c < connectionDelays.Length; c++)
            {
                connectionDelays[c] = delay ? 300 : 0;
            }
            nodes[0] = CreateAreaTriggerNode(id++, -1, pos + new Vector2(0, -2), 0, 1, new[] { rectangleShapeNodeId }, triggerNodeIds.ToArray(), connectionDelays);
            nodes[1] = CreateRectangleShapeNode(rectangleShapeNodeId, pos, size, size, 15);

            return nodes;
        }
        public static XElement CreateTogglePhysicsNode(int id, bool enabled, int triggerTimes, Vector2 pos, int state, int[] doodads)
        {
            XElement node = CreateScriptNodeBase(id, "TogglePhysics", enabled, triggerTimes, pos);
            node.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("state", state),
                CreateDictionaryNode("doodad", new[]
                {
                    CreateXNode("static", doodads)
                }),
            }));
            return node;
        }
        public static XElement CreateCameraShakeNode(int id, bool enabled, int triggerTimes, Vector2 pos, int duration, float xamount, float yamount, int speed)
        {
            XElement node = CreateScriptNodeBase(id, "CameraShake", enabled, triggerTimes, pos);
            node.Add(CreateDictionaryNode(new[]
            {
                CreateXNode("duration", duration),
                CreateXNode("x-amount", xamount),
                CreateXNode("y-amount", yamount),
                CreateXNode("speed", speed),
            }));
            return node;
        }

        public static XElement CreateScriptNodeBase(int id, string type, bool enabled, int triggerTimes, Vector2 pos)
        {
            XElement node = new XElement("dictionary");
            node.Add(CreateXNode("id", id));
            node.Add(CreateXNode("type", type));
            node.Add(CreateXNode("enabled", enabled));
            node.Add(CreateXNode("trigger-times", triggerTimes));
            node.Add(CreateXNode("pos", pos));
            return node;
        }

        public static XElement CreateXNode(string type, string name, string value)
        {
            XElement node = new XElement(type);
            node.SetAttributeValue("name", name);
            node.Value = value;
            return node;
        }
        public static XElement CreateXNode(string name, int value)
        {
            XElement node = new XElement("int");
            node.SetAttributeValue("name", name);
            node.Value = value.ToString();
            return node;
        }
        public static XElement CreateXNode(string name, int[] values)
        {
            XElement node = new XElement("int-arr");
            node.SetAttributeValue("name", name);
            node.Value = "";
            if (values != null && values.Length > 0)
            {
                foreach (int value in values)
                {
                    node.Value += $"{value} ";
                }
                node.Value = node.Value.Substring(0, node.Value.Length - 1);
            }
            return node;
        }
        public static XElement CreateXNode(string name, float value)
        {
            XElement node = new XElement("float");
            node.SetAttributeValue("name", name);
            node.Value = value.ToString();
            return node;
        }
        public static XElement CreateXNode(string name, bool value)
        {
            XElement node = new XElement("bool");
            node.SetAttributeValue("name", name);
            node.Value = value ? "True" : "False";
            return node;
        }
        public static XElement CreateXNode(string name, string value)
        {
            XElement node = new XElement("string");
            node.SetAttributeValue("name", name);
            node.Value = value;
            return node;
        }
        public static XElement CreateXNode(string name, Vector2 value)
        {
            XElement node = new XElement("vec2");
            node.SetAttributeValue("name", name);
            node.Value = $"{value.X.ToString(System.Globalization.CultureInfo.InvariantCulture)} {value.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            //node.Value = $"{value.X} {value.Y}";
            return node;
        }
        public static XElement CreateDictionaryNode(XElement[] nodes)
        {
            return CreateDictionaryNode("parameters", nodes);
        }
        public static XElement CreateDictionaryNode(string name, XElement[] nodes)
        {
            XElement dictNode = new XElement("dictionary");
            dictNode.SetAttributeValue("name", name);
            if (nodes != null)
            {
                foreach (XElement node in nodes)
                {
                    dictNode.Add(node);
                }
            }
            return dictNode;
        }

        public static XElement CreateDoodadNode(int id, string type, Vector2 pos, bool needSync)
        {
            XElement rootNode = new XElement("dictionary");
            rootNode.Add(CreateXNode("id", id));
            rootNode.Add(CreateXNode("type", type));
            rootNode.Add(CreateXNode("pos", pos));
            rootNode.Add(CreateXNode("need-sync", needSync));
            return rootNode;
        }
        public static XElement CreateDoodadNode(int id, string type, string pos, bool needSync)
        {
            XElement rootNode = new XElement("dictionary");
            rootNode.Add(CreateXNode("id", id));
            rootNode.Add(CreateXNode("type", type));
            rootNode.Add(CreateXNode("vec2", "pos", pos));
            rootNode.Add(CreateXNode("need-sync", needSync));
            return rootNode;
        }
        public static void AddConnectionNodes(XElement node, int[] connections)
        {
            AddConnectionNodes(node, connections, new int[connections.Length]);
        }
        public static void AddConnectionNodes(XElement node, int[] connections, int[] connectionDelays)
        {
            if (node.Element("int-arr") == null)
            {
                node.Add(CreateXNode("connections", connections));
                if (connectionDelays == null)
                    connectionDelays = new int[connections.Length];
                node.Add(CreateXNode("connection-delays", connectionDelays));
            }
            else
            {
                XElement[] connectionNodes = node.Elements("int-arr").ToArray();
                connectionNodes[0].Value += " " + string.Join(" ", connections);
                connectionNodes[1].Value += " " + string.Join(" ", connectionDelays);
            }
        }
        public static void SetConnectionNodes(XElement node, int connection)
        {
            SetConnectionNodes(node, new[] { connection }, new int[1]);
        }
        public static void SetConnectionNodes(XElement node, int[] connections)
        {
            SetConnectionNodes(node, connections, new int[connections.Length]);
        }
        public static void SetConnectionNodes(XElement node, int[] connections, int[] connectionDelays)
        {
            XElement[] connectionNodes = node.Elements("int-arr").ToArray();
            connectionNodes[0].Value = string.Join(" ", connections);
            connectionNodes[1].Value = string.Join(" ", connectionDelays);
        }
        //public static void AppendConnectionNodes(XElement node, int[] connections)
        //{
        //    AppendConnectionNodes(node, connections, new int[connections.Length]);
        //}
        //public static void AppendConnectionNodes(XElement node, int[] connections, int[] connectionDelays)
        //{
        //    XElement[] connectionNodes = node.Elements("int-arr").ToArray();
        //    connectionNodes[0].Value += " " + string.Join(" ", connections);
        //    connectionNodes[1].Value += " " + string.Join(" ", connectionDelays);
        //}

        public static void EditShowSpeechBubbleNode(XElement node, string text)
        {
            XElement paramsNode = node.Element("dictionary");
            XElement textNode = paramsNode.Elements("string").ToArray()[1];
            textNode.Value = text;
        }

        private static XElement CreateGUIBaseNode(string name, string idName, string offset)
        {
            XElement guiNode = new XElement(name);
            guiNode.SetAttributeValue("id", idName);
            guiNode.SetAttributeValue("offset", offset);
            return guiNode;
        }
        public static XElement CreateGUITextNode(string idName, string offset, string text, string font, bool visible = true)
        {
            XElement guiNode = CreateGUIBaseNode("text", idName, offset);
            guiNode.SetAttributeValue("text", text);
            guiNode.SetAttributeValue("font", font);
            if (!visible)
                guiNode.SetAttributeValue("visible", "false");
            return guiNode;
        }
        public static XElement CreateGUISpriteNode(string idName, string offset, string image, bool visible)
        {
            XElement guiNode = CreateGUIBaseNode("sprite", idName, offset);
            guiNode.SetAttributeValue("image", image);
            guiNode.SetAttributeValue("visible", visible ? "true" : "false");
            return guiNode;
        }

        public static XElement CreateTweakNode(string id, int cost, string category, string name, string description, bool shared = false)
        {
            XElement tweakNode = new XElement("dictionary");
            tweakNode.SetAttributeValue("id", id);
            tweakNode.SetAttributeValue("cost", cost);
            tweakNode.SetAttributeValue("cat", category);
            tweakNode.SetAttributeValue("name", name);
            tweakNode.SetAttributeValue("desc", description);
            if (shared)
                tweakNode.SetAttributeValue("shared", "true");
            return tweakNode;
        }
    }
}
