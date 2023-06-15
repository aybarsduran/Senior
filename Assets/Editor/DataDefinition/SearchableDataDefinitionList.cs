using System;
using System.Collections.Generic;

namespace IdenticalStudios
{
    public abstract class SearchableDataDefinitionList<T> : DataDefinitionList<T> where T : DataDefinition<T>
    {
        public bool IsSearching => m_Filter != string.Empty && m_Filter != CustomGUILayout.k_SearchString;

        private string m_Filter = CustomGUILayout.k_SearchString;


        public SearchableDataDefinitionList(string listName, params ListAction[] customActions) : base(listName, customActions) { }

        protected override void SetDefinitions(T[] dataDefs)
        {
            int prevScriptableCount = Count;

            Definitions.Clear();

            if (dataDefs != null)
            {
                bool isFilterEmpty = string.IsNullOrEmpty(m_Filter) ||
                                 m_Filter.Length < 4 ||
                                 m_Filter == CustomGUILayout.k_SearchString;

                if (isFilterEmpty)
                    Definitions.AddRange(dataDefs);
                else
                    Search(Definitions, dataDefs, m_Filter);
            }

            if (prevScriptableCount != Count)
            {
                int indexToSelect = SelectedIndex;

                if (m_Filter.Length > 3)
                    indexToSelect = GetMostSimilarIndex();

                SelectIndex(indexToSelect);
            }
        }

        protected void DrawSearchBar()
        {
            string prevSearch = m_Filter;

            if (CustomGUILayout.SearchBar(ref m_Filter))
                FocusList(this);

            if (m_Filter != prevSearch)
                RefreshDefinitions();
        }

        private void Search(List<T> list, T[] options, string filter)
        {
            var simplifiedFilter = filter.ToLower();
            for (var i = 0; i < options.Length; i++)
            {
                var option = options[i].Name;
                if (string.IsNullOrEmpty(filter) || option.ToLower().Contains(simplifiedFilter))
                {
                    if (string.Equals(options[i].Name, filter, StringComparison.CurrentCultureIgnoreCase))
                        list.Insert(0, options[i]);
                    else
                        list.Add(options[i]);
                }
            }
        }

        private int GetMostSimilarIndex()
        {
            int mostSimilarValue = 1000;
            int mostSimilarIndex = 0;
            for (int i = 0; i < Definitions.Count; i++)
            {
                int value = m_Filter.DamerauLevenshteinDistanceTo(Definitions[i].Name);
                if (value < mostSimilarValue)
                {
                    mostSimilarIndex = i;
                    mostSimilarValue = value;
                }
            }

            return mostSimilarIndex;
        }
    }
}