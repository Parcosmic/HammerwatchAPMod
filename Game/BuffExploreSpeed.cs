using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiltedEngine;
using TiltedEngine.WorldObjects;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;

namespace HammerwatchAP.Game
{
    public class BuffExploreSpeed : IBuff
    {
        public bool remove = false;

        public const uint EFFECT_ID = 10U;

        public uint EffectId
        {
            get
            {
                return EFFECT_ID;
            }
        }

        public Color ColorModifier
        {
            get
            {
                return Color.Yellow;
            }
        }

        public float SpeedModifier
        {
            get
            {
                return QoL.EXPLORE_SPEED_MULTIPLIER;
            }
        }

        public float DamageModifier
        {
            get
            {
                return 1f;
            }
        }

        public bool Stunned
        {
            get
            {
                return false;
            }
        }

        public BuffExploreSpeed()
        {
        }

        public void Add(WorldActor actor)
        {
        }

        public void OnDestroy(WorldActor actor)
        {
        }

        public void Refresh(IBuff buff)
        {
            remove = true;
        }

        public void Remove(WorldActor actor)
        {
        }

        public bool Update(int ms, WorldActor worldActor)
        {
            return !remove;
        }
    }
}
