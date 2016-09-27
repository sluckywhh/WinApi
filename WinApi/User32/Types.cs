﻿using System;
using System.Runtime.InteropServices;
using WinApi.Core;

namespace WinApi.User32
{
    public delegate IntPtr WindowProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public delegate IntPtr GetMsgProc(int code, IntPtr wParam, IntPtr lParam);
    public delegate void TimerProc(IntPtr hWnd, uint uMsg, IntPtr nIdEvent, uint dwTickCountMillis);

    [StructLayout(LayoutKind.Sequential)]
    public struct Message
    {
        public IntPtr WindowHandle;
        public uint Value;
        public IntPtr WParam;
        public IntPtr LParam;
        public uint Time;
        public Point Point;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public Rectangle(int left = 0, int top = 0, int right = 0, int bottom = 0)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Rectangle(int width = 0, int height = 0)
        {
            this.Left = 0;
            this.Top = 0;
            this.Right = width;
            this.Bottom = height;
        }

        public Rectangle(int all = 0)
        {
            Left = Right = Top = Bottom = all;
        }

        public int Left, Top, Right, Bottom;

        public int Width
        {
            get { return Right - Left; }
            set { Right = Left + value; }
        }

        public int Height
        {
            get { return Bottom - Top; }
            set { Bottom = Top + value; }
        }

        public Size GetSize()
        {
            return new Size(Width, Height);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct PaintStruct
    {
        public IntPtr HandleDC;
        public int EraseBackgroundValue;
        public Rectangle PaintRectangle;
        private int ReservedInternalRestore;
        private int ReservedInternalIncUpdate;
        private fixed byte ReservedInternalRgb [32];

        public bool ShouldEraseBackground
        {
            get { return EraseBackgroundValue > 0; }
            set { EraseBackgroundValue = value ? 1 : 0; }
        }
    }

    /// <summary>
    ///     Note: Marshalled
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = Properties.BuildCharSet)]
    public struct WindowClassEx
    {
        public uint Size;
        public WindowClassStyles Styles;
        [MarshalAs(UnmanagedType.FunctionPtr)] public WindowProc WindowProc;
        public int ClassExtraBytes;
        public int WindowExtraBytes;
        public IntPtr InstanceHandle;
        public IntPtr IconHandle;
        public IntPtr CursorHandle;
        public IntPtr BackgroundBrushHandle;
        public string MenuName;
        public string ClassName;
        public IntPtr SmallIconHandle;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct WindowClassExBlittable
    {
        public uint Size;
        public WindowClassStyles Styles;
        public IntPtr WindowProc;
        public int ClassExtraBytes;
        public int WindowExtraBytes;
        public IntPtr InstanceHandle;
        public IntPtr IconHandle;
        public IntPtr CursorHandle;
        public IntPtr BackgroundBrushHandle;
        public IntPtr MenuName;
        public IntPtr ClassName;
        public IntPtr SmallIconHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowInfo
    {
        public uint Size;
        public Rectangle WindowRectangle;
        public Rectangle ClientRectangle;
        public WindowStyles Styles;
        public WindowExStyles ExStyles;
        public uint WindowStatus;
        public uint BorderX;
        public uint BorderY;
        public ushort WindowType;
        public ushort CreatorVersion;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CreateStruct
    {
        public IntPtr CreateParams;
        public IntPtr InstanceHandle;
        public IntPtr MenuHandle;
        public IntPtr ParentWindowHandle;
        public int Height;
        public int Width;
        public int Y;
        public int X;
        public WindowStyles Styles;
        public IntPtr Name;
        public IntPtr ClassName;
        public WindowExStyles ExStyles;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowPlacement
    {
        public uint Size;
        public WindowPlacementFlags Flags;
        public ShowWindowCommands ShowCmd;
        public Point MinPosition;
        public Point MaxPosition;
        public Rectangle NormalPosition;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BlendFunction
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public AlphaFormat AlphaFormat;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AnimationInfo
    {
        /// <summary>
        ///     Creates an AMINMATIONINFO structure.
        /// </summary>
        /// <param name="iMinAnimate">If non-zero and SPI_SETANIMATION is specified, enables minimize/restore animation.</param>
        public AnimationInfo(int iMinAnimate)
        {
            Size = (uint) Marshal.SizeOf<AnimationInfo>();
            MinAnimate = iMinAnimate;
        }

        /// <summary>
        ///     Always must be set to (System.UInt32)Marshal.SizeOf(typeof(ANIMATIONINFO)).
        /// </summary>
        public uint Size;

        /// <summary>
        ///     If non-zero, minimize/restore animation is enabled, otherwise disabled.
        /// </summary>
        public int MinAnimate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MinimizedMetrics
    {
        public uint Size;
        public int Width;
        public int HorizontalGap;
        public int VerticalGap;
        public ArrangeFlags Arrange;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TrackMouseEventOptions
    {
        public uint Size;
        public TrackMouseEventFlags Flags;
        public IntPtr TrackedWindowHandle;
        public uint HoverTime;

        public const uint DefaultHoverTime = 0xFFFFFFFF;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MinMaxInfo
    {
        Point Reserved;
        /// <summary>
        /// The maximized width (x member) and the maximized height (y member) of the window. For top-level windows, this value is based on the width of the primary monitor.
        /// </summary>
        public Point MaxSize;
        /// <summary>
        /// The position of the left side of the maximized window (x member) and the position of the top of the maximized window (y member). For top-level windows, this value is based on the position of the primary monitor.
        /// </summary>
        public Point MaxPosition;
        /// <summary>
        /// The minimum tracking width (x member) and the minimum tracking height (y member) of the window. This value can be obtained programmatically from the system metrics SM_CXMINTRACK and SM_CYMINTRACK (see the GetSystemMetrics function).
        /// </summary>
        public Point MinTrackSize;
        /// <summary>
        /// The maximum tracking width (x member) and the maximum tracking height (y member) of the window. This value is based on the size of the virtual screen and can be obtained programmatically from the system metrics SM_CXMAXTRACK and SM_CYMAXTRACK (see the GetSystemMetrics function).
        /// </summary>
        public Point MaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowPosition
    {
        public IntPtr WindowHandle;
        public IntPtr HwndZOrderInsertAfter;
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public WindowPositionFlags Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NonClientArea
    {
        public NonClientSizeParamRegionUnion Region;
        public WindowPosition* Position;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct NonClientSizeParamRegionUnion
    {
        [FieldOffset(0)] public NonClientSizeInputParam Input;
        [FieldOffset(0)] public NonClientSizeInputParam Output;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NonClientSizeInputParam
    {
        public Rectangle TargetWindowRect;
        public Rectangle CurrentWindowRect;
        public Rectangle CurrentClientRect;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NonClientSizeOutputParam
    {
        public Rectangle ResultClientRect;
        public Rectangle SrcRect;
        public Rectangle DestRect;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NonClientAreaRectangle
    {
        [FieldOffset(0)] public Rectangle* InputWindowRect;
        [FieldOffset(0)] public Rectangle* OutputClientRect;
    }
}