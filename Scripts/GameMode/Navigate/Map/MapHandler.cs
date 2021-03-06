using Godot;

namespace Quadrecep.GameMode.Navigate.Map
{
    public class MapHandler : Quadrecep.Map.MapHandler
    {
        private string _mapFile;
        private MapObject _mapObject;

        public MapHandler(string mapSetPath) : base(mapSetPath)
        {
        }

        public override PackedScene Scene => Play.Scene;
        public override string DifficultyName => _mapObject.DifficultyName;
        public override string GameModeShortName => GameModeInfo.ShortName;

        public override void ReadMap(string file)
        {
            _mapFile = file;
            _mapObject = Global.DeserializeFromFile<MapObject>(MapSetPath, file);
        }

        public override T GetMap<T>() where T : class
        {
            return _mapObject as T;
        }

        public override APlayBase InitScene()
        {
            var scene = Scene.Instance<Play>();
            scene.MapSetFile = MapSetPath;
            scene.MapObject = _mapObject;
            return scene;
        }
    }
}