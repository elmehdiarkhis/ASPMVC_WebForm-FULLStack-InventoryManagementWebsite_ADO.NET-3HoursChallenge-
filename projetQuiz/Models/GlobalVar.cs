using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projetQuiz.Models
{
    public static class GlobalVar
    {
        public static Int32 userId { get; set; }

        public static bool updateClicked = false;

        public static bool prixClicked = false;
        public static bool idClicked = false;
        public static bool typeClicked = false;

        public static string choosenType { get; set; }

        public static int idToUpdate { get; set; }

    }
}