using System.Collections.Generic;
using System.Linq;
using Quadrecep.GameMode.Keys.Map;
using Quadrecep.Gameplay;
using InputEvent = Quadrecep.Gameplay.InputEvent<Quadrecep.GameMode.Keys.Map.NoteObject>;

namespace Quadrecep.GameMode.Keys
{
    public class InputProcessor : AInputProcessor<NoteObject>
    {
        private readonly Queue<JudgementNode> _judgementNodePool = new();
        public int LaneCount = 4;

        public override void _Ready()
        {
            InitTracks(LaneCount);
        }

        public override void FeedNotes(List<NoteObject> notes)
        {
            foreach (var note in notes)
            {
                // Place note press event
                // Don't clear InputLeft when the note is not a long note and is not primary direction
                ExpectedInputs[note.Lane].Enqueue(new InputEvent(note.StartTime, note.Lane, false,
                    note: note.IsLongNote ? null : note));
                // Places long note releases at primary directions.
                // We assume that there wouldn't be any notes inside a long note.
                // It's the mapper's responsibility not to do so.
                if (note.IsLongNote)
                    ExpectedInputs[note.Lane].Enqueue(new InputEvent(note.EndTime, note.Lane, true, note: note));
            }

            Counter.ValidInputCount = ValidInputCount;
            InitJudgementNodePool();
        }

        protected virtual void InitJudgementNodePool()
        {
            _judgementNodePool.Clear();
            for (var i = 0; i < ExpectedInputs.Sum(x => x.Count); i++)
                _judgementNodePool.Enqueue(JudgementNode.Scene.Instance<JudgementNode>());
        }
    }
}