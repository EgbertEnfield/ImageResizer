using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            Options argValue = OptionAnalyzer.AnalyzeArguments(args);

            if (argValue == null)
            {
                Console.ReadKey();
                return;
            }
            else if (argValue.InputPath == "clipboard")
            {
                try
                {
                    image = ImageProcesser.GetClipBoardImage();
                    //if (image == null)
                    //{
                    //    Console.WriteLine(string.Format("No images exists on the clipboard."));
                    //    Console.ReadKey();
                    //    return;
                    //}
                    if (argValue.Mode == SizeMode.percent)
                    {
                        double percent = (double)argValue.Size / (double)100;
                        int newWidth = (int)Math.Round((double)image.Width * percent);
                        int newHeight = (int)Math.Round((double)image.Height * percent);

                        image = ImageProcesser.CreateThumbnail(image, newWidth, newHeight);
                    }
                    else if (argValue.Mode == SizeMode.pixel)
                    {
                        double ratio = (double)argValue.Size / (double)image.Width;
                        int newWidth = (int)Math.Round((double)image.Width * ratio);
                        int newHeight = (int)Math.Round((double)image.Height * ratio);

                        image = ImageProcesser.CreateThumbnail(image, newWidth, newHeight);
                    }
                    Console.WriteLine("[Suceeded] Loaded from clipboard.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("{0} HResult:0x{1:X8}\n\t[Error] Unknown error occured.", ex.GetType(), ex.HResult));
                    return;
                }
            }
            else if (Regex.IsMatch(argValue.InputPath, @"\A[a-zA-Z]:\\.*\..+\z|\A\.\\.*\..+\z|\A[^\\]*\..+\z"))
            {
                try
                {
                    image = Image.FromFile(argValue.InputPath);
                    Console.WriteLine(string.Format("[Suceeded] Loaded from {0}", argValue.InputPath));
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine(string.Format("{0} HResult:0x{1:X8}\n\t[Error] Source file does not exists.", ex.GetType(), ex.HResult));
                    return;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(string.Format("{0} HResult:0x{1:X8}\n\t[Error] Source path format is uri", ex.GetType(), ex.HResult));
                    return;
                }
                catch (OutOfMemoryException ex)
                {
                    Console.WriteLine(string.Format("{0} HResult:0x{1:X8}\n\t[Error] Image format of source file is not available.", ex.GetType(), ex.HResult));
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("{0} HResult:0x{1:X8}\n\t[Error] Unknown error occured.", ex.GetType(), ex.HResult));
                    return;
                }
            }
            else
            {
                OptionAnalyzer.ShowError("unable path or format");
                return;
            }

            if (argValue.OutputPath == "clipboard")
            {
                try
                {
                    Clipboard.Clear();
                    Clipboard.SetImage(image);
                    Console.WriteLine("[Suceeded] Saved on the clipboard.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("{0}: Unknown error occured.", ex));
                    return;
                }
            }
            else if (Regex.IsMatch(argValue.OutputPath, @"\A[a-zA-Z]:\\.*\..+\z|\A\.\\.*\..+\z|\A[^\\]*\..+\z"))
            {
                try
                {
                    string ext = Path.GetExtension(argValue.OutputPath);
                    switch (ext)
                    {
                        case ".jpg":
                        case ".jpeg":
                            image.Save(argValue.OutputPath, ImageFormat.Jpeg);
                            break;
                        case ".png":
                            image.Save(argValue.OutputPath, ImageFormat.Png);
                            break;
                        case ".gif":
                            image.Save(argValue.OutputPath, ImageFormat.Gif);
                            break;
                        case ".bmp":
                            image.Save(argValue.OutputPath, ImageFormat.Bmp);
                            break;
                    }
                    Console.WriteLine(string.Format("[Suceeded] Saved at {0}", argValue.OutputPath));
                }
                catch (Exception ex)
                {
                    OptionAnalyzer.ShowError(string.Format("{0}: can't save image", ex));
                    return;
                }
            }
            else
            {
                OptionAnalyzer.ShowError("unable path or format");
                return;
            }
            Console.ReadLine();
        }
    }

    public class OptionAnalyzer
    {
        /// <summary>
        /// Analyze parameters and return class object
        /// </summary>
        /// <param name="arguments">起動時のプログラムの引数</param>
        /// <returns>値が入ったArgumentValueのインスタンス</returns>
        public static Options AnalyzeArguments(string[] arguments)
        {
            Options options;
            try
            {
                if (arguments[0] == "-?")
                {
                    ShowError("");
                    return null;
                }

                var result = (ParserResult<Options>)Parser.Default.ParseArguments<Options>(arguments);
                if (arguments[0] == "--help" || arguments[0] == "--version")
                {
                    return null;
                }

                if (result.Tag == ParserResultType.Parsed)
                {
                    var parsed = (Parsed<Options>)result;
                    options = parsed.Value;
                    Console.WriteLine(string.Format("Input:    {0}", options.InputPath));
                    Console.WriteLine(string.Format("Output:   {0}", options.OutputPath));
                    Console.WriteLine(string.Format("Size:     {0}", options.Size));
                    Console.WriteLine(string.Format("SizeMode: {0}", options.Mode));
                    Console.WriteLine();
                    return options;
                }
                else
                {
                    return null;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                var result = (ParserResult<Options>)Parser.Default.ParseArguments<Options>(arguments);
                var parsed = (Parsed<Options>)result;
                options = parsed.Value;
                Console.WriteLine(string.Format("Source: {0}", options.InputPath));
                Console.WriteLine(string.Format("Dest:   {0}", options.OutputPath));
                Console.WriteLine(string.Format("Size:   {0}", options.Size));
                Console.WriteLine(string.Format("Mode:   {0}", options.Mode));
                Console.WriteLine();
                Console.WriteLine(string.Format("{0} HResult:0x{1:X8}\n\t[Notice] No argument specified. Use default.", ex.GetType(), ex.HResult));
                return options;
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
                // Console.ReadKey();
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

    public class Options
    {
        [Option('i', "input", Default = "clipboard", HelpText = "Source file path. You can open on the clipboard with using \"clipboard\"")]
        public string InputPath { get; set; }

        [Option('o', "output", Default = "clipboard", HelpText = "Destination file path. You can output on the clipboard with using \"clipboard\"")]
        public string OutputPath { get; set; }

        [Option('s', "size", Default = 100, HelpText = "Specify percentage or pixel with using -m")]
        public int Size { get; set; }

        [Option('j', "json", HelpText = "load settings from json.(Unimplemented)")]
        public string LoadFromJson { get; set; }

        [Option('m', "mode", Default = SizeMode.percent, HelpText = "specify size mode")]
        public SizeMode Mode { get; set; }
    }

    public enum SizeMode
    {
        percent,
        pixel,
    }

    [DataContract]
    class JsonOptions
    {
    }
}
