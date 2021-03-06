using System;
using System.Collections.Generic;
using System.Diagnostics;
using Veldrid.MetalBindings;

namespace Veldrid.MTL
{
    internal class MTLSwapchainFramebuffer : MTLFramebufferBase
    {
        private readonly MTLGraphicsDevice _gd;
        private readonly CAMetalLayer _layer;
        private readonly MTLPlaceholderTexture _placeholderTexture;
        private CAMetalDrawable _drawable;
        private MTLTexture _depthTexture;

        public override uint Width => _placeholderTexture.Width;
        public override uint Height => _placeholderTexture.Height;

        public override OutputDescription OutputDescription { get; }

        public CAMetalDrawable CurrentDrawable => _drawable;

        private readonly FramebufferAttachment[] _colorTargets;
        private readonly FramebufferAttachment? _depthTarget;
        private readonly PixelFormat? _depthFormat;

        public override IReadOnlyList<FramebufferAttachment> ColorTargets => _colorTargets;
        public override FramebufferAttachment? DepthTarget => _depthTarget;

        public MTLSwapchainFramebuffer(
            MTLGraphicsDevice gd,
            CAMetalLayer layer,
            uint width,
            uint height,
            PixelFormat? depthFormat,
            PixelFormat colorFormat)
            : base()
        {
            _gd = gd;
            _layer = layer;

            OutputAttachmentDescription? depthAttachment = null;
            if (depthFormat != null)
            {
                _depthFormat = depthFormat;
                depthAttachment = new OutputAttachmentDescription(depthFormat.Value);
                RecreateDepthTexture(width, height);
                _depthTarget = new FramebufferAttachment(_depthTexture, 0);
            }
            OutputAttachmentDescription colorAttachment = new OutputAttachmentDescription(colorFormat);

            OutputDescription = new OutputDescription(depthAttachment, colorAttachment);
            _placeholderTexture = new MTLPlaceholderTexture();
            _placeholderTexture.Resize(width, height);
            _colorTargets = new[] { new FramebufferAttachment(_placeholderTexture, 0) };
        }

        private void RecreateDepthTexture(uint width, uint height)
        {
            Debug.Assert(_depthFormat.HasValue);
            if (_depthTexture != null)
            {
                _depthTexture.Dispose();
            }

            _depthTexture = Util.AssertSubtype<Texture, MTLTexture>(
                _gd.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
                    width, height, 1, 1, _depthFormat.Value, TextureUsage.DepthStencil)));
        }

        public void Resize(uint width, uint height)
        {
            _placeholderTexture.Resize(width, height);

            if (_depthFormat.HasValue)
            {
                RecreateDepthTexture(width, height);
            }
        }

        public void GetNextDrawable()
        {
            if (_drawable.NativePtr != IntPtr.Zero)
            {
                ObjectiveCRuntime.objc_msgSend(_drawable.NativePtr, "release");
            }

            _drawable = _layer.nextDrawable();
            if (_drawable.NativePtr == IntPtr.Zero)
            {
                Console.WriteLine("Got a null drawable.");
            }
        }

        public override bool IsRenderable => !_drawable.IsNull;

        public override MTLRenderPassDescriptor CreateRenderPassDescriptor()
        {
            var ret = MTLUtil.AllocInit<MTLRenderPassDescriptor>();
            var color0 = ret.colorAttachments[0];
            color0.texture = _drawable.texture;
            color0.loadAction = MTLLoadAction.Load;

            if (_depthTarget != null)
            {
                var depthAttachment = ret.depthAttachment;
                depthAttachment.texture = _depthTexture.DeviceTexture;
                depthAttachment.loadAction = MTLLoadAction.Load;
            }

            return ret;
        }

        public override void Dispose()
        {
            _depthTexture?.Dispose();
        }
    }
}