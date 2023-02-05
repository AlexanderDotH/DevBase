﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace DevBase.Avalonia.Scaling;

/// <summary>
    /// Represents an UI object containing multiple <see cref="IScalable"/>s or <see cref="ScalableObject"/>s itself.
    /// </summary>
    public class ScalableObject
    {
        /// <summary>
        /// The <see cref="Action"/> to be invoked before this <see cref="ScalableObject"/> begins resizing.
        /// </summary>
        public Action PreScalingAction { get; }
        /// <summary>
        /// The <see cref="Action"/> to be invoked after this <see cref="ScalableObject"/> completed resizing.
        /// </summary>
        public Action PostScalingAction { get; }
        /// <summary>
        /// The bindings associated with this <see cref="ScalableObject"/>.
        /// </summary>
        public Dictionary<string, IScalable> Bindings { get; } = new Dictionary<string, IScalable>();

        /// <summary>
        /// Initializes a new <see cref="ScalableObject"/> from the provided <paramref name="avaloniaObject"/>.
        /// </summary>
        /// <param name="avaloniaObject">The <see cref="AvaloniaObject"/> to be mapped to this new instance of <see cref="ScalableObject"/>.</param>
        public ScalableObject(AvaloniaObject avaloniaObject)
        {
            if (avaloniaObject is TextBlock textBlock)
            {
                Register(avaloniaObject, TextBlock.FontSizeProperty, textBlock.FontSize);
            }
            if (avaloniaObject is TemplatedControl templatedControl)
            {
                Register(avaloniaObject, TemplatedControl.FontSizeProperty, templatedControl.FontSize);
            }
            if (avaloniaObject is Border border)
            {
                Register(avaloniaObject, Border.CornerRadiusProperty, border.CornerRadius);
            }
            if (avaloniaObject is Decorator decorator)
            {
                Register(avaloniaObject, Decorator.PaddingProperty, decorator.Padding);
            }
            
            if (avaloniaObject is Layoutable layoutable)
            {
                Register(avaloniaObject, Layoutable.WidthProperty, layoutable.Width);
                Register(avaloniaObject, Layoutable.HeightProperty, layoutable.Height);
                Register(avaloniaObject, Layoutable.MarginProperty, layoutable.Margin);
            }
        }

        private IScalable Register(AvaloniaObject avaloniaObject, AvaloniaProperty<double> avaloniaProperty, double defaultValue)
        {
            return Register<ScalableDouble, double>(avaloniaObject, avaloniaProperty, defaultValue);
        }

        private IScalable Register(AvaloniaObject avaloniaObject, AvaloniaProperty<CornerRadius> avaloniaProperty, CornerRadius defaultValue)
        {
            return Register<ScalableCornerRadius, CornerRadius>(avaloniaObject, avaloniaProperty, defaultValue);
        }

        private IScalable Register(AvaloniaObject avaloniaObject, AvaloniaProperty<Thickness> avaloniaProperty, Thickness defaultValue)
        {
            return Register<ScalableThickness, Thickness>(avaloniaObject, avaloniaProperty, defaultValue);
        }

        private IScalable Register<T1, T2>(AvaloniaObject avaloniaObject, AvaloniaProperty<T2> avaloniaProperty, T2 defaultValue) where T1 : Scalable<T2>, new()
        {
            string key = avaloniaProperty.Name;
            if (!Bindings.TryGetValue(key, out IScalable scalable))
            {
                T1 newScalable = new T1();
                newScalable.Initialize(avaloniaObject, avaloniaProperty, defaultValue);
                scalable = newScalable;
                Bindings.Add(key, scalable);
            }
            return scalable;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj);
        }

        /// <summary>
        /// Applies the specified <paramref name="scalingFactor"/> to this <see cref="ScalableObject"/> and all of it's children.
        /// </summary>
        /// <param name="scalingFactor">The scaling factor to be applied to all <see cref="IScalable"/>s of this <see cref="ScalableObject"/>.</param>
        public void ApplyScaling(double scalingFactor)
        {
            PreScalingAction?.Invoke();
            foreach (IScalable binding in Bindings.Values)
            {
                binding.ApplyScaling(scalingFactor);
            }
            PostScalingAction?.Invoke();
        }

        private sealed class ScalableDouble : Scalable<double>
        {
            public ScalableDouble()
            {
            }

            public ScalableDouble(AvaloniaObject avaloniaObject, AvaloniaProperty<double> avaloniaProperty, double defaultValue) : base(avaloniaObject, avaloniaProperty, defaultValue)
            {
            }

            public override void ApplyScaling(double scalingFactor)
            {
                SetValue(Math.Abs(DefaultValue * scalingFactor));
            }
        }

        private sealed class ScalableThickness : Scalable<Thickness>
        {
            public ScalableThickness()
            {
            }

            public ScalableThickness(AvaloniaObject avaloniaObject, AvaloniaProperty<Thickness> avaloniaProperty, Thickness defaultValue) : base(avaloniaObject, avaloniaProperty, defaultValue)
            {
            }

            public override void ApplyScaling(double scalingFactor)
            {
                SetValue(new Thickness(DefaultValue.Left * scalingFactor, DefaultValue.Top * scalingFactor, DefaultValue.Right * scalingFactor, DefaultValue.Bottom * scalingFactor));
            }
        }

        private sealed class ScalableCornerRadius : Scalable<CornerRadius>
        {
            public ScalableCornerRadius()
            {
            }

            public ScalableCornerRadius(AvaloniaObject avaloniaObject, AvaloniaProperty<CornerRadius> avaloniaProperty, CornerRadius defaultValue) : base(avaloniaObject, avaloniaProperty, defaultValue)
            {
            }

            public override void ApplyScaling(double scalingFactor)
            {
                CornerRadius cornerRadius = new CornerRadius(DefaultValue.TopLeft * scalingFactor, DefaultValue.TopRight * scalingFactor, DefaultValue.BottomRight * scalingFactor, DefaultValue.BottomLeft * scalingFactor);
                SetValue(cornerRadius);
            }
        }
    }