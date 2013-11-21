using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LargeSort
{
    /// <summary>
    /// 行単位でPeek可能なReader
    /// ThreadSafeじゃないので注意
    /// </summary>
    public class LinePeekableReader : IDisposable
    {
        public LinePeekableReader(TextReader aUnderlying)
        {
            mUnderlying = aUnderlying;
        }

        private TextReader mUnderlying;
        private string mPeekLine;

        public string PeekLine()
        {
            if (mPeekLine == null)
            {
                mPeekLine = mUnderlying.ReadLine();
            }

            return mPeekLine;
        }

        public string ReadLine()
        {
            if (mPeekLine != null)
            {
                string tResult = mPeekLine;
                mPeekLine = null;
                return tResult;
            }

            return mUnderlying.ReadLine();
        }

        public void Dispose()
        {
            mUnderlying.Dispose();
        }
    }
}
