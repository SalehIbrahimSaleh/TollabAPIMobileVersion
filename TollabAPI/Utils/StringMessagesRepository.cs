using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TollabAPI.Utils
{
    public sealed class StringMessagesRepository
    {
        private static StringMessagesRepository instance = null;
        private static readonly object padLock = new object();
        private static JObject allMessages = new JObject();
        private static bool allMessagesinitialization = false;

        StringMessagesRepository()
        {

        }
        /// <summary>
        /// ///////////////factory method////////////////////////
        /// </summary>
        public static StringMessagesRepository Instance
        {

            get
            {
                lock (padLock)
                {
                    if (instance == null)
                    {
                        instance = new StringMessagesRepository();
                    }
                    return instance;
                }
            }

        }
        public async Task initAllMessages(string language)
        {
            /// test if the json object(all messages are initialized) get out and dont read again

            if (!allMessagesinitialization)
            {

                language = (language == null || language.ToLower().Contains("en")) ? "" : "AR";
                allMessages = await getJsonFromFile(language);


                //////////////read default json (en) and union them as a complement if the (AR file for example not containing it)
                ////////////  and combine them if the requested language not english


                if (!language.ToLower().Equals(""))
                {
                    JObject DefaultMessages = await getJsonFromFile("");
                    await compineAllMessages(DefaultMessages);

                }



            }

        }
        private async Task compineAllMessages(JObject defaultMessages)
        {
            await Task.Run(() =>
            {

                foreach (var jsonKValuePairs in defaultMessages)
                {
                    try
                    {
                        if (allMessages[jsonKValuePairs.Key] == null)
                            allMessages.Add(jsonKValuePairs.Key, jsonKValuePairs.Value);
                    }
                    catch (Exception e)
                    { }
                }

            });
        }
        private async Task<JObject> getJsonFromFile(string language)
        {

            string PATH = System.Web.Hosting.HostingEnvironment.MapPath(@"~/\UtilsResponseMesseges\\assets\\strings" + language.ToUpper() + ".json");
            using (var reader = File.OpenText(PATH)) //E:\Xapps\Projects\DrOnTimeGit\DrOnTimeWebAPI\DataAccess\Utils\assets
            {
                return JObject.Parse(await reader.ReadToEndAsync());
            }
        }
        public string tryGetStringValues(string Key)
        {
            try
            {
                return allMessages.GetValue(Key).ToString();
            }
            catch { return ""; }
        }

    }

}