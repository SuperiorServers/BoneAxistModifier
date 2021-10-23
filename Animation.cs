using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoneAxisModifier
{
	public class Animation
	{
		public FileInfo SourceFile { get; set; }

        public string Name;

        public string Target;

        public decimal Transform;

        private JObject Json;

        public void Open()
        {
            Console.WriteLine(SourceFile.FullName);

			using StreamReader reader = SourceFile.OpenText();
			Json = (JObject)JsonConvert.DeserializeObject(reader.ReadToEnd());
		}

        public bool HasName()
		{
            JArray boneAnims = (JArray)Json["BoneAnims"];

            foreach (JObject anim in boneAnims)
                if ((string)anim["Name"] == Name)
                    return true;

            return false;
        }

        public bool HasTarget()
		{
            JArray boneAnims = (JArray)Json["BoneAnims"];

            foreach (JObject anim in boneAnims)
            {
                if ((string)anim["Name"] == Name)
                {
                    JArray curves = (JArray)anim["Curves"];

                    foreach (JObject curve in curves)
                        if ((string)curve["Target"] == Target)
                            return true;
                }    
            }

            return false;
        }


        public bool Modify() // why think harder when you have crtl c, ctrl v
		{
            JArray boneAnims = (JArray)Json["BoneAnims"];

            foreach (JObject anim in boneAnims)
            {
                if ((string)anim["Name"] == Name)
                {
                    JArray curves = (JArray)anim["Curves"];

                    foreach (JObject curve in curves)
					{
                        if ((string)curve["Target"] == Target)
						{
                            foreach (JProperty keyframe in curve["KeyFrames"])
							{
                                if (!decimal.TryParse((string)keyframe.Value["Value"], out decimal value))
                                    return false;

                                keyframe.Value["Value"] = value + Transform;
							}
						}
					}

                    Console.WriteLine(anim["BaseData"]["Rotate"]);
                    string[] rotate = anim["BaseData"]["Rotate"].ToString().Replace(" ", string.Empty).Split(';');

                    if (!decimal.TryParse(rotate[1], out decimal y))
                        return false;

                    anim["BaseData"]["Rotate"] = $"{rotate[0]};{y + Transform};{rotate[2]}";
                }
            }

            File.WriteAllText(SourceFile.FullName, Json.ToString());


            return true;
        }
    }
}
