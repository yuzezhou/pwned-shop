using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace pwned_shop.Utils
{
    public class ValidateRegistration
    {
        public bool ValidateEmail(string text)
        {
            bool output = false;
            string pat = @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[\w]{2,}";
            Regex r = new Regex(pat);
            Match m = r.Match(text);
            if (m.Success)
            {
                output = true;
            }
            return output;
        }
        public bool ValidateDOB(DateTime dob)
        {
            bool output = false;
            DateTime minimum = DateTime.Now.AddYears(-100);
            if (((DateTime.Compare(dob, DateTime.Now)) < 0)&&((DateTime.Compare(minimum,dob)) < 0))
            {
                output = true;
            }
            return output;
        }
        public bool ValidatePassword(string password)
        {
            bool output = true;            
            foreach (char x in password)
            { 
                if (Char.IsWhiteSpace(x))
                {
                    output = false;
                }  
            }
            if ((password.Length)<8 )
            {
                output = false;
            }
            return output;
        }
    }
}
