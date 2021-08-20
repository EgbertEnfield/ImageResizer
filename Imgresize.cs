using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using System.Threading;
using CommandLine;
using CommandLine.Text;

namespace ImageTools
{
    class Imgresize
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Image image = null;
            ArgumentValue argValue = OptionAnalyzer.AnalyzeArguments(args);
            if (argValue.InputPath == "clipboard")
            {
                image = ImageProcesser.GetClipBoardImage();
                if (image != null)
                {
                    if (argValue.UsePercent == true)
                    {
                        double percent = argValue.Ratio * 0.01;
                        int newWidth = (int)Math.Round((double)image.Width * percent);
                        int newHeight = (int)Math.Round((double)image.Height * percent);

                        Console.WriteLine($"{image.Width}, {image.Height}");
                        Console.WriteLine($"{percent}, {newWidth}, {newHeight}");

                        image = ImageProcesser.CreateThumbnail(image, newWidth, newHeight);
                    }
                    else if (argValue.UsePixel == true)
                    {
                        double ratio = (double)argValue.Ratio / (double)image.Width;
                        int newWidth = (int)Math.Round((double)image.Width * ratio);
                        int newHeight = (int)Math.Round((double)image.Height * ratio);

                        image = ImageProcesser.CreateThumbnail(image, newWidth, newHeight);
                    }
                }
            }
            else 
            if (Regex.IsMatch(argValue.InputPath, @"^?:\\*\.*"))
            {
                try
                {
                    image = Image.FromFile(argValue.InputPath);
                }
                catch (Exception ex)
                {
                    OptionAnalyzer.ShowError($"{ex}: Can't open file");
                }
            }

