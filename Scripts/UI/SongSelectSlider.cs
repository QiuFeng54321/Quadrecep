using System.Threading;
using System.Threading.Tasks;
using Godot;
using Quadrecep.Map;

namespace Quadrecep.UI
{
    public class SongSelectSlider : Control
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private int _mapIndex;
        public float Rate = 1.0f;

        public int MapIndex
        {
            get => _mapIndex;
            set
            {
                if (_mapIndex == value) return;
                _mapIndex = value;
                RefreshFocusChild();
            }
        }

        public SongSelectElement FocusedElement => HBoxContainer.GetChildren()[MapIndex] as SongSelectElement;

        private HBoxContainer HBoxContainer => GetNode<HBoxContainer>("ScrollContainer/HBoxContainer");

        public int ChildrenCount => HBoxContainer.GetChildren().Count;

        private void RefreshFocusChild()
        {
            UpdateElementFocus();
            GetParent<SongSelect>().ChangeBackgroundTexture();
        }

        public override void _Ready()
        {
            Global.MapContainingDirectory.Open($"user://{AMapSet<object>.MapDirectory}");
            LoadElements(Global.MapContainingDirectory);
            RefreshFocusChild();
        }

        public void RefreshElements()
        {
            ClearChildren();
            LoadElements(Global.MapContainingDirectory);
            RefreshFocusChild();
        }

        private void ClearChildren()
        {
            foreach (Node child in HBoxContainer.GetChildren())
            {
                HBoxContainer.RemoveChild(child);
                child.QueueFree();
            }
        }

        private void LoadElements(Directory dir)
        {
            dir.ListDirBegin();
            var fileName = dir.GetNext();
            while (!string.IsNullOrEmpty(fileName))
            {
                if (dir.CurrentIsDir() && !fileName.StartsWith("."))
                {
                    var element = SongSelectElement.Scene.Instance<SongSelectElement>();
                    GD.Print(fileName);
                    element.MapFile = fileName;
                    element.Index = ChildrenCount;
                    element.CancellationTokenSource = _cancellationTokenSource;
                    element.Parent = this;
                    HBoxContainer.AddChild(element);
                    Task.Run(() => element.LoadMaps());
                }

                fileName = dir.GetNext();
            }
        }

        public override void _Process(float delta)
        {
            // if (!HasFocus()) return;
            if (Input.IsActionJustPressed("ui_left")) MapIndex = MapIndex - 1 < 0 ? ChildrenCount - 1 : MapIndex - 1;
            if (Input.IsActionJustPressed("ui_right")) MapIndex = (MapIndex + 1) % ChildrenCount;
        }

        private void UpdateElementFocus()
        {
            FocusedElement.GrabFocus();
        }
    }
}