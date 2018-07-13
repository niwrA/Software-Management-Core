using niwrA.CommandManager;
using niwrA.CommandManager.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace TextFragmentsShared
{
    public interface ITextFragmentState
    {
        Guid Guid { get; set; }
        string Text { get; set; }
    }
    public class TextFragment : ICommandableEntity
    {
        private ITextFragmentState _state;
        public TextFragment(ITextFragmentRepository repo)
        {
            if(_state == null)
            {
                _state = repo.CreateTextFragmentState();
                _state.Guid = Guid.NewGuid();
                repo.AddTextFragmentState(_state);
            }
        }
        public TextFragment(ITextFragmentRepository repo, ITextFragmentState state): this(repo)
        {
            _state = state;
        }
        public Guid Guid { get { return _state.Guid; } }
        public string Text { get { return _state.Text; } }

        internal void SetText(string text)
        {
            _state.Text = text;
        }
    }
}
