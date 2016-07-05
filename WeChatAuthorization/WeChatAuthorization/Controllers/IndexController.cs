using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WeChatAuthorization.Controllers
{
    public class IndexController : Controller
    {
        public ActionResult Index()
        {
            string weChatAppId = ConfigurationManager.AppSettings["WeiXinAppId"];
            string redictUrl = ConfigurationManager.AppSettings["RedirectUrl"];

            if (string.IsNullOrEmpty(weChatAppId))
            {
                return Content("出错了，尚未配置微信相关的API信息！");
            }

            string state = Guid.NewGuid().ToString().Replace("-", "");
            Session["state"] = state;

            string sendUrl = "https://open.weixin.qq.com/connect/qrconnect?appid=" + weChatAppId + "&redirect_uri=" + HttpUtility.UrlEncode(redictUrl) + "&response_type=code&scope=snsapi_login&state=" + state + "#wechat_redirect";

            return Redirect(sendUrl);
        }

        public ActionResult CallBack(string state, string code)
        {
            return Content(state, code);
        }
    }
}