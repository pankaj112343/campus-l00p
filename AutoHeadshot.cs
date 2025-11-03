using System;
using System.Collections.Generic;
using System.Linq;
using FreeFire.Config;

namespace FreeFire.AutoHeadshot
{
    /// <summary>
    /// Main Auto Headshot System
    /// Coordinates target detection, aim assistance, and automatic headshot execution
    /// </summary>
    public class AutoHeadshot
    {
        private HeadshotConfig config;
        private TargetDetection targetDetection;
        private HeadshotAimBot aimBot;
        
        public bool IsEnabled { get; set; }
        public bool AutoFireEnabled { get; set; }
        public Target CurrentTarget { get; private set; }
        
        // Statistics
        public int TotalShots { get; private set; }
        public int HeadshotCount { get; private set; }
        public float HeadshotAccuracy => TotalShots > 0 ? (float)HeadshotCount / TotalShots * 100f : 0f;
        
        public AutoHeadshot(HeadshotConfig configuration)
        {
            config = configuration ?? HeadshotConfig.LoadDefault();
            targetDetection = new TargetDetection(config);
            aimBot = new HeadshotAimBot(config);
            IsEnabled = false;
            AutoFireEnabled = false;
            TotalShots = 0;
            HeadshotCount = 0;
        }
        
        /// <summary>
        /// Main update loop - call this every frame
        /// </summary>
        public void Update(float deltaTime, PlayerState playerState, List<Enemy> enemies)
        {
            if (!IsEnabled || !config.EnableAimAssist)
                return;
            
            // Detect and prioritize targets
            var targets = targetDetection.DetectTargets(playerState, enemies);
            
            if (targets.Count == 0)
            {
                CurrentTarget = null;
                return;
            }
            
            // Select best target based on priority
            CurrentTarget = SelectBestTarget(targets, playerState);
            
            if (CurrentTarget == null)
                return;
            
            // Apply aim assistance to target's head
            var aimAdjustment = aimBot.CalculateAimAdjustment(
                playerState.AimPosition,
                CurrentTarget.HeadPosition,
                CurrentTarget.Velocity,
                CurrentTarget.Distance,
                deltaTime
            );
            
            // Apply the aim adjustment
            ApplyAimAdjustment(playerState, aimAdjustment);
            
            // Auto fire if enabled and target is in crosshair
            if (AutoFireEnabled && IsTargetInCrosshair(playerState, CurrentTarget))
            {
                Fire(playerState, CurrentTarget);
            }
        }
        
        /// <summary>
        /// Select the best target based on multiple factors
        /// </summary>
        private Target SelectBestTarget(List<Target> targets, PlayerState playerState)
        {
            if (targets.Count == 0)
                return null;
            
            // Priority factors:
            // 1. Distance (closer is better)
            // 2. Angle to target (smaller angle is better)
            // 3. Target health (lower health is better for quick kills)
            // 4. Target threat level (attacking players are priority)
            
            return targets
                .OrderBy(t => t.Priority)
                .ThenBy(t => t.Distance)
                .ThenBy(t => t.AngleToPlayer)
                .FirstOrDefault();
        }
        
        /// <summary>
        /// Apply aim adjustment to player's aim
        /// </summary>
        private void ApplyAimAdjustment(PlayerState playerState, Vector3 adjustment)
        {
            // Smooth aim adjustment based on aim assist strength
            var smoothedAdjustment = adjustment * config.AimAssistStrength;
            
            playerState.AimPosition += smoothedAdjustment;
            
            // Apply sensitivity scaling based on current scope
            var sensitivity = config.GetScopeSensitivity(playerState.CurrentScope);
            playerState.AimPosition *= (sensitivity / 100f);
        }
        
        /// <summary>
        /// Check if target is within crosshair threshold
        /// </summary>
        private bool IsTargetInCrosshair(PlayerState playerState, Target target)
        {
            var angleToTarget = Vector3.AngleBetween(
                playerState.AimDirection,
                (target.HeadPosition - playerState.Position).Normalized()
            );
            
            // Threshold depends on weapon type and distance
            float threshold = 2.0f; // degrees
            
            if (target.Distance > 50f)
                threshold = 3.0f;
            if (target.Distance > 100f)
                threshold = 5.0f;
            
            return angleToTarget <= threshold;
        }
        
        /// <summary>
        /// Fire at the target
        /// </summary>
        private void Fire(PlayerState playerState, Target target)
        {
            TotalShots++;
            
            // Calculate if this will be a headshot
            bool isHeadshot = IsTargetInCrosshair(playerState, target);
            
            if (isHeadshot)
            {
                HeadshotCount++;
                
                // Calculate damage
                float damage = CalculateHeadshotDamage(
                    playerState.CurrentWeapon,
                    target.HelmetLevel
                );
                
                Console.WriteLine($"[HEADSHOT] {playerState.CurrentWeapon} -> {target.Name} | Damage: {damage:F1} | Distance: {target.Distance:F1}m");
                
                // Apply damage to target
                target.Health -= damage;
                
                if (target.Health <= 0)
                {
                    Console.WriteLine($"[ELIMINATED] {target.Name}");
                }
            }
            else
            {
                Console.WriteLine($"[SHOT] {playerState.CurrentWeapon} -> {target.Name} | Body shot");
            }
        }
        