            if (argValue.OutputPath == "clipboard")
            {
                Clipboard.Clear();
                Clipboard.SetImage(image);
            }
            else
            {
                Console.WriteLine(argValue.OutputPath);
                image.Save(argValue.OutputPath, ImageFormat.Png);
                string ext = Path.GetExtension(argValue.InputPath);
                switch (ext)
                {
                    case ".jpg":
                    case ".jpeg":
                        Console.WriteLine("jpg");
                        break;
                    case ".png":
                        Console.WriteLine("png");
                        break;
                }
            }
            Console.ReadLine();
        }

        static string GetEscapedPath(string path)
        {
            if (Regex.IsMatch(path, @"^[a-zA-Z]:\\*"))
            {
                Regex.Replace(path, @"\\", @"\\\\");
                return path;
            }
            else
            {
                return path;
            }
        }
    }

    public class OptionAnalyzer
    {
        /// <summary>
        /// プログラムの引数を解析し、不適ならばヘルプを表示する。
        /// </summary>
        /// <param name="arguments">起動時のプログラムの引数</param>
        /// <returns>値が入ったArgumentValueのインスタンス</returns>
        public static ArgumentValue AnalyzeArguments(string[] arguments)
        {
            Options options;
            var result = (ParserResult<Options>)Parser.Default.ParseArguments<Options>(arguments);
            if (result.Tag == ParserResultType.Parsed)
            {
                var parsed = (Parsed<Options>)result;
                options = parsed.Value;
                if (options.UsePercent == true && options.UsePixel == true)
                {
                    ShowError("-p and -x option can't be specified at same time.");
                    return new ArgumentValue();
                }
                else if (options.UsePercent == false && options.UsePixel == false)
                {
                    ShowError("-p or -x option must be specified at same time.");
                    return new ArgumentValue();
                }
                else
                {
                    var argValues = new ArgumentValue()
                    {
                        InputPath = options.InputPath,
                        OutputPath = options.OutputPath,
                        JsonPath = options.LoadFromJson,
                        Ratio = options.Ratio,
                        UsePercent = options.UsePercent,
                        UsePixel = options.UsePixel
                    };
                    return argValues;
                }
            }
            else
            {
                try
                {
                    if (arguments[0] != "--version" || arguments[0] == "-?")
                    {
                        Console.WriteLine("You can specify \"clipboad\" at \"source\" and \"dest\"");
                        Console.WriteLine("Application will load from clipboad and outputs picture specified path or on clipboad");
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine("Exception happend");
                    Console.WriteLine(string.Format("{0}: No argument exists", ex.GetType().ToString()));
                }
                return new ArgumentValue();
            }
        }

        public static void ShowError(string message)
        {
            using (var parser = new Parser((setting) => setting.HelpWriter = null))
            {
                var parsed = parser.ParseArguments<Options>( new string[] { "--help" } );
                parsed.WithNotParsed(er =>
                {
                    var helpText = HelpText.AutoBuild(parsed);
                    Console.WriteLine(helpText);
                    Console.WriteLine(message);
                });
            }
        }
    }

    public static class JsonSerializer
    {
        /// <summary>
        /// Serialize class-object to Json
        /// </summary>
        /// <typeparam name="T">class name</typeparam>
        /// <param name="src">instance of T</param>
        /// <param name="useIndent">Whether to indent json</param>
        /// <returns>serialized Json</returns>
        public static string Serialize<T>(T src, bool useIndent)
        {
            using (var ms = new MemoryStream())
            {
                var settings = new DataContractJsonSerializerSettings()
                {
                    UseSimpleDictionaryFormat = true,
                };
                var serializer = new DataContractJsonSerializer(typeof(T), settings);

                if (useIndent)
                {
                    using (var writer = JsonReaderWriterFactory.CreateJsonWriter(ms, Encoding.UTF8, false, true, "  "))
                    {
                        serializer.WriteObject(writer, src);
                        writer.Flush();
                    }
                }
                else
                {
                    serializer.WriteObject(ms, src);
                }

                using (var sr = new StreamReader(ms))
                {
                    ms.Position = 0;
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Deserialize Json to class-object
        /// </summary>
        /// <typeparam name="T">class name</typeparam>
        /// <param name="json">json string</param>
        /// <returns>class instance</returns>
        public static T Deserialize<T>(string json)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var setting = new DataContractJsonSerializerSettings()
                {
                    UseSimpleDictionaryFormat = true,
                };
                var serializer = new DataContractJsonSerializer(typeof(T), setting);
                return (T)serializer.ReadObject(ms);
            }
        }
    }

    public static class ImageProcesser
    {
        internal static Image GetClipBoardImage()
        {
            Image image = null;
            Thread thread = new Thread(() => image = Clipboard.GetImage());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (image == null)
            {
                string errmsg = "No image founds on the clipboard";
                OptionAnalyzer.ShowError(errmsg);
                Console.ReadLine();
                return null;
            }
            return image;
        }

        internal static Image CreateThumbnail(Image image, int width, int height)
        {
            Bitmap canvas = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(canvas);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);

            float fw = (float)width / (float)image.Width;
            float fh = (float)height / (float)image.Height;

            float scale = Math.Min(fw, fh);
            fw = image.Width * scale;
            fh = image.Height * scale;

            g.DrawImage(image, (width - fw) / 2, (height - fh) / 2, fw, fh);
            g.Dispose();

            return canvas;
        }
    }

    class Options
    {
        [Option('i', "input", Default = "clipboard", Separator = ',', Required = true, HelpText = "Source file path")]
        public string InputPath { get; set; }

        [Option('o', "output", Default = "clipboard", Separator = ',', Required = true, HelpText = "Destination file path")]
        public string OutputPath { get; set; }

        [Option('r', "ratio", Required = true, HelpText = "Ratio")]
        public int Ratio { get; set; }

        [Option('j', "json", HelpText = "load settings from json.")]
        public string LoadFromJson { get; set; }

        [Option('p', "percent", HelpText = "Use percentage to ratio")]
        public bool UsePercent { get; set; }

        [Option('x', "pixel", HelpText = "Use pixel to ratio")]
        public bool UsePixel { get; set; }
    }

    public class ArgumentValue
    {
        public string InputPath { get; set; }

        public string OutputPath { get; set; }

        public string JsonPath { get; set; }

        public int Ratio { get; set; }

        public bool UsePercent { get; set; }

        public bool UsePixel { get; set; }
    }

    [DataContract]
    class JsonOptions
    {
    }
}
