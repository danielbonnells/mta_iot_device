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

}
