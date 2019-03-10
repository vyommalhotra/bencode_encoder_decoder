using System;
using System.Collections.Generic;
using System.Text;

namespace torrent_client.Utilities
{
    public class Decoder
    {
        //Special Characters
        private static byte DictionaryStart = Encoding.UTF8.GetBytes("d")[0]; 
        private static byte ListStart = Encoding.UTF8.GetBytes("l")[0]; 
        private static byte NumberStart = Encoding.UTF8.GetBytes("i")[0]; 
        private static byte StringDivider = Encoding.UTF8.GetBytes(":")[0]; 
        private static byte End = Encoding.UTF8.GetBytes("e")[0]; 

        private IEnumerator<byte> Data;

        public Decoder(byte[] bytes)
        {
            Data = ((IEnumerable<byte>)bytes).GetEnumerator();
            Data.MoveNext();
        }

        public object Decode()
        {
            if (Data.Current == NumberStart)
            {
                Data.MoveNext();
                return DecodeNumber();
            }

            else if (Data.Current == ListStart)
            {
                Data.MoveNext();
                return DecodeList();
            }

            else if (Data.Current == DictionaryStart)
            {
                Data.MoveNext();
                return DecodeDictionary();
            }

            else
            {
                return DecodeString();
            }
            
        }

        private long DecodeNumber()
        {
            var bytes = new List<byte>();

            while (Data.Current != End)
            {
                bytes.Add(Data.Current);
                Data.MoveNext();
            }

            var result = Int64.Parse(Encoding.UTF8.GetString(bytes.ToArray()));
            return result;
        }

        private List<object> DecodeList()
        {
            var result = new List<object>();

            while (Data.Current != End)
            {
                var obj = Decode();
                result.Add(obj);
            }

            Data.MoveNext();
            return result;
        }

        private Dictionary<object, object> DecodeDictionary()
        {
            var dict = new Dictionary<object, object>();

            while (Data.Current != End)
            {
                var key = Decode();
                var val = Decode();

                dict.Add(key, val);
            }

            Data.MoveNext();
            return dict;
        }

        private string DecodeString()
        {
            //parse length
            var bytes = new List<byte>();

            while (Data.Current != StringDivider)
            {
                bytes.Add(Data.Current);
                Data.MoveNext();
            }

            var length = Int64.Parse(Encoding.UTF8.GetString(bytes.ToArray()));

            //parse string
            Data.MoveNext();
            bytes = new List<byte>();

            for (int i = 0; i < length; i++)
            {
                bytes.Add(Data.Current);
                Data.MoveNext();
            }

            var result = Encoding.UTF8.GetString(bytes.ToArray());
            return result;
        }
    }
}
