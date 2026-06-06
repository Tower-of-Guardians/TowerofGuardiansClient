using UnityEngine;

namespace Jongmin
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private CardView view;
        [SerializeField] private CardPointer pointer;

        public BattleCardData BattleCardData { get; private set; }
        public CardData CardData { get; private set; }

        public CardPointer Pointer => pointer;

        private void Awake()
        {
            view ??= GetComponent<CardView>();
            pointer ??= GetComponent<CardPointer>();
            pointer?.SetOwner(this);
        }

        public void SetBattleCardData(BattleCardData battleCardData)
        {
            BattleCardData = battleCardData;
            CardData = battleCardData?.data;
            view.UpdateModel(CardData);
        }

        public void SetCardData(CardData cardData)
        {
            BattleCardData = null;
            CardData = cardData;
            view.UpdateModel(CardData);
        }

        private void OnDisable()
        {
            BattleCardData = null;
            CardData = null;
        }
    }
}
