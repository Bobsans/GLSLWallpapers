using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;
using SciterSharp;
using MessageBox = System.Windows.Forms.MessageBox;

namespace GLSLWallpapers.Helpers {
    public static class ShaderRegistry {
        static readonly Dictionary<string, ShaderInfo> shaders = new Dictionary<string, ShaderInfo>();

        public static SciterValue SciterValue {
            get {
                return new SciterValue(shaders.Select(pair => new SciterValue {
                    ["Name"] = new SciterValue(pair.Value.Name),
                    ["Author"] = new SciterValue(pair.Value.Author),
                    ["FileName"] = new SciterValue(pair.Value.FileName),
                    ["Image"] = new SciterValue(Converters.ImageToBase64Url(pair.Value.Image))
                }));
            }
        }

        public static void Load() {
            if (!Directory.Exists(Reference.SHADERS_DIRECTORY)) {
                Directory.CreateDirectory(Reference.SHADERS_DIRECTORY);
            }

            shaders.Clear();
            foreach (string path in Directory.GetFiles(Reference.SHADERS_DIRECTORY, $"*.{Reference.SHADER_FILE_EXTENSION}")) {
                ShaderInfo info = ShaderInfo.FromFile(path);
                shaders.Add(info.FileName, info);
            }

            if (Config.ShaderName == null) {
                Config.ShaderName = First().FileName;
            }
        }

        public static bool Has(string name) {
            return shaders.ContainsKey(name);
        }

        public static ShaderInfo Get(string name) {
            return shaders.ContainsKey(name) ? shaders[name] : null;
        }

        public static ShaderInfo First() {
            return shaders.Count > 0 ? shaders.First().Value : null;
        }

        public static IEnumerable<ShaderInfo> All() {
            return shaders.Values.AsEnumerable();
        }

        public static void Install(string path) {
            if (path != null && File.Exists(path)) {
                try {
                    ShaderInfo info = ShaderInfo.FromFile(path);

                    if (Has(info.FileName)) {
                        DialogResult result = MessageBox.Show($@"Wallpaper ""{info.FileName}"" already exist. Rewrite?", @"Wallpaper exist", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.No) {
                            return;
                        }
                    }

                    string destPath = Path.Combine(Reference.SHADERS_DIRECTORY, Path.GetFileName(path));
                    File.Copy(path, destPath, true);

                    info.FilePath = destPath;

                    shaders[info.FileName] = info;
                    Config.ShaderName = info.FileName;

                    return;
                } catch (Exception ex) {
                    Logger.Error($"Error wallpaper installation: {ex}");
                }
            }

            MessageBox.Show($@"Invalid wallpaper file: ""{path}""");
        }
    }

    public class ShaderInfo {
        public string FilePath { get; set; }
        public string FileName => Path.GetFileNameWithoutExtension(FilePath);
        public string Name { get; private set; }
        public string Author { get; private set; }
        public string VertexCode { get; private set; }
        public string FragmentCode { get; private set; }
        public Image Image { get; private set; }

        public static ShaderInfo FromFile(string path) {
            ShaderInfo info = new ShaderInfo {
                FilePath = Path.GetFullPath(path)
            };

            using (FileStream stream = new FileStream(path, FileMode.Open)) {
                ZipArchive file = new ZipArchive(stream, ZipArchiveMode.Read);

                foreach (ZipArchiveEntry entry in file.Entries) {
                    using Stream entryStream = entry.Open();
                    if (entry.Name == "info.ini") {
                        using StreamReader reader = new StreamReader(entryStream);
                        SectionData packInfo = new StreamIniDataParser().ReadData(reader).Sections.GetSectionData("Info");
                        if (packInfo != null) {
                            info.Name = packInfo.Keys.ContainsKey("Name") ? packInfo.Keys.GetKeyData("Name").Value : "Unnamed";
                            info.Author = packInfo.Keys.ContainsKey("Author") ? packInfo.Keys.GetKeyData("Author").Value : "Unknown";
                        }
                    } else if (entry.Name == "shader.vert") {
                        using StreamReader reader = new StreamReader(entryStream);
                        info.VertexCode = reader.ReadToEnd();
                    } else if (entry.Name == "shader.frag") {
                        using StreamReader reader = new StreamReader(entryStream);
                        info.FragmentCode = reader.ReadToEnd();
                    } else if (entry.Name == "thumbnail.png") {
                        info.Image = Image.FromStream(entryStream);
                    }
                }
            }

            return info;
        }
    }
}
