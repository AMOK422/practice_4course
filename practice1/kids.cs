using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practice1
{
    public class kids
    {
        public int id {  get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string patronymic { get; set; }

        public int group {  get; set; }
        public int id_father { get; set; }
        public int id_mother { get; set; }

        //public string type { get; set;}
    }
}
