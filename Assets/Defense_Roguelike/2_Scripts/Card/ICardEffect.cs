public interface ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount);
}

public interface ICardBaseStatCalculator
{
    float CalculateBaseStat(CardData cardData);
}

public interface ICardLevelStatCalculator
{
    float CalculateLevelStat(CardData cardData);
}

public interface ICardEventRegister
{
    void Register();
}