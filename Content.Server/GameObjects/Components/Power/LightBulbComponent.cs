﻿using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using SpriteComponent = Robust.Server.GameObjects.SpriteComponent;

namespace Content.Server.GameObjects.Components.Power
{
    public enum LightBulbState
    {
        Normal,
        Broken,
        Burned,
    }

    public enum LightBulbType
    {
        Bulb,
        Tube,
    }

    /// <summary>
    ///     Component that represents a light bulb. Can be broken, or burned, which turns them mostly useless.
    /// </summary>
    public class LightBulbComponent : Component
    {

        /// <summary>
        ///     Invoked whenever the state of the light bulb changes.
        /// </summary>
        public event EventHandler<EventArgs> OnLightBulbStateChange;
        public event EventHandler<EventArgs> OnLightColorChange;

        private Color _color = Color.White;

        [ViewVariables(VVAccess.ReadWrite)] public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                OnLightColorChange?.Invoke(this, null);
                UpdateColor();
            }
        }

        public override string Name => "LightBulb";

        public LightBulbType Type = LightBulbType.Tube;

        private int _burningTemperature;
        public int BurningTemperature => _burningTemperature;

        private float _powerUse;
        public float PowerUse => _powerUse;

        /// <summary>
        ///     The current state of the light bulb. Invokes the OnLightBulbStateChange event when set.
        ///     It also updates the bulb's sprite accordingly.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)] public LightBulbState State
        {
            get { return _state; }
            set
            {
                var sprite = Owner.GetComponent<SpriteComponent>();
                OnLightBulbStateChange?.Invoke(this, EventArgs.Empty);
                _state = value;
                switch (value)
                {
                    case LightBulbState.Normal:
                        sprite.LayerSetState(0, "normal");
                        break;
                    case LightBulbState.Broken:
                        sprite.LayerSetState(0, "broken");
                        break;
                    case LightBulbState.Burned:
                        sprite.LayerSetState(0, "burned");
                        break;
                }
            }
        }

        private LightBulbState _state = LightBulbState.Normal;

        public override void ExposeData(ObjectSerializer serializer)
        {
            serializer.DataField(ref Type, "bulb", LightBulbType.Tube);
            serializer.DataField(ref _color, "color", Color.White);
            serializer.DataFieldCached(ref _burningTemperature, "BurningTemperature", 1400);
            serializer.DataFieldCached(ref _powerUse, "PowerUse", 40);
        }

        public void UpdateColor()
        {
            var sprite = Owner.GetComponent<SpriteComponent>();
            sprite.Color = Color;
        }

        public override void Initialize()
        {
            base.Initialize();
            UpdateColor();
        }
    }
}
