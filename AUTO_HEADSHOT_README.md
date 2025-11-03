# Free Fire Auto Headshot System

## Overview
A comprehensive auto headshot system for Free Fire game mechanics, featuring advanced aim assistance, target detection, and automatic headshot execution.

## Features

### 1. **Auto Headshot System** (`AutoHeadshot.cs`)
- Main coordination system for auto headshot functionality
- Automatic target selection and prioritization
- Auto-fire capability when target is in crosshair
- Real-time statistics tracking (shots, headshots, accuracy)
- Integration with existing `HeadshotConfig.cs`

### 2. **Advanced Aim Bot** (`HeadshotAimBot.cs`)
- Smooth aim transitions to target heads
- Predictive aiming for moving targets
- Bullet drop compensation for long-range shots
- Weapon-specific recoil patterns and compensation
- Distance-based aim scaling
- Natural movement smoothing

### 3. **Target Detection** (`TargetDetection.cs`)
- Enemy detection within configurable range
- Line of sight checking
- Target prioritization based on:
  - Distance (closer = higher priority)
  - Angle to player (smaller angle = higher priority)
  - Target health (lower health = easier kill)
  - Threat level (attacking enemies = highest priority)
  - Helmet level (lower helmet = easier kill)
- Target tracking and prediction
- Movement detection and speed calculation

### 4. **Configuration System** (`HeadshotConfig.cs`)
- Comprehensive configuration options
- Weapon-specific headshot multipliers
- Aim assist settings
- Recoil control settings
- Sensitivity settings for different scopes
- Graphics and audio optimization

## System Architecture

```
AutoHeadshot (Main Controller)
    ├── TargetDetection (Enemy Detection & Tracking)
    │   ├── Detect enemies within range
    │   ├── Calculate priorities
    │   └── Track target movements
    │
    ├── HeadshotAimBot (Aim Assistance)
    │   ├── Calculate aim adjustments
    │   ├── Predict target positions
    │   ├── Compensate for recoil
    │   └── Apply bullet drop compensation
    │
    └── HeadshotConfig (Configuration)
        ├── Weapon multipliers
        ├── Aim assist settings
        └── Sensitivity settings
```

## Weapon Support

### Assault Rifles
- AK47 (2.05x headshot multiplier)
- M4A1 (2.0x headshot multiplier)
- SCAR (2.0x headshot multiplier)
- Groza (2.1x headshot multiplier)
- AN94 (2.0x headshot multiplier)
- M14 (2.15x headshot multiplier)

### Sniper Rifles
- AWM (2.95x headshot multiplier)
- Kar98k (2.8x headshot multiplier)
- M82B (3.0x headshot multiplier)
- SVD (2.5x headshot multiplier)
- SKS (2.4x headshot multiplier)

### SMGs
- MP40 (1.9x headshot multiplier)
- Thompson (1.9x headshot multiplier)
- UMP (1.85x headshot multiplier)
- P90 (1.85x headshot multiplier)
- MP5 (1.9x headshot multiplier)

### Shotguns
- M1887 (1.5x headshot multiplier)
- SPAS12 (1.5x headshot multiplier)
- M1014 (1.5x headshot multiplier)

### Pistols
- Desert Eagle (2.2x headshot multiplier)
- M1911 (2.0x headshot multiplier)
- USP (1.95x headshot multiplier)

## Usage Example

```csharp
// Initialize configuration
var config = HeadshotConfig.LoadDefault();

// Create auto headshot system
var autoHeadshot = new AutoHeadshot(config);
autoHeadshot.IsEnabled = true;
autoHeadshot.AutoFireEnabled = true;

// Create player state
var player = new PlayerState
{
    Position = new Vector3(0, 0, 0),
    AimPosition = new Vector3(0, 1.7f, 50),
    AimDirection = new Vector3(0, 0, 1),
    CurrentWeapon = "M4A1",
    CurrentScope = "reddot",
    Health = 100f
};

// Create enemies
var enemies = new List<Enemy>
{
    new Enemy("Enemy_1", new Vector3(5, 0, 45), 100f, 2),
    new Enemy("Enemy_2", new Vector3(-4, 0, 55), 100f, 1)
};

// Update loop (call every frame)
autoHeadshot.Update(deltaTime, player, enemies);

// Get statistics
Console.WriteLine(autoHeadshot.GetStats());
```

## Demo Scenarios

The `Program.cs` includes 4 demo scenarios:

1. **Close Range Combat (10-30m)**
   - Weapon: M4A1 with Red Dot
   - Fast-paced close quarters combat

2. **Medium Range Combat (30-70m)**
   - Weapon: AK47 with 2x Scope
   - Moving targets at medium distance

3. **Long Range Sniping (70-150m)**
   - Weapon: AWM with 8x Scope
   - Precision shots with bullet drop compensation

4. **Multiple Target Engagement**
   - Weapon: Groza with 4x Scope
   - Priority-based target switching

## Compilation

