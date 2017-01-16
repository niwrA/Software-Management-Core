using CommandsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace TextFragmentsShared
{
    public interface ITextFragmentRepository: IEntityRepository
    {
        void AddTextFragmentState(ITextFragmentState textFragmentState);
        void DeleteTextFragmentStateByGuid(Guid guid);
        ITextFragmentState GetTextFragmentStateByGuid(Guid guid);
        ITextFragmentState CreateTextFragmentState();
        IEnumerable<ITextFragmentState> GetAllTextFragmentStates();
    }
    public class TextFragments : ITextFragments
    {
        private Injections _inject;
        public TextFragments(Injections inject)
        {
            _inject = inject;
        }
        public TextFragment CreateTextFragment(string text)
        {
            var textFragment = new TextFragment(_inject.TextFragmentRepository);
            textFragment.SetText(text);
            return textFragment;
        }

        public class Injections
        {
            public ITextFragmentRepository TextFragmentRepository { get; set; }
        }

        public IEnumerable<TextFragment> GetAllTextFragments()
        {
            var allStates = _inject.TextFragmentRepository.GetAllTextFragmentStates();
            var returnSet = new List<TextFragment>();
            foreach(var state in allStates)
            {
                returnSet.Add(new TextFragment(_inject.TextFragmentRepository, state));
            }
            return returnSet;
        }
    }
}
