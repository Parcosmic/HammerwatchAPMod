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
        private const string mailSendSndName = "sound/misc.xml:info_mission";
        private const string aaaSndName = "sound/misc.xml:special_serious_aaaaaaaaaa";
        private static TiltedEngine.Audio.Sound countdownSnd;
        private static TiltedEngine.Audio.Sound countdownFinishSnd;
        private static TiltedEngine.Audio.Sound exploreSpeedSnd;
        private static TiltedEngine.Audio.Sound mailSendSnd;
        private static TiltedEngine.Audio.Sound aaaSnd;
        private static TiltedEngine.Audio.SoundInstance aaaSoundInstance;

        public static void LoadSounds()
        {
            countdownSnd = GameBase.Instance.resources.GetResource<TiltedEngine.Audio.Sound>(countdownSndName);
            countdownFinishSnd = GameBase.Instance.resources.GetResource<TiltedEngine.Audio.Sound>(countdownFinishSndName);
            exploreSpeedSnd = GameBase.Instance.resources.GetResource<TiltedEngine.Audio.Sound>(exploreSpeedSndName);
            mailSendSnd = GameBase.Instance.resources.GetResource<TiltedEngine.Audio.Sound>(mailSendSndName);
            aaaSnd = GameBase.Instance.resources.GetResource<TiltedEngine.Audio.Sound>(aaaSndName);
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
        public static void PlayMailSendSound()
        {
            mailSendSnd.Play2D(false);
        }
        public static void PlayAaaSound()
        {
            if(aaaSoundInstance != null)
            {
                StopAaaSound();
            }
            aaaSoundInstance = aaaSnd.Play2D(true);
        }
        public static void StopAaaSound()
        {
            if (aaaSoundInstance == null) return;
            aaaSoundInstance.Stop();
            aaaSoundInstance.Dispose();
        }
    }
}
