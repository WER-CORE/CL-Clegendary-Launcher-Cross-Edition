using Avalonia.Animation.Easings;
using Avalonia.Animation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using Avalonia.Styling;
using System.Threading;

namespace CL.Class
{
    public class AnimationHelper
    {
        public async Task FadeInAsync(Control element, double durationSeconds)
        {
            if (element == null) return;

            element.IsVisible = true;

            var animation = new Animation
            {
                Duration = TimeSpan.FromSeconds(durationSeconds),
                FillMode = FillMode.Forward,
                Easing = new QuadraticEaseInOut()
            };

            animation.Children.Add(new KeyFrame { Cue = new Cue(0), Setters = { new Setter(Control.OpacityProperty, 0.0) } });
            animation.Children.Add(new KeyFrame { Cue = new Cue(1), Setters = { new Setter(Control.OpacityProperty, 1.0) } });

            await animation.RunAsync(element, CancellationToken.None);
        }

        public async Task FadeOutAsync(Control element, double durationSeconds)
        {
            if (element == null) return;

            var animation = new Animation
            {
                Duration = TimeSpan.FromSeconds(durationSeconds),
                FillMode = FillMode.Forward,
                Easing = new QuadraticEaseInOut()
            };

            animation.Children.Add(new KeyFrame { Cue = new Cue(0), Setters = { new Setter(Control.OpacityProperty, 1.0) } });
            animation.Children.Add(new KeyFrame { Cue = new Cue(1), Setters = { new Setter(Control.OpacityProperty, 0.0) } });

            await animation.RunAsync(element, CancellationToken.None);

            element.IsVisible = false;
        }

    }
}
