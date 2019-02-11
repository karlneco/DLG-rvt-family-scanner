using System;

public class Partitions : StorageStreamBase
{
	public Partitions(string fileName, StorageInfo storage) : base (fileName, storage)
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
            StreamInfo[] stream = Storage.GetStreams();
            foreach (StreamInfo s in streams)
            {
                Console.WriteLine(s.Name);
            }
        }
        catch { }
    }

}


