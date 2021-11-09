using System.IO;
using System.IO.Compression;

namespace Quadrecep.UI
{
    public class MapDropImporter : FileDropHandler
    {
        public override void OnFileDrop(string[] files, int screen)
        {
            foreach (var file in files) ImportMap(file);
            RefreshSongSelectSlider();
        }

        public static void ImportMap(string file)
        {
            ZipFile.ExtractToDirectory(file,
                Global.RelativeToMap(Path.GetFileNameWithoutExtension(file), absolutePath: true));
        }

        private void RefreshSongSelectSlider()
        {
            GetNode<SongSelectSlider>("../SongSelectSlider").RefreshElements();
        }
    }
}