using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Algorithms
{
    /// <summary>
    /// Interface containing all methods to be implemented
    /// by string search Algorithms
    /// </summary>
    public interface IKeywordsMatchAlgorithms
    {
        #region Methods & Properties

        /// <summary>
        /// List of keywords to search for
        /// </summary>
        string[] Keywords { get; set; }


        /// <summary>
        /// Searches passed text and returns all occurrences of any keyword
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <returns>Array of occurrences</returns>
        KeywordsMatchResult[] FindAll(string text);

        /// <summary>
        /// Searches passed text and returns first occurrence of any keyword
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <returns>First occurrence of any keyword (or StringSearchResult.Empty if text doesn't contain any keyword)</returns>
        KeywordsMatchResult FindFirst(string text);

        /// <summary>
        /// Searches passed text and returns true if text contains any keyword
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <returns>True when text contains any keyword</returns>
        bool ContainsAny(string text);

        #endregion
    }
}