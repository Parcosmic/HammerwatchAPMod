using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARPGGame;

namespace HammerwatchAP.Util
{
    public static class SoundHelper
    {
        private const string countdownSndName = "sound/bonus.xml:bonus_gate";
        private const string countdownFinishSndName = "sound/misc.xml:end_screen"; //sound/misc.xml:info_mission
        private const string exploreSpeedSndName = "sound/misc.xml:info_note_tutorial"; //sound/misc.xml:missing
        private static TiltedEngine.Audio.Sound countdownSnd;
        private static TiltedEngine.Audio.Sound countdownFinishSnd;
        private static TiltedEngine.Audio.Sound exploreSpeedSnd;

        public static void LoadSounds()
        {
            countdownSnd = GameBase.Instance.resources.GetResource<TiltedEngine.Audio.Sound>(countdownSndName);
            countdownFinishSnd = GameBase.Instance.resources.GetResource<TiltedEngine.Audio.Sound>(countdownFinishSndName);
            exploreSpeedSnd = GameBase.Instance.resources.GetResource<TiltedEngine.Audio.Sound>(exploreSpeedSndName);
        }

        public static void PlayCountdownSound()
        {
            countdownSnd.Play2D(false);
        }
        public static void PlayCountdownFinishSound()
        {
            countdownFinishSnd.Play2D(false);
        }
        public static void PlayExploreSpeedSound()
        {
            exploreSpeedSnd.Play2D(false);
        }
    }
}
