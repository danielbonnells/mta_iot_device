using Google.Protobuf;

namespace ESP32_MTA_Feed;

public class GeneralService
{

public static T ToObject<T>(byte[] buf) where T : IMessage<T>, new()
        {
            if (buf == null)
                return default;

            using (var ms = new MemoryStream(buf))
            {
                MessageParser<T> parser = new MessageParser<T>(() => new T());
                return parser.ParseFrom(ms);
            }
        }

    public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
        return dateTime;
    }

    public static string MtaUri(string routeId){

        ArgumentNullException.ThrowIfNull(routeId);
        routeId = routeId.Trim().ToUpper();
        string routeEndpoint = routeId switch
        {
            "A" or "C" or "E" => "Blue",
            "B" or "D" or "F" or "M" => "Orange",
            "N" or "Q" or "R" or "W" => "Yellow",
            "J" or "Z" => "Brown",
            "1" or "2" or "3" => "Red",
            "4" or "5" or "6" => "Green",
            "7" => "Purple",
            _ => "",
        };

        return routeEndpoint;
    }

    public static List<string> GetRelatedLines(string routeId){

        ArgumentNullException.ThrowIfNull(routeId);
        routeId = routeId.Trim().ToUpper();
        string[] routeEndpoint = routeId switch
        {
            "A" or "C" or "E" => ["A", "C", "E"],
            "B" or "D" or "F" or "M" => ["B", "D", "F"],
            "N" or "Q" or "R" or "W" => ["N", "Q", "R", "W"],
            "J" or "Z" => ["J", "Z"],
            "1" or "2" or "3" => ["1", "2", "3"],
            "4" or "5" or "6" => ["4", "5", "6"],
            "7" => ["7"],
            _ => [""],
        };

        return routeEndpoint.ToList();
    }

}
