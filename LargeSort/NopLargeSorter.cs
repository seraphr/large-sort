using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LargeSort
{
    /// <summary>
    /// 実際にはソートを行わないLargeSorterです
    /// </summary>
    class NopLargeSorter : LargeSorter
    {
        public NopLargeSorter(DirectoryInfo aTempDir, int aSortLinesAtOnce)
            : base(aTempDir, aSortLinesAtOnce)
        {

        }

        protected override void Sort(List<IList<string>> aRecords, int aSortIndex)
        {
        }
    }
}
