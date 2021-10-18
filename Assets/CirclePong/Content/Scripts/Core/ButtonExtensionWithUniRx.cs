using UniRx;
using System;
using UnityEngine.UI;
using UniRx.Triggers;

namespace PersonalProjectExtension
{
    public static class ButtonExtensionWithUniRx
    {
        public static IObservable<Unit> OnClickIntentAsObservable(this Button button, float intentMilliSeconds = 300)
        {
            return button.OnClickAsObservable()
                .ThrottleFirst(TimeSpan.FromMilliseconds(intentMilliSeconds))
                .AsUnitObservable();
        }

        public static IObservable<ButtonTapType> OnClickAndHoldAsObservable(this Button button, float interval = 100, int switchHoldNum = 5, int swtchLongHoldNum = 15)
        {
            var switchHoldTimer = Observable.Timer(TimeSpan.FromMilliseconds(interval * switchHoldNum)).AsUnitObservable();
            var switchLongHoldTimer = Observable.Timer(TimeSpan.FromMilliseconds(interval * (swtchLongHoldNum))).AsUnitObservable();

            var onClickAsObservable = button.OnPointerUpAsObservable()
                .First()
                .TakeUntil(switchHoldTimer)
                .Select(_ => ButtonTapType.onClick);

            var onHoldAsObservable = Observable.Interval(TimeSpan.FromMilliseconds(interval))
                .SkipUntil(switchHoldTimer)
                .TakeUntil(button.OnPointerUpAsObservable())
                .TakeUntil(switchLongHoldTimer)
                .Select(_ => ButtonTapType.onHold);

            var onLongHoldAsObservable = Observable.Interval(TimeSpan.FromMilliseconds(interval))
                .SkipUntil(switchLongHoldTimer)
                .TakeUntil(button.OnPointerUpAsObservable())
                .Select(_ => ButtonTapType.onLongHold);

            return button.OnPointerDownAsObservable()
                .SelectMany(_ =>Observable.Merge(onClickAsObservable, onHoldAsObservable, onLongHoldAsObservable));
        }
    }

    public enum ButtonTapType
    {
        onClick = 0,
        onHold = 1,
        onLongHold = 2,
    }
}