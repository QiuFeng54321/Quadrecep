using System;
using Godot;

namespace Quadrecep.GameMode.Keys
{
    public class Receptor : Node2D
    {
        private static readonly string FallbackTexture =
            $"{Global.TexturesPath}/GameModes/Keys/Keys4/Receptors/receptor-up-1.png";

        public static PackedScene Scene;
        private Texture _upTexture, _downTexture;
        public int KeyIndex;
        public Playfield Parent;

        public override void _Ready()
        {
            LoadTexture();
        }

        public void SetTextureState(bool release)
        {
            GetNode<Sprite>("Sprite").Texture = release ? _upTexture : _downTexture;
        }

        public Vector2 MaxSize()
        {
            return new Vector2(Math.Max(_upTexture.GetWidth(), _downTexture.GetWidth()),
                Math.Max(_upTexture.GetHeight(), _downTexture.GetHeight()));
        }

        public void LoadTexture()
        {
            _upTexture = Global.LoadImage(
                $"res://Textures/GameModes/Keys/Keys{Parent.Parent.InputRetriever.Keys}/Receptors/receptor-up-{KeyIndex + 1}.png",
                FallbackTexture);
            _downTexture = Global.LoadImage(
                $"res://Textures/GameModes/Keys/Keys{Parent.Parent.InputRetriever.Keys}/Receptors/receptor-down-{KeyIndex + 1}.png",
                FallbackTexture);
        }
    }
}