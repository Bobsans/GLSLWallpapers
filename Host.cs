using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using GLSLWallpapers.Helpers;
using SciterSharp;
using SciterSharp.Interop;

namespace GLSLWallpapers {
    class Host : BaseHost {
        public Host(SciterWindow window) {
            Setup(window);
            AttachEvh(new HostEvh(this));
            SetupPage("index.html");

            #if DEBUG
            try {
                DebugInspect();
            } catch {
                // ignore
            }
            #endif

            window.Show();
        }
    }

    class HostEvh : SciterEventHandler {
        static Host Host { get; set; }

        public HostEvh(Host host) {
            Host = host;
        }

        protected override bool OnScriptCall(SciterElement se, string name, SciterValue[] args, out SciterValue result) {
            switch (name) {
                case "LoadConfigs":
                    result = Config.SciterValue;
                    return true;
                case "LoadShaders":
                    result = ShaderRegistry.SciterValue;
                    return true;
                case "ConfigSetInteger":
                    result = SetConfigValue(args[0].Get(""), args[1].Get(0));
                    return true;
                case "ConfigSetDouble":
                    result = SetConfigValue(args[0].Get(""), args[1].Get(0D));
                    return true;
                case "ConfigSetBoolean":
                    result = SetConfigValue(args[0].Get(""), args[1].Get(false));
                    return true;
                case "ConfigSetString":
                    result = SetConfigValue(args[0].Get(""), args[1].Get(""));
                    return true;
            }

            result = null;
            return false;
        }

        static SciterValue SetConfigValue<T>(string name, T value) {
            try {
                Config.SetFieldValue(name, value);
                return new SciterValue(true);
            } catch {
                return new SciterValue(false);
            }
        }
    }

    class BaseHost : SciterHost {
        SciterWindow window;
        readonly SciterArchive archive = new SciterArchive();

        public BaseHost() {
            #if !DEBUG
            archive.Open(ArchiveResource.resources);
            #endif
        }

        protected void Setup(SciterWindow wnd) {
            window = wnd;
            SetupWindow(wnd);
        }

        protected void SetupPage(string pageFromResFolder) {
            #if DEBUG
            string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../data/app/", pageFromResFolder));
            Debug.Assert(File.Exists(path));
            string url = "file://" + path.Replace('\\', '/');
            #else
            string url = "archive://app/" + pageFromResFolder;
            #endif
            bool result = window.LoadPage(url);
            Debug.Assert(result);
        }

        protected override SciterXDef.LoadResult OnLoadData(SciterXDef.SCN_LOAD_DATA sld) {
            if (sld.uri.StartsWith("archive://app/")) {
                string path = sld.uri.Substring(14);
                byte[] data = archive.Get(path);
                if (data != null) {
                    SciterX.API.SciterDataReady(window._hwnd, sld.uri, data, (uint)data.Length);
                }
            }

            return base.OnLoadData(sld);
        }
    }
}
