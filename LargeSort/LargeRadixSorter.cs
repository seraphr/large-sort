using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LargeSort
{
    /// <summary>
    /// MSD radix sortを用いてソートを行うLargeSorterです。
    /// </summary>
    class LargeRadixSorter : LargeSorter
    {
        public LargeRadixSorter(DirectoryInfo aTempDir, int aSortLinesAtOnce)
            : base(aTempDir, aSortLinesAtOnce)
        {

        }

        private class BufferItem
        {
            public BufferItem(IList<string> aRecord, char aKey)
            {
                Record = aRecord;
                Key = aKey;
            }

            public IList<string> Record { get; private set; }
            public char Key { get; private set; }
        }

        protected override void Sort(List<IList<string>> aRecords, int aSortIndex)
        {
            int tSize = aRecords.Count;
            var tBuffer = new BufferItem[tSize];
            Func<IList<string>, int, char> toKey = (aList, aDepth) => {
                string tKeyString = aList[aSortIndex];
                if (tKeyString.Length <= aDepth)
                    return char.MinValue;

                return tKeyString[aDepth]; 
            };

            RadixSort(aRecords, tBuffer, 0, 0, tSize, toKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aRecords">ソート対象</param>
        /// <param name="aBuffer">使用するバッファ。 aRecordsと同じ長さを持つ配列</param>
        /// <param name="aDepth">現在ソートしている桁数。最上位桁を0とする</param>
        /// <param name="aStart">ソート区間の開始。閉区間</param>
        /// <param name="aEnd">ソート区間の終端。開区間</param>
        /// <param name="toKey">レコードと呼び出しの深さから、ソートのKeyとなる値を取り出す関数。文字長を超えた場合は0を返す</param>
        private void RadixSort(List<IList<string>> aRecords, BufferItem[] aBuffer, int aDepth, int aStart, int aEnd, Func<IList<string>, int, char> toKey)
        {
            int tSize = aEnd - aStart;

            // ソート対象区間が1以下ならソート済み
            if (tSize <= 1)
                return;
            
            // ソート対象キーにはascii codeのみ含まれるものとする
            int[] tHistogram = new int[256];
            for (int i = aStart; i < aEnd; i++)
            {
                var tRecord = aRecords[i];
                char tKey = toKey(tRecord, aDepth);

                // 対象キーの数を数える
                tHistogram[tKey]++;
                aBuffer[i] = new BufferItem(tRecord, tKey);
            }

            // 全てのKeyがnull文字であれば、ソート済み
            if (tHistogram[0] == tSize)
            {
                return;
            }

            // tHistgram[key] = keyを持つレコードが 格納されるべきindex終端+1 になるように調整
            tHistogram[0] += aStart;
            for (int i = 1; i < tHistogram.Length; i++)
            {
                tHistogram[i] += tHistogram[i - 1];
            }

            // 後ろからソート済みになるように書き戻す
            // ついでにtHistgram[key] = keyを持つレコードが格納されているindex始端 となるように調整
            for (int i = aEnd - 1; i >= aStart; i--)
            {
                var tRecordItem = aBuffer[i];
                char tKey = tRecordItem.Key;
                int tIndex = --tHistogram[tKey];
                aRecords[tIndex] = tRecordItem.Record;
            }

            int tNewDepth = aDepth + 1;

            for (int i = 1; i < tHistogram.Length; i++)
            {
                int tNewStart = tHistogram[i - 1];
                int tNewEnd = tHistogram[i];
                RadixSort(aRecords, aBuffer, tNewDepth, tNewStart, tNewEnd, toKey);
            }
        }
    }
}
