using System;
using System.Collections.Generic;
using System.Threading;
using FreeFire.Config;
using FreeFire.AutoHeadshot;

namespace FreeFire
{
    /// <summary>
    /// Demo Program for Auto Headshot System
    /// Demonstrates the functionality of the auto headshot system
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║        FREE FIRE AUTO HEADSHOT SYSTEM - DEMO              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            
            // Load configuration
            Console.WriteLine("[SYSTEM] Loading configuration...");
            var config = HeadshotConfig.LoadDefault();
            
            // Validate configuration
            if (config.ValidateConfig())
            {
                Console.WriteLine("[SYSTEM] Configuration validated successfully!");
            }
            else
            {
                Console.WriteLine("[ERROR] Configuration validation failed!");
                return;
            }
            
            Console.WriteLine();
            Console.WriteLine("Configuration Settings:");
            Console.WriteLine($"  - Aim Assist: {(config.EnableAimAssist ? "ENABLED" : "DISABLED")}");
            Console.WriteLine($"  - Aim Assist Strength: {config.AimAssistStrength * 100}%");
            Console.WriteLine($"  - Aim Assist Range: {config.AimAssistRange}m");
            Console.WriteLine($"  - Head Tracking Speed: {config.HeadTrackingSpeed}x");
            Console.WriteLine($"  - Recoil Compensation: {(config.EnableRecoilCompensation ? "ENABLED" : "DISABLED")}");
            Console.WriteLine($"  - Predictive Aiming: {(config.PredictiveAiming ? "ENABLED" : "DISABLED")}");
            Console.WriteLine();
            
            // Initialize auto headshot system
            Console.WriteLine("[SYSTEM] Initializing Auto Headshot System...");
            var autoHeadshot = new AutoHeadshot(config);
            autoHeadshot.IsEnabled = true;
            autoHeadshot.AutoFireEnabled = true;
            Console.WriteLine("[SYSTEM] Auto Headshot System initialized!");
            Console.WriteLine();
            
            // Run demo scenarios
            Console.WriteLine("Press any key to start demo scenarios...");
            Console.ReadKey();
            Console.WriteLine();
            
            // Scenario 1: Close Range Combat
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("SCENARIO 1: Close Range Combat (10-30m)");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            RunCloseRangeCombat(autoHeadshot, config);
            
            Console.WriteLine();
            Console.WriteLine("Press any key to continue to next scenario...");
            Console.ReadKey();
            Console.WriteLine();
            
            // Scenario 2: Medium Range Combat
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("SCENARIO 2: Medium Range Combat (30-70m)");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            RunMediumRangeCombat(autoHeadshot, config);
            
            Console.WriteLine();
            Console.WriteLine("Press any key to continue to next scenario...");
            Console.ReadKey();
            Console.WriteLine();
            
            // Scenario 3: Long Range Sniping
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("SCENARIO 3: Long Range Sniping (70-150m)");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            RunLongRangeSniping(autoHeadshot, config);
            
            Console.WriteLine();
            Console.WriteLine("Press any key to continue to next scenario...");
            Console.ReadKey();
            Console.WriteLine();
            
            // Scenario 4: Multiple Targets
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("SCENARIO 4: Multiple Target Engagement");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            RunMultipleTargets(autoHeadshot, config);
            
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("FINAL STATISTICS");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine(autoHeadshot.GetStats());
            Console.WriteLine();
            
            // Display weapon comparison
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("WEAPON HEADSHOT DAMAGE COMPARISON");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            DisplayWeaponComparison(config);
            
