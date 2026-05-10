public sealed class WaitingRecipe
{
    public RecipeSO RecipeSO { get; }
    public float TimeLimit { get; }
    public float TimeRemaining { get; private set; }
    public float TimeNormalised => TimeRemaining / TimeLimit;
    public bool IsExpired => TimeRemaining <= 0f;

    public WaitingRecipe(RecipeSO recipeSO, float timeLimit)
    {
        RecipeSO = recipeSO;
        TimeLimit = timeLimit;
        TimeRemaining = timeLimit;
    }

    public bool Tick(float deltaTime)
    {
        TimeRemaining -= deltaTime;
        return IsExpired;
    }
}