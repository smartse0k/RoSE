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

        public static byte[]? HexToByteArray(string hexString)
        {
            string pureHexString = hexString.Replace(" ", "");

            if (pureHexString.Length == 0 || pureHexString.Length % 2 == 1)
            {
                return null;
            }

            byte[] result = new byte[pureHexString.Length / 2];

            for (int i = 0; i < pureHexString.Length; i += 2)
            {
                result[i / 2] = Convert.ToByte(pureHexString.Substring(i, 2), 16);
            }

            return result;
        }
    }
}
