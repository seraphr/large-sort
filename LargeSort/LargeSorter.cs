using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LargeSort
{
    public abstract class LargeSorter
    {
        public LargeSorter(DirectoryInfo aTempDir, int aSortLinesAtOnce)
        {
            if (!aTempDir.Exists)
                aTempDir.Create();

            foreach (var tFile in aTempDir.GetFiles()){
                tFile.Delete();
            }

            mTempDir = aTempDir;
            mSortLinesAtOnce = aSortLinesAtOnce;
        }

        private DirectoryInfo mTempDir;
        private int mSortLinesAtOnce;

        public void Sort(TextReader aReader, TextWriter aOutput, int aSortIndex)
        {
            var tParser = new SimpleCsvParser(aReader);
            List<FileInfo> tSortedFiles = new List<FileInfo>();

            FileInfo tSorted;
            while((tSorted = SortOnce(tParser, aSortIndex, mTempDir, mSortLinesAtOnce)) != null){
                tSortedFiles.Add(tSorted);
            }

            Console.WriteLine(DateTime.Now + " マージするよ " + tSortedFiles.Count + " files");
            Merge(tSortedFiles, aOutput);
        }

        private FileInfo SortOnce(SimpleCsvParser aParser, int aSortIndex, DirectoryInfo aTempDir, int aSortAtOnce)
        {
            var tTempFile = new FileInfo(aTempDir.FullName + "\\" + Path.GetRandomFileName());

            List<IList<string>> tRecords = new List<IList<string>>(aSortAtOnce);

            {
                IList<string> tRecord;
                while (tRecords.Count < aSortAtOnce && (tRecord = aParser.parseRecord()) != null)
                {
                    tRecords.Add(tRecord);
                }
            }

            if (tRecords.Count == 0)
                return null;

            Sort(tRecords, aSortIndex);

            using (var tWriter = new StreamWriter(tTempFile.OpenWrite()))
            {
                foreach (var tRecord in tRecords)
                {
                    // ソート対象カラムにはカンマとか無いという前提で、Wrapせずに先頭に書き込む
                    tWriter.Write(tRecord[aSortIndex]);
                    foreach (var tField in tRecord)
                    {
                        tWriter.Write(",");
                        tWriter.Write(WrapField(tField));
                    }
                    tWriter.WriteLine();
                }
            }


            return tTempFile;
        }

        private string WrapField(string aField)
        {
            if (aField.Contains('"'))
            {
                return '"' + aField.Replace("\"", "\"\"") + '"';
            }
            else
            {
                return aField;
            }
        }

        protected abstract void Sort(List<IList<string>> aRecords, int aSortIndex);

        protected virtual void Merge(List<FileInfo> aSortedFiles, TextWriter aOutput)
        {
            // FileOpenに失敗した時の事考えてないので注意
            var tOpened = aSortedFiles.Map(aFile => new LinePeekableReader(new StreamReader(aFile.OpenRead())));
            Merge(tOpened, aOutput);

            tOpened.ForEach(aReader => aReader.Dispose());

        }

        /// <summary>
        /// 部分ファイルを一気にマージする
        /// </summary>
        /// <param name="aReaders"></param>
        /// <param name="aOutput"></param>
        private void Merge(List<LinePeekableReader> aReaders, TextWriter aOutput)
        {
            LinePeekableReader tMinReader = null;
            List<LinePeekableReader> tReaders = new List<LinePeekableReader>(aReaders);

            while(true){
                // 読み終わったReaderを除去
                tReaders.RemoveAll(r => r.PeekLine() == null);
                tMinReader = tReaders.MinBy((l, r) => l.PeekLine().CompareTo(r.PeekLine()));
                if (tMinReader == null)
                    break;

                string tWriteLine = tMinReader.ReadLine();
                aOutput.WriteLine(RemoveFirstField(tWriteLine));
            }
        }

        /// <summary>
        /// 先頭に付与したソート対象カラムを除去する
        /// </summary>
        /// <param name="aLine"></param>
        /// <returns></returns>
        private string RemoveFirstField(string aLine)
        {
            return aLine.Substring(aLine.IndexOf(',') + 1);
        }
    }

    static class ListUtils
    {
        public static List<R> Map<T, R>(this List<T> aList, Func<T, R> f){
            List<R> tResult = new List<R>(aList.Count);

            foreach (var tItem in aList)
            {
                tResult.Add(f(tItem));
            }

            return tResult;
        }

        public static T MinBy<T>(this List<T> aList, Comparison<T> f)
        {
            if (aList.Count == 0)
                return default(T);

            T tMin = aList[0];

            for (int i = 1; i < aList.Count; i++)
            {
                T tTarget = aList[i];
                if (f(tTarget, tMin) < 0)
                {
                    tMin = tTarget;
                }
            }

            return tMin;
        }
    }
}