### Using Mono Compiler (mcs)
```bash
mcs -out:AutoHeadshot.exe HeadshotConfig.cs AutoHeadshot.cs HeadshotAimBot.cs TargetDetection.cs Program.cs
```

### Using .NET Compiler (csc)
```bash
csc /out:AutoHeadshot.exe HeadshotConfig.cs AutoHeadshot.cs HeadshotAimBot.cs TargetDetection.cs Program.cs
```

### Using .NET CLI
```bash
dotnet new console -n FreeFire
# Copy all .cs files to the project
dotnet build
dotnet run
```

## Running the Demo

```bash
# After compilation
./AutoHeadshot.exe

# Or with mono
mono AutoHeadshot.exe

# Or with .NET
dotnet run
```

## Key Features Explained

### 1. Target Prioritization
The system automatically selects the best target based on:
- **Distance**: Closer enemies are prioritized
- **Angle**: Enemies in your crosshair are prioritized
- **Health**: Lower health enemies for quick eliminations
- **Threat**: Attacking enemies get highest priority
- **Helmet Level**: Enemies with weaker helmets are easier targets

### 2. Predictive Aiming
- Calculates bullet travel time based on distance
- Predicts target position based on velocity
- Adjusts aim point to intercept moving targets
- Configurable prediction factor (0.0 - 1.0)

### 3. Recoil Compensation
- Weapon-specific recoil patterns
- Automatic vertical and horizontal compensation
- Scales with number of shots fired
- Configurable compensation strength

### 4. Bullet Drop Compensation
- Distance-based bullet drop calculation
- Weapon-specific adjustments (snipers have less drop)
- Automatic elevation adjustment for long-range shots

### 5. Smooth Aim Movement
- Natural-looking aim transitions
- Configurable smoothing factor
- Distance-based aim speed scaling
- Prevents unrealistic aim snapping

## Configuration Options

### Aim Assist
- `EnableAimAssist`: Enable/disable aim assistance
- `AimAssistStrength`: Strength of aim assistance (0.0 - 1.0)
- `AimAssistRange`: Maximum range for aim assist (meters)
- `HeadTrackingSpeed`: Speed of head tracking (multiplier)

### Recoil Control
- `EnableRecoilCompensation`: Enable/disable recoil compensation
- `VerticalRecoilControl`: Vertical recoil reduction (0.0 - 1.0)
- `HorizontalRecoilControl`: Horizontal recoil reduction (0.0 - 1.0)

### Prediction
- `PredictiveAiming`: Enable/disable target prediction
- `PredictionFactor`: Strength of prediction (0.0 - 1.0)
- `BulletSpeed`: Bullet speed multiplier
- `BulletDrop`: Bullet drop multiplier

### Sensitivity (per scope type)
- `GeneralSensitivity`: Base sensitivity
- `RedDotSensitivity`: Red dot sight sensitivity
- `TwoXScopeSensitivity`: 2x scope sensitivity
- `FourXScopeSensitivity`: 4x scope sensitivity
- `EightXScopeSensitivity`: 8x scope sensitivity

## Statistics Tracking

The system tracks:
- Total shots fired
- Total headshots landed
- Headshot accuracy percentage
- Real-time combat feedback

## File Structure

```
/vercel/sandbox/
├── HeadshotConfig.cs       # Configuration system
├── AutoHeadshot.cs         # Main auto headshot controller
├── HeadshotAimBot.cs       # Aim assistance implementation
├── TargetDetection.cs      # Target detection and tracking
├── Program.cs              # Demo application
└── AUTO_HEADSHOT_README.md # This file
```

## Technical Details

### Vector3 Class
Custom 3D vector implementation with:
- Basic arithmetic operations (+, -, *)
- Magnitude and normalization
- Distance calculation
- Dot product
- Angle calculation

### PlayerState Class
Tracks player information:
- Position and aim position
- Aim direction
- Current weapon and scope
- Health status

### Enemy Class
Tracks enemy information:
- Position and head position
- Velocity (for prediction)
- Health and helmet level
- Attack status

### Target Class
Enhanced enemy information with:
- Distance to player
- Angle to player
- Priority score
- Last seen timestamp

## Performance Considerations

- Efficient target detection with range filtering
- FOV-based culling to reduce processing
- Smooth aim transitions to prevent frame drops
- Configurable update rates
- Optimized vector calculations

## Safety Features

- Maximum aim adjustment limits (prevents unrealistic snapping)
- Smooth aim transitions (looks natural)
- Configurable strength settings (adjustable to preference)
- Validation of configuration values
- Range-based target filtering

## Future Enhancements

Potential improvements:
- Advanced obstacle detection (cover system)
- Team identification (friend/foe)
- Weapon-specific optimal ranges
- Dynamic sensitivity adjustment
- Machine learning for pattern recognition
- Advanced movement prediction algorithms

## Notes

- This is a demonstration/educational project
- All values are based on Free Fire game mechanics
- Weapon damage and multipliers are approximate
- System designed for single-player/training scenarios
- Configuration can be adjusted for different playstyles

## License

Educational and demonstration purposes only.

## Author

Created for Free Fire game mechanics demonstration.
