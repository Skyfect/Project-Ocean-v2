﻿// Crest Ocean System

// Copyright 2021 Wave Harmonic Ltd

using Crest.Spline;
using UnityEngine;

namespace Crest
{
    /// <summary>
    /// Custom spline point data for waves
    /// </summary>
    [AddComponentMenu("")]
    public class SplinePointDataWaves : SplinePointDataBase
    {
        /// <summary>
        /// The version of this asset. Can be used to migrate across versions. This value should
        /// only be changed when the editor upgrades the version.
        /// </summary>
        [SerializeField, HideInInspector]
#pragma warning disable 414
        int _version = 0;
#pragma warning restore 414

        [Tooltip("Weight multiplier to scale waves."), SerializeField]
        [DecoratedField, OnChange(nameof(NotifyOfSplineChange))]
        float _weight = 1f;
        public float Weight { get => _weight; set => _weight = value; }

        public override Vector2 GetData()
        {
            return new Vector2(_weight, 0f);
        }
    }
}
