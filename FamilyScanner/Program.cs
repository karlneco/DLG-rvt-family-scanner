using StructuredStorage;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FamilyScanner
{
    class Program
    {
        public static System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();

        public static OrderedDictionary FamilyRoots = new OrderedDictionary();

        static void Main(string[] args)
        {
            
            //if there are no directories on the command line just die
            if (args.Length < 1)
            {
                Console.WriteLine("Please provide a folder to process.");
                return;
            }


            //go through all the familes in the tree
            Console.WriteLine("Searching for Revit Families...");
            string[] FilesInPath = Directory.GetFiles(args[0], "*.rfa", SearchOption.AllDirectories);
            string rootCRC = "";

            Console.WriteLine("Found {0} Revit families to process", FilesInPath.Length);
            foreach (string file in FilesInPath)
            {
                //Console.WriteLine("\nProcessing {0}...", file);
                Storage storage = new Storage(file);
                //is this a valid revit (family) file? If not then just skip and move on to the next one
                if (storage.IsInitialized == false)
                {
                    Console.WriteLine("[ERROR] {0} doesn't seem to be a valid Family", file);
                    continue;
                }

                //get the CRC of the root partition of the family
                rootCRC = (string)storage.PartitionsInfo.Partitions[0];
                //Console.WriteLine("CRC of partition 0: {0}", rootCRC);

                //find the root of the family tree, if there is no tree for this root, start a new tree
                //and send the family in for generating the tree
                FamilyNode f = null;
                try
                {
                    f = (FamilyNode)FamilyRoots[rootCRC];
                    f.AddChild(rootCRC, file, storage); //this will trace the tree internaly
                }
                catch
                {
                    //no tree exists for this CRC
                    f = new FamilyNode(rootCRC, file, storage);
                    FamilyRoots.Add(rootCRC, f);
                }
            }


            //display the tree of results
            int uniqueCount = 0;
            foreach (FamilyNode f in FamilyRoots.Values)
            {
                Console.WriteLine("CRC: {0}\t{1}", f.rootCRC, f.files.First<string>());
                uniqueCount++;
                if (f.files.Count > 1)
                {
                    foreach (string same in f.files.Skip(1))
                    {
                        Console.WriteLine("\t\t{0}", same);
                    }
                }
            }
            Console.WriteLine("\nFrom the {0} families, {1} are UNIQUE.", FilesInPath.Length, uniqueCount);
            timer.Stop();
            Console.WriteLine("Processed in {0} ms.",timer.ElapsedMilliseconds);

        }
    }



    class FamilyNode
    {
        Hashtable Decendants = new Hashtable();
        public List<string> files = new List<string>();

        public string rootCRC;
        private string file;
        private Storage storage;

        public FamilyNode(string rootCRC, string file, Storage storage)
        {
            this.rootCRC = rootCRC;
            files.Add(file);
            this.storage = storage;
            //AddChild()
        }

        internal void AddChild(string rootCRC, string file, Storage storage)
        {
            rootCRC = (string)storage.PartitionsInfo.Partitions["0"];
            files.Add(file);
        }
    }



}
