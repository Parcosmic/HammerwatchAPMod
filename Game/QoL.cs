using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARPGGame;
using ARPGGame.Behaviors.Players;
using TiltedEngine;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;
using HammerwatchAP.Hooks;

namespace HammerwatchAP.Game
{
    public static class QoL
    {
        public static float EXPLORE_SPEED_MULTIPLIER = 3;
        public static int EXPLORE_SPEED_RESET_TIME = 5000;

        public static int[] playerExploreSpeedCounter = new int[4];
        public static bool[] immortalPlayers = new bool[4];

        public static void Setup()
        {
            playerExploreSpeedCounter = new int[4];
            immortalPlayers = new bool[4];
        }

        public static void TickExploreSpeed(int ms)
        {
            for (int p = 0; p < GameBase.Instance.Players.Count; p++)
            {
                PlayerInfo player = GameBase.Instance.Players[p];
                if (player == null) continue;
                if (player.IsLocalPlayer && ArchipelagoManager.ExploreSpeed)
                {
                    if (playerExploreSpeedCounter[p] > 0 && playerExploreSpeedCounter[p] - ms <= 0 && ArchipelagoManager.ExploreSpeedPing)
                        SoundHelper.PlayExploreSpeedSound();
                    playerExploreSpeedCounter[p] -= ms;
                    if (player.Controls.BackPress) //TODO: make this a dedicated button
                    {
                        if (playerExploreSpeedCounter[p] <= 0)
                        {
                            if (player.Actor != null)
                            {
                                PlayerActorBehavior playerBehavior = player.Actor.Behavior as PlayerActorBehavior;
                                if (playerBehavior != null)
                                {
                                    playerBehavior.AddHitEffect(new BuffExploreSpeed());
                                }
                            }
                        }
                    }
                    if (player.Controls.Attack1 || player.Controls.Attack2 || player.Controls.Attack3 || player.Controls.Attack4)
                    {
                        ResetExploreSpeed(player);
                    }
                }
                else
                {
                    ResetExploreSpeed(player);
                }
            }
        }
        public static void ResetExploreSpeed(PlayerInfo player)
        {
            if (player == null || player.Actor == null || player.Actor.Behavior.Immortal) return;
            playerExploreSpeedCounter[player.PeerID] = EXPLORE_SPEED_RESET_TIME;
            PlayerActorBehavior playerBehavior = player.Actor.Behavior as PlayerActorBehavior;
            //List<IBuff> playerActiveHitEffects = HookPlayerActorBehavior.GetPlayerActorBehaviorActiveHitEffects(playerBehavior);
            //List<IBuff> newActiveHitEffects = new List<IBuff>(playerActiveHitEffects);
            RemovePlayerHitEffectsWithId(playerBehavior, BuffExploreSpeed.EFFECT_ID);
            //for (int b = 0; b < playerActiveHitEffects.Count; b++)
            //{
            //    if (playerActiveHitEffects[b].EffectId != BuffExploreSpeed.EFFECT_ID) continue;
            //    playerActiveHitEffects.RemoveAt(b--);
            //}
            //HookPlayerActorBehavior.SetPlayerActorBehaviorActiveHitEffects(playerBehavior, newActiveHitEffects);
        }
        public static void RefreshImmortalPlayers()
        {
            foreach (PlayerInfo player in GameBase.Instance.Players)
            {
                if (player == null || player.Actor == null || !immortalPlayers[player.PeerID]) continue;
                player.Actor.Behavior.Immortal = true;
            }
        }
        public static void UpdateImmortalPlayers()
        {
            foreach (PlayerInfo player in GameBase.Instance.Players)
            {
                if (player == null || player.Actor == null || player.Actor.Behavior == null) continue;
                if (!(player.Actor.Behavior is PlayerActorBehavior playerBehavior)) continue;
                SetPlayerImmortal(playerBehavior, immortalPlayers[player.PeerID]);
            }
        }
        public static void SetPlayerImmortal(PlayerActorBehavior playerBehavior, bool immortal)
        {
            RemovePlayerHitEffectsWithId(playerBehavior, BuffInstaKill.EFFECT_ID);
            playerBehavior.Immortal = immortal;
            if (immortal)
            {
                playerBehavior.AddHitEffect(new BuffInstaKill());
            }
        }
        private static bool RemovePlayerHitEffectsWithId(PlayerActorBehavior playerBehavior, uint effectId)
        {
            bool effectRemoved = false;
            List<IBuff> playerActiveHitEffects = HookPlayerActorBehavior.GetPlayerActorBehaviorActiveHitEffects(playerBehavior);
            for (int b = 0; b < playerActiveHitEffects.Count; b++)
            {
                if (playerActiveHitEffects[b].EffectId != effectId) continue;
                playerActiveHitEffects.RemoveAt(b--);
                effectRemoved = true;
            }
            return effectRemoved;
        }
    }
}
