using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crm.Helpers
{
    public class LinkBuilder
    {

        public static string GetImageLink(string relative_path)
        {
            //TODO временно
            int extPos = relative_path.LastIndexOf(".");
            string filename = relative_path.Substring(0, extPos);
            string ext = relative_path.Substring(extPos + 1);
            return string.Format("{2}{0}.{1}", filename, ext, Helpers.Config.GetAppSettings("FileStorageDomain"));

        }
        public static string GetImageTopCropLink(string relative_path, int width, int height)
        {
            //TODO временно
            //"http://s1.buyreklama.dev.citystar.ru/0t/ce538837a03325dd43e9314a49d4666f-topcrop-50x50.png"
            int extPos = relative_path.LastIndexOf(".");
            string filename = relative_path.Substring(0, extPos);
            string ext = relative_path.Substring(extPos + 1);
            return string.Format("{4}{0}-topcrop-{1}x{2}.{3}", filename, width, height, ext, Helpers.Config.GetAppSettings("FileStorageDomain"));

        }

        public static string EngineCardLink(string key)
        {
            return string.Format("/crm/getenginedata?key={0}&type=card&id=**", key);
        }

        internal static string GetImageHeightLink(string relative_path, int height)
        {
            int extPos = relative_path.LastIndexOf(".");
            string filename = relative_path.Substring(0, extPos);
            string ext = relative_path.Substring(extPos + 1);
            return string.Format("{3}{0}-h-{1}.{2}", filename, height, ext, Helpers.Config.GetAppSettings("FileStorageDomain"));
        }
    }
}