using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace ImageResizer
{
    class Imgresize
    {
        static void Main(string[] args)
        {
        }


    }

    class OptionAnalyzer
    {

    }

    class JsonSerializer
    {

    }

    class Options
    {
        [Option('i', "input", HelpText = "入力先のファイル名")]
        public string InputPath { get; set; }

        [Option('o', "output", HelpText = "出力先のファイル名")]
        public string OutputPath { get; set; }

        [Option('r', "ratio", HelpText = "画像の拡大/縮小率")]
        public int Ratio { get; set; }

        [Option('c', "clipboad", HelpText = "クリップボードから読み込む")]
        public bool UseClipboad { get; set; }

        [Option("overwrite", HelpText = "元のファイルを上書きする")]
        public bool Overwrite { get; set; }

        public string ShowUsage()
        {
            var help = new HelpText();

            help.AddDashesToOption = true;

            help.AddPreOptionsLine("構文:  imresize [options] \n");
            help.AddPreOptionsLine("オプション");
            help.AddOptions<Options>(this);

            return help;
        }
    }
}
