using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LargeSort
{
    /// <summary>
    /// ListのSortメソッドを用いてソートを行うLargeSorterです。
    /// </summary>
    public class LargeQuickSorter : LargeSorter
    {
        public LargeQuickSorter(DirectoryInfo aTempDir, int aSortLinesAtOnce)
            : base(aTempDir, aSortLinesAtOnce)
        {

        }

        protected override void Sort(List<IList<string>> aRecords, int aSortIndex)
        {
            aRecords.Sort((l, r) => l[aSortIndex].CompareTo(r[aSortIndex]));
        }
    }
}
