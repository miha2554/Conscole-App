using System.Text.Json;

namespace Test
{
    public static class MeetingsList
    {
        public static List<Meeting> meetings { get; private set; } = new List<Meeting>();

        public static void AddToList(Meeting meeting)
        {
            foreach (Meeting oldMeeting in meetings)
            {
                if (
                    (meeting.start.ToUnixTimeSeconds() >= oldMeeting.start.ToUnixTimeSeconds()
                     && meeting.start.ToUnixTimeSeconds() <= oldMeeting.end.ToUnixTimeSeconds())
                    ||
                    (meeting.start.ToUnixTimeSeconds() < oldMeeting.start.ToUnixTimeSeconds()
                    && meeting.end.ToUnixTimeSeconds() >= oldMeeting.start.ToUnixTimeSeconds())
                )
                {
                    throw new ArgumentException(
                        "Данная встреча пересекается с другой встречей",
                        nameof(meeting)
                    );
                }
            }

            meetings.Add(meeting);
        }

        public static void ExportToFile(string filePath, DateTimeOffset exportDate)
        {
            List<Meeting> day_meetings = new List<Meeting>();
            for (int i = 0; i < meetings.Count; i++)
            {
                if (meetings[i].start.Date <= exportDate.Date &&  exportDate.Date <= meetings[i].end.Date)
                {
                    day_meetings.Add(meetings[i]);
                }
            }
            string jsonString = JsonSerializer.Serialize(day_meetings);
            File.WriteAllText(filePath, jsonString);
        }
        public static void RemoveByIndex(int index)
        {
            meetings.RemoveAt(index);
        }
    }
}
