using System;
using System.Collections.Generic;
using System.Linq;
using FreeFire.Config;

namespace FreeFire.AutoHeadshot
{
    /// <summary>
    /// Target Detection and Tracking System
    /// Handles enemy detection, distance calculation, and target prioritization
    /// </summary>
    public class TargetDetection
    {
        private HeadshotConfig config;
        private List<Target> trackedTargets;
        private float detectionRadius;
        
        public TargetDetection(HeadshotConfig configuration)
        {
            config = configuration;
            trackedTargets = new List<Target>();
            detectionRadius = config.AimAssistRange;
        }
        
        /// <summary>
        /// Detect all valid targets within range
        /// </summary>
        public List<Target> DetectTargets(PlayerState player, List<Enemy> enemies)
        {
            var validTargets = new List<Target>();
            
            foreach (var enemy in enemies)
            {
                // Skip dead enemies
                if (enemy.Health <= 0)
                    continue;
                
                // Calculate distance to enemy
                float distance = Vector3.Distance(player.Position, enemy.Position);
                
                // Check if within detection range
                if (distance > detectionRadius)
                    continue;
                
                // Calculate angle to enemy
                Vector3 directionToEnemy = (enemy.Position - player.Position).Normalized();
                float angleToEnemy = Vector3.AngleBetween(player.AimDirection, directionToEnemy);
                
                // Check if within field of view (FOV)
                float maxFOV = 90f; // degrees
                if (angleToEnemy > maxFOV)
                    continue;
                
                // Check line of sight (simplified - in real game would do raycast)
                bool hasLineOfSight = CheckLineOfSight(player.Position, enemy.Position);
                if (!hasLineOfSight)
                    continue;
                
                // Create target object
                var target = new Target
                {
                    Name = enemy.Name,
                    Position = enemy.Position,
                    HeadPosition = enemy.HeadPosition,
                    Velocity = enemy.Velocity,
                    Distance = distance,
                    AngleToPlayer = angleToEnemy,
                    Health = enemy.Health,
                    HelmetLevel = enemy.HelmetLevel,
                    IsAttacking = enemy.IsAttacking,
                    Priority = CalculateTargetPriority(enemy, distance, angleToEnemy),
                    LastSeenTime = DateTime.Now
                };
                
                validTargets.Add(target);
            }
            
            // Update tracked targets
            UpdateTrackedTargets(validTargets);
            
            return validTargets;
        }
        
        /// <summary>
        /// Calculate target priority score (lower is better)
        /// </summary>
        private float CalculateTargetPriority(Enemy enemy, float distance, float angle)
        {
            float priority = 0f;
            
            // Distance factor (closer = higher priority)
            priority += distance * 0.5f;
            
            // Angle factor (smaller angle = higher priority)
            priority += angle * 2.0f;
            
            // Health factor (lower health = higher priority for quick kills)
            priority += (enemy.Health / 100f) * 10f;
            
            // Attacking enemies get highest priority
            if (enemy.IsAttacking)
                priority -= 50f;
            
            // Helmet level (lower helmet = easier kill)
            priority += enemy.HelmetLevel * 5f;
            
            return priority;
        }
        
        /// <summary>
        /// Check line of sight between two points (simplified)
        /// In a real implementation, this would use raycasting
        /// </summary>
        private bool CheckLineOfSight(Vector3 from, Vector3 to)
        {
            // Simplified - always return true
            // In real game, would check for obstacles using raycast
            return true;
        }
        
        /// <summary>
        /// Update list of tracked targets
        /// </summary>
        private void UpdateTrackedTargets(List<Target> currentTargets)
        {
            // Remove targets that haven't been seen for a while
            trackedTargets.RemoveAll(t => 
                (DateTime.Now - t.LastSeenTime).TotalSeconds > 2.0);
            
            // Update or add current targets
            foreach (var target in currentTargets)
            {
                var existing = trackedTargets.FirstOrDefault(t => t.Name == target.Name);
                if (existing != null)
                {
                    // Update existing target
                    existing.Position = target.Position;
                    existing.HeadPosition = target.HeadPosition;
                    existing.Velocity = target.Velocity;
                    existing.Distance = target.Distance;
                    existing.AngleToPlayer = target.AngleToPlayer;
                    existing.Health = target.Health;
                    existing.Priority = target.Priority;
                    existing.LastSeenTime = DateTime.Now;
                }
                else
                {
                    // Add new target
                    trackedTargets.Add(target);
                }
            }
        }
        
        /// <summary>
        /// Get target by name
        /// </summary>
        public Target GetTargetByName(string name)
        {
            return trackedTargets.FirstOrDefault(t => t.Name == name);
        }
        
        /// <summary>
        /// Get closest target
        /// </summary>
        public Target GetClosestTarget()
        {
            return trackedTargets.OrderBy(t => t.Distance).FirstOrDefault();
        }
        