        /// <summary>
        /// Calculate headshot damage with weapon and helmet factors
        /// </summary>
        private float CalculateHeadshotDamage(string weaponName, int helmetLevel)
        {
            // Base damage varies by weapon (simplified)
            float baseDamage = GetWeaponBaseDamage(weaponName);
            
            return config.CalculateHeadshotDamage(baseDamage, weaponName, helmetLevel);
        }
        
        /// <summary>
        /// Get base damage for weapon (simplified values)
        /// </summary>
        private float GetWeaponBaseDamage(string weaponName)
        {
            var damageMap = new Dictionary<string, float>
            {
                // Assault Rifles
                { "AK47", 61f },
                { "M4A1", 53f },
                { "SCAR", 53f },
                { "Groza", 61f },
                { "AN94", 60f },
                { "M14", 77f },
                
                // Sniper Rifles
                { "AWM", 90f },
                { "Kar98k", 90f },
                { "M82B", 91f },
                { "SVD", 89f },
                { "SKS", 82f },
                
                // SMGs
                { "MP40", 48f },
                { "Thompson", 48f },
                { "UMP", 47f },
                { "P90", 46f },
                { "MP5", 48f },
                
                // Shotguns
                { "M1887", 100f },
                { "SPAS12", 58f },
                { "M1014", 19f },
                
                // Pistols
                { "Desert_Eagle", 89f },
                { "M1911", 62f },
                { "USP", 48f }
            };
            
            return damageMap.ContainsKey(weaponName) ? damageMap[weaponName] : 50f;
        }
        
        /// <summary>
        /// Enable or disable the auto headshot system
        /// </summary>
        public void Toggle()
        {
            IsEnabled = !IsEnabled;
            Console.WriteLine($"[AUTO HEADSHOT] {(IsEnabled ? "ENABLED" : "DISABLED")}");
        }
        
        /// <summary>
        /// Reset statistics
        /// </summary>
        public void ResetStats()
        {
            TotalShots = 0;
            HeadshotCount = 0;
        }
        
        /// <summary>
        /// Get current statistics
        /// </summary>
        public string GetStats()
        {
            return $"Shots: {TotalShots} | Headshots: {HeadshotCount} | Accuracy: {HeadshotAccuracy:F1}%";
        }
    }
    
    /// <summary>
    /// Player state information
    /// </summary>
    public class PlayerState
    {
        public Vector3 Position { get; set; }
        public Vector3 AimPosition { get; set; }
        public Vector3 AimDirection { get; set; }
        public string CurrentWeapon { get; set; }
        public string CurrentScope { get; set; }
        public float Health { get; set; }
        
        public PlayerState()
        {
            Position = new Vector3(0, 0, 0);
            AimPosition = new Vector3(0, 0, 0);
            AimDirection = new Vector3(0, 0, 1);
            CurrentWeapon = "M4A1";
            CurrentScope = "reddot";
            Health = 100f;
        }
    }
    
    /// <summary>
    /// Enemy information
    /// </summary>
    public class Enemy
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 HeadPosition { get; set; }
        public Vector3 Velocity { get; set; }
        public float Health { get; set; }
        public int HelmetLevel { get; set; }
        public bool IsAttacking { get; set; }
        
        public Enemy(string name, Vector3 position, float health = 100f, int helmetLevel = 0)
        {
            Name = name;
            Position = position;
            HeadPosition = position + new Vector3(0, 1.7f, 0); // Head is ~1.7m above ground
            Velocity = new Vector3(0, 0, 0);
            Health = health;
            HelmetLevel = helmetLevel;
            IsAttacking = false;
        }
    }
    
    /// <summary>
    /// Simple 3D Vector class
    /// </summary>
    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        
        public static Vector3 operator *(Vector3 a, float scalar)
        {
            return new Vector3(a.X * scalar, a.Y * scalar, a.Z * scalar);
        }
        
        public float Magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        
        public Vector3 Normalized()
        {
            float mag = Magnitude();
            return mag > 0 ? new Vector3(X / mag, Y / mag, Z / mag) : new Vector3(0, 0, 0);
        }
        
        public static float Distance(Vector3 a, Vector3 b)
        {
            return (a - b).Magnitude();
        }
        
        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        
        public static float AngleBetween(Vector3 a, Vector3 b)
        {
            float dot = Dot(a.Normalized(), b.Normalized());
            dot = Math.Max(-1f, Math.Min(1f, dot)); // Clamp to avoid NaN
            return (float)(Math.Acos(dot) * 180.0 / Math.PI);
        }
        
        public override string ToString()
        {
            return $"({X:F2}, {Y:F2}, {Z:F2})";
        }
    }
}
