using Godot;
using Quadrecep.Map;

namespace Quadrecep
{
    public class CreateMap : Node
    {
        private Map.Map _map;
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        public void _OnCreateMapPressed()
        {
            _map = new Map.Map();
            _map.CreateMap("Test");
            _map.MapSet.AudioPath = "テレキャスターヒーホーイlong ver  すりぃ feat鏡音レン.mp3";
            _map.MapSet.BackgroundPath = "Telecaster_B-Boy_highres.jpg";
            _map.GetMap(0).AddNote(new NoteObject(1000, 0, 0b1101));
            _map.GetMap(0).AddNote(new NoteObject(2000, 0, 0b0111));
            _map.GetMap(0).AddNote(new NoteObject(3000, 0, 0b1010));
            _map.GetMap(0).AddNote(new NoteObject(4000, 0, 0b1100));
            _map.GetMap(0).AddNote(new NoteObject(20000, 0, 0b1010));
            _map.GetMap(0).AddSV(new ScrollVelocity(500, 0.5f));
            _map.GetMap(0).AddSV(new ScrollVelocity(1500, 1.5f));
            _map.GetMap(0).AddSV(new ScrollVelocity(3000, 1f));
            _map.GetMap(0).AddSV(new ScrollVelocity(3600, 2.0f));
            _map.GetMap(0).AddSV(new ScrollVelocity(5000, 10.0f));
            _map.SaveMap();
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
        }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
    }
}