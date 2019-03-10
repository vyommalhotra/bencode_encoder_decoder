using System;
using System.Collections.Generic;
using System.Text;
using Decoder = torrent_client.Utilities.Decoder;
using Encoder = torrent_client.Utilities.Encoder;


namespace torrent_client
{
    class Program
    {
        static void Main(string[] args)
        {
            TestDecoder();
            TestEncoder();
        }

        static void TestEncoder()
        {
            var dict = new Dictionary<string, object>();
            dict["cow"] = "moo";
            dict["spam"] = "eggs";
            var bytes = new Encoder(dict).Encode();
            var result = new Decoder(bytes).Decode();
        }

        static void TestDecoder()
        {
            var bytes = Encoding.ASCII.GetBytes("d3:cow3:moo4:spam4:eggse");
            var dict = new Decoder(bytes).Decode();
        }
    }
}
