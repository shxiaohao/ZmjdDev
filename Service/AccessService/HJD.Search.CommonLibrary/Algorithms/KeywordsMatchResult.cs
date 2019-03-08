using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Algorithms
{
    public struct KeywordsMatchResult
    {
        #region Members

        private int _index;
        private string _keyword;

        /// <summary>
        /// Initialize string search result
        /// </summary>
        /// <param name="index">Index in text</param>
        /// <param name="keyword">Found keyword</param>
        public KeywordsMatchResult(int index, string keyword)
        {
            _index = index; _keyword = keyword;
        }


        /// <summary>
        /// Returns index of found keyword in original text
        /// </summary>
        public int Index
        {
            get { return _index; }
        }


        /// <summary>
        /// Returns keyword found by this result
        /// </summary>
        public string Keyword
        {
            get { return _keyword; }
        }


        /// <summary>
        /// Returns empty search result
        /// </summary>
        public static KeywordsMatchResult Empty
        {
            get { return new KeywordsMatchResult(-1, ""); }
        }

        #endregion
    }
}