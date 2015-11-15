using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK_Test_project.Utility
{
    public class Extensions
    {
        public static Vector4 Cross(Vector4 u, Vector4 v)
        {
            return new Vector4(u[1] * v[2] - u[2] * v[1],
                               u[2] * v[0] - u[0] * v[2],
                               u[0] * v[1] - u[1] * v[0],
                               1.0f);
        }
    }
}
