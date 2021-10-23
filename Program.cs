using System;
using System.IO;

namespace BoneAxisModifier
{
	class Program
	{
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
                Console.WriteLine($"Invalid target name: '{args[2]}'!");
                Console.ReadLine();
                return;
            }

            if (!anim.Modify())
			{
                Console.WriteLine("Error modifying file. Unexpected or invalid formatting!");
                Console.ReadLine();
            }
        }
    }
}
