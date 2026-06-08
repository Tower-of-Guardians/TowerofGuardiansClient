namespace Jongmin
{
    public class DefFieldSystem : FieldSystem, IDEFCardDropTarget
    {
        public override void CreateCard(BattleCardData battleCardData)
        {
            if (!CanAdd)
            {
                return;
            }
            
            var card = Factory.Create();
            card.SetBattleCardData(battleCardData, CardType.DefField);
            card.View.LockAtk();
            Container.Add(card);
            Layout.UpdateLayout(false, false, false);
            GameData.Instance.attackField.Add(card.CardData);
            RequestUpdateActionCountEvent(1);
        }
    }
}