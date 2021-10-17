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

        public static IObservable<ButtonTapType> OnClickAndHoldAsObservable(this Button button, double ms = 1000, int switchNum = 5, bool isPlaySE = true)
        {
            const float DEFAULT_HOLD_SPAN = 200;
            var holdTimer = Observable.Timer(TimeSpan.FromMilliseconds(ms)).AsUnitObservable();
            var longHoldTimer = Observable.Timer(TimeSpan.FromMilliseconds(ms + DEFAULT_HOLD_SPAN * switchNum)).AsUnitObservable();

            var onClickAsObservable = button.OnPointerUpAsObservable()
                .Do(_ => Debug.Log("///onClick"))
                .TakeUntil(holdTimer)
                .Select(_ => ButtonTapType.onClick)
                .First();

            var onHoldAsObservable = Observable.Interval(TimeSpan.FromMilliseconds(100))
                .SkipUntil(holdTimer)
                .Do(_ => Debug.Log("///onHold"))
                .TakeUntil(button.OnPointerUpAsObservable())
                .TakeUntil(longHoldTimer)
                .Select(_ => ButtonTapType.onHold);

            var onLongHoldAsObservable = Observable.Interval(TimeSpan.FromMilliseconds(100))
                .SkipUntil(longHoldTimer)
                .Do(_ => Debug.Log("///longHold"))
                .TakeUntil(button.OnPointerUpAsObservable())
                .Select(_ => ButtonTapType.onLongHold);

            return button.OnPointerDownAsObservable()
                .SelectMany(_ =>{
                    Debug.Log("///PointerDown");
                    return Observable.Merge(onClickAsObservable, onHoldAsObservable, onLongHoldAsObservable);
                });
        }
    }

    public enum ButtonTapType
    {
        onClick = 0,
        onHold = 1,
        onLongHold = 2,
    }
}