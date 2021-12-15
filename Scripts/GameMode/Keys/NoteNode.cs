using System.Collections.Generic;
using System.Linq;
using Godot;
using Quadrecep.GameMode.Keys.Map;
using Path = Quadrecep.Gameplay.Path;

namespace Quadrecep.GameMode.Keys
{
    public class NoteNode : Sprite
    {
        private static List<Texture[]> _hitObjectTextures = new() {null};
        private static List<Texture[]> _holdBodyTextures = new() {null};
        private static List<Texture[]> _holdEndTextures = new() {null};
        private static List<Texture[]> _holdHitObjectTextures = new() {null};

        public static PackedScene Scene;
        private readonly Queue<Path> _visiblePaths = new();
        public List<Path> Paths = new();

        private bool _hasParent;
        public NoteObject Note;
        public Playfield Parent;
        private int Key => Note.Lane;

        public float XPositionBase => (Parent.ReceptorX[Key] + Parent.ReceptorX[Key + 1]) / 2;
        public bool Finished => _visiblePaths.Count == 0;

        public override void _Ready()
        {
            Texture = _hitObjectTextures[Parent.Parent.InputRetriever.Keys][Key];
            Offset = new Vector2(-Texture.GetWidth() / 2f, -Texture.GetHeight());
            // Visible = false;
        }

        public override void _Process(float delta)
        {
            if (_visiblePaths.Count != 0) Position = _visiblePaths.Peek()[Parent.Parent.Time];
        }

        public void GenerateVisiblePaths(Vector2 regionPos1, Vector2 regionPos2)
        {
            _visiblePaths.Clear();
            foreach (var visiblePath in Paths.Select(path => Path.CutVisiblePath(path, regionPos1, regionPos2, false))
                .Where(visiblePath => visiblePath != null)) _visiblePaths.Enqueue(visiblePath);
        }

        public void CheckVisible()
        {
            if (RemoveIfFinished()) return;

            while (Parent.Parent.Time > _visiblePaths.Peek().EndTime)
            {
                _visiblePaths.Dequeue();
                if (RemoveIfFinished()) return;
            }

            if (Parent.Parent.Time < _visiblePaths.Peek().StartTime)
            {
                RemoveFromParent();
                return;
            }

            AddToParent();
        }

        private bool RemoveIfFinished()
        {
            if (!Finished) return false;
            RemoveFromParent();
            return true;
        }

        private void AddToParent()
        {
            if (_hasParent) return;
            _hasParent = true;
            Parent.Notes.AddChild(this);
        }

        private void RemoveFromParent()
        {
            if (!_hasParent) return;
            Parent.Notes.RemoveChild(this);
            _hasParent = false;
        }

        public static void LoadTextures(int keys)
        {
            FillTexturesUpTo(keys, ref _hitObjectTextures);
            LoadTexturesForKey(keys, "hitobject", ref _hitObjectTextures);
            FillTexturesUpTo(keys, ref _holdBodyTextures);
            LoadTexturesForKey(keys, "holdbody", ref _holdBodyTextures);
            FillTexturesUpTo(keys, ref _holdEndTextures);
            LoadTexturesForKey(keys, "holdend", ref _holdEndTextures);
            FillTexturesUpTo(keys, ref _holdHitObjectTextures);
            LoadTexturesForKey(keys, "holdhitobject", ref _holdHitObjectTextures);
        }

        private static void LoadTexturesForKey(int keys, string name, ref List<Texture[]> to)
        {
            for (var i = 0; i < keys; i++)
                to[keys][i] =
                    Global.LoadImage(ImgPath(keys, name, i + 1), ImgPath(4, name, 1));
        }

        private static string ImgPath(int keys, string name, int i)
        {
            return $"{Global.TexturesPath}/GameModes/Keys/Keys{keys}/HitObjects/note-{name}-{i}.png";
        }

        private static void FillTexturesUpTo(int keys, ref List<Texture[]> to)
        {
            for (var i = to.Count; i <= keys; i++) to.Add(new Texture[i]);
        }

        public void ClearInput()
        {
            _visiblePaths.Clear();
            RemoveFromParent();
        }
    }
}