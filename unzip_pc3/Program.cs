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
                if (File.Exists(args[1]))
                {
                    // тут распаковываем архивчик
                    if (args[0] == "e")
                    {
                        string fileName = args[1];
                        using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
                        {
                            fs.Seek(60L, SeekOrigin.Begin);

                            using (ZlibStream zs = new ZlibStream(fs, CompressionMode.Decompress))
                            {
                                using (StreamReader sr = new StreamReader(zs))
                                {
                                    //тут текстово содержание файла
                                    string s = sr.ReadToEnd();

                                    using (FileStream fs_out = File.Open(fileName + ".txt", FileMode.Create, FileAccess.ReadWrite))
                                    {
                                        fs_out.Write(Encoding.Default.GetBytes(s), 0, Encoding.Default.GetBytes(s).Length);
                                    }
                                }
                            }
                        }
                    }

                    // Тут запаковываем все обратно
                    if (args[0] == "c")
                    {
                        string fileName = args[1];
                        using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
                        {
                            using (StreamReader sr = new StreamReader(fs))
                            {
                                // заголовок файла, без него автокад не работает
                                String pref_s = "PIAFILEVERSION_2.0,PC3VER1,compress\r\npmzlibcodec";
                                /*
                                is a good starting point, but contains several errors/lack of information
                                The header size is 60 No 59
                                bytes 49->52 = ZlibCodec.Adler32
                                bytes 53->56 = decompresse stream size
                                bytes 57->60 = compressed stream size
                                Эти данные(байты) должны вычеслятся
                                */
                                String s = sr.ReadToEnd();
                                //Byte[] ZlibCodec_Adler32 = { 157, 94, 173, 006 };
                                long decompresse_stream_size = fs.Length;
                                //тут считаем размер запакованных данных
                                byte[] compressed_stream_size;
                                using (var ms = new MemoryStream())
                                {
                                    using (ZlibStream deflateStream = new ZlibStream(ms, CompressionMode.Compress,
                                                                             CompressionLevel.BestCompression, false))
                                    {
                                     deflateStream.Write(Encoding.Default.GetBytes(s), 0, Encoding.Default.GetBytes(s).Length);
                                    }
                                    compressed_stream_size = ms.ToArray();
                                }

                                // Тут все пишем в файл
                                using (FileStream fs_out = File.Open(fileName + ".pc3", FileMode.Create, FileAccess.ReadWrite))
                                {
                                    using (ZlibStream zs = new ZlibStream(fs_out, CompressionMode.Compress,
                                                                             CompressionLevel.BestCompression, false))
                                    {
                                        fs_out.Write(Encoding.Default.GetBytes(pref_s), 0, Encoding.Default.GetBytes(pref_s).Length);
                                        //fs_out.Write(ZlibCodec_Adler32, 0, 4);
                                        fs_out.Write(BitConverter.GetBytes(new ZlibCodec(CompressionMode.Compress).Adler32), 0, 4);
                                        fs_out.Write(BitConverter.GetBytes(decompresse_stream_size), 0, 4);
                                        fs_out.Write(BitConverter.GetBytes(compressed_stream_size.Length), 0, 4);
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
