using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ass01_21127367
{
    public static class ShellIcon
    {
        #region Interop constants

        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;

        #endregion

        #region Interop data types

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [Flags]
        private enum SHGFI : int
        {
            /// <summary>get icon</summary>
            Icon = 0x000000100,
            /// <summary>get display name</summary>
            DisplayName = 0x000000200,
            /// <summary>get type name</summary>
            TypeName = 0x000000400,
            /// <summary>get attributes</summary>
            Attributes = 0x000000800,
            /// <summary>get icon location</summary>
            IconLocation = 0x000001000,
            /// <summary>return exe type</summary>
            ExeType = 0x000002000,
            /// <summary>get system icon index</summary>
            SysIconIndex = 0x000004000,
            /// <summary>put a link overlay on icon</summary>
            LinkOverlay = 0x000008000,
            /// <summary>show icon in selected state</summary>
            Selected = 0x000010000,
            /// <summary>get only specified attributes</summary>
            Attr_Specified = 0x000020000,
            /// <summary>get large icon</summary>
            LargeIcon = 0x000000000,
            /// <summary>get small icon</summary>
            SmallIcon = 0x000000001,
            /// <summary>get open icon</summary>
            OpenIcon = 0x000000002,
            /// <summary>get shell size icon</summary>
            ShellIconSize = 0x000000004,
            /// <summary>pszPath is a pidl</summary>
            PIDL = 0x000000008,
            /// <summary>use passed dwFileAttribute</summary>
            UseFileAttributes = 0x000000010,
            /// <summary>apply the appropriate overlays</summary>
            AddOverlays = 0x000000020,
            /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
            OverlayIndex = 0x000000040,
        }

        #endregion

        private class Win32
        {
            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("User32.dll")]
            public static extern int DestroyIcon(IntPtr hIcon);

            /// <summary>
            /// Creates an array of handles to large or small icons extracted 
            /// from the specified executable file, DLL, or icon file.
            [DllImport("shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int ExtractIconEx(string lpszFile, int nIconIndex, out IntPtr phiconLarge, out IntPtr phiconSmall, int nIcons);
        }

        public static ImageSource GetSmallFolderIcon()
        {
            return GetIcon(FILE_ATTRIBUTE_DIRECTORY, SHGFI.SmallIcon);
        }

        public static ImageSource GetLargeFolderIcon()
        {
            return GetIcon(FILE_ATTRIBUTE_DIRECTORY, SHGFI.LargeIcon);
        }

        public static ImageSource GetSmallIcon(string fileName)
        {
            return GetIcon(fileName, SHGFI.SmallIcon);
        }

        public static ImageSource GetLargeIcon(string fileName)
        {
            return GetIcon(fileName, SHGFI.LargeIcon);
        }

        private static ImageSource GetIcon(string fileName, SHGFI flags)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr hImgSmall = Win32.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), (uint)(SHGFI.Icon | flags));

            if (hImgSmall == IntPtr.Zero)
                return null;

            try
            {
                Icon icon = (Icon)Icon.FromHandle(shinfo.hIcon).Clone();
                return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                Win32.DestroyIcon(shinfo.hIcon);
            }
        }

        private static ImageSource GetIcon(uint dwFileAttributes, SHGFI flags)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr hImgSmall = Win32.SHGetFileInfo(null, dwFileAttributes, ref shinfo, (uint)Marshal.SizeOf(shinfo), (uint)(SHGFI.Icon | flags));

            if (hImgSmall == IntPtr.Zero)
                return null;

            try
            {
                Icon icon = (Icon)Icon.FromHandle(shinfo.hIcon).Clone();
                return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                Win32.DestroyIcon(shinfo.hIcon);
            }
        }
    }
}
