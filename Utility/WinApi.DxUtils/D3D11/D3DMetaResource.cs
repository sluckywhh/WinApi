﻿using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using WinApi.Core;
using WinApi.DxUtils.Core;
using Device = SharpDX.DXGI.Device;
using Device1 = SharpDX.Direct3D11.Device1;

namespace WinApi.DxUtils.D3D11
{
    public class D3DMetaResource : D3D11Container, ID3D11MetaResourceImpl
    {
        private readonly D3DMetaResourceOptions m_creationOpts;
        private Adapter m_adapter;
        private DeviceContext1 m_context;
        private Device1 m_device;
        private SharpDX.DXGI.Device2 m_dxgiDevice;
        private Factory2 m_dxgiFactory;
        private bool m_isDisposed;
        private RenderTargetView m_renderTargetView;
        private SwapChain1 m_swapChain;

        public D3DMetaResource(D3DMetaResourceOptions creationOpts)
        {
            m_creationOpts = creationOpts;
        }

        public IntPtr Hwnd { get; private set; }
        public Size Size { get; private set; }

        public Device1 Device1
        {
            get { return m_device; }
            private set { m_device = value; }
        }

        public DeviceContext1 Context1
        {
            get { return m_context; }
            private set { m_context = value; }
        }

        public SharpDX.DXGI.Device2 DxgiDevice2
        {
            get { return m_dxgiDevice; }
            private set { m_dxgiDevice = value; }
        }

        public Factory2 DxgiFactory2
        {
            get { return m_dxgiFactory; }
            private set { m_dxgiFactory = value; }
        }

        public SwapChain1 SwapChain1
        {
            get { return m_swapChain; }
            private set { m_swapChain = value; }
        }

        public override RenderTargetView RenderTargetView
        {
            get { return m_renderTargetView; }
            protected set { m_renderTargetView = value; }
        }

        public override Adapter Adapter
        {
            get { return m_adapter; }
            protected set { m_adapter = value; }
        }

        public void Dispose()
        {
            m_isDisposed = true;
            GC.SuppressFinalize(this);
            Destroy();
            LinkedResources.Clear();
        }

        public override Device DxgiDevice => DxgiDevice2;
        public override Factory DxgiFactory => DxgiFactory2;
        public override SwapChain SwapChain => SwapChain1;
        public override DeviceContext Context => Context1;
        public override SharpDX.Direct3D11.Device Device => Device1;

        public void Initalize(IntPtr hwnd, Size size)
        {
            CheckDisposed();
            if (Device1 != null)
                Destroy();
            Hwnd = hwnd;
            Size = GetValidatedSize(ref size);
            ConnectRenderTargetView();
            InvokeInitializedEvent();
        }

        public void EnsureInitialized()
        {
            CheckDisposed();
            if (Device1 == null)
                Initalize(Hwnd, Size);
        }

        public void Resize(Size size)
        {
            CheckDisposed();
            Size = GetValidatedSize(ref size);
            try
            {
                DisconnectLinkedResources();
                DisconnectRenderTargetView();
                DisposableHelpers.DisposeAndSetNull(ref m_renderTargetView);
                // Resize retaining other properties.
                SwapChain1?.ResizeBuffers(0, Size.Width, Size.Height, Format.Unknown, SwapChainFlags.None);
                ConnectRenderTargetView();
                ConnectLinkedResources();
            }
            catch (SharpDXException ex)
            {
                if (ErrorHelpers.ShouldResetDxgiForError(ex.Descriptor))
                    Destroy();
                else throw;
            }
        }

        ~D3DMetaResource()
        {
            Dispose();
        }

        private void CheckDisposed()
        {
            if (m_isDisposed) throw new ObjectDisposedException(nameof(D3DMetaResource));
        }

        public void Destroy()
        {
            DisconnectLinkedResources();
            DisposableHelpers.DisposeAndSetNull(ref m_renderTargetView);
            DisposableHelpers.DisposeAndSetNull(ref m_swapChain);
            DisposableHelpers.DisposeAndSetNull(ref m_context);
            DisposableHelpers.DisposeAndSetNull(ref m_dxgiFactory);
            DisposableHelpers.DisposeAndSetNull(ref m_adapter);
            DisposableHelpers.DisposeAndSetNull(ref m_dxgiDevice);
            DisposableHelpers.DisposeAndSetNull(ref m_device);
            InvokeDestroyedEvent();
        }

        protected override void CreateDevice()
        {
            Device1 = D3DMetaFactory.CreateD3DDevice1(m_creationOpts);
        }

        protected override void CreateDxgiDevice()
        {
            EnsureDevice();
            DxgiDevice2 = Device1.QueryInterface<SharpDX.DXGI.Device2>();
        }

        protected override void CreateAdapter()
        {
            EnsureDxgiDevice();
            Adapter = DxgiDevice2.GetParent<Adapter>();
        }

        protected override void CreateDxgiFactory()
        {
            EnsureAdapter();
            DxgiFactory2 = Adapter.GetParent<Factory2>();
        }

        protected override void CreateSwapChain()
        {
            EnsureDevice();
            EnsureDxgiFactory();
            SwapChain1 = D3DMetaFactory.CreateSwapChain1(m_creationOpts, this);
        }

        protected override void CreateContext()
        {
            EnsureDevice();
            Context1 = Device1.ImmediateContext1;
        }
    }
}