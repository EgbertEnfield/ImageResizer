using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using System.Threading;
using CommandLine;
using CommandLine.Text;

namespace ImageResizer
{
    class Imgresize
    {
        [STAThread]
        private static void Main(string[] args)
        {
            ArgumentValue argValue = OptionAnalyzer.AnalyzeArguments(args);
            Console.WriteLine($"{argValue.InputPath}, {argValue.OutputPath}, {argValue.Ratio}, {argValue.JsonPath}");
            if (argValue.InputPath == "clipboad")
            {
                Image image = GetClipBoardImage();
                if (image != null)
                {
                    Console.WriteLine("=͟͟͞͞○ヽ(･ω･`ヽ)ｷｬｯﾁ!");
                    image.Save("C:\\Users\\stalin\\desktop\\foo.png", ImageFormat.Png);
                }
            }
            Console.ReadLine();
        }

        public static Image GetClipBoardImage()
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
                var argValues = new ArgumentValue()
                {
                    InputPath = options.InputPath,
                    OutputPath = options.OutputPath,
                    JsonPath = options.LoadFromJson,
                    Ratio = options.Ratio,
                };
                return argValues;
            }
            else
            {
                try
                {
                    if (arguments[0] != "--version")
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

    class Options
    {
        [Value(0, Required = true, HelpText = "Source file path")]
        public string InputPath { get; set; }

        [Value(1, Required = true, HelpText = "Destination file path")]
        public string OutputPath { get; set; }

        [Value(2, Required = true, HelpText = "Ratio")]
        public int Ratio { get; set; }

        [Option('j', "json", HelpText = "load settings from json.")]
        public string LoadFromJson { get; set; }

        [Option('s', "scaling", HelpText = "画像の補間アルゴリズム")]
        public string Scaling { get; set; }
    }

    [DataContract]
    class JsonOptions
    {
    }

    public class ArgumentValue
    {
        public string InputPath { get; set; }

        public string OutputPath { get; set; }

        public string JsonPath { get; set; }

        public int Ratio { get; set; }
    }
}
