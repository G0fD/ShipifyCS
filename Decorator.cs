using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace endproject
{
    internal abstract class Decorator : DBUsers
    {
        protected DBUsers _dbusers;

        public Decorator(DBUsers dbusers)
        {
            this._dbusers = dbusers;
        }

        public override bool IsAdmin()
        {
            if (this._dbusers != null)
            {
                return this._dbusers.IsAdmin();
            }
            else
            {
                return false;
            }
        }
    }
}
