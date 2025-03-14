using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HammerwatchAP.Util
{
    public static class MiscHelper
    {

        public enum VersionMisMatch
        {
            None,
            Major,
            Minor,
            Build,
        }
        public static VersionMisMatch CheckVersion(Version clientVersion, string serverVersion)
        {
            string[] splits = serverVersion.Split('.');
            int majorVersion = int.Parse(splits[0]);
            if (clientVersion.Major > majorVersion)
                return VersionMisMatch.None;
            if (clientVersion.Major < majorVersion)
                return VersionMisMatch.Major;
            if (splits.Length > 1)
            {
                int minorVersion = int.Parse(splits[1]);
                if (clientVersion.Minor > minorVersion)
                    return VersionMisMatch.None;
                if (clientVersion.Minor < minorVersion)
                    return VersionMisMatch.Minor;
                if (splits.Length > 2)
                {
                    int buildVersion = int.Parse(splits[2]);
                    if (clientVersion.Build > buildVersion)
                        return VersionMisMatch.None;
                    if (clientVersion.Build < buildVersion)
                        return VersionMisMatch.Build;
                }
            }
            return VersionMisMatch.None;
        }

        public static void DeepCopy(string fromFolder, string toFolder, string exceptionDir = "")
        {
            string[] files = Directory.GetFiles(fromFolder);
            Directory.CreateDirectory(toFolder);
            foreach (string file in files)
            {
                string dest = Path.Combine(toFolder, Path.GetFileName(file));
                if (File.Exists(dest)) File.Delete(dest);
                File.Copy(file, dest);
            }
            string[] folders = Directory.GetDirectories(fromFolder);
            foreach (string folderPath in folders)
            {
                string folderName = Path.GetFileNameWithoutExtension(folderPath);
                if (folderName == exceptionDir) continue;
                DeepCopy(folderPath, Path.Combine(toFolder, folderName), exceptionDir);
            }
        }
        public static int[] SplitIntsFromString(string str)
        {
            string[] splits = str.Split(' ');
            int[] ints = new int[splits.Length];
            for (int i = 0; i < ints.Length; i++)
            {
                ints[i] = int.Parse(splits[i]);
            }
            return ints;
        }
        public static T RandomFromList<T>(Random random, List<T> list)
        {
            return list[random.Next(list.Count)];
        }
        public static T GetDictWeighted<T>(Dictionary<T, int> dict, Random random)
        {
            int total = 0;
            foreach (var pair in dict)
            {
                total += pair.Value;
            }
            int value = random.Next(0, total);
            foreach (var pair in dict)
            {
                value -= pair.Value;
                if (value < 0)
                    return pair.Key;
            }
            return default(T);
        }
        public static bool ReduceCountFromDict<T>(ref Dictionary<T, int> dict, T key, int amount = 1) //Return bool is if the key was removed or not
        {
            dict[key] -= amount;
            if (dict[key] > 0) return false;
            dict.Remove(key);
            return true;
        }
    }
}
