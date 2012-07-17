﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.Data;
using Hadouken.Data.Models;

namespace Hadouken.Impl.Http.Controllers.Api
{
    public class ConfigController : Controller
    {
        private IDataRepository _data;

        public ConfigController(IDataRepository data)
        {
            _data = data;
        }

        [HttpGet]
        [Route("/api/config")]
        public ActionResult GetConfig()
        {
            if (!String.IsNullOrEmpty(Context.Request.QueryString["key"]))
            {
                string[] keys = Context.Request.QueryString["key"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                var settings = _data.List<Setting>(s => keys.Contains(s.Key));

                return Json(settings.ToDictionary(s => s.Key, s => s.Value));
            }

            return Json(_data.List<Setting>().ToDictionary(x => x.Key, x => x.Value));
        }

        [HttpPost]
        [Route("/api/config")]
        public ActionResult ChangeConfig()
        {
            var dictionary = BindModel<Dictionary<string, string>>();

            foreach (var key in dictionary.Keys)
            {
                string value = dictionary[key];

                Setting setting = _data.Single<Setting>(x => x.Key == key);

                if (setting == null)
                {
                    setting = new Setting() { Key = key, Value = value };
                    _data.Save(setting);
                }
                else
                {
                    setting.Value = value;
                    _data.Update(setting);
                }
            }

            return Json(true);
        }
    }
}