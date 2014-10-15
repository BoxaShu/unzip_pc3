using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


using Ionic.Zlib;

namespace unzip_pc3
{
    class Program
    {
        static void Main(string[] args)
        {

            if(args.Length == 2){
                if (args[0] == "e")
                {
                    if(File.Exists(args[1]))
                    {
                        //' на основе вот этого топака : http://www.theswamp.org/index.php?topic=41529.0
                        string fileName =  args[0]; // "E:\\3\\Default Windows System Printer.pc3";
                        using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
                        {
                            fs.Seek(60L, SeekOrigin.Begin);

                            using (ZlibStream zs = new ZlibStream(fs, CompressionMode.Decompress))
                            {
                                using (StreamReader sr = new StreamReader(zs))
                                {
                                    //тут текстово содержание файла
                                    string s = sr.ReadToEnd();

                                    using (StreamWriter fs_out = File.CreateText(fileName + ".txt"))
                                    {
                                        fs_out.WriteLine(s);

                                    }
                                }
                            }
                        }


                    }
                }

                if (args[0] == "c")
                {

                }
             }
         //Console.ReadLine();

        }
    }
}
