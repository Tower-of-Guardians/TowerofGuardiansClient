namespace Jongmin
{
    public class AtkFieldSystem : FieldSystem, IATKCardDropTarget
    {
        public override void CreateCard(BattleCardData battleCardData)
        {
            if (!CanAdd)
            {
                return;
            }
            
            var card = Factory.Create();
            card.SetBattleCardData(battleCardData, CardType.AtkField);
            card.View.LockDef();
            Container.Add(card);
            Layout.UpdateLayout(false, false, false);
            GameData.Instance.attackField.Add(card.CardData);
            RequestUpdateActionCountEvent(1);
        }
    }
}