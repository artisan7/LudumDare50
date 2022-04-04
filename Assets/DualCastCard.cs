public class DualCastCard : Card
{
    public override void PlayCard()
    {
        GameController.Instance.hasDualCast = true;
        GameController.Instance.dualCastIndicator.gameObject.SetActive(true);
        base.PlayCard();
    }
}
