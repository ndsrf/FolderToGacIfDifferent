// Decompiled with JetBrains decompiler

using System;
using System.IO;

namespace FolderToGacIfDifferent
{
    internal class Comparisons
    {
        public static bool FilesContentsAreEqual(FileInfo fileInfo1, FileInfo fileInfo2)
        {
            if (fileInfo1.Length != fileInfo2.Length)
                return false;
            using (FileStream fileStream1 = fileInfo1.OpenRead())
            {
                using (FileStream fileStream2 = fileInfo2.OpenRead())
                    return Comparisons.StreamsContentsAreEqual((Stream)fileStream1, (Stream)fileStream2);
            }
        }

        private static bool StreamsContentsAreEqual(Stream stream1, Stream stream2)
        {
            byte[] buffer1 = new byte[4096];
            byte[] buffer2 = new byte[4096];
        label_1:
            int num1 = stream1.Read(buffer1, 0, 4096);
            int num2 = stream2.Read(buffer2, 0, 4096);
            if (num1 != num2)
                return false;
            if (num1 == 0)
                return true;
            int num3 = (int)Math.Ceiling((double)num1 / 8.0);
            for (int index = 0; index < num3; ++index)
            {
                if (BitConverter.ToInt64(buffer1, index * 8) != BitConverter.ToInt64(buffer2, index * 8))
                    return false;
            }
            goto label_1;
        }
    }
}
