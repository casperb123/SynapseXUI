using MahApps.Metro.Controls;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace SynapseXUI.Entities
{
    public class MetroProgressBarAnimateBehavior : Behavior<MetroProgressBar>
    {
        private bool isAnimating = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            MetroProgressBar progressBar = AssociatedObject;
            progressBar.ValueChanged += ProgressBar_ValueChanged;
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isAnimating)
                return;

            isAnimating = true;

            DoubleAnimation animation = new DoubleAnimation(e.OldValue, e.NewValue, new Duration(TimeSpan.FromMilliseconds(180)), FillBehavior.Stop);
            animation.Completed += Animation_Completed;

            ((MetroProgressBar)sender).BeginAnimation(RangeBase.ValueProperty, animation);
            e.Handled = true;
        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            isAnimating = false;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            MetroProgressBar progressBar = AssociatedObject;
            progressBar.ValueChanged -= ProgressBar_ValueChanged;
        }
    }
}
