﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Runtime.InteropServices;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transforms;

namespace osu.Framework.Graphics.UserInterface
{
    public partial class CircularProgress : Sprite, IHasCurrentValue<double>
    {
        private readonly BindableWithCurrent<double> current = new BindableWithCurrent<double>();

        public Bindable<double> Current
        {
            get => current.Current;
            set => current.Current = value;
        }

        [BackgroundDependencyLoader]
        private void load(ShaderManager shaders, IRenderer renderer)
        {
            Texture ??= renderer.WhitePixel;
            TextureShader = shaders.Load(VertexShaderDescriptor.TEXTURE_2, "CircularProgress");
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Current.BindValueChanged(c =>
            {
                if (!double.IsFinite(c.NewValue))
                    throw new ArgumentException($"{nameof(Current)} must be finite, but is {c.NewValue}.");

                Invalidate(Invalidation.DrawNode);
            }, true);
        }

        protected override DrawNode CreateDrawNode() => new CircularProgressDrawNode(this);

        public TransformSequence<CircularProgress> FillTo(double newValue, double duration = 0, Easing easing = Easing.None)
            => FillTo(newValue, duration, new DefaultEasingFunction(easing));

        public TransformSequence<CircularProgress> FillTo<TEasing>(double newValue, double duration, in TEasing easing)
            where TEasing : IEasingFunction
            => this.TransformBindableTo(Current, newValue, duration, easing);

        private float innerRadius = 1;

        /// <summary>
        /// The inner fill radius, relative to the <see cref="Drawable.DrawSize"/> of the <see cref="CircularProgress"/>.
        /// The value range is 0 to 1 where 0 is invisible and 1 is completely filled.
        /// The entire texture still fills the disk without cropping it.
        /// </summary>
        public float InnerRadius
        {
            get => innerRadius;
            set
            {
                if (!float.IsFinite(value))
                    throw new ArgumentException($"{nameof(InnerRadius)} must be finite, but is {value}.");

                innerRadius = Math.Clamp(value, 0, 1);
                Invalidate(Invalidation.DrawNode);
            }
        }

        private bool roundedCaps;

        public bool RoundedCaps
        {
            get => roundedCaps;
            set
            {
                if (roundedCaps == value)
                    return;

                roundedCaps = value;
                Invalidate(Invalidation.DrawNode);
            }
        }

        protected class CircularProgressDrawNode : SpriteDrawNode
        {
            public new CircularProgress Source => (CircularProgress)base.Source;

            public CircularProgressDrawNode(CircularProgress source)
                : base(source)
            {
            }

            protected float InnerRadius { get; private set; }
            protected float Progress { get; private set; }
            protected float TexelSize { get; private set; }
            protected bool RoundedCaps { get; private set; }

            public override void ApplyState()
            {
                base.ApplyState();

                InnerRadius = Source.innerRadius;
                Progress = Math.Abs((float)Source.current.Value);
                RoundedCaps = Source.roundedCaps;

                // smoothstep looks too sharp with 1px, let's give it a bit more
                TexelSize = 1.5f / ScreenSpaceDrawQuad.Size.X;
            }

            private IUniformBuffer<CircularProgressParameters> parametersBuffer;

            protected override void Blit(IRenderer renderer)
            {
                if (InnerRadius == 0 || (!RoundedCaps && Progress == 0))
                    return;

                base.Blit(renderer);
            }

            protected override void BindUniformResources(IShader shader, IRenderer renderer)
            {
                base.BindUniformResources(shader, renderer);

                parametersBuffer ??= renderer.CreateUniformBuffer<CircularProgressParameters>();
                parametersBuffer.Data = new CircularProgressParameters
                {
                    InnerRadius = InnerRadius,
                    Progress = Progress,
                    TexelSize = TexelSize,
                    RoundedCaps = RoundedCaps,
                };

                shader.BindUniformBlock("m_CircularProgressParameters", parametersBuffer);
            }

            protected internal override bool CanDrawOpaqueInterior => false;

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                parametersBuffer?.Dispose();
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private record struct CircularProgressParameters
            {
                public UniformFloat InnerRadius;
                public UniformFloat Progress;
                public UniformFloat TexelSize;
                public UniformBool RoundedCaps;
            }
        }
    }

    public static class CircularProgressTransformSequenceExtensions
    {
        public static TransformSequence<CircularProgress> FillTo(this TransformSequence<CircularProgress> t, double newValue, double duration = 0, Easing easing = Easing.None)
            => t.FillTo(newValue, duration, new DefaultEasingFunction(easing));

        public static TransformSequence<CircularProgress> FillTo<TEasing>(this TransformSequence<CircularProgress> t, double newValue, double duration, TEasing easing)
            where TEasing : IEasingFunction
            => t.Append(cp => cp.FillTo(newValue, duration, easing));
    }
}
