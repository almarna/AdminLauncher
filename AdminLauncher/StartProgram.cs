using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdminLauncher
{
    public class StartProgram
    {
        public string Name { get; set; }
        public string Program { get; set; }
        public string WorkingDirectory { get; set; }
        public string Arguments { get; set; }

        private const string path = "StartPrograms.json";

        public static List<StartProgram> Read()
        {
            List<StartProgram> result;
            JsonSerializer serializer = new JsonSerializer();

            using (StreamReader file = File.OpenText(path))
            {
                using (JsonReader reader = new JsonTextReader(file))
                {
                    result = serializer.Deserialize<List<StartProgram>>(reader);
                }
            }
            return result;
        }

        public static void Write(List<StartProgram> startPrograms)
        {
            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            serializer.Converters.Add(new JavaScriptDateTimeConverter());


            using (StreamWriter sw = new StreamWriter(path))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, startPrograms);
                }
            }
        }

    }
}