using Models.Friends;
using Models.Music;
using PlayGround.Extensions;
using Seido.Utilities.SeedGenerator;

namespace Playground.Lesson03;

public static class HomeExercise03Init
{
    public static (List<Friend> friends, Friend friendNoAddress, 
                    Friend friendNoPets, Friend friendNoQuotes, 
                    Friend friendHasMany) GetData(bool generateNewData)
    {
        if (generateNewData)
        {
            //Generate Seeded Data and Serialize to Json
            var seededFriends = new SeedGenerator().ItemsToList<Friend>(100);
            seededFriends.SerializeJson("friends.json");
            seededFriends.First(f => f.Address == null).SerializeJson("friendNoAddress.json");
            seededFriends.First(f => f.Pets.Count == 0).SerializeJson("friendNoPets.json");
            seededFriends.First(f => f.Quotes.Count == 0).SerializeJson("friendNoQuotes.json");
            seededFriends.First(f => f.Address != null && f.Pets.Count > 1 && f.Quotes.Count > 1).SerializeJson("friendHasMany.json");
        }

        //Deserialize Data from Json
        return (
            new List<Friend>().DeSerializeJson("friends.json"),
            new Friend().DeSerializeJson("friendNoAddress.json"),
            new Friend().DeSerializeJson("friendNoPets.json"),
            new Friend().DeSerializeJson("friendNoQuotes.json"),
            new Friend().DeSerializeJson("friendHasMany.json") 
        );
    }
}