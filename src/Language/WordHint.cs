using System.Collections.Generic;

namespace SplitAndMerge
{
    public class WordHint
    {
        string m_text;

        public int Id { get; }
        public string OriginalText { get; }
        public string Text { get { return m_text; } }

        public bool Equals(WordHint other)
        {
            return Id == other.Id;
        }
        public bool Exists(List<WordHint> others)
        {
            for (int i = 0; i < others.Count; i++)
            {
                if (Equals(others[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public WordHint(string word, int id)
        {
            OriginalText = word;
            Id = id;
            m_text = Utils.RemovePrefix(OriginalText);
        }
    }
}