        /// <summary>
        /// Get highest priority target
        /// </summary>
        public Target GetHighestPriorityTarget()
        {
            return trackedTargets.OrderBy(t => t.Priority).FirstOrDefault();
        }
        
        /// <summary>
        /// Get all targets within a specific distance
        /// </summary>
        public List<Target> GetTargetsWithinDistance(float maxDistance)
        {
            return trackedTargets.Where(t => t.Distance <= maxDistance).ToList();
        }
        
        /// <summary>
        /// Get all attacking targets
        /// </summary>
        public List<Target> GetAttackingTargets()
        {
            return trackedTargets.Where(t => t.IsAttacking).ToList();
        }
        
        /// <summary>
        /// Predict target movement
        /// </summary>
        public Vector3 PredictTargetPosition(Target target, float timeAhead)
        {
            return target.Position + (target.Velocity * timeAhead);
        }
        
        /// <summary>
        /// Check if target is moving
        /// </summary>
        public bool IsTargetMoving(Target target)
        {
            return target.Velocity.Magnitude() > 0.1f;
        }
        
        /// <summary>
        /// Calculate target movement speed
        /// </summary>
        public float GetTargetSpeed(Target target)
        {
            return target.Velocity.Magnitude();
        }
        
        /// <summary>
        /// Get target movement direction
        /// </summary>
        public Vector3 GetTargetMovementDirection(Target target)
        {
            return target.Velocity.Normalized();
        }
        
        /// <summary>
        /// Check if target is behind cover (simplified)
        /// </summary>
        public bool IsTargetBehindCover(Target target, PlayerState player)
        {
            // Simplified - in real game would check for obstacles
            // For now, check if target is significantly lower than player
            float heightDifference = player.Position.Y - target.Position.Y;
            return heightDifference > 2.0f;
        }
        
        /// <summary>
        /// Calculate time to kill for a target
        /// </summary>
        public float CalculateTimeToKill(Target target, string weaponName, float fireRate)
        {
            // Get weapon damage
            float damage = GetWeaponDamage(weaponName);
            
            // Calculate headshot damage
            float headshotDamage = config.CalculateHeadshotDamage(
                damage, 
                weaponName, 
                target.HelmetLevel
            );
            
            // Calculate shots needed
            int shotsNeeded = (int)Math.Ceiling(target.Health / headshotDamage);
            
            // Calculate time (fire rate is shots per second)
            return shotsNeeded / fireRate;
        }
        
        /// <summary>
        /// Get weapon damage (simplified)
        /// </summary>
        private float GetWeaponDamage(string weaponName)
        {
            var damageMap = new Dictionary<string, float>
            {
                { "AK47", 61f }, { "M4A1", 53f }, { "SCAR", 53f },
                { "Groza", 61f }, { "AN94", 60f }, { "M14", 77f },
                { "AWM", 90f }, { "Kar98k", 90f }, { "M82B", 91f },
                { "SVD", 89f }, { "SKS", 82f },
                { "MP40", 48f }, { "Thompson", 48f }, { "UMP", 47f },
                { "P90", 46f }, { "MP5", 48f },
                { "M1887", 100f }, { "SPAS12", 58f }, { "M1014", 19f },
                { "Desert_Eagle", 89f }, { "M1911", 62f }, { "USP", 48f }
            };
            
            return damageMap.ContainsKey(weaponName) ? damageMap[weaponName] : 50f;
        }
        
        /// <summary>
        /// Clear all tracked targets
        /// </summary>
        public void ClearTrackedTargets()
        {
            trackedTargets.Clear();
        }
        
        /// <summary>
        /// Get count of tracked targets
        /// </summary>
        public int GetTrackedTargetCount()
        {
            return trackedTargets.Count;
        }
    }
    
    /// <summary>
    /// Target information class
    /// </summary>
    public class Target
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 HeadPosition { get; set; }
        public Vector3 Velocity { get; set; }
        public float Distance { get; set; }
        public float AngleToPlayer { get; set; }
        public float Health { get; set; }
        public int HelmetLevel { get; set; }
        public bool IsAttacking { get; set; }
        public float Priority { get; set; }
        public DateTime LastSeenTime { get; set; }
        
        public Target()
        {
            Name = "Unknown";
            Position = new Vector3(0, 0, 0);
            HeadPosition = new Vector3(0, 0, 0);
            Velocity = new Vector3(0, 0, 0);
            Distance = 0f;
            AngleToPlayer = 0f;
            Health = 100f;
            HelmetLevel = 0;
            IsAttacking = false;
            Priority = 0f;
            LastSeenTime = DateTime.Now;
        }
        
        public override string ToString()
        {
            return $"{Name} | Distance: {Distance:F1}m | Health: {Health:F0} | Priority: {Priority:F1}";
        }
    }
}
