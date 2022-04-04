public class BlockCard : Card
{
    public int blockGain;

    public override void PlayCard()
    {
        GameController.Instance.block += blockGain;
        base.PlayCard();
    }
}
