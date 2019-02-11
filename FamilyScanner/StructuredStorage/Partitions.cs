using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.IO.Packaging;

namespace StructuredStorage
{
    public class PartitionsStorage : StorageStreamBase
    {

        public OrderedDictionary Partitions = new OrderedDictionary();

        public PartitionsStorage(string fileName, StorageInfo storage) : base(fileName, storage)
        {
            ReadPartitions();
        }

        public void ReadPartitions()
        {
            if (IsInitialized)
            {
                return;
            }

            try
            {
                StreamInfo[] partitions = Storage.GetSubStorageInfo("Partitions").GetStreams();
                if (partitions.Length > 1)
                {
                    Console.WriteLine("hold it");
                }
                foreach (StreamInfo p in partitions)
                {
                    CrcStream partitionStream = new CrcStream(p.GetStream());
                    StreamReader r = new StreamReader(partitionStream);
                    r.ReadToEnd();

                    Partitions.Add(p.Name, partitionStream.ReadCrc.ToString("X8"));
                    //Console.WriteLine("Stream {0}: {1}", p.Name, partitionStream.ReadCrc.ToString("X8"));
                }
            }
            catch { IsInitialized = false; }
            IsInitialized = true;
        }

    }



}
