﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;

namespace DevBase.Web.Cookie
{
    public class GlobalCookieContainer
    {
        private static GlobalCookieContainer _instance;

        private GenericTupleList<Object, CookieContainer> _cookies;

        public GlobalCookieContainer()
        {
            this._cookies = new GenericTupleList<Object, CookieContainer>();
        }

        public CookieContainer GetCookieContainer(Object obj)
        {
            CookieContainer container = this._cookies.FindEntry(obj);
            if (container == null)
            {
                CookieContainer cookieContainer = new CookieContainer();
                this._cookies.Add(obj, cookieContainer);
                return cookieContainer;
            }
            else
            {
                return container;
            }
        }

        public static GlobalCookieContainer INSTANCE
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GlobalCookieContainer();
                }

                return _instance;
            }
        }

    }
}
