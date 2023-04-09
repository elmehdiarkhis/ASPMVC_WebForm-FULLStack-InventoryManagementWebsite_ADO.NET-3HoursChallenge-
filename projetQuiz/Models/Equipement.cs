using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projetQuiz.Models
{
    public class Equipement
    {
        public int EquipementId { get; set; }
        public int numSerie { get; set; }
        public string nom { get; set; }
        public string type { get; set; }
        public int prix { get; set; }
        public string description { get; set; }
    }
}