            Console.WriteLine();
            Console.WriteLine("Demo completed! Press any key to exit...");
            Console.ReadKey();
        }
        
        /// <summary>
        /// Scenario 1: Close Range Combat
        /// </summary>
        static void RunCloseRangeCombat(AutoHeadshot autoHeadshot, HeadshotConfig config)
        {
            Console.WriteLine("[INFO] Weapon: M4A1 with Red Dot Sight");
            Console.WriteLine("[INFO] Engaging enemies at close range...");
            Console.WriteLine();
            
            // Create player state
            var player = new PlayerState
            {
                Position = new Vector3(0, 0, 0),
                AimPosition = new Vector3(0, 1.7f, 10),
                AimDirection = new Vector3(0, 0, 1),
                CurrentWeapon = "M4A1",
                CurrentScope = "reddot",
                Health = 100f
            };
            
            // Create enemies
            var enemies = new List<Enemy>
            {
                new Enemy("Enemy_1", new Vector3(2, 0, 15), 100f, 1),
                new Enemy("Enemy_2", new Vector3(-3, 0, 20), 80f, 0)
            };
            
            // Simulate combat
            SimulateCombat(autoHeadshot, player, enemies, 5, 0.016f);
        }
        
        /// <summary>
        /// Scenario 2: Medium Range Combat
        /// </summary>
        static void RunMediumRangeCombat(AutoHeadshot autoHeadshot, HeadshotConfig config)
        {
            Console.WriteLine("[INFO] Weapon: AK47 with 2x Scope");
            Console.WriteLine("[INFO] Engaging enemies at medium range...");
            Console.WriteLine();
            
            var player = new PlayerState
            {
                Position = new Vector3(0, 0, 0),
                AimPosition = new Vector3(0, 1.7f, 50),
                AimDirection = new Vector3(0, 0, 1),
                CurrentWeapon = "AK47",
                CurrentScope = "2x",
                Health = 100f
            };
            
            var enemies = new List<Enemy>
            {
                new Enemy("Enemy_3", new Vector3(5, 0, 45), 100f, 2),
                new Enemy("Enemy_4", new Vector3(-4, 0, 55), 100f, 1)
            };
            
            // Add some movement to enemies
            enemies[0].Velocity = new Vector3(2, 0, 0);
            enemies[1].Velocity = new Vector3(-1.5f, 0, 0);
            
            SimulateCombat(autoHeadshot, player, enemies, 8, 0.016f);
        }
        
        /// <summary>
        /// Scenario 3: Long Range Sniping
        /// </summary>
        static void RunLongRangeSniping(AutoHeadshot autoHeadshot, HeadshotConfig config)
        {
            Console.WriteLine("[INFO] Weapon: AWM with 8x Scope");
            Console.WriteLine("[INFO] Engaging enemies at long range...");
            Console.WriteLine();
            
            var player = new PlayerState
            {
                Position = new Vector3(0, 0, 0),
                AimPosition = new Vector3(0, 1.7f, 100),
                AimDirection = new Vector3(0, 0, 1),
                CurrentWeapon = "AWM",
                CurrentScope = "8x",
                Health = 100f
            };
            
            var enemies = new List<Enemy>
            {
                new Enemy("Enemy_5", new Vector3(3, 0, 120), 100f, 3),
                new Enemy("Enemy_6", new Vector3(-2, 0, 95), 100f, 2)
            };
            
            // Add movement
            enemies[0].Velocity = new Vector3(1, 0, 0.5f);
            enemies[1].Velocity = new Vector3(-0.8f, 0, -0.3f);
            
            SimulateCombat(autoHeadshot, player, enemies, 4, 0.016f);
        }
        
        /// <summary>
        /// Scenario 4: Multiple Targets
        /// </summary>
        static void RunMultipleTargets(AutoHeadshot autoHeadshot, HeadshotConfig config)
        {
            Console.WriteLine("[INFO] Weapon: Groza with 4x Scope");
            Console.WriteLine("[INFO] Engaging multiple enemies...");
            Console.WriteLine();
            
            var player = new PlayerState
            {
                Position = new Vector3(0, 0, 0),
                AimPosition = new Vector3(0, 1.7f, 40),
                AimDirection = new Vector3(0, 0, 1),
                CurrentWeapon = "Groza",
                CurrentScope = "4x",
                Health = 100f
            };
            
            var enemies = new List<Enemy>
            {
                new Enemy("Enemy_7", new Vector3(8, 0, 35), 100f, 1),
                new Enemy("Enemy_8", new Vector3(-6, 0, 40), 80f, 0),
                new Enemy("Enemy_9", new Vector3(0, 0, 50), 100f, 2),
                new Enemy("Enemy_10", new Vector3(10, 0, 45), 60f, 1)
            };
            
            // Mark some as attacking
            enemies[1].IsAttacking = true;
            enemies[3].IsAttacking = true;
            
            // Add movement
            enemies[0].Velocity = new Vector3(-1, 0, 0);
            enemies[1].Velocity = new Vector3(1.5f, 0, 0.5f);
            enemies[2].Velocity = new Vector3(0, 0, -1);
            enemies[3].Velocity = new Vector3(-2, 0, 0);
            
            SimulateCombat(autoHeadshot, player, enemies, 12, 0.016f);
        }
        
        /// <summary>
        /// Simulate combat scenario
        /// </summary>
        static void SimulateCombat(
            AutoHeadshot autoHeadshot, 
            PlayerState player, 
            List<Enemy> enemies, 
            int frames, 
            float deltaTime)
        {
            for (int i = 0; i < frames; i++)
            {
                // Update enemy positions based on velocity
                foreach (var enemy in enemies)
                {
                    enemy.Position = enemy.Position + (enemy.Velocity * deltaTime);
                    enemy.HeadPosition = enemy.Position + new Vector3(0, 1.7f, 0);
                }
                
                // Update auto headshot system
                autoHeadshot.Update(deltaTime, player, enemies);
                
                // Small delay for visualization
                Thread.Sleep(100);
                
                // Remove dead enemies
                enemies.RemoveAll(e => e.Health <= 0);
                
                if (enemies.Count == 0)
                {
                    Console.WriteLine("[INFO] All enemies eliminated!");
                    break;
                }
            }
            
            Console.WriteLine();
            Console.WriteLine($"[STATS] {autoHeadshot.GetStats()}");
        }
        
        /// <summary>
        /// Display weapon headshot damage comparison
        /// </summary>
        static void DisplayWeaponComparison(HeadshotConfig config)
        {
            var weapons = new List<string> 
            { 
                "AK47", "M4A1", "Groza", "AWM", "Kar98k", 
                "MP40", "M1887", "Desert_Eagle" 
            };
            
            var baseDamages = new Dictionary<string, float>
            {
                { "AK47", 61f }, { "M4A1", 53f }, { "Groza", 61f },
                { "AWM", 90f }, { "Kar98k", 90f },
                { "MP40", 48f }, { "M1887", 100f }, { "Desert_Eagle", 89f }
            };
            
            Console.WriteLine($"{"Weapon",-15} {"Base",-8} {"No Helmet",-12} {"Level 1",-12} {"Level 2",-12} {"Level 3",-12}");
            Console.WriteLine(new string('─', 75));
            
            foreach (var weapon in weapons)
            {
                float baseDmg = baseDamages[weapon];
                float noHelmet = config.CalculateHeadshotDamage(baseDmg, weapon, 0);
                float level1 = config.CalculateHeadshotDamage(baseDmg, weapon, 1);
                float level2 = config.CalculateHeadshotDamage(baseDmg, weapon, 2);
                float level3 = config.CalculateHeadshotDamage(baseDmg, weapon, 3);
                
                Console.WriteLine($"{weapon,-15} {baseDmg,-8:F0} {noHelmet,-12:F1} {level1,-12:F1} {level2,-12:F1} {level3,-12:F1}");
            }
        }
    }
}
