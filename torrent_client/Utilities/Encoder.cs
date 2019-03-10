using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace torrent_client.Utilities
{
    public class Encoder
    {
        //Special Characters
        private static byte DictionaryStart = Encoding.UTF8.GetBytes("d")[0]; 
        private static byte ListStart = Encoding.UTF8.GetBytes("l")[0]; 
        private static byte NumberStart = Encoding.UTF8.GetBytes("i")[0]; 
        private static byte StringDivider = Encoding.UTF8.GetBytes(":")[0]; 
        private static byte End = Encoding.UTF8.GetBytes("e")[0]; 

        private object Data;

        public Encoder(object data)
        {
            Data = data;
        }

        public byte[] Encode()
        {
            return EncodeNext(Data);
        }

        public byte[] EncodeNext(object data)
        {
            if (data is byte[])
            {
                return EncodeBytes(data);
            }

            else if (data is string)
            {
                return EncodeString(data);
            }

            else if (data is long)
            {
                return EncodeNumber(data);
            }

            else if (data.GetType() == typeof(List<object>))
            {
                return EncodeList(data);
            }

            else if (data.GetType() == typeof(Dictionary<string, object>))
            {
                return EncodeDictionary(data);
            }

            else
            {
                return new List<byte>().ToArray();
            }
        }

        public byte[] EncodeBytes(object data)
        {
            var bytes = (byte[])data;
            var length = Encoding.UTF8.GetBytes(Convert.ToString(bytes.Length));
            var result = new List<byte>();

            result.AddRange(length);
            result.Add(StringDivider);
            result.AddRange(bytes);

            return result.ToArray();
        }

        public byte[] EncodeString(object data)
        {
            var words = (string)data;
            var bytes = Encoding.UTF8.GetBytes(words);

            return EncodeBytes(bytes);
        }

        public byte[] EncodeNumber(object data)
        {
            var numbers = (long)data;
            var result = new List<byte>();
            var bytes = Encoding.UTF8.GetBytes(numbers.ToString());

            result.Add(NumberStart);
            result.AddRange(bytes);
            result.Add(End);

            return result.ToArray();
        }

        public byte[] EncodeList(object data)
        {
            var list = (List<object>)data;
            var result = new List<byte>();

            result.Add(ListStart);

            foreach (var item in list)
            {
                result.AddRange(EncodeNext(item));
            }

            result.Add(End);

            return result.ToArray();
        }

        public byte[] EncodeDictionary(object data)
        {
            var dict = (Dictionary<string, object>)data;
            var result = new List<byte>();
            var sortedKeys = dict.Keys.ToList().OrderBy(x => BitConverter.ToString(Encoding.UTF8.GetBytes(x)));

            result.Add(DictionaryStart);

            foreach (var key in sortedKeys)
            {
                var encodedKey = EncodeString(key);
                var encodedVal = EncodeNext(dict[key]);

                result.AddRange(encodedKey);
                result.AddRange(encodedVal);
            }

            result.Add(End);

            return result.ToArray();
        }
    }
}
