using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Quadrecep.GameMode.Keys.Map;
using Quadrecep.Map;
using Path = Quadrecep.Gameplay.Path;

namespace Quadrecep.GameMode.Keys
{
    public class Playfield : CanvasLayer
    {
        public const float RealCoverHeight = 600;

        public readonly List<NoteNode> NoteNodes = new();

        private Vector2 _receptorSize = new(256, 277);
        private Vector2 _receptorsSize;
        public Play Parent;
        public float[] ReceptorX;
        private Vector2 VisibleRegionPos1 => new(-1579, PlayfieldNoteTopY - 100);
        private Vector2 VisibleRegionPos2 => new(2600.5f, 100);
        protected float PlayfieldNoteTopY => -RealCoverHeight * 4 + _receptorsSize.y;
        private Sprite BorderL => Cover.GetNode<Sprite>("BorderL");
        private Sprite BorderR => Cover.GetNode<Sprite>("BorderR");
        public Sprite Cover => GetNode<Sprite>("Main/Cover");

        private Sprite Receptors => GetNode<Sprite>("Main/Receptors");

        public Node2D Notes => GetNode<Node2D>("Main/Receptors/Notes");

        public override void _Process(float delta)
        {
            for (var i = 0; i < NoteNodes.Count; i++)
            {
                NoteNodes[i].CheckVisible();
                if (!NoteNodes[i].Finished) continue;
                NoteNodes.RemoveAt(i);
                i--;
            }
        }

        public void InitField()
        {
            ReceptorX = new float[Parent.InputRetriever.Keys + 1];
            // 0.167F: 2 / 3 / 4 = 1 / 6, 2 / 3 for a 4K playfield by default
            Cover.Scale = new Vector2(Parent.InputRetriever.Keys * Config.PlayfieldWidthPerKey,
                RealCoverHeight / Cover.Texture.GetHeight());
            BorderL.Scale = new Vector2(1, RealCoverHeight / BorderL.Texture.GetHeight() / Cover.Scale.y);
            BorderL.Position = new Vector2(Cover.Texture.GetWidth() / -2f - BorderL.Texture.GetWidth() / 2f, 0);
            BorderR.Scale = new Vector2(1, RealCoverHeight / BorderR.Texture.GetHeight() / Cover.Scale.y);
            BorderR.Position = new Vector2(Cover.Texture.GetWidth() / 2f + BorderR.Texture.GetWidth() / 2f, 0);
            PlaceReceptors();
            Receptors.Scale =
                new Vector2(Cover.Texture.GetWidth() * Cover.Scale.x / _receptorsSize.x,
                    0.25f);
            Receptors.Position = new Vector2(Cover.Texture.GetWidth() * Cover.Scale.x / -2f,
                RealCoverHeight / 2f - _receptorSize.y * Receptors.Scale.y);
            NoteNode.LoadTextures(Parent.InputRetriever.Keys);
        }


        public void GenerateNoteNodes(List<NoteObject> notes, List<ScrollVelocity> svs)
        {
            GD.Print("Starting Generation");
            if (notes.Count == 0) return;
            for (var i = 0; i < Parent.InputRetriever.Keys; i++)
            {
                var lane = i;
                var laneNotes =
                    new Queue<NoteObject>(notes.Where(x => x.Lane == lane));
                var laneSVs = svs.Where(x => x.Key == -1 || x.Key == lane).Prepend(new ScrollVelocity(0, 1)).ToList();
                var svIndex = 0;
                var currentPosition = new Vector2(0, 0); // Origin
                var paths = new List<Path>();
                var lastSVFactor = 1.0f; // Initial SV
                var baseOffset = new Vector2((ReceptorX[i] + ReceptorX[i + 1]) / 2, 0);

                for (; svIndex < laneSVs.Count; svIndex++)
                {
                    // If there are SV changes between [lastNoteStartTime...currentNoteStartTime), 
                    // break the path into sections
                    while (laneNotes.Count != 0 && (svIndex == laneSVs.Count - 1 ||
                                                    svIndex < laneSVs.Count - 1 && laneNotes.Peek().StartTime <
                                                    laneSVs[svIndex + 1].Time))
                    {
                        var node = NoteNode.Scene.Instance<NoteNode>();
                        var note = laneNotes.Dequeue();
                        if (note.CustomPaths != null && note.CustomPaths.Count != 0)
                        {
                            node.Paths = note.CustomPaths;
                        }
                        else
                        {
                            var path = new Path(Parent.BaseSV, Mathf.Abs(lastSVFactor), laneSVs[svIndex].Time,
                                note.EndTime,
                                new Vector2(0, lastSVFactor > 0 ? 1 : -1),
                                currentPosition, null);
                            node.Paths = paths.Append(path).ToList();
                        }

                        node.Paths = node.Paths.Select(x => x + baseOffset + -node.Paths.Last().EndPosition)
                            .ToList();

                        node.GenerateVisiblePaths(VisibleRegionPos1, VisibleRegionPos2);
                        node.Parent = this;
                        node.Note = note;
                        NoteNodes.Add(node);
                    }

                    // Add final path
                    // If there isn't any SVs between, the start time will be lastNoteStartTime
                    if (svIndex != laneSVs.Count - 1)
                    {
                        paths.Add(new Path(Parent.BaseSV, Mathf.Abs(lastSVFactor), laneSVs[svIndex].Time,
                            laneSVs[svIndex + 1].Time, new Vector2(0, lastSVFactor > 0 ? 1 : -1),
                            currentPosition, null));
                        currentPosition = paths[paths.Count - 1].EndPosition;
                    }

                    lastSVFactor = laneSVs[svIndex].Factor;
                    // GD.Print($"Lane {i} {svIndex}th SV");
                }

                GD.Print($"Lane {i} done");
            }

            NoteNodes.Sort((x, y) => x.Note.StartTime.CompareTo(y.Note.StartTime));
        }

        public void PlaceReceptors()
        {
            for (var i = 0; i < Parent.InputRetriever.Keys; i++)
            {
                var receptor = Receptor.Scene.Instance<Receptor>();
                receptor.Parent = this;
                receptor.KeyIndex = i;
                receptor.LoadTexture();
                receptor.Position = new Vector2(_receptorsSize.x, 0);
                ReceptorX[i] = _receptorsSize.x;
                _receptorsSize.x += receptor.MaxSize().x;
                _receptorSize.x = Math.Max(_receptorSize.x, receptor.MaxSize().x);
                _receptorsSize.y = Math.Max(_receptorsSize.y, receptor.MaxSize().y);
                Receptors.AddChild(receptor);
            }

            ReceptorX[Parent.InputRetriever.Keys] = _receptorsSize.x;

            _receptorSize.y = _receptorsSize.y;

            foreach (var r in Receptors.GetChildren())
                if (r is Receptor receptor)
                    receptor.Position = new Vector2(receptor.Position.x, _receptorSize.y - receptor.MaxSize().y);
        }
    }
}