using System;
using FreeFire.Config;

namespace FreeFire.AutoHeadshot
{
    /// <summary>
    /// Advanced Aim Bot for Headshot Assistance
    /// Handles smooth aim transitions, target prediction, and recoil compensation
    /// </summary>
    public class HeadshotAimBot
    {
        private HeadshotConfig config;
        private Vector3 lastAimAdjustment;
        private float smoothingFactor;
        
        public HeadshotAimBot(HeadshotConfig configuration)
        {
            config = configuration;
            lastAimAdjustment = new Vector3(0, 0, 0);
            smoothingFactor = 0.3f; // Smoothing for natural aim movement
        }
        
        /// <summary>
        /// Calculate aim adjustment to target's head with prediction
        /// </summary>
        public Vector3 CalculateAimAdjustment(
            Vector3 currentAim,
            Vector3 targetHeadPosition,
            Vector3 targetVelocity,
            float distance,
            float deltaTime)
        {
            // Predict target position based on velocity and distance
            Vector3 predictedPosition = PredictTargetPosition(
                targetHeadPosition,
                targetVelocity,
                distance
            );
            
            // Calculate required aim adjustment
            Vector3 aimVector = predictedPosition - currentAim;
            
            // Apply distance-based scaling
            float distanceScale = CalculateDistanceScale(distance);
            aimVector = aimVector * distanceScale;
            
            // Apply head tracking speed
            aimVector = aimVector * config.HeadTrackingSpeed;
            
            // Apply smoothing for natural movement
            Vector3 smoothedAdjustment = ApplySmoothing(aimVector, deltaTime);
            
            // Apply aim assist strength
            smoothedAdjustment = smoothedAdjustment * config.AimAssistStrength;
            
            lastAimAdjustment = smoothedAdjustment;
            
            return smoothedAdjustment;
        }
        
        /// <summary>
        /// Predict target position based on velocity and bullet travel time
        /// </summary>
        private Vector3 PredictTargetPosition(
            Vector3 currentPosition,
            Vector3 velocity,
            float distance)
        {
            if (!config.PredictiveAiming)
                return currentPosition;
            
            // Estimate bullet travel time based on distance
            // Average bullet speed in Free Fire is ~400 m/s
            float bulletSpeed = 400f * config.BulletSpeed;
            float travelTime = distance / bulletSpeed;
            
            // Predict where target will be
            Vector3 prediction = velocity * travelTime * config.PredictionFactor;
            
            return currentPosition + prediction;
        }
        
        /// <summary>
        /// Calculate distance-based scaling factor
        /// Closer targets need less aggressive aim adjustment
        /// </summary>
        private float CalculateDistanceScale(float distance)
        {
            if (distance < 10f)
                return 0.5f;
            else if (distance < 30f)
                return 0.7f;
            else if (distance < 50f)
                return 0.9f;
            else if (distance < 100f)
                return 1.0f;
            else
                return 1.2f; // Long range needs more aggressive aim
        }
        
        /// <summary>
        /// Apply smoothing to aim adjustment for natural movement
        /// </summary>
        private Vector3 ApplySmoothing(Vector3 targetAdjustment, float deltaTime)
        {
            // Lerp between last adjustment and target adjustment
            float lerpFactor = Math.Min(1.0f, smoothingFactor * deltaTime * 60f);
            
            Vector3 smoothed = new Vector3(
                Lerp(lastAimAdjustment.X, targetAdjustment.X, lerpFactor),
                Lerp(lastAimAdjustment.Y, targetAdjustment.Y, lerpFactor),
                Lerp(lastAimAdjustment.Z, targetAdjustment.Z, lerpFactor)
            );
            
            return smoothed;
        }
        
        /// <summary>
        /// Linear interpolation
        /// </summary>
        private float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
        
        /// <summary>
        /// Calculate recoil compensation for continuous fire
        /// </summary>
        public Vector3 CalculateRecoilCompensation(
            string weaponName,
            int shotsFired,
            float deltaTime)
        {
            if (!config.EnableRecoilCompensation)
                return new Vector3(0, 0, 0);
            
            // Get weapon-specific recoil pattern
            var recoilPattern = GetWeaponRecoilPattern(weaponName);
            
            // Calculate recoil for current shot
            float verticalRecoil = recoilPattern.VerticalRecoil * shotsFired;
            float horizontalRecoil = recoilPattern.HorizontalRecoil * shotsFired;
            
            // Apply recoil control from config
            config.ApplyRecoilCompensation(ref verticalRecoil, ref horizontalRecoil);
            
            // Convert to vector (negative because we're compensating)
            return new Vector3(
                -horizontalRecoil,
                -verticalRecoil,
                0
            );
        }
        
