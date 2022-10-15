using System.Text.Json;

namespace Test
{
    public class Meeting
    {
        public string name { get; set; }
        public DateTimeOffset start { get; set; }
        public DateTimeOffset end { get; set; }
        public uint notificationTime { get; set; }

        public Meeting(string name, DateTimeOffset start, DateTimeOffset end, uint notificationTime)
        {
            this.name = name;
            this.start = start;
            this.end = end;
            this.notificationTime = notificationTime;

            long miliseconds = this.start.ToUnixTimeMilliseconds() - DateTimeOffset.Now.ToUnixTimeMilliseconds() - notificationTime * 60 * 1000;
            if(miliseconds < 0)
            {
                throw new OverflowException();
            }

            new Notifier(
                miliseconds,
                () => Console.WriteLine($"\nВ {this.start.ToString(Program.dateFormat)} произойдет встреча {this.name}.")
            );
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
