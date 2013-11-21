using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LargeSort
{
    /// <summary>
    /// 簡易CSVパーザです。
    /// RFC4180に準拠し、フィールドに改行コードを含まないCSVデータが入力として与えられることを前提としています。
    /// </summary>
    public class SimpleCsvParser
    {
        public SimpleCsvParser(TextReader aReader)
        {
            mReader = aReader;
        }

        private TextReader mReader;

        public IList<string> parseRecord()
        {
            string tLine = mReader.ReadLine();
            if (tLine == null)
                return null;

            var tRecord = new List<string>();
            var tLineReader = new StringReader(tLine);

            string tField;
            while ((tField = parseField(tLineReader)) != null)
            {
                tRecord.Add(tField);
            }
            
            return tRecord;
        }

        /// <summary>
        /// 超適当実装
        /// 想定していない、不正なCSVが入ってきた時の挙動は不定です。
        /// </summary>
        /// <param name="aReader"></param>
        /// <returns></returns>
        private string parseField(TextReader aReader)
        {
            var tReader = aReader;

            var tHead = tReader.Read();
            if (tHead < 0)
                return null;

            var tBuilder = new StringBuilder();
            bool tQuoted = tHead == '"';
            if (!tQuoted)
                tBuilder.Append((char)tHead);

            int tRead;
            while ((tRead = tReader.Read()) != -1)
            {
                var tChar = (char)tRead;
                if (tQuoted && tChar == '"')
                {
                    int tNext = tReader.Peek();
                    if (tNext == '"')
                    {
                        tReader.Read();
                        tBuilder.Append('"');
                        continue;
                    }
                    else if(tNext == ',')
                    {
                        tReader.Read();
                        return tBuilder.ToString();
                    }
                }
                else if (!tQuoted && tChar == ',')
                {
                    return tBuilder.ToString();
                }

                tBuilder.Append(tChar);
            }

            if(tQuoted)
                throw new Exception("ダブルクオートで始まったフィールドがダブルクオートで終わりませんでした。");

            return tBuilder.ToString();
        }
    }
}
