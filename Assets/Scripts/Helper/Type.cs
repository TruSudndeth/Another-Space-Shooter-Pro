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
}
