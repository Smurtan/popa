using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace azs.Data
{

    public class Core
    {
        private static FuelTrakEntities _context;

        public static FuelTrakEntities GetContext()
        {
            if (_context == null)
            {
                _context = new FuelTrakEntities();
            }
            return _context;
        }


       
    }


}