        /// <summary>
        /// Get weapon-specific recoil pattern
        /// </summary>
        private RecoilPattern GetWeaponRecoilPattern(string weaponName)
        {
            // Simplified recoil patterns for different weapon types
            var patterns = new System.Collections.Generic.Dictionary<string, RecoilPattern>
            {
                // Assault Rifles - Medium recoil
                { "AK47", new RecoilPattern(0.15f, 0.08f) },
                { "M4A1", new RecoilPattern(0.10f, 0.05f) },
                { "SCAR", new RecoilPattern(0.11f, 0.06f) },
                { "Groza", new RecoilPattern(0.14f, 0.07f) },
                { "AN94", new RecoilPattern(0.12f, 0.06f) },
                { "M14", new RecoilPattern(0.18f, 0.09f) },
                
                // Sniper Rifles - High recoil but single shot
                { "AWM", new RecoilPattern(0.30f, 0.10f) },
                { "Kar98k", new RecoilPattern(0.28f, 0.09f) },
                { "M82B", new RecoilPattern(0.35f, 0.12f) },
                { "SVD", new RecoilPattern(0.25f, 0.08f) },
                { "SKS", new RecoilPattern(0.22f, 0.07f) },
                
                // SMGs - Low recoil, high fire rate
                { "MP40", new RecoilPattern(0.08f, 0.04f) },
                { "Thompson", new RecoilPattern(0.09f, 0.05f) },
                { "UMP", new RecoilPattern(0.07f, 0.04f) },
                { "P90", new RecoilPattern(0.06f, 0.03f) },
                { "MP5", new RecoilPattern(0.08f, 0.04f) },
                
                // Shotguns - Very high recoil
                { "M1887", new RecoilPattern(0.40f, 0.15f) },
                { "SPAS12", new RecoilPattern(0.35f, 0.12f) },
                { "M1014", new RecoilPattern(0.30f, 0.10f) },
                
                // Pistols - Low recoil
                { "Desert_Eagle", new RecoilPattern(0.20f, 0.08f) },
                { "M1911", new RecoilPattern(0.12f, 0.05f) },
                { "USP", new RecoilPattern(0.10f, 0.04f) }
            };
            
            return patterns.ContainsKey(weaponName) 
                ? patterns[weaponName] 
                : new RecoilPattern(0.10f, 0.05f);
        }
        
        /// <summary>
        /// Calculate optimal aim point based on target stance
        /// </summary>
        public Vector3 CalculateOptimalAimPoint(
            Vector3 targetPosition,
            TargetStance stance)
        {
            // Adjust aim point based on target stance
            float headHeightOffset = 1.7f; // Standing head height
            
            switch (stance)
            {
                case TargetStance.Crouching:
                    headHeightOffset = 1.2f;
                    break;
                case TargetStance.Prone:
                    headHeightOffset = 0.4f;
                    break;
                case TargetStance.Jumping:
                    headHeightOffset = 2.0f;
                    break;
                case TargetStance.Standing:
                default:
                    headHeightOffset = 1.7f;
                    break;
            }
            
            return targetPosition + new Vector3(0, headHeightOffset, 0);
        }
        
        /// <summary>
        /// Check if aim adjustment is within valid range
        /// </summary>
        public bool IsAimAdjustmentValid(Vector3 adjustment)
        {
            float magnitude = adjustment.Magnitude();
            
            // Prevent unrealistic aim snapping
            float maxAdjustmentPerFrame = 5.0f;
            
            return magnitude <= maxAdjustmentPerFrame;
        }
        
        /// <summary>
        /// Apply bullet drop compensation for long range shots
        /// </summary>
        public Vector3 ApplyBulletDropCompensation(
            Vector3 aimPoint,
            float distance,
            string weaponName)
        {
            // Bullet drop is more significant at longer ranges
            if (distance < 50f)
                return aimPoint;
            
            // Calculate drop based on distance and weapon type
            float dropFactor = config.BulletDrop;
            float drop = 0f;
            
            if (distance < 100f)
                drop = (distance - 50f) * 0.01f * dropFactor;
            else
                drop = (50f * 0.01f + (distance - 100f) * 0.02f) * dropFactor;
            
            // Sniper rifles have less drop due to higher velocity
            if (weaponName.Contains("AWM") || weaponName.Contains("Kar98k") || 
                weaponName.Contains("M82B") || weaponName.Contains("SVD"))
            {
                drop *= 0.7f;
            }
            
            return aimPoint + new Vector3(0, drop, 0);
        }
    }
    
    /// <summary>
    /// Recoil pattern for weapons
    /// </summary>
    public class RecoilPattern
    {
        public float VerticalRecoil { get; set; }
        public float HorizontalRecoil { get; set; }
        
        public RecoilPattern(float vertical, float horizontal)
        {
            VerticalRecoil = vertical;
            HorizontalRecoil = horizontal;
        }
    }
    
    /// <summary>
    /// Target stance enumeration
    /// </summary>
    public enum TargetStance
    {
        Standing,
        Crouching,
        Prone,
        Jumping
    }
}
