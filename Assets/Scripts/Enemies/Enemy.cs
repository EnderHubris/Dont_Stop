interface IEnemy
{
    void TakeDamage(int amount, bool getAura = true);
    
    void Ignite(float duration);
    void Shock(int damage);

    bool IsDead();
}

interface IBoss
{
    void StartFight();
}