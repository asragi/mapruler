using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace sub_theme
{
    class Program
    {
        static void Main(string[] args)
        {
            string myDirectory = Directory.GetCurrentDirectory();
            Console.WriteLine("PicturePathIs...");
            string inputPath = myDirectory + "\\" + Console.ReadLine();
            Console.WriteLine("TargetPathIs...");
            string targetPath = myDirectory + "\\" + Console.ReadLine();
            try
            {
                Color[,] pixelData;
                int width, height;
                ImageFormat format;
                //指定したパスから画像を読み込む
                using (Bitmap img = new Bitmap(Image.FromFile(inputPath)))
                {
                    //画像サイズを取得
                    width = img.Width;
                    height = img.Height;
                    //追記：元画像のフォーマットを保持
                    format = img.RawFormat;
                    //ピクセルデータを取得
                    pixelData = new Color[img.Width, img.Height];
                    for (int y = 0; y < img.Height; y++)
                    {
                        for (int x = 0; x < img.Width; x++)
                        {
                            pixelData[x, y] = img.GetPixel(x, y);
                        }
                    }
                }

                // 処理
                var textLine = "";
                using (var sw = new StreamWriter(
                    targetPath,
                    false,
                    System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    var sb = new System.Text.StringBuilder(width * height);
                    CompressSystem(pixelData, width, height, sb);
                    textLine = sb.ToString();
                    sw.Write(textLine);
                    sw.Close();
                }
                Console.WriteLine("success!");
            }
            catch (Exception e)
            {
                Console.WriteLine("error!");
                Console.WriteLine(e.ToString());
            }
            Console.ReadKey();
        }

        /// <summary>
        /// 白赤画像の単純な二値変換
        /// </summary>
        private static void OlderSystem(Color[,] pixelData, int width, int height, System.Text.StringBuilder sb)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (pixelData[j, i].B > 100) sb.Append(0);
                    else sb.Append(1);
                }
                sb.AppendLine();
            }
        }

        // ````````
        // `000111`
        // `110000`
        // ````````
        // ↓↓↓↓
        // 0,3
        // 1,3
        //
        // 1,2
        // 0,4
        /// <summary>
        /// 圧縮式
        /// </summary>
        private static void CompressSystem(Color[,] pixelData, int width, int height, System.Text.StringBuilder sb)
        {
            int count = 0;
            for (int i = 0; i < height; i++)
            {
                bool isGround = pixelData[0, i].B > 100;
                for (int j = 0; j < width; j++)
                {
                    if(pixelData[j, i].B > 100 != isGround)
                    {
                        WriteData(sb, isGround, count);
                        isGround = pixelData[j, i].B > 100;
                        count = 0;
                    }
                    count++;
                }
                WriteData(sb, isGround, count);
                count = 0;
                sb.AppendLine();
            }

            void WriteData(System.Text.StringBuilder _sb, bool _isGround, int _count)
            {
                int num = _isGround ? 0 : 1;
                _sb.Append(num);
                _sb.Append(",");
                _sb.Append(_count);
                _sb.AppendLine();
            }
        }
    }
}
