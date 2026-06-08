using DG.Tweening;
using UnityEngine;

namespace Jongmin
{
    public class FieldCardLayout
    {
        private readonly FieldSystem _system;
        private readonly FieldUIDesigner _designer;
        private readonly CardContainer _container;
        private readonly PreviewCard _previewCard;

        public FieldCardLayout(FieldSystem system, FieldUIDesigner designer, CardContainer container, PreviewCard previewCard)
        {
            _system = system;
            _designer = designer;
            _container = container;
            _previewCard = previewCard;
        }

        public void UpdateLayout(bool isIncludePreview, bool isAnime = true, bool isSorting = true)
        {
            var cards = _container.Cards;
            var cardCount = cards.Count;
            
            var prevPreviewPosition 
                = cardCount > 0 ? cards[^1].RectTransform.anchoredPosition
                                : CardLayoutCalculator.CalculatedFieldCardPosition(0, _designer.ATKLimit, _designer.Space);

            CalculateCardLayout(cardCount, isAnime, isSorting);
            CalculatePreview(isIncludePreview, cardCount, prevPreviewPosition);
        }

        private void CalculateCardLayout(int cardCount, bool isAnime, bool isSorting)
        {
            var cards = _container.Cards;

            for (var i = 0; i < cardCount; i++)
            {
                if (_system.HoverCard == cards[i])
                {
                    continue;
                }
                
                var layoutPosition = CardLayoutCalculator.CalculatedFieldCardPosition(i, _designer.ATKLimit, _designer.Space);

                cards[i]?.DOKill();

                if (isAnime || isSorting || cardCount - i > 1)
                {
                    cards[i].RectTransform.DOAnchorPos(layoutPosition, _designer.AnimeDuration).SetEase(Ease.InOutSine);
                }
                else
                {
                    cards[i].RectTransform.anchoredPosition = layoutPosition;
                }
            }
        }

        private void CalculatePreview(bool isCalculate, int cardCount, Vector2 prevPreviewPosition)
        {
            if (!isCalculate)
            {
                return;
            }

            if (cardCount == _designer.ATKLimit)
            {
                return;
            }
            
            var previewPosition = CardLayoutCalculator.CalculatedFieldCardPosition(cardCount, _designer.ATKLimit, _designer.Space);
            _previewCard.RectTransform.anchoredPosition = prevPreviewPosition;
            _previewCard.RectTransform.anchoredPosition = previewPosition;
        }
    }
}