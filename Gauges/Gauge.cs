﻿namespace Gauges
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    [TemplatePart(Name = IndicatorTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TrackTemplateName, Type = typeof(FrameworkElement))]
    public class Gauge : RangeBase
    {
        public static readonly DependencyProperty MarkerProperty = DependencyProperty.Register(
            "Marker", typeof(Marker), typeof(Gauge), new PropertyMetadata(default(Marker)));

        public static readonly DependencyProperty ShowTrackProperty = DependencyProperty.Register(
            "ShowTrack",
            typeof(bool),
            typeof(Gauge),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShowLabelsProperty = DependencyProperty.Register(
            "ShowLabels",
            typeof(bool),
            typeof(Gauge),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShowMajorTicksProperty = DependencyProperty.Register(
            "ShowMajorTicks",
            typeof(bool),
            typeof(Gauge),
            new PropertyMetadata(true));

        public static readonly DependencyProperty TickFrequencyProperty = TickBar.TickFrequencyProperty.AddOwner(
            typeof(Gauge),
            new FrameworkPropertyMetadata(-1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TicksProperty = TickBar.TicksProperty.AddOwner(
            typeof(Gauge),
            new FrameworkPropertyMetadata(new DoubleCollection(), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty PlacementProperty = TickBar.PlacementProperty.AddOwner(
            typeof(Gauge),
            new FrameworkPropertyMetadata(default(TickBarPlacement), FrameworkPropertyMetadataOptions.AffectsRender, OnPlacementChanged));

        private const string IndicatorTemplateName = "PART_Indicator";
        private const string TrackTemplateName = "PART_Track";
        private readonly TranslateTransform indicatorTransform = new TranslateTransform();
        private FrameworkElement indicator;
        private FrameworkElement track;

        static Gauge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Gauge), new FrameworkPropertyMetadata(typeof(Gauge)));
        }

        public Gauge()
        {
        }

        public Marker Marker
        {
            get { return (Marker)this.GetValue(MarkerProperty); }
            set { this.SetValue(MarkerProperty, value); }
        }

        public bool ShowLabels
        {
            get { return (bool)GetValue(ShowLabelsProperty); }
            set { SetValue(ShowLabelsProperty, value); }
        }

        public bool ShowTrack
        {
            get { return (bool)GetValue(ShowTrackProperty); }
            set { SetValue(ShowTrackProperty, value); }
        }

        public bool ShowMajorTicks
        {
            get { return (bool)GetValue(ShowMajorTicksProperty); }
            set { SetValue(ShowMajorTicksProperty, value); }
        }

        public Double TickFrequency
        {
            get { return (Double)this.GetValue(TickFrequencyProperty); }
            set { this.SetValue(TickFrequencyProperty, value); }
        }

        public TickBarPlacement Placement
        {
            get { return (TickBarPlacement)this.GetValue(PlacementProperty); }
            set { this.SetValue(PlacementProperty, value); }
        }

        public DoubleCollection Ticks
        {
            get { return (DoubleCollection)this.GetValue(TicksProperty); }
            set { this.SetValue(TicksProperty, value); }
        }

        /// <summary>
        ///     Called when a template is applied to a <see cref="T:System.Windows.Controls.ProgressBar" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (this.Template == null)
            {
                return;
            }

            base.OnApplyTemplate();
            this.indicator = this.GetTemplateChild(IndicatorTemplateName) as FrameworkElement;
            this.track = this.GetTemplateChild(TrackTemplateName) as FrameworkElement;
            if (this.indicator != null)
            {
                this.indicator.RenderTransform = this.indicatorTransform;
                this.indicator.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        /// <summary>
        ///     Updates the current position of the <see cref="T:System.Windows.Controls.ProgressBar" /> when the
        ///     <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum" /> property changes.
        /// </summary>
        /// <param name="oldMinimum">
        ///     Old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum" />
        ///     property.
        /// </param>
        /// <param name="newMinimum">
        ///     New value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum" />
        ///     property.
        /// </param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            this.SetIndicatorPos();
        }

        /// <summary>
        ///     Updates the current position of the <see cref="T:System.Windows.Controls.ProgressBar" /> when the
        ///     <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum" /> property changes.
        /// </summary>
        /// <param name="oldMaximum">
        ///     Old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum" />
        ///     property.
        /// </param>
        /// <param name="newMaximum">
        ///     New value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum" />
        ///     property.
        /// </param>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            this.SetIndicatorPos();
        }

        /// <summary>
        ///     Updates the current position of the <see cref="T:System.Windows.Controls.ProgressBar" /> when the
        ///     <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value" /> property changes.
        /// </summary>
        /// <param name="oldValue">Old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value" /> property.</param>
        /// <param name="newValue">New value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value" /> property.</param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            this.SetIndicatorPos();
        }

        private static void OnPlacementChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var gauge = (Gauge)o;
            if (gauge == null)
            {
                return;
            }
            gauge.SetIndicatorPos();
        }

        private void SetIndicatorPos()
        {
            if (this.indicator == null || this.track == null)
            {
                return;
            }

            Point p = this.PosFromValue(this.Value);
            if (this.Placement == TickBarPlacement.Bottom || this.Placement == TickBarPlacement.Top)
            {
                this.indicatorTransform.SetCurrentValue(TranslateTransform.YProperty, p.Y);
                var animation = new DoubleAnimation(this.track.ActualWidth * (p.X - 0.5), TimeSpan.FromMilliseconds(200));
                this.indicatorTransform.BeginAnimation(TranslateTransform.XProperty, animation);
            }
            else
            {
                this.indicatorTransform.SetCurrentValue(TranslateTransform.XProperty, p.X);
                var animation = new DoubleAnimation(this.track.ActualHeight * (0.5 - p.Y), TimeSpan.FromMilliseconds(200));
                this.indicatorTransform.BeginAnimation(TranslateTransform.YProperty, animation);
            }
        }

        private Point PosFromValue(double value)
        {
            if (this.Placement == TickBarPlacement.Bottom || this.Placement == TickBarPlacement.Top)
            {
                double minimum = this.Minimum;
                double maximum = this.Maximum;
                var x = (value - minimum) / Math.Abs(maximum - minimum);
                var y = 0;
                return new Point(x, y);
            }
            else
            {
                double minimum = this.Minimum;
                double maximum = this.Maximum;
                var x = 0;
                var y = (value - minimum) / Math.Abs(maximum - minimum);
                return new Point(x, y);
            }
        }
    }
}