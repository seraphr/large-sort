using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LargeSort
{
    class Program
    {
        static void Main(string[] args)
        {
            FileInfo tSortFile = new FileInfo("data.dat");
            int tSortFileLines = 18765432;
            int tSortAtOnce = 700000;

            var tQuickSorter = new LargeQuickSorter(new DirectoryInfo("temp_quick"), tSortAtOnce);
            var tRadixSorter = new LargeRadixSorter(new DirectoryInfo("temp_raix"), tSortAtOnce);
            var tNopSorter = new NopLargeSorter(new DirectoryInfo("temp_nop"), tSortAtOnce);
            FileInfo tQuickSortedFile = new FileInfo("quick_sorted.dat");
            FileInfo tRadixSortedFile = new FileInfo("radix_sorted.dat");
            FileInfo tNopSortedFile = new FileInfo("nop_sorted.dat");

            CreateFile(tSortFile, tSortFileLines);

            Console.WriteLine("radix sort");
            Sort(tRadixSorter, tSortFile, tRadixSortedFile, 2);
            Console.WriteLine();
            Console.WriteLine("quick sort");
            Sort(tQuickSorter, tSortFile, tQuickSortedFile, 2);
            Console.WriteLine();
            Console.WriteLine("nop sort");
            Sort(tNopSorter, tSortFile, tNopSortedFile, 2);

            Console.WriteLine("owata");
            Console.Read();
        }

        private static void Sort(LargeSorter aSorter, FileInfo aSortFile, FileInfo aOutputFile, int aSortIndex)
        {
            using(var tReader = new StreamReader(aSortFile.OpenRead()))
            using (var tWriter = new StreamWriter(aOutputFile.OpenWrite()))
            {
                Console.WriteLine(DateTime.Now + "ソートを開始します。");
                var tSorter = aSorter;
                tSorter.Sort(tReader, tWriter, aSortIndex);
                Console.WriteLine(DateTime.Now + "ソート完了。");
            }
        }

        private static void CreateFile(FileInfo aFile, int aLineCount)
        {
            if (aFile.Exists)
                aFile.Delete();

            Console.WriteLine(DateTime.Now + "ソート対象ファイルを生成します。");
            Random tRnd = new Random(34456);
            using (var tWriter = new StreamWriter(aFile.OpenWrite()))
            {
                for (int i = 0; i < aLineCount; i++)
                {
                    tWriter.WriteLine(CreateLine(tRnd));
                }
            }

            Console.WriteLine(DateTime.Now + "生成完了 " + aLineCount + "行");
        }

        private static char[] ALPHABET = new char[]{
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'g',
            'h',
            'i',
            'j',
            'k',
            'l',
            'm',
            'n',
            'o',
            'p',
            'q',
            'r',
            's',
            't',
            'u',
            'v',
            'w',
            'x',
            'y',
            'z'};
        private static string CreateRandomString(Random aRnd, int aLength){
            StringBuilder tBuilder = new StringBuilder();

            for (int i = 0; i < aLength; i++)
			{
                tBuilder.Append(ALPHABET[aRnd.Next(ALPHABET.Length)]);
			}

            return tBuilder.ToString();
        }

        private static string CreateLine(Random aRnd)
        {
            StringBuilder tBuilder = new StringBuilder();

            tBuilder.Append(CreateRandomString(aRnd, 10));
            tBuilder.Append(',');
            tBuilder.Append(CreateRandomString(aRnd, 10));
            tBuilder.Append(',');
            tBuilder.Append(CreateRandomString(aRnd, 50));
            tBuilder.Append(',');
            tBuilder.Append(CreateRandomString(aRnd, 21));
            tBuilder.Append(',');
            tBuilder.Append(CreateRandomString(aRnd, 15));
            tBuilder.Append(',');
            tBuilder.Append(CreateRandomString(aRnd, 16));

            return tBuilder.ToString();
        }
    }
}
