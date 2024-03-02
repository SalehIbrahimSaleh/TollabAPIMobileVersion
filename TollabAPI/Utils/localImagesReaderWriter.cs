using System;
using System.IO;
using System.Web;

namespace TollabAPI.Utils
{
    public sealed class LocalImagesReaderWriter
    {
        private static LocalImagesReaderWriter instance = null;
        private static readonly object padLock = new object();


        LocalImagesReaderWriter()
        {

        }
        /// <summary>
        /// ///////////////factory method////////////////////////
        /// </summary>
        public static LocalImagesReaderWriter Instance
        {

            get
            {
                lock (padLock)
                {
                    if (instance == null)
                    {
                        instance = new LocalImagesReaderWriter();
                    }
                    return instance;
                }
            }

        }




        public bool saveImage(HttpPostedFile image, string filenName)
        {

            try
            {
                var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/\ProfileImages\\" + filenName + ".jpg");
                image.SaveAs(filePath);
                return true;

            }
            catch (Exception e)
            { return false; }

        }

        public bool saveImage(string avatar, string filenName)
        {

            try
            {

                var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/\ProfileImages\\" + filenName + ".jpg");
                byte[] imageBytes = Convert.FromBase64String(avatar);

                File.WriteAllBytes(filePath, imageBytes);
                return true;
            }
            catch (Exception e)
            {
                throw e;
                return false;

            }
        }


        public bool saveCarImage(string avatar, string filenName)
        {

            try
            {

                var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/\CarImages\\" + filenName + ".jpg");
                byte[] imageBytes = Convert.FromBase64String(avatar);

                File.WriteAllBytes(filePath, imageBytes);
                return true;
            }
            catch (Exception e)
            {
                throw e;

            }
        }

    }
}