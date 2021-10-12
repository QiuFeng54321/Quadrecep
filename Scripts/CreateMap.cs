using Godot;
using System;
using System.Linq;
using Quadrecep.Map;

public class CreateMap : Node
{
    private Map _map;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public void _OnCreateMapPressed()
    {
        _map = new Map();
        _map.CreateMap("Test");
        _map.MapSet.AudioPath = "テレキャスターヒーホーイlong ver  すりぃ feat鏡音レン.mp3";
        _map.MapSet.BackgroundPath = "Telecaster_B-Boy_highres.jpg";
        var path = new DirectionObject(new int[]{1, 1, 0, 1});
        _map.GetMap(0).AddNote(new NoteObject(1000, 0, path.RawDirection));
        _map.SaveMap();
        var verifyPath = new DirectionObject(path.RawDirection);
        GD.Print("Direction: " + string.Join(", ", verifyPath.Direction));
        GD.Print("VecDirection: " + verifyPath.NetDirection);
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
