namespace Util
{
    public class PrettyConverter
    {
        public static string ToHexString(ArraySegment<byte> segment)
        {
            byte[] data = segment.ToArray();
            string[] hex = Array.ConvertAll(data, x => x.ToString("X2"));
            return string.Join(" ", hex);
        }
    }
}
