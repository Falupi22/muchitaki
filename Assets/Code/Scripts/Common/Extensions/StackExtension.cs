using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Code.Scripts.Common.Extensions
{
    public static class StackExtension
    {
        /// <summary>
        /// Spills the content of the other stack to the current, emptying the other
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stack"></param>
        /// <param name="other"></param>
        public static void Spill<T>(this Stack<T> stack, Stack<T> other)
        {
            while (other.Count > 0) 
            { 
                stack.Push(other.Pop());
            }
        }
    }
}
