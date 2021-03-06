﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace CodeBlox
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Write("check or read (c/r): ");
                string input = Console.ReadLine();
                bool read = input.StartsWith("r", StringComparison.OrdinalIgnoreCase);

                Console.Write("File name: ");
                string path = Console.ReadLine();

                path = Path.Combine("blocks", path);
                if (!path.EndsWith(".block"))
                    path += ".block";

                if (read)
                {
                    ReadBlock(path);
                }
                else
                {
                    CodeBlock block;
                    using (FileStream fs = File.OpenRead(path))
                        block = new CodeBlock(fs);

                    foreach (string filePath in Directory.EnumerateFiles("versions", "*.exe"))
                    {
                        Console.Write(string.Format("'{0}': ", Path.GetFileName(filePath)));
                        byte[] data = File.ReadAllBytes(filePath);
                        Console.WriteLine(block.SeekCount(data, out int whatever));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("done");
            Console.ReadLine();
        }

        const int BlockLength = 32;
        static void ReadBlock(string filePath)
        {

            int address;
            while (true)
            {
                Console.Write("Address: ");

                string input = Console.ReadLine();
                if (input.StartsWith("0x"))
                    input = input.Substring(2);

                if (int.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out address))
                    break;
            }

            byte[] data = File.ReadAllBytes("Stronghold Crusader.exe");

            address -= 0x400000;
            if (address < 0x1000 || address >= data.Length)
                throw new Exception("Address is out of range! " + address);

            int size;
            while (true)
            {
                Console.Write("Choose size: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out size))
                    break;
            }

            if (size <= 0 || address + size > data.Length)
                throw new Exception("Size is out of range! " + size);

            byte[] buf = new byte[size];
            Buffer.BlockCopy(data, address, buf, 0, size);

            if (File.Exists(filePath))
            {
                Console.WriteLine("File does already exists! Press enter to continue.");
                Console.ReadLine();
            }

            using (FileStream fs = File.OpenWrite(filePath))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                for (int i = 0; i < buf.Length; i++)
                {
                    sw.Write(buf[i].ToString("X2"));
                    if (i < buf.Length - 1)
                        sw.Write(" ");
                }
            }
        }
    }
}
