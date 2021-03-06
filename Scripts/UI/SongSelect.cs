using Godot;

namespace Quadrecep.UI
{
    public class SongSelect : Control
    {
        public static readonly Color InitialColor = new(1, 1, 1, 0.75f);
        public static readonly Color FinalColor = new(1, 1, 1, 0);
        public SongSelectSlider Slider => GetNode<SongSelectSlider>("SongSelectSlider");
        public Label Rate => GetNode<Label>("Rate");
        public static PackedScene Scene;

        public override void _Ready()
        {
            Slider.Parent = this;
        }

        /// <summary>
        /// Transitions textures of two map sets smoothly
        /// </summary>
        public void ChangeBackgroundTexture()
        {
            var tween = GetNode<Tween>("BackgroundTransition");
            var blurredBackground = GetNode<TextureRect>("BlurredBackground");
            var blurredBackground2 = GetNode<TextureRect>("BlurredBackground2");
            blurredBackground2.Texture = Slider.FocusedElement.GetNode<TextureRect>("Preview").Texture;
            tween.InterpolateProperty(blurredBackground, "modulate", InitialColor, FinalColor,
                SongSelectElement.FocusDuration);
            tween.InterpolateProperty(blurredBackground2, "modulate", FinalColor, InitialColor,
                SongSelectElement.FocusDuration);
            tween.Start();
        }

        public void _OnTweenAllComplete()
        {
            var blurredBackground = GetNode<TextureRect>("BlurredBackground");
            var blurredBackground2 = GetNode<TextureRect>("BlurredBackground2");
            Global.SwapTexture(blurredBackground, blurredBackground2);
            blurredBackground.Modulate = InitialColor;
            blurredBackground2.Modulate = FinalColor;
        }
    }
}