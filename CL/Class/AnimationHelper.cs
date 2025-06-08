using Avalonia.Animation.Easings;
using Avalonia.Animation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using Avalonia.Styling;
using System.Threading;
using Avalonia.Media;

namespace CL.Class
{
    /// Клас для допомоги з анімаціями в Avalonia
    public class AnimationHelper
    {
        /// <summary>
        /// Анімація згладжування для елемента керування (Показ елемента)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="durationSeconds"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Анімація згладжування для елемента керування (Сховання елемента)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="durationSeconds"></param>
        /// <returns></returns>
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
        public void AnimateBorder(double targetX, double targetY, Control border)
        {
            const int durationMs = 300;
            const int fps = 60;
            int frameCount = durationMs * fps / 1000;
            int currentFrame = 0;

            if (border.RenderTransform is not TranslateTransform transform)
            {
                transform = new TranslateTransform();
                border.RenderTransform = transform;
            }

            double startX = transform.X;
            double startY = transform.Y;
            double deltaX = targetX - startX;
            double deltaY = targetY - startY;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000.0 / fps)
            };

            timer.Tick += (_, _) =>
            {
                currentFrame++;
                double t = (double)currentFrame / frameCount;

                t = 1 - Math.Pow(1 - t, 4);

                transform.X = startX + deltaX * t;
                transform.Y = startY + deltaY * t;

                if (currentFrame >= frameCount)
                    timer.Stop();
            };

            timer.Start();
        }
        /// <summary>
        /// Метод для анімації прогрес-бару
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public async Task AnimateProgressBarAsync(double from, double to, TimeSpan duration, ProgressBar progressBar)
        {
            int steps = 10;
            double stepValue = (to - from) / steps;
            int stepTime = (int)(duration.TotalMilliseconds / steps);

            for (int i = 1; i <= steps; i++)
            {
                progressBar.Value = from + (stepValue * i);
                await Task.Delay(stepTime);
            }
        }
    }
}
