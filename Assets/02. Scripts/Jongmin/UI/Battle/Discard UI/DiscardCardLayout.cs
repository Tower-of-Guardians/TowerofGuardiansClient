using DG.Tweening;
using UnityEngine;

namespace Jongmin
{
    public class DiscardCardLayout
    {
        private readonly DiscardSystem _system;
        private readonly DiscardUIDesigner _designer;
        private readonly CardContainer _container;
        private readonly PreviewCard _previewCard;

        public DiscardCardLayout(DiscardSystem system, DiscardUIDesigner designer, CardContainer container, PreviewCard previewCard)
        {
            _system = system;
            _designer = designer;
            _container = container;
            _previewCard = previewCard;
        }

        public void UpdateLayout(bool isIncludePreview, bool isAnime = true, bool isSorting = true)
        {
            var cards = _container.Cards;
            
            var realCardCount = cards.Count;
            var virtualCardCount = isIncludePreview ? realCardCount + 1 : realCardCount;
            if (virtualCardCount == 0)
            {
                return;
            }

            var prevPreviewPosition = realCardCount > 0 ? cards[^1].RectTransform.anchoredPosition : Vector2.zero;

            CalculateCardLayout(realCardCount, virtualCardCount, isAnime, isSorting);
            CalculatePreview(isIncludePreview, virtualCardCount, prevPreviewPosition);
        }

        private void CalculateCardLayout(int realCardCount, int virtualCardCount, bool isAnime, bool isSorting)
        {
            var cards = _container.Cards;

            for (var i = 0; i < realCardCount; i++)
            {
                if (_system.HoverCard == cards[i])
                {
                    continue;
                }
                
                var layoutPosition = CardLayoutCalculator.CalculatedThrowCardPosition(i, virtualCardCount, _designer.Space);

                cards[i]?.DOKill();
                if (isAnime || isSorting)
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
            
            var previewLayoutPosition = CardLayoutCalculator.CalculatedThrowCardPosition(cardCount - 1, cardCount, _designer.Space);
            _previewCard.RectTransform.anchoredPosition = prevPreviewPosition;
            _previewCard.RectTransform.anchoredPosition = previewLayoutPosition;
        }
    }
}