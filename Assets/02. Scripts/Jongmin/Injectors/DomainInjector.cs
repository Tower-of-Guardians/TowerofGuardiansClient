using UnityEngine;

namespace Jongmin
{
    public class DomainInjector : MonoBehaviour, IInjector
    {
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private HandDomain handDomain;
        [SerializeField] private FieldDomain fieldDomain;
        [SerializeField] private DiscardDomain discardDomain;
        [SerializeField] private DeckDomain deckDomain;
        [SerializeField] private ManualDomain manualDomain;
        [SerializeField] private EffectDomain effectDomain;
        [SerializeField] private NotifyDomain notifyDomain;
        
        public void Inject()
        {
            notifyDomain.Construct();
            
            var dropSystem = new CardDropSystem(
                handDomain.System, 
                fieldDomain.AtkSystem, 
                fieldDomain.DefSystem, 
                discardDomain.System, 
                turnManager, 
                notifyDomain.System
            );
            
            DIContainer.Register<CardDropSystem>(dropSystem);
            
            handDomain.Construct(dropSystem);
            fieldDomain.Construct(dropSystem);
            discardDomain.Construct(dropSystem);
            deckDomain.Construct();
            manualDomain.Construct();
            effectDomain.Construct();
        }
    }
}