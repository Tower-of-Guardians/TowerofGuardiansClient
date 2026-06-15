using System.Collections;
using UnityEngine;

namespace Jongmin
{
    public class EffectDomain : MonoBehaviour
    {
        [SerializeField] private RectTransform cardRoot;
        [SerializeField] private EffectSystem effectSystem;
        
        [Header("References")]
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private HandDomain handDomain;
        [SerializeField] private Transform drawButton;
        [SerializeField] private Transform discardButton;

        private CardContainer _cardContainer;
        private EffectCardFactory _cardFactory;

        public void Construct()
        {
            _cardContainer = new CardContainer();
            _cardFactory = new EffectCardFactory(cardRoot);
            
            effectSystem.Construct(_cardContainer, _cardFactory);
        }

        public IEnumerator DrawHandCards(int count = -1)
        {
            yield return effectSystem.DrawHandCards(GameData.Instance.NextDeckSet(count == -1 ? turnManager.MaxHandCount : count), 
                                                    handDomain.System, 
                                                    drawButton.transform.position,
                                                    handDomain.View.transform.position);
        }

        public IEnumerator DiscardHandCards()
        {
            yield return effectSystem.DiscardHandCards(handDomain.Container.Cards, 
                                                       handDomain.System, 
                                                       discardButton.transform.position);
        }
    }
}