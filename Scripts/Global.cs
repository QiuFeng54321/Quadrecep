using Godot;
using Quadrecep.Database;

namespace Quadrecep
{
    public class Global : Node
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            DatabaseHandler.Initialize();
            OS.VsyncEnabled = false;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
        }
    }
}