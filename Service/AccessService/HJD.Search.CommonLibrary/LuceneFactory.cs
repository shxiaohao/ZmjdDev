using HJD.Search.CommonLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using com.hankcs.hanlp;
using java.util;
using com.hankcs.hanlp.suggest;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.tokenizer;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.seg.common;
using System.Text.RegularExpressions;
using PanGu;
using HJD.AccessService.Contract;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace HJD.Search.CommonLibrary
{
    public class LuceneFactory
    {
        private static object lockObj = new object();
        private static IndexReader reader = null; 
	    private static IndexSearcher searcher = null;   

	    /**   
	    *获得IndexReader对象,判断是否为最新,不是则重新打开 (以下省略了try...catch)   
	    *@param  file    索引路径的File对象   
	    *@return   IndexReader对象   
	    **/   
	    public static IndexReader GetIndexReader(string path)
	    {
            lock (lockObj)
            {
                if (reader == null)
                {
                    reader = IndexReader.Open(FSDirectory.Open(path), true);
                }
                else
                {
                    if (!reader.IsCurrent())
                    {
                        reader = reader.Reopen();
                    }
                }
                return reader;   
            }
	    }   

	    /**   
	    * 获得IndexSearcher对象,判断当前的Searcher中reader是否为最新,如果不是,则重新创建IndexSearcher(省略了try...catch)   
	    * @param reader   
	    *            IndexReader对象   
	    * @return IndexSearcher对象   
	    */   
	    public static IndexSearcher GetIndexSearcher(IndexReader reader) 
	    {
            lock (lockObj)
            {
                if (searcher == null) 
		        {   
			        searcher = new IndexSearcher(reader);   
		        }
		        else
		        {   
			        IndexReader r = searcher.IndexReader;   
			        if(!r.IsCurrent())
			        {   
				        searcher = new IndexSearcher(reader);   
			        }   
		        }   
		        return searcher;   
            }
		
	    }
    }
}
