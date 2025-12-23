using Models.Music;
using Models.Friends;
using Newtonsoft.Json;

namespace PlayGround.Extensions;

public static class ModelExtensions
{
    public static T SerializeJson<T>(this T @this, string jsonFileName) where T : class, new()
    {
        string sJson = JsonConvert.SerializeObject(@this, Formatting.Indented);

        using (Stream s = File.Create(fname(jsonFileName)))
        using (TextWriter writer = new StreamWriter(s))
            writer.Write(sJson);
        
        return @this;
    }

    public static T DeSerializeJson<T>(this T @this, string jsonFileName) where T : class, new()
    {
        using (Stream s = File.OpenRead(fname(jsonFileName)))
        using (TextReader reader = new StreamReader(s))
        {
            var flist = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            return flist ?? new T();
        }
    }

    public static List<T> SerializeJson<T>(this List<T> list, string jsonFileName) where T : class, new()
    {
        string sJson = JsonConvert.SerializeObject(list, Formatting.Indented);

        using (Stream s = File.Create(fname(jsonFileName)))
        using (TextWriter writer = new StreamWriter(s))
            writer.Write(sJson);
        
        return list;
    }

    public static List<T> DeSerializeJson<T>(this List<T> list, string jsonFileName) where T : class, new()
    {
        using (Stream s = File.OpenRead(fname(jsonFileName)))
        using (TextReader reader = new StreamReader(s))
        {
            var flist = JsonConvert.DeserializeObject<List<T>>(reader.ReadToEnd());
            return flist ?? new List<T>();
        }
    }

    static string fname(string name)
    {
        var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        documentPath = Path.Combine(documentPath, "AdvancedProgramming", "Linq");
        if (!Directory.Exists(documentPath)) Directory.CreateDirectory(documentPath);
        return Path.Combine(documentPath, name);
    }
}

