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
        // args[0] - ключ для паковки / распаковки
        //              e - распаковать / extract
        //              c - запаковать / compress
        // args[1] - путь до файла, который будем паковать или распаковывать / path to file
        
        static void Main(string[] args)
        {
            //' на основе вот этого топака : http://www.theswamp.org/index.php?topic=41529.0

            if(args.Length == 2){
                // тут распаковываем архивчик
                if (args[0] == "e")
                {
                    if(File.Exists(args[1]))
                    {
                        
                        string fileName =  args[1];
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

                // Тут запаковываем все обратно
                if (args[0] == "c")
                {
                    if (File.Exists(args[1]))
                    {
                        string fileName = args[1];
                        using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
                        {
                            using (StreamReader sr = new StreamReader(fs))
                            {
                                String pref_s = "PIAFILEVERSION_2.0,PC3VER1,compress\r\npmzlibcodec";
                                
                                //Byte[] pref_byte = { 255,255,255,255,255,255,255,000,255,255,255,000 };
                                //Byte[] pref_byte = { 157, 94, 173, 006, 215, 016, 000, 000, 199, 004, 000, 000 };
                                // Эти данные(байты) должны вычеслятся
                                Byte[] pref_byte = { 157, 94, 173, 006, 215, 016, 000, 000, 199, 004, 000, 000 };

                                pref_s = pref_s + Encoding.Default.GetString(pref_byte);

                                String s = sr.ReadToEnd();
                                                               

                                using (FileStream fs_out = File.Open(fileName + ".pc3", FileMode.Create, FileAccess.ReadWrite))
                                {
                                    using (ZlibStream zs = new ZlibStream(fs_out, CompressionMode.Compress,
                                                                             CompressionLevel.BestCompression, false))
                                    {
                                        fs_out.Write(Encoding.Default.GetBytes(pref_s), 0, Encoding.Default.GetBytes(pref_s).Length);
                                        zs.Write(Encoding.Default.GetBytes(s), 0, Encoding.Default.GetBytes(s).Length);
                                    }
                                }
                            }
                        }
                    }
                }
             }


        }
    }
}
