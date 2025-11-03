using System;
using System.Collections.Generic;

namespace FreeFire.Config
{
    /// <summary>
    /// Free Fire Headshot Configuration
    /// Place this file in your data folder
    /// </summary>
    [Serializable]
    public class HeadshotConfig
    {
        // Headshot Damage Multipliers
        public float HeadshotDamageMultiplier = 2.0f;
        public float HelmetReductionLevel1 = 0.3f;
        public float HelmetReductionLevel2 = 0.4f;
        public float HelmetReductionLevel3 = 0.5f;

        // Aim Assist Settings
        public bool EnableAimAssist = true;
        public float AimAssistStrength = 0.75f;
        public float AimAssistRange = 100f;
        public float HeadTrackingSpeed = 1.5f;

        // Sensitivity Settings
        public float GeneralSensitivity = 100f;
        public float RedDotSensitivity = 95f;
        public float TwoXScopeSensitivity = 85f;
        public float FourXScopeSensitivity = 70f;
        public float EightXScopeSensitivity = 50f;
        public float FreeLookSensitivity = 100f;

        // Crosshair Settings
        public string CrosshairColor = "Red";
        public int CrosshairSize = 2;
        public float CrosshairOpacity = 1.0f;
        public bool ShowHitMarker = true;

        // Weapon-Specific Headshot Multipliers
        public Dictionary<string, float> WeaponHeadshotMultipliers = new Dictionary<string, float>
        {
            // Assault Rifles
            { "AK47", 2.05f },
            { "M4A1", 2.0f },
            { "SCAR", 2.0f },
            { "Groza", 2.1f },
            { "AN94", 2.0f },
            { "M14", 2.15f },
            
            // Sniper Rifles
            { "AWM", 2.95f },
            { "Kar98k", 2.8f },
            { "M82B", 3.0f },
            { "SVD", 2.5f },
            { "SKS", 2.4f },
            
            // SMGs
            { "MP40", 1.9f },
            { "Thompson", 1.9f },
            { "UMP", 1.85f },
            { "P90", 1.85f },
            { "MP5", 1.9f },
            
            // Shotguns
            { "M1887", 1.5f },
            { "SPAS12", 1.5f },
            { "M1014", 1.5f },
            
            // Pistols
            { "Desert_Eagle", 2.2f },
            { "M1911", 2.0f },
            { "USP", 1.95f }
        };

        // Recoil Control Settings
        public float VerticalRecoilControl = 0.8f;
        public float HorizontalRecoilControl = 0.85f;
        public bool EnableRecoilCompensation = true;

        // Advanced Settings
        public float BulletSpeed = 1.0f;
        public float BulletDrop = 1.0f;
        public int TickRate = 60;
        public bool PredictiveAiming = true;
        public float PredictionFactor = 0.5f;

        // Graphics Settings for Better Visibility
        public string GraphicsQuality = "Medium";
        public bool EnableShadows = false;
        public bool EnableBloom = false;
        public int RenderDistance = 75;
        public int FPSLimit = 60;

        // Audio Settings
        public float MasterVolume = 100f;
        public float SFXVolume = 100f;
        public float FootstepVolume = 150f;
        public float GunshotVolume = 100f;

        // HUD Settings
        public bool ShowMinimap = true;
        public float MinimapSize = 1.0f;
        public bool ShowDamageNumbers = true;
        public bool ShowKillFeed = true;

        /// <summary>
        /// Calculate final headshot damage with helmet reduction
        /// </summary>
        public float CalculateHeadshotDamage(float baseDamage, string weaponName, int helmetLevel)
        {
            float multiplier = WeaponHeadshotMultipliers.ContainsKey(weaponName) 
                ? WeaponHeadshotMultipliers[weaponName] 
                : HeadshotDamageMultiplier;

            float damage = baseDamage * multiplier;

            // Apply helmet reduction
            switch (helmetLevel)
            {
                case 1:
                    damage *= (1 - HelmetReductionLevel1);
                    break;
                case 2:
                    damage *= (1 - HelmetReductionLevel2);
                    break;
                case 3:
                    damage *= (1 - HelmetReductionLevel3);
                    break;
            }

            return damage;
        }

        /// <summary>
        /// Get optimal sensitivity for current scope
        /// </summary>
        public float GetScopeSensitivity(string scopeType)
        {
            switch (scopeType.ToLower())
            {
                case "reddot":
                case "holographic":
                    return RedDotSensitivity;
                case "2x":
                    return TwoXScopeSensitivity;
                case "4x":
                    return FourXScopeSensitivity;
                case "8x":
                    return EightXScopeSensitivity;
                default:
                    return GeneralSensitivity;
            }
        }

        /// <summary>
        /// Apply recoil compensation
        /// </summary>
        public void ApplyRecoilCompensation(ref float verticalRecoil, ref float horizontalRecoil)
        {
            if (EnableRecoilCompensation)
            {
                verticalRecoil *= (1 - VerticalRecoilControl);
                horizontalRecoil *= (1 - HorizontalRecoilControl);
            }
        }

        /// <summary>
        /// Load default configuration
        /// </summary>
        public static HeadshotConfig LoadDefault()
        {
            return new HeadshotConfig();
        }

        /// <summary>
        /// Validate configuration values
        /// </summary>
        public bool ValidateConfig()
        {
            if (HeadshotDamageMultiplier < 1.0f || HeadshotDamageMultiplier > 5.0f)
                return false;
            
            if (AimAssistStrength < 0f || AimAssistStrength > 1.0f)
                return false;

            if (GeneralSensitivity < 1f || GeneralSensitivity > 200f)
                return false;

            return true;
        }
    }
}
