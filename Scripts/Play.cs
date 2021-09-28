using Godot;

public class Play : Node2D
{
    public Map Map;
    public string MapFile;
    private AudioStream _stream;

    private ImageTexture _backgroundTexture;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Map = new Map("Test");
        Map.ReadMap();
        ((Label) GetNode(new NodePath("HUD/Name"))).Text = Map.MapSet.Name;
        LoadBackground();
        LoadAudio();
    }

    private void LoadAudio()
    {
        var audioFile = new File();
        audioFile.Open(MapPath(Map.MapSet.Audio), File.ModeFlags.Read);
        var buffer = audioFile.GetBuffer((int) audioFile.GetLen());
        if (Map.MapSet.Audio.EndsWith(".mp3"))
        {
            var mp3Stream = new AudioStreamMP3();
            mp3Stream.Data = buffer;
            _stream = mp3Stream;
        }
        else if (Map.MapSet.Audio.EndsWith(".wav"))
        {
            var wavStream = new AudioStreamSample();
            wavStream.Data = buffer;
            _stream = wavStream;
        }
        else if (Map.MapSet.Audio.EndsWith(".ogg"))
        {
            var oggStream = new AudioStreamOGGVorbis();
            oggStream.Data = buffer;
            _stream = oggStream;
        }
        var audioPlayer = (AudioStreamPlayer) GetNode(new NodePath("ParallaxBackground/AudioStreamPlayer"));
        audioPlayer.Stream = _stream;
        audioPlayer.Playing = true;
    }
    
    

    private void LoadBackground()
    {
        var img = new Image();
        var imgPath = MapPath(Map.MapSet.Background);
        GD.Print($"Loading background from {imgPath}");
        img.Load(imgPath);
        _backgroundTexture = new ImageTexture();
        _backgroundTexture.CreateFromImage(img);
        var bg = (TextureRect) GetNode(new NodePath("ParallaxBackground/Background"));
        bg.Texture = _backgroundTexture;
        bg.Visible = true;
    }

    private string MapPath(string mapRelativePath)
    {
        return $"user://{Map.MapFile}/{mapRelativePath}";
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
