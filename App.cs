using System;
using GLSLWallpapers.Helpers;
using SciterSharp;
using SciterSharp.Interop;

namespace GLSLWallpapers {
    class SciterMessages : SciterDebugOutputHandler {
        protected override void OnOutput(SciterXDef.OUTPUT_SUBSYTEM subsystem, SciterXDef.OUTPUT_SEVERITY severity, string text) {
            Console.WriteLine(text);
        }
    }

    static class App {
        public static bool IsRunning { get; private set; }
        static SciterMessages sm = new SciterMessages();
        static SciterWindow AppWindow { get; set; }
        static Host AppHost { get; set; }

        public static void Run() {
            IsRunning = true;
            AppWindow = new SciterWindow();

            AppWindow.CreateMainWindow(1280, 720);
            AppWindow.CenterTopLevelWindow();
            AppWindow.Title = Reference.NAME;
            Win32.EnableWindowBlur(AppWindow._hwnd);

            AppHost = new Host(AppWindow);

            PInvokeUtils.RunMsgLoop();
            IsRunning = false;
        }
    }
}
