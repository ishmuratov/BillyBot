using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureSearchExample
{
    [Serializable]
    public class ResponseCache : IData
    {
        public List<YandexImageResponse> CacheData { get; set; }

        public ResponseCache()
        {
            CacheData = new List<YandexImageResponse>();
        }

        public void Add(YandexImageResponse _newResponse)
        {
            if (CacheData != null)
            {
                CacheData.Add(_newResponse);
            }
        }

        public void Remove(string _name)
        {
            for (int i = 0; i < CacheData.Count; i++)
            {
                if (CacheData[i].Name == _name)
                {
                    CacheData.Remove(CacheData[i]);
                }
            }
        }

        public YandexImageResponse FindCacheOReturnNull(string _name)
        {
            foreach (YandexImageResponse anyNote in CacheData)
            {
                if (anyNote.Name == _name)
                {
                    return anyNote;
                }
            }
            return null;
        }
    }
}
