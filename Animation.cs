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

        public string BaseDataKey;

        public int BaseDataIndex;

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

        public void Modify() // why think harder when you have crtl c, ctrl v
		{
            JArray boneAnims = (JArray)Json["BoneAnims"];

            foreach (JObject anim in boneAnims)
            {
                if ((string)anim["Name"] == Name && anim["Curves"] != null)
                {
                    JArray curves = (JArray)anim["Curves"];

                    foreach (JObject curve in curves)
                    {
                        if ((string)curve["Target"] == Target)
                        {
                            // Offset
                            if (curve["Offset"] != null)
                            {
                                if (!decimal.TryParse(curve["Offset"].ToString(), out decimal offset))
                                    throw new Exception($"Could not parse Offset!");

                                curve["Offset"] = offset + Transform;
                            }

                            if (curve["KeyFrames"] != null)
                            {
                                foreach (JProperty keyframe in curve["KeyFrames"])
                                {
                                    if (!decimal.TryParse((string)keyframe.Value["Value"], out decimal value))
                                        throw new Exception($"Could not parse KeyFrames!");

                                    keyframe.Value["Value"] = value + Transform;
                                }
                            }
                        }
                    }

                    if (anim["BaseData"] == null || anim["BaseData"][BaseDataKey] == null)
                        throw new Exception($"BaseData missing or BaseData->'{BaseDataKey}' missing!");

                    List<decimal> baseData = new() { };

                    foreach (string baseNum in anim["BaseData"][BaseDataKey].ToString().Replace(" ", string.Empty).Split(';'))
					{
                        if (!decimal.TryParse(baseNum, out decimal parsedNum))
                            throw new Exception($"Could not parse BaseData->'{BaseDataKey}'!");
         
                        baseData.Add(parsedNum);
                    }

                    baseData[BaseDataIndex] += Transform;

                    anim["BaseData"][BaseDataKey] = $"{baseData[0]};{baseData[1]};{baseData[2]}";
                }
            }

            File.WriteAllText(SourceFile.FullName, Json.ToString());
        }
    }
}
