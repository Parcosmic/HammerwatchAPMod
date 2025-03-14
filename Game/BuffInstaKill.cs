using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiltedEngine;
using TiltedEngine.WorldObjects;
using HammerwatchAP.Archipelago;

namespace HammerwatchAP.Game
{
    public class BuffInstaKill : IBuff
    {
        public bool remove;

        public const uint EFFECT_ID = 11U;

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
                return Color.MediumVioletRed;
            }
        }

        public float SpeedModifier
        {
            get
            {
                return 1f;
            }
        }

        public float DamageModifier
        {
            get
            {
                return 50f;
            }
        }

        public bool Stunned
        {
            get
            {
                return false;
            }
        }

        public BuffInstaKill()
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
