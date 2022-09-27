﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ConstantsDLL;

// Code borrowed from the internets to change screen resolution
namespace OfflineDriverInstallerOOBE
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DEVMODE1
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;

        public short dmOrientation;
        public short dmPaperSize;
        public short dmPaperLength;
        public short dmPaperWidth;

        public short dmScale;
        public short dmCopies;
        public short dmDefaultSource;
        public short dmPrintQuality;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        public short dmLogPixels;
        public short dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;

        public int dmDisplayFlags;
        public int dmDisplayFrequency;

        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;

        public int dmPanningWidth;
        public int dmPanningHeight;
    };

    public static class PrmaryScreenResolution
    {
        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE1 devMode);
        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettingsA(string deviceName, int modeNum, ref DEVMODE1 devMode);
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE1 devMode, int flags);

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int CDS_UPDATEREGISTRY = 0x01;
        public const int CDS_TEST = 0x02;
        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const int DISP_CHANGE_RESTART = 1;
        public const int DISP_CHANGE_FAILED = -1;
        public static List<string> resListW;
        public static List<string> resListH;


        public static string ChangeResolution(int width, int height, bool verbose)
        {
            getResolutions();
            string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
            string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();
            int sW = Convert.ToInt32(screenWidth);
            int sH = Convert.ToInt32(screenHeight);

            if (verbose)
            {
                Console.WriteLine(StringsAndConstants.CHECKING_AVAILABLE_RESOLUTIONS);
                Console.WriteLine();
                Console.WriteLine(StringsAndConstants.CHECKING_RESOLUTION);
                Console.WriteLine();
                Console.WriteLine(sW + "x" + sH);
                Console.WriteLine();
            }

            DEVMODE1 dm = GetDevMode1();

            if (resListW.Contains(width.ToString()) && resListH.Contains(height.ToString()) && sW < width)
            {
                if (verbose)
                {
                    Console.WriteLine(StringsAndConstants.CHANGING_RESOLUTION);
                    Console.WriteLine();
                }
                if (0 != EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm))
                {
                    dm.dmPelsWidth = width;
                    dm.dmPelsHeight = height;

                    int iRet = ChangeDisplaySettings(ref dm, CDS_TEST);

                    if (iRet == DISP_CHANGE_FAILED)
                    {
                        return StringsAndConstants.resChangeFailed;
                    }
                    else
                    {
                        iRet = ChangeDisplaySettings(ref dm, CDS_UPDATEREGISTRY);
                        switch (iRet)
                        {
                            case DISP_CHANGE_SUCCESSFUL:
                                {
                                    if (verbose)
                                    {
                                        Console.WriteLine(StringsAndConstants.CHANGING_RESOLUTION_SUCCESSFUL);
                                        Console.WriteLine();
                                    }
                                    return StringsAndConstants.resChangeSuccess;
                                }
                            case DISP_CHANGE_RESTART:
                                {
                                    return StringsAndConstants.resChangeReboot;
                                }
                            default:
                                {
                                    return StringsAndConstants.resChangeFailed;
                                }
                        }
                    }
                }
                else
                {
                    return StringsAndConstants.resChangeFailed;
                }
            }
            if (verbose)
            {
                Console.WriteLine(StringsAndConstants.CHANGING_RESOLUTION_UNNECESSARY);
                Console.WriteLine();
            }
            return StringsAndConstants.resChangeSuccess;
        }

        private static void getResolutions()
        {
            DEVMODE1 vDevMode = new DEVMODE1();
            resListW = new List<string>();
            resListH = new List<string>();
            int i = 0;
            while (EnumDisplaySettingsA(null, i, ref vDevMode))
            {
                resListW.Add(vDevMode.dmPelsWidth.ToString());
                resListH.Add(vDevMode.dmPelsHeight.ToString());
                i++;
            }
        }

        private static DEVMODE1 GetDevMode1()
        {
            DEVMODE1 dm = new DEVMODE1();
            dm.dmDeviceName = new String(new char[32]);
            dm.dmFormName = new String(new char[32]);
            dm.dmSize = (short)Marshal.SizeOf(dm);
            return dm;
        }
    }
}