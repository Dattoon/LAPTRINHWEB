using LAPTRINHWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LAPTRINHWEB.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base con di me may
        protected DataClasses1DataContext data = new DataClasses1DataContext("DATTOON\\DATER");
    }
}