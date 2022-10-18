using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Type
{
    public enum Tags
    {
        Player, Enemies, Astroids, ELaser, Laser
    };

    public enum PowerUps
    {
        Tripple, Shield, Speed
    }
    
    public enum Points
    {
        Enemy = 20, Astroid = 10, PowerUp = 50, Boss = 100, MiniBoss = 50
    }
    
    public enum LaserTag
    {
        Player, Enemy
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
}
