using System;
using System.Collections.Generic;
using System.Text;

namespace Allocator
{
    class Program
    {
        unsafe static void Main()
        {
            LinearAllocator Allocator = LinearAllocator.GetInstance();
            int* a = (int*)Allocator.MemAlloc(8);
            *a = 9999;
            int* d = (int*)Allocator.MemAlloc(4);
            *d = 9999;
            a = (int*)Allocator.MemReAlloc(a, 4);
            Allocator.MemFree(a);
            Allocator.MemFree(d);
            Allocator.Dump();
        }
    }
}
