using UnityEngine;

public class HealCard : Card
{
    public int healAmount;

    public override void PlayCard()
    {
        GameController.Instance.currentHp = Mathf.Min(GameController.Instance.currentHp + healAmount, GameController.Instance.maxHp);
        base.PlayCard();
    }
}
