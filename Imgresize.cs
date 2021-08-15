using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using CommandLine;
using CommandLine.Text;

namespace ImageResizer
{
    class Imgresize
    {
        private static void Main(string[] args)
        {
            OptionAnalyzer.AnalyzeArguments(args);
        }
    }

    public class OptionAnalyzer
    {
        public static void AnalyzeArguments(string[] args)
        {
            using (var parser = new Parser((setting) => setting.HelpWriter = null))
            {
                var parsed = parser.ParseArguments<Options>(args);
                parsed.WithParsed(suceeded =>
                {
                    Console.WriteLine("succeeded");
                });
                parsed.WithNotParsed(failed =>
                {
                    var helpText = HelpText.AutoBuild(parsed);
                    Console.WriteLine(helpText);
                    Console.ReadLine();
                });
            }
            Console.ReadLine();
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
        [Value(0, Required = true, HelpText = "入力先のファイル名")]
        public string InputPath { get; set; }
        
        [Value(1, Required = true, HelpText = "出力先のファイル名")]
        public string OutputPath { get; set; }
        
        [Value(2, Required = true, HelpText = "画像の拡大/縮小率")]
        public int Ratio { get; set; }
        
        [Option('c', "clipboad", HelpText = "クリップボードから読み込む")]
        public bool UseClipboad { get; set; }
        
        [Option('o', "overwrite", HelpText = "元のファイルを上書きする")]
        public bool Overwrite { get; set; }
    }
}
