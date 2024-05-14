using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAppDP.Models
{
    //Model ini berfungsi untuk memetakan url kedalam bentuk seperti ini, /Product/Details/1
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}