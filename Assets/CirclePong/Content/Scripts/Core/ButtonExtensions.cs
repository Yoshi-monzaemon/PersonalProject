using UniRx;
using System;
using UnityEngine.UI;
using UnityEngine;
using UniRx.Triggers;

namespace PersonalProjectExtension
{
    public static class ButtonExtensions
    {
        public static IObservable<Unit> OnClickIntentAsObservable(this Button button, float intentMilliSeconds = 300)
        {
            return button.OnClickAsObservable()
                .ThrottleFirst(TimeSpan.FromMilliseconds(intentMilliSeconds))
                .AsUnitObservable();
        }

        public static IObservable<ButtonTapType> OnClickAndHoldAsObservable(this Button button, double ms = 100, int switchNum = 5, bool isPlaySE = true)
        {
            var switchHoldTimer = Observable.Timer(TimeSpan.FromMilliseconds(ms * switchNum)).AsUnitObservable();
            var switchLongHoldTimer = Observable.Timer(TimeSpan.FromMilliseconds(ms * (switchNum*3 + 1))).AsUnitObservable();

            var onClickAsObservable = button.OnPointerUpAsObservable()
                .First()
                .Do(_ => Debug.Log("///onClick"))
                .TakeUntil(switchHoldTimer)
                .Select(_ => ButtonTapType.onClick);

            var onHoldAsObservable = Observable.Interval(TimeSpan.FromMilliseconds(ms))
                .SkipUntil(switchHoldTimer)
                .Do(_ => Debug.Log("///onHold"))
                .Select(_ => ButtonTapType.onHold)
                .TakeUntil(button.OnPointerUpAsObservable())
                .TakeUntil(switchLongHoldTimer);

            var onLongHoldAsObservable = Observable.Interval(TimeSpan.FromMilliseconds(ms))
                .SkipUntil(switchLongHoldTimer)
                .Do(_ => Debug.Log("///longHold"))
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