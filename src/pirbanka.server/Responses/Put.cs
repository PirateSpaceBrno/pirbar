using JamesWright.SimpleHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PirBanka.Server.Responses
{
    public class Put
    {
        public static KeyValuePair<Regex, Action<Request, Response>> Action(string endpoint)
        {
            var action = Actions.FirstOrDefault(x => x.Key == endpoint);

            if (!action.Equals(new KeyValuePair<string, Action<Request, Response>>()))
            {
                return new KeyValuePair<Regex, Action<Request, Response>>(new Regex(action.Key), action.Value);
            }

            return new KeyValuePair<Regex, Action<Request, Response>>();
        }

        private static Dictionary<string, Action<Request, Response>> Actions = new Dictionary<string, Action<Request, Response>>()
        {
            //{
            //    @"",
            //    new Action<Request, Response>( async (req, res) =>
            //    {

            //    }
            //)}
        };
    }
}
