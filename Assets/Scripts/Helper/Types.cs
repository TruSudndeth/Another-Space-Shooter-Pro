using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Types
{
    public enum Tag
    {
        Player, Enemies, Astroids, ELaser, Laser
    };
    
    public enum Enemy
    {
        Default, Enemy, Scifi_Drone_04, Alien_Ship_001, Boss
    }

    public enum PowerUps
    {
        TripleShotPowerUp, ShieldPowerUp, SpeedPowerUp, AmmoPickup, HealthPackRed, BombPickup
    }
    
    public enum Points
    {
        Enemy = 20, Astroid = 10, PowerUp = 50, Boss = 100, MiniBoss = 50
    }
    
    public enum LaserTag
    {
        PlayerLaser, EnemyLaser
    }
    
    public enum GameState
    {
        MainNenu = 0, Level0 = 1, Level1 = 2
    }

    public enum VFX
    {
        PlayerDeath = 0, EnemyDeath = 1, AstroidDeath = 2, MiniBossDeath = 3, 
        BossDeath = 4, Laser = 5, Tripple = 6, Shield = 7, Speed = 8
    }
    
    public enum SFX
    {
        Default = 0, PlayerDeath = 1, EnemyDeath = 2, AstroidDeath = 3, MiniBossDeath = 4,
        BossDeath = 5, Laser = 6, Tripple = 7, ShieldOn = 8, ShieldOff = 9, SpeedBoost = 10, PickUp = 11,
        LaserDamage01 = 12, LaserDamage02 = 13, LaserDamage03 = 14, LaserDamage04 = 15, LaserDamage05 = 16, EnemyLaser = 17,
        MiniBossLaser = 18, BossLaser = 19, BombAlert = 20, ErrorSound = 21, UI_Hover = 22, UI_Click = 23
    }
}
