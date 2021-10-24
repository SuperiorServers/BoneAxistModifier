using System;
using System.IO;
using System.Text.RegularExpressions;

namespace BoneAxisModifier
{
	class Program
	{
        static readonly Regex BaseDataRegex = new(@"(Scale|Position|Rotate)([XYZ])");

        static void Main(string[] args)
		{
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: BoneAxisModifier.exe <file> <animName> <animTarget> <additionTransform>");
                Console.ReadLine();
                return;
            }

            string file = args[0];

            if (!File.Exists(file))
            {
                Console.WriteLine($"'{file}' does not exist!");
                Console.ReadLine();
                return;
            }

            if (!file.EndsWith(".json"))
            {
                Console.WriteLine("Warning supplied file is not a JSON file! Stuff may break!\n\n Press any key to continue..");
                Console.ReadLine();
            }

            string baseDataKey = args[2];

            if (!BaseDataRegex.IsMatch(args[2]))
			{
                Console.WriteLine($"Invalid target: {args[2]}");
                Console.ReadLine();
                return;
            }

            int baseDataIndex = 1;
            MatchCollection matches = BaseDataRegex.Matches(args[2]);

			foreach (Match v in matches)
            {
                if (v.Groups.Count > 1)
                {
                    baseDataKey = v.Groups[1].Value;

                    if (baseDataKey == "Position") // 1 off inconsistent naming
                        baseDataKey = "Translate";

                    switch (v.Groups[2].Value)
					{
                        case "X":
                            baseDataIndex = 0;
                            break;

                        case "Y":
                            baseDataIndex = 1;
                            break;
                        case "Z":
                            baseDataIndex = 2;
                            break;

                    }
                }
            }

            if (!decimal.TryParse(args[3], out decimal transform))
			{
				Console.WriteLine($"Invalid transform: {args[3]}");
				Console.ReadLine();
				return;
			}

            Animation anim = new()
            {
                SourceFile = new FileInfo(file),
                Name = args[1],
                Target = args[2],
                BaseDataKey = baseDataKey,
                BaseDataIndex = baseDataIndex,
                Transform = transform
            };

            anim.Open();

            if (!anim.HasName())
			{
                Console.WriteLine($"Invalid anim name: '{args[1]}'!");
                Console.ReadLine();
                return;
			}

            if (!anim.HasTarget())
            {
                Console.WriteLine($"Invalid target name: '{args[2]}'!\n");
            }

            try
			{
                anim.Modify();
            }
            catch (Exception e)
			{
                Console.WriteLine(e.Message);

#if DEBUG
                Console.WriteLine(e.StackTrace);
#endif

                Console.ReadLine();
            }
        }
    }
}
