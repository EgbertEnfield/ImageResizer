using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
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
                    helpText.AddPostOptionsLine("You can specify \"clipboad\" at \"source\" and \"dest\"");
                    helpText.AddPostOptionsLine("Application will load from clipboad and outputs picture specified path or on clipboad");
                    Console.WriteLine(helpText);
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
        [Value(0, Required = true, HelpText = "Source file path")]
        public string InputPath { get; set; }

        [Value(1, Required = true, HelpText = "Destination file path")]
        public string OutputPath { get; set; }

        [Value(2, Required = true, HelpText = "Ratio")]
        public int Ratio { get; set; }

        [Option('j', "json", HelpText = "load settings from json.")]
        public string LoadFromJson { get; set; }
    }

    [DataContract]
    class JsonOptions
    {
    }
}
