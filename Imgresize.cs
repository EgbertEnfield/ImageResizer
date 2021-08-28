﻿using System;
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

namespace ImgresizeGUI
{
    class Imgresize
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Image image = null;
            Options argValue = OptionAnalyzer.AnalyzeArguments(args);

            if (argValue == null) return;

            else if (argValue.InputPath == "clipboard")
            {
                try
                {
                    image = ImageProcesser.GetClipBoardImage();
                    if (image == null) throw new NullReferenceException();
                    switch (argValue.Mode)
                    {
                        case SizeMode.percent:
                            double percent = (double)argValue.Size / (double)100;
                            int newWidth_p = (int)Math.Round((double)image.Width * percent);
                            int newHeight_p = (int)Math.Round((double)image.Height * percent);

                            image = ImageProcesser.CreateThumbnail(image, newWidth_p, newHeight_p);
                            break;
                        case SizeMode.pixel:
                            double ratio = (double)argValue.Size / (double)image.Width;
                            int newWidth_x = (int)Math.Round((double)image.Width * ratio);
                            int newHeight_x = (int)Math.Round((double)image.Height * ratio);

                            image = ImageProcesser.CreateThumbnail(image, newWidth_x, newHeight_x);
                            break;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[Suceeded] Loaded from clipboard.");
                }
                catch (NullReferenceException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("[Error] There are no images on the clipboard.\tCode:0x{0:X8}", ex.HResult));
                    return;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("[Error] Unknown error occured.\tCode:0x{0:X8}", ex.HResult));
                    return;
                }
                finally
                {
                    Console.ResetColor();
#if DEBUG
                    Console.ReadKey();
#endif
                }
            }
            else if (Regex.IsMatch(argValue.InputPath, @"\A[a-zA-Z]:\\.*\..+\z|\A\.\\.*\..+\z|\A[^\\]*\..+\z"))
            {
                try
                {
                    image = Image.FromFile(argValue.InputPath);
                    if (image == null) throw new NullReferenceException();
                    switch (argValue.Mode)
                    {
                        case SizeMode.percent:
                            double percent = (double)argValue.Size / (double)100;
                            int newWidth_p = (int)Math.Round((double)image.Width * percent);
                            int newHeight_p = (int)Math.Round((double)image.Height * percent);

                            image = ImageProcesser.CreateThumbnail(image, newWidth_p, newHeight_p);
                            break;
                        case SizeMode.pixel:
                            double ratio = (double)argValue.Size / (double)image.Width;
                            int newWidth_x = (int)Math.Round((double)image.Width * ratio);
                            int newHeight_x = (int)Math.Round((double)image.Height * ratio);

                            image = ImageProcesser.CreateThumbnail(image, newWidth_x, newHeight_x);
                            break;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(string.Format("[Suceeded] Loaded from {0}", argValue.InputPath));
                }
                catch (NullReferenceException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("[Error] No images exists on the clipboard.\tCode:0x{0:X8}", ex.HResult));
                    return;
                }
                catch (FileNotFoundException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("[Error] Source file does not exists.\tCode:0x{0:X8}", ex.HResult));
                    return;
                }
                catch (ArgumentException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("[Error] Source path format is uri.\tCode:0x{0:X8}", ex.HResult));
                    return;
                }
                catch (OutOfMemoryException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("[Error] Image format of source file is not available.\tCode:0x{0:X8}", ex.HResult));
                    return;
                }
                finally
                {
                    Console.ResetColor();
#if DEBUG
                    Console.ReadKey();
#endif
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
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[Suceeded] Saved on the clipboard.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("[Error] Unknown error occured.\tCode:0x{0:X8}", ex.HResult));
                    return;
                }
                finally
                {
                    Console.ResetColor();
#if DEBUG
                    Console.ReadKey();
#endif
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
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(string.Format("[Suceeded] Saved at {0}", argValue.OutputPath));
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("[Error]\tUnknown error occured.\tCode:0x{0:X8}", ex.HResult));
                    return;
                }
                finally
                {
                    Console.ResetColor();
#if DEBUG
                    Console.ReadKey();
#endif
                }
            }
            else
            {
                OptionAnalyzer.ShowError("unable path or format");
                return;
            }
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
                if (arguments.Length != 0)
                {
                    if (arguments[0] == "-?")
                    {
                        ShowError("");
                        return null;
                    }
                }
                else throw new ArgumentException();

                var result = (ParserResult<Options>)Parser.Default.ParseArguments<Options>(arguments);
                if (arguments[0] == "--help" || arguments[0] == "--version")
                {
                    return null;
                }

                if (result.Tag == ParserResultType.Parsed)
                {
                    var parsed = (Parsed<Options>)result;
                    options = parsed.Value;
                    Console.WriteLine(string.Format("Source:\t{0}", options.InputPath));
                    Console.WriteLine(string.Format("Dest:\t{0}", options.OutputPath));
                    Console.WriteLine(string.Format("Size:\t{0} {1}", options.Size, options.Mode));
                    Console.WriteLine();
                    return options;
                }
                else
                {
                    return null;
                }
            }
            catch (ArgumentException ex)
            {
                var result = (ParserResult<Options>)Parser.Default.ParseArguments<Options>(arguments);
                var parsed = (Parsed<Options>)result;
                options = parsed.Value;
                Console.WriteLine(string.Format("Source:\t{0}", options.InputPath));
                Console.WriteLine(string.Format("Dest:\t{0}", options.OutputPath));
                Console.WriteLine(string.Format("Size:\t{0} {1}", options.Size, options.Mode));
                Console.WriteLine();
                Console.WriteLine(string.Format("[info] There are no arguments.\tCode:0x{0:X8}", ex.HResult));
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
