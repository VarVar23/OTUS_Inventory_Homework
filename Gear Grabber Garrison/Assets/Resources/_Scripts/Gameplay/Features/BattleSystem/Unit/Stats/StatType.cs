namespace Gameplay.Features.Battlesystem
{   
    public enum StatType
    {
        // Movement
        MoveSpeed,
        MovePatrolRadius,
        
        // Health
        HealthMax,
        HealthRegen,
        HealthRegenRate,
        
        // Combat
        AttackClass, //класс атаки 0-бычная мечем, 1-дистанционная лук, 2-магическая
        AttackMaxDamage,
        AttackSpeed,
        AttackRange,
        
        // Stun
        StunChance,
        StunDuration,
        StunResistance,
        
        //Fire
        FireStrange,
        FireTime
        
        
    }
}
