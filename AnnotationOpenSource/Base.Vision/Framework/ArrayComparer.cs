using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Vision.Framework
{
    public class PointArrayComparer : IComparer
    {
        // IComparer implementation
        public int Compare(object obj1, object obj2)
        {
            int result;

            Point[] m_Temp1 = obj1 as Point[];
            Point[] m_Temp2 = obj2 as Point[];

            if (m_Temp1 != null && m_Temp2 != null)
            {
                if (m_Temp1.Length < m_Temp2.Length)
                {
                    obj2 = m_Temp1;
                    obj1 = m_Temp2;
                    result = 1;
                }
                else if (m_Temp1.Length > m_Temp2.Length)
                    result = -1;
                else
                    result = 0;

                return result;
            }
            else
            {
                throw new ArgumentException("Objects are not arrays of doubles!");
            }
        }
    }
